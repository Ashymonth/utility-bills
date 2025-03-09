using Ardalis.Specification;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate.Specifications;

public sealed class GetAllPlatformsWithCredentials : Specification<ReadingPlatform>
{
    public GetAllPlatformsWithCredentials()
    {
        Query.Include(platform => platform.Credentials);
    }
    
    public GetAllPlatformsWithCredentials(string userId)
    {
        Query.Include(platform => platform.Credentials.Where(credential => credential.UserId == userId));
    }
}