
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json;

namespace VM
{
    public class MetadataInf
    {
        static async Task Main(string[] args)
        {
            await GetMetadata();
        }

        public static async Task  GetMetadata()
        {
            KeyVaultSecret secret;
            var client = new SecretClient(new Uri("https://myappkeyvault02.vault.azure.net"), new DefaultAzureCredential());

            secret= client.GetSecret("secretName");
            string secrets = secret.Value;
            secret = client.GetSecret("tenantId");
            string tenantId = secret.Value;
            secret = client.GetSecret("clientId");
            string clientId = secret.Value;
            secret = client.GetSecret("clientSecret");
            string clientSecret = secret.Value;
            secret = client.GetSecret("subscriptionId");
            string subscriptionId = secret.Value;
            secret = client.GetSecret("resourceGroupName");
            string resourceGroupName = secret.Value;
            secret = client.GetSecret("vmName");
            string vmName = secret.Value;
            
            var app = ConfidentialClientApplicationBuilder.Create(clientId).WithClientSecret(clientSecret).WithTenantId(tenantId).Build();
            var authResult = await app.AcquireTokenForClient(new string[] {
    "https://management.azure.com/.default"
}).ExecuteAsync();
            // Use the access token to authenticate the API call
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            // API Call to Get VM Details
            // https://learn.microsoft.com/en-us/rest/api/compute/virtual-machines/get?tabs=HTTP
            var response = await httpClient.GetAsync($"https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Compute/virtualMachines/{vmName}?api-version=2022-08-01");
            if (response.IsSuccessStatusCode)
            {
                // Retrieve the response content as a JSON string
                var jsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("-------------------Get VM Details--------------------------");
                Console.WriteLine(JObject.Parse(jsonString));
            }
            else
            {
                throw new Exception($"Failed to retrieve VM metadata: {response.StatusCode} - {response.ReasonPhrase}");
            }

        }

       
    }
}
