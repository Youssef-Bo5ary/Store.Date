using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.HandelResponses
{
    public class CustomException : Response
    {
        public CustomException(int StatusCode, string? Message = null,string? details=null)
            : base(StatusCode, Message)
        {
            Details = details;
        }
        public string? Details { get; set; }
    }
}
