using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace InTune
{
    public static class WebApiConfig
    {
        public const string DefaultRouteName = "DefaultApi";
        public const string ContactActionRouteName = "Action";

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: DefaultRouteName,
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.Routes.MapHttpRoute(
            //    name: ContactActionRouteName,
            //    routeTemplate: "api/{controller}/{action}/{register}"
            //);
        }
    }
}
