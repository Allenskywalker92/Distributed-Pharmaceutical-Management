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
    
    public partial class Payment
    {
        public int PaymentID { get; set; }
        public Nullable<int> DrugstoreID { get; set; }
        public Nullable<bool> PaymentType { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<double> Balance { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    
        public virtual Drugstore Drugstore { get; set; }
    }
}