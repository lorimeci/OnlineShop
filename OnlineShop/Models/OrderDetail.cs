using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        /*lidhja midis modeleve*/
        /*modeli orderdetail eshte lidhur me modelin item dhe modelin order*/
        public virtual Item Item { get; set; }
        public virtual Order Order { get; set; }
    }
}