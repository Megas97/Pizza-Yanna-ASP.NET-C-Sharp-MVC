using System.ComponentModel.DataAnnotations;

namespace PizzaYanna.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public string ConfirmPassword { get; set; }
    }

    public class UserMetadata
    {
        [Display(Name = "User ID")]
        public int UserID { get; set; }

        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required")]
        [RegularExpression("^[A-Za-zА-Яа-я]*$")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required")]
        [RegularExpression("^[A-Za-zА-Яа-я]*$")]
        public string LastName { get; set; }

        [Display(Name = "Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Address is required")]
        [RegularExpression("^[A-Za-zА-Яа-я0-9]+[A-Za-zА-Яа-я0-9., ]*$")]
        public string Address { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[A-Za-z0-9.-]*[@][A-Za-z.-]*[.][A-Za-z.-]+")]
        public string EmailID { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 characters are required")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirmed password does not match the password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Is Email Verified?")]
        public bool IsEmailVerified { get; set; }

        [Display(Name = "Activation Code")]
        public System.Guid ActivationCode { get; set; }

        [Display(Name = "Reset Password Code")]
        public string ResetPasswordCode { get; set; }

        [Display(Name = "Can Use Promo Codes?")]
        public bool CanUsePromoCodes { get; set; }

        [Display(Name = "Current Promo Code")]
        public string CurrentPromoCode { get; set; }
    }
}