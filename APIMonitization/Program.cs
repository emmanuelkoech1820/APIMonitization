using Microsoft.Azure.Management.ApiManagement;
using Microsoft.Azure.Management.ApiManagement.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Rest.Azure.OData;
using System;

namespace APIMonitization
{
    class Program
    {
        static void Main(string[] args)
        {
            string clientId = "23195229-34e6-446c-9bba-e5d77d15c1e8";
            string tenantId = "2d65fea0-8478-44f2-ac0f-1a58b1382aa2";
            string clientSecret = "N-T53b~OfmUgEBJjPhdjUG1RhoZQ_YdWdS";
            string resourceGroup = "RSG-DEV-OMNI-WE";
            string APIName = "api-omnichannel-dev";
            var credentials = SdkContext.AzureCredentialsFactory
            .FromServicePrincipal(clientId,
            clientSecret,
            tenantId,
            AzureEnvironment.AzureGlobalCloud);
            var apimClient = new ApiManagementClient(credentials);
            apimClient.SubscriptionId = "ce521e88-98bc-4f45-8409-0f1bde1e8256";
            var products = apimClient.Product.ListByService(resourceGroup, APIName);

            var odataQuery = new ODataQuery<ReportRecordContract>();
            odataQuery.Filter = "timestamp ge datetime'2021-06-01T00:00:00' and timestamp le datetime'2021-06-30T00:00:00'";
            var report = apimClient.Reports.ListByProduct(odataQuery, resourceGroup, APIName);

            var usageByUsers = apimClient.Reports.ListByUser(odataQuery, resourceGroup, APIName);
            foreach (var usageByUser in usageByUsers)
            {
                try
                {
                    //var user = apimClient.User.Get(resourceGroup, APIName, usageByUser.UserId);
                    decimal bill = usageByUser.CallCountTotal ?? (decimal)(usageByUser.CallCountTotal.Value * 0.001);
                    //Console.WriteLine($"{user.Name} {user.LastName} owns ${bill} for the usage in June");
                }
                catch (Exception ex)
                {
                    //special handling of a deleted user
                    //Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
