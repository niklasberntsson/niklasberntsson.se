using MinaReceptLibrary;
using MinaReceptLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MinaReceptWebb.Controllers
{
    public class SettingsController : BaseController
    {
        public const string FIRST_DAY = "FirstDay";
        public const string DAYS_COUNT = "DayCount";


        // GET: Settings
        public ActionResult Index()
        {
            User user = (User) Session["User"];

            if (user == null)
            {
                return View("Error");
            }

            CheckSettingKeys(user.Id);

            ViewBag.Days = GetDays();
            List<MySettings> settings = Repository.GetSettings(user.Id);

            foreach (MySettings setting in settings)
            {
                if (setting.Key == FIRST_DAY)
                {
                    ViewBag.FirstDay = setting.Value;
                }
                if (setting.Key == DAYS_COUNT)
                {
                    ViewBag.DaysCount = Int32.Parse(setting.Value);
                }
            }
            

            return View();
        }

        [HttpPost]
        public void Change(FormCollection collection)
        {
            int weekCount = Int32.Parse(collection["weekCount"]);
            string day = collection["days"];

            Repository.UpdateSettings(FIRST_DAY, day, User.Id);
            Repository.UpdateSettings(DAYS_COUNT, weekCount + "", User.Id);

            Response.Redirect("/WeekPlan/Index");
        }

        private string[] GetDays()
        {
            return new string[] { "Måndag", "Tisdag", "Onsdag", "Torsdag", "Fredag", "Lördag", "Söndag" };
        }

        private void CheckSettingKeys(int userId)
        {
            

            if (!Repository.SettingsExists(FIRST_DAY, userId))
            {
                MySettings setting = new MySettings
                {
                    Key = FIRST_DAY,
                    Value = "Måndag",
                    UserId = userId
                };

                Repository.InsertSettings(setting);
            }

            if (!Repository.SettingsExists(DAYS_COUNT, userId))
            {
                MySettings setting = new MySettings
                {
                    Key = DAYS_COUNT,
                    Value = "7",
                    UserId = userId
                };

                Repository.InsertSettings(setting);
            }
        }
    }
}