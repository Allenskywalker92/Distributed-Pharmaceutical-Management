using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using DMS.DAL;
using DMS.Models;

namespace DMS.Controllers
{
    public class DrugstoreAccountController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET api/<controller>
        public IEnumerable<object> GetAllAccounts()
        {
            var listAccountDrugstore = unitOfWork.AccountRepository.Get(b=>b.RoleID==4&&b.IsActive==true).ToList();
            var accounts = (from entity in listAccountDrugstore
                select new
                { 
                    AccountID = entity.AccountID,
                    Email = entity.Email,
                    Password = entity.Password,
                   
                }).ToList();
            //var accounts = new List<AccountPOCO>();
            //var account=new Models.AccountPOCO();
            //for (int i = 0; i < listAccountDrugstore.Count(); i++)
            //{
            //    account = new Models.AccountPOCO();
            //    account.AccountID = listAccountDrugstore[i].AccountID;
            //    account.Password = listAccountDrugstore[i].Password;
            //    account.UserName = listAccountDrugstore[i].UserName;
            //    account.RoleID = listAccountDrugstore[i].RoleID;
            //    account.IsActive = listAccountDrugstore[i].IsActive;
            //    account.ProfileID = listAccountDrugstore[i].ProfileID;
            //    account.IsPending = listAccountDrugstore[i].IsPending;
            //    accounts.Add(account);
            //}
            return accounts;
        }

        // GET api/<controller>/5
        public Account GetAccount(int id)
        {
            var accountDrugstore = unitOfWork.AccountRepository.GetByID(id);
            if (accountDrugstore == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return accountDrugstore;

        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
            var accountDrugstore = unitOfWork.AccountRepository.GetByID(id);
            unitOfWork.AccountRepository.Delete(accountDrugstore);
            unitOfWork.AccountRepository.SaveChanges();
        }
    }
}