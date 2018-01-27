using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

//add these using statements
using Microsoft.Azure.KeyVault;
using System.Web.Configuration;

namespace FourthCoffeeAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        async protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(Util.GetToken));            
            var sec = await kv.GetSecretAsync(WebConfigurationManager.AppSettings["SecretUri"]);
            Util.EncryptSecret = sec.Value;
        }
    }
}
