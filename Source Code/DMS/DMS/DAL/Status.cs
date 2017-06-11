using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS.DAL
{
    public class Status
    {
        public enum StatusEnum
        {
            NotApprove = 1,
            Approved = 2,
            Inprogress = 3,
            Complete = 4,
            Deleted=5,
        };
    }
}