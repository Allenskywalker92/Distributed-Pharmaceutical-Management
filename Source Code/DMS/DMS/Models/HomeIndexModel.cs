using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS.Models
{
    public class HomeIndexModel
    {
        public IEnumerable<DAL.Drug> Drug { get; set; }
        public IEnumerable<DAL.DrugType> DrugType { get; set; }
    }
}