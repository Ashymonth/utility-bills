using Ardalis.Specification;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Specifications;

public sealed class GetPlatformByIdSpecification : Specification<UtilityPaymentPlatform> 
{
    public GetPlatformByIdSpecification(Guid platformId)
    {
        Query.Where(platform => platform.Id == platformId);
    }
}