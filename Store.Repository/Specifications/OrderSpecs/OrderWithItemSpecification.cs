using Store.Date.Entities.OrderEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Specifications.OrderSpecs
{
    public class OrderWithItemSpecification : BaseSpecification<Order>
    {
        public OrderWithItemSpecification(string buyerEmail) : base(order=>order.BuyerEmail==buyerEmail)
        {
            AddInclud(order => order.DeliveryMethod);
            AddInclud(order => order.OrderItems);
            AddorderByDesc(order => order.OrderDate);
        }
        public OrderWithItemSpecification(Guid id) : base(order => order.Id == id)
        {
            AddInclud(order => order.DeliveryMethod);
            AddInclud(order => order.OrderItems);
        }
    }
}
