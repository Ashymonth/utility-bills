using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using UtilityBills.Telegram.Workflows.AddCredentialWorkflowFeature;
using UtilityBills.Telegram.Workflows.AddCredentialWorkflowFeature.Steps;
using UtilityBills.Telegram.Workflows.Core;
using UtilityBills.Telegram.Workflows.Core.Steps;
using UtilityBills.Telegram.Workflows.SendMeterReadingsWorkflowFeature;
using UtilityBills.Telegram.Workflows.SendMeterReadingsWorkflowFeature.Steps;
using WorkflowCore.Interface;

namespace UtilityBills.Telegram.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegramBot(this IServiceCollection services, string botToken)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(botToken);
        
        services.AddWorkflow(options =>
        {
            options.UseIdleTime(TimeSpan.FromHours(1));
        });
        
        services.AddTransient<LoadUtilityPlatformList>();
        services.AddTransient<AddSelectedPlatformCredential>();
        services.AddTransient<RequestMeterReadings>();
        services.AddTransient<SendMeterReadingsStep>();
        services.AddTransient<EnsureMeterReadingsWereSentStep>();
        services.AddTransient<GetPreviousMeterReadingsStep>();
        
        services.AddTransient<SendMessageToUser>();
        services.AddTransient<SendMessageWithOptionsToUser>();
        services.AddTransient<SendInlineDataMessageToUser>();

        services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(botToken));
        services.AddHostedService<TelegramHostService>();

        return services;
    }
    
    public static void UseTelegramBot(this IServiceProvider app)
    {
        var workflowHost = app.GetRequiredService<IWorkflowHost>();

        workflowHost.RegisterWorkflow<AddCredentialWorkflow, AddCredentialWorkflowData>();
        workflowHost.RegisterWorkflow<SendMeterReadingsWorkflow, SendMeterReadingsWorkflowData>();
        workflowHost.Start();
    }
}