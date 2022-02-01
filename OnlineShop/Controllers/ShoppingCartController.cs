using OnlineShop.Models;
using OnlineShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    /*shoppingcartcontroller ka tre qellime*/
    /*1.shton itemsat ne shporte*/
    /*2.heq itemsat nga shporta jone*/
    /*3.shfaqe itemsat ne shporten tone*/
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart

            ShoppingStoreEntities storeDB = new ShoppingStoreEntities();

            public ActionResult Index()
            {
                var cart = ShoppingCart.GetCart(this.HttpContext);


                var viewModel = new ShoppingCartViewModel
                {
                    CartItems = cart.GetCartItems(),
                    CartTotal = cart.GetTotal()
                };

                return View(viewModel);
            }
        /*merr id e itemsave si parameter*//*lorena*/
            public ActionResult AddToCart(int id)
            {
            /* i shton ne databaze*/
                var addedItem = storeDB.Items
                    .Single(item => item.ItemId == id);


                var cart = ShoppingCart.GetCart(this.HttpContext);

                cart.AddToCart(addedItem);

            /*pastaj i ridjeton per te pare itemsa tee tjere*/
                return RedirectToAction("Index");
            }
        /*motoda e hqejes se itemsit,merr parameter id item*/
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {

            var cart = ShoppingCart.GetCart(this.HttpContext);

            /*merr emrin e itemsit nga databaza*/
            string itemName = storeDB.Carts.Single(item => item.RecordId == id).Item.Title;

            /*e heq nga databaza*/
            int itemCount = cart.RemoveFromCart(id);
            /*pastaj shfaq kete mesazh te useri per te konfimuar me userin heqjen e itemsit nga shporta*/

            var results = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(itemName) +
                    " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }

    }
}
