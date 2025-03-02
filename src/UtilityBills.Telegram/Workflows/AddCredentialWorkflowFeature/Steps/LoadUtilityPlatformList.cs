using UtilityBills.Abstractions;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Specifications;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.AddCredentialWorkflowFeature.Steps;

public class LoadUtilityPlatformList(IRepository<ReadingPlatform> repository) : IStepBody
{
    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = [];

    public List<ReadingPlatform> ReadingPlatforms { get; set; } = null!;

    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var platforms = await repository.ListAsync(new GetAllPlatformsWithCredentials(), context.CancellationToken);

        if (platforms.Count == 0)
        {
            throw new InvalidOperationException("Unable to find any utility platforms");
        }

        ReadingPlatforms = platforms;

        return ExecutionResult.Next();
    }
}