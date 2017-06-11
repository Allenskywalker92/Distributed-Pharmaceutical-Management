using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using DMS.DAL;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace DMS.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        private UnitOfWork unitOfWork = new UnitOfWork();
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
        public ActionResult RegisterDrugstoreAccount(string username, string password, string fullname, string email, string phonenumber, string drugstoreName, string drugstoreAddress, string coordinate, int districtID)
        {
            var account = new Account();
            var accountInfo = new AccountProfile();
            var drugstore = new Drugstore();
            account.Email = email;
            account.RoleID = 4;
            //var md5Hasher = new MD5CryptoServiceProvider();

            //var encoder = new UTF8Encoding();
            //var hashed = md5Hasher.ComputeHash(encoder.GetBytes(password)).ToString();
            account.Password = md5(password);
            //accountInfo.Email = email;
            accountInfo.Phone = phonenumber;
            accountInfo.FullName = fullname;
            account.Role = unitOfWork.RoleRepository.Get(b => b.RoleID == 4).SingleOrDefault();
            account.IsActive = true;
            account.IsPending = true;
            account.AccountProfile = accountInfo;
            //account.AccountProfiles.Add(accountInfo);
            var checkUser = unitOfWork.AccountRepository.Get(b => b.Email == email).SingleOrDefault();
            //Check if already have that email
            if (checkUser == null)
            {
                var drugstoreTemp = unitOfWork.DrugStoreRepository.Get(b => b.DrugstoreName == drugstoreName && b.Coordinate == coordinate).FirstOrDefault();
                //Check if already have that drugstore
                if (drugstoreTemp != null)
                {
                    //var ownerDrugstore = unitOfWork.AccountRepository.Get(b => b.DrugstoreID == drugstoreTemp.DrugstoreID).SingleOrDefault();
                    //Check if that drugstore already have owner
                    if (drugstoreTemp.OwnerID == null)
                    {

                        //account.DrugstoreID = drugstoreTemp.DrugstoreID;
                        unitOfWork.AccountRepository.Insert(account);
                        unitOfWork.AccountRepository.SaveChanges();
                        var accountDrugstore = unitOfWork.AccountRepository.Get(b => b.Email == account.Email).SingleOrDefault();
                        if (accountDrugstore != null)
                        {
                            drugstoreTemp.OwnerID = accountDrugstore.AccountID;
                            unitOfWork.DrugStoreRepository.Update(drugstoreTemp);
                            unitOfWork.DrugStoreRepository.SaveChanges();
                        }


                        return Json(new { type = "Successful" });
                    }
                    else
                    {
                        return Json(new { type = "DrugstoreAlreadyHaveAccount", message = "Thông tin nhà thuốc bạn vừa đăng ký đã có trong hệ thống." });
                    }

                }
                //Dont have that drugstore
                drugstore.DrugstoreName = drugstoreName;
                drugstore.DistrictID = districtID;
                drugstore.Address = drugstoreAddress;
                drugstore.DrugstoreTypeID = unitOfWork.DrugStoreTypeRepository.GetByID(3).DrugstoreTypeID;
                //drugstore.Debt = 0;
                drugstore.Coordinate = coordinate;
                //drugstore.Debt = 0;
                drugstore.IsActive = true;
                //drugstore.OwnerID = account.AccountID;
                //account.Drugstore=drugstore;
                unitOfWork.AccountRepository.Insert(account);
                unitOfWork.AccountRepository.SaveChanges();
                var accountTemp = unitOfWork.AccountRepository.Get(b => b.Email == account.Email).SingleOrDefault();
                if (accountTemp != null)
                {
                    drugstore.OwnerID = accountTemp.AccountID;
                    unitOfWork.DrugStoreRepository.Insert(drugstore);
                    unitOfWork.DrugStoreRepository.SaveChanges();
                }

                return Json(new { type = "Successful" });
            }
            return Json(new
            {
                type = "Fail",
                message = "Tên tài khoản này đã có người sử dụng.Xin thử lại bằng tên khác!"
            });
        }

        public ActionResult Register()
        {
            //var listDistrict = unitOfWork.DistrictRepository.GetAll();
            //ViewBag.District = listDistrict;
            var listCity = unitOfWork.CityRepository.GetAll();
            ViewBag.City = listCity;
            return View();
        }

        public JsonResult ListDistrict(int cityId)
        {
            //var listDistrict = unitOfWork.DistrictRepository.Get(b=>b.CityID==cityId);
            var listDistrict = (from b in unitOfWork.DistrictRepository.Get(b => b.CityID == cityId)
                                select new { b.DistrictID, b.DistrictName }).ToList();
            return Json(listDistrict);
        }
        public ActionResult RegisterResult(string drugstoreName, string drugstoreAddress)
        {
            return View();
        }

        public ActionResult MappingDrugstore()
        {
            return View();
        }

        public ActionResult ListNewRegister()
        {
            var user = (Account)Session["User"];
            //var drugstoreGroups = unitOfWork.DrugstoreGroupRepository.Get(b => b.SalesmanID == user.AccountID).ToList();
            var accountList = new List<Account>();
            ////for (int i = 0; i < drugstoreGroups.Count; i++)
            ////{
            ////    for (int j = 0; j < drugstoreGroups[i].Drugstores.Count; j++)
            ////    {
            ////        var item = drugstoreGroups[i].Drugstores.ElementAt(j);
            ////        if (item.Account != null && item.Account.IsPending == true)
            ////        {
            ////            accountList.Add(item.Account);
            ////        }
            ////    }
            ////}
            //var districtList = unitOfWork.DistrictRepository.Get(b => b.SalesmanID == user.AccountID).ToList();
            //for (int i = 0; i < districtList.Count; i++)
            //{
            //    for (int j = 0; j < districtList[i].Drugstores.Count; j++)
            //    {
            //        var item = districtList[i].Drugstores.ElementAt(j);
            //        if (item.Account != null && item.Account.IsPending == true)
            //        {
            //            accountList.Add(item.Account);
            //        }
            //    }
            //}
            ////var accountList =
            ////    unitOfWork.AccountRepository.Get(
            ////        b => b.RoleID == 4 && b.IsPending == true && b.IsActive == true);
            ////var drugstoreList = unitOfWork.DrugStoreRepository.Get(b => b.OwnerID != null && b.ApprovedByStaffID == null);
            var salesman = (Account)Session["User"];
            var salesmanID = salesman.AccountID;
            var listDrugstore = unitOfWork.DrugStoreRepository.Get(d => d.District.SalesmanID == salesmanID && d.Account != null && d.Account.IsPending == true
                && d.Account.IsActive == true).ToList();
            accountList = listDrugstore.Select(b => b.Account).ToList();
            return View(accountList);
        }
        //public ActionResult Mapping()
        //{
        //    var accountMapping = (Account)Session["RegisterUser"];
        //    var drugstoreName = accountMapping.Drugstore.DrugstoreName;
        //    var drugstoreAddress = accountMapping.Drugstore.Address;
        //    var drugstore =
        //        unitOfWork.DrugStoreRepository.Get(
        //            b => b.DrugstoreName == drugstoreName && b.Address == drugstoreAddress).SingleOrDefault();
        //    accountMapping.Drugstore = null;
        //    accountMapping.DrugstoreID = drugstore.DrugstoreID;
        //    unitOfWork.AccountRepository.Insert(accountMapping);
        //    unitOfWork.AccountRepository.SaveChanges();
        //    return RedirectToAction("RegisterResult", "Account");
        //}
        //public ActionResult UpdateDrugstoreAccountID()
        //{
        //    //var account = (Account)Session["RegisterUser"];
        //    //var accountID =
        //    //    unitOfWork.AccountRepository.Get(b => b.UserName == account.UserName && b.Password == account.Password);
        //    //var drugstoreName = account.Drugstores.SingleOrDefault().DrugstoreName;
        //    //var drugstoreAddress = account.Drugstores.SingleOrDefault().Address;
        //    //unitOfWork.AccountRepository.Insert(account);
        //    //unitOfWork.AccountRepository.SaveChanges();
        //    //return RedirectToAction("RegisterResult", "Account");
        //}
        public ActionResult LogIn()
        {
            if (Session["Email"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult ApproveAccount(int accountID, int drugstoreTypeID)
        {
            var account = unitOfWork.AccountRepository.GetByID(accountID);
            account.IsPending = false;
            unitOfWork.AccountRepository.Update(account);
            unitOfWork.AccountRepository.SaveChanges();
            var drugstore = account.Drugstores.FirstOrDefault();
            if (drugstore != null)
            {
                drugstore.DrugstoreTypeID = drugstoreTypeID;
                unitOfWork.DrugStoreRepository.Update(drugstore);
                unitOfWork.DrugStoreRepository.SaveChanges();
            }
            return null;
        }
        public ActionResult NotApprove(int accountID)
        {
            var account = unitOfWork.AccountRepository.GetByID(accountID);
            var drugstore = account.Drugstores.FirstOrDefault();
            unitOfWork.DrugStoreRepository.Delete(drugstore);
            unitOfWork.DrugStoreRepository.SaveChanges();
            unitOfWork.AccountRepository.Delete(account);
            unitOfWork.AccountRepository.SaveChanges();
            return null;
        }
        [HttpPost]
        public int CheckLogin(string email, string password)
        {
            var users = unitOfWork.AccountRepository.GetAll();
            Account check = null;
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            UTF8Encoding encoder = new UTF8Encoding();
            var hashed = md5Hasher.ComputeHash(encoder.GetBytes(password)).ToString();
            foreach (var account in users)
            {
                if (account.Email == email && account.IsActive == true)
                {
                    if (account.Password == md5(password))
                    {
                        check = account;
                    }
                }

            }
            if (check != null)
            {
                Session.Timeout = 60;
                Session["Email"] = check.Email;
                Session["User"] = check;
                Session["UserRole"] = check.Role.RoleName;
                //Session["DrugStoreType"]= check.
                FormsAuthentication.SetAuthCookie(check.Email, false);

                string roles = Session["UserRole"].ToString();
                if (roles == "Admin")
                {
                    return 1;
                }
                else if (roles == "Staff")
                {
                    return 2;
                }
                else if (roles == "Salesman")
                {
                    return 3;
                }
                else if (roles == "DrugstoreUser")
                {
                    var listDrugstore = unitOfWork.DrugStoreRepository.GetAll();
                    var drugstore = listDrugstore.Where(d => d.OwnerID == check.AccountID);
                    var drugstoreTypeID = drugstore.Select(d => d.DrugstoreTypeID)
                            .SingleOrDefault()
                            .ToString();
                    //(from d in unitOfWork.DrugStoreRepository.GetAll()
                    //             where d.Account.AccountID == check.AccountID
                    //             select d.DrugstoreTypeID).Single().ToString();
                    Session["DrugStoreTypeID"] = drugstoreTypeID;
                    return 4;
                }

            }
            // If we got this far, something failed, redisplay form
            return 0;
        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session["Email"] = null;
            Session["UserRole"] = null;
            Session["Cart"] = null;
            Session["User"] = null;
            Session["DrugStoreTypeID"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ForgetPassword()
        {
            return View();
        }
        public JsonResult GetPassword(string email)
        {
            var users = unitOfWork.AccountRepository.Get(b => b.Email == email).SingleOrDefault();
            var result = false;
            if (users != null)
            {
                result = true;
                var password = RandomString(6);
                var emailContent = "Mật khẩu của bạn là :" + password;
                SendMail(email, "Lấy lại mật khẩu", emailContent);
                users.Password = md5(password);
                unitOfWork.AccountRepository.Update(users);
                unitOfWork.AccountRepository.SaveChanges();
                return Json(result);
            }
            return Json(result);
        }
        private void SendMail(string diachinhan, string tieude, string noidung)
        {
            string EmailAddress = "truongvothienvu92@gmail.com";
            string Password = "thienvu3230";
            MailMessage mailMessage = new MailMessage(EmailAddress, diachinhan);
            mailMessage.Subject = tieude;
            mailMessage.Body = noidung;
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = UTF8Encoding.UTF8;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = EmailAddress,
                Password = Password
            };
            //smtpClient.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
            //smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            //smtpClient.Timeout = 20000;
            smtpClient.ServicePoint.MaxIdleTime = 1000;
            smtpClient.Send(mailMessage);
            mailMessage.Dispose();
            smtpClient.Dispose();
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        public JsonResult DoChangePassword(string oldPassword,string newPassword)
        {
            var result = false;
            if (Session["User"]!=null)
            {
                var user = (Account)Session["User"];
                if (user.Password == (md5(oldPassword)))
                {
                    var account = unitOfWork.AccountRepository.Get(b => b.AccountID == user.AccountID).SingleOrDefault();
                    account.Password = md5(newPassword);
                    unitOfWork.AccountRepository.Update(account);
                    unitOfWork.AccountRepository.SaveChanges();
                    result = true;
                    Session["User"] = account;
                    return Json(result);
                }
            }
            return Json(result);
        }
    }
}
