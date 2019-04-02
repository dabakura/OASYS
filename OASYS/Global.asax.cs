using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OASYS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public static int ID_USER
        {
            get { return int.Parse(HttpContext.Current.Session["ID_USER"].ToString()); }
            set { HttpContext.Current.Session["ID_USER"] = value; }
        }

        public static int Rol
        {
            get { return int.Parse(HttpContext.Current.Session["rol"].ToString()); }
            set { HttpContext.Current.Session["rol"] = value; }
        }

        public static int ID_USUARIOCREA
        {
            get { return int.Parse(HttpContext.Current.Application["ID_USUARIOCREA"].ToString()); }
            set { HttpContext.Current.Application["ID_USUARIOCREA"] = value; }
        }

        public static string Bar
        {
            get
            {
                return HttpContext.Current.Application["Bar"] as string;
            }
            set
            {
                HttpContext.Current.Application["Bar"] = value;
            }
        }
    }
}
