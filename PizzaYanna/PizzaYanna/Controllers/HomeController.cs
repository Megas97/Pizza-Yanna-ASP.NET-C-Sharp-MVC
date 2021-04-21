using PizzaYanna.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PizzaYanna.Controllers
{
    public class HomeController : Controller
    {
        #region // Index Action
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region // Promotions Action
        [HttpGet]
        public ActionResult Promotions()
        {
            using (DBEntities de = new DBEntities())
            {
                if (de.PromotionsTexts.Count() > 0)
                {
                    ViewBag.PromosText = de.PromotionsTexts.FirstOrDefault().PromoText;
                }
                else
                {
                    ViewBag.PromosText = "";
                }
                return View();
            }
        }
        #endregion

        #region // Promotions POST Action
        [Authorize]
        [HttpPost]
        public ActionResult Promotions(FormCollection formCollection)
        {
            string message = "";
            bool promoItemFound = false;
            bool codeFound = false;
            using (DBEntities de = new DBEntities())
            {
                if (formCollection["PromoCodeInput"] == "")
                {
                    ViewBag.Message = "Please enter a promo code first!";
                    return View();
                }

                if (de.PromotionsTexts.Count() > 0)
                {
                    ViewBag.PromosText = de.PromotionsTexts.FirstOrDefault().PromoText;
                }
                else
                {
                    ViewBag.PromosText = "";
                }

                User currentUser = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (currentUser != null)
                {
                    if (currentUser.IsEmailVerified != true)
                    {
                        message = "You cannot apply a promo code until you verify your account!";
                    }
                    else
                    {
                        if (currentUser.CanUsePromoCodes != true)
                        {
                            ViewBag.Message = "You are allowed to use only one promo code per order!";
                            return View();
                        }
                        string code = formCollection["PromoCodeInput"];
                        foreach (var promo in de.Promos)
                        {
                            if (promo.PromoCode.Equals(code))
                            {
                                if ((currentUser.CurrentPromoCode == null) || ((currentUser.CurrentPromoCode != null) && (!currentUser.CurrentPromoCode.Equals(code))))
                                {
                                    ViewBag.Message = "You are only allowed to use the promo code linked to your profile!";
                                    return View();
                                }
                                string promoCode = promo.PromoCode;
                                string itemName = promo.ItemName;
                                string itemSize = promo.ItemSize;
                                string itemType = promo.ItemType;
                                double percent = promo.PromoPercent;
                                var userOrders = de.Orders.Where(a => a.UserID == currentUser.UserID);
                                if (userOrders.Count() > 0)
                                {
                                    foreach (var order in userOrders)
                                    {
                                        if (itemType.Equals("Pizza"))
                                        {
                                            if (order.Pizza != null)
                                            {
                                                if (order.Pizza.PizzaName.StartsWith("Custom: "))
                                                {
                                                    if ((itemName.Equals(order.Pizza.PizzaName.Split(new string[] { "Custom: " }, StringSplitOptions.None)[1])) && (itemSize.Equals(order.Pizza.PizzaSize)))
                                                    {
                                                        if (order.PizzaAmount == 1)
                                                        {
                                                            order.PromoPercent = percent;
                                                            message = "You successfully used promo code '" + promoCode + "' with your pizza order!";
                                                            promoItemFound = true;
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ViewBag.Message = "All promo codes apply only to a single product!";
                                                            return View();
                                                        }
                                                    }
                                                }
                                                else if ((itemName.Equals(order.Pizza.PizzaName)) && (itemSize.Equals(order.Pizza.PizzaSize)))
                                                {
                                                    if (order.PizzaAmount == 1)
                                                    {
                                                        order.PromoPercent = percent;
                                                        message = "You successfully used promo code '" + promoCode + "' with your pizza order!";
                                                        promoItemFound = true;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        ViewBag.Message = "All promo codes apply only to a single product!";
                                                        return View();
                                                    }
                                                }
                                            }
                                        }
                                        else if (itemType.Equals("Drink"))
                                        {
                                            if (order.Drink != null)
                                            {
                                                if ((itemName.Equals(order.Drink.DrinkName)) && (itemSize.Equals(order.Drink.DrinkSize)))
                                                {
                                                    if (order.DrinkAmount == 1)
                                                    {
                                                        order.PromoPercent = percent;
                                                        message = "You successfully used promo code '" + promoCode + "' with your drink order!";
                                                        promoItemFound = true;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        ViewBag.Message = "All promo codes apply only to a single product!";
                                                        return View();
                                                    }
                                                }
                                            }
                                        }
                                        else if (itemType.Equals("Dessert"))
                                        {
                                            if (order.Dessert != null)
                                            {
                                                if ((itemName.Equals(order.Dessert.DessertName)) && (itemSize.Equals(order.Dessert.DessertSize)))
                                                {
                                                    if (order.DessertAmount == 1)
                                                    {
                                                        order.PromoPercent = percent;
                                                        message = "You successfully used promo code '" + promoCode + "' with your dessert order!";
                                                        promoItemFound = true;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        ViewBag.Message = "All promo codes apply only to a single product!";
                                                        return View();
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (!promoItemFound)
                                    {
                                        ViewBag.Message = "The entered promo code doesn't match any of your cart products!";
                                        return View();
                                    }
                                    else
                                    {
                                        currentUser.CanUsePromoCodes = false;
                                    }
                                }
                                else
                                {
                                    ViewBag.Message = "Please add products to your cart before using a promo code!";
                                    return View();
                                }

                                codeFound = true;
                            }
                        }

                        if (!codeFound)
                        {
                            ViewBag.Message = "The code you entered is invalid!";
                            return View();
                        }

                        de.Configuration.ValidateOnSaveEnabled = false;
                        de.SaveChanges();
                    }
                }
            }

            ViewBag.Message = message;
            return View();
        }
        #endregion

        #region // About Action
        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
        #endregion

        #region // Contacts Action
        [HttpGet]
        public ActionResult Contacts()
        {
            ViewBag.Message = "Address";

            return View();
        }
        #endregion

        #region // Show Pizzas Action
        [HttpGet]
        public ActionResult ShowPizzas()
        {
            string message = "";
            string ingredients = "";
            int availablePizzas = 0;
            List<Pizza> pizzasList = new List<Pizza>();
            using (DBEntities de = new DBEntities())
            {
                if (de.Pizzas.Count() > 0)
                {
                    foreach (var pizza in de.Pizzas)
                    {
                        if (pizza != null)
                        {
                            if (!pizza.PizzaName.StartsWith("Custom: "))
                            {
                                pizzasList.Add(pizza);
                                ingredients = "";
                                var recipesIngredientsForRecipe = pizza.Recipe.RecipesIngredients.Where(a => a.RecipeID == pizza.RecipeID);
                                int i = 0;
                                foreach (var item in recipesIngredientsForRecipe)
                                {
                                    string separator = "";
                                    if (i < recipesIngredientsForRecipe.Count() - 1)
                                    {
                                        separator = ", ";
                                    }
                                    else
                                    {
                                        separator = "";
                                    }
                                    ingredients += item.Ingredient.IngredientName + separator;
                                    i++;
                                }
                                ViewData.Add(pizza.PizzaName + " " + pizza.PizzaSize, ingredients);
                                availablePizzas++;
                            }
                        }
                    }
                }
            }

            if (availablePizzas == 1)
            {
                message = "There is " + availablePizzas + " pizza available!";
            }
            else if (availablePizzas > 1)
            {
                message = "There are " + availablePizzas + " pizzas available!";
            }
            else
            {
                message = "There are no pizzas available!";
            }

            if (TempData["showPizzasOrderInfo"] != null)
            {
                ViewBag.Message2 = TempData["showPizzasOrderInfo"].ToString();
            }

            ViewBag.Message = message;
            return View(pizzasList.OrderBy(a => a.PizzaName).ThenByDescending(a => a.PizzaSize));
        }
        #endregion

        #region // Create Order With Pizza Action
        [Authorize]
        [HttpGet]
        public ActionResult CreateOrderWithPizza(int id)
        {
            string message = "";
            bool canUploadNew = false;
            using (DBEntities de = new DBEntities())
            {
                var currentUserEmailIdentity = HttpContext.User.Identity.Name;
                var currentUser = de.Users.Where(a => a.EmailID == currentUserEmailIdentity).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot order anything until you verify your account!";
                }
                else
                {
                    var pizza = de.Pizzas.Where(a => a.PizzaID == id).FirstOrDefault();
                    if (pizza != null)
                    {
                        if (de.Orders.Count() > 0)
                        {
                            foreach (var ordr in de.Orders)
                            {
                                if (ordr != null)
                                {
                                    if ((ordr.Pizza != null) && (ordr.UserID == currentUser.UserID) && (ordr.Pizza.PizzaName.Equals(pizza.PizzaName)) && (ordr.Pizza.PizzaSize.Equals(pizza.PizzaSize)))
                                    {
                                        ordr.PizzaAmount += 1;
                                        message = "Pizza order successfully updated!";
                                        canUploadNew = false;
                                        break;
                                    }
                                    else
                                    {
                                        canUploadNew = true;
                                    }
                                }
                            }
                            de.SaveChanges();
                            if (canUploadNew == true)
                            {
                                Order order = new Order();
                                order.Pizza = pizza;
                                if (currentUser != null)
                                {
                                    order.User = currentUser;
                                }
                                order.PizzaAmount = 1;
                                de.Orders.Add(order);
                                message = "Pizza successfully added to cart!";
                                de.SaveChanges();
                            }
                        }
                        else
                        {
                            Order order = new Order();
                            order.Pizza = pizza;
                            if (currentUser != null)
                            {
                                order.User = currentUser;
                            }
                            order.PizzaAmount = 1;
                            de.Orders.Add(order);
                            message = "Pizza successfully added to cart!";
                            de.SaveChanges();
                        }
                    }
                }
            }

            TempData["showPizzasOrderInfo"] = message;
            return RedirectToAction("ShowPizzas", "Home");
        }
        #endregion

        #region // Create Custom Pizza Action
        [Authorize]
        [HttpGet]
        public ActionResult CreateCustomPizza(int id, string openedFrom = "MainMenu")
        {
            using (DBEntities de = new DBEntities())
            {
                var pizza = de.Pizzas.Where(a => a.PizzaID == id).FirstOrDefault();
                if (pizza != null)
                {
                    ViewBag.Message = TempData["customOrderInfo"];
                    ViewBag.OpenedFrom = openedFrom;
                    return View(pizza);
                }
            }

            ViewBag.Message = TempData["customOrderInfo"];
            ViewBag.OpenedFrom = openedFrom;
            return View();
        }
        #endregion

        #region // Create Order With Custom Pizza POST Action
        [Authorize]
        [HttpPost]
        public ActionResult CreateOrderWithCustomPizza(int id, string openedFrom, FormCollection formCollection)
        {
            string message = "";
            int savedID = id;
            string savedOpenedFrom = openedFrom;
            bool ingredientsSelected = false;
            using (DBEntities de = new DBEntities())
            {
                User currentUser = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot order a custom pizza until you verify your account!";
                }
                else
                {
                    if (formCollection["CustomPizzaSizesList"] == "")
                    {
                        TempData["customOrderInfo"] = "Pizza size is required!";
                        return RedirectToAction("CreateCustomPizza", "Home", new { id = savedID, openedFrom = savedOpenedFrom });
                    }

                    var pizza = de.Pizzas.Where(a => a.PizzaID == id).FirstOrDefault();
                    string selectedPizzaSize = formCollection["CustomPizzaSizesList"];

                    // If user has not changed pizza recipe use the saved one instead of creating a custom one
                    // If user has selected the same size as the real pizza size
                    if (formCollection["CustomPizzaSizesList"].Equals(pizza.PizzaSize))
                    {
                        List<string> pizzaIngredientsList = new List<string>();
                        var pizzaRecipeIngredients = pizza.Recipe.RecipesIngredients;
                        foreach (var recpIngr in pizzaRecipeIngredients)
                        {
                            if (!pizzaIngredientsList.Contains(recpIngr.Ingredient.IngredientName))
                            {
                                pizzaIngredientsList.Add(recpIngr.Ingredient.IngredientName);
                            }
                        }
                        List<string> selectedIngredientsList = new List<string>();
                        foreach (var checkbox in formCollection)
                        {
                            if (checkbox != null)
                            {
                                string isChecked = formCollection[checkbox.ToString()];
                                if (isChecked == "true,false") // This means the checkbox is selected, "false" means it's not selected.
                                {
                                    if (!selectedIngredientsList.Contains(checkbox.ToString()))
                                    {
                                        selectedIngredientsList.Add(checkbox.ToString());
                                    }
                                }
                            }
                        }
                        bool areEqual = pizzaIngredientsList.SequenceEqual(selectedIngredientsList);
                        if (areEqual)
                        {
                            bool canUploadNew = false;
                            if (pizza != null)
                            {
                                if (de.Orders.Count() > 0)
                                {
                                    foreach (var ordr in de.Orders)
                                    {
                                        if (ordr != null)
                                        {
                                            if ((ordr.Pizza != null) && (ordr.UserID == currentUser.UserID) && (ordr.Pizza.PizzaName.Equals(pizza.PizzaName)) && (ordr.Pizza.PizzaSize.Equals(pizza.PizzaSize)))
                                            {
                                                ordr.PizzaAmount += 1;
                                                TempData["customOrderInfo"] = "Pizza order successfully updated!";
                                                canUploadNew = false;
                                                break;
                                            }
                                            else
                                            {
                                                canUploadNew = true;
                                            }
                                        }
                                    }
                                    de.SaveChanges();
                                    if (canUploadNew == true)
                                    {
                                        Order ordr = new Order();
                                        ordr.Pizza = pizza;
                                        if (currentUser != null)
                                        {
                                            ordr.User = currentUser;
                                        }
                                        ordr.PizzaAmount = 1;
                                        de.Orders.Add(ordr);
                                        TempData["customOrderInfo"] = "Pizza successfully added to cart!";
                                        de.SaveChanges();
                                    }
                                }
                                else
                                {
                                    Order ordr = new Order();
                                    ordr.Pizza = pizza;
                                    if (currentUser != null)
                                    {
                                        ordr.User = currentUser;
                                    }
                                    ordr.PizzaAmount = 1;
                                    de.Orders.Add(ordr);
                                    TempData["customOrderInfo"] = "Pizza successfully added to cart!";
                                    de.SaveChanges();
                                }
                            }
                            return RedirectToAction("CreateCustomPizza", "Home", new { id = savedID, openedFrom = savedOpenedFrom });
                        }
                    }
                    // If user chooses another pizza size but there is another uploaded pizza with this size and same name and ingredients choose that instead of creating a custom one
                    else
                    {
                        var existingPizza = de.Pizzas.Where(a => a.PizzaSize.Equals(selectedPizzaSize)).Where(a => a.PizzaName.Equals(pizza.PizzaName)).FirstOrDefault();
                        if (existingPizza != null)
                        {
                            if (!existingPizza.PizzaName.StartsWith("Custom: "))
                            {
                                List<string> pizzaIngredientsList = new List<string>();
                                var pizzaRecipeIngredients = existingPizza.Recipe.RecipesIngredients;
                                foreach (var recpIngr in pizzaRecipeIngredients)
                                {
                                    if (!pizzaIngredientsList.Contains(recpIngr.Ingredient.IngredientName))
                                    {
                                        pizzaIngredientsList.Add(recpIngr.Ingredient.IngredientName);
                                    }
                                }
                                List<string> selectedIngredientsList = new List<string>();
                                foreach (var checkbox in formCollection)
                                {
                                    if (checkbox != null)
                                    {
                                        string isChecked = formCollection[checkbox.ToString()];
                                        if (isChecked == "true,false") // This means the checkbox is selected, "false" means it's not selected.
                                        {
                                            if (!selectedIngredientsList.Contains(checkbox.ToString()))
                                            {
                                                selectedIngredientsList.Add(checkbox.ToString());
                                            }
                                        }
                                    }
                                }
                                bool areEqual = pizzaIngredientsList.SequenceEqual(selectedIngredientsList);
                                if (areEqual)
                                {
                                    int existingPizzaID = existingPizza.PizzaID;
                                    bool canUploadNew = false;
                                    if (pizza != null)
                                    {
                                        if (de.Orders.Count() > 0)
                                        {
                                            foreach (var ordr in de.Orders)
                                            {
                                                if (ordr != null)
                                                {
                                                    if ((ordr.Pizza != null) && (ordr.UserID == currentUser.UserID) && (ordr.Pizza.PizzaName.Equals(pizza.PizzaName)) && (ordr.Pizza.PizzaSize.Equals(pizza.PizzaSize)))
                                                    {
                                                        ordr.PizzaAmount += 1;
                                                        TempData["customOrderInfo"] = "Pizza order successfully updated!";
                                                        canUploadNew = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        canUploadNew = true;
                                                    }
                                                }
                                            }
                                            de.SaveChanges();
                                            if (canUploadNew == true)
                                            {
                                                Order ordr = new Order();
                                                ordr.Pizza = pizza;
                                                if (currentUser != null)
                                                {
                                                    ordr.User = currentUser;
                                                }
                                                ordr.PizzaAmount = 1;
                                                de.Orders.Add(ordr);
                                                TempData["customOrderInfo"] = "Pizza successfully added to cart!";
                                                de.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            Order ordr = new Order();
                                            ordr.Pizza = pizza;
                                            if (currentUser != null)
                                            {
                                                ordr.User = currentUser;
                                            }
                                            ordr.PizzaAmount = 1;
                                            de.Orders.Add(ordr);
                                            TempData["customOrderInfo"] = "Pizza successfully added to cart!";
                                            de.SaveChanges();
                                        }
                                    }
                                    return RedirectToAction("CreateCustomPizza", "Home", new { id = existingPizzaID, openedFrom = savedOpenedFrom });
                                }
                            }
                        }
                    }

                    foreach (var ordr in currentUser.Orders)
                    {
                        if (ordr.Pizza != null)
                        {
                            if (ordr.Pizza.PizzaName.Equals("Custom: " + pizza.PizzaName) && ordr.Pizza.PizzaSize.Equals(selectedPizzaSize))
                            {
                                List<string> pizzaIngredientsList = new List<string>();
                                var pizzaRecipeIngredients = ordr.Pizza.Recipe.RecipesIngredients;
                                foreach (var recpIngr in pizzaRecipeIngredients)
                                {
                                    if (!pizzaIngredientsList.Contains(recpIngr.Ingredient.IngredientName))
                                    {
                                        pizzaIngredientsList.Add(recpIngr.Ingredient.IngredientName);
                                    }
                                }
                                List<string> selectedIngredientsList = new List<string>();
                                foreach (var checkbox in formCollection)
                                {
                                    if (checkbox != null)
                                    {
                                        string isChecked = formCollection[checkbox.ToString()];
                                        if (isChecked == "true,false") // This means the checkbox is selected, "false" means it's not selected.
                                        {
                                            if (!selectedIngredientsList.Contains(checkbox.ToString()))
                                            {
                                                selectedIngredientsList.Add(checkbox.ToString());
                                            }
                                        }
                                    }
                                }
                                bool areEqual = pizzaIngredientsList.SequenceEqual(selectedIngredientsList);
                                if (areEqual)
                                {
                                    ordr.PizzaAmount += 1;
                                    TempData["customOrderInfo"] = "Custom pizza order successfully updated!";
                                    de.SaveChanges();
                                    return RedirectToAction("CreateCustomPizza", "Home", new { id = savedID, openedFrom = savedOpenedFrom });
                                }
                            }
                        }
                    }

                    Recipe recipe = new Recipe();
                    Pizza customPizza = new Pizza();
                    de.SaveChanges();
                    string pizzaName = "Custom: " + pizza.PizzaName;
                    string recipeName = "Custom: " + pizza.Recipe.RecipeName;
                    double ingredientsPrice = 0.0;
                    double pizzaPrice = 0.0;
                    string pizzaSize = "";
                    string imagePath = pizza.PizzaPicturePath;
                    recipe.RecipeName = recipeName;
                    de.Recipes.Add(recipe);
                    de.SaveChanges();

                    if (formCollection != null)
                    {
                        pizzaSize = formCollection["CustomPizzaSizesList"];
                    }

                    foreach (var checkbox in formCollection)
                    {
                        foreach (var ingredient in de.Ingredients)
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
                                        recpIngr.Recipe = recipe;
                                        recpIngr.Ingredient = ingr;
                                        recipe.RecipesIngredients.Add(recpIngr);
                                        ingredientsSelected = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (ingredientsSelected == false)
                    {
                        TempData["customOrderInfo"] = "Pizza ingredients are required!";
                        return RedirectToAction("CreateCustomPizza", "Home", new { id = savedID, openedFrom = savedOpenedFrom });
                    }

                    foreach (var ingr in recipe.RecipesIngredients)
                    {
                        if (ingr != null)
                        {
                            ingredientsPrice += ingr.Ingredient.IngredientPrice;
                        }
                    }

                    // Pizza price = Ingredients Price + 50% of ingredients price (overcharge, salaries) + size increase.
                    if (pizzaSize.Equals("Small (450g)"))
                    {
                        pizzaPrice = (ingredientsPrice + 0.5 * ingredientsPrice);
                    }
                    else if (pizzaSize.Equals("Medium (700g)"))
                    {
                        pizzaPrice = (ingredientsPrice + 0.5 * ingredientsPrice) + (ingredientsPrice + 0.5 * ingredientsPrice) * 0.2;
                    }
                    else if (pizzaSize.Equals("Large (950g)"))
                    {
                        pizzaPrice = (ingredientsPrice + 0.5 * ingredientsPrice) + (ingredientsPrice + 0.5 * ingredientsPrice) * 0.4;
                    }

                    customPizza.PizzaName = pizzaName;
                    customPizza.Recipe = recipe;
                    customPizza.PizzaPrice = pizzaPrice;
                    customPizza.PizzaSize = pizzaSize;
                    customPizza.PizzaPicturePath = imagePath;
                    de.Pizzas.Add(customPizza);
                    de.SaveChanges();
                    Order order = new Order();
                    order.Pizza = customPizza;
                    order.PizzaAmount = 1;
                    order.User = currentUser;
                    de.Orders.Add(order);
                    de.SaveChanges();
                    message = "Custom pizza successfully added to cart!";
                }
            }

            TempData["customOrderInfo"] = message;
            return RedirectToAction("CreateCustomPizza", "Home", new { id = savedID, openedFrom = savedOpenedFrom });
        }
        #endregion

        #region // Show Drinks Action
        [HttpGet]
        public ActionResult ShowDrinks()
        {
            string message = "";
            int availableDrinks = 0;
            List<Drink> drinksList = new List<Drink>();
            using (DBEntities de = new DBEntities())
            {
                if (de.Drinks.Count() > 0)
                {
                    foreach (var drink in de.Drinks)
                    {
                        if (drink != null)
                        {
                            drinksList.Add(drink);
                            availableDrinks++;
                        }
                    }
                }
            }

            if (availableDrinks == 1)
            {
                message = "There is " + availableDrinks + " drink available!";
            }
            else if (availableDrinks > 1)
            {
                message = "There are " + availableDrinks + " drinks available!";
            }
            else
            {
                message = "There are no drinks available!";
            }

            if (TempData["showDrinksOrderInfo"] != null)
            {
                ViewBag.Message2 = TempData["showDrinksOrderInfo"].ToString();
            }

            ViewBag.Message = message;
            return View(drinksList.OrderBy(a => a.DrinkName).ThenByDescending(a => a.DrinkSize));
        }
        #endregion

        #region // Create Order With Drink Action
        [Authorize]
        [HttpGet]
        public ActionResult CreateOrderWithDrink(int id)
        {
            string message = "";
            bool canUploadNew = false;
            using (DBEntities de = new DBEntities())
            {
                var currentUserEmailIdentity = HttpContext.User.Identity.Name;
                var currentUser = de.Users.Where(a => a.EmailID == currentUserEmailIdentity).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot order anything until you verify your account!";
                }
                else
                {
                    var drink = de.Drinks.Where(a => a.DrinkID == id).FirstOrDefault();
                    if (drink != null)
                    {
                        if (de.Orders.Count() > 0)
                        {
                            foreach (var ordr in de.Orders)
                            {
                                if (ordr != null)
                                {
                                    if ((ordr.Drink != null) && (ordr.UserID == currentUser.UserID) && (ordr.Drink.DrinkName.Equals(drink.DrinkName)) && (ordr.Drink.DrinkSize.Equals(drink.DrinkSize)))
                                    {
                                        ordr.DrinkAmount += 1;
                                        message = "Drink order successfully updated!";
                                        canUploadNew = false;
                                        break;
                                    }
                                    else
                                    {
                                        canUploadNew = true;
                                    }
                                }
                            }
                            de.SaveChanges();
                            if (canUploadNew == true)
                            {
                                Order order = new Order();
                                order.Drink = drink;
                                if (currentUser != null)
                                {
                                    order.User = currentUser;
                                }
                                order.DrinkAmount = 1;
                                de.Orders.Add(order);
                                message = "Drink successfully added to cart!";
                                de.SaveChanges();
                            }
                        }
                        else
                        {
                            Order order = new Order();
                            order.Drink = drink;
                            if (currentUser != null)
                            {
                                order.User = currentUser;
                            }
                            order.DrinkAmount = 1;
                            de.Orders.Add(order);
                            message = "Drink successfully added to cart!";
                            de.SaveChanges();
                        }
                    }
                }
            }

            TempData["showDrinksOrderInfo"] = message;
            return RedirectToAction("ShowDrinks", "Home");
        }
        #endregion

        #region // Show Desserts Action
        [HttpGet]
        public ActionResult ShowDesserts()
        {
            string message = "";
            int availableDesserts = 0;
            List<Dessert> dessertsList = new List<Dessert>();
            using (DBEntities de = new DBEntities())
            {
                if (de.Desserts.Count() > 0)
                {
                    foreach (var dessert in de.Desserts)
                    {
                        if (dessert != null)
                        {
                            dessertsList.Add(dessert);
                            availableDesserts++;
                        }
                    }
                }
            }

            if (availableDesserts == 1)
            {
                message = "There is " + availableDesserts + " dessert available!";
            }
            else if (availableDesserts > 1)
            {
                message = "There are " + availableDesserts + " desserts available!";
            }
            else
            {
                message = "There are no desserts available!";
            }

            if (TempData["showDessertsOrderInfo"] != null)
            {
                ViewBag.Message2 = TempData["showDessertsOrderInfo"].ToString();
            }

            ViewBag.Message = message;
            return View(dessertsList.OrderBy(a => a.DessertName).ThenByDescending(a => a.DessertSize));
        }
        #endregion

        #region // Create Order With Dessert Action
        [Authorize]
        [HttpGet]
        public ActionResult CreateOrderWithDessert(int id)
        {
            string message = "";
            bool canUploadNew = false;
            using (DBEntities de = new DBEntities())
            {
                var currentUserEmailIdentity = HttpContext.User.Identity.Name;
                var currentUser = de.Users.Where(a => a.EmailID == currentUserEmailIdentity).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot order anything until you verify your account!";
                }
                else
                {
                    var dessert = de.Desserts.Where(a => a.DessertID == id).FirstOrDefault();
                    if (dessert != null)
                    {
                        if (de.Orders.Count() > 0)
                        {
                            foreach (var ordr in de.Orders)
                            {
                                if (ordr != null)
                                {
                                    if ((ordr.Dessert != null) && (ordr.UserID == currentUser.UserID) && (ordr.Dessert.DessertName.Equals(dessert.DessertName)) && (ordr.Dessert.DessertSize.Equals(dessert.DessertSize)))
                                    {
                                        ordr.DessertAmount += 1;
                                        message = "Dessert order successfully updated!";
                                        canUploadNew = false;
                                        break;
                                    }
                                    else
                                    {
                                        canUploadNew = true;
                                    }
                                }
                            }
                            de.SaveChanges();
                            if (canUploadNew == true)
                            {
                                Order order = new Order();
                                order.Dessert = dessert;
                                if (currentUser != null)
                                {
                                    order.User = currentUser;
                                }
                                order.DessertAmount = 1;
                                de.Orders.Add(order);
                                message = "Dessert successfully added to cart!";
                                de.SaveChanges();
                            }
                        }
                        else
                        {
                            Order order = new Order();
                            order.Dessert = dessert;
                            if (currentUser != null)
                            {
                                order.User = currentUser;
                            }
                            order.DessertAmount = 1;
                            de.Orders.Add(order);
                            message = "Dessert successfully added to cart!";
                            de.SaveChanges();
                        }
                    }
                }
            }

            TempData["showDessertsOrderInfo"] = message;
            return RedirectToAction("ShowDesserts", "Home");
        }
        #endregion

        #region // Checkout Action
        [Authorize]
        [HttpGet]
        public ActionResult Checkout()
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                List<Order> ordersList = new List<Order>();
                double totalOrderPrice = 0.0;
                double oldTotalOrderPrice = 0.0;
                var currentUserEmailIdentity = HttpContext.User.Identity.Name;
                var currentUser = de.Users.Where(a => a.EmailID == currentUserEmailIdentity).FirstOrDefault();
                var ordersByCurrentUser = de.Orders.Where(a => a.UserID == currentUser.UserID);
                int items = 0;
                foreach (var order in ordersByCurrentUser)
                {
                    ordersList.Add(order);

                    if (order.Pizza != null)
                    {
                        if (!ViewData.ContainsKey("pizzaPicturePath" + order.OrderID))
                        {
                            ViewData.Add("pizzaPicturePath" + order.OrderID, order.Pizza.PizzaPicturePath);
                        }

                        if (!ViewData.ContainsKey("pizzaName" + order.OrderID))
                        {
                            ViewData.Add("pizzaName" + order.OrderID, order.Pizza.PizzaName);
                        }

                        if (!ViewData.ContainsKey("pizzaSize" + order.OrderID))
                        {
                            ViewData.Add("pizzaSize" + order.OrderID, order.Pizza.PizzaSize);
                        }

                        if (!ViewData.ContainsKey("pizzaPrice" + order.OrderID))
                        {
                            if (order.PromoPercent != null)
                            {
                                if (!ViewData.ContainsKey("pizzaOldPrice" + order.OrderID))
                                {
                                    ViewData.Add("pizzaOldPrice" + order.OrderID, order.Pizza.PizzaPrice);
                                }
                                if (!ViewData.ContainsKey("pizzaPrice" + order.OrderID))
                                {
                                    ViewData.Add("pizzaPrice" + order.OrderID, order.Pizza.PizzaPrice - (order.PromoPercent.GetValueOrDefault() / 100 * order.Pizza.PizzaPrice));
                                }
                                totalOrderPrice += (order.Pizza.PizzaPrice - (order.PromoPercent.GetValueOrDefault() / 100 * order.Pizza.PizzaPrice)) * order.PizzaAmount.GetValueOrDefault();
                                oldTotalOrderPrice += order.Pizza.PizzaPrice * order.PizzaAmount.GetValueOrDefault();
                            }
                            else
                            {
                                if (!ViewData.ContainsKey("pizzaPrice" + order.OrderID))
                                {
                                    ViewData.Add("pizzaPrice" + order.OrderID, order.Pizza.PizzaPrice);
                                }
                                totalOrderPrice += order.Pizza.PizzaPrice * order.PizzaAmount.GetValueOrDefault();
                                oldTotalOrderPrice += order.Pizza.PizzaPrice * order.PizzaAmount.GetValueOrDefault();
                            }
                        }

                        items += order.PizzaAmount.GetValueOrDefault();
                    }

                    if (order.Drink != null)
                    {
                        if (!ViewData.ContainsKey("drinkPicturePath" + order.OrderID))
                        {
                            ViewData.Add("drinkPicturePath" + order.OrderID, order.Drink.DrinkPicturePath);
                        }

                        if (!ViewData.ContainsKey("drinkName" + order.OrderID))
                        {
                            ViewData.Add("drinkName" + order.OrderID, order.Drink.DrinkName);
                        }

                        if (!ViewData.ContainsKey("drinkSize" + order.OrderID))
                        {
                            ViewData.Add("drinkSize" + order.OrderID, order.Drink.DrinkSize);
                        }

                        if (!ViewData.ContainsKey("drinkPrice" + order.OrderID))
                        {
                            if (order.PromoPercent != null)
                            {
                                if (!ViewData.ContainsKey("drinkOldPrice" + order.OrderID))
                                {
                                    ViewData.Add("drinkOldPrice" + order.OrderID, order.Drink.DrinkPrice);
                                }
                                if (!ViewData.ContainsKey("drinkPrice" + order.OrderID))
                                {
                                    ViewData.Add("drinkPrice" + order.OrderID, order.Drink.DrinkPrice - (order.PromoPercent.GetValueOrDefault() / 100 * order.Drink.DrinkPrice));
                                }
                                totalOrderPrice += (order.Drink.DrinkPrice - (order.PromoPercent.GetValueOrDefault() / 100 * order.Drink.DrinkPrice)) * order.DrinkAmount.GetValueOrDefault();
                                oldTotalOrderPrice += order.Drink.DrinkPrice * order.DrinkAmount.GetValueOrDefault();
                            }
                            else
                            {
                                if (!ViewData.ContainsKey("drinkPrice" + order.OrderID))
                                {
                                    ViewData.Add("drinkPrice" + order.OrderID, order.Drink.DrinkPrice);
                                }
                                totalOrderPrice += order.Drink.DrinkPrice * order.DrinkAmount.GetValueOrDefault();
                                oldTotalOrderPrice += order.Drink.DrinkPrice * order.DrinkAmount.GetValueOrDefault();
                            }
                        }

                        items += order.DrinkAmount.GetValueOrDefault();
                    }

                    if (order.Dessert != null)
                    {
                        if (!ViewData.ContainsKey("dessertPicturePath" + order.OrderID))
                        {
                            ViewData.Add("dessertPicturePath" + order.OrderID, order.Dessert.DessertPicturePath);
                        }

                        if (!ViewData.ContainsKey("dessertName" + order.OrderID))
                        {
                            ViewData.Add("dessertName" + order.OrderID, order.Dessert.DessertName);
                        }

                        if (!ViewData.ContainsKey("dessertSize" + order.OrderID))
                        {
                            ViewData.Add("dessertSize" + order.OrderID, order.Dessert.DessertSize);
                        }

                        if (!ViewData.ContainsKey("dessertPrice" + order.OrderID))
                        {
                            if (order.PromoPercent != null)
                            {
                                if (!ViewData.ContainsKey("dessertOldPrice" + order.OrderID))
                                {
                                    ViewData.Add("dessertOldPrice" + order.OrderID, order.Dessert.DessertPrice);
                                }
                                if (!ViewData.ContainsKey("dessertPrice" + order.OrderID))
                                {
                                    ViewData.Add("dessertPrice" + order.OrderID, order.Dessert.DessertPrice - (order.PromoPercent.GetValueOrDefault() / 100 * order.Dessert.DessertPrice));
                                }
                                totalOrderPrice += (order.Dessert.DessertPrice - (order.PromoPercent.GetValueOrDefault() / 100 * order.Dessert.DessertPrice)) * order.DessertAmount.GetValueOrDefault();
                                oldTotalOrderPrice += order.Dessert.DessertPrice * order.DessertAmount.GetValueOrDefault();
                            }
                            else
                            {
                                if (!ViewData.ContainsKey("dessertPrice" + order.OrderID))
                                {
                                    ViewData.Add("dessertPrice" + order.OrderID, order.Dessert.DessertPrice);
                                }
                                totalOrderPrice += order.Dessert.DessertPrice * order.DessertAmount.GetValueOrDefault();
                                oldTotalOrderPrice += order.Dessert.DessertPrice * order.DessertAmount.GetValueOrDefault();
                            }
                        }

                        items += order.DessertAmount.GetValueOrDefault();
                    }
                }

                if ((oldTotalOrderPrice > 0) && (oldTotalOrderPrice != totalOrderPrice))
                {
                    if (!ViewData.ContainsKey("oldTotalOrderPrice"))
                    {
                        ViewData.Add("oldTotalOrderPrice", oldTotalOrderPrice);
                    }
                }

                if (!ViewData.ContainsKey("totalOrderPrice"))
                {
                    ViewData.Add("totalOrderPrice", totalOrderPrice);
                }

                if (items == 1)
                {
                    message = "There is " + items.ToString() + " item in your cart!";
                }
                else if (items > 1)
                {
                    message = "There are " + items.ToString() + " items in your cart!";
                }
                else
                {
                    message = "Your cart is empty!";
                }

                ViewBag.Message = message;
                ViewBag.Message2 = TempData["orderInfo"];
                return View(ordersList);
            }
        }
        #endregion

        #region // Add Pizza Order Action
        [Authorize]
        [HttpGet]
        public ActionResult AddPizzaOrder(int id)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User currentUser = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot update your orders until you verify your account!";
                }
                else
                {
                    var order = de.Orders.Where(a => a.OrderID == id).FirstOrDefault();
                    if (order != null)
                    {
                        message = "Pizza order successfully updated!";
                        order.PizzaAmount += 1;
                        order.PromoPercent = null;
                        currentUser.CanUsePromoCodes = true;
                        de.Configuration.ValidateOnSaveEnabled = false;
                        de.SaveChanges();
                    }
                }
            }

            TempData["orderInfo"] = message;
            return RedirectToAction("Checkout", "Home");
        }
        #endregion

        #region // Add Drink Order Action
        [Authorize]
        [HttpGet]
        public ActionResult AddDrinkOrder(int id)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User currentUser = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot update your orders until you verify your account!";
                }
                else
                {
                    var order = de.Orders.Where(a => a.OrderID == id).FirstOrDefault();
                    if (order != null)
                    {
                        message = "Drink order successfully updated!";
                        order.DrinkAmount += 1;
                        order.PromoPercent = null;
                        currentUser.CanUsePromoCodes = true;
                        de.Configuration.ValidateOnSaveEnabled = false;
                        de.SaveChanges();
                    }
                }
            }

            TempData["orderInfo"] = message;
            return RedirectToAction("Checkout", "Home");
        }
        #endregion

        #region // Add Dessert Order Action
        [Authorize]
        [HttpGet]
        public ActionResult AddDessertOrder(int id)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User currentUser = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot update your orders until you verify your account!";
                }
                else
                {
                    var order = de.Orders.Where(a => a.OrderID == id).FirstOrDefault();
                    if (order != null)
                    {
                        message = "Dessert order successfully updated!";
                        order.DessertAmount += 1;
                        order.PromoPercent = null;
                        currentUser.CanUsePromoCodes = true;
                        de.Configuration.ValidateOnSaveEnabled = false;
                        de.SaveChanges();
                    }
                }
            }

            TempData["orderInfo"] = message;
            return RedirectToAction("Checkout", "Home");
        }
        #endregion

        #region // View Pizza Order Action
        [Authorize]
        [HttpGet]
        public ActionResult ViewPizzaOrder(int id)
        {
            string message = "";
            Pizza pizza = null;
            using (DBEntities de = new DBEntities())
            {
                var order = de.Orders.Where(a => a.OrderID == id).FirstOrDefault();
                if (order != null)
                {
                    message = "Detailed information about your pizza order.";
                    pizza = order.Pizza;
                    ViewBag.Quantity = order.PizzaAmount;
                    if (order.PromoPercent != null)
                    {
                        if (!ViewData.ContainsKey("pizzaOldPrice"))
                        {
                            ViewData.Add("pizzaOldPrice", order.Pizza.PizzaPrice);
                        }
                        if (!ViewData.ContainsKey("pizzaPrice"))
                        {
                            ViewData.Add("pizzaPrice", order.Pizza.PizzaPrice - (order.PromoPercent.GetValueOrDefault() / 100 * order.Pizza.PizzaPrice));
                        }
                        if (!ViewData.ContainsKey("oldTotalPizzaPrice"))
                        {
                            ViewData.Add("oldTotalPizzaPrice", order.Pizza.PizzaPrice * order.PizzaAmount);
                        }
                        if (!ViewData.ContainsKey("totalPizzaPrice"))
                        {
                            ViewData.Add("totalPizzaPrice", (order.Pizza.PizzaPrice - (order.PromoPercent.GetValueOrDefault() / 100 * order.Pizza.PizzaPrice)) * order.PizzaAmount);
                        }
                    }
                    else
                    {
                        if (!ViewData.ContainsKey("pizzaPrice"))
                        {
                            ViewData.Add("pizzaPrice", order.Pizza.PizzaPrice);
                        }
                        if (!ViewData.ContainsKey("totalPizzaPrice"))
                        {
                            ViewData.Add("totalPizzaPrice", order.Pizza.PizzaPrice * order.PizzaAmount);
                        }
                    }
                    ViewBag.Message = message;
                    return View(pizza);
                }
            }

            ViewBag.Message = message;
            return View();
        }
        #endregion

        #region // View Drink Order Action
        [Authorize]
        [HttpGet]
        public ActionResult ViewDrinkOrder(int id)
        {
            string message = "";
            Drink drink = null;
            using (DBEntities de = new DBEntities())
            {
                var order = de.Orders.Where(a => a.OrderID == id).FirstOrDefault();
                if (order != null)
                {
                    message = "Detailed information about your drink order.";
                    drink = order.Drink;
                    ViewBag.Quantity = order.DrinkAmount;
                    if (order.PromoPercent != null)
                    {
                        if (!ViewData.ContainsKey("drinkOldPrice"))
                        {
                            ViewData.Add("drinkOldPrice", order.Drink.DrinkPrice);
                        }
                        if (!ViewData.ContainsKey("drinkPrice"))
                        {
                            ViewData.Add("drinkPrice", order.Drink.DrinkPrice - (order.PromoPercent.GetValueOrDefault() / 100 * order.Drink.DrinkPrice));
                        }
                        if (!ViewData.ContainsKey("oldTotalDrinkPrice"))
                        {
                            ViewData.Add("oldTotalDrinkPrice", order.Drink.DrinkPrice * order.DrinkAmount);
                        }
                        if (!ViewData.ContainsKey("totalDrinkPrice"))
                        {
                            ViewData.Add("totalDrinkPrice", (order.Drink.DrinkPrice - (order.PromoPercent.GetValueOrDefault() / 100 * order.Drink.DrinkPrice)) * order.DrinkAmount);
                        }
                    }
                    else
                    {
                        if (!ViewData.ContainsKey("drinkPrice"))
                        {
                            ViewData.Add("drinkPrice", order.Drink.DrinkPrice);
                        }
                        if (!ViewData.ContainsKey("totalDrinkPrice"))
                        {
                            ViewData.Add("totalDrinkPrice", order.Drink.DrinkPrice * order.DrinkAmount);
                        }
                    }
                    ViewBag.Message = message;
                    return View(drink);
                }
            }

            ViewBag.Message = message;
            return View();
        }
        #endregion

        #region // View Dessert Order Action
        [Authorize]
        [HttpGet]
        public ActionResult ViewDessertOrder(int id)
        {
            string message = "";
            Dessert dessert = null;
            using (DBEntities de = new DBEntities())
            {
                var order = de.Orders.Where(a => a.OrderID == id).FirstOrDefault();
                if (order != null)
                {
                    message = "Detailed information about your dessert order.";
                    dessert = order.Dessert;
                    ViewBag.Quantity = order.DessertAmount;
                    if (order.PromoPercent != null)
                    {
                        if (!ViewData.ContainsKey("dessertOldPrice"))
                        {
                            ViewData.Add("dessertOldPrice", order.Dessert.DessertPrice);
                        }
                        if (!ViewData.ContainsKey("dessertPrice"))
                        {
                            ViewData.Add("dessertPrice", order.Dessert.DessertPrice - (order.PromoPercent.GetValueOrDefault() / 100 * order.Dessert.DessertPrice));
                        }
                        if (!ViewData.ContainsKey("oldTotalDessertPrice"))
                        {
                            ViewData.Add("oldTotalDessertPrice", order.Dessert.DessertPrice * order.DessertAmount);
                        }
                        if (!ViewData.ContainsKey("totalDessertPrice"))
                        {
                            ViewData.Add("totalDessertPrice", (order.Dessert.DessertPrice - (order.PromoPercent.GetValueOrDefault() / 100 * order.Dessert.DessertPrice)) * order.DessertAmount);
                        }
                    }
                    else
                    {
                        if (!ViewData.ContainsKey("dessertPrice"))
                        {
                            ViewData.Add("dessertPrice", order.Dessert.DessertPrice);
                        }
                        if (!ViewData.ContainsKey("totalDessertPrice"))
                        {
                            ViewData.Add("totalDessertPrice", order.Dessert.DessertPrice * order.DessertAmount);
                        }
                    }
                    ViewBag.Message = message;
                    return View(dessert);
                }
            }

            ViewBag.Message = message;
            return View();
        }
        #endregion

        #region // Remove Pizza Order Action
        [Authorize]
        [HttpGet]
        public ActionResult RemovePizzaOrder(int id)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User currentUser = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot remove your orders until you verify your account!";
                }
                else
                {
                    var order = de.Orders.Where(a => a.OrderID == id).FirstOrDefault();
                    if (order != null)
                    {
                        if (order.PizzaAmount > 1)
                        {
                            message = "Pizza order successfully updated!";
                            order.PizzaAmount -= 1;
                        }
                        else
                        {
                            // Remove any custom recipe and pizza the user may have created.
                            var pizza = order.Pizza;
                            var recipe = pizza.Recipe;
                            if (pizza != null)
                            {
                                if (pizza.PizzaName.StartsWith("Custom: "))
                                {
                                    de.Pizzas.Remove(pizza);
                                    if (recipe != null)
                                    {
                                        var recipeIngredients = recipe.RecipesIngredients;
                                        de.RecipesIngredients.RemoveRange(recipeIngredients);
                                        de.Recipes.Remove(recipe);
                                    }
                                }
                            }
                            message = "Pizza order successfully removed!";
                            de.Orders.Remove(order);
                            de.SaveChanges();
                            if (de.Orders.Count() > 0)
                            {
                                if (de.Orders.Where(a => a.UserID == currentUser.UserID).Count() == 0)
                                {
                                    currentUser.CanUsePromoCodes = true;
                                    de.Configuration.ValidateOnSaveEnabled = false;
                                }
                            }
                            else
                            {
                                currentUser.CanUsePromoCodes = true;
                                de.Configuration.ValidateOnSaveEnabled = false;
                            }
                        }
                        de.SaveChanges();
                    }
                }
            }

            TempData["orderInfo"] = message;
            return RedirectToAction("Checkout", "Home");
        }
        #endregion

        #region // Remove Drink Order Action
        [Authorize]
        [HttpGet]
        public ActionResult RemoveDrinkOrder(int id)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User currentUser = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot remove your orders until you verify your account!";
                }
                else
                {
                    var order = de.Orders.Where(a => a.OrderID == id).FirstOrDefault();
                    if (order != null)
                    {
                        if (order.DrinkAmount > 1)
                        {
                            message = "Drink order successfully updated!";
                            order.DrinkAmount -= 1;
                        }
                        else
                        {
                            message = "Drink order successfully removed!";
                            de.Orders.Remove(order);
                            de.SaveChanges();
                            if (de.Orders.Count() > 0)
                            {
                                if (de.Orders.Where(a => a.UserID == currentUser.UserID).Count() == 0)
                                {
                                    currentUser.CanUsePromoCodes = true;
                                    de.Configuration.ValidateOnSaveEnabled = false;
                                }
                            }
                            else
                            {
                                currentUser.CanUsePromoCodes = true;
                                de.Configuration.ValidateOnSaveEnabled = false;
                            }
                        }
                        de.SaveChanges();
                    }
                }
            }

            TempData["orderInfo"] = message;
            return RedirectToAction("Checkout", "Home");
        }
        #endregion

        #region // Remove Dessert Order Action
        [Authorize]
        [HttpGet]
        public ActionResult RemoveDessertOrder(int id)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User currentUser = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot remove your orders until you verify your account!";
                }
                else
                {
                    var order = de.Orders.Where(a => a.OrderID == id).FirstOrDefault();
                    if (order != null)
                    {
                        if (order.DessertAmount > 1)
                        {
                            message = "Dessert order successfully updated!";
                            order.DessertAmount -= 1;
                        }
                        else
                        {
                            message = "Dessert order successfully removed!";
                            de.Orders.Remove(order);
                            de.SaveChanges();
                            if (de.Orders.Count() > 0)
                            {
                                if (de.Orders.Where(a => a.UserID == currentUser.UserID).Count() == 0)
                                {
                                    currentUser.CanUsePromoCodes = true;
                                    de.Configuration.ValidateOnSaveEnabled = false;
                                }
                            }
                            else
                            {
                                currentUser.CanUsePromoCodes = true;
                                de.Configuration.ValidateOnSaveEnabled = false;
                            }
                        }
                        de.SaveChanges();
                    }
                }
            }

            TempData["orderInfo"] = message;
            return RedirectToAction("Checkout", "Home");
        }
        #endregion

        #region // Checkout Completed Action
        [Authorize]
        [HttpGet]
        public ActionResult CheckoutCompleted()
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                var currentUserEmailIdentity = HttpContext.User.Identity.Name;
                var currentUser = de.Users.Where(a => a.EmailID == currentUserEmailIdentity).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot checkout until you verify your account!";
                }
                else
                {
                    var ordersByCurrentUser = de.Orders.Where(a => a.UserID == currentUser.UserID);
                    if (ordersByCurrentUser.Count() > 0)
                    {
                        foreach (var order in ordersByCurrentUser)
                        {
                            if (order != null)
                            {
                                OrderHistory orderHistory = new OrderHistory();
                                orderHistory.UserID = currentUser.UserID;
                                if (order.Pizza != null)
                                {
                                    orderHistory.PizzaID = order.Pizza.PizzaID;
                                    orderHistory.PizzaName = order.Pizza.PizzaName;
                                    if (order.PromoPercent != null)
                                    {
                                        orderHistory.PizzaPrice = order.Pizza.PizzaPrice - (order.PromoPercent / 100 * order.Pizza.PizzaPrice);
                                    }
                                    else
                                    {
                                        orderHistory.PizzaPrice = order.Pizza.PizzaPrice;
                                    }
                                    orderHistory.PizzaAmount = order.PizzaAmount;
                                    orderHistory.PizzaSize = order.Pizza.PizzaSize;
                                    orderHistory.PizzaPicturePath = order.Pizza.PizzaPicturePath;
                                }
                                if (order.Drink != null)
                                {
                                    orderHistory.DrinkID = order.Drink.DrinkID;
                                    orderHistory.DrinkName = order.Drink.DrinkName;
                                    if (order.PromoPercent != null)
                                    {
                                        orderHistory.DrinkPrice = order.Drink.DrinkPrice - (order.PromoPercent / 100 * order.Drink.DrinkPrice);
                                    }
                                    else
                                    {
                                        orderHistory.DrinkPrice = order.Drink.DrinkPrice;
                                    }
                                    orderHistory.DrinkAmount = order.DrinkAmount;
                                    orderHistory.DrinkSize = order.Drink.DrinkSize;
                                    orderHistory.DrinkPicturePath = order.Drink.DrinkPicturePath;
                                }
                                if (order.Dessert != null)
                                {
                                    orderHistory.DessertID = order.Dessert.DessertID;
                                    orderHistory.DessertName = order.Dessert.DessertName;
                                    if (order.PromoPercent != null)
                                    {
                                        orderHistory.DessertPrice = order.Dessert.DessertPrice - (order.PromoPercent / 100 * order.Dessert.DessertPrice);
                                    }
                                    else
                                    {
                                        orderHistory.DessertPrice = order.Dessert.DessertPrice;
                                    }
                                    orderHistory.DessertAmount = order.DessertAmount;
                                    orderHistory.DessertSize = order.Dessert.DessertSize;
                                    orderHistory.DessertPicturePath = order.Dessert.DessertPicturePath;
                                }
                                orderHistory.Timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                                de.OrderHistories.Add(orderHistory);

                                if (order.Pizza != null && order.Pizza.PizzaName.StartsWith("Custom: "))
                                {
                                    // Remove any custom recipe and pizza the user may have created.
                                    var pizza = order.Pizza;
                                    var recipe = pizza.Recipe;
                                    if (pizza != null)
                                    {
                                        if (pizza.PizzaName.StartsWith("Custom: "))
                                        {
                                            de.Pizzas.Remove(pizza);
                                        }
                                    }
                                    if (recipe != null)
                                    {
                                        var recipeIngredients = recipe.RecipesIngredients;
                                        de.RecipesIngredients.RemoveRange(recipeIngredients);
                                        de.Recipes.Remove(recipe);
                                    }
                                }
                                de.Orders.Remove(order);
                            }
                        }
                        message = "Your orders are on their way to your address at " + currentUser.Address + "!";

                        if (currentUser.CurrentPromoCode != null)
                        {
                            Promo promo = de.Promos.Where(a => a.PromoCode.Equals(currentUser.CurrentPromoCode)).FirstOrDefault();
                            var userOrders = de.Orders.Where(a => a.UserID == currentUser.UserID);
                            foreach (var order in userOrders)
                            {
                                if (order.PromoPercent != null)
                                {
                                    if (order.PromoPercent == promo.PromoPercent)
                                    {
                                        de.Promos.Remove(promo);
                                        currentUser.CurrentPromoCode = null;
                                    }
                                }
                            }
                        }
                        de.Configuration.ValidateOnSaveEnabled = false;
                        de.SaveChanges();

                        currentUser.CanUsePromoCodes = true;

                        if ((currentUser.CurrentPromoCode == null) || (currentUser.CurrentPromoCode == ""))
                        {
                            foreach (var promo in de.Promos.OrderBy(x => Guid.NewGuid()).ToList())
                            {
                                bool promoCodeFree = true;
                                foreach (var user in de.Users)
                                {
                                    if (user.CurrentPromoCode != null)
                                    {
                                        if (user.CurrentPromoCode.Equals(promo.PromoCode))
                                        {
                                            promoCodeFree = false;
                                            break;
                                        }
                                    }
                                }
                                if (promoCodeFree == true)
                                {
                                    currentUser.CurrentPromoCode = promo.PromoCode;
                                    ViewBag.Message2 = "Promo Code: " + promo.PromoCode + "<br/>Category: " + promo.ItemType +"<br/>Product: " + promo.ItemName + "<br/>Size: " + promo.ItemSize + "<br/>Discount: " + promo.PromoPercent + "%";
                                    break;
                                }
                            }
                        }
                        de.Configuration.ValidateOnSaveEnabled = false;
                        de.SaveChanges();
                    }
                    else
                    {
                        message = "You have not placed any orders yet!";
                    }
                }
            }

            ViewBag.Message = message;
            return View();
        }
        #endregion

        #region // Order History Action
        [Authorize]
        [HttpGet]
        public ActionResult OrderHistory()
        {
            string message = "";
            List<OrderHistory> orderHistoryList = new List<OrderHistory>();
            double totalOrderPrice = 0.0;
            using (DBEntities de = new DBEntities())
            {
                var currentUserEmailIdentity = HttpContext.User.Identity.Name;
                var currentUser = de.Users.Where(a => a.EmailID == currentUserEmailIdentity).FirstOrDefault();
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
                    message = "There is " + numberOfOrders.ToString() + " order in your order history!";
                }
                else if (numberOfOrders > 1)
                {
                    message = "There are " + numberOfOrders.ToString() + " orders in your order history!";
                }
                else
                {
                    message = "Your order history is empty!";
                }
            }

            var ordersGroupedByTimestamp = orderHistoryList.OrderBy(item => item.Timestamp).GroupBy(item => item.Timestamp).OrderByDescending(item => DateTime.Parse(item.Key));
            GroupedOrderHistoryModel groupedOrderHistoryModel = new GroupedOrderHistoryModel();
            groupedOrderHistoryModel.groupedOrderHistoryList = ordersGroupedByTimestamp;

            if (TempData["showPizzasOrderAgainInfo"] != null)
            {
                ViewBag.Message2 = TempData["showPizzasOrderAgainInfo"].ToString();
            }

            if (TempData["showDrinksOrderAgainInfo"] != null)
            {
                ViewBag.Message2 = TempData["showDrinksOrderAgainInfo"].ToString();
            }

            if (TempData["showDessertsOrderAgainInfo"] != null)
            {
                ViewBag.Message2 = TempData["showDessertsOrderAgainInfo"].ToString();
            }

            ViewBag.Message = message;
            return View(groupedOrderHistoryModel);
        }
        #endregion

        #region // Order Pizza Again From History
        [Authorize]
        [HttpGet]
        public ActionResult OrderPizzaAgain(int id, int amount)
        {
            string message = "";
            bool canUploadNew = false;
            using (DBEntities de = new DBEntities())
            {
                var currentUserEmailIdentity = HttpContext.User.Identity.Name;
                var currentUser = de.Users.Where(a => a.EmailID == currentUserEmailIdentity).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot order anything until you verify your account!";
                }
                else
                {
                    var pizza = de.Pizzas.Where(a => a.PizzaID == id).FirstOrDefault();
                    if (pizza != null)
                    {
                        if (de.Orders.Count() > 0)
                        {
                            foreach (var ordr in de.Orders)
                            {
                                if (ordr != null)
                                {
                                    if ((ordr.Pizza != null) && (ordr.UserID == currentUser.UserID) && (ordr.Pizza.PizzaName.Equals(pizza.PizzaName)) && (ordr.Pizza.PizzaSize.Equals(pizza.PizzaSize)))
                                    {
                                        ordr.PizzaAmount += 1;
                                        message = "Pizza order successfully updated!";
                                        canUploadNew = false;
                                        break;
                                    }
                                    else
                                    {
                                        canUploadNew = true;
                                    }
                                }
                            }
                            de.SaveChanges();
                            if (canUploadNew == true)
                            {
                                Order order = new Order();
                                order.Pizza = pizza;
                                if (currentUser != null)
                                {
                                    order.User = currentUser;
                                }
                                order.PizzaAmount = amount;
                                de.Orders.Add(order);
                                message = "Pizza successfully added to cart!";
                                de.SaveChanges();
                            }
                        }
                        else
                        {
                            Order order = new Order();
                            order.Pizza = pizza;
                            if (currentUser != null)
                            {
                                order.User = currentUser;
                            }
                            order.PizzaAmount = amount;
                            de.Orders.Add(order);
                            message = "Pizza successfully added to cart!";
                            de.SaveChanges();
                        }
                    }
                }

                TempData["showPizzasOrderAgainInfo"] = message;
                return RedirectToAction("OrderHistory", "Home");
            }
        }
        #endregion

        #region // Order Drink Again From History
        [Authorize]
        [HttpGet]
        public ActionResult OrderDrinkAgain(int id, int amount)
        {
            string message = "";
            bool canUploadNew = false;
            using (DBEntities de = new DBEntities())
            {
                var currentUserEmailIdentity = HttpContext.User.Identity.Name;
                var currentUser = de.Users.Where(a => a.EmailID == currentUserEmailIdentity).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot order anything until you verify your account!";
                }
                else
                {
                    var drink = de.Drinks.Where(a => a.DrinkID == id).FirstOrDefault();
                    if (drink != null)
                    {
                        if (de.Orders.Count() > 0)
                        {
                            foreach (var ordr in de.Orders)
                            {
                                if (ordr != null)
                                {
                                    if ((ordr.Drink != null) && (ordr.UserID == currentUser.UserID) && (ordr.Drink.DrinkName.Equals(drink.DrinkName)) && (ordr.Drink.DrinkSize.Equals(drink.DrinkSize)))
                                    {
                                        ordr.DrinkAmount += 1;
                                        message = "Drink order successfully updated!";
                                        canUploadNew = false;
                                        break;
                                    }
                                    else
                                    {
                                        canUploadNew = true;
                                    }
                                }
                            }
                            de.SaveChanges();
                            if (canUploadNew == true)
                            {
                                Order order = new Order();
                                order.Drink = drink;
                                if (currentUser != null)
                                {
                                    order.User = currentUser;
                                }
                                order.DrinkAmount = amount;
                                de.Orders.Add(order);
                                message = "Drink successfully added to cart!";
                                de.SaveChanges();
                            }
                        }
                        else
                        {
                            Order order = new Order();
                            order.Drink = drink;
                            if (currentUser != null)
                            {
                                order.User = currentUser;
                            }
                            order.DrinkAmount = amount;
                            de.Orders.Add(order);
                            message = "Drink successfully added to cart!";
                            de.SaveChanges();
                        }
                    }
                }
            }

            TempData["showDrinksOrderAgainInfo"] = message;
            return RedirectToAction("OrderHistory", "Home");
        }
        #endregion

        #region // Order Dessert Again From History
        [Authorize]
        [HttpGet]
        public ActionResult OrderDessertAgain(int id, int amount)
        {
            string message = "";
            bool canUploadNew = false;
            using (DBEntities de = new DBEntities())
            {
                var currentUserEmailIdentity = HttpContext.User.Identity.Name;
                var currentUser = de.Users.Where(a => a.EmailID == currentUserEmailIdentity).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot order anything until you verify your account!";
                }
                else
                {
                    var dessert = de.Desserts.Where(a => a.DessertID == id).FirstOrDefault();
                    if (dessert != null)
                    {
                        if (de.Orders.Count() > 0)
                        {
                            foreach (var ordr in de.Orders)
                            {
                                if (ordr != null)
                                {
                                    if ((ordr.Dessert != null) && (ordr.UserID == currentUser.UserID) && (ordr.Dessert.DessertName.Equals(dessert.DessertName)) && (ordr.Dessert.DessertSize.Equals(dessert.DessertSize)))
                                    {
                                        ordr.DessertAmount += 1;
                                        message = "Dessert order successfully updated!";
                                        canUploadNew = false;
                                        break;
                                    }
                                    else
                                    {
                                        canUploadNew = true;
                                    }
                                }
                            }
                            de.SaveChanges();
                            if (canUploadNew == true)
                            {
                                Order order = new Order();
                                order.Dessert = dessert;
                                if (currentUser != null)
                                {
                                    order.User = currentUser;
                                }
                                order.DessertAmount = amount;
                                de.Orders.Add(order);
                                message = "Dessert successfully added to cart!";
                                de.SaveChanges();
                            }
                        }
                        else
                        {
                            Order order = new Order();
                            order.Dessert = dessert;
                            if (currentUser != null)
                            {
                                order.User = currentUser;
                            }
                            order.DessertAmount = amount;
                            de.Orders.Add(order);
                            message = "Dessert successfully added to cart!";
                            de.SaveChanges();
                        }
                    }
                }
            }

            TempData["showDessertsOrderAgainInfo"] = message;
            return RedirectToAction("OrderHistory", "Home");
        }
        #endregion

        #region // Terms & Conditions Action
        [HttpGet]
        public ActionResult TermsAndConditions()
        {
            return View();
        }
        #endregion

        #region // Not Found Action
        [HttpGet]
        public ActionResult NotFound()
        {
            return View();
        }
        #endregion
    }
}