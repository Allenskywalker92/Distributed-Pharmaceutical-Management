//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DMS.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class DrugOrderDetail
    {
        public int DrugOrderDetailsID { get; set; }
        public int DrugOrderID { get; set; }
        public int DrugId { get; set; }
        public int UnitID { get; set; }
        public int Quantity { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<int> DeliveryQuantity { get; set; }
        public string Note { get; set; }
    
        public virtual Drug Drug { get; set; }
        public virtual DrugOrder DrugOrder { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
