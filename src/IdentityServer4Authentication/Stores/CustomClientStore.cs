using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4Authentication.Stores
{
    public class CustomClientStore : IClientStore
    {
        public static IEnumerable<Client> AllClients { get; } = new[]
        {
            new Client
            {
                ClientId = "myClient",
                ClientName = "My Custom Client",
                AccessTokenLifetime = 60 * 60 * 24,
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                RequireClientSecret = false,
                AllowedScopes = 
                {
                    "myAPIs"
                }
            },
            // resource owner password grant client
            new Client
            {
                ClientId = "ro.client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AccessTokenLifetime = 60 * 60,
                RequireClientSecret = true,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = { "api1" }
            }
        };

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return Task.FromResult(AllClients.FirstOrDefault(c => c.ClientId == clientId));
        }
    }
}
