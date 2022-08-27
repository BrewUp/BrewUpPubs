using BrewUpPubs.Modules.Pubs.Shared.Events;

namespace BrewUpPubs.Modules.Pubs.Tests.Unit;

public class PubsServiceTests
{
    [Fact]
    public void Can_Retrive_EventName()
    {
        var eventName = nameof(BeerProductionStarted).ToLower();

        Assert.Equal("beerproductionstarted", eventName);
    }
}