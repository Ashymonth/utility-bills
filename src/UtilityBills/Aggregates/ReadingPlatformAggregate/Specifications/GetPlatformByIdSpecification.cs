using Ardalis.Specification;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate.Specifications;

public sealed class GetPlatformByIdSpecification : Specification<ReadingPlatform> 
{
    public GetPlatformByIdSpecification(Guid platformId)
    {
        Query.Where(platform => platform.Id == platformId);
    }
}