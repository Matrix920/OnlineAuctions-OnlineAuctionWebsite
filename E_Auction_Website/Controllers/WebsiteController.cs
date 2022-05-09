using E_Auction_Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Auction_Website.Controllers
{
    public class WebsiteController : Controller
    {
        public const String USER = "user";

        AuctionDBEntities db = new AuctionDBEntities();

        // GET: Website
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if(Session[USER]!=null)
            {
                var user = (User)Session[USER];
                return RedirectToAction("Home", new { user = user });

            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(String Login,String Password)
        {
            var user = db.Users.Where(x => x.Login.Equals(Login) && x.Password.Equals(Password)).SingleOrDefault();

            if (user != null)
            {
                Session[USER] = user;

                return RedirectToAction("Home", new { user = user });
            }

            return View(new { Message = "Incorrect Login" });
        }

        public ActionResult Home(User user)
        {
            return View(user);
        }

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Login");
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(String Username,long Tel,String Login,String Password)
        {
            var user = new User(Username, Password, Login, Tel);

            db.Users.Add(user);

            db.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult AddProduct(String ProductName,int UserID,decimal InitialPrice)
        {
            var product = new Product(UserID, ProductName, InitialPrice, false);

            db.Products.Add(product);

            db.SaveChanges();

            return RedirectToAction("MyProducts", new { UserID = UserID });
        }

        public ActionResult AddProduct()
        {
            return View();
        }

        public ActionResult MyProducts(int UserID)
        {
            var products = db.Products.Where(x => x.UserID == UserID).ToList();

            return View(products);
        }

        public ActionResult MyProductDetails(int ProductID)
        {
            var p = db.Products.Find(ProductID);
            var product = new Product(p, p.BidOffers.ToList());

            return View(product);
        }

        public ActionResult SoldProduct(int ProductID)
        {
            var product = db.Products.Find(ProductID);
            product.IsSold = true;
            db.SaveChanges();

            return RedirectToAction("MyProductDetails", new { ProductID = ProductID });
        }

        [HttpGet]
        public ActionResult OthersProducts(int UserID)
        {
            var products = db.Products.Where(x => x.UserID != UserID && !x.IsSold).ToList();

            return View(products);
        }

        public ActionResult OtherProductDetails(int ProductID)
        {
            var product = db.Products.Find(ProductID);

            decimal maxBid=product.InitialPrice;
            if (product.BidOffers.Count>0)
                maxBid = product.BidOffers.Max(x => x.OfferedPrice);

            ViewBag.MaxBid = maxBid;

            return View(product);

        }


        //Search
        [HttpPost]
        public ActionResult OthersProducts(String ProductName,int UserID)
        {
            var products = db.Products.Where(x => x.UserID != UserID && x.ProductName.Contains(ProductName)).ToList();

            return View(products);
        }

        public ActionResult BidProduct(int UserID,int ProductID,decimal OfferedPrice)
        {
            var product = db.Products.Find(ProductID);

            var maxBid = product.InitialPrice;
            if(product.BidOffers.Count>0)
                maxBid = product.BidOffers.Max(x => x.OfferedPrice);

            if (OfferedPrice < maxBid)
            {
                ViewBag.Message = "Offered Price is less than max bid";
                return RedirectToAction("OtherProductDetails", new { ProductID = ProductID });
            }
            else
            {
                var bidOffer = new BidOffer(UserID, ProductID, OfferedPrice);
                db.BidOffers.Add(bidOffer);
                db.SaveChanges();

                return RedirectToAction("OtherProductDetails", new { ProductID = ProductID });
            }


        }
    }
}