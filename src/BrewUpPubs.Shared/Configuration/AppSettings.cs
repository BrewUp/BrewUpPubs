namespace BrewUpPubs.Shared.Configuration;

public class AppSettings
{
    public EventStoreSettings EventStoreSettings { get; set; } = new();
    public TokenParameters TokenParameters { get; set; } = new();
    public PrincipalSettings PrincipalSettings { get; set; } = new();
}

public class EventStoreSettings
{
    public string ConnectionString { get; set; } = string.Empty;
}

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}

public class TokenParameters
{
    public string ServerRealm { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Metadata { get; set; } = string.Empty;
}

public class PrincipalSettings
{
    public string IdClaimType { get; set; } =
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    public string ResourceAccessClaimType { get; set; } = "resource_access";
    public string ResourceAccessI3PortalBackendContext { get; set; } = "i3portal-backend";
    public string ResourceAccessContextRoles { get; set; } = "roles";
}