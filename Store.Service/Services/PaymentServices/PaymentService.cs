using AutoMapper;
using Microsoft.Extensions.Configuration;
using Store.Date.Entities;
using Store.Date.Entities.OrderEntity;
using Store.Repository.Interfaces;
using Store.Repository.Specifications.OrderSpecs;
using Store.Service.Services.BasketServices;
using Store.Service.Services.BasketServices.Dtos;
using Store.Service.Services.OrderServices.Dtos;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = Store.Date.Entities.Product;

namespace Store.Service.Services.PaymentServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfwork _unitOfwork;
        private readonly IBasketService _basket;
        private readonly IMapper _mapper;

        public PaymentService(IConfiguration configuration,IUnitOfwork unitOfwork,IBasketService basket,IMapper mapper)
        {
            _configuration = configuration;
            _unitOfwork = unitOfwork;
            _basket = basket;
            _mapper = mapper;
        }
        public async Task<CustomerBasketDto> CreateOrUpdatePayment(CustomerBasketDto basket)
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:Secretkey"];

            if(basket is null)
                throw new Exception("Basket Is Empty");
            var deliverMethod = await _unitOfwork.Repository<DeliveryMethod, int>().GetByIdAsync(basket.DeliveryMethodId.Value);
            if (deliverMethod is null)
                throw new Exception("Delivery Method Isn't provided");

            decimal shippingPrice = deliverMethod.Price;
            foreach(var item in basket.BasketItems)
            {
                var product = await _unitOfwork.Repository<Product, int>().GetByIdAsync(item.ProductId);

                if (item.Price != product.Price)
                    item.Price=product.Price;

            }
            var service = new PaymentIntentService();

            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                paymentIntent = await service.CreateAsync(options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret=paymentIntent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100),

                };
                await service.UpdateAsync(basket.PaymentIntentId,options);
            }

            await _basket.UpdateBasketAsync(basket);
            return basket;
        }

        public async Task<OrderDetailsdto> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var specs = new OrderWithPaymentIntentSpecification(paymentIntentId);
            var order = await _unitOfwork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);
            if (order is null)
                throw new Exception("Order Doen't Exist");
            order.OrderPaymentStatus = OrderPaymentStatus.Failed;
            _unitOfwork.Repository<Order, Guid>().Update(order);
            await _unitOfwork.CompleteAsync();
            var mappedOrder = _mapper.Map<OrderDetailsdto>(order);
            return mappedOrder;
        }

        public async Task<OrderDetailsdto> UppdateOrderPaymentFailed(string paymentIntentId)
        {
            var specs = new OrderWithPaymentIntentSpecification(paymentIntentId);
            var order = await _unitOfwork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);
            if (order is null)
                throw new Exception("Order Doen't Exist");
            order.OrderPaymentStatus = OrderPaymentStatus.Recived;
            _unitOfwork.Repository<Order, Guid>().Update(order);
            await _unitOfwork.CompleteAsync();
            await _basket.DeleteBasketAsync(order.BasketId);
            var mappedOrder = _mapper.Map<OrderDetailsdto>(order);
            return mappedOrder;
        }
    }
}
