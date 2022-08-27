using Muflone.Core;

namespace BrewUpPubs.Modules.Pubs.Shared.CustomTypes;

public sealed class BatchId : DomainId
{
    public BatchId(Guid value) : base(value)
    {
    }
}