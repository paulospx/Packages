using Microsoft.Graph;

namespace ActivateRoles
{
    internal static class AzureManager
    {
        /// <summary>
        /// Lists the service principal IDs in the tenant that match the given display name.
        /// </summary>
        /// <param name="graphClient">An authenticated GraphServiceClient instance.</param>
        /// <param name="servicePrincipalName">The display name of the service principal to search for.</param>
        /// <returns>A list of matching service principal IDs.</returns>
        public static async Task<List<string>> ListServicePrincipalIdsByNameAsync(GraphServiceClient graphClient, string servicePrincipalName)
        {
            var ids = new List<string>();
            try
            {
                // Filter service principals by displayName
                var result = await graphClient.ServicePrincipals
                    .GetAsync(requestConfig =>
                    {
                        requestConfig.QueryParameters.Filter = $"displayName eq '{servicePrincipalName}'";
                        requestConfig.QueryParameters.Select = new[] { "id", "displayName" };
                    });

                if (result?.Value != null)
                {
                    ids.AddRange(result.Value.Select(sp => sp.Id));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving service principals: {ex.Message}");
            }
            return ids;
        }

    }
}
