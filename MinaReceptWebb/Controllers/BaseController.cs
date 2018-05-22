using MinaReceptLibrary;
using MinaReceptLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MinaReceptWebb.Controllers
{
    public class BaseController : Controller
    {
        public string ConnectionString
        {
            get { return WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; }
        }

        public User User
        {
            get
            {
                User user = (User) Session["User"];

                return user;
            }
        }

        public RecipeRepository Repository
        {
            get
            {
                return new RecipeRepository(ConnectionString);
            }
        }

        public List<MySettings> GetMySettings()
        {
            return Repository.GetSettings(User.Id);
        }
    }
}