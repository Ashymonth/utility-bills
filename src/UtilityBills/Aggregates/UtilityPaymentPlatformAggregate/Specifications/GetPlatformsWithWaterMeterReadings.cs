using Ardalis.Specification;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Specifications;

public sealed class GetPlatformsWithWaterMeterReadings : Specification<UtilityPaymentPlatform>
{
    public GetPlatformsWithWaterMeterReadings(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        Query.Where(platform => platform.PlatformType == UtilityPaymentPlatformType.Kvado ||
                                platform.PlatformType == UtilityPaymentPlatformType.Orient)
            .Include(platform => platform.Credentials.Where(credential => credential.UserId == userId));

    }
}