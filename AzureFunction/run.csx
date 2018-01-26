#r "Newtonsoft.Json"

using System.Net;
using System.Configuration;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.Text;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    string secPwInKeyVault = GetSecret();
    log.Info("My Secret – " + secPwInKeyVault);    

    return new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent(JsonConvert.SerializeObject(secPwInKeyVault), Encoding.UTF8, "application/json")};    
}

public static string GetSecret()
{
    string secretUri = ConfigurationManager.AppSettings["KeyVaultUri"];    
    var serviceTokenProvider = new AzureServiceTokenProvider();    
    var kvToken = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(serviceTokenProvider.KeyVaultTokenCallback)); 
    var kvSecret = kvToken.GetSecretAsync(secretUri).Result;
    return kvSecret.Value;
}