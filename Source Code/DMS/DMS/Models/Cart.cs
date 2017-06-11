using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DMS.DAL;

namespace DMS.Models
{
    public class Cart
    {
        public Drug Drug { get; set; }
        public Price Price { get; set; }
        public Unit Unit { get; set; }
        //public DrugOrderDetail DrugOrderDetail { get; set; }
        public int Quantity { get; set; }
        // vl, quan he 1 1 ha
        // sao deo co DrugID o day code cho de~
    }
}