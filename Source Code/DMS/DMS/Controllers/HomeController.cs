using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMS.DAL;
using DMS.Models;

namespace DMS.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index()
        {
            
                var drug = unitOfWork.DrugRepository.Get(b=>b.IsActive==true);
                var drugType = unitOfWork.DrugTypeRepository.Get(b=>b.IsActive==true);
                ViewBag.DrugType = drugType;
                var homeIndexModel = new HomeIndexModel();
                homeIndexModel.Drug = drug;
                homeIndexModel.DrugType = drugType;
   
            return View(homeIndexModel);
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult SearchArea()
        {
            var drugType = unitOfWork.DrugTypeRepository.Get(b=>b.IsActive==true);
           // ViewBag.DrugTypeID = new SelectList(unitOfWork.DrugTypeRepository.GetAll(), "DrugTypeID", "DrugTypeName");
            return PartialView("SearchAreaPartial", drugType);
        }


        public ActionResult Navigation()
        {
            var drugType = unitOfWork.DrugTypeRepository.Get(b=>b.IsActive==true);
            return PartialView("NavigationPartial", drugType);
        }
        public ActionResult ShoppingCartDropDownPartial()
        {
            return PartialView("ShoppingCartDropDownPartial");
        }

        public ActionResult ListDrugOfCategory(int id)
        {
            var drugstype = unitOfWork.DrugTypeRepository.GetByID(id);
            ViewBag.DrugTypeName = drugstype.DrugTypeName;
            var listDrug = unitOfWork.DrugRepository.GetAll().Where(d => d.DrugTypeID == id&&d.IsActive==true).ToList();
            return View(listDrug);
        }

        
        public ActionResult Search(string search)
        {
            var drugs = unitOfWork.DrugRepository.GetAll();
            if (!String.IsNullOrEmpty(search))
            {
                drugs = drugs.Where(b => b.DrugName.ToUpper().Contains(search.ToUpper())&&b.IsActive==true).ToList();
            }
           return View(drugs);
        }
    }
}
