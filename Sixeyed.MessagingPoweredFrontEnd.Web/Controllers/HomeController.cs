using Sixeyed.MessagingPoweredFrontEnd.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sixeyed.MessagingPoweredFrontEnd.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string user)
        {
            var model = new MailModel
            {
                Sender = user
            };

            return View(model);
        }
    }
}