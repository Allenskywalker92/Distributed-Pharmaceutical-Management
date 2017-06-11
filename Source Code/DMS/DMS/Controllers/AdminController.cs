using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMS.DAL;
using System.Web.Security;
using System.Security.Cryptography;

namespace DMS.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        UnitOfWork unitOfWork = new UnitOfWork();

        public static byte[] encryptData(string data)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
        }
        public static string md5(string data)
        {
            return BitConverter.ToString(encryptData(data)).Replace("-", "").ToLower();
        }
        public ActionResult Index()
        {
            if (Session["User"] != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        
        public ActionResult ManageStaff()
        {
            if (Session["User"] != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ListStaff()
        {
            var staffs = unitOfWork.AccountRepository.Get(s => s.RoleID == 2).ToList();
            return PartialView(staffs);
        }

        public ActionResult ListSalesman()
        {
            var salesmans = unitOfWork.AccountRepository.Get(s => s.RoleID == 3).ToList();
            return PartialView(salesmans);
        }

        public ActionResult ListDeliveryMan()
        {
            var deliverymans = unitOfWork.DeliverymanRepository.GetAll();
            return PartialView(deliverymans);
        }

        public ActionResult ListUser()
        {
            var users = unitOfWork.AccountRepository.Get(u => u.RoleID == 4).ToList();
            return View(users);
        }

        public ActionResult Block(int id)
        {
            var account = unitOfWork.AccountRepository.GetByID(id);
            if (account.RoleID == 2 || account.RoleID == 3)
            {
                account.IsActive = false;
                unitOfWork.AccountRepository.Update(account);
                unitOfWork.AccountRepository.SaveChanges();
                if (account.RoleID == 3)
                {
                    var listDistrict=unitOfWork.DistrictRepository.Get(b => b.SalesmanID == id);
                    foreach (var district in listDistrict)
                    {
                        district.SalesmanID = null;
                        unitOfWork.DistrictRepository.Update(district);
                    }
                    unitOfWork.DistrictRepository.SaveChanges();
                }
                return RedirectToAction("ManageStaff", "Admin");
            }
            else if(account.RoleID==4) {
                account.IsActive = false;
                unitOfWork.AccountRepository.Update(account);
                unitOfWork.AccountRepository.SaveChanges();
                return RedirectToAction("ListUser", "Admin");
            }

            return View();
        }

        public ActionResult Unblock(int id)
        {
            var account = unitOfWork.AccountRepository.GetByID(id);
            if (account.RoleID == 2 || account.RoleID == 3)
            {
                account.IsActive = true;
                unitOfWork.AccountRepository.Update(account);
                unitOfWork.AccountRepository.SaveChanges();
                return RedirectToAction("ManageStaff", "Admin");
            }
            else if (account.RoleID == 4)
            {
                account.IsActive = true;
                unitOfWork.AccountRepository.Update(account);
                unitOfWork.AccountRepository.SaveChanges();
                return RedirectToAction("ListUser", "Admin");
            }

            return View();
        }

        public ActionResult CreateNewStaff(string username, string password, string fullname, string address, string phonenumber, string coordinate, string email)
        {
            Account staff = new Account();
            AccountProfile staffProfile = new AccountProfile();
            staff.Email = email;
            staff.RoleID = 2;

            staff.Password = md5(password);
            //staffProfile.Email = email;
            staffProfile.Phone = phonenumber;
            staffProfile.FullName = fullname;
            staffProfile.Address = address;
            staffProfile.Coordinate = coordinate;
            staff.IsActive = true;
            staff.IsPending = true;
            staff.AccountProfile = staffProfile;
            unitOfWork.AccountRepository.Insert(staff);
            unitOfWork.AccountRepository.SaveChanges();

            return View("ManageStaff", "Admin");
        }
        public ActionResult CreateStaff()
        {
            if (Session["User"] != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CreateNewSalesman(string username, string password, string fullname, string address, string phonenumber, string coordinate, string email)
        {
            Account salesman = new Account();
            AccountProfile salesmanProfile = new AccountProfile();
            salesman.Email = email;
            salesman.RoleID = 3;

            salesman.Password = md5(password);
            //salesmanProfile.Email = email;
            salesmanProfile.Phone = phonenumber;
            salesmanProfile.FullName = fullname;
            salesmanProfile.Address = address;
            salesmanProfile.Coordinate = coordinate;
            salesman.IsActive = true;
            salesman.IsPending = true;
            salesman.AccountProfile = salesmanProfile;
            unitOfWork.AccountRepository.Insert(salesman);
            unitOfWork.AccountRepository.SaveChanges();

            return RedirectToAction("ManageStaff", "Admin");
        }
        public ActionResult CreateSalesman()
        {
            if (Session["User"] != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public JsonResult CreateNewDelivery(string fullname, string address, string phonenumber)
        {
            DeliveryMan delivery = new DeliveryMan();
            var result = false;
            delivery.Phone = phonenumber;
            delivery.FullName = fullname;
            delivery.Address = address;
            delivery.IsActive = true;
            unitOfWork.DeliverymanRepository.Insert(delivery);
            unitOfWork.DeliverymanRepository.SaveChanges();
            result = true;
            return Json(result);
        }

        public ActionResult CreateDelivery()
        {
            if (Session["User"] != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EditDelivery(int id)
        {
            var Delivery = unitOfWork.DeliverymanRepository.GetByID(id);
            if (Delivery == null)
            {
                return HttpNotFound();
            }
            return View(Delivery);
        }

        public ActionResult EditDeli(int id, string fullname, string address, string phonenumber)
        {
            var Delivery = unitOfWork.DeliverymanRepository.GetByID(id);
            var result = false;
            Delivery.FullName = fullname;
            Delivery.Address = address;
            Delivery.Phone = phonenumber;
            unitOfWork.DeliverymanRepository.Update(Delivery);
            unitOfWork.DeliverymanRepository.SaveChanges();
            result = true;
            return Json(result);
        }

       
    }
}
