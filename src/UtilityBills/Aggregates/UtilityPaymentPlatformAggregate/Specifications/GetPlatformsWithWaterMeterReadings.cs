using Ardalis.Specification;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Specifications;

public sealed class GetPlatformsWithMeterReadings : Specification<UtilityPaymentPlatform>
{
    public GetPlatformsWithMeterReadings(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        Query.Where(platform => platform.PlatformType == UtilityPaymentPlatformType.Kvado ||
                                platform.PlatformType == UtilityPaymentPlatformType.Orient)
            .Include(platform => platform.Credentials.Where(credential => credential.UserId == userId));

    }
}