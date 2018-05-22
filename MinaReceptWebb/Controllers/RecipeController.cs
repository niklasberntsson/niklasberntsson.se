using MinaReceptLibrary.Database;
using MinaReceptLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MinaReceptWebb.Controllers
{
    public class RecipeController : BaseController
    {
        // GET: Recipe
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add(string name, string link)
        {
            RecipeDatabase db = new RecipeDatabase(ConnectionString);

            Recipe recipe = new Recipe { Name = name, Link = link };

            db.Insert(recipe);

            return null;
        }
    }
}