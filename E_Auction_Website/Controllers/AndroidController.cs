using E_Auction_Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Auction_Website.Controllers
{
    public class AndroidController : Controller
    {
        public const String USER = "user";

        AuctionDBEntities db = new AuctionDBEntities();


        [HttpPost]
        public ActionResult Login(String Login, String Password)
        {
            var user = db.Users.Where(x => x.Login.Equals(Login) && x.Password.Equals(Password)).SingleOrDefault() ;

            if (user != null)
            {
                user = new User(user);
                return Json(user, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = false },JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Signup(String Username, long Tel, String Login, String Password)
        {
            var user = new User(Username, Password, Login, Tel);

            db.Users.Add(user);

            db.SaveChanges();

            return Json(new { Message = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddProduct(String ProductName, int UserID, decimal InitialPrice)
        {
            var product = new Product(UserID, ProductName, InitialPrice, false);

            db.Products.Add(product);

            db.SaveChanges();

            return Json(new { Message = true });
        }

        public ActionResult MyProducts(int UserID)
        {
            var products = db.Products.Where(x => x.UserID == UserID).ToList().Select(x=>new Product(x)).ToList();

            return Json(products,JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyProductDetails(int ProductID)
        {
            var p = db.Products.Find(ProductID);
            var product = new Product(p, p.BidOffers.ToList());

            return Json(product,JsonRequestBehavior.AllowGet);
        }

        public ActionResult SoldProduct(int ProductID)
        {
            var product = db.Products.Find(ProductID);
            product.IsSold = true;
            db.SaveChanges();

            return Json(new { Message = true },JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult OthersProducts(int UserID)
        {
            var products = db.Products.Where(x => x.UserID != UserID && !x.IsSold).ToList().Select(x=>new Product(x)).ToList();

            return Json(products,JsonRequestBehavior.AllowGet);
        }

        public ActionResult OtherProductDetails(int ProductID)
        {
            var product = db.Products.Find(ProductID);

            decimal maxBid = product.InitialPrice;
            if (product.BidOffers.Count > 0)
                maxBid = product.BidOffers.Max(x => x.OfferedPrice);

            product = new Product(product, maxBid);

            return Json(product, JsonRequestBehavior.AllowGet);

        }


        //Search
        [HttpPost]
        public ActionResult OthersProducts(String ProductName, int UserID)
        {
            var products = db.Products.Where(x => x.UserID != UserID && x.ProductName.Contains(ProductName)).ToList().Select(x=>new Product(x)).ToList();

            return Json(products,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BidProduct(int UserID, int ProductID, decimal OfferedPrice)
        {
            var product = db.Products.Find(ProductID);

            var bidOffer = new BidOffer(UserID, ProductID, OfferedPrice);
            db.BidOffers.Add(bidOffer);
            db.SaveChanges();

            return Json(new { Message = true },JsonRequestBehavior.AllowGet);
        }
    }
}