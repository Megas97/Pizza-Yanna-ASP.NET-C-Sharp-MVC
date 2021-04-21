using System.ComponentModel.DataAnnotations;

namespace PizzaYanna.Models
{
    [MetadataType(typeof(DrinkMetadata))]
    public partial class Drink
    {

    }

    public class DrinkMetadata
    {
        [Display(Name = "Drink ID")]
        public int DrinkID { get; set; }

        [Display(Name = "Drink Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Drink name is required")]
        public string DrinkName { get; set; }

        [Display(Name = "Drink Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Drink price is required")]
        public double DrinkPrice { get; set; }

        [Display(Name = "Drink Image")]
        public string DrinkPicturePath { get; set; }

        [Display(Name = "Drink Size")]
        public string DrinkSize { get; set; }
    }
}