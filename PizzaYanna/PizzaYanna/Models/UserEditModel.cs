using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaYanna.Models
{
    public class UserEditModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string EmailID { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }
    }
}