using Microsoft.Extensions.Localization;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;
using UtilityBills.Telegram.Extensions.WorkflowExtensions;
using UtilityBills.Telegram.Workflows.AddCredentialWorkflowFeature.Steps;
using UtilityBills.Telegram.Workflows.Core.Models;
using WorkflowCore.Interface;

namespace UtilityBills.Telegram.Workflows.AddCredentialWorkflowFeature;

public class AddCredentialWorkflow : IWorkflow<AddCredentialWorkflowData>
{
    private readonly IPasswordProtector _passwordProtector;
    private readonly IStringLocalizer<AddCredentialWorkflow> _localizer;

    public AddCredentialWorkflow(IPasswordProtector passwordProtector,
        IStringLocalizer<AddCredentialWorkflow> localizer)
    {
        _passwordProtector = passwordProtector;
        _localizer = localizer;
    }

    public void Build(IWorkflowBuilder<AddCredentialWorkflowData> workflowBuilder)
    {
        workflowBuilder
            .StartWith<LoadUtilityPlatformList>()
            .Input(list => list.UserId, data => data.UserId)
            .Input(list => list.SentMessageIds, data => data.SentMessageIds)
            .Output(data => data.UserId, list => list.UserId)
            .Output(data => data.SentMessageIds, list => list.SentMessageIds)
            .Output(data => data.UtilityPaymentPlatforms, list => list.UtilityPaymentPlatforms)
            .SendInlineData(_localizer.GetString("Select platform for which you want to add credentials"),
                data => data.GetButtonsToSelectPlatform().ToArray())
            .WaitForUserInlineData(data => data.PlatformId, o => Guid.Parse((o as UserMessage)!.Message))
            .SendMessageToUser(_localizer.GetString("Input email"))
            .WaitForUserMessage(data => data.Email, message => Email.Create(message.Message))
            .If(data => data.Email.IsFailed)
            .Do(builder => builder
                .SendMessageToUser(data => data.Email.Errors.First().Message)
                .EndWorkflow())
            .SendMessageToUser(_localizer.GetString("Input password"))
            .WaitForUserMessage(data => data.Password, message => Password.Create(message.Message, _passwordProtector))
            .If(data => data.Password.IsFailed)
            .Do(builder => builder.SendMessageToUser(data => data.Email.Errors
                .First().Message).EndWorkflow())
            .Then<AddSelectedPlatformCredential>()
            .Input(credential => credential.UserId, data => data.UserId)
            .Input(credential => credential.PlatformId, data => data.PlatformId)
            .Input(credential => credential.Email, data => data.Email.Value)
            .Input(credential => credential.Password, data => data.Password.Value)
            .Output(data => data.CreatedCredential, credential => credential.CreatedCredentials)
            .If(data => data.CreatedCredential.IsFailed).Do(builder =>
                builder.SendMessageToUser(_localizer.GetString(
                        "Unable to get authorize on platform. Check your email or password and try again"))
                    .EndWorkflow())
            .If(data => !data.CreatedCredential.IsFailed).Do(builder =>
                builder.SendMessageToUser(
                    _localizer.GetString("You're successfully added credentials to the platform")))
            .EndWorkflow();
    }

    public string Id => nameof(AddCredentialWorkflow);
    public int Version => 1;
}