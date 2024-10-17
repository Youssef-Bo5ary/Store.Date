using AutoMapper;
using StackExchange.Redis;
using Store.Date.Entities;
using Store.Date.Entities.OrderEntity;
using Store.Repository.Interfaces;
using Store.Repository.Specifications.OrderSpecs;
using Store.Service.Services.BasketServices;
using Store.Service.Services.OrderServices.Dtos;
using Store.Service.Services.PaymentServices;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order = Store.Date.Entities.OrderEntity.Order;
using Product = Store.Date.Entities.Product;

namespace Store.Service.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IBasketService _basketService;
        private readonly IUnitOfwork _unitOfwork;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketService basketService,IUnitOfwork unitOfwork,IMapper mapper,IPaymentService paymentService)
        {
            _basketService = basketService;
            _unitOfwork = unitOfwork;
            _mapper = mapper;
            _paymentService = paymentService;
        }
        public async Task<OrderDetailsdto> CreateOrderAsync(OrderDto input)
        {

            #region GetBasket
            var basket = await _basketService.GetBasketAsync(input.BasketId);
            if (basket is null)
                throw new Exception("Basket Dosn't Exist");
            #endregion

            #region fill Order Item List With Items in the basket
            var orderItems = new List<OrderItemDto>();
            foreach(var basketItem in basket.BasketItems)
            {
                var productItem =await _unitOfwork.Repository<Product,int>().GetByIdAsync(basketItem.ProductId);
                
                if(productItem is null)
                    throw new Exception($"Product With Id : {basketItem.ProductId} Dosn't Exist");
                var itemOrdered = new ProductItem
                {
                    ProductId = productItem.Id,
                    ProductName = productItem.Name,
                    PictureUrl = productItem.PictureUrl
                };
                var orderItem = new OrderItem
                {
                    Price= productItem.Price,
                    Quantity=basketItem.Quantity,
                    ProductItem=itemOrdered
                };

                var mapOrderItem=_mapper.Map<OrderItemDto>(orderItem);
                orderItems.Add(mapOrderItem);
            }
            #endregion

            #region GetDelivery Method
            var deliveryMethod = await _unitOfwork.Repository<DeliveryMethod, int>().GetByIdAsync(input.DeliveryMethodId);
            if (deliveryMethod is null)
                throw new Exception("Delivery Method Isn't provided");
            #endregion

            #region Calculate SubTotal
            var subTotal = orderItems.Sum(item => item.Quantity * item.Price);
            #endregion

            #region To Do => Payment

            var specs = new OrderWithPaymentIntentSpecification(basket.PaymentIntentId);
            var existingOrder = await _unitOfwork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);

            if(existingOrder is null)
                await _paymentService.CreateOrUpdatePayment(basket);
            

            #endregion

            #region Create Order

            var mappedShippingAdress = _mapper.Map<ShippingAdress>(input.ShippingAdress);
            var mappedOrderItems = _mapper.Map<List<OrderItem>>(orderItems);
            var order = new Date.Entities.OrderEntity.Order
            {
                DeliveryMethodId = deliveryMethod.Id,
                ShippingAdress = mappedShippingAdress,
                BuyerEmail = input.BuyerEmail,
                BasketId = input.BasketId,
                OrderItems = mappedOrderItems,
                SubTotal = subTotal,
                PaymentIntentId=basket.PaymentIntentId
            };
            await _unitOfwork.Repository<Date.Entities.OrderEntity.Order,Guid>().AddAsync(order);

            await _unitOfwork.CompleteAsync();
            
            var mappedOrder=_mapper.Map<OrderDetailsdto>(order);
            return mappedOrder;

            #endregion

        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetAllDeliverMethodsAsync()
        => await _unitOfwork.Repository<DeliveryMethod, int>().GetAllAsync();

        public async Task<IReadOnlyList<OrderDetailsdto>> GetAllOrdersfromUserAsync(string buyerEmail)
        {
            var spec = new OrderWithItemSpecification(buyerEmail);
            var orders = await _unitOfwork.Repository<Date.Entities.OrderEntity.Order, Guid>().GetAllWithSpecificationAsync(spec);
            if (!orders.Any())
                throw new Exception("You Don't Have Any Orders");
            var mappedOrders=_mapper.Map<List<OrderDetailsdto>>(orders);
            return mappedOrders;
        }

        public async Task<OrderDetailsdto> GetorderIdAsync(Guid id)
        {
            var spec = new OrderWithItemSpecification(id);
            var order = await _unitOfwork.Repository<Date.Entities.OrderEntity.Order, Guid>().GetWithSpecificationByIdAsync(spec);
            if (order is null)
                throw new Exception($"There is no order with this id {id}");
            var mappedOrder = _mapper.Map<OrderDetailsdto>(order);
            return mappedOrder;
        }
    }
}
