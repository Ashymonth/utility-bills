using Ardalis.Specification;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate.Specifications;

public sealed class GetPlatformsWithMeterReadings : Specification<ReadingPlatform>
{
    public GetPlatformsWithMeterReadings(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        Query.Where(platform => platform.PlatformType == ReadingPlatformType.Kvado ||
                                platform.PlatformType == ReadingPlatformType.Orient)
            .Include(platform => platform.Credentials.Where(credential => credential.UserId == userId));

    }
}