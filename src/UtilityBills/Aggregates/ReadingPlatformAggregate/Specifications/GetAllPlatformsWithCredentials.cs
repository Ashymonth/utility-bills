using Ardalis.Specification;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate.Specifications;

public sealed class GetAllPlatformsWithCredentials : Specification<ReadingPlatform>
{
    public GetAllPlatformsWithCredentials()
    {
        Query.Include(platform => platform.Credentials);
    }
}