using System.ComponentModel.DataAnnotations;

namespace PizzaYanna.Models
{
    public class UserLoginModel
    {
        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [RegularExpression("^[A-Za-z0-9.-]*[@][A-Za-z.-]*[.][A-Za-z.-]+")]
        public string EmailID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}