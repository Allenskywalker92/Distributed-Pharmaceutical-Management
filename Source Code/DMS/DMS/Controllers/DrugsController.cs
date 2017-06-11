using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using DMS.DAL;
using System.Web.Script.Serialization;
using DMS.Models;
using Newtonsoft.Json;

namespace DMS.Controllers
{
    public class DrugsController : Controller
    {
        //
        // GET: /Drugs/
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult DrugDetails(int id)
        {
            int? drugId = id;
            var drug = unitOfWork.DrugRepository.GetByID(drugId);
            ViewBag.BoxPrice = (from p in unitOfWork.PriceRepository.GetAll()
                                     where p.DrugID == drugId & p.UnitID==1
                                     select p.UnitPrice).Single();
            ViewBag.PackagePrice=(from p in unitOfWork.PriceRepository.GetAll()
                                     where p.DrugID == drugId & p.UnitID==2
                                     select p.UnitPrice).Single();
            if (Session["DrugStoreTypeID"]!=null)
            {
                var drugstoreTypeID = Int16.Parse(Session["DrugStoreTypeID"].ToString());
                ViewBag.ActualValue = (100 - (from p in unitOfWork.DiscountRateRepository.GetAll()
                                              where p.DrugID == drugId & p.DrugstoreTypeID == drugstoreTypeID
                                              select p.Discount).SingleOrDefault())/100;
            }
                
            return View(drug);
        }
        public ActionResult SearchResult()
        {
            return View();

        }

        [HttpPost]
        public JsonResult GetAllDrug()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            var drugs = unitOfWork.DrugRepository.Get(model => model.IsActive == true, includeProperties: "DrugType");
            IEnumerable<DrugModel> drugList = from drug in drugs
                                             select new DrugModel()
                                                 {
                                                     DrugID = drug.DrugID,
                                                     DrugName = drug.DrugName,
                                                     //BoxPrice = drug.BoxPrice,
                                                     //PackagePrice = drug.PackagePrice,
                                                 };
            try
            {
                var result = Json(drugList);
                return result;
            }
            catch (JsonSerializationException e)
            {

                Console.WriteLine(e.Message);
            }
            return null;
        }
    }
}
