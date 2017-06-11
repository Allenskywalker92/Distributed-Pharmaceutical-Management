using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMS.DAL;

namespace DMS.Controllers
{
    public class FavoriteListController : Controller
    {
        //
        // GET: /FavoriteList/
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult List()
        {
            if (Session["User"]!=null)
            {
                var user = (Account) Session["User"];
                //var favoriteList = unitOfWork.FavoriteListRepository.Get(b => b.AccountID == user.AccountID);
                return View();
            }
            return View();
        }

    }
}
