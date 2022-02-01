using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    [Authorize]
    public class CheckOutController : Controller
    {

        ShoppingStoreEntities storeDB = new ShoppingStoreEntities();
        const string PromoCode = "50";
        /*metoda get*/
        /*metoda adres and payment lejon userin te fusi informacion*/
        public ActionResult AddressAndPayment()
        {
            return View();
        }
        /*metoda post do procesoje orderin*//*Lorena*/
        [HttpPost]
        public ActionResult AddressAndPayment(FormCollection values)
        {
            var order = new Order();
            TryUpdateModel(order);

            try
            {
                if (string.Equals(values["PromoCode"], PromoCode,
                    StringComparison.OrdinalIgnoreCase) == false)
                {
                    return View(order);
                }
                else
                {
                    /*marrim orderin dhe e ruajme */
                    order.Username = User.Identity.Name;
                    order.OrderDate = DateTime.Now;


                    storeDB.Orders.Add(order);
                    storeDB.SaveChanges();

                    /*e prrocesojme orderin e marre */
                    var cart = ShoppingCart.GetCart(this.HttpContext);
                    cart.CreateOrder(order);
                    /*dhe pastaj e ridrejtojme te metoda complete me poshte*/
                    return RedirectToAction("Complete",
                        new { id = order.OrderId });
                }
            }
            catch
            {

                return View(order);
            }
        }

        /*metoda complete i cila do shfaqet pasi useri ka kryer ne menyre te sukseshme orderin*//*klara*/
        public ActionResult Complete(int? id)
        {

            bool isValid = storeDB.Orders.Any(
                o => o.OrderId == id &&
                o.Username == User.Identity.Name);

            if (isValid)
            {
                /*dhe kthen nje view e cila shfaq id e orderit si confirmin*/
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
    }
}
