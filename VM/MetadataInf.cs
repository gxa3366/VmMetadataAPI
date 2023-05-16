
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

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
            var client = new SecretClient(new Uri("https://myappkeyvault02.vault.azure.net"), new DefaultAzureCredential());
            var secret = client.GetSecret("secretName").Value;
            string tenantId = "c3595f37-8e9d-4988-b294-892bea17912b";
            string clientId = "d7dbb159-ee9a-4143-809f-6fd1e55d4fa7"; 
            string clientSecret = "lkI8Q~oJT140p4sROgUdbwbaxo0tL~7jDVdUKc~k";
            string subscriptionId = "efe469ef-6c58-45d6-a6bf-f517fbe5c50c";
            string resourceGroupName = "TechDemo";
            string vmName = "azv001";
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
