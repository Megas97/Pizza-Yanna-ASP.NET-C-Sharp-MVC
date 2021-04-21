using System.ComponentModel.DataAnnotations;

namespace PizzaYanna.Models
{
    [MetadataType(typeof(DessertMetadata))]
    public partial class Dessert
    {

    }

    public class DessertMetadata
    {
        [Display(Name = "Dessert ID")]
        public int DessertID { get; set; }

        [Display(Name = "Dessert Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Dessert name is required")]
        public string DessertName { get; set; }

        [Display(Name = "Dessert Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Dessert price is required")]
        public double DessertPrice { get; set; }

        [Display(Name = "Dessert Image")]
        public string DessertPicturePath { get; set; }

        [Display(Name = "Dessert Size")]
        public string DessertSize { get; set; }
    }
}