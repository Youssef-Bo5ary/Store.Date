using Store.Date.Entities;
using Store.Service.Services.OrderServices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.OrderServices
{
    public interface IOrderService
    {
        Task<OrderDetailsdto> CreateOrderAsync(OrderDto input);
        Task<IReadOnlyList<OrderDetailsdto>> GetAllOrdersfromUserAsync(string buyerEmail);
        Task<OrderDetailsdto> GetorderIdAsync(Guid id);
        Task<IReadOnlyList<DeliveryMethod>>GetAllDeliverMethodsAsync();
    }
}
