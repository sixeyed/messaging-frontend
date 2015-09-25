using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Sixeyed.MessagingPoweredFrontEnd.Web.Startup))]

namespace Sixeyed.MessagingPoweredFrontEnd.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
