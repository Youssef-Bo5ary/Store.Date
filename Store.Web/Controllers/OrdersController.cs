using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Date.Entities;
using Store.Service.HandelResponses;
using Store.Service.Services.OrderServices;
using Store.Service.Services.OrderServices.Dtos;
using System.Security.Claims;

namespace Store.Web.Controllers
{
    
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        public async Task<ActionResult<OrderDetailsdto>> CreateOrderAsync(OrderDto input)
        {
            var order = await _orderService.CreateOrderAsync(input);
            if (order is null)
                return BadRequest(new Response(400, "Error While Creating Your order"));
            return Ok(order);
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDetailsdto>>> GetAllOrdersfromUserAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var orders = await _orderService.GetAllOrdersfromUserAsync(email);

            return Ok(orders);
        }
        [HttpGet]
        public async Task<ActionResult<OrderDetailsdto>> GetorderIdAsync(Guid id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var orders = await _orderService.GetorderIdAsync(id);

            return Ok(orders);
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetAllDeliverMethodsAsync()
        => Ok(await _orderService.GetAllDeliverMethodsAsync());

    }
}
