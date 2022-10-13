namespace BrewUpPubs.Shared.Configuration;

public class AppSettings
{
    public EventStoreSettings EventStoreSettings { get; set; } = new();
    public TokenParameters TokenParameters { get; set; } = new();
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