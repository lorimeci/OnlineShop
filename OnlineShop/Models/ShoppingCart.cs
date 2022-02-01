using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Models
{
    /*mban llogjiken e biznesit per shtimin dhe heqjen e produkteve*/
    public class ShoppingCart
    {
        ShoppingStoreEntities storeDB = new ShoppingStoreEntities();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }
        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }
        /*shton item te zgjedhur nga perdoruesi ne shporte*/ /*lori*/
        public void AddToCart(Item item)
        {

            var cartItem = storeDB.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.ItemId == item.ItemId);
            /*nqs ky item nuk ekziston gia ne shporte e shtojme kte item*/
            if (cartItem == null)
            {

                cartItem = new Cart
                {
                    ItemId = item.ItemId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                storeDB.Carts.Add(cartItem);
            }
            /*nqs ekziston thjesht e shtojme me 1 shporten*/
            else
            {

                cartItem.Count++;
            }

            storeDB.SaveChanges();
        }
        /*klara*/
        public int RemoveFromCart(int id)
        {

            var cartItem = storeDB.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.RecordId == id);
            /*kontrollon x item count*/
            int itemCount = 0;
            /*nqs eshte me shume se 1,do e zbrese do e heqe nga shporta*/
            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                /*ndryshe thjesht do e heqi */
                {
                    storeDB.Carts.Remove(cartItem);
                }

                storeDB.SaveChanges();
            }
            return itemCount;
        }
        /*lorena*/
        public void EmptyCart()
        {
            var cartItems = storeDB.Carts.Where(
                cart => cart.CartId == ShoppingCartId);
/*i ben loop itemst tone ne shporte*/
/*dhe i fshin*/
            foreach (var cartItem in cartItems)
            {
                storeDB.Carts.Remove(cartItem);
            }

            storeDB.SaveChanges();
        }


        /*klara*/
        /*per te mar itemsat ne shporte*/
        public List<Cart> GetCartItems()
        {
            return storeDB.Carts.Where(
                cart => cart.CartId == ShoppingCartId).ToList();
        }
        /*kemi nji selekt query nga databasa per te numru te gjith itemsat qe kemi ne shporten tone*/
        /*per te shfaq te useri itemsat ne shporte*//*lori*/
        public int GetCount()
        {

            int? count = (from cartItems in storeDB.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
/*e kemi len me pikepresje ,sps ne rastet qe s ka itemsat te lejoje dhe vlera null*/
            return count ?? 0;
        }
        /*metoda e get total perdoret per te specifiku per do item  athere therret per cdo tem cmimin perkates*/
        public decimal GetTotal()
        {

            decimal? total = (from cartItems in storeDB.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count *
                              cartItems.Item.Price).Sum();

            return total ?? decimal.Zero;
        }
        /*metoda create order merr orderin si parameter*/
        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();
            /*kontrllon itemsat ne shporte*/
            /*merr detajet e tyre dhe pastaj shton cmimin per selin prej tyre*//*lorena*/
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ItemId = item.ItemId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Item.Price,
                    Quantity = item.Count
                };

                orderTotal += (item.Count * item.Item.Price);
                /*dhe pastaj i ruan ne databaze*/

                storeDB.OrderDetails.Add(orderDetail);

            }

            order.Total = orderTotal;


            storeDB.SaveChanges();
            /*pastaj empty shporten*/

            EmptyCart();

            return order.OrderId;
        }

        public string GetCartId(HttpContextBase context)/*klara*/
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] =
                        context.User.Identity.Name;
                }
                else
                {

                    Guid tempCartId = Guid.NewGuid();

                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }
        /*shikon i ben loop te gjith itemsave ne shopping cart ,qe ne baz te emailit qe ka ti ruaje ndryshimet*/
        public void MigrateCart(string Email)
        {
            var shoppingCart = storeDB.Carts.Where(
                c => c.CartId == ShoppingCartId);

            foreach (Cart item in shoppingCart)
            {
                item.CartId = Email;
            }
            storeDB.SaveChanges();
            EmptyCart();
        }
    }
}