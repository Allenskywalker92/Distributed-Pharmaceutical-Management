using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using DMS.DAL;
using System.Data.Objects.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.Xml;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Xml.Linq;
using DMS.Service;

namespace DMS.Controllers
{
    public class StaffController : Controller
    {
        //
        // GET: /Admin/
        private UnitOfWork unitOfWork = new UnitOfWork();
        KMeans kMeans = new KMeans();
        public ActionResult Index()
        {
            if (Session["Email"] != null && Session["UserRole"].ToString().Equals("Staff"))
            {
                return RedirectToAction("ListOrderNotApprove");
            }
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// Quản lí thuốc
        /// </summary>
        /// <returns></returns>
        public ActionResult ListDrug()
        {
            if (Session["Email"] != null && Session["UserRole"].ToString().Equals("Staff"))
            {
                var listDrug = unitOfWork.DrugRepository.Get(b => b.IsActive == true);
                return View(listDrug.ToList());
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Admin/Create

        public ActionResult CreateDrug()
        {
            if (Session["Email"] != null && Session["UserRole"].ToString().Equals("Staff"))
            {
                //ViewBag.DrugCompanyID = new SelectList(unitOfWork.DrugCompanyRepository.GetAll(), "DrugCompanyID", "DrugCompanyName");
                ViewBag.DrugTypeID = new SelectList(unitOfWork.DrugTypeRepository.Get(b=>b.IsActive==true), "DrugTypeID", "DrugTypeName");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Admin/Create

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateDrug(FormCollection form, HttpPostedFileBase file)
        {
            Drug drug = new Drug();
            Price packagePrice = new Price();
            Price boxPrice = new Price();
            DiscountRate bigDiscountRate = new DiscountRate();
            DiscountRate mediumDiscountRate = new DiscountRate();
            DiscountRate smallDiscountRate = new DiscountRate();
            try
            {
                //drug.DrugCompanyID = int.Parse(form["DrugCompanyID"]);
                drug.DrugTypeID = int.Parse(form["DrugTypeID"]);
                drug.DrugName = form["DrugName"];
                drug.Description = form["Description"];
                packagePrice.UnitPrice = float.Parse(form["price-package"]);
                packagePrice.UnitID = 2;
                boxPrice.UnitPrice = float.Parse(form["price-box"]);
                boxPrice.UnitID = 1;
                bigDiscountRate.Discount = float.Parse(form["big-discount"]);
                bigDiscountRate.DrugstoreTypeID = 1;
                mediumDiscountRate.Discount = float.Parse(form["medium-discount"]);
                mediumDiscountRate.DrugstoreTypeID = 2;
                smallDiscountRate.Discount = float.Parse(form["small-discount"]);
                smallDiscountRate.DrugstoreTypeID = 3;
                //drug.BoxPrice =float.Parse(form["BoxPrice"]);
                //drug.PackagePrice = float.Parse(form["PackagePrice"]);
                //drug.Price = float.Parse(form["Price"]);
                drug.Prices.Add(boxPrice);
                drug.Prices.Add(packagePrice);
                drug.DiscountRates.Add(bigDiscountRate);
                drug.DiscountRates.Add(mediumDiscountRate);
                drug.DiscountRates.Add(smallDiscountRate);
                if (file != null)
                {
                    //file.SaveAs(HttpContext.Server.MapPath("/assets/images/Drugs") + file.FileName);
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/assets/images/Drugs"), pic);
                    file.SaveAs(path);
                    drug.ImageUrl = "/assets/images/Drugs/" + file.FileName;

                }
                drug.IsActive = true;
                unitOfWork.DrugRepository.Insert(drug);
                unitOfWork.DrugRepository.SaveChanges();
            }
            catch (FormatException e)
            {

                Console.WriteLine(e.Message);
            }

            return RedirectToAction("ListDrug");
        }

        //
        // GET: /Edit drug

        public ActionResult EditDrug(int id = 0)
        {
            if (Session["Email"] != null && Session["UserRole"].ToString().Equals("Staff"))
            {
                var drug = unitOfWork.DrugRepository.GetByID(id);
                if (drug == null)
                {
                    return HttpNotFound();
                }
                //ViewBag.DrugCompanyID = new SelectList(unitOfWork.DrugCompanyRepository.GetAll(), "DrugCompanyID", "DrugCompanyName", drug.DrugCompany.DrugCompanyID);
                ViewBag.DrugTypeID = new SelectList(unitOfWork.DrugTypeRepository.GetAll(), "DrugTypeID", "DrugTypeName", drug.DrugType.DrugTypeID);
                return View(drug);
            }
            return RedirectToAction("Index", "Home");
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult EditDrug(FormCollection col, HttpPostedFileBase file)
        {
            var id = int.Parse(col["DrugID"]);
            var drug = unitOfWork.DrugRepository.GetByID(id);

            //Price packagePrice = new Price();
            //Price boxPrice = new Price();
            //DiscountRate bigDiscountRate = new DiscountRate();
            //DiscountRate mediumDiscountRate = new DiscountRate();
            //DiscountRate smallDiscountRate = new DiscountRate();

            //drug.DrugCompanyID = int.Parse(col["DrugCompanyID"]);
            drug.DrugTypeID = int.Parse(col["DrugTypeID"]);
            drug.DrugName = col["DrugName"];
            drug.Description = col["Description"];
            //drug.BoxPrice = float.Parse(col["BoxPrice"]);
            //drug.PackagePrice = float.Parse(col["PackagePrice"]);
            //drug.Price = float.Parse(col["Price"]);
            var boxPrice = unitOfWork.PriceRepository.Get(b => b.DrugID == id && b.UnitID == 1).Single();
            var packagePrice = unitOfWork.PriceRepository.Get(b => b.DrugID == id && b.UnitID == 2).Single();
            boxPrice.UnitPrice = float.Parse(col["price-box"]);
            packagePrice.UnitPrice = float.Parse(col["price-package"]);
            unitOfWork.PriceRepository.Update(boxPrice);
            unitOfWork.PriceRepository.Update(packagePrice);
            unitOfWork.PriceRepository.SaveChanges();
            //packagePrice.UnitPrice = float.Parse(col["price-package"]);
            //packagePrice.UnitID = 2;
            //boxPrice.UnitPrice = float.Parse(col["price-box"]);
            //boxPrice.UnitID = 1;

            var bigDiscountRate = unitOfWork.DiscountRateRepository.Get(b => b.DrugID == id && b.DrugstoreTypeID == 1).Single();
            var mediumDiscountRate = unitOfWork.DiscountRateRepository.Get(b => b.DrugID == id && b.DrugstoreTypeID == 2).Single();
            var smallDiscountRate = unitOfWork.DiscountRateRepository.Get(b => b.DrugID == id && b.DrugstoreTypeID == 3).Single();
            bigDiscountRate.Discount = float.Parse(col["big-discount"]);
            mediumDiscountRate.Discount = float.Parse(col["medium-discount"]);
            smallDiscountRate.Discount = float.Parse(col["small-discount"]);
            unitOfWork.DiscountRateRepository.Update(bigDiscountRate);
            unitOfWork.DiscountRateRepository.Update(smallDiscountRate);
            unitOfWork.DiscountRateRepository.Update(mediumDiscountRate);
            unitOfWork.DiscountRateRepository.SaveChanges();
            //drug.Prices.Add(boxPrice);
            //drug.Prices.Add(packagePrice);

            //drug.DiscountRates.Add(bigDiscountRate);
            //drug.DiscountRates.Add(mediumDiscountRate);
            //drug.DiscountRates.Add(smallDiscountRate);

            if (file != null)
            {
                file.SaveAs(HttpContext.Server.MapPath("/assets/images/Drugs") + file.FileName);
                drug.ImageUrl = "/assets/images/Drugs/" + file.FileName;

            }
            unitOfWork.DrugRepository.Update(drug);
            unitOfWork.DrugRepository.SaveChanges();
            return RedirectToAction("ListDrug");
        }

        //
        // GET: /Admin/Delete/5

        public ActionResult DeleteDrug(int id)
        {
            if (Session["Email"] != null && Session["UserRole"].ToString().Equals("Staff"))
            {
                var drug = unitOfWork.DrugRepository.GetByID(id);
                if (drug == null)
                {
                    return HttpNotFound();
                }
                return View(drug);
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost, ActionName("DeleteDrug")]
        public ActionResult DeleteComfirmed(int id)
        {
            var drug = unitOfWork.DrugRepository.GetByID(id);
            drug.IsActive = false;
            unitOfWork.DrugRepository.Update(drug);
            unitOfWork.DrugRepository.SaveChanges();
            return RedirectToAction("ListDrug");
        }



        /// <summary>
        /// quản lí category
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult ListCategory()
        {
            if (Session["Email"] != null && Session["UserRole"].ToString().Equals("Staff"))
            {
                var listCategory = unitOfWork.DrugTypeRepository.Get(b => b.IsActive == true);
                return View(listCategory);
            }
            return RedirectToAction("Index", "Home");
        }
        //Create category
        public ActionResult CreateCategory()
        {
            if (Session["Email"] != null && Session["UserRole"].ToString().Equals("Staff"))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult CreateCategory(DrugType category)
        {
            if (ModelState.IsValid)
            {
                category.IsActive = true;
                unitOfWork.DrugTypeRepository.Insert(category);
                unitOfWork.DrugTypeRepository.SaveChanges();
                return RedirectToAction("ListCategory");
            }
            return View(category);
        }

        //Edit category
        public ActionResult EditCategory(int id = 0)
        {
            if (Session["Email"] != null && Session["UserRole"].ToString().Equals("Staff"))
            {
                var drugType = unitOfWork.DrugTypeRepository.GetByID(id);
                if (drugType == null)
                {
                    return HttpNotFound();
                }
                return View(drugType);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult EditCategory(DrugType drugType)
        {
            if (ModelState.IsValid)
            {
                drugType.IsActive = true;
                unitOfWork.DrugTypeRepository.Update(drugType);
                unitOfWork.DrugTypeRepository.SaveChanges();
                return RedirectToAction("ListCategory");
            }
            return View(drugType);
        }

        //Delete category
        public ActionResult DeleteCategory(int id = 0)
        {
            var drugType = unitOfWork.DrugTypeRepository.GetByID(id);
            if (drugType == null)
            {
                return HttpNotFound();
            }
            return View(drugType);
        }

        [HttpPost, ActionName("DeleteCategory")]
        public ActionResult DeleteConfirmed(int id = 0)
        {
            var drugType = unitOfWork.DrugTypeRepository.GetByID(id);
            drugType.IsActive = false;
            unitOfWork.DrugTypeRepository.Update(drugType);
            foreach (var drug in drugType.Drugs)
            {
                drug.IsActive = false;
                unitOfWork.DrugRepository.Update(drug);
            }
            unitOfWork.DrugRepository.SaveChanges();
            unitOfWork.DrugTypeRepository.SaveChanges();
            return RedirectToAction("ListCategory");
        }


        /// <summary>
        /// Quản lí nhà thuốc
        /// </summary>
        /// <returns></returns>
        public ActionResult ListDrugstore()
        {
            if (Session["Email"] != null && Session["UserRole"].ToString().Equals("Staff"))
            {
                var listDrugstore = unitOfWork.DrugStoreRepository.GetAll();
                //ViewBag.ListDrugstoreGroup = unitOfWork.DrugstoreGroupRepository.GetAll();
                return View(listDrugstore.ToList());
            }
            return RedirectToAction("Index", "Home");
        }

        //Create Drugstore
        public ActionResult AddDrugstore()
        {
            if (Session["Email"] != null && Session["UserRole"].ToString().Equals("Staff"))
            {
                var listCity = unitOfWork.CityRepository.GetAll();
                ViewBag.City = listCity;
                //ViewBag.DistrictID = new SelectList(unitOfWork.DistrictRepository.GetAll(), "DistrictID", "DistrictName");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult AddDrugstore(Drugstore drugStore)
        {
            if (ModelState.IsValid)
            {
                drugStore.DrugstoreTypeID = 3;
                unitOfWork.DrugStoreRepository.Insert(drugStore);
                unitOfWork.DrugStoreRepository.SaveChanges();
                return RedirectToAction("AddDrugstore");
            }
            return View(drugStore);
        }


        //Edit Drugstore
        public ActionResult EditDrugstore(int id = 0)
        {
            if (Session["Email"] != null && Session["UserRole"].ToString().Equals("Staff"))
            {
                var drugStore = unitOfWork.DrugStoreRepository.GetByID(id);
                if (drugStore == null)
                {
                    return HttpNotFound();
                }
                ViewBag.DrugstoreTypeID = new SelectList(unitOfWork.DrugStoreTypeRepository.GetAll(), "DrugstoreTypeID", "DrugstoreTypeName", drugStore.DrugstoreTypeID);
                ViewBag.CityID = new SelectList(unitOfWork.CityRepository.GetAll(), "CityID", "CityName", drugStore.District.CityID);
                ViewBag.DistrictID = new SelectList(unitOfWork.DistrictRepository.GetAll(), "DistrictID", "DistrictName", drugStore.DistrictID);
                return View(drugStore);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult EditDrugstore(Drugstore drugStore)
        {

            var drugstoretemp = unitOfWork.DrugStoreRepository.GetByID(drugStore.DrugstoreID);
            drugstoretemp.DrugstoreName = drugStore.DrugstoreName;
            drugstoretemp.Address = drugStore.Address;
            drugstoretemp.Coordinate = drugStore.Coordinate;
            drugstoretemp.DrugstoreTypeID = drugStore.DrugstoreTypeID;
            drugstoretemp.DistrictID = drugStore.DistrictID;
            unitOfWork.DrugStoreRepository.Update(drugstoretemp);
            unitOfWork.DrugStoreRepository.SaveChanges();
            return RedirectToAction("ListDrugstore");

            // return View(drugStore);
        }

        public ActionResult DeleteDrugstore(int id)
        {
            try
            {
                var drugstoretemp = unitOfWork.DrugStoreRepository.GetByID(id);
                //drugstoretemp.DrugstoreName = drugStore.DrugstoreName;
                //drugstoretemp.Address = drugStore.Address;
                //drugstoretemp.Coordinate = drugStore.Coordinate;
                //drugstoretemp.DrugstoreTypeID = drugStore.DrugstoreTypeID;
                    if (drugstoretemp.OwnerID == null)
                    {
                        unitOfWork.DrugStoreRepository.Delete(drugstoretemp);
                        unitOfWork.DrugStoreRepository.SaveChanges();
                    }
            }
            catch (Exception)
            {

            }


            return RedirectToAction("ListDrugstore");

            // return View(drugStore);
        }
        //public ActionResult ListDrugstoreGroup()
        //{
        //    var listDrugstoreGroup = unitOfWork.DrugstoreGroupRepository.GetAll();
        //    //ViewBag.ListDrugstoreGroup = unitOfWork.DrugstoreGroupRepository.GetAll();
        //    return View(listDrugstoreGroup.ToList());
        //}
        //public ActionResult EditDrugstoreGroup(int id = 0)
        //{
        //    ViewBag.DrugstoreGroupID = id;
        //    var drugstoreGroup = unitOfWork.DrugstoreGroupRepository.GetByID(id);
        //    if (drugstoreGroup == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    if (drugstoreGroup.Account != null)
        //    {
        //        ViewBag.SalesmanID = new SelectList(unitOfWork.AccountRepository.Get(b => b.RoleID == 3 && b.IsActive == true), "AccountID", "Username", drugstoreGroup.SalesmanID);
        //    }
        //    else
        //    {
        //        ViewBag.SalesmanID = new SelectList(unitOfWork.AccountRepository.Get(b => b.RoleID == 3 && b.IsActive == true), "AccountID", "Username");

        //    }
        //    return View(drugstoreGroup);
        //}

        //[HttpPost]
        //public ActionResult EditDrugstoreGroup(DrugstoreGroup drugStoreGroup)
        //{
        //    var drugstoreGroupTemp = unitOfWork.DrugstoreGroupRepository.GetByID(drugStoreGroup.DrugstoreGroupID);
        //    drugstoreGroupTemp.SalesmanID = drugStoreGroup.SalesmanID;
        //    unitOfWork.DrugstoreGroupRepository.Update(drugstoreGroupTemp);
        //    unitOfWork.DrugstoreGroupRepository.SaveChanges();
        //    return RedirectToAction("ListDrugstoreGroup");
        //    // return View(drugStore);
        //}

        //public ActionResult DeleteDrugstoreGroup(int id = 0)
        //{
        //    var drugType = unitOfWork.DrugstoreGroupRepository.GetByID(id);
        //    if (drugType == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(drugType);
        //}

        //[HttpPost, ActionName("DeleteDrugstoreGroup")]
        //public ActionResult DeleteDrugstoreGroupConfirmed(int id)
        //{
        //    var drugstoreGroup = unitOfWork.DrugstoreGroupRepository.GetByID(id);
        //    for (int i = 0; i < drugstoreGroup.Drugstores.Count; i++)
        //    {
        //        var drugstore = drugstoreGroup.Drugstores.ElementAt(i);
        //        drugstore.DrugstoreGroupID = null;
        //        unitOfWork.DrugStoreRepository.Update(drugstore);
        //        unitOfWork.DrugStoreRepository.SaveChanges();
        //    }
        //    unitOfWork.DrugstoreGroupRepository.Delete(drugstoreGroup);
        //    unitOfWork.DrugstoreGroupRepository.SaveChanges();
        //    return RedirectToAction("ListDrugstoreGroup");
        //}

        public ActionResult DrugstoreDetails(int id = 0)
        {
            var drugStore = unitOfWork.DrugStoreRepository.GetByID(id);
            return View(drugStore);
        }

        public ActionResult OwnerDrugstoreDetails(int DrugstoreID = 0)
        {
            var drugstore = unitOfWork.DrugStoreRepository.GetByID(DrugstoreID);

            //var accountID = (from a in unitOfWork.DrugStoreRepository.GetAll()
            //                 where a.DrugstoreID == DrugstoreID
            //                 select a.OwnerID).Single();

            //var dd = (from s in unitOfWork.AccountRepository.GetAll()
            //          join p in unitOfWork.AccountProfileRepository.GetAll()
            //          on s.ProfileID equals p.ProfileID
            //          join d in unitOfWork.DrugStoreRepository.GetAll()
            //          on s.AccountID equals d.OwnerID
            //          where s.AccountID == accountID
            //          group new { s, p } by new { s.AccountID, p.FullName, p.Phone, p.Email, d.DrugstoreName, d.Address, d.DrugstoreID }
            //              into k
            //              select new
            //              {
            //                  Count = k.Count(),
            //                  k.Key.AccountID,
            //                  k.Key.Email,
            //                  k.Key.Phone,
            //                  k.Key.FullName,
            //                  k.Key.Address,
            //                  k.Key.DrugstoreName,
            //                  k.Key.DrugstoreID
            //              }).Single();
            if (drugstore.Account != null)
            {
                return Json(new
            {
                DN = drugstore.DrugstoreName,
                FN = drugstore.Account.AccountProfile.FullName,
                A = drugstore.Address,
                //E = drugstore.Account.AccountProfile.Email,
                P = drugstore.Account.AccountProfile.Phone
            }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    DN = drugstore.DrugstoreName,
                    A = drugstore.Address,
                }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new
            // {
            //     DN = dd.DrugstoreName,
            //     FN = dd.FullName,
            //     A = dd.Address,
            //     E = dd.Email,
            //     P = dd.Phone
            // }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageDrugstoreDetails(int id = 0)
        {
            var drugstore = unitOfWork.DrugStoreRepository.GetByID(id);
            var listSalesman = (from s in unitOfWork.AccountRepository.GetAll()
                                join p in unitOfWork.AccountProfileRepository.GetAll()
                                on s.ProfileID equals p.ProfileID
                                where s.RoleID == 3
                                group new { s, p } by new { s.AccountID, p.FullName }
                                    into k
                                    select new
                                    {
                                        Count = k.Count(),
                                        k.Key.AccountID,
                                        k.Key.FullName
                                    }).ToList();


            ViewBag.DrugstoreTypeID = new SelectList(unitOfWork.DrugStoreTypeRepository.GetAll(), "DrugstoreTypeID", "DrugstoreTypeName", drugstore.DrugstoreTypeID);
            //ViewBag.AccountID = new SelectList(listSalesman, "AccountID", "FullName", drugstore.DrugstoreGroup.SalesmanID);
            //if (drugstore.DrugstoreGroup != null)
            //{
            //    ViewBag.DrugstoreGroupID = new SelectList(unitOfWork.DrugstoreGroupRepository.GetAll(), "DrugstoreGroupID", "DrugstoreGroupID", drugstore.DrugstoreGroupID);
            //}
            //else
            //{
            //    ViewBag.DrugstoreGroupID = new SelectList(unitOfWork.DrugstoreGroupRepository.GetAll(), "DrugstoreGroupID", "DrugstoreGroupID", "-Chọn nhóm-");
            //}
            ViewBag.DrugstoreID = id;
            return PartialView();
        }

        //[HttpPost]
        //public ActionResult ManageDrugstore(string DrugstoreID, string DrugstoreGroupID, string DrugstoreType)
        //{
        //    var id = int.Parse(DrugstoreID);
        //    var drugstore = unitOfWork.DrugStoreRepository.GetByID(id);
        //    drugstore.DrugstoreTypeID = int.Parse(DrugstoreType);
        //    drugstore.DrugstoreGroupID = int.Parse(DrugstoreGroupID);
        //    unitOfWork.DrugStoreRepository.Update(drugstore);
        //    unitOfWork.DrugStoreRepository.SaveChanges();
        //    return Json(new { D = drugstore.DrugstoreTypeID, S = drugstore.DrugstoreGroupID }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult DrugstorePayment(int id)
        {
            ViewBag.DrugstoreID = id;
            var listPayment = unitOfWork.PaymentRepository.Get(b => b.DrugstoreID == id);
            return PartialView(listPayment);
        }

        public ActionResult GetDebt(int DrugstoreID)
        {
            float totalDebt = 0;
            var debt = unitOfWork.PaymentRepository.GetAll().Where(d => d.DrugstoreID == DrugstoreID).ToList();
            //foreach (var i in debt)
            //{
            //    if (i.PaymentType.Equals(true))
            //    {
            //        totalDebt = totalDebt - float.Parse(i.Amount.ToString());
            //    }
            //    else
            //    {
            //        totalDebt = totalDebt + float.Parse(i.Amount.ToString());
            //    }
            //}
            //totalDebt = Math.Abs(totalDebt);
            if (unitOfWork.DrugStoreRepository.GetByID(DrugstoreID).Debt!=null)
            {
                totalDebt = float.Parse(unitOfWork.DrugStoreRepository.GetByID(DrugstoreID).Debt.ToString());
            }
            return Json(totalDebt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Payment(int DrugstoreID, string amount, string paymentType,string name,string phoneNumber)
        {
            var payment = new Payment();
            payment.DrugstoreID = DrugstoreID;
            payment.FullName = name;
            payment.PhoneNumber = phoneNumber;
            var paymentHistory = unitOfWork.PaymentRepository.Get(b => b.DrugstoreID == DrugstoreID).ToList();
            var drugstore = unitOfWork.DrugStoreRepository.Get(b => b.DrugstoreID == DrugstoreID).Single();
            //if (paymentHistory.Count() == 0)
            //{
            //    payment.DrugstoreID = DrugstoreID;
            //    payment.Amount = float.Parse(amount);
            //    payment.Balance = float.Parse(amount);
            //}
            //else
            //{
            //    payment.DrugstoreID = DrugstoreID;
            //    payment.Amount = float.Parse(amount);
            //    payment.Balance = paymentHistory.Last().Balance + float.Parse(amount);
            //}
            if (paymentHistory.Count() > 0)
            {
                if (paymentType == "2")
                {
                    payment.DrugstoreID = DrugstoreID;
                    payment.Amount = float.Parse(amount);
                    payment.Balance = paymentHistory.Last().Balance + float.Parse(amount);
                    payment.PaymentType = true;
                    drugstore.Debt = payment.Balance;

                }
                else if (paymentType == "1")
                {
                    payment.DrugstoreID = DrugstoreID;
                    payment.Amount = float.Parse(amount);
                    payment.Balance = paymentHistory.Last().Balance - float.Parse(amount);
                    payment.PaymentType = false;
                    drugstore.Debt = payment.Balance;
                }

            }
            else
            {
                if (paymentType == "2")
                {
                    payment.DrugstoreID = DrugstoreID;
                    payment.Amount = float.Parse(amount);
                    payment.Balance = 0 + float.Parse(amount);
                    payment.PaymentType = true;
                    drugstore.Debt = payment.Balance;
                }
                else if (paymentType == "1")
                {
                    payment.DrugstoreID = DrugstoreID;
                    payment.Amount = float.Parse(amount);
                    payment.Balance = 0 - float.Parse(amount);
                    payment.PaymentType = false;
                    drugstore.Debt = payment.Balance;
                }
            }


            payment.IsActive = true;
            payment.Date = DateTime.Now;
            unitOfWork.DrugStoreRepository.Update(drugstore);
            unitOfWork.DrugStoreRepository.SaveChanges();
            unitOfWork.PaymentRepository.Insert(payment);
            unitOfWork.PaymentRepository.SaveChanges();
            return new EmptyResult();
            // return RedirectToAction("DrugstorePayment", "Staff");
        }

        //public ActionResult ApproveDrugstore()
        //{
        //    //var listDrugstore = unitOfWork.DrugStoreRepository.GetAll().Where(d => d.DrugstoreGroup.SalesmanID != null).ToList();
        //    return View(listDrugstore);
        //}

        //public ActionResult ApprovedDrugstore(int id)
        //{
        //    var staff = (Account)Session["User"];
        //    var drugstore = unitOfWork.DrugStoreRepository.GetByID(id);
        //    //drugstore.ApprovedByStaffID = staff.AccountID;
        //    unitOfWork.DrugStoreRepository.Update(drugstore);
        //    unitOfWork.DrugStoreRepository.SaveChanges();
        //    return RedirectToAction("ApproveDrugstore", "Staff");
        //}

        /// <summary>
        /// Quản lí yêu cầu
        /// </summary>
        /// <param name="DrugstoreID"></param>
        /// <returns></returns>
        public ActionResult GetOrderDrugstore(int id)
        {
            var orderHistory = unitOfWork.DrugOrderRepository.Get(b => b.DrugstoreID == id).OrderBy(b => b.DrugOrderID);
            return PartialView(orderHistory);
        }



        public ActionResult ListOrderNotApprove()
        {
            var listOrder = unitOfWork.DrugOrderRepository.Get(o => o.Status == 1);
            return View(listOrder);
        }

        public ActionResult ListAllOrder()
        {
            var listAllOrder = unitOfWork.DrugOrderRepository.GetAll();
            return View(listAllOrder);
        }
        //Xác nhận đơn hàng


        //Huỷ đơn hàng

        public ActionResult CancelOrder(int id)
        {
            var order = unitOfWork.DrugOrderRepository.GetByID(id);
            order.Status = (int)Status.StatusEnum.Deleted;
            unitOfWork.DrugOrderRepository.Update(order);
            unitOfWork.DrugOrderRepository.SaveChanges();
            return RedirectToAction("ListOrderNotApprove", "Staff");
        }

        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase file)
        {
            DataSet ds = new DataSet();
            if (Request.Files["file"].ContentLength > 0)
            {
                string fileExtension =
                                     System.IO.Path.GetExtension(Request.Files["file"].FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("/Excel/") + Request.Files["file"].FileName;
                    //if (System.IO.File.Exists(fileLocation))
                    //{
                    //    System.IO.File.Delete(fileLocation);
                    //}
                    //Request.Files["file"].SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=Excel 12.0;";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    // OleDbConnection excelConnection = new System.Data.OleDb.OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }
                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                    var test = excelSheets[0];
                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                    excelConnection1.Close();
                    excelConnection.Close();
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //  string conn = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
                    //  SqlConnection con = new SqlConnection(conn);
                    //   string query = "Insert into Drugstore(DrugstoreName,Address) Values('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "')";//query string
                    //   con.Open();
                    //   SqlCommand cmd = new SqlCommand(query, con);
                    //   unitOfWork.DrugStoreRepository.dbSet.SqlQuery(query);// thực thi câu query
                    //  cmd.ExecuteNonQuery();
                    //   con.Close();
                    try
                    {
                        Drugstore drugstore = new Drugstore();
                        drugstore.DrugstoreName = ds.Tables[0].Rows[i][0].ToString();
                        drugstore.Address = ds.Tables[0].Rows[i][1].ToString();
                        var coordinate = GetGeocode(ds.Tables[0].Rows[i][1].ToString());
                        drugstore.Coordinate = coordinate;
                        drugstore.IsActive = true;
                        var drugstoreTemp = unitOfWork.DrugStoreRepository.Get(
                            b => b.DrugstoreName == drugstore.DrugstoreName && b.Coordinate == drugstore.Coordinate).ToList();
                        if (drugstoreTemp.Count == 0)
                        {
                            drugstore.DrugstoreTypeID = 3;
                            unitOfWork.DrugStoreRepository.Insert(drugstore);
                            unitOfWork.DrugStoreRepository.SaveChanges();
                        }
                    }
                    catch (Exception)
                    {
                    }

                }

            }

            return RedirectToAction("ListDrugstore");
        }

        public string GetGeocode(string address)
        {
            // var address = "123 something st, somewhere";
            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("GeocodeResponse").Element("result");
            var locationElement = result.Element("geometry").Element("location");
            var lat = locationElement.Element("lat").Value;
            var lng = locationElement.Element("lng").Value;

            var coordinate = lat + "," + lng;
            return coordinate;
        }

        public ActionResult GetCoordinate(int DrugstoreID)
        {
            var coordinate = (from p
                                  in unitOfWork.DrugStoreRepository.GetAll()
                              where p.DrugstoreID == DrugstoreID
                              select new { p.Coordinate }
                             ).Single();
            return Json(coordinate, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GroupDrugstoreOption()
        //{
        //    var listDrugstore = unitOfWork.DrugStoreRepository.Get(b => b.DrugstoreGroup == null).ToList();
        //    var listAllDrugstore = unitOfWork.DrugStoreRepository.GetAll().ToList();
        //    ViewBag.ListAllDrugstore = listAllDrugstore;
        //    Session["ListSalesman"] = unitOfWork.AccountRepository.Get(b => b.Role.RoleID == 3 & b.IsActive == true).ToList();
        //    return View(listDrugstore);
        //}
        //[HttpPost]
        //public ActionResult StepOne(bool option, int number)
        //{
        //    var allGroups = new List<Tuple<List<Drugstore>, string>>();
        //    if (option)
        //    {
        //        allGroups = kMeans.Processing(number);
        //    }
        //    else
        //    {
        //        var listDrugstoreGroupOld = unitOfWork.DrugstoreGroupRepository.GetAll().ToList();
        //        var listDrugstore = unitOfWork.DrugStoreRepository.GetAll().ToList();
        //        for (int i = 0; i < listDrugstore.Count; i++)
        //        {
        //            listDrugstore[i].DrugstoreGroupID = null;
        //            unitOfWork.DrugStoreRepository.Update(listDrugstore[i]);
        //            unitOfWork.DrugStoreRepository.SaveChanges();
        //        }
        //        for (int i = 0; i < listDrugstoreGroupOld.Count; i++)
        //        {
        //            unitOfWork.DrugstoreGroupRepository.Delete(listDrugstoreGroupOld[i]);
        //            unitOfWork.DrugstoreGroupRepository.SaveChanges();
        //        }
        //        allGroups = kMeans.Processing(number);
        //    }
        //    var listDrugstoreGroup = new List<DrugstoreGroup>();
        //    if (allGroups != null)
        //    {
        //        for (int i = 0; i < number; i++)
        //        {
        //            //var index = -1;
        //            //double minDistance = double.MaxValue;
        //            //for (int j = 0; j < allGroups.Count; j++)
        //            //{
        //            //    double curDistance = DistanceFromSalesman(allGroups[j].Item2, listSalemans[i]);
        //            //    if (curDistance < minDistance)
        //            //    {
        //            //        minDistance = curDistance;
        //            //        index = j;
        //            //    }
        //            //}
        //            var drugstoreGroup = new DrugstoreGroup();
        //            //drugstoreGroup.SalesmanID = listSalemans[i].AccountID;
        //            drugstoreGroup.DrugstoreCentroid = allGroups[i].Item2;
        //            unitOfWork.DrugstoreGroupRepository.Insert(drugstoreGroup);
        //            unitOfWork.DrugstoreGroupRepository.SaveChanges();

        //            for (int k = 0; k < allGroups[i].Item1.Count; k++)
        //            {
        //                var drugstoreID = allGroups[i].Item1[k].DrugstoreID;
        //                var drugstore =
        //                    unitOfWork.DrugStoreRepository.GetAll().Single(s => s.DrugstoreID == drugstoreID);
        //                drugstore.DrugstoreGroupID = drugstoreGroup.DrugstoreGroupID;
        //                unitOfWork.DrugStoreRepository.Update(drugstore);
        //                unitOfWork.DrugStoreRepository.SaveChanges();
        //            }
        //            listDrugstoreGroup.Add(drugstoreGroup);
        //            //allGroups.Remove(allGroups[i]);
        //        }

        //        Session["ListDrugstoreGroup"] = listDrugstoreGroup;
        //    }


        //    return View();
        //}

        //public ActionResult StepOne()
        //{
        //    var listDrugstoreGroup = (List<DrugstoreGroup>)Session["ListDrugstoreGroup"];
        //    return View(listDrugstoreGroup);
        //}
        //public ActionResult StepTwo(bool isAutomatic)
        //{
        //    var listDrugstoreGroups = (List<DrugstoreGroup>)Session["ListDrugstoreGroup"];
        //    var drugstoreController = new DrugstoreController();

        //    if (isAutomatic)
        //    {
        //        var listSalesman = unitOfWork.AccountRepository.Get(b => b.Role.RoleID == 3 & b.IsActive == true).ToList();
        //        Random r = new Random();
        //        //foreach (int i in Enumerable.Range(0, listDrugstoreGroups.Count).OrderBy(x => r.Next()))
        //        for (int i = 0; i < listDrugstoreGroups.Count; i++)
        //        {
        //            var index = -1;
        //            double minDistance = double.MaxValue;
        //            if (listSalesman.Count == 0)
        //            {
        //                listSalesman = unitOfWork.AccountRepository.Get(b => b.Role.RoleID == 3 & b.IsActive == true).ToList();
        //            }
        //            for (int j = listSalesman.Count - 1; j >= 0; j--)
        //            {
        //                double curDistance = drugstoreController.DistanceFromSalesman(listDrugstoreGroups[i].DrugstoreCentroid, listSalesman[j]);
        //                if (curDistance < minDistance)
        //                {
        //                    minDistance = curDistance;
        //                    index = j;
        //                }
        //            }
        //            listDrugstoreGroups[i].SalesmanID = listSalesman[index].AccountID;
        //            var drugstoreGroup = unitOfWork.DrugstoreGroupRepository.GetByID(listDrugstoreGroups[i].DrugstoreGroupID);
        //            drugstoreGroup.SalesmanID = listSalesman[index].AccountID;
        //            unitOfWork.DrugstoreGroupRepository.Update(drugstoreGroup);
        //            unitOfWork.DrugstoreGroupRepository.SaveChanges();
        //            listSalesman.Remove(listSalesman[index]);
        //        }
        //        Session["ListDrugstoreGroup"] = listDrugstoreGroups;
        //    }
        //    else
        //    {
        //        for (int i = 0; i < listDrugstoreGroups.Count; i++)
        //        {
        //            var drugstoreGroup = unitOfWork.DrugstoreGroupRepository.GetByID(listDrugstoreGroups[i].DrugstoreGroupID);
        //            drugstoreGroup.SalesmanID = listDrugstoreGroups[i].SalesmanID;
        //            unitOfWork.DrugstoreGroupRepository.Update(drugstoreGroup);
        //            unitOfWork.DrugstoreGroupRepository.SaveChanges();
        //        }
        //    }
        //    return RedirectToAction("Result");
        //}

        public ActionResult Result()
        {
            return View();
        }
        //public ActionResult UpdateGroup(int drugstoreGroupID, int salesmanID)
        //{
        //    var listDrugstoreGroups = (List<DrugstoreGroup>)Session["ListDrugstoreGroup"];
        //    var listSalesman = (List<Account>)Session["ListSalesman"];
        //    for (int i = 0; i < listDrugstoreGroups.Count; i++)
        //    {
        //        if (listDrugstoreGroups[i].DrugstoreGroupID == drugstoreGroupID)
        //        {
        //            listDrugstoreGroups[i].SalesmanID = salesmanID;
        //            break;
        //        }
        //    }
        //    Session["ListDrugstoreGroup"] = listDrugstoreGroups;
        //    return null;
        //}
        //public ActionResult AssignForSaleman()
        //{
        //    //var listDrugstoreGroup = unitOfWork.DrugstoreGroupRepository.Get(b => b.SalesmanID == null);
        //    Session["ListDrugstoreGroup"] = listDrugstoreGroup;
        //    Session["ListSalesman"] = unitOfWork.AccountRepository.Get(b => b.Role.RoleID == 3 & b.IsActive == true).ToList();
        //    return View();
        //}

        public ActionResult ListDistrict(int cityID = 1)
        {
            var listSalesman = unitOfWork.AccountRepository.Get(b => b.RoleID == 3).ToList();
            //ViewBag.Salesman = new SelectList(list, "AccountID",
            //    "Email","----Chọn----");
            ViewBag.Salesman = listSalesman;
            var listCity = unitOfWork.CityRepository.GetAll();
            //ViewBag.Salesman = new SelectList(list, "AccountID",
            //    "Email","----Chọn----");
            ViewBag.City = listCity;
            ViewBag.CityID = cityID;
            var listDistrict = unitOfWork.DistrictRepository.Get(b => b.CityID == cityID).ToList();
            return View(listDistrict);
        }

        public ActionResult AssignSaleman(int districtID, int salesmanID, int cityID)
        {
            var district = unitOfWork.DistrictRepository.GetByID(districtID);
            district.SalesmanID = salesmanID;
            unitOfWork.DistrictRepository.Update(district);
            unitOfWork.DistrictRepository.SaveChanges();
            return RedirectToAction("ListDistrict", new { cityID = cityID });
        }

        public ActionResult ReviewOrder(int orderID)
        {
            var drugsOrder = unitOfWork.DrugOrderRepository.GetByID(orderID);
            Session["DrugsOrderDetails"] = drugsOrder.DrugOrderDetails.ToList();
            return View(drugsOrder);
        }

        public JsonResult UpdateDeliveryQuantity(int drugorderDetailsID, int deliveryQuantity)
        {
            var result = false;
            if (Session["DrugsOrderDetails"] != null)
            {
                var drugsOrderDetails = (List<DrugOrderDetail>)Session["DrugsOrderDetails"];
                drugsOrderDetails.Single(b => b.DrugOrderDetailsID == drugorderDetailsID).DeliveryQuantity = deliveryQuantity;
                result = true;
                Session["DrugsOrderDetails"] = drugsOrderDetails;
            }
            return Json(result);
        }
        public JsonResult UpdateNote(int drugorderDetailsID,  string note)
        {
            var result = false;
            if (Session["DrugsOrderDetails"] != null)
            {
                var drugsOrderDetails = (List<DrugOrderDetail>)Session["DrugsOrderDetails"];
                drugsOrderDetails.Single(b => b.DrugOrderDetailsID == drugorderDetailsID).Note = note;
                Session["DrugsOrderDetails"] = drugsOrderDetails;
                result = true;
            }
            return Json(result);
        }
        public ActionResult ApproveOrder(int id)
        {
            var order = unitOfWork.DrugOrderRepository.GetByID(id);
            order.Status = (int)Status.StatusEnum.Approved;
            var listOrderDeitals = order.DrugOrderDetails.ToList();
            double? totalPrice = 0;
            if (Session["DrugsOrderDetails"] != null)
            {
                var drugsOrderDetails = (List<DrugOrderDetail>)Session["DrugsOrderDetails"];
                for (int i = 0; i < listOrderDeitals.Count; i++)
                {
                    var temp =drugsOrderDetails.Single(b => b.DrugOrderDetailsID == listOrderDeitals[i].DrugOrderDetailsID);
                    listOrderDeitals[i].DeliveryQuantity =temp.DeliveryQuantity;
                    listOrderDeitals[i].Note = temp.Note;
                    unitOfWork.DrugOrderDetailRepository.Update(listOrderDeitals[i]);
                    totalPrice =totalPrice+ listOrderDeitals[i].DeliveryQuantity*listOrderDeitals[i].UnitPrice;
                }
                Session["DrugsOrderDetails"] = null;
                order.TotalPrice = totalPrice.Value;
            }
            unitOfWork.DrugOrderRepository.Update(order);
            unitOfWork.DrugOrderRepository.SaveChanges();
            return RedirectToAction("ListOrderNotApprove", "Staff");
        }
        public ActionResult OrderDetails(int orderID)
        {
            var drugsOrder = unitOfWork.DrugOrderRepository.GetByID(orderID);
            Session["DrugsOrderDetails"] = drugsOrder.DrugOrderDetails.ToList();
            return View(drugsOrder);
        }

        
    }
}
