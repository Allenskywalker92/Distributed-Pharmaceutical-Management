using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS.Models
{
    public class AccountPOCO
    {
        public AccountPOCO()
        {
        }

        public int AccountID { get; set; }
        public int RoleID { get; set; }
        public int ProfileID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<bool> IsPending { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}