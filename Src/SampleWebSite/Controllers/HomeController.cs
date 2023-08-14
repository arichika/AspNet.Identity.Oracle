using System.Web.Mvc;

namespace SampleWebSite.Controllers {
    public class HomeController : Controller {
        public ActionResult Index()//This is constructor
        {
            return View();
        }

        [Authorize]
        public ActionResult About() {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
