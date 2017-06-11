using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMS.DAL;
using DMS.Models;

namespace DMS.Controllers
{
    public class OrderController : Controller
    {
        //
        // GET: /Order/
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult CheckOut()
        {
            if (Session["Cart"] != null)
            {
                var user = (Account)Session["User"];
                var drugstore =
                    unitOfWork.DrugStoreRepository.Get(b => b.OwnerID == user.AccountID).SingleOrDefault();
                //var drugstore = unitOfWork.DrugStoreRepository.Get(b => b.DrugstoreID == user.DrugstoreID).SingleOrDefault();
                return View(drugstore);
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult OrderDetails(int orderID)
        {
            if (Session["User"] != null)
            {
                var account = (Account)Session["User"];
                var order = unitOfWork.DrugOrderRepository.Get(b => b.DrugOrderID == orderID).SingleOrDefault();
                return View(order);
            }

            return View();
        }
        public ActionResult OrderHistory()
        {
            if (Session["User"]!=null)
            {
                var account = (Account)Session["User"];
                var drugstore =
                    unitOfWork.DrugStoreRepository.Get(b => b.OwnerID == account.AccountID).SingleOrDefault();
                var orderHistory = unitOfWork.DrugOrderRepository.Get(b => b.DrugstoreID == drugstore.DrugstoreID).OrderBy(b => b.DrugOrderID);
                return View(orderHistory);
            }
            
            return View();
        }
        public ActionResult CreateOrder(string note)
        {
            if (Session["Cart"]!=null)
            {
                var cartList = (List<Cart>)Session["Cart"];
                var user = (Account)Session["User"];
                var order = new DrugOrder();
                var orderDetails = new DrugOrderDetail();
                var drugstore =
                    unitOfWork.DrugStoreRepository.Get(b => b.OwnerID == user.AccountID).SingleOrDefault();
                order.DrugstoreID = drugstore.DrugstoreID;
                double totalprice = 0;
                double actualvalue;
                for (int i = 0; i < cartList.Count; i++)
                {
                    orderDetails = new DrugOrderDetail();
                    orderDetails.DrugId = cartList[i].Drug.DrugID;
                    orderDetails.Quantity = cartList[i].Quantity;
                    orderDetails.DeliveryQuantity = cartList[i].Quantity;
                    orderDetails.UnitID = cartList[i].Unit.UnitId;
                    orderDetails.UnitPrice = cartList[i].Drug.Prices.Where(b => b.UnitID == orderDetails.UnitID).Select(b => b.UnitPrice).SingleOrDefault() * orderDetails.Quantity *
                        (100 - cartList[i].Drug.DiscountRates.Where(b => b.DrugstoreTypeID == drugstore.DrugstoreTypeID).Select(b => b.Discount).SingleOrDefault()) / 100;
                    totalprice = totalprice +
                        cartList[i].Drug.Prices.Where(b => b.UnitID == orderDetails.UnitID).Select(b => b.UnitPrice).SingleOrDefault() * orderDetails.Quantity *
                        (100 - cartList[i].Drug.DiscountRates.Where(b => b.DrugstoreTypeID == drugstore.DrugstoreTypeID).Select(b => b.Discount).SingleOrDefault()) / 100;
                    order.DrugOrderDetails.Add(orderDetails);
                }
                order.DateOrder = DateTime.Now;
                order.TotalPrice = totalprice;
     
                order.IsActive = true;
                order.Note = note;
                order.Status = (int)Status.StatusEnum.NotApprove;
                order.SalesmanID = drugstore.District.SalesmanID;
                bool check = unitOfWork.DrugOrderRepository.Insert(order);
                unitOfWork.DrugOrderRepository.SaveChanges();
                unitOfWork.DrugOrderDetailRepository.SaveChanges();
                //drugstore.Debt = drugstore.Debt + totalprice;
                unitOfWork.DrugStoreRepository.Update(drugstore);
                unitOfWork.DrugStoreRepository.SaveChanges();
                if (check)
                {
                    Session["Cart"] = null;
                }
            }
            return View();
        }

        public ActionResult DeleteOrder(int orderID)
        {
            var order = unitOfWork.DrugOrderRepository.GetByID(orderID);
            if (order.Status != 3 && order.Status!=4)
            {
                order.Status = (int)Status.StatusEnum.Deleted;
                unitOfWork.DrugOrderRepository.SaveChanges();
                return Json(new{type="success"});
            }
            return Json(new { type = "fail" });
        }

        public ActionResult UpdateProfile(string fullname, string phone, string email)
        {
            var user = (Account)Session["User"];
            var profile = unitOfWork.AccountProfileRepository.Get(b => b.ProfileID == user.ProfileID).SingleOrDefault();
            profile.FullName = fullname;
            profile.Phone = phone;
            //profile.Email = email;

            unitOfWork.AccountProfileRepository.Update(profile);
            unitOfWork.AccountProfileRepository.SaveChanges();
            return new EmptyResult();
        }

        //public ActionResult UpdateNote(string note)
        //{
        //    var user = (Account)Session["User"];
        //    var profile = unitOfWork.AccountProfileRepository.Get(b => b.ProfileID == user.ProfileID).SingleOrDefault();
            
        //    unitOfWork.AccountProfileRepository.Update(profile);
        //    unitOfWork.AccountProfileRepository.SaveChanges();
        //    return new EmptyResult();
        //}
    }
}
