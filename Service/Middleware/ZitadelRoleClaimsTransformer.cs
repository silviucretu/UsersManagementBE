using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

public class ZitadelRoleClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity;

        //extract the roles from JWT token based on pattern
        var roleClaim = principal.FindFirst("urn:zitadel:iam:org:project:roles");

        if (roleClaim != null)
        {
            var rolesJson = JsonDocument.Parse(roleClaim.Value);
            foreach (var role in rolesJson.RootElement.EnumerateObject())
            {
                identity.AddClaim(new Claim("role", role.Name));
            }
        }

        //extract the tenant from JWT token based on pattern
        var metadataClaim = principal.FindFirst("urn:zitadel:iam:user:metadata");
        if (metadataClaim != null)
        {
            var metadataJson = JsonDocument.Parse(metadataClaim.Value);

            if (metadataJson.RootElement.TryGetProperty("tenant", out var encodedTenantProp))
            {
                var encodedTenant = encodedTenantProp.GetString();
                if (!string.IsNullOrEmpty(encodedTenant))
                {
                    try
                    {
                        var decodedTenant = Encoding.UTF8.GetString(Convert.FromBase64String(encodedTenant + "=="));
                        identity.AddClaim(new Claim("tenant", decodedTenant));
                    }
                    catch
                    {
                    }
                }
            }
        }

        return Task.FromResult(principal);
    }
}
