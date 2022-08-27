using Muflone.Core;

namespace BrewUpPubs.Modules.Pubs.Shared.CustomTypes;

public class BeerId : DomainId
{
    public BeerId(Guid value) : base(value)
    {
    }
}