namespace BrewUpPubs.Modules.Pubs.Shared.Dtos;

public class BeerJson
{
    public string BeerId { get; set; } = string.Empty;
    public string BeerType { get; set; } = string.Empty;
    public double Quantity { get; set; } = 0;
}