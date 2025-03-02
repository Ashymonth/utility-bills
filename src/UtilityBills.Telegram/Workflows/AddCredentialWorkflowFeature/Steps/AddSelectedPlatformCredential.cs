using FluentResults;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Services;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.AddCredentialWorkflowFeature.Steps;

public class AddSelectedPlatformCredential : IStepBody
{
    private readonly IReadingPlatformService _platformService;

    public AddSelectedPlatformCredential(IReadingPlatformService platformService)
    {
        _platformService = platformService;
    }

    public Guid PlatformId { get; set; }

    public Email Email { get; set; } = null!;

    public Password Password { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public Result<ReadingPlatformCredential> CreatedCredentials { get; set; } = null!;
    
    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        ArgumentNullException.ThrowIfNull(Email);
        ArgumentNullException.ThrowIfNull(Password);
        ArgumentException.ThrowIfNullOrWhiteSpace(UserId);

        CreatedCredentials = await _platformService.AddCredentialToPlatformAsync(PlatformId, Email, Password, UserId);
        
        return ExecutionResult.Next();
    }
}