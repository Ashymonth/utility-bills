using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Services;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.AddCredentialWorkflowFeature.Steps;

public class AddSelectedPlatformCredential : IStepBody
{
    private readonly IUtilityPaymentPlatformService _platformService;

    public AddSelectedPlatformCredential(IUtilityPaymentPlatformService platformService)
    {
        _platformService = platformService;
    }

    public Guid PlatformId { get; set; }

    public Email Email { get; set; } = null!;

    public Password Password { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public Result<UtilityPaymentPlatformCredential> CreatedCredentials { get; set; } = null!;
    
    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        ArgumentNullException.ThrowIfNull(Email);
        ArgumentNullException.ThrowIfNull(Password);
        ArgumentException.ThrowIfNullOrWhiteSpace(UserId);

        CreatedCredentials = await _platformService.AddCredentialToPlatformAsync(PlatformId, Email, Password, UserId);
        
        return ExecutionResult.Next();
    }
}