using Ardalis.Specification;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate.Specifications;

public sealed class GetPlatformWithUserCredentialsSpecification : Specification<ReadingPlatform>, 
    ISingleResultSpecification<ReadingPlatform>
{
    public GetPlatformWithUserCredentialsSpecification(Guid platformId, string userId)
    {
        Query.Where(platform => platform.Id == platformId)
            .Include(platform => platform.Credentials.Where(credential => credential.UserId == userId));
    }
}