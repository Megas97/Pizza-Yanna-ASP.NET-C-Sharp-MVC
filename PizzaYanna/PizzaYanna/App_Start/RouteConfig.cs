using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PizzaYanna
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Promotions
            routes.MapRoute(
                name: "Promotions",
                url: "promotions",
                defaults: new { controller = "Home", action = "Promotions", id = UrlParameter.Optional }
            );

            // Show Pizzas
            routes.MapRoute(
                name: "Pizzas",
                url: "pizzas",
                defaults: new { controller = "Home", action = "ShowPizzas", id = UrlParameter.Optional }
            );

            // Show Drinks
            routes.MapRoute(
                name: "Drinks",
                url: "drinks",
                defaults: new { controller = "Home", action = "ShowDrinks", id = UrlParameter.Optional }
            );

            // Show Desserts
            routes.MapRoute(
                name: "Desserts",
                url: "desserts",
                defaults: new { controller = "Home", action = "ShowDesserts", id = UrlParameter.Optional }
            );

            // My Profile
            routes.MapRoute(
                name: "My Profile",
                url: "my-profile",
                defaults: new { controller = "User", action = "ViewMyProfile", id = UrlParameter.Optional }
            );

            // Edit Profile
            routes.MapRoute(
               name: "Edit Profile",
               url: "edit-profile",
               defaults: new { controller = "User", action = "EditProfile", id = UrlParameter.Optional }
            );

            // Order History
            routes.MapRoute(
               name: "Order History",
               url: "order-history",
               defaults: new { controller = "Home", action = "OrderHistory", id = UrlParameter.Optional }
            );

            // Order Pizza Again
            routes.MapRoute(
               name: "Order Pizza Again",
               url: "order-pizza-again-{id}-{amount}",
               defaults: new { controller = "Home", action = "OrderPizzaAgain", id = UrlParameter.Optional, amount = UrlParameter.Optional }
            );

            // Order Drink Again
            routes.MapRoute(
               name: "Order Drink Again",
               url: "order-drink-again-{id}-{amount}",
               defaults: new { controller = "Home", action = "OrderDrinkAgain", id = UrlParameter.Optional, amount = UrlParameter.Optional }
            );

            // Order Dessert Again
            routes.MapRoute(
               name: "Order Dessert Again",
               url: "order-dessert-again-{id}-{amount}",
               defaults: new { controller = "Home", action = "OrderDessertAgain", id = UrlParameter.Optional, amount = UrlParameter.Optional }
            );

            // About
            routes.MapRoute(
               name: "About",
               url: "about",
               defaults: new { controller = "Home", action = "About", id = UrlParameter.Optional }
            );

            // Contacts
            routes.MapRoute(
               name: "Contacts",
               url: "contacts",
               defaults: new { controller = "Home", action = "Contacts", id = UrlParameter.Optional }
            );

            // Terms & Conditions
            routes.MapRoute(
               name: "Terms & Conditions",
               url: "terms-and-conditions",
               defaults: new { controller = "Home", action = "TermsAndConditions", id = UrlParameter.Optional }
            );

            // Admin Panel
            routes.MapRoute(
               name: "Admin Panel",
               url: "admin",
               defaults: new { controller = "Admin", action = "AdminPanel", id = UrlParameter.Optional }
            );

            // Upload Ingredient
            routes.MapRoute(
               name: "Upload Ingredient",
               url: "admin/upload-ingredient",
               defaults: new { controller = "Admin", action = "UploadIngredient", id = UrlParameter.Optional }
            );

            // Upload Recipe
            routes.MapRoute(
               name: "Upload Recipe",
               url: "admin/upload-recipe",
               defaults: new { controller = "Admin", action = "UploadRecipe", id = UrlParameter.Optional }
            );

            // Upload Pizza
            routes.MapRoute(
               name: "Upload Pizza",
               url: "admin/upload-pizza",
               defaults: new { controller = "Admin", action = "UploadPizza", id = UrlParameter.Optional }
            );

            // Upload Drink
            routes.MapRoute(
               name: "Upload Drink",
               url: "admin/upload-drink",
               defaults: new { controller = "Admin", action = "UploadDrink", id = UrlParameter.Optional }
            );

            // Upload Dessert
            routes.MapRoute(
               name: "Upload Dessert",
               url: "admin/upload-dessert",
               defaults: new { controller = "Admin", action = "UploadDessert", id = UrlParameter.Optional }
            );

            // Manage Users
            routes.MapRoute(
               name: "Manage Users",
               url: "admin/manage-users",
               defaults: new { controller = "Admin", action = "ManageUsers", id = UrlParameter.Optional }
            );

            // User Order History
            routes.MapRoute(
               name: "User Order History",
               url: "admin/manage-users/user-order-history-{id}",
               defaults: new { controller = "Admin", action = "UserOrderHistory", id = UrlParameter.Optional }
            );

            // View Statistics
            routes.MapRoute(
               name: "View Statistics",
               url: "admin/view-statistics",
               defaults: new { controller = "Admin", action = "ViewStatistics", id = UrlParameter.Optional }
            );

            // View Addresses
            routes.MapRoute(
               name: "View Addresses",
               url: "admin/view-statistics/view-addresses",
               defaults: new { controller = "Admin", action = "ViewAddresses", id = UrlParameter.Optional }
            );

            // View Total Pizzas Sold
            routes.MapRoute( // http://localhost:59005/Admin/ViewTotalPizzasSold?date=20.07.2020&startTime=13%3A32%3A00&endTime=13%3A32%3A00 -- Remove the optional attributes for such parameter URLs
               name: "View Total Pizzas Sold",
               url: "admin/view-statistics/total-pizzas-sold",
               defaults: new { controller = "Admin", action = "ViewTotalPizzasSold"}
            );

            // View Total Drinks Sold
            routes.MapRoute( // http://localhost:59005/Admin/ViewTotalDrinksSold?date=20.07.2020&startTime=13%3A32%3A00&endTime=13%3A32%3A00 -- Remove the optional attributes for such parameter URLs
               name: "View Total Drinks Sold",
               url: "admin/view-statistics/total-drinks-sold",
               defaults: new { controller = "Admin", action = "ViewTotalDrinksSold" }
            );

            // View Total Desserts Sold
            routes.MapRoute( // http://localhost:59005/Admin/ViewTotalDessertsSold?date=20.07.2020&startTime=13%3A32%3A00&endTime=13%3A32%3A00 -- Remove the optional attributes for such parameter URLs
               name: "View Total Desserts Sold",
               url: "admin/view-statistics/total-desserts-sold",
               defaults: new { controller = "Admin", action = "ViewTotalDessertsSold" }
            );

            // Manage Promotions
            routes.MapRoute(
               name: "Manage Promotions",
               url: "admin/manage-promotions",
               defaults: new { controller = "Admin", action = "ManagePromotions", id = UrlParameter.Optional }
            );

            // Delete Ingredient
            routes.MapRoute(
               name: "Delete Ingredient",
               url: "admin/delete-ingredient",
               defaults: new { controller = "Admin", action = "DeleteIngredient", id = UrlParameter.Optional }
            );

            // Delete Recipe
            routes.MapRoute(
               name: "Delete Recipe",
               url: "admin/delete-recipe",
               defaults: new { controller = "Admin", action = "DeleteRecipe", id = UrlParameter.Optional }
            );

            // Delete Pizza
            routes.MapRoute(
               name: "Delete Pizza",
               url: "admin/delete-pizza",
               defaults: new { controller = "Admin", action = "DeletePizza", id = UrlParameter.Optional }
            );

            // Delete Drink
            routes.MapRoute(
               name: "Delete Drink",
               url: "admin/delete-drink",
               defaults: new { controller = "Admin", action = "DeleteDrink", id = UrlParameter.Optional }
            );

            // Delete Dessert
            routes.MapRoute(
               name: "Delete Dessert",
               url: "admin/delete-dessert",
               defaults: new { controller = "Admin", action = "DeleteDessert", id = UrlParameter.Optional }
            );

            // Access Denied
            routes.MapRoute(
               name: "Access Denied",
               url: "admin/access-denied",
               defaults: new { controller = "Admin", action = "AccessDenied", id = UrlParameter.Optional }
            );

            // Checkout
            routes.MapRoute(
               name: "Checkout",
               url: "checkout",
               defaults: new { controller = "Home", action = "Checkout", id = UrlParameter.Optional }
            );

            // Checkout Completed
            routes.MapRoute(
               name: "Checkout Completed",
               url: "checkout/completed",
               defaults: new { controller = "Home", action = "CheckoutCompleted", id = UrlParameter.Optional }
            );

            // Register
            routes.MapRoute(
               name: "Register",
               url: "register/{id}",
               defaults: new { controller = "User", action = "Register", id = UrlParameter.Optional }
            );

            // Login
            routes.MapRoute(
               name: "Login",
               url: "login/{id}",
               defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional }
            );

            // Verify Account
            routes.MapRoute(
               name: "Verify Account",
               url: "verify-account/{id}",
               defaults: new { controller = "User", action = "VerifyAccount", id = UrlParameter.Optional }
            );

            // Forgot Password
            routes.MapRoute(
               name: "Forgot Password",
               url: "forgot-password/{id}",
               defaults: new { controller = "User", action = "ForgotPassword", id = UrlParameter.Optional }
            );

            // Reset Password
            routes.MapRoute(
               name: "Reset Password",
               url: "reset-password/{id}",
               defaults: new { controller = "User", action = "ResetPassword", id = UrlParameter.Optional }
            );

            // Create Order With Pizza
            routes.MapRoute(
               name: "Create Order With Pizza",
               url: "create-pizza-order-{id}",
               defaults: new { controller = "Home", action = "CreateOrderWithPizza", id = UrlParameter.Optional }
            );

            // Create Custom Pizza
            routes.MapRoute(
               name: "Create Custom Pizza",
               url: "create-custom-pizza-{id}",
               defaults: new { controller = "Home", action = "CreateCustomPizza", id = UrlParameter.Optional }
            );

            // Create Order With Drink
            routes.MapRoute(
               name: "Create Order With Drink",
               url: "create-drink-order-{id}",
               defaults: new { controller = "Home", action = "CreateOrderWithDrink", id = UrlParameter.Optional }
            );

            // Create Order With Dessert
            routes.MapRoute(
               name: "Create Order With Dessert",
               url: "create-dessert-order-{id}",
               defaults: new { controller = "Home", action = "CreateOrderWithDessert", id = UrlParameter.Optional }
            );

            // Add Pizza Order
            routes.MapRoute(
               name: "Add Pizza Order",
               url: "add-pizza-order-{id}",
               defaults: new { controller = "Home", action = "AddPizzaOrder", id = UrlParameter.Optional }
            );

            // View Pizza Order
            routes.MapRoute(
               name: "View Pizza Order",
               url: "view-pizza-order-{id}",
               defaults: new { controller = "Home", action = "ViewPizzaOrder", id = UrlParameter.Optional }
            );

            // Remove Pizza Order
            routes.MapRoute(
               name: "Remove Pizza Order",
               url: "remove-pizza-order-{id}",
               defaults: new { controller = "Home", action = "RemovePizzaOrder", id = UrlParameter.Optional }
            );

            // Add Drink Order
            routes.MapRoute(
               name: "Add Drink Order",
               url: "add-drink-order-{id}",
               defaults: new { controller = "Home", action = "AddDrinkOrder", id = UrlParameter.Optional }
            );

            // View Drink Order
            routes.MapRoute(
               name: "View Drink Order",
               url: "view-drink-order-{id}",
               defaults: new { controller = "Home", action = "ViewDrinkOrder", id = UrlParameter.Optional }
            );

            // Remove Drink Order
            routes.MapRoute(
               name: "Remove Drink Order",
               url: "remove-drink-order-{id}",
               defaults: new { controller = "Home", action = "RemoveDrinkOrder", id = UrlParameter.Optional }
            );

            // Add Dessert Order
            routes.MapRoute(
               name: "Add Dessert Order",
               url: "add-dessert-order-{id}",
               defaults: new { controller = "Home", action = "AddDessertOrder", id = UrlParameter.Optional }
            );

            // View Dessert Order
            routes.MapRoute(
               name: "View Dessert Order",
               url: "view-dessert-order-{id}",
               defaults: new { controller = "Home", action = "ViewDessertOrder", id = UrlParameter.Optional }
            );

            // Remove Dessert Order
            routes.MapRoute(
               name: "Remove Dessert Order",
               url: "remove-dessert-order-{id}",
               defaults: new { controller = "Home", action = "RemoveDessertOrder", id = UrlParameter.Optional }
            );

            // Not Found
            routes.MapRoute(
                name: "Not Found",
                url: "not-found",
                defaults: new { controller = "Home", action = "NotFound", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
