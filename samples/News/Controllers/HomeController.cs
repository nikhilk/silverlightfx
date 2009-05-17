// HomeController.cs
//

using System;
using System.ComponentModel;
using System.ComponentModel.Navigation;

namespace NewsWidget.Controllers {

    public sealed class HomeController : Controller {

        public ActionResult About() {
            return View("About");
        }
    }
}
