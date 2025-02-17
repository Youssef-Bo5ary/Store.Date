﻿using Store.Date.Entities.OrderEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Specifications.OrderSpecs
{
    public class OrderWithPaymentIntentSpecification : BaseSpecification<Order>
    {
        public OrderWithPaymentIntentSpecification(string? paymentIntentId) 
            : base(order=>order.PaymentIntentId==paymentIntentId)
        {
        }
    }
}
