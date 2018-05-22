using MinaReceptLibrary;
using MinaReceptLibrary.Database;
using MinaReceptLibrary.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MinaReceptWebb.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            HttpCookie cookie = Request.Cookies["User"];

            RecipeRepository repo = new RecipeRepository(ConnectionString);

            if (cookie != null)
            {
                string mail = cookie.Values["mail"];

                if (!String.IsNullOrEmpty(mail))
                {
                    User user = repo.GetUserByMail(mail);

                    if (user != null)
                    {
                        Session["User"] = user;
                    }
                }

            }

            

            ViewBag.Recipies = repo.GetRecipies();

            ViewBag.Count = repo.GetCountRecipies();

            List<BaseItem> meals = repo.GetMealType();

            meals.Insert(0, new BaseItem { Id = -1, Name = "-" });

            ViewBag.Meals = meals;
            
            return View();
        }

        public ActionResult Add()
        {
            RecipeRepository repo = new RecipeRepository(ConnectionString);

            ViewBag.Types = repo.GetTypes();
            ViewBag.Origin = repo.GetOrigin();
            ViewBag.Meal = repo.GetMealType();

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            HttpCookie cookie = Request.Cookies["User"];

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
            }
            
            if (Session["User"] != null)
            {
                Session["User"] = null;
            }

            RecipeRepository repo = new RecipeRepository(ConnectionString);

            ViewBag.Recipies = repo.GetRecipies();

            List<BaseItem> meals = repo.GetMealType();

            meals.Insert(0, new BaseItem { Id = -1, Name = "-" });

            ViewBag.Meals = meals;

            return View("Index");
        }

        [HttpPost]
        public ActionResult doLogin(FormCollection collection)
        {
            RecipeRepository repo = new RecipeRepository(ConnectionString);

            User user = repo.Login(collection["mail"], collection["password"]);

            if (user != null)
            {
                Session["User"] = user;

                ViewBag.Recipies = repo.GetRecipies();

                List<BaseItem> meals = repo.GetMealType();

                meals.Insert(0, new BaseItem { Id = -1, Name = "-" });

                ViewBag.Meals = meals;

                if (collection["remember"] != null)
                {
                    HttpCookie cookie = new HttpCookie("User");
                    cookie["mail"] = user.Mail;
                    cookie.Expires = DateTime.Now.AddYears(10);
                    Response.Cookies.Add(cookie);
                }

                Response.Redirect("/Home/Index");
                
            }

            return View("Login");
        }

        [HttpPost]
        public ActionResult DoRegister(FormCollection collection)
        {
            string mail = collection["mail"];
            string pwd1 = collection["pwd1"];
            string pwd2 = collection["pwd2"];

            ViewBag.Mail = mail;

            if (pwd1.Length < 4)
            {
                ViewBag.Password = "Lösenordet måste vara minst 4 bokstäver";
                return View("Register");
            }

            if (pwd1 != pwd2)
            {
                ViewBag.Password = "Lösenorden måste vara samma";
                return View("Register");
            }

            User user = new User { Mail = mail, Password = pwd1, MemberSince = DateTime.Now.ToShortDateString() };

            RecipeRepository repo = new RecipeRepository(ConnectionString);

            repo.InsertUser(user);

           

            return View("Login");

        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Insert(string name, int type, string link, int origin, int meal)
        {
            if(Session["User"]  == null)
            {
                //TODO: return error that you must be login before adding recipies
                return null;
            }

            RecipeRepository repo = new RecipeRepository(ConnectionString);

            Recipe recipe = new Recipe { Name = name, Link = link };

            recipe.RecipeType.Id = type;
            recipe.Origin.Id = origin;
            recipe.Meal.Id = meal;
            User user = Session["User"] as User;
            recipe.User = user;

            repo.AddRecipe(recipe);

            ViewBag.Recipies = repo.GetRecipies();
            List<BaseItem> meals = repo.GetMealType();

            meals.Insert(0, new BaseItem { Id = -1, Name = "-" });

            ViewBag.Meals = meals;

            return View("Index");
        }

        [HttpGet]
        public ActionResult Search(string query)
        {
            RecipeRepository repo = new RecipeRepository(ConnectionString);

            List<Recipe> recipies = repo.Search(query);

            string json = JsonConvert.SerializeObject(recipies);

            return Content(json, "application/json");
        }

        [HttpGet]
        public ActionResult GetRecipiesWithMealId(int mealId)
        {
            RecipeRepository repo = new RecipeRepository(ConnectionString);

            List<Recipe> recipies = repo.GetRecipies(mealId);

            string json = JsonConvert.SerializeObject(recipies);

            return Content(json, "application/json");
        }
    }
}