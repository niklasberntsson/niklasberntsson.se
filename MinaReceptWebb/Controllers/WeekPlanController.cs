using MinaReceptLibrary;
using MinaReceptLibrary.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MinaReceptWebb.Controllers
{
    public class WeekPlanController : BaseController
    {
        // GET: WeekPlan
        public ActionResult Index()
        {
            if(User == null){
                return null;
            }

            SetIndexViewData();
           

            return View();
        }

        private void SetIndexViewData()
        {
            RecipeRepository repo = new RecipeRepository(ConnectionString);

            WeekPlan activeWeekPlan = new WeekPlan();

            List<WeekPlan> weekPlans = repo.GetWeekPlans(User.Id);

            int weekPlanId = 0;
            foreach (WeekPlan weekPlan in weekPlans)
            {
                if (weekPlan.Active)
                {
                    foreach (WeekItem item in weekPlan.WeekItems)
                    {
                        Recipe recipe = repo.GetRecipe(item.RecipeId);

                        item.Recipe = recipe;

                        weekPlanId = weekPlan.Id;

                    }
                    activeWeekPlan = weekPlan;
                }
            }

            List<MySettings> settings = GetMySettings();

            //antar dagar i veckoplanen
            MySettings firstDay = settings.First(x => x.Key == "FirstDay");
            MySettings days = settings.First(x => x.Key == "DayCount");

            string[] t = new string[] { "Måndag", "Tisdag", "Onsdag", "Torsdag", "Fredag", "Lördag", "Söndag" };
            string[] m = new string[Int32.Parse(days.Value)];

            int counter = GetDayInt(firstDay.Value);
            for (int i = 1; i <= Int32.Parse(days.Value); i++)
            {
                if (counter == 8)
                {
                    counter = 1;
                }
                m[(i-1)] = t[counter-1];

                counter++;
            }
            ViewBag.WeekDays = m;
            //ViewBag.WeekDays = new string[] { "Måndag", "Tisdag", "Onsdag", "Torsdag", "Fredag", "Lördag", "Söndag" };

            //Add empty 
            activeWeekPlan.WeekItems.Insert(0, new WeekItem { Id = 0, Recipe = new Recipe { Name = "-", Id = 0 }, RecipeId = 0 });

            ViewBag.WeekPlan = activeWeekPlan;
            
        }

        [HttpGet]
        public ActionResult SaveItem(string weekPlanId, string recipeId, string day)
        {
            return null;
        }

        [HttpGet]
        public ActionResult AddWeekPlan(string name, int active)
        {
            RecipeRepository repo = new RecipeRepository(ConnectionString);
            bool isActive = false;
            //set all other weekplans Active = false;
            if (active == 1)
            {
                isActive = true;
                repo.SetWeekPlanToInactive(User.Id);
            }

            int week = WeekOfYearISO8601(DateTime.Now);

            WeekPlan weekPlan = new WeekPlan
            {
                Name = name,
                UserId = User.Id,
                Week = week,
                CreatedAt = DateTime.Now.ToShortDateString(),
                Active = isActive
            };
            repo.InsertWeekPlan(weekPlan);

            SetIndexViewData();

            return Content("ok");
        }

        [HttpGet]
        public ActionResult ActiveWeekPlanExists()
        {
            if (User == null)
            {
                return Content("0");
            }

            int id = 0;

            //Get week 
            int week = WeekOfYearISO8601(DateTime.Now);

            RecipeRepository repo = new RecipeRepository(ConnectionString);

            WeekPlan weekPlan = repo.GetActiveWeekPlan(User.Id);

            if (weekPlan == null)
            {
                //create a new one

                weekPlan = new WeekPlan
                {
                    Name = week + "",
                    UserId = User.Id,
                    Week = week,
                    CreatedAt = DateTime.Now.ToShortDateString(),
                    Active = true
                };
                repo.InsertWeekPlan(weekPlan);

                weekPlan = repo.GetActiveWeekPlan(User.Id);
            }

            

            return Content(weekPlan.Id + "");
        }

        [HttpGet]
        public ActionResult AddRecipe(int weekPlanId, int recipeId)
        {
            WeekItem item = new WeekItem
            {
                RecipeId = recipeId,
                WeekId = weekPlanId,
                WeekDay = String.Empty
            };

            RecipeRepository repo = new RecipeRepository(ConnectionString);

            repo.InsertWeekItem(item);
            return Content("200");
        }

        [HttpGet]
        public ActionResult AddItemToWeekPlan(string weekPlanId, string weekItemId, string day)
        {
            int wpId = Int32.Parse(weekPlanId);
            int itemId = Int32.Parse(weekItemId.Split('_')[1]);

            RecipeRepository repo = new RecipeRepository(ConnectionString);

            repo.UpdateWeekItemWithDay(itemId, day);

            return null;
        }

        private int WeekOfYearISO8601(DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date.AddDays(4 - (day == 0 ? 7 : day)), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public int GetDayInt(string day)
        {
            switch (day)
            {
                case "Måndag":
                    return 1;
                case "Tisdag":
                    return 2;
                case "Onsdag":
                    return 3;
                case "Torsdag":
                    return 4;
                case "Fredag":
                    return 5;
                case "Lördag":
                    return 6;
                case "Söndag":
                    return 7;
                default:
                    return 0;
                    
            }
        }
    }
}