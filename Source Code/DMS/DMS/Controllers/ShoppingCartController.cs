using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using DMS.DAL;
using DMS.Models;

namespace DMS.Controllers
{
    public class ShoppingCartController : Controller
    {
        //
        // GET: /ShoppingCart/
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult ShowCart()
        {

            if (Session["Email"] != null)
            {
                var userRole = Session["UserRole"];
                if (userRole.Equals("DrugstoreUser"))
                {
                    var account = (Account) Session["User"];
                    if (account.IsPending==false)
                    {
                        return View();
                        
                    }
                    else
                    {
                        return RedirectToAction("Error", "Account");
                    }
                }
            }
            return RedirectToAction("Login","Account");
        }
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            if (Session["Email"]!=null)
            {
                var userRole = Session["UserRole"];
                if (userRole.Equals("DrugstoreUser") || userRole.Equals("Salesman"))
                {
                    var account = (Account) Session["User"];
                    if (account.IsPending==false)
                    {
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
                    else
                    {
                        return Json(new {message = "AccountNotActiveYet"});
                    }
                   
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
                if (userRole.Equals("DrugstoreUser") || userRole.Equals("Salesman"))
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
                if (userRole.Equals("DrugstoreUser") || userRole.Equals("Salesman"))
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
    }
}
