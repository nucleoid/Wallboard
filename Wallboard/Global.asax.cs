using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac.Integration.Mvc;
using LoggingServer.Common;
using NLog;
using RestSharp;
using Wallboard.Autofac;
using Wallboard.Automapper;

namespace Wallboard
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Feeds", // Route name
                "{controller}/{action}/{key}", // URL with parameters
                new { controller = "Home", action = "Index" } // Parameter defaults
            );
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var environment = ConfigurationManager.AppSettings["environment"];
            var loggingServerEndPoint = ConfigurationManager.AppSettings["loggingServerEndPoint"];
            LogManager.Configuration = NLogConfiguration.ConfigureServerLogger(null, environment, loggingServerEndPoint,
                Assembly.GetExecutingAssembly(), LogLevel.Debug);
            BootStrap();
        }

        private void BootStrap()
        {
            AutomapperConfig.Setup();
            DependencyContainer.Register(new TaskModule(), new MvcModule(Assembly.GetExecutingAssembly()));
            DependencyContainer.BuildContainer();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(DependencyContainer.Container));
        }
    }
}