using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMS.DAL;
using DMS.Models;

namespace DMS.Controllers
{
    public class SalesmanController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        //
        // GET: /Salesman/
        public ActionResult Index()
        {
            return RedirectToAction("ListDrugstore");
        }
        /// <summary>
        /// List danh sách những drugstore đã  dc verify bởi salesman 
        /// </summary>
        /// <returns></returns>
        public ActionResult ListDrugstore()
        {
            var salesman = (Account)Session["User"];
            var salesmanID = salesman.AccountID;
            //var listDistrict = unitOfWork.DistrictRepository.Get(b => b.SalesmanID == salesmanID).ToList();
            var listDrugstore = unitOfWork.DrugStoreRepository.Get(d => d.District.SalesmanID == salesmanID && d.Account != null && d.Account.IsPending == false  
                &&d.Account.IsActive == true).ToList();
            return View(listDrugstore);
        }
        public ActionResult ListDrugstoreNoAccount()
        {
            var salesman = (Account)Session["User"];
            var salesmanID = salesman.AccountID;
            var listDrugstore = unitOfWork.DrugStoreRepository.Get(d => d.District.SalesmanID == salesmanID && d.Account == null).ToList();
            return View(listDrugstore);
        }
        /// <summary>
        /// List danh sách những drugstore chưa dc verify
        /// </summary>
        /// <returns></returns>
        public ActionResult ListDrugstoreNotVerify()
        {
            var salesman = (Account)Session["User"];
            var salesmanID = salesman.AccountID;
            //var listDrugstore = unitOfWork.DrugStoreRepository.GetAll().Where(d => d.DrugstoreGroup != null ).ToList();
            //var listDrugstore = unitOfWork.DrugStoreRepository.Get(d=>d.Account.IsPending==true).ToList();
            var listDrugstore = unitOfWork.DrugStoreRepository.GetAll().Where(d => d.District.SalesmanID == salesmanID && d.Account != null && d.Account.IsPending == true
                && d.Account.IsActive == true).ToList();
            return View(listDrugstore);
        }
        /// <summary>
        /// salesman xác thực drugstore
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult VerifyDrugstore(int id)
        {
            var salesman = (Account)Session["User"];
            var salesmanID = salesman.AccountID;
            var drugstore = unitOfWork.DrugStoreRepository.GetByID(id);
            //drugstore.DrugstoreGroup.SalesmanID = salesmanID;
            unitOfWork.DrugStoreRepository.Update(drugstore);
            unitOfWork.DrugStoreRepository.SaveChanges();
            return RedirectToAction("ListDrugstoreNotVerify", "Salesman");
        }
        public ActionResult DrugstoreDetails(int id = 0)
        {
            var drugStore = unitOfWork.DrugStoreRepository.GetByID(id);
            return View(drugStore);
        }
        public ActionResult SalesmanCart(int drugstoreID)
        {
            var drugstore = unitOfWork.DrugStoreRepository.GetByID(drugstoreID);
            Session["DrugStoreTypeID"] = drugstore.DrugstoreTypeID;
            Session["DrugStoreID"] = drugstoreID;
            return View();
        }

        public ActionResult ListAllOrder()
        {

            var salesman = (Account)Session["User"];
            var salesmanID = salesman.AccountID;
            var listDrugstoreID = unitOfWork.DrugStoreRepository.Get(s => s.District.SalesmanID == salesmanID);
            //var listOrder = new List<DrugOrder>();
            //for (int i = 0; i < listDrugstoreID.Count(); i++)
            //{
            //    var drugstoreID = listDrugstoreID.ElementAt(i).DrugstoreID;
            //    var orders = unitOfWork.DrugOrderRepository.Get(a => a.DrugstoreID == drugstoreID);
            //    listOrder.AddRange(orders);
            //}
            var orders = unitOfWork.DrugOrderRepository.Get(a => a.SalesmanID == salesmanID);
            return View(orders);
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            if (Session["Email"] != null)
            {
                var userRole = Session["UserRole"];
                var account = (Account)Session["User"];
                var drugTypeID = Int16.Parse(Session["DrugStoreTypeID"].ToString());
                // Get the cart 
                List<Cart> cartList;
                if (Session["Cart"] != null)
                {
                    cartList = (List<Cart>)Session["Cart"];
                    var cartItem = cartList
                       .FirstOrDefault(item => item.Drug.DrugID == id);
                    if (cartItem != null)
                    {
                        //cartItem.Drug.DiscountRates = cartItem.Drug.DiscountRates.Where(
                        //    b => b.Drug.DrugID == id && b.DrugstoreTypeID == drugTypeID).Select(b=>b.Drug.DiscountRates).SingleOrDefault();
                        cartItem.Quantity = cartItem.Quantity + quantity;
                    }
                    else
                    {
                        Price price = (DAL.Price)unitOfWork.PriceRepository.Get(b => b.UnitID == 2 && b.DrugID == id).SingleOrDefault();
                        cartList.Add(new Cart { Drug = unitOfWork.DrugRepository.GetByID(id), Price = price, Unit = unitOfWork.UnitRepository.GetByID(2), Quantity = quantity });
                    }
                }
                else
                {
                    Price price = (DAL.Price)unitOfWork.PriceRepository.Get(b => b.UnitID == 2 && b.DrugID == id).SingleOrDefault();
                    cartList = new List<Cart> { new Cart { Drug = unitOfWork.DrugRepository.GetByID(id), Price = price, Unit = unitOfWork.UnitRepository.GetByID(2), Quantity = quantity } };
                }
                Session["Cart"] = cartList;
                // cai drug cua m ko serialize dc,
                // vi no bi referece loop, co nhieu cach solve\
                var drug = unitOfWork.DrugRepository.GetByID(id);
                var drugstoreTypeID = int.Parse(Session["DrugStoreTypeID"].ToString());
                try
                {
                    return Json(new
                    {
                        DrugId = drug.DrugID,
                        DrugName = drug.DrugName,
                        //DrugCompany = drug.DrugCompany.DrugCompanyName,
                        ActualValue = (100 - double.Parse(drug.DiscountRates.Where(b => b.DrugstoreTypeID == drugstoreTypeID).Select(b => b.Discount).SingleOrDefault().ToString())) / 100,
                        DrugType = drug.DrugType.DrugTypeName,
                        Price = drug.Prices.Where(b => b.UnitID == 2).Select(b => b.UnitPrice),
                        Quantity = quantity,
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        message = ex.ToString()
                    });
                }


            }
            return null;
        }

        [HttpPost]
        public ActionResult RemoveCart(int drugId)
        {
            var cartList = (List<Cart>)Session["Cart"];
            var cartItem = cartList.SingleOrDefault(m => m.Drug.DrugID == drugId);
            cartList.Remove(cartItem);
            Session["Cart"] = cartList;
            return new EmptyResult();
        }
        [HttpPost]
        public ActionResult UpdateCartCount(int drugId, int cartCount)
        {
            if (Session["Email"] != null)
            {
                var userRole = Session["UserRole"];
                if (userRole.Equals("DrugstoreUser"))
                {
                    var drugTypeID = Session["DrugStoreTypeID"];
                    // Get the cart 
                    var cartList = new List<Cart>();
                    //var cartList = (List<Cart>)Session["Cart"];
                    if (Session["Cart"] != null)
                    {
                        cartList = (List<Cart>)Session["Cart"];
                        var cartItem = cartList
                           .Single(item => item.Drug.DrugID == drugId);
                        if (cartItem != null)
                        {
                            cartItem.Quantity = cartCount;

                        }
                        else
                        {
                            Price price = (DAL.Price)unitOfWork.PriceRepository.Get(b => b.UnitID == 2 && b.DrugID == drugId).SingleOrDefault();
                            cartList.Add(new Cart { Drug = unitOfWork.DrugRepository.GetByID(drugId), Price = price, Unit = unitOfWork.UnitRepository.GetByID(2), Quantity = cartCount });
                        }
                    }
                    else
                    {
                        //cartList.Add(new Cart { Drug = unitOfWork.DrugRepository.GetByID(drugId), Quantity = cartCount });
                        Price price = (DAL.Price)unitOfWork.PriceRepository.Get(b => b.UnitID == 2 && b.DrugID == drugId).SingleOrDefault();
                        cartList.Add(new Cart { Drug = unitOfWork.DrugRepository.GetByID(drugId), Price = price, Unit = unitOfWork.UnitRepository.GetByID(2), Quantity = cartCount });
                    }
                    Session["Cart"] = cartList;
                    //var drug = unitOfWork.DrugRepository.GetByID(drugId);
                    var drug = cartList.Find(cart => cart.Drug.DrugID == drugId);
                    var drugstoreTypeID = int.Parse(Session["DrugStoreTypeID"].ToString());
                    try
                    {
                        return Json(new
                        {
                            DrugId = drug.Drug.DrugID,
                            DrugName = drug.Drug.DrugName,
                            Price = drug.Price.UnitPrice,
                            ActualValue = (100 - double.Parse(drug.Drug.DiscountRates.Where(b => b.DrugstoreTypeID == drugstoreTypeID).Select(b => b.Discount).SingleOrDefault().ToString())) / 100,
                            //Price = drug.Prices.Where(b => b.UnitID == 2).Select(b => b.UnitPrice),
                            Quantity = cartCount,
                        });
                    }
                    catch (Exception ex)
                    {
                        return Json(new
                        {
                            message = ex.ToString()
                        });
                    }
                }

            }
            return null;
        }

        [HttpPost]
        public ActionResult UpdateUnit(int drugID, int unitID)
        {
            if (Session["Email"] != null)
            {
                var userRole = Session["UserRole"];
                if (userRole.Equals("DrugstoreUser"))
                {
                    var drugTypeID = Session["DrugStoreTypeID"];
                    // Get the cart 
                    var cartList = new List<Cart>();
                    //var cartList = (List<Cart>)Session["Cart"];
                    if (Session["Cart"] != null)
                    {
                        cartList = (List<Cart>)Session["Cart"];
                        var cartItem = cartList
                           .Single(item => item.Drug.DrugID == drugID);
                        if (cartItem != null)
                        {
                            cartItem.Unit.UnitId = unitID;
                            Price price = (DAL.Price)unitOfWork.PriceRepository.Get(b => b.UnitID == unitID && b.DrugID == drugID).SingleOrDefault();
                            cartItem.Price.UnitPrice = price.UnitPrice;
                        }
                        else
                        {
                            Price price = (DAL.Price)unitOfWork.PriceRepository.Get(b => b.UnitID == unitID && b.DrugID == drugID).SingleOrDefault();
                            cartList.Add(new Cart { Drug = unitOfWork.DrugRepository.GetByID(drugID), Price = price, Unit = unitOfWork.UnitRepository.GetByID(unitID), Quantity = 1 });
                        }
                    }
                    else
                    {
                        //cartList.Add(new Cart { Drug = unitOfWork.DrugRepository.GetByID(drugId), Quantity = cartCount });
                        Price price = (DAL.Price)unitOfWork.PriceRepository.Get(b => b.UnitID == unitID && b.DrugID == drugID).SingleOrDefault();
                        cartList.Add(new Cart { Drug = unitOfWork.DrugRepository.GetByID(drugID), Price = price, Unit = unitOfWork.UnitRepository.GetByID(2), Quantity = 1 });
                    }
                    Session["Cart"] = cartList;
                    //var drug = unitOfWork.DrugRepository.GetByID(drugId);
                    var drug = cartList.Find(cart => cart.Drug.DrugID == drugID);
                    Price unitPrice = (DAL.Price)unitOfWork.PriceRepository.Get(b => b.UnitID == unitID && b.DrugID == drugID).SingleOrDefault();
                    var drugstoreTypeID = int.Parse(Session["DrugStoreTypeID"].ToString());
                    try
                    {
                        return Json(new
                        {
                            DrugId = drug.Drug.DrugID,
                            DrugName = drug.Drug.DrugName,
                            Price = unitPrice.UnitPrice,
                            ActualValue = (100 - double.Parse(drug.Drug.DiscountRates.Where(b => b.DrugstoreTypeID == drugstoreTypeID).Select(b => b.Discount).SingleOrDefault().ToString())) / 100,
                            //Price = drug.Prices.Where(b => b.UnitID == 2).Select(b => b.UnitPrice),
                            Quantity = drug.Quantity,
                        });
                    }
                    catch (Exception ex)
                    {
                        return Json(new
                        {
                            message = ex.ToString()
                        });
                    }
                }

            }
            return null;
        }

        public ActionResult CheckOut()
        {
            if (Session["Cart"] != null)
            {
                var drugstoreID = (int)Session["DrugStoreID"];
                var drugstore =
                    unitOfWork.DrugStoreRepository.GetByID(drugstoreID);
                //var drugstore = unitOfWork.DrugStoreRepository.Get(b => b.DrugstoreID == user.DrugstoreID).SingleOrDefault();
                Session["Owner"] = unitOfWork.AccountRepository.Get(b => b.AccountID ==drugstore.OwnerID).SingleOrDefault();

                return View(drugstore);
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult CreateOrder()
        {
            if (Session["Cart"] != null)
            {
                var cartList = (List<Cart>)Session["Cart"];
                //var user = (Account)Session["User"];
                var order = new DrugOrder();
                var orderDetails = new DrugOrderDetail();
                var drugstoreID = (int)Session["DrugStoreID"];
                var drugstore =
                    unitOfWork.DrugStoreRepository.GetByID(drugstoreID);
                order.DrugstoreID = drugstore.DrugstoreID;
                double totalprice = 0;
                double actualvalue;
                for (int i = 0; i < cartList.Count; i++)
                {
                    orderDetails = new DrugOrderDetail();
                    orderDetails.DrugId = cartList[i].Drug.DrugID;
                    orderDetails.Quantity = cartList[i].Quantity;
                    orderDetails.UnitID = cartList[i].Unit.UnitId;

                    totalprice = totalprice +
                        cartList[i].Drug.Prices.Where(b => b.UnitID == orderDetails.UnitID).Select(b => b.UnitPrice).SingleOrDefault() * orderDetails.Quantity *
                        (100 - cartList[i].Drug.DiscountRates.Where(b => b.DrugstoreTypeID == drugstore.DrugstoreTypeID).Select(b => b.Discount).SingleOrDefault()) / 100;
                    order.DrugOrderDetails.Add(orderDetails);
                }
                order.DateOrder = DateTime.Now;
                order.TotalPrice = totalprice;

                order.IsActive = true;

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
            return RedirectToAction("SalesmanCart", new { drugstoreID = (int)Session["DrugStoreID"] });
        }
        public ActionResult OrderDetails(int orderID)
        {
            var drugsOrder = unitOfWork.DrugOrderRepository.GetByID(orderID);
            Session["DrugsOrderDetails"] = drugsOrder.DrugOrderDetails.ToList();
            return View(drugsOrder);
        }
    }



}
