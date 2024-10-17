using Store.Service.Services.BasketServices.Dtos;
using Store.Service.Services.OrderServices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.PaymentServices
{
    public interface IPaymentService
    {
        Task<CustomerBasketDto>CreateOrUpdatePayment(CustomerBasketDto input);
        Task<OrderDetailsdto>UpdateOrderPaymentSucceeded(string paymentIntentId);
        Task<OrderDetailsdto> UppdateOrderPaymentFailed(string paymentIntentId);
    }
}
