using Sixeyed.MessagingPoweredFrontEnd.Core.Messaging;
using Sixeyed.MessagingPoweredFrontEnd.Web.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sixeyed.MessagingPoweredFrontEnd.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            //start the messsage handlers:
            NewMailHandler.Init();
            ReplyHandler.Init();
        }
    }
}
