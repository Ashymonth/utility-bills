using Ardalis.Specification;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Specifications;

public sealed class GetAllPlatformsWithCredentials : Specification<UtilityPaymentPlatform>
{
    public GetAllPlatformsWithCredentials()
    {
        Query.Include(platform => platform.Credentials);
    }
}