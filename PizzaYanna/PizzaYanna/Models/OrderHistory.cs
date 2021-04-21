//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PizzaYanna.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderHistory
    {
        public int OrderHistoryID { get; set; }
        public int UserID { get; set; }
        public Nullable<int> PizzaID { get; set; }
        public string PizzaName { get; set; }
        public string PizzaSize { get; set; }
        public Nullable<double> PizzaPrice { get; set; }
        public Nullable<int> PizzaAmount { get; set; }
        public string PizzaPicturePath { get; set; }
        public Nullable<int> DrinkID { get; set; }
        public string DrinkName { get; set; }
        public string DrinkSize { get; set; }
        public Nullable<double> DrinkPrice { get; set; }
        public Nullable<int> DrinkAmount { get; set; }
        public string DrinkPicturePath { get; set; }
        public Nullable<int> DessertID { get; set; }
        public string DessertName { get; set; }
        public string DessertSize { get; set; }
        public Nullable<double> DessertPrice { get; set; }
        public Nullable<int> DessertAmount { get; set; }
        public string DessertPicturePath { get; set; }
        public string Timestamp { get; set; }
    
        public virtual User User { get; set; }
    }
}
