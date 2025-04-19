using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineMusicStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Change to string to match Identity
        public string CardHolder { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public virtual List<OrderItem> Items { get; set; }
    }


}