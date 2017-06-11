using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMS.DAL;

namespace DMS.Controllers
{
    public class PaymentController : Controller
    {
        //
        // GET: /Payment/
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult PaymentHistory()
        {
            if (Session["User"] != null)
            {
                double debt = 0;
                var account = (Account)Session["User"];
                var drugstore =
                    unitOfWork.DrugStoreRepository.Get(b => b.OwnerID == account.AccountID).SingleOrDefault();
                var paymentHistory = unitOfWork.PaymentRepository.Get(b => b.DrugstoreID == drugstore.DrugstoreID&&b.IsActive==true).Reverse();
                if (drugstore.Debt != null)
                {
                    Session["Debt"] = drugstore.Debt;
                }
                else {
                    Session["Debt"] = debt;
                }
                //try
                //{
                //    var  lastPayment = unitOfWork.PaymentRepository.Get(b => b.DrugstoreID == drugstore.DrugstoreID).ToList();
                //if (lastPayment.Count>0)
                //{
                //    Session["Debt"] =lastPayment.Last().Balance;
                //}
                //}
                //catch (Exception)
                //{
                    
                //} 
                
                return View(paymentHistory);
            }

            return View();
        }

        [HttpPost]
        public ActionResult UpdatePaymentStatus(DateTime date)
        {
            
                var account = (Account)Session["User"];
                var drugstore = unitOfWork.DrugStoreRepository.Get(b => b.OwnerID == account.AccountID).SingleOrDefault();
                var payment = unitOfWork.PaymentRepository.Get(b => b.DrugstoreID == drugstore.DrugstoreID && b.Date <= date);
                for (int i = 0; i < payment.Count(); i++)
                {
                    var item = payment.ElementAt(i);
                    item.IsActive = false;
                    unitOfWork.PaymentRepository.Update(item);
                    unitOfWork.PaymentRepository.SaveChanges();
                }
                return RedirectToAction("PaymentHistory");
        
        }


    }
}
