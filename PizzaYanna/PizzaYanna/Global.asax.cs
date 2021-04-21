using PizzaYanna.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PizzaYanna
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            /*var trigger = new DailyTrigger(23, 59, 59); // Check every day at 23:59:59
            trigger.OnTimeTriggered += () =>
            {
                var currentDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss").Substring(0, 10);
                using (DBEntities de = new DBEntities())
                {
                    var allPromos = de.Promos;
                    foreach (var promo in allPromos)
                    {
                        if (DateTime.Parse(currentDate) >= DateTime.Parse(promo.ItemDeadline.Substring(0, 10)))
                        {
                            User user = de.Users.Where(a => a.CurrentPromoCode.Equals(promo.PromoCode)).FirstOrDefault();
                            if (user != null)
                            {
                                user.CurrentPromoCode = null;
                            }
                            de.Promos.Remove(promo);
                        }
                    }
					de.Configuration.ValidateOnSaveEnabled = false; // Added to avoid issue with confirm password match on save changes | Had to use this to make user updating part above work
                    de.SaveChanges();
                }
            };*/
        }

        void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            var currentDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            using (DBEntities de = new DBEntities())
            {
                foreach (var promo in de.Promos)
                {
                    if (DateTime.Parse(currentDate) > DateTime.Parse(promo.ItemDeadline))
                    {
                        User user = de.Users.Where(a => a.CurrentPromoCode.Equals(promo.PromoCode)).FirstOrDefault();
                        if (user != null)
                        {
                            user.CurrentPromoCode = null;
                        }
                        de.Promos.Remove(promo);
                    }
                }
                de.Configuration.ValidateOnSaveEnabled = false; // Added to avoid issue with confirm password match on save changes | Had to use this to make user updating part above work
                de.SaveChanges();
            }
        }
    }
}
