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
    
    public partial class UsersRole
    {
        public int UsersRolesID { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
    
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
