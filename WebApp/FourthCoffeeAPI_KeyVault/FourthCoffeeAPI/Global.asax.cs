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
using System.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SqlServer.Management.AlwaysEncrypted.AzureKeyVaultProvider;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FourthCoffeeAPI
{    
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static ClientCredential _clientCredential;

        async protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            //Adds the Azure Key Vault Provider to ensure the column is unencrypted when queryed upon
            //Reference: https://blogs.msdn.microsoft.com/sqlsecurity/2015/11/10/using-the-azure-key-vault-key-store-provider-for-always-encrypted/
            SqlColumnEncryptionAzureKeyVaultProvider azureKeyVaultProvider = new SqlColumnEncryptionAzureKeyVaultProvider(Util.GetToken);
            Dictionary<string, SqlColumnEncryptionKeyStoreProvider> providers = new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>();
            providers.Add(SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, azureKeyVaultProvider);
            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(providers);            
            
            //Get an access token for the Key Vault to get the secret out...
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(Util.GetToken));            
            var sec = await kv.GetSecretAsync(WebConfigurationManager.AppSettings["SecretUri"]);

            //ensure that the connect string has the "Column Encryption Setting=Enabled" in it!
            Util.EncryptSecret = sec.Value;
        }
    }
}
