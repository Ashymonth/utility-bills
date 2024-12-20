using Ardalis.Specification;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Specifications;

public sealed class GetPlatformWithUserCredentialsSpecification : Specification<UtilityPaymentPlatform>, 
    ISingleResultSpecification<UtilityPaymentPlatform>
{
    public GetPlatformWithUserCredentialsSpecification(Guid platformId, string userId)
    {
        Query.Where(platform => platform.Id == platformId)
            .Include(platform => platform.Credentials.Where(credential => credential.UserId == userId));
    }
}