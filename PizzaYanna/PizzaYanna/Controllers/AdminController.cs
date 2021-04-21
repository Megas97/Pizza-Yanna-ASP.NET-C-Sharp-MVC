using PizzaYanna.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace PizzaYanna.Controllers
{
    public class AdminController : Controller
    {
        #region // Access Denied Action
        [HttpGet]
        public ActionResult AccessDenied()
        {
            return View();
        }
        #endregion

        #region // Admin Panel Action
        [Authorize]
        [HttpGet]
        public ActionResult AdminPanel()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Ingredient Action
        [Authorize]
        [HttpGet]
        public ActionResult UploadIngredient()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Ingredient POST Action
        [Authorize]
        [HttpPost]
        public ActionResult UploadIngredient(Ingredient ingredient, FormCollection formCollection)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        ViewBag.Message = "You cannot upload ingredients until you verify your account!"; ;
                        return View();
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    bool canUpload = false;
                                    if (ModelState.IsValid)
                                    {
                                        if (ingredient != null)
                                        {
                                            if (de.Ingredients.Count() < 1)
                                            {
                                                canUpload = true;
                                            }
                                            else
                                            {
                                                foreach (var ingr in de.Ingredients)
                                                {
                                                    if (ingr.IngredientName.Equals(ingredient.IngredientName))
                                                    {
                                                        canUpload = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        canUpload = true;
                                                    }
                                                }
                                            }

                                            if (canUpload == true)
                                            {
                                                ingredient.IngredientPrice = Double.Parse(formCollection["IngredientPriceInput"].Replace(".", ","));
                                                de.Ingredients.Add(ingredient);
                                                de.SaveChanges();
                                                message = "Ingredient uploaded successfully!";
                                            }
                                            else
                                            {
                                                message = "This ingredient is already present in the database!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message = "Invalid Request!";
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    ModelState.Clear();
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Recipe Action
        [Authorize]
        [HttpGet]
        public ActionResult UploadRecipe()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Recipe POST Action
        [Authorize]
        [HttpPost]
        public ActionResult UploadRecipe(Recipe recipe, FormCollection formCollection)
        {
            string message = "";
            bool ingredientsAdded = false;
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        ViewBag.Message = "You cannot upload recipes until you verify your account!";
                        return View();
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    bool canUpload = false;
                                    if (ModelState.IsValid)
                                    {
                                        if (recipe != null)
                                        {
                                            if (de.Recipes.Count() < 1)
                                            {
                                                canUpload = true;
                                            }
                                            else
                                            {
                                                foreach (var recp in de.Recipes)
                                                {
                                                    if (recp.RecipeName.Equals(recipe.RecipeName))
                                                    {
                                                        canUpload = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        canUpload = true;
                                                    }
                                                }
                                            }

                                            if (canUpload == true)
                                            {
                                                de.Recipes.Add(recipe);
                                                de.SaveChanges();
                                                var recp = de.Recipes.Where(a => a.RecipeName.Equals(recipe.RecipeName)).FirstOrDefault();
                                                if (recp != null)
                                                {
                                                    if (formCollection != null)
                                                    {
                                                        foreach (var checkbox in formCollection)
                                                        {
                                                            if (checkbox != null)
                                                            {
                                                                string isChecked = formCollection[checkbox.ToString()];
                                                                if (isChecked == "true,false") // This means the checkbox is selected, "false" means it's not selected.
                                                                {
                                                                    Ingredient ingr = de.Ingredients.Where(a => a.IngredientName.Equals(checkbox.ToString())).FirstOrDefault();
                                                                    if (ingr != null)
                                                                    {
                                                                        RecipesIngredient recpIngr = new RecipesIngredient();
                                                                        recpIngr.Recipe = recp;
                                                                        recpIngr.Ingredient = ingr;
                                                                        recp.RecipesIngredients.Add(recpIngr);
                                                                        de.SaveChanges();
                                                                        message = "Recipe uploaded successfully!";
                                                                        ingredientsAdded = true;
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (ingredientsAdded == false)
                                                        {
                                                            message = "You must select at least one ingredient!";
                                                            de.Recipes.Remove(recipe);
                                                            de.SaveChanges();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                message = "This recipe is already present in the database!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message = "Invalid Request!";
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    ModelState.Clear();
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Pizza Action
        [Authorize]
        [HttpGet]
        public ActionResult UploadPizza()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Pizza POST Action
        [Authorize]
        [HttpPost]
        public ActionResult UploadPizza(Pizza pizza, FormCollection formCollection, HttpPostedFileBase Image)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        ViewBag.Message = "You cannot upload pizzas until you verify your account!";
                        return View();
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    bool canUpload = false;

                                    string selectedRecipeString = formCollection["RecipesList"];
                                    Recipe selectedRecipe = de.Recipes.Where(a => a.RecipeName.Equals(selectedRecipeString)).FirstOrDefault();
                                    double ingredientsPrice = 0.0;

                                    if ((selectedRecipe == null) && (formCollection["SizesList"] == "") && (Image == null))
                                    {
                                        ViewBag.Message = "Pizza recipe, pizza size and pizza image are required";
                                        return View();
                                    }
                                    else if ((selectedRecipe == null) && (formCollection["SizesList"] == "") && (Image != null))
                                    {
                                        ViewBag.Message = "Pizza recipe and pizza size are required";
                                        return View();
                                    }
                                    else if ((selectedRecipe == null) && (formCollection["SizesList"] != "") && (Image == null))
                                    {
                                        ViewBag.Message = "Pizza recipe and pizza image are required";
                                        return View();
                                    }
                                    else if ((selectedRecipe != null) && (formCollection["SizesList"] == "") && (Image == null))
                                    {
                                        ViewBag.Message = "Pizza size and pizza image are required";
                                        return View();
                                    }
                                    else if ((selectedRecipe == null) && (formCollection["SizesList"] != "") && (Image != null))
                                    {
                                        ViewBag.Message = "Pizza recipe is required";
                                        return View();
                                    }
                                    else if ((selectedRecipe != null) && (formCollection["SizesList"] == "") && (Image != null))
                                    {
                                        ViewBag.Message = "Pizza size is required";
                                        return View();
                                    }
                                    else if ((selectedRecipe != null) && (formCollection["SizesList"] != "") && (Image == null))
                                    {
                                        ViewBag.Message = "Pizza image is required";
                                        return View();
                                    }

                                    // Check if the image file is of a supported extension.
                                    string ext = Path.GetExtension(Image.FileName);
                                    if ((ext.Equals(".xbm")) || (ext.Equals(".bmp")) || (ext.Equals(".jpeg")) || (ext.Equals(".webp")) || (ext.Equals(".svgz")) || (ext.Equals(".gif")) || (ext.Equals(".jfif")) || (ext.Equals(".png")) || (ext.Equals(".svg")) || (ext.Equals(".jpg")) || (ext.Equals(".ico")) || (ext.Equals(".tiff")) || (ext.Equals(".pjpeg")) || (ext.Equals(".pjp")) || (ext.Equals(".tif")))
                                    {
                                        if (selectedRecipe != null)
                                        {
                                            int selectedRecipeID = selectedRecipe.RecipeID;
                                            pizza.Recipe = selectedRecipe;
                                            var recipesIngredientsForRecipe = pizza.Recipe.RecipesIngredients.Where(a => a.RecipeID == selectedRecipeID);
                                            foreach (var item in recipesIngredientsForRecipe)
                                            {
                                                ingredientsPrice += item.Ingredient.IngredientPrice;
                                            }
                                        }

                                        if (formCollection["SizesList"] != "")
                                        {
                                            pizza.PizzaSize = formCollection["SizesList"];
                                        }

                                        // Pizza price = Ingredients Price + 50% of ingredients price (overcharge, salaries) + size increase.
                                        if (pizza.PizzaSize.Equals("Small (450g)"))
                                        {
                                            double price = (ingredientsPrice + 0.5 * ingredientsPrice);
                                            pizza.PizzaPrice = Math.Round(price, 2);
                                        }
                                        else if (pizza.PizzaSize.Equals("Medium (700g)"))
                                        {
                                            double price = (ingredientsPrice + 0.5 * ingredientsPrice) + (ingredientsPrice + 0.5 * ingredientsPrice) * 0.2;
                                            pizza.PizzaPrice = Math.Round(price, 2);
                                        }
                                        else if (pizza.PizzaSize.Equals("Large (950g)"))
                                        {
                                            double price = (ingredientsPrice + 0.5 * ingredientsPrice) + (ingredientsPrice + 0.5 * ingredientsPrice) * 0.4;
                                            pizza.PizzaPrice = Math.Round(price, 2);
                                        }

                                        pizza.PizzaPicturePath = ""; // We make the path be empty because we just need it to be initialized and later we'll set the real value.

                                        if (de.Pizzas.Count() < 1)
                                        {
                                            canUpload = true;
                                        }
                                        else
                                        {
                                            foreach (var item in de.Pizzas)
                                            {
                                                if (item.PizzaName.Equals(pizza.PizzaName) && (item.PizzaSize.Equals(pizza.PizzaSize)))
                                                {
                                                    canUpload = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    canUpload = true;
                                                }
                                            }
                                        }

                                        if (canUpload == true)
                                        {
                                            if (ModelState.IsValid)
                                            {
                                                de.Pizzas.Add(pizza);
                                                de.SaveChanges();
                                                string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                                                string extension = Path.GetExtension(Image.FileName);
                                                // We get the actual uploaded pizza's id and set it as value for the path attribute
                                                int pizzaIndex = pizza.PizzaID;
                                                pizza.PizzaPicturePath = "/Uploads/Pizzas/" + pizzaIndex + extension; // Pizza File Name == ID (Unique!)
                                                Image.SaveAs(Server.MapPath(pizza.PizzaPicturePath));
                                                de.SaveChanges();
                                                message = "Pizza uploaded successfully!";
                                            }
                                            else
                                            {
                                                message = "Invalid Request!";
                                            }
                                        }
                                        else
                                        {
                                            message = "This pizza is already present in the database!";
                                        }
                                    }
                                    else
                                    {
                                        message = "The selected image is not of a supported file format! " + ext;
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    ModelState.Clear();
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Drink Action
        [Authorize]
        [HttpGet]
        public ActionResult UploadDrink()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Drink POST Action
        [Authorize]
        [HttpPost]
        public ActionResult UploadDrink(Drink drink, FormCollection formCollection, HttpPostedFileBase Image)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        ViewBag.Message = "You cannot upload drinks until you verify your account!";
                        return View();
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    bool canUpload = false;

                                    if ((formCollection["SizesList"] == "") && (Image == null))
                                    {
                                        ViewBag.Message = "Drink size and drink image are required";
                                        return View();
                                    }else if ((formCollection["SizesList"] == "") && (Image != null))
                                    {
                                        ViewBag.Message = "Drink size is required";
                                        return View();
                                    }else if ((formCollection["SizesList"] != "") && (Image == null))
                                    {
                                        ViewBag.Message = "Drink image is required";
                                        return View();
                                    }

                                    // Check if the image file is of a supported extension.
                                    string ext = Path.GetExtension(Image.FileName);
                                    if ((ext.Equals(".xbm")) || (ext.Equals(".bmp")) || (ext.Equals(".jpeg")) || (ext.Equals(".webp")) || (ext.Equals(".svgz")) || (ext.Equals(".gif")) || (ext.Equals(".jfif")) || (ext.Equals(".png")) || (ext.Equals(".svg")) || (ext.Equals(".jpg")) || (ext.Equals(".ico")) || (ext.Equals(".tiff")) || (ext.Equals(".pjpeg")) || (ext.Equals(".pjp")) || (ext.Equals(".tif")))
                                    {
                                        if (formCollection["SizesList"] != "")
                                        {
                                            drink.DrinkSize = formCollection["SizesList"];
                                        }

                                        drink.DrinkPicturePath = ""; // We make the path be empty because we just need it to be initialized and later we'll set the real value.

                                        if (de.Drinks.Count() < 1)
                                        {
                                            canUpload = true;
                                        }
                                        else
                                        {
                                            foreach (var item in de.Drinks)
                                            {
                                                if (item.DrinkName.Equals(drink.DrinkName) && (item.DrinkSize.Equals(drink.DrinkSize)))
                                                {
                                                    canUpload = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    canUpload = true;
                                                }
                                            }
                                        }

                                        if (canUpload == true)
                                        {
                                            if (ModelState.IsValid)
                                            {
                                                drink.DrinkPrice = Double.Parse(formCollection["DrinkPriceInput"].Replace(".", ","));
                                                de.Drinks.Add(drink);
                                                de.SaveChanges();
                                                string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                                                string extension = Path.GetExtension(Image.FileName);
                                                // We get the actual uploaded drink's id and set it as value for the path attribute
                                                int drinkIndex = drink.DrinkID;
                                                drink.DrinkPicturePath = "/Uploads/Drinks/" + drinkIndex + extension; // Drink File Name == ID (Unique!)
                                                Image.SaveAs(Server.MapPath(drink.DrinkPicturePath));
                                                de.SaveChanges();
                                                message = "Drink uploaded successfully!";
                                            }
                                            else
                                            {
                                                message = "Invalid Request!";
                                            }
                                        }
                                        else
                                        {
                                            message = "This drink is already present in the database!";
                                        }
                                    }
                                    else
                                    {
                                        message = "The selected image is not of a supported file format! " + ext;
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    ModelState.Clear();
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Dessert Action
        [Authorize]
        [HttpGet]
        public ActionResult UploadDessert()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Dessert POST Action
        [Authorize]
        [HttpPost]
        public ActionResult UploadDessert(Dessert dessert, FormCollection formCollection, HttpPostedFileBase Image)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        ViewBag.Message = "You cannot upload desserts until you verify your account!";
                        return View();
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    bool canUpload = false;

                                    if ((formCollection["SizesList"] == "") && (Image == null))
                                    {
                                        ViewBag.Message = "Dessert size and dessert image are required";
                                        return View();
                                    }
                                    else if ((formCollection["SizesList"] == "") && (Image != null))
                                    {
                                        ViewBag.Message = "Dessert size is required";
                                        return View();
                                    }
                                    else if ((formCollection["SizesList"] != "") && (Image == null))
                                    {
                                        ViewBag.Message = "Dessert image is required";
                                        return View();
                                    }

                                    // Check if the image file is of a supported extension.
                                    string ext = Path.GetExtension(Image.FileName);
                                    if ((ext.Equals(".xbm")) || (ext.Equals(".bmp")) || (ext.Equals(".jpeg")) || (ext.Equals(".webp")) || (ext.Equals(".svgz")) || (ext.Equals(".gif")) || (ext.Equals(".jfif")) || (ext.Equals(".png")) || (ext.Equals(".svg")) || (ext.Equals(".jpg")) || (ext.Equals(".ico")) || (ext.Equals(".tiff")) || (ext.Equals(".pjpeg")) || (ext.Equals(".pjp")) || (ext.Equals(".tif")))
                                    {
                                        if (formCollection["SizesList"] != "")
                                        {
                                            dessert.DessertSize = formCollection["SizesList"];
                                        }

                                        dessert.DessertPicturePath = ""; // We make the path be empty because we just need it to be initialized and later we'll set the real value.

                                        if (de.Desserts.Count() < 1)
                                        {
                                            canUpload = true;
                                        }
                                        else
                                        {
                                            foreach (var item in de.Desserts)
                                            {
                                                if (item.DessertName.Equals(dessert.DessertName) && (item.DessertSize.Equals(dessert.DessertSize)))
                                                {
                                                    canUpload = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    canUpload = true;
                                                }
                                            }
                                        }

                                        if (canUpload == true)
                                        {
                                            if (ModelState.IsValid)
                                            {
                                                dessert.DessertPrice = Double.Parse(formCollection["DessertPriceInput"].Replace(".", ","));
                                                de.Desserts.Add(dessert);
                                                de.SaveChanges();
                                                string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                                                string extension = Path.GetExtension(Image.FileName);
                                                // We get the actual uploaded drink's id and set it as value for the path attribute
                                                int dessertIndex = dessert.DessertID;
                                                dessert.DessertPicturePath = "/Uploads/Desserts/" + dessertIndex + extension; // Dessert File Name == ID (Unique!)
                                                Image.SaveAs(Server.MapPath(dessert.DessertPicturePath));
                                                de.SaveChanges();
                                                message = "Dessert uploaded successfully!";
                                            }
                                            else
                                            {
                                                message = "Invalid Request!";
                                            }
                                        }
                                        else
                                        {
                                            message = "This dessert is already present in the database!";
                                        }
                                    }
                                    else
                                    {
                                        message = "The selected image is not of a supported file format! " + ext;
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    ModelState.Clear();
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Manage Users Action
        [Authorize]
        [HttpGet]
        public ActionResult ManageUsers()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                var allUsers = de.Users.ToList();
                                ViewBag.Message = TempData["giveAdminText"];
                                ViewBag.Message2 = TempData["revokeAdminText"];
                                ViewBag.Message3 = TempData["deleteUserText"];
                                return View(allUsers.OrderByDescending(a => a.EmailID));
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Give Admin POST Action
        [Authorize]
        [HttpPost]
        public ActionResult GiveAdmin(FormCollection formCollection)
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        TempData["giveAdminText"] = "You cannot give admin access until you verify your account!";
                        return RedirectToAction("ManageUsers", "Admin");
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["UsersList1"] != "")
                                    {
                                        string userEmail = formCollection["UsersList1"].ToString();
                                        User selectedUser = de.Users.Where(a => a.EmailID == userEmail).FirstOrDefault();
                                        if (selectedUser != null)
                                        {
                                            var selectedUserRole = selectedUser.UsersRoles.FirstOrDefault();
                                            Role role = de.Roles.Where(a => a.RoleName.Equals("Admin")).FirstOrDefault();
                                            selectedUserRole.User = selectedUser;
                                            selectedUserRole.Role = role;
                                            selectedUser.UsersRoles.Add(selectedUserRole);
                                            de.SaveChanges();
                                            TempData["giveAdminText"] = "User with email '" + userEmail + "' was given admin access!";
                                        }
                                    }
                                    else
                                    {
                                        TempData["giveAdminText"] = "Please select a user to which to give admin access to first!";
                                    }
                                }
                            }
                        }
                    }

                    return RedirectToAction("ManageUsers", "Admin");
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Revoke Admin POST Action
        [Authorize]
        [HttpPost]
        public ActionResult RevokeAdmin(FormCollection formCollection)
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        TempData["revokeAdminText"] = "You cannot revoke admin access until you verify your account!";
                        return RedirectToAction("ManageUsers", "Admin");
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["UsersList2"] != "")
                                    {
                                        string userEmail = formCollection["UsersList2"].ToString();
                                        User selectedUser = de.Users.Where(a => a.EmailID == userEmail).FirstOrDefault();
                                        if (selectedUser != null)
                                        {
                                            var selectedUserRole = selectedUser.UsersRoles.FirstOrDefault();
                                            Role role = de.Roles.Where(a => a.RoleName.Equals("User")).FirstOrDefault();
                                            selectedUserRole.User = selectedUser;
                                            selectedUserRole.Role = role;
                                            selectedUser.UsersRoles.Add(selectedUserRole);
                                            de.SaveChanges();
                                            TempData["revokeAdminText"] = "User with email '" + userEmail + "' was revoked admin access!";
                                        }
                                    }
                                    else
                                    {
                                        TempData["revokeAdminText"] = "Please select a user from which to revoke admin access first!";
                                    }
                                }
                            }
                        }
                    }

                    return RedirectToAction("ManageUsers", "Admin");
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete User POST Action
        [Authorize]
        [HttpPost]
        public ActionResult DeleteUser(FormCollection formCollection)
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        TempData["deleteUserText"] = "You cannot delete users until you verify your account!";
                        return RedirectToAction("ManageUsers", "Admin");
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["UsersList3"] != "")
                                    {
                                        string userEmail = formCollection["UsersList3"].ToString();
                                        User selectedUser = de.Users.Where(a => a.EmailID == userEmail).FirstOrDefault();
                                        if (selectedUser != null)
                                        {
                                            var userOrders = de.Orders.Where(a => a.UserID == selectedUser.UserID);
                                            foreach (var order in userOrders)
                                            {
                                                if (order != null)
                                                {
                                                    de.Orders.Remove(order);
                                                }
                                            }
                                            var userRoleToDelete = de.UsersRoles.Where(a => a.UserID == selectedUser.UserID).FirstOrDefault();
                                            de.UsersRoles.Remove(userRoleToDelete);
                                            de.Users.Remove(selectedUser);
                                            de.SaveChanges();
                                            TempData["deleteUserText"] = "User with email '" + userEmail + "' was deleted!";
                                        }
                                    }
                                    else
                                    {
                                        TempData["deleteUserText"] = "Please select a user which to delete first!";
                                    }
                                }
                            }
                        }
                    }

                    return RedirectToAction("ManageUsers", "Admin");
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // View User Order History Action
        [Authorize]
        [HttpGet]
        public ActionResult UserOrderHistory(int id)
        {
            string message = "";
            List<OrderHistory> orderHistoryList = new List<OrderHistory>();
            double totalOrderPrice = 0.0;
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        ViewBag.Message = "You cannot view user's orders until you verify your account!";
                        var ordersGroupedByTimestamp1 = orderHistoryList.OrderBy(item => item.Timestamp).GroupBy(item => item.Timestamp);
                        GroupedOrderHistoryModel groupedOrderHistoryModel1 = new GroupedOrderHistoryModel();
                        groupedOrderHistoryModel1.groupedOrderHistoryList = ordersGroupedByTimestamp1;
                        return View(groupedOrderHistoryModel1);
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    var currentUser = de.Users.Where(a => a.UserID == id).FirstOrDefault();
                                    var orderHistoryByCurrentUser = de.OrderHistories.Where(a => a.UserID == currentUser.UserID);
                                    foreach (var orderHistory in orderHistoryByCurrentUser)
                                    {
                                        orderHistoryList.Add(orderHistory);

                                        if (orderHistory.PizzaName != null)
                                        {
                                            if (!ViewData.ContainsKey("pizzaPicturePath" + orderHistory.OrderHistoryID))
                                            {
                                                if (System.IO.File.Exists(@"" + Server.MapPath(orderHistory.PizzaPicturePath)))
                                                {
                                                    ViewData.Add("pizzaPicturePath" + orderHistory.OrderHistoryID, orderHistory.PizzaPicturePath);
                                                }
                                                else
                                                {
                                                    bool foundOtherPizza = false;
                                                    string otherPizzaImage = "";
                                                    foreach (var pizza in de.Pizzas)
                                                    {
                                                        if (orderHistory.PizzaName.StartsWith("Custom: "))
                                                        {
                                                            if (("Custom: " + pizza.PizzaName).Equals(orderHistory.PizzaName))
                                                            {
                                                                foundOtherPizza = true;
                                                                otherPizzaImage = pizza.PizzaPicturePath;
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (pizza.PizzaName.Equals(orderHistory.PizzaName) && pizza.PizzaSize.Equals(orderHistory.PizzaSize))
                                                            {
                                                                foundOtherPizza = true;
                                                                otherPizzaImage = pizza.PizzaPicturePath;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    if (foundOtherPizza)
                                                    {
                                                        ViewData.Add("pizzaPicturePath" + orderHistory.OrderHistoryID, otherPizzaImage);
                                                    }
                                                    else
                                                    {
                                                        ViewData.Add("pizzaPicturePath" + orderHistory.OrderHistoryID, "/Design/Unavailable.png");
                                                    }
                                                }
                                            }

                                            if (!ViewData.ContainsKey("pizzaName" + orderHistory.OrderHistoryID))
                                            {
                                                ViewData.Add("pizzaName" + orderHistory.OrderHistoryID, orderHistory.PizzaName);
                                            }

                                            if (!ViewData.ContainsKey("pizzaSize" + orderHistory.OrderHistoryID))
                                            {
                                                ViewData.Add("pizzaSize" + orderHistory.OrderHistoryID, orderHistory.PizzaSize);
                                            }

                                            if (!ViewData.ContainsKey("pizzaPrice" + orderHistory.OrderHistoryID))
                                            {
                                                ViewData.Add("pizzaPrice" + orderHistory.OrderHistoryID, orderHistory.PizzaPrice);
                                            }
                                            totalOrderPrice += orderHistory.PizzaPrice.GetValueOrDefault() * orderHistory.PizzaAmount.GetValueOrDefault();
                                        }

                                        if (orderHistory.DrinkName != null)
                                        {
                                            if (!ViewData.ContainsKey("drinkPicturePath" + orderHistory.OrderHistoryID))
                                            {
                                                if (System.IO.File.Exists(@"" + Server.MapPath(orderHistory.DrinkPicturePath)))
                                                {
                                                    ViewData.Add("drinkPicturePath" + orderHistory.OrderHistoryID, orderHistory.DrinkPicturePath);
                                                }
                                                else
                                                {
                                                    bool foundOtherDrink = false;
                                                    string otherDrinkImage = "";
                                                    foreach (var drink in de.Drinks)
                                                    {
                                                        if (drink.DrinkName.Equals(orderHistory.DrinkName) && drink.DrinkSize.Equals(orderHistory.DrinkSize))
                                                        {
                                                            foundOtherDrink = true;
                                                            otherDrinkImage = drink.DrinkPicturePath;
                                                            break;
                                                        }
                                                    }
                                                    if (foundOtherDrink)
                                                    {
                                                        ViewData.Add("drinkPicturePath" + orderHistory.OrderHistoryID, otherDrinkImage);
                                                    }
                                                    else
                                                    {
                                                        ViewData.Add("drinkPicturePath" + orderHistory.OrderHistoryID, "/Design/Unavailable.png");
                                                    }
                                                }
                                            }

                                            if (!ViewData.ContainsKey("drinkName" + orderHistory.OrderHistoryID))
                                            {
                                                ViewData.Add("drinkName" + orderHistory.OrderHistoryID, orderHistory.DrinkName);
                                            }

                                            if (!ViewData.ContainsKey("drinkSize" + orderHistory.OrderHistoryID))
                                            {
                                                ViewData.Add("drinkSize" + orderHistory.OrderHistoryID, orderHistory.DrinkSize);
                                            }

                                            if (!ViewData.ContainsKey("drinkPrice" + orderHistory.OrderHistoryID))
                                            {
                                                ViewData.Add("drinkPrice" + orderHistory.OrderHistoryID, orderHistory.DrinkPrice);
                                            }
                                            totalOrderPrice += orderHistory.DrinkPrice.GetValueOrDefault() * orderHistory.DrinkAmount.GetValueOrDefault();
                                        }

                                        if (orderHistory.DessertName != null)
                                        {
                                            if (!ViewData.ContainsKey("dessertPicturePath" + orderHistory.OrderHistoryID))
                                            {
                                                if (System.IO.File.Exists(@"" + Server.MapPath(orderHistory.DessertPicturePath)))
                                                {
                                                    ViewData.Add("dessertPicturePath" + orderHistory.OrderHistoryID, orderHistory.DessertPicturePath);
                                                }
                                                else
                                                {
                                                    bool foundOtherDessert = false;
                                                    string otherDessertImage = "";
                                                    foreach (var dessert in de.Desserts)
                                                    {
                                                        if (dessert.DessertName.Equals(orderHistory.DessertName) && dessert.DessertSize.Equals(orderHistory.DessertSize))
                                                        {
                                                            foundOtherDessert = true;
                                                            otherDessertImage = dessert.DessertPicturePath;
                                                            break;
                                                        }
                                                    }
                                                    if (foundOtherDessert)
                                                    {
                                                        ViewData.Add("dessertPicturePath" + orderHistory.OrderHistoryID, otherDessertImage);
                                                    }
                                                    else
                                                    {
                                                        ViewData.Add("dessertPicturePath" + orderHistory.OrderHistoryID, "/Design/Unavailable.png");
                                                    }
                                                }
                                            }

                                            if (!ViewData.ContainsKey("dessertName" + orderHistory.OrderHistoryID))
                                            {
                                                ViewData.Add("dessertName" + orderHistory.OrderHistoryID, orderHistory.DessertName);
                                            }

                                            if (!ViewData.ContainsKey("dessertSize" + orderHistory.OrderHistoryID))
                                            {
                                                ViewData.Add("dessertSize" + orderHistory.OrderHistoryID, orderHistory.DessertSize);
                                            }

                                            if (!ViewData.ContainsKey("dessertPrice" + orderHistory.OrderHistoryID))
                                            {
                                                ViewData.Add("dessertPrice" + orderHistory.OrderHistoryID, orderHistory.DessertPrice);
                                            }
                                            totalOrderPrice += orderHistory.DessertPrice.GetValueOrDefault() * orderHistory.DessertAmount.GetValueOrDefault();
                                        }
                                    }

                                    if (!ViewData.ContainsKey("totalOrderPrice"))
                                    {
                                        ViewData.Add("totalOrderPrice", totalOrderPrice);
                                    }

                                    int numberOfOrders = 0;
                                    foreach (var item in orderHistoryList.OrderBy(item => item.Timestamp).GroupBy(item => item.Timestamp))
                                    {
                                        numberOfOrders++;
                                    }

                                    if (numberOfOrders == 1)
                                    {
                                        message = "There is " + numberOfOrders.ToString() + " order in " + currentUser.EmailID + "'s order history!";
                                    }
                                    else if (numberOfOrders > 1)
                                    {
                                        message = "There are " + numberOfOrders.ToString() + " orders in " + currentUser.EmailID + "'s order history!";
                                    }
                                    else
                                    {
                                        message = currentUser.EmailID + "'s order history is empty!";
                                    }
                                }
                            }
                        }

                        var ordersGroupedByTimestamp = orderHistoryList.OrderBy(item => item.Timestamp).GroupBy(item => item.Timestamp).OrderByDescending(item => DateTime.Parse(item.Key));
                        GroupedOrderHistoryModel groupedOrderHistoryModel = new GroupedOrderHistoryModel();
                        groupedOrderHistoryModel.groupedOrderHistoryList = ordersGroupedByTimestamp;

                        ViewBag.Message = message;
                        return View(groupedOrderHistoryModel);
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // View Statistics Action
        [Authorize]
        [HttpGet]
        public ActionResult ViewStatistics()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                DateTimePickerModel dateTimePickerModel = new DateTimePickerModel();
                                GroupedOrderHistoryModel groupedOrderHistoryModel = new GroupedOrderHistoryModel();
                                List<OrderHistory> orderHistoryList = new List<OrderHistory>();
                                var ordersGroupedByTimestamp = orderHistoryList.OrderByDescending(item => item.Timestamp).GroupBy(item => item.Timestamp);
                                groupedOrderHistoryModel.groupedOrderHistoryList = ordersGroupedByTimestamp;
                                var dateTimePickerAndGroupedOrderHistoryModel = new DateTimePickerAndGroupedOrderHistoryModel();
                                dateTimePickerAndGroupedOrderHistoryModel.dateTimePickerModel = dateTimePickerModel;
                                dateTimePickerAndGroupedOrderHistoryModel.groupedOrderHistoryModel = groupedOrderHistoryModel;
                                return View(dateTimePickerAndGroupedOrderHistoryModel);
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // View Statistics POST Action
        [Authorize]
        [HttpPost]
        public ActionResult ViewStatistics(DateTimePickerAndGroupedOrderHistoryModel model, FormCollection formCollection)
        {
            string message = "";
            List<OrderHistory> orderHistoryList = new List<OrderHistory>();
            string inputedStartDate = model.dateTimePickerModel.DateChooserStart.ToString("dd.MM.yyyy HH:mm:ss").Substring(0, 10);
            string inputedEndDate = model.dateTimePickerModel.DateChooserEnd.ToString("dd.MM.yyyy HH:mm:ss").Substring(0, 10);
            string inputedStartTime = model.dateTimePickerModel.TimeChooserStart.ToString("dd.MM.yyyy HH:mm:ss").Substring(11, 5) + ":00";
            string inputedEndTime = model.dateTimePickerModel.TimeChooserEnd.ToString("dd.MM.yyyy HH:mm:ss").Substring(11, 5) + ":00";
            var dateComparison = DateTime.Compare(DateTime.Parse(model.dateTimePickerModel.DateChooserEnd.ToString("dd.MM.yyyy HH:mm:ss"), new CultureInfo("bg-BG")), DateTime.Parse(model.dateTimePickerModel.DateChooserStart.ToString("dd.MM.yyyy HH:mm:ss"), new CultureInfo("bg-BG")));
            var timeComparison = DateTime.Compare(DateTime.Parse(model.dateTimePickerModel.TimeChooserEnd.ToString("dd.MM.yyyy HH:mm:ss"), new CultureInfo("bg-BG")), DateTime.Parse(model.dateTimePickerModel.TimeChooserStart.ToString("dd.MM.yyyy HH:mm:ss"), new CultureInfo("bg-BG")));

            if (dateComparison < 0)
            {
                message = "End date cannot be before start date!";
                ViewBag.Message = message;
                DateTimePickerModel dateTimePickerModel1 = new DateTimePickerModel();
                GroupedOrderHistoryModel groupedOrderHistoryModel1 = new GroupedOrderHistoryModel();
                List<OrderHistory> orderHistoryList1 = new List<OrderHistory>();
                var ordersGroupedByTimestamp1 = orderHistoryList1.OrderBy(item => item.Timestamp).GroupBy(item => item.Timestamp);
                groupedOrderHistoryModel1.groupedOrderHistoryList = ordersGroupedByTimestamp1;
                var dateTimePickerAndGroupedOrderHistoryModel1 = new DateTimePickerAndGroupedOrderHistoryModel();
                dateTimePickerAndGroupedOrderHistoryModel1.dateTimePickerModel = dateTimePickerModel1;
                dateTimePickerAndGroupedOrderHistoryModel1.groupedOrderHistoryModel = groupedOrderHistoryModel1;
                return View(dateTimePickerAndGroupedOrderHistoryModel1);
            }
            else if (dateComparison == 0)
            {
                // https://docs.microsoft.com/en-us/dotnet/api/system.datetime.compare?view=netcore-3.1
                if (timeComparison < 0)
                {
                    message = "End time cannot be before start time!";
                    ViewBag.Message = message;
                    DateTimePickerModel dateTimePickerModel2 = new DateTimePickerModel();
                    GroupedOrderHistoryModel groupedOrderHistoryModel2 = new GroupedOrderHistoryModel();
                    List<OrderHistory> orderHistoryList2 = new List<OrderHistory>();
                    var ordersGroupedByTimestamp2 = orderHistoryList2.OrderBy(item => item.Timestamp).GroupBy(item => item.Timestamp);
                    groupedOrderHistoryModel2.groupedOrderHistoryList = ordersGroupedByTimestamp2;
                    var dateTimePickerAndGroupedOrderHistoryModel2 = new DateTimePickerAndGroupedOrderHistoryModel();
                    dateTimePickerAndGroupedOrderHistoryModel2.dateTimePickerModel = dateTimePickerModel2;
                    dateTimePickerAndGroupedOrderHistoryModel2.groupedOrderHistoryModel = groupedOrderHistoryModel2;
                    return View(dateTimePickerAndGroupedOrderHistoryModel2);
                }
                using (DBEntities de = new DBEntities())
                {
                    var selectedOrders = de.OrderHistories.Where(a => a.Timestamp.Substring(0, 10) == DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss").Substring(0, 10));
                    if (timeComparison == 0)
                    {
                        selectedOrders = de.OrderHistories.Where(a => a.Timestamp.Substring(0, 10) == inputedStartDate);
                        if (selectedOrders.Count() > 0)
                        {
                            ViewBag.OrdersDateText = "All orders on " + inputedStartDate + " (" + DateTime.Parse(inputedStartDate).DayOfWeek.ToString() + ")";
                        }
                    }
                    else if (timeComparison > 0)
                    {
                        // https://docs.microsoft.com/en-us/dotnet/api/system.string.compare?view=netcore-3.1
                        selectedOrders = de.OrderHistories.Where(a => a.Timestamp.Substring(0, 10) == inputedStartDate && String.Compare(a.Timestamp.Substring(11, 8), inputedStartTime) >= 0 && String.Compare(a.Timestamp.Substring(11, 8), inputedEndTime) <= 0);

                        if (selectedOrders.Count() > 0)
                        {
                            ViewBag.OrdersDateText = "All orders on " + inputedStartDate + " (" + DateTime.Parse(inputedStartDate).DayOfWeek.ToString() + ") between " + inputedStartTime + " and " + inputedEndTime;
                        }
                    }
                    int soldPizzas = 0;
                    int soldDrinks = 0;
                    int soldDesserts = 0;
                    double totalIncome = 0.0;
                    foreach (var order in selectedOrders)
                    {
                        orderHistoryList.Add(order);

                        if (order.PizzaName != null)
                        {
                            if (!ViewData.ContainsKey("pizzaPicturePath" + order.OrderHistoryID))
                            {
                                if (System.IO.File.Exists(@"" + Server.MapPath(order.PizzaPicturePath)))
                                {
                                    ViewData.Add("pizzaPicturePath" + order.OrderHistoryID, order.PizzaPicturePath);
                                }
                                else
                                {
                                    bool foundOtherPizza = false;
                                    string otherPizzaImage = "";
                                    foreach (var pizza in de.Pizzas)
                                    {
                                        if (order.PizzaName.StartsWith("Custom: "))
                                        {
                                            if (("Custom: " + pizza.PizzaName).Equals(order.PizzaName))
                                            {
                                                foundOtherPizza = true;
                                                otherPizzaImage = pizza.PizzaPicturePath;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (pizza.PizzaName.Equals(order.PizzaName) && pizza.PizzaSize.Equals(order.PizzaSize))
                                            {
                                                foundOtherPizza = true;
                                                otherPizzaImage = pizza.PizzaPicturePath;
                                                break;
                                            }
                                        }
                                    }
                                    if (foundOtherPizza)
                                    {
                                        ViewData.Add("pizzaPicturePath" + order.OrderHistoryID, otherPizzaImage);
                                    }
                                    else
                                    {
                                        ViewData.Add("pizzaPicturePath" + order.OrderHistoryID, "/Design/Unavailable.png");
                                    }
                                }
                            }

                            if (!ViewData.ContainsKey("pizzaName" + order.OrderHistoryID))
                            {
                                ViewData.Add("pizzaName" + order.OrderHistoryID, order.PizzaName);
                            }

                            if (!ViewData.ContainsKey("pizzaSize" + order.OrderHistoryID))
                            {
                                ViewData.Add("pizzaSize" + order.OrderHistoryID, order.PizzaSize);
                            }

                            if (!ViewData.ContainsKey("pizzaPrice" + order.OrderHistoryID))
                            {
                                ViewData.Add("pizzaPrice" + order.OrderHistoryID, order.PizzaPrice);
                            }

                            totalIncome += order.PizzaPrice.GetValueOrDefault() * order.PizzaAmount.GetValueOrDefault();
                            soldPizzas += order.PizzaAmount.GetValueOrDefault();
                        }

                        if (order.DrinkName != null)
                        {
                            if (!ViewData.ContainsKey("drinkPicturePath" + order.OrderHistoryID))
                            {
                                if (System.IO.File.Exists(@"" + Server.MapPath(order.DrinkPicturePath)))
                                {
                                    ViewData.Add("drinkPicturePath" + order.OrderHistoryID, order.DrinkPicturePath);
                                }
                                else
                                {
                                    bool foundOtherDrink = false;
                                    string otherDrinkImage = "";
                                    foreach (var drink in de.Drinks)
                                    {
                                        if (drink.DrinkName.Equals(order.DrinkName) && drink.DrinkSize.Equals(order.DrinkSize))
                                        {
                                            foundOtherDrink = true;
                                            otherDrinkImage = drink.DrinkPicturePath;
                                            break;
                                        }
                                    }
                                    if (foundOtherDrink)
                                    {
                                        ViewData.Add("drinkPicturePath" + order.OrderHistoryID, otherDrinkImage);
                                    }
                                    else
                                    {
                                        ViewData.Add("drinkPicturePath" + order.OrderHistoryID, "/Design/Unavailable.png");
                                    }
                                }
                            }

                            if (!ViewData.ContainsKey("drinkName" + order.OrderHistoryID))
                            {
                                ViewData.Add("drinkName" + order.OrderHistoryID, order.DrinkName);
                            }

                            if (!ViewData.ContainsKey("drinkSize" + order.OrderHistoryID))
                            {
                                ViewData.Add("drinkSize" + order.OrderHistoryID, order.DrinkSize);
                            }

                            if (!ViewData.ContainsKey("drinkPrice" + order.OrderHistoryID))
                            {
                                ViewData.Add("drinkPrice" + order.OrderHistoryID, order.DrinkPrice);
                            }

                            totalIncome += order.DrinkPrice.GetValueOrDefault() * order.DrinkAmount.GetValueOrDefault();
                            soldDrinks += order.DrinkAmount.GetValueOrDefault();
                        }

                        if (order.DessertName != null)
                        {
                            if (!ViewData.ContainsKey("dessertPicturePath" + order.OrderHistoryID))
                            {
                                if (System.IO.File.Exists(@"" + Server.MapPath(order.DessertPicturePath)))
                                {
                                    ViewData.Add("dessertPicturePath" + order.OrderHistoryID, order.DessertPicturePath);
                                }
                                else
                                {
                                    bool foundOtherDessert = false;
                                    string otherDessertImage = "";
                                    foreach (var dessert in de.Desserts)
                                    {
                                        if (dessert.DessertName.Equals(order.DessertName) && dessert.DessertSize.Equals(order.DessertSize))
                                        {
                                            foundOtherDessert = true;
                                            otherDessertImage = dessert.DessertPicturePath;
                                            break;
                                        }
                                    }
                                    if (foundOtherDessert)
                                    {
                                        ViewData.Add("dessertPicturePath" + order.OrderHistoryID, otherDessertImage);
                                    }
                                    else
                                    {
                                        ViewData.Add("dessertPicturePath" + order.OrderHistoryID, "/Design/Unavailable.png");
                                    }
                                }
                            }

                            if (!ViewData.ContainsKey("dessertName" + order.OrderHistoryID))
                            {
                                ViewData.Add("dessertName" + order.OrderHistoryID, order.DessertName);
                            }

                            if (!ViewData.ContainsKey("dessertSize" + order.OrderHistoryID))
                            {
                                ViewData.Add("dessertSize" + order.OrderHistoryID, order.DessertSize);
                            }

                            if (!ViewData.ContainsKey("dessertPrice" + order.OrderHistoryID))
                            {
                                ViewData.Add("dessertPrice" + order.OrderHistoryID, order.DessertPrice);
                            }

                            totalIncome += order.DessertPrice.GetValueOrDefault() * order.DessertAmount.GetValueOrDefault();
                            soldDesserts += order.DessertAmount.GetValueOrDefault();
                        }
                    }

                    if (selectedOrders.Count() > 0)
                    {
                        ViewBag.TotalPizzas = soldPizzas.ToString();
                        ViewBag.TotalDrinks = soldDrinks.ToString();
                        ViewBag.TotalDesserts = soldDesserts.ToString();
                        ViewBag.TotalIncome = "Total income: " + String.Format("{0:0.00}", totalIncome) + " BGN";
                        ViewBag.StartDate = inputedStartDate;
                        ViewBag.EndDate = null;
                        ViewBag.StartTime = inputedStartTime;
                        ViewBag.EndTime = inputedEndTime;
                    }

                    if (!ViewData.ContainsKey("totalIncome"))
                    {
                        ViewData.Add("totalIncome", totalIncome);
                    }

                    if (selectedOrders.Count() == 1)
                    {
                        message = "There is " + selectedOrders.Count().ToString() + " order in the selected interval!";
                    }
                    else if (selectedOrders.Count() > 1)
                    {
                        message = "There are " + selectedOrders.Count().ToString() + " orders in the selected interval!";
                    }
                    else
                    {
                        message = "There are no orders in the selected interval!";
                    }
                }

                DateTimePickerModel dateTimePickerModel3 = new DateTimePickerModel();
                GroupedOrderHistoryModel groupedOrderHistoryModel3 = new GroupedOrderHistoryModel();
                var ordersGroupedByTimestamp3 = orderHistoryList.OrderBy(item => item.Timestamp).GroupBy(item => item.Timestamp).OrderByDescending(a => DateTime.Parse(a.Key));
                groupedOrderHistoryModel3.groupedOrderHistoryList = ordersGroupedByTimestamp3;
                var dateTimePickerAndGroupedOrderHistoryModel3 = new DateTimePickerAndGroupedOrderHistoryModel();
                dateTimePickerAndGroupedOrderHistoryModel3.dateTimePickerModel = dateTimePickerModel3;
                dateTimePickerAndGroupedOrderHistoryModel3.groupedOrderHistoryModel = groupedOrderHistoryModel3;
                ViewBag.Message = message;
                return View(dateTimePickerAndGroupedOrderHistoryModel3);
            }
            else if (dateComparison > 0)
            {
                using (DBEntities de = new DBEntities())
                {
                    var selectedOrders = de.OrderHistories.ToList().Where(a => DateTime.Parse(a.Timestamp) >= DateTime.Parse(inputedStartDate + " " + inputedStartTime) && DateTime.Parse(a.Timestamp.Substring(0, 10)) <= DateTime.Parse(inputedEndDate + " " + inputedEndTime));

                    if (selectedOrders.Count() > 0)
                    {
                        ViewBag.OrdersDateText = "All orders between " + inputedStartDate + " (" + DateTime.Parse(inputedStartDate).DayOfWeek.ToString() + ") at " + inputedStartTime + " and " + inputedEndDate + " (" + DateTime.Parse(inputedEndDate).DayOfWeek.ToString() + ") at " + inputedEndTime;
                    }
                    int soldPizzas = 0;
                    int soldDrinks = 0;
                    int soldDesserts = 0;
                    double totalIncome = 0.0;
                    foreach (var order in selectedOrders)
                    {
                        orderHistoryList.Add(order);

                        if (order.PizzaName != null)
                        {
                            if (!ViewData.ContainsKey("pizzaPicturePath" + order.OrderHistoryID))
                            {
                                if (System.IO.File.Exists(@"" + Server.MapPath(order.PizzaPicturePath)))
                                {
                                    ViewData.Add("pizzaPicturePath" + order.OrderHistoryID, order.PizzaPicturePath);
                                }
                                else
                                {
                                    bool foundOtherPizza = false;
                                    string otherPizzaImage = "";
                                    foreach (var pizza in de.Pizzas)
                                    {
                                        if (order.PizzaName.StartsWith("Custom: "))
                                        {
                                            if (("Custom: " + pizza.PizzaName).Equals(order.PizzaName))
                                            {
                                                foundOtherPizza = true;
                                                otherPizzaImage = pizza.PizzaPicturePath;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (pizza.PizzaName.Equals(order.PizzaName) && pizza.PizzaSize.Equals(order.PizzaSize))
                                            {
                                                foundOtherPizza = true;
                                                otherPizzaImage = pizza.PizzaPicturePath;
                                                break;
                                            }
                                        }
                                    }
                                    if (foundOtherPizza)
                                    {
                                        ViewData.Add("pizzaPicturePath" + order.OrderHistoryID, otherPizzaImage);
                                    }
                                    else
                                    {
                                        ViewData.Add("pizzaPicturePath" + order.OrderHistoryID, "/Design/Unavailable.png");
                                    }
                                }
                            }

                            if (!ViewData.ContainsKey("pizzaName" + order.OrderHistoryID))
                            {
                                ViewData.Add("pizzaName" + order.OrderHistoryID, order.PizzaName);
                            }

                            if (!ViewData.ContainsKey("pizzaSize" + order.OrderHistoryID))
                            {
                                ViewData.Add("pizzaSize" + order.OrderHistoryID, order.PizzaSize);
                            }

                            if (!ViewData.ContainsKey("pizzaPrice" + order.OrderHistoryID))
                            {
                                ViewData.Add("pizzaPrice" + order.OrderHistoryID, order.PizzaPrice);
                            }

                            totalIncome += order.PizzaPrice.GetValueOrDefault() * order.PizzaAmount.GetValueOrDefault();
                            soldPizzas += order.PizzaAmount.GetValueOrDefault();
                        }

                        if (order.DrinkName != null)
                        {
                            if (!ViewData.ContainsKey("drinkPicturePath" + order.OrderHistoryID))
                            {
                                if (System.IO.File.Exists(@"" + Server.MapPath(order.DrinkPicturePath)))
                                {
                                    ViewData.Add("drinkPicturePath" + order.OrderHistoryID, order.DrinkPicturePath);
                                }
                                else
                                {
                                    bool foundOtherDrink = false;
                                    string otherDrinkImage = "";
                                    foreach (var drink in de.Drinks)
                                    {
                                        if (drink.DrinkName.Equals(order.DrinkName) && drink.DrinkSize.Equals(order.DrinkSize))
                                        {
                                            foundOtherDrink = true;
                                            otherDrinkImage = drink.DrinkPicturePath;
                                            break;
                                        }
                                    }
                                    if (foundOtherDrink)
                                    {
                                        ViewData.Add("drinkPicturePath" + order.OrderHistoryID, otherDrinkImage);
                                    }
                                    else
                                    {
                                        ViewData.Add("drinkPicturePath" + order.OrderHistoryID, "/Design/Unavailable.png");
                                    }
                                }
                            }

                            if (!ViewData.ContainsKey("drinkName" + order.OrderHistoryID))
                            {
                                ViewData.Add("drinkName" + order.OrderHistoryID, order.DrinkName);
                            }

                            if (!ViewData.ContainsKey("drinkSize" + order.OrderHistoryID))
                            {
                                ViewData.Add("drinkSize" + order.OrderHistoryID, order.DrinkSize);
                            }

                            if (!ViewData.ContainsKey("drinkPrice" + order.OrderHistoryID))
                            {
                                ViewData.Add("drinkPrice" + order.OrderHistoryID, order.DrinkPrice);
                            }

                            totalIncome += order.DrinkPrice.GetValueOrDefault() * order.DrinkAmount.GetValueOrDefault();
                            soldDrinks += order.DrinkAmount.GetValueOrDefault();
                        }

                        if (order.DessertName != null)
                        {
                            if (!ViewData.ContainsKey("dessertPicturePath" + order.OrderHistoryID))
                            {
                                if (System.IO.File.Exists(@"" + Server.MapPath(order.DessertPicturePath)))
                                {
                                    ViewData.Add("dessertPicturePath" + order.OrderHistoryID, order.DessertPicturePath);
                                }
                                else
                                {
                                    bool foundOtherDessert = false;
                                    string otherDessertImage = "";
                                    foreach (var dessert in de.Desserts)
                                    {
                                        if (dessert.DessertName.Equals(order.DessertName) && dessert.DessertSize.Equals(order.DessertSize))
                                        {
                                            foundOtherDessert = true;
                                            otherDessertImage = dessert.DessertPicturePath;
                                            break;
                                        }
                                    }
                                    if (foundOtherDessert)
                                    {
                                        ViewData.Add("dessertPicturePath" + order.OrderHistoryID, otherDessertImage);
                                    }
                                    else
                                    {
                                        ViewData.Add("dessertPicturePath" + order.OrderHistoryID, "/Design/Unavailable.png");
                                    }
                                }
                            }

                            if (!ViewData.ContainsKey("dessertName" + order.OrderHistoryID))
                            {
                                ViewData.Add("dessertName" + order.OrderHistoryID, order.DessertName);
                            }

                            if (!ViewData.ContainsKey("dessertSize" + order.OrderHistoryID))
                            {
                                ViewData.Add("dessertSize" + order.OrderHistoryID, order.DessertSize);
                            }

                            if (!ViewData.ContainsKey("dessertPrice" + order.OrderHistoryID))
                            {
                                ViewData.Add("dessertPrice" + order.OrderHistoryID, order.DessertPrice);
                            }

                            totalIncome += order.DessertPrice.GetValueOrDefault() * order.DessertAmount.GetValueOrDefault();
                            soldDesserts += order.DessertAmount.GetValueOrDefault();
                        }
                    }

                    if (selectedOrders.Count() > 0)
                    {
                        ViewBag.TotalPizzas = soldPizzas.ToString();
                        ViewBag.TotalDrinks = soldDrinks.ToString();
                        ViewBag.TotalDesserts = soldDesserts.ToString();
                        ViewBag.TotalIncome = "Total income: " + String.Format("{0:0.00}", totalIncome) + " BGN";
                        ViewBag.StartDate = inputedStartDate;
                        ViewBag.EndDate = inputedEndDate;
                        ViewBag.StartTime = inputedStartTime;
                        ViewBag.EndTime = inputedEndTime;
                    }

                    if (!ViewData.ContainsKey("totalIncome"))
                    {
                        ViewData.Add("totalIncome", totalIncome);
                    }

                    if (selectedOrders.Count() == 1)
                    {
                        message = "There is " + selectedOrders.Count().ToString() + " order in the selected interval!";
                    }
                    else if (selectedOrders.Count() > 1)
                    {
                        message = "There are " + selectedOrders.Count().ToString() + " orders in the selected interval!";
                    }
                    else
                    {
                        message = "There are no orders in the selected interval!";
                    }

                    DateTimePickerModel dateTimePickerModel4 = new DateTimePickerModel();
                    GroupedOrderHistoryModel groupedOrderHistoryModel4 = new GroupedOrderHistoryModel();
                    var ordersGroupedByTimestamp4 = orderHistoryList.OrderBy(item => item.Timestamp).GroupBy(item => item.Timestamp).OrderByDescending(a => DateTime.Parse(a.Key));
                    groupedOrderHistoryModel4.groupedOrderHistoryList = ordersGroupedByTimestamp4;
                    var dateTimePickerAndGroupedOrderHistoryModel4 = new DateTimePickerAndGroupedOrderHistoryModel();
                    dateTimePickerAndGroupedOrderHistoryModel4.dateTimePickerModel = dateTimePickerModel4;
                    dateTimePickerAndGroupedOrderHistoryModel4.groupedOrderHistoryModel = groupedOrderHistoryModel4;
                    ViewBag.Message = message;
                    return View(dateTimePickerAndGroupedOrderHistoryModel4);
                }
            }

            DateTimePickerModel dateTimePickerModel5 = new DateTimePickerModel();
            GroupedOrderHistoryModel groupedOrderHistoryModel5 = new GroupedOrderHistoryModel();
            var ordersGroupedByTimestamp5 = orderHistoryList.OrderBy(item => item.Timestamp).GroupBy(item => item.Timestamp);
            groupedOrderHistoryModel5.groupedOrderHistoryList = ordersGroupedByTimestamp5;
            var dateTimePickerAndGroupedOrderHistoryModel5 = new DateTimePickerAndGroupedOrderHistoryModel();
            dateTimePickerAndGroupedOrderHistoryModel5.dateTimePickerModel = dateTimePickerModel5;
            dateTimePickerAndGroupedOrderHistoryModel5.groupedOrderHistoryModel = groupedOrderHistoryModel5;
            ViewBag.Message = message;
            return View(dateTimePickerAndGroupedOrderHistoryModel5);
        }
        #endregion

        #region // View Total Pizzas Sold Action
        [Authorize]
        [HttpGet]
        public ActionResult ViewTotalPizzasSold(string startDate, string startTime, string endTime, string endDate = null)
        {
            string message = "";
            string pizzasText = "";
            double totalIncome = 0.0;
            int totalPizzasSold = 0;
            List<Pizza> pizzasList = new List<Pizza>();
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                var allOrdersInInterval = de.OrderHistories.ToList().Where(a => a.Timestamp.Substring(0, 10) == startDate);
                                if (endDate == null)
                                {
                                    allOrdersInInterval = de.OrderHistories.Where(a => a.Timestamp.Substring(0, 10) == startDate);
                                    var comparison = DateTime.Compare(DateTime.Parse(endTime), DateTime.Parse(startTime));
                                    // https://docs.microsoft.com/en-us/dotnet/api/system.datetime.compare?view=netcore-3.1
                                    if (comparison < 0)
                                    {
                                        message = "End time cannot be before start time!";
                                        ViewBag.Message = message;
                                        return View();
                                    }
                                    else if (comparison == 0)
                                    {
                                        allOrdersInInterval = de.OrderHistories.Where(a => a.Timestamp.Substring(0, 10) == startDate);
                                        pizzasText = "All pizzas sold on " + startDate + " (" + DateTime.Parse(startDate).DayOfWeek.ToString() + ")";
                                    }
                                    else if (comparison > 0)
                                    {
                                        allOrdersInInterval = de.OrderHistories.Where(a => a.Timestamp.Substring(0, 10) == startDate && String.Compare(a.Timestamp.Substring(11, 8), startTime) >= 0 && String.Compare(a.Timestamp.Substring(11, 8), endTime) <= 0);
                                        pizzasText = "All pizzas sold on " + startDate + " (" + DateTime.Parse(startDate).DayOfWeek.ToString() + ") between " + startTime + " and " + endTime;
                                    }
                                }
                                else
                                {
                                    var comparison = DateTime.Compare(DateTime.Parse(endDate), DateTime.Parse(startDate));
                                    if (comparison < 0)
                                    {
                                        message = "End date cannot be before start date!";
                                        ViewBag.Message = message;
                                        return View();
                                    }
                                    else
                                    {
                                        allOrdersInInterval = de.OrderHistories.ToList().Where(a => DateTime.Parse(a.Timestamp) >= DateTime.Parse(startDate + " " + startTime) && DateTime.Parse(a.Timestamp) <= DateTime.Parse(endDate + " " + endTime));
                                        pizzasText = "All pizzas sold between " + startDate + " (" + DateTime.Parse(startDate).DayOfWeek.ToString() + ") at " + startTime + " and " + endDate + " (" + DateTime.Parse(endDate).DayOfWeek.ToString() + ") at " + endTime;
                                    }
                                }

                                foreach (var pizza in de.Pizzas)
                                {
                                    int pizzaCount = 0;
                                    double pizzasTotalPrice = 0;
                                    foreach (var order in allOrdersInInterval)
                                    {
                                        if (order.PizzaName != null)
                                        {
                                            if (order.PizzaName.Equals(pizza.PizzaName) || order.PizzaName.Equals("Custom: " + pizza.PizzaName))
                                            {
                                                bool canAddPizza = true;
                                                if (!pizzasList.Contains(pizza))
                                                {
                                                    foreach (var item in pizzasList)
                                                    {
                                                        if (item.PizzaName.Equals(pizza.PizzaName) || ("Custom: " + item.PizzaName).Equals(pizza.PizzaName))
                                                        {
                                                            canAddPizza = false;
                                                            break;
                                                        }
                                                    }
                                                    if (canAddPizza)
                                                    {
                                                        pizzasList.Add(pizza);
                                                    }
                                                }
                                                pizzaCount += order.PizzaAmount.GetValueOrDefault();
                                                if (canAddPizza)
                                                {
                                                    totalPizzasSold += order.PizzaAmount.GetValueOrDefault();
                                                    totalIncome += order.PizzaPrice.GetValueOrDefault() * order.PizzaAmount.GetValueOrDefault();
                                                }
                                                pizzasTotalPrice += order.PizzaPrice.GetValueOrDefault() * order.PizzaAmount.GetValueOrDefault();
                                            }
                                        }
                                    }
                                    if (!ViewData.ContainsKey("pizzaCount" + pizza.PizzaID))
                                    {
                                        ViewData.Add("pizzaCount" + pizza.PizzaID, pizzaCount);
                                    }
                                    if (!ViewData.ContainsKey("pizzasTotalPrice" + pizza.PizzaID))
                                    {
                                        ViewData.Add("pizzasTotalPrice" + pizza.PizzaID, pizzasTotalPrice);
                                    }
                                }

                                // https://stackoverflow.com/questions/3944803/use-linq-to-get-items-in-one-list-that-are-not-in-another-list
                                var ordersLeft = allOrdersInInterval.Where(a => pizzasList.All(b => b.PizzaID != a.PizzaID));
                                List<Pizza> leftoversList = new List<Pizza>();
                                if (ordersLeft.Count() > 0)
                                {
                                    int pizzaCount = 0;
                                    double pizzasTotalPrice = 0;
                                    var pizzaCountDict = new Dictionary<string, int>(); // 1: name, 2: value
                                    var pizzasTotalPriceDict = new Dictionary<string, double>(); // 1: name, 2: total price for all pizzas with name
                                    foreach (var order in ordersLeft)
                                    {
                                        if (order.PizzaName != null)
                                        {
                                            if (pizzasList.Count > 0)
                                            {
                                                foreach (var pizza in pizzasList.ToList())
                                                {
                                                    // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.exists?view=netcore-3.1
                                                    if (!pizzasList.Exists(a => a.PizzaName.Equals(order.PizzaName) || ("Custom: " + a.PizzaName).Equals(order.PizzaName)))
                                                    {
                                                        if (!leftoversList.Exists(a => a.PizzaName.Equals(order.PizzaName) || ("Custom: " + a.PizzaName).Equals(order.PizzaName)))
                                                        {
                                                            Pizza ghostPizza = new Pizza();
                                                            ghostPizza.PizzaID = order.PizzaID.GetValueOrDefault();
                                                            ghostPizza.PizzaName = order.PizzaName.StartsWith("Custom: ") ? order.PizzaName.Substring(8) : order.PizzaName;
                                                            ghostPizza.PizzaPicturePath = "/Design/Unavailable.png";
                                                            ghostPizza.PizzaPrice = order.PizzaPrice.GetValueOrDefault();
                                                            ghostPizza.PizzaSize = order.PizzaSize;
                                                            leftoversList.Add(ghostPizza);
                                                            pizzaCount += order.PizzaAmount.GetValueOrDefault();
                                                            pizzasTotalPrice += order.PizzaPrice.GetValueOrDefault() * order.PizzaAmount.GetValueOrDefault();
                                                            totalPizzasSold += order.PizzaAmount.GetValueOrDefault();
                                                            totalIncome += order.PizzaPrice.GetValueOrDefault() * order.PizzaAmount.GetValueOrDefault();
                                                        }
                                                        else
                                                        {
                                                            pizzaCount += order.PizzaAmount.GetValueOrDefault();
                                                            pizzasTotalPrice += order.PizzaPrice.GetValueOrDefault() * order.PizzaAmount.GetValueOrDefault();
                                                            totalPizzasSold += order.PizzaAmount.GetValueOrDefault();
                                                            totalIncome += order.PizzaPrice.GetValueOrDefault() * order.PizzaAmount.GetValueOrDefault();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (!leftoversList.Exists(a => a.PizzaName.Equals(order.PizzaName) || ("Custom: " + a.PizzaName).Equals(order.PizzaName)))
                                                {
                                                    Pizza ghostPizza = new Pizza();
                                                    ghostPizza.PizzaID = order.PizzaID.GetValueOrDefault();
                                                    ghostPizza.PizzaName = order.PizzaName.StartsWith("Custom: ") ? order.PizzaName.Substring(8) : order.PizzaName;
                                                    ghostPizza.PizzaPicturePath = "/Design/Unavailable.png";
                                                    ghostPizza.PizzaPrice = order.PizzaPrice.GetValueOrDefault();
                                                    ghostPizza.PizzaSize = order.PizzaSize;
                                                    leftoversList.Add(ghostPizza);
                                                    pizzaCount += order.PizzaAmount.GetValueOrDefault();
                                                    pizzasTotalPrice += order.PizzaPrice.GetValueOrDefault() * order.PizzaAmount.GetValueOrDefault();
                                                    totalPizzasSold += order.PizzaAmount.GetValueOrDefault();
                                                    totalIncome += order.PizzaPrice.GetValueOrDefault() * order.PizzaAmount.GetValueOrDefault();
                                                }
                                                else
                                                {
                                                    pizzaCount += order.PizzaAmount.GetValueOrDefault();
                                                    pizzasTotalPrice += order.PizzaPrice.GetValueOrDefault() * order.PizzaAmount.GetValueOrDefault();
                                                    totalPizzasSold += order.PizzaAmount.GetValueOrDefault();
                                                    totalIncome += order.PizzaPrice.GetValueOrDefault() * order.PizzaAmount.GetValueOrDefault();
                                                }
                                            }
                                            if (pizzaCountDict.ContainsKey(order.PizzaName.StartsWith("Custom: ") ? order.PizzaName.Substring(8) : order.PizzaName))
                                            {
                                                pizzaCountDict.TryGetValue(order.PizzaName.StartsWith("Custom: ") ? order.PizzaName.Substring(8) : order.PizzaName, out var value);
                                                pizzaCountDict[order.PizzaName.StartsWith("Custom: ") ? order.PizzaName.Substring(8) : order.PizzaName] = value + pizzaCount;
                                            }
                                            else
                                            {
                                                pizzaCountDict.Add(order.PizzaName.StartsWith("Custom: ") ? order.PizzaName.Substring(8) : order.PizzaName, pizzaCount);
                                            }
                                            if (pizzasTotalPriceDict.ContainsKey(order.PizzaName.StartsWith("Custom: ") ? order.PizzaName.Substring(8) : order.PizzaName))
                                            {
                                                pizzasTotalPriceDict.TryGetValue(order.PizzaName.StartsWith("Custom: ") ? order.PizzaName.Substring(8) : order.PizzaName, out var value);
                                                pizzasTotalPriceDict[order.PizzaName.StartsWith("Custom: ") ? order.PizzaName.Substring(8) : order.PizzaName] = value + pizzasTotalPrice;
                                            }
                                            else
                                            {
                                                pizzasTotalPriceDict.Add(order.PizzaName.StartsWith("Custom: ") ? order.PizzaName.Substring(8) : order.PizzaName, pizzasTotalPrice);
                                            }
                                            pizzaCount = 0;
                                            pizzasTotalPrice = 0;
                                        }
                                    }
                                    foreach (var item in pizzaCountDict)
                                    {
                                        if (!ViewData.ContainsKey("pizzaCount" + item.Key))
                                        {
                                            ViewData.Add("pizzaCount" + item.Key, item.Value);
                                        }
                                        else
                                        {
                                            ViewData["pizzaCount" + item.Key] = Convert.ToInt32(ViewData["pizzaCount" + item.Key]) + item.Value;
                                        }
                                    }
                                    foreach (var item in pizzasTotalPriceDict)
                                    {
                                        if (!ViewData.ContainsKey("pizzasTotalPrice" + item.Key))
                                        {
                                            ViewData.Add("pizzasTotalPrice" + item.Key, item.Value);
                                        }
                                        else
                                        {
                                            ViewData["pizzasTotalPrice" + item.Key] = Convert.ToInt32(ViewData["pizzasTotalPrice" + item.Key]) + item.Value;
                                        }
                                    }
                                    pizzasList.AddRange(leftoversList.Where(a => pizzasList.All(b => b.PizzaName != a.PizzaName)));
                                }

                                if (totalPizzasSold > 0)
                                {
                                    ViewBag.Message2 = "Total pizzas sold: " + totalPizzasSold;
                                }

                                if (totalIncome > 0)
                                {
                                    ViewBag.Message3 = "Total income: " + String.Format("{0:0.00}", totalIncome) + " BGN";
                                }

                                if (pizzasList.Count() > 0)
                                {
                                    ViewBag.OrdersDateText = pizzasText;
                                }
                                else
                                {
                                    ViewBag.OrdersDateText = "There are no pizzas in the selected interval!";
                                    pizzasList = new List<Pizza>();
                                }

                                return View(pizzasList.OrderBy(a => a.PizzaName));
                            }
                        }
                    }
                }

                return RedirectToAction("AccessDenied", "Admin");
            }
        }
        #endregion

        #region // View Total Drinks Sold Action
        [Authorize]
        [HttpGet]
        public ActionResult ViewTotalDrinksSold(string startDate, string startTime, string endTime, string endDate = null)
        {
            string message = "";
            string drinksText = "";
            double totalIncome = 0.0;
            int totalDrinksSold = 0;
            List<Drink> drinksList = new List<Drink>();
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                var allOrdersInInterval = de.OrderHistories.ToList().Where(a => a.Timestamp.Substring(0, 10) == startDate);
                                if (endDate == null)
                                {
                                    var comparison = DateTime.Compare(DateTime.Parse(endTime), DateTime.Parse(startTime));
                                    // https://docs.microsoft.com/en-us/dotnet/api/system.datetime.compare?view=netcore-3.1
                                    if (comparison < 0)
                                    {
                                        message = "End time cannot be before start time!";
                                        ViewBag.Message = message;
                                        return View();
                                    }
                                    else if (comparison == 0)
                                    {
                                        allOrdersInInterval = de.OrderHistories.Where(a => a.Timestamp.Substring(0, 10) == startDate);
                                        drinksText = "All drinks sold on " + startDate + " (" + DateTime.Parse(startDate).DayOfWeek.ToString() + ")";
                                    }
                                    else if (comparison > 0)
                                    {
                                        allOrdersInInterval = de.OrderHistories.Where(a => a.Timestamp.Substring(0, 10) == startDate && String.Compare(a.Timestamp.Substring(11, 8), startTime) >= 0 && String.Compare(a.Timestamp.Substring(11, 8), endTime) <= 0);
                                        drinksText = "All drinks sold on " + startDate + " (" + DateTime.Parse(startDate).DayOfWeek.ToString() + ") between " + startTime + " and " + endTime;
                                    }
                                }
                                else
                                {
                                    var comparison = DateTime.Compare(DateTime.Parse(endDate), DateTime.Parse(startDate));
                                    if (comparison < 0)
                                    {
                                        message = "End date cannot be before start date!";
                                        ViewBag.Message = message;
                                        return View();
                                    }
                                    else
                                    {
                                        allOrdersInInterval = de.OrderHistories.ToList().Where(a => DateTime.Parse(a.Timestamp) >= DateTime.Parse(startDate + " " + startTime) && DateTime.Parse(a.Timestamp) <= DateTime.Parse(endDate + " " + endTime));
                                        drinksText = "All drinks sold between " + startDate + " (" + DateTime.Parse(startDate).DayOfWeek.ToString() + ") at " + startTime + " and " + endDate + " (" + DateTime.Parse(endDate).DayOfWeek.ToString() + ") at " + endTime;
                                    }
                                }

                                foreach (var drink in de.Drinks)
                                {
                                    int drinkCount = 0;
                                    double drinksTotalPrice = 0;
                                    foreach (var order in allOrdersInInterval)
                                    {
                                        if (order.DrinkName != null)
                                        {
                                            if (order.DrinkName.Equals(drink.DrinkName))
                                            {
                                                bool canAddDrink = true;
                                                if (!drinksList.Contains(drink))
                                                {
                                                    foreach (var item in drinksList)
                                                    {
                                                        if (item.DrinkName.Equals(drink.DrinkName))
                                                        {
                                                            canAddDrink = false;
                                                            break;
                                                        }
                                                    }
                                                    if (canAddDrink)
                                                    {
                                                        drinksList.Add(drink);
                                                    }
                                                }
                                                drinkCount += order.DrinkAmount.GetValueOrDefault();
                                                if (canAddDrink)
                                                {
                                                    totalDrinksSold += order.DrinkAmount.GetValueOrDefault();
                                                    totalIncome += order.DrinkPrice.GetValueOrDefault() * order.DrinkAmount.GetValueOrDefault();
                                                }
                                                drinksTotalPrice += order.DrinkPrice.GetValueOrDefault() * order.DrinkAmount.GetValueOrDefault();
                                            }
                                        }
                                    }
                                    if (!ViewData.ContainsKey("drinkCount" + drink.DrinkID))
                                    {
                                        ViewData.Add("drinkCount" + drink.DrinkID, drinkCount);
                                    }
                                    if (!ViewData.ContainsKey("drinksTotalPrice" + drink.DrinkID))
                                    {
                                        ViewData.Add("drinksTotalPrice" + drink.DrinkID, drinksTotalPrice);
                                    }
                                }

                                // https://stackoverflow.com/questions/3944803/use-linq-to-get-items-in-one-list-that-are-not-in-another-list
                                var ordersLeft = allOrdersInInterval.Where(a => drinksList.All(b => b.DrinkID != a.DrinkID));
                                List<Drink> leftoversList = new List<Drink>();
                                if (ordersLeft.Count() > 0)
                                {
                                    int drinkCount = 0;
                                    double drinksTotalPrice = 0;
                                    var drinkCountDict = new Dictionary<string, int>(); // 1: name, 2: value
                                    var drinksTotalPriceDict = new Dictionary<string, double>(); // 1: name, 2: total price for all pizzas with name
                                    foreach (var order in ordersLeft)
                                    {
                                        if (order.DrinkName != null)
                                        {
                                            if (drinksList.Count > 0)
                                            {
                                                foreach (var drink in drinksList.ToList())
                                                {
                                                    // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.exists?view=netcore-3.1
                                                    if (!drinksList.Exists(a => a.DrinkName.Equals(order.DrinkName)))
                                                    {
                                                        if (!leftoversList.Exists(a => a.DrinkName.Equals(order.DrinkName)))
                                                        {
                                                            Drink ghostDrink = new Drink();
                                                            ghostDrink.DrinkID = order.DrinkID.GetValueOrDefault();
                                                            ghostDrink.DrinkName = order.DrinkName;
                                                            ghostDrink.DrinkPicturePath = "/Design/Unavailable.png";
                                                            ghostDrink.DrinkPrice = order.DrinkPrice.GetValueOrDefault();
                                                            ghostDrink.DrinkSize = order.DrinkSize;
                                                            leftoversList.Add(ghostDrink);
                                                            drinkCount += order.DrinkAmount.GetValueOrDefault();
                                                            drinksTotalPrice += order.DrinkPrice.GetValueOrDefault() * order.DrinkAmount.GetValueOrDefault();
                                                            totalDrinksSold += order.DrinkAmount.GetValueOrDefault();
                                                            totalIncome += order.DrinkPrice.GetValueOrDefault() * order.DrinkAmount.GetValueOrDefault();
                                                        }
                                                        else
                                                        {
                                                            drinkCount += order.DrinkAmount.GetValueOrDefault();
                                                            drinksTotalPrice += order.DrinkPrice.GetValueOrDefault() * order.DrinkAmount.GetValueOrDefault();
                                                            totalDrinksSold += order.DrinkAmount.GetValueOrDefault();
                                                            totalIncome += order.DrinkPrice.GetValueOrDefault() * order.DrinkAmount.GetValueOrDefault();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (!leftoversList.Exists(a => a.DrinkName.Equals(order.DrinkName)))
                                                {
                                                    Drink ghostDrink = new Drink();
                                                    ghostDrink.DrinkID = order.DrinkID.GetValueOrDefault();
                                                    ghostDrink.DrinkName = order.DrinkName;
                                                    ghostDrink.DrinkPicturePath = "/Design/Unavailable.png";
                                                    ghostDrink.DrinkPrice = order.DrinkPrice.GetValueOrDefault();
                                                    ghostDrink.DrinkSize = order.DrinkSize;
                                                    leftoversList.Add(ghostDrink);
                                                    drinkCount += order.DrinkAmount.GetValueOrDefault();
                                                    drinksTotalPrice += order.DrinkPrice.GetValueOrDefault() * order.DrinkAmount.GetValueOrDefault();
                                                    totalDrinksSold += order.DrinkAmount.GetValueOrDefault();
                                                    totalIncome += order.DrinkPrice.GetValueOrDefault() * order.DrinkAmount.GetValueOrDefault();
                                                }
                                                else
                                                {
                                                    drinkCount += order.DrinkAmount.GetValueOrDefault();
                                                    drinksTotalPrice += order.DrinkPrice.GetValueOrDefault() * order.DrinkAmount.GetValueOrDefault();
                                                    totalDrinksSold += order.DrinkAmount.GetValueOrDefault();
                                                    totalIncome += order.DrinkPrice.GetValueOrDefault() * order.DrinkAmount.GetValueOrDefault();
                                                }
                                            }
                                            if (drinkCountDict.ContainsKey(order.DrinkName))
                                            {
                                                drinkCountDict.TryGetValue(order.DrinkName, out var value);
                                                drinkCountDict[order.DrinkName] = value + drinkCount;
                                            }
                                            else
                                            {
                                                drinkCountDict.Add(order.DrinkName, drinkCount);
                                            }
                                            if (drinksTotalPriceDict.ContainsKey(order.DrinkName))
                                            {
                                                drinksTotalPriceDict.TryGetValue(order.DrinkName, out var value);
                                                drinksTotalPriceDict[order.DrinkName] = value + drinksTotalPrice;
                                            }
                                            else
                                            {
                                                drinksTotalPriceDict.Add(order.DrinkName, drinksTotalPrice);
                                            }
                                            drinkCount = 0;
                                            drinksTotalPrice = 0;
                                        }
                                    }
                                    foreach (var item in drinkCountDict)
                                    {
                                        if (!ViewData.ContainsKey("drinkCount" + item.Key))
                                        {
                                            ViewData.Add("drinkCount" + item.Key, item.Value);
                                        }
                                        else
                                        {
                                            ViewData["drinkCount" + item.Key] = Convert.ToInt32(ViewData["drinkCount" + item.Key]) + item.Value;
                                        }
                                    }
                                    foreach (var item in drinksTotalPriceDict)
                                    {
                                        if (!ViewData.ContainsKey("drinksTotalPrice" + item.Key))
                                        {
                                            ViewData.Add("drinksTotalPrice" + item.Key, item.Value);
                                        }
                                        else
                                        {
                                            ViewData["drinksTotalPrice" + item.Key] = Convert.ToInt32(ViewData["drinksTotalPrice" + item.Key]) + item.Value;
                                        }
                                    }
                                    drinksList.AddRange(leftoversList.Where(a => drinksList.All(b => b.DrinkName != a.DrinkName)));
                                }

                                if (totalDrinksSold > 0)
                                {
                                    ViewBag.Message2 = "Total drinks sold: " + totalDrinksSold;
                                }

                                if (totalIncome > 0)
                                {
                                    ViewBag.Message3 = "Total income: " + String.Format("{0:0.00}", totalIncome) + " BGN";
                                }

                                if (drinksList.Count() > 0)
                                {
                                    ViewBag.OrdersDateText = drinksText;
                                }
                                else
                                {
                                    ViewBag.OrdersDateText = "There are no drinks in the selected interval!";
                                    drinksList = new List<Drink>();
                                }

                                return View(drinksList.OrderBy(a => a.DrinkName));
                            }
                        }
                    }
                }

                return RedirectToAction("AccessDenied", "Admin");
            }
        }
        #endregion

        #region // View Total Desserts Sold Action
        [Authorize]
        [HttpGet]
        public ActionResult ViewTotalDessertsSold(string startDate, string startTime, string endTime, string endDate = null)
        {
            string message = "";
            string dessertsText = "";
            double totalIncome = 0.0;
            int totalDessertsSold = 0;
            List<Dessert> dessertsList = new List<Dessert>();
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                var allOrdersInInterval = de.OrderHistories.ToList().Where(a => a.Timestamp.Substring(0, 10) == startDate);
                                if (endDate == null)
                                {
                                    var comparison = DateTime.Compare(DateTime.Parse(endTime), DateTime.Parse(startTime));
                                    // https://docs.microsoft.com/en-us/dotnet/api/system.datetime.compare?view=netcore-3.1
                                    if (comparison < 0)
                                    {
                                        message = "End time cannot be before start time!";
                                        ViewBag.Message = message;
                                        return View();
                                    }
                                    else if (comparison == 0)
                                    {
                                        allOrdersInInterval = de.OrderHistories.Where(a => a.Timestamp.Substring(0, 10) == startDate);
                                        dessertsText = "All desserts sold on " + startDate + " (" + DateTime.Parse(startDate).DayOfWeek.ToString() + ")";
                                    }
                                    else if (comparison > 0)
                                    {
                                        allOrdersInInterval = de.OrderHistories.Where(a => a.Timestamp.Substring(0, 10) == startDate && String.Compare(a.Timestamp.Substring(11, 8), startTime) >= 0 && String.Compare(a.Timestamp.Substring(11, 8), endTime) <= 0);
                                        dessertsText = "All desserts sold on " + startDate + " (" + DateTime.Parse(startDate).DayOfWeek.ToString() + ") between " + startTime + " and " + endTime;
                                    }
                                }
                                else
                                {
                                    var comparison = DateTime.Compare(DateTime.Parse(endDate), DateTime.Parse(startDate));
                                    if (comparison < 0)
                                    {
                                        message = "End date cannot be before start date!";
                                        ViewBag.Message = message;
                                        return View();
                                    }
                                    else
                                    {
                                        allOrdersInInterval = de.OrderHistories.ToList().Where(a => DateTime.Parse(a.Timestamp) >= DateTime.Parse(startDate + " " + startTime) && DateTime.Parse(a.Timestamp) <= DateTime.Parse(endDate + " " + endTime));
                                        dessertsText = "All desserts sold between " + startDate + " (" + DateTime.Parse(startDate).DayOfWeek.ToString() + ") at " + startTime + " and " + endDate + " (" + DateTime.Parse(endDate).DayOfWeek.ToString() + ") at " + endTime;
                                    }
                                }

                                foreach (var dessert in de.Desserts)
                                {
                                    int dessertCount = 0;
                                    double dessertsTotalPrice = 0;
                                    foreach (var order in allOrdersInInterval)
                                    {
                                        if (order.DessertName != null)
                                        {
                                            if (order.DessertName.Equals(dessert.DessertName))
                                            {
                                                bool canAddDessert = true;
                                                if (!dessertsList.Contains(dessert))
                                                {
                                                    foreach (var item in dessertsList)
                                                    {
                                                        if (item.DessertName.Equals(dessert.DessertName))
                                                        {
                                                            canAddDessert = false;
                                                            break;
                                                        }
                                                    }
                                                    if (canAddDessert)
                                                    {
                                                        dessertsList.Add(dessert);
                                                    }

                                                }
                                                dessertCount += order.DessertAmount.GetValueOrDefault();
                                                if (canAddDessert)
                                                {
                                                    totalDessertsSold += order.DessertAmount.GetValueOrDefault();
                                                    totalIncome += order.DessertPrice.GetValueOrDefault() * order.DessertAmount.GetValueOrDefault();
                                                }
                                                dessertsTotalPrice += order.DessertPrice.GetValueOrDefault() * order.DessertAmount.GetValueOrDefault();
                                            }
                                        }
                                    }
                                    if (!ViewData.ContainsKey("dessertCount" + dessert.DessertID))
                                    {
                                        ViewData.Add("dessertCount" + dessert.DessertID, dessertCount);
                                    }
                                    if (!ViewData.ContainsKey("dessertsTotalPrice" + dessert.DessertID))
                                    {
                                        ViewData.Add("dessertsTotalPrice" + dessert.DessertID, dessertsTotalPrice);
                                    }
                                }

                                // https://stackoverflow.com/questions/3944803/use-linq-to-get-items-in-one-list-that-are-not-in-another-list
                                var ordersLeft = allOrdersInInterval.Where(a => dessertsList.All(b => b.DessertID != a.DessertID));
                                List<Dessert> leftoversList = new List<Dessert>();
                                if (ordersLeft.Count() > 0)
                                {
                                    int dessertCount = 0;
                                    double dessertsTotalPrice = 0;
                                    var dessertCountDict = new Dictionary<string, int>(); // 1: name, 2: value
                                    var dessertsTotalPriceDict = new Dictionary<string, double>(); // 1: name, 2: total price for all drinks with name
                                    foreach (var order in ordersLeft)
                                    {
                                        if (order.DessertName != null)
                                        {
                                            if (dessertsList.Count() > 0)
                                            {
                                                foreach (var dessert in dessertsList.ToList())
                                                {
                                                    // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.exists?view=netcore-3.1
                                                    if (!dessertsList.Exists(a => a.DessertName.Equals(order.DessertName)))
                                                    {
                                                        if (!leftoversList.Exists(a => a.DessertName.Equals(order.DessertName)))
                                                        {
                                                            Dessert ghostDessert = new Dessert();
                                                            ghostDessert.DessertID = order.DessertID.GetValueOrDefault();
                                                            ghostDessert.DessertName = order.DessertName;
                                                            ghostDessert.DessertPicturePath = "/Design/Unavailable.png";
                                                            ghostDessert.DessertPrice = order.DessertPrice.GetValueOrDefault();
                                                            ghostDessert.DessertSize = order.DessertSize;
                                                            leftoversList.Add(ghostDessert);
                                                            dessertCount += order.DessertAmount.GetValueOrDefault();
                                                            dessertsTotalPrice += order.DessertPrice.GetValueOrDefault() * order.DessertAmount.GetValueOrDefault();
                                                            totalDessertsSold += order.DessertAmount.GetValueOrDefault();
                                                            totalIncome += order.DessertPrice.GetValueOrDefault() * order.DessertAmount.GetValueOrDefault();
                                                        }
                                                        else
                                                        {
                                                            dessertCount += order.DessertAmount.GetValueOrDefault();
                                                            dessertsTotalPrice += order.DessertPrice.GetValueOrDefault() * order.DessertAmount.GetValueOrDefault();
                                                            totalDessertsSold += order.DessertAmount.GetValueOrDefault();
                                                            totalIncome += order.DessertPrice.GetValueOrDefault() * order.DessertAmount.GetValueOrDefault();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (!leftoversList.Exists(a => a.DessertName.Equals(order.DessertName)))
                                                {
                                                    Dessert ghostDessert = new Dessert();
                                                    ghostDessert.DessertID = order.DessertID.GetValueOrDefault();
                                                    ghostDessert.DessertName = order.DessertName;
                                                    ghostDessert.DessertPicturePath = "/Design/Unavailable.png";
                                                    ghostDessert.DessertPrice = order.DessertPrice.GetValueOrDefault();
                                                    ghostDessert.DessertSize = order.DessertSize;
                                                    leftoversList.Add(ghostDessert);
                                                    dessertCount += order.DessertAmount.GetValueOrDefault();
                                                    dessertsTotalPrice += order.DessertPrice.GetValueOrDefault() * order.DessertAmount.GetValueOrDefault();
                                                    totalDessertsSold += order.DessertAmount.GetValueOrDefault();
                                                    totalIncome += order.DessertPrice.GetValueOrDefault() * order.DessertAmount.GetValueOrDefault();
                                                }
                                                else
                                                {
                                                    dessertCount += order.DessertAmount.GetValueOrDefault();
                                                    dessertsTotalPrice += order.DessertPrice.GetValueOrDefault() * order.DessertAmount.GetValueOrDefault();
                                                    totalDessertsSold += order.DessertAmount.GetValueOrDefault();
                                                    totalIncome += order.DessertPrice.GetValueOrDefault() * order.DessertAmount.GetValueOrDefault();
                                                }
                                            }
                                            if (dessertCountDict.ContainsKey(order.DessertName))
                                            {
                                                dessertCountDict.TryGetValue(order.DessertName, out var value);
                                                dessertCountDict[order.DessertName] = value + dessertCount;
                                            }
                                            else
                                            {
                                                dessertCountDict.Add(order.DessertName, dessertCount);
                                            }
                                            if (dessertsTotalPriceDict.ContainsKey(order.DessertName))
                                            {
                                                dessertsTotalPriceDict.TryGetValue(order.DessertName, out var value);
                                                dessertsTotalPriceDict[order.DessertName] = value + dessertsTotalPrice;
                                            }
                                            else
                                            {
                                                dessertsTotalPriceDict.Add(order.DessertName, dessertsTotalPrice);
                                            }
                                            dessertCount = 0;
                                            dessertsTotalPrice = 0;
                                        }
                                    }
                                    foreach (var item in dessertCountDict)
                                    {
                                        if (!ViewData.ContainsKey("dessertCount" + item.Key))
                                        {
                                            ViewData.Add("dessertCount" + item.Key, item.Value);
                                        }
                                        else
                                        {
                                            ViewData["dessertCount" + item.Key] = Convert.ToInt32(ViewData["dessertCount" + item.Key]) + item.Value;
                                        }
                                    }
                                    foreach (var item in dessertsTotalPriceDict)
                                    {
                                        if (!ViewData.ContainsKey("dessertsTotalPrice" + item.Key))
                                        {
                                            ViewData.Add("dessertsTotalPrice" + item.Key, item.Value);
                                        }
                                        else
                                        {
                                            ViewData["dessertsTotalPrice" + item.Key] = Convert.ToInt32(ViewData["dessertsTotalPrice" + item.Key]) + item.Value;
                                        }
                                    }
                                    dessertsList.AddRange(leftoversList.Where(a => dessertsList.All(b => b.DessertName != a.DessertName)));
                                }

                                if (totalDessertsSold > 0)
                                {
                                    ViewBag.Message2 = "Total desserts sold: " + totalDessertsSold;
                                }

                                if (totalIncome > 0)
                                {
                                    ViewBag.Message3 = "Total income: " + String.Format("{0:0.00}", totalIncome) + " BGN";
                                }

                                if (dessertsList.Count() > 0)
                                {
                                    ViewBag.OrdersDateText = dessertsText;
                                }
                                else
                                {
                                    ViewBag.OrdersDateText = "There are no desserts in the selected interval!";
                                    dessertsList = new List<Dessert>();
                                }

                                return View(dessertsList.OrderBy(a => a.DessertName));
                            }
                        }
                    }
                }

                return RedirectToAction("AccessDenied", "Admin");
            }
        }
        #endregion

        #region // View Addresses Action
        [Authorize]
        [HttpGet]
        public ActionResult ViewAddresses()
        {
            string message = "";
            List <String> allAddresses = new List<String>();
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                foreach (var usr in de.Users)
                                {
                                    allAddresses.Add(usr.Address);
                                }
                                var dict = allAddresses.GroupBy(a => a).ToDictionary(b => b.Key, b => b.Count()).OrderByDescending(c => c.Value).ThenBy(d => d.Key);
                                if (dict.Count() > 0)
                                {
                                    message = "Delivery addresses";
                                }
                                else
                                {
                                    message = "There are no delivery addresses!";
                                }
                                ViewBag.Message = message;
                                return View(dict);
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Manage Promotions Action
        [Authorize]
        [HttpGet]
        public ActionResult ManagePromotions()
        {
            PromoCodesModel promoModel = new PromoCodesModel();
            List<Pizza> pizzasList = new List<Pizza>();
            List<Drink> drinksList = new List<Drink>();
            List<Dessert> dessertsList = new List<Dessert>();
            List<object> productsList = new List<object>();
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                foreach (var pizza in de.Pizzas)
                                {
                                    pizzasList.Add(pizza);
                                }

                                foreach (var drink in de.Drinks)
                                {
                                    drinksList.Add(drink);
                                }

                                foreach (var dessert in de.Desserts)
                                {
                                    dessertsList.Add(dessert);
                                }

                                ViewBag.Message = TempData["uploadPromoCodeText"];
                                ViewBag.Message2 = TempData["deletePromoCodeText"];
                                ViewBag.Message3 = TempData["managePromotionsText"];
                                if (de.PromotionsTexts.Count() > 0)
                                {
                                    ViewBag.PromoInputText = de.PromotionsTexts.FirstOrDefault().PromoText;
                                }
                                else
                                {
                                    ViewBag.PromoInputText = "";
                                }

                                foreach (var pizza in pizzasList.OrderBy(a => a.PizzaName))
                                {
                                    productsList.Add(pizza);
                                }

                                foreach (var drink in drinksList.OrderBy(a => a.DrinkName))
                                {
                                    productsList.Add(drink);
                                }

                                foreach (var dessert in dessertsList.OrderBy(a => a.DessertName))
                                {
                                    productsList.Add(dessert);
                                }

                                promoModel.allProducts = productsList;

                                if (de.Promos.Count() < 1)
                                {
                                    promoModel.allPromos = new List<Promo>();
                                }
                                else
                                {
                                    promoModel.allPromos = de.Promos.OrderBy(a => a.ItemName).ToList();
                                }
                                return View(promoModel);
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Promo Code POST Action
        [Authorize]
        [HttpPost]
        public ActionResult UploadPromoCode(FormCollection formCollection)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        TempData["uploadPromoCodeText"] = "You cannot upload promo codes until you verify your account!";
                        return RedirectToAction("ManagePromotions", "Admin");
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["ProductsList"] == "")
                                    {
                                        TempData["uploadPromoCodeText"] = "Product is required";
                                        return RedirectToAction("ManagePromotions", "Admin");
                                    }
                                    if (formCollection["PromoCodesQuantity"] == "")
                                    {
                                        TempData["uploadPromoCodeText"] = "Promo codes number is required";
                                        return RedirectToAction("ManagePromotions", "Admin");
                                    }
                                    if (formCollection["PromoPercentInput"] == "")
                                    {
                                        TempData["uploadPromoCodeText"] = "Promo percent is required";
                                        return RedirectToAction("ManagePromotions", "Admin");
                                    }

                                    string size = formCollection["ProductsList"].Split(new[] { " - " }, StringSplitOptions.None).Last();
                                    string name = formCollection["ProductsList"].Split(new[] { " - " + size }, StringSplitOptions.None).First().Split(new[] { " - " }, StringSplitOptions.None).Last();
                                    string type = formCollection["ProductsList"].Split(new[] { " - " + name + " - " + size }, StringSplitOptions.None).First();

                                    // https://math.stackexchange.com/questions/443774/how-to-calculate-the-different-combinations-possible
                                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                                    var random = new Random();
                                    String[] promoCodesArray = new String[Int32.Parse(formCollection["PromoCodesQuantity"])];
                                    for (int i = 0; i < Int32.Parse(formCollection["PromoCodesQuantity"]); i++)
                                    {
                                        var stringChars = new char[8];
                                        for (int j = 0; j < stringChars.Length; j++)
                                        {
                                            stringChars[j] = chars[random.Next(chars.Length)];
                                        }
                                        var finalString = new String(stringChars);
                                        Promo temp = de.Promos.Where(a => a.PromoCode.Equals(finalString)).FirstOrDefault();
                                        while (temp != null)
                                        {
                                            stringChars = new char[8];
                                            for (int j = 0; j < stringChars.Length; j++)
                                            {
                                                stringChars[j] = chars[random.Next(chars.Length)];
                                            }
                                            finalString = new String(stringChars);
                                            temp = de.Promos.Where(a => a.PromoCode.Equals(finalString)).FirstOrDefault();
                                        }
                                        if (temp == null)
                                        {
                                            while (promoCodesArray.Contains(finalString))
                                            {
                                                stringChars = new char[8];
                                                for (int j = 0; j < stringChars.Length; j++)
                                                {
                                                    stringChars[j] = chars[random.Next(chars.Length)];
                                                }
                                                finalString = new String(stringChars);
                                            }
                                        }
                                        promoCodesArray[i] = finalString;
                                    }

                                    for (int i = 0; i < promoCodesArray.Length; i++)
                                    {
                                        Promo promo = new Promo();
                                        promo.ItemName = name;
                                        promo.ItemSize = size;
                                        promo.ItemType = type;
                                        promo.PromoCode = promoCodesArray[i];
                                        promo.PromoPercent = Double.Parse(formCollection["PromoPercentInput"].Replace(".", ","));
                                        promo.ItemDeadline = DateTime.Parse(formCollection["PromoDeadline"]).ToString("dd.MM.yyyy HH:mm:ss").Substring(0, 10) + " 23:59:59";
                                        de.Promos.Add(promo);
                                        de.SaveChanges();
                                        message = "Promo codes uploaded successfully!";
                                    }

                                    TempData["uploadPromoCodeText"] = message;
                                    return RedirectToAction("ManagePromotions", "Admin");
                                }
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Promo Code POST Action
        [Authorize]
        [HttpPost]
        public ActionResult DeletePromoCode(FormCollection formCollection)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        TempData["deletePromoCodeText"] = "You cannot delete promo codes until you verify your account!";
                        return RedirectToAction("ManagePromotions", "Admin");
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["PromosList"] == "")
                                    {
                                        TempData["deletePromoCodeText"] = "Promo is required";
                                        return RedirectToAction("ManagePromotions", "Admin");
                                    }

                                    string[] promoInfo = formCollection["PromosList"].Split(new[] { " - " }, StringSplitOptions.None);
                                    string promoCode = promoInfo[0];
                                    double promoPercent = Double.Parse(promoInfo[1].Replace("%", ""));
                                    string itemName = promoInfo[2];
                                    string itemSize = promoInfo[3];
                                    string itemDeadline = promoInfo[4];

                                    foreach (var promo in de.Promos)
                                    {
                                        if ((promo.PromoCode.Equals(promoCode)) && (promo.PromoPercent == promoPercent) && (promo.ItemName.Equals(itemName)) && (promo.ItemSize.Equals(itemSize)) && (promo.ItemDeadline.Equals(itemDeadline)))
                                        {
                                            User codeUser = de.Users.Where(a => a.CurrentPromoCode.Equals(promo.PromoCode)).FirstOrDefault();
                                            if (codeUser != null)
                                            {
                                                codeUser.CurrentPromoCode = null;
                                            }
                                            de.Promos.Remove(promo);
                                            message = "Promo code successfully deleted!";
                                            break;
                                        }
                                    }

                                    de.Configuration.ValidateOnSaveEnabled = false;
                                    de.SaveChanges();

                                    TempData["deletePromoCodeText"] = message;
                                    return RedirectToAction("ManagePromotions", "Admin");
                                }
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Manage Promotions Text
        [ValidateInput(false)]
        [Authorize]
        [HttpPost]
        public ActionResult ManagePromoText(FormCollection formCollection)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        TempData["managePromotionsText"] = "You cannot manage promotions text until you verify your account!";
                        return RedirectToAction("ManagePromotions", "Admin");
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                if (formCollection["promoTextInput"] == "")
                                {
                                    if (de.PromotionsTexts.Count() > 0)
                                    {
                                        de.PromotionsTexts.FirstOrDefault().PromoText = null;
                                    }
                                    message = "Promotions text successfully cleared!";
                                }
                                else
                                {
                                    if (de.PromotionsTexts.Count() > 0)
                                    {
                                        de.PromotionsTexts.FirstOrDefault().PromoText = formCollection["promoTextInput"];
                                    }
                                    else
                                    {
                                        PromotionsText promosText = new PromotionsText();
                                        promosText.PromoText = formCollection["promoTextInput"];
                                        de.PromotionsTexts.Add(promosText);
                                    }
                                    message = "Promotions text successfully updated!";
                                }

                                de.SaveChanges();

                                TempData["managePromotionsText"] = message;
                                return RedirectToAction("ManagePromotions", "Admin");
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Ingredient Action
        [Authorize]
        [HttpGet]
        public ActionResult DeleteIngredient()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Ingredient POST Action
        [Authorize]
        [HttpPost]
        public ActionResult DeleteIngredient(FormCollection formCollection)
        {
            string message = "";
            bool ingredientUsed = false;
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        message = "You cannot delete ingredients until you verify your account!";
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["IngredientsList"] != "")
                                    {
                                        string ingredientName = formCollection["IngredientsList"].ToString();
                                        Ingredient ingredient = de.Ingredients.Where(a => a.IngredientName == ingredientName).FirstOrDefault();
                                        if (ingredient != null)
                                        {
                                            int ingredientID = ingredient.IngredientID;
                                            foreach (var recpIngr in de.RecipesIngredients)
                                            {
                                                if (recpIngr.IngredientID == ingredientID)
                                                {
                                                    ingredientUsed = true;
                                                    break;
                                                }
                                            }

                                            if (ingredientUsed == false)
                                            {
                                                message = "Ingredient '" + ingredient.IngredientName + "' successfully deleted!";
                                                de.Ingredients.Remove(ingredient);
                                                de.SaveChanges();
                                            }
                                            else
                                            {
                                                message = "This ingredient is being used at the moment so it cannot be deleted!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message = "Please select an ingredient from the list first!";
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Recipe Action
        [Authorize]
        [HttpGet]
        public ActionResult DeleteRecipe()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Recipe POST Action
        [Authorize]
        [HttpPost]
        public ActionResult DeleteRecipe(FormCollection formCollection)
        {
            string message = "";
            bool recipeUsed = false;
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        message = "You cannot delete recipes until you verify your account!";
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["RecipesList"] != "")
                                    {
                                        string recipeName = formCollection["RecipesList"].ToString();
                                        Recipe recipe = de.Recipes.Where(a => a.RecipeName == recipeName).FirstOrDefault();
                                        if (recipe != null)
                                        {
                                            foreach (Pizza pizza in de.Pizzas)
                                            {
                                                if (pizza.Recipe == recipe)
                                                {
                                                    recipeUsed = true;
                                                    break;
                                                }
                                            }

                                            if (recipeUsed == false)
                                            {
                                                message = "Recipe '" + recipe.RecipeName + "' successfully deleted!";
                                                var recipeIngredients = recipe.RecipesIngredients;
                                                de.RecipesIngredients.RemoveRange(recipeIngredients);
                                                de.Recipes.Remove(recipe);
                                                de.SaveChanges();
                                            }
                                            else
                                            {
                                                message = "This recipe is being used at the moment so it cannot be deleted!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message = "Please select a recipe from the list first!";
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Pizza Action
        [Authorize]
        [HttpGet]
        public ActionResult DeletePizza()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Pizza POST Action
        [Authorize]
        [HttpPost]
        public ActionResult DeletePizza(FormCollection formCollection)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        message = "You cannot delete pizzas until you verify your account!";
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["PizzasList"] != "")
                                    {
                                        string[] pizzaNameAndSize = formCollection["PizzasList"].ToString().Split(new string[] { " - " }, StringSplitOptions.None);
                                        string pizzaName = pizzaNameAndSize[0];
                                        string pizzaSize = pizzaNameAndSize[1];
                                        var pizza = de.Pizzas.Where(a => (a.PizzaName == pizzaName) && (a.PizzaSize == pizzaSize)).FirstOrDefault();
                                        if (pizza != null)
                                        {
                                            int pizzaID = pizza.PizzaID;
                                            var ordersWithPizza = de.Orders.Where(a => a.PizzaID == pizzaID);
                                            string imagePath = pizza.PizzaPicturePath;
                                            if (System.IO.File.Exists(Request.MapPath(imagePath)))
                                            {
                                                foreach (var order in ordersWithPizza)
                                                {
                                                    if (order.PizzaID == pizza.PizzaID)
                                                    {
                                                        de.Orders.Remove(order); // Delete the order which contains the selected pizza as well, otherwise there'll be an error.
                                                    }
                                                }
                                                de.SaveChanges();
                                                foreach (var promo in de.Promos)
                                                {
                                                    if ((promo.ItemName.Equals(pizza.PizzaName)) && (promo.ItemSize.Equals(pizza.PizzaSize)))
                                                    {
                                                        User codeUser = de.Users.Where(a => a.CurrentPromoCode.Equals(promo.PromoCode)).FirstOrDefault();
                                                        if (codeUser != null)
                                                        {
                                                            codeUser.CurrentPromoCode = null;
                                                        }
                                                        de.Promos.Remove(promo);
                                                    }
                                                }
                                                de.Configuration.ValidateOnSaveEnabled = false;
                                                de.SaveChanges();
                                                if (de.Orders.Count() > 0)
                                                {
                                                    if (de.Orders.Where(a => a.UserID == user.UserID).Count() == 0)
                                                    {
                                                        user.CanUsePromoCodes = true;
                                                        de.Configuration.ValidateOnSaveEnabled = false;
                                                    }
                                                }
                                                else
                                                {
                                                    user.CanUsePromoCodes = true;
                                                    de.Configuration.ValidateOnSaveEnabled = false;
                                                }
                                                de.SaveChanges();
                                                de.Pizzas.Remove(pizza);
                                                foreach (var promo in de.Promos)
                                                {
                                                    if (promo.ItemName.Equals(pizza.PizzaName) && (promo.ItemSize.Equals(pizza.PizzaSize)))
                                                    {
                                                        de.Promos.Remove(promo);
                                                    }
                                                }
                                                de.SaveChanges();
                                                System.IO.File.Delete(Request.MapPath(imagePath));
                                                message = "Pizza '" + pizza.PizzaName + " - " + pizza.PizzaSize + "' successfully deleted!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message = "Please select a pizza from the list first!";
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Drink Action
        [Authorize]
        [HttpGet]
        public ActionResult DeleteDrink()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Drink POST Action
        [Authorize]
        [HttpPost]
        public ActionResult DeleteDrink(FormCollection formCollection)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        message = "You cannot delete drinks until you verify your account!";
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["DrinksList"] != "")
                                    {
                                        string[] drinkNameAndSize = formCollection["DrinksList"].ToString().Split(new string[] { " - " }, StringSplitOptions.None);
                                        string drinkName = drinkNameAndSize[0];
                                        string drinkSize = drinkNameAndSize[1];
                                        var drink = de.Drinks.Where(a => (a.DrinkName == drinkName) && (a.DrinkSize == drinkSize)).FirstOrDefault();
                                        if (drink != null)
                                        {
                                            int drinkID = drink.DrinkID;
                                            var ordersWithDrink = de.Orders.Where(a => a.DrinkID == drinkID);
                                            string imagePath = drink.DrinkPicturePath;
                                            if (System.IO.File.Exists(Request.MapPath(imagePath)))
                                            {
                                                foreach (var order in ordersWithDrink)
                                                {
                                                    if (order.DrinkID == drink.DrinkID)
                                                    {
                                                        de.Orders.Remove(order); // Delete the order which contains the selected drink as well, otherwise there'll be an error.
                                                    }
                                                }
                                                de.SaveChanges();
                                                foreach (var promo in de.Promos)
                                                {
                                                    if ((promo.ItemName.Equals(drink.DrinkName)) && (promo.ItemSize.Equals(drink.DrinkSize)))
                                                    {
                                                        User codeUser = de.Users.Where(a => a.CurrentPromoCode.Equals(promo.PromoCode)).FirstOrDefault();
                                                        if (codeUser != null)
                                                        {
                                                            codeUser.CurrentPromoCode = null;
                                                        }
                                                        de.Promos.Remove(promo);
                                                    }
                                                }
                                                de.Configuration.ValidateOnSaveEnabled = false;
                                                de.SaveChanges();
                                                if (de.Orders.Count() > 0)
                                                {
                                                    if (de.Orders.Where(a => a.UserID == user.UserID).Count() == 0)
                                                    {
                                                        user.CanUsePromoCodes = true;
                                                        de.Configuration.ValidateOnSaveEnabled = false;
                                                    }
                                                }
                                                else
                                                {
                                                    user.CanUsePromoCodes = true;
                                                    de.Configuration.ValidateOnSaveEnabled = false;
                                                }
                                                de.SaveChanges();
                                                de.Drinks.Remove(drink);
                                                foreach (var promo in de.Promos)
                                                {
                                                    if (promo.ItemName.Equals(drink.DrinkName) && (promo.ItemSize.Equals(drink.DrinkSize)))
                                                    {
                                                        de.Promos.Remove(promo);
                                                    }
                                                }
                                                de.SaveChanges();
                                                System.IO.File.Delete(Request.MapPath(imagePath));
                                                message = "Drink '" + drink.DrinkName + " - " + drink.DrinkSize + "' successfully deleted!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message = "Please select a drink from the list first!";
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Dessert Action
        [Authorize]
        [HttpGet]
        public ActionResult DeleteDessert()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Dessert POST Action
        [Authorize]
        [HttpPost]
        public ActionResult DeleteDessert(FormCollection formCollection)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        message = "You cannot delete desserts until you verify your account!";
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["DessertsList"] != "")
                                    {
                                        string[] dessertNameAndSize = formCollection["DessertsList"].ToString().Split(new string[] { " - " }, StringSplitOptions.None);
                                        string dessertName = dessertNameAndSize[0];
                                        string dessertSize = dessertNameAndSize[1];
                                        var dessert = de.Desserts.Where(a => (a.DessertName == dessertName) && (a.DessertSize == dessertSize)).FirstOrDefault();
                                        if (dessert != null)
                                        {
                                            int dessertID = dessert.DessertID;
                                            var ordersWithDessert = de.Orders.Where(a => a.DessertID == dessertID);
                                            string imagePath = dessert.DessertPicturePath;
                                            if (System.IO.File.Exists(Request.MapPath(imagePath)))
                                            {
                                                foreach (var order in ordersWithDessert)
                                                {
                                                    if (order.DessertID == dessert.DessertID)
                                                    {
                                                        de.Orders.Remove(order); // Delete the order which contains the selected dessert as well, otherwise there'll be an error.
                                                    }
                                                }
                                                de.SaveChanges();
                                                foreach (var promo in de.Promos)
                                                {
                                                    if ((promo.ItemName.Equals(dessert.DessertName)) && (promo.ItemSize.Equals(dessert.DessertSize)))
                                                    {
                                                        User codeUser = de.Users.Where(a => a.CurrentPromoCode.Equals(promo.PromoCode)).FirstOrDefault();
                                                        if (codeUser != null)
                                                        {
                                                            codeUser.CurrentPromoCode = null;
                                                        }
                                                        de.Promos.Remove(promo);
                                                    }
                                                }
                                                de.Configuration.ValidateOnSaveEnabled = false;
                                                de.SaveChanges();
                                                if (de.Orders.Count() > 0)
                                                {
                                                    if (de.Orders.Where(a => a.UserID == user.UserID).Count() == 0)
                                                    {
                                                        user.CanUsePromoCodes = true;
                                                        de.Configuration.ValidateOnSaveEnabled = false;
                                                    }
                                                }
                                                else
                                                {
                                                    user.CanUsePromoCodes = true;
                                                    de.Configuration.ValidateOnSaveEnabled = false;
                                                }
                                                de.SaveChanges();
                                                de.Desserts.Remove(dessert);
                                                foreach (var promo in de.Promos)
                                                {
                                                    if (promo.ItemName.Equals(dessert.DessertName) && (promo.ItemSize.Equals(dessert.DessertSize)))
                                                    {
                                                        de.Promos.Remove(promo);
                                                    }
                                                }
                                                de.SaveChanges();
                                                System.IO.File.Delete(Request.MapPath(imagePath));
                                                message = "Dessert '" + dessert.DessertName + " - " + dessert.DessertSize + "' successfully deleted!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message = "Please select a dessert from the list first!";
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion
    }
}