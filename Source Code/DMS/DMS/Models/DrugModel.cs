using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS.Models
{
    public class DrugModel
    {
        public int DrugID { get; set; }
        public string DrugName { get; set; }
        public Nullable<double> BoxPrice { get; set; }
        public Nullable<double> PackagePrice { get; set; }
    }
}