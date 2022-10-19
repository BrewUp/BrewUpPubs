using System.Security.Claims;
using BrewUpPubs.Shared.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace BrewUpPubs.Transformers;

public class ClaimsTransformers : IClaimsTransformation
{
    private readonly AppSettings _settings;
    private readonly Dictionary<string, string> _rolesDictionary = new();

    public ClaimsTransformers(IOptions<AppSettings> options)
    {
        _settings = options.Value;
    }
    
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var authorizations = await UserAuthorizations(principal);
        authorizations = authorizations.Concat(ResourceAccessAuthorizations(principal)).ToList();
        authorizations = authorizations.Distinct().ToList();

        var claimsIdentity = new ClaimsIdentity();
        foreach (var authorization in authorizations)
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, authorization));

        principal.AddIdentity(claimsIdentity);

        return await Task.FromResult(principal);throw new NotImplementedException();
    }
    
    private async Task<IEnumerable<string>> UserAuthorizations(ClaimsPrincipal principal)
    {
        var sub = principal.Claims.FirstOrDefault(c => c.Type == _settings.PrincipalSettings.IdClaimType)?.Value;
        if (sub is null)
            return new List<string>();

        // var resultUser = await _usersService.GetUserAsync(sub);
        // if (resultUser.IsLeft)
        //     return new List<string>();
        //
        // var user = resultUser.RightToArray().First();
        // return user.Authorizations.ToList();

        return Enumerable.Empty<string>();
    }
    
    private IEnumerable<string> ResourceAccessAuthorizations(ClaimsPrincipal principal)
    {
        var emptyAuthorizations = new List<string>();

        try
        {
            var jsonString = principal.Claims.FirstOrDefault(c => c.Type == _settings.PrincipalSettings.ResourceAccessClaimType)
                ?.Value;
            if (string.IsNullOrEmpty(jsonString)) return emptyAuthorizations;

            var resources = JObject.Parse(jsonString);
            var i3portalBackendResources = resources[_settings.PrincipalSettings.ResourceAccessI3PortalBackendContext];
            if (i3portalBackendResources is null) return emptyAuthorizations;

            var roles = i3portalBackendResources[_settings.PrincipalSettings.ResourceAccessContextRoles]
                .ToObject<string[]>();

            return roles is null
                ? emptyAuthorizations
                : roles;
        }
        catch
        {
            return emptyAuthorizations;
        }
    }
}