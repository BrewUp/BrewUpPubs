using BrewUpPubs.Modules.Pubs.Shared.CustomTypes;
using BrewUpPubs.Modules.Pubs.Shared.Dtos;
using BrewUpPubs.ReadModel.Abstracts;

namespace BrewUpPubs.ReadModel.Models;

public class Beer : ModelBase
{
    public string BeerType { get; private set; } = string.Empty;
    public double Quantity { get; private set; } = 0;

    protected Beer()
    {}

    public static Beer CreateBeer(BeerId beerId, BeerType beerType) =>
        new(beerId.Value, beerType.Value);

    public void UpdateQuantity(Quantity quantity) => Quantity = quantity.Value;

    private Beer(Guid beerId, string beerType)
    {
        Id = beerId.ToString();
        BeerType = beerType;
    }

    public BeerJson ToJson() => new()
    {
        BeerId = Id,
        BeerType = BeerType,
        Quantity = Quantity
    };
}