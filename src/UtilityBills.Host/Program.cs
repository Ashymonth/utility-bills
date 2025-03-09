using System.Linq.Expressions;
using System.Reflection;
using Hangfire;
using Hangfire.PostgreSql;
using KvadoClient.Extensions;
using Microsoft.EntityFrameworkCore;
using OrientClient.Extensions;
using TelegramBotCommandFramework.Services;
using UtilityBills.Abstractions.Services;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Application.Extensions;
using UtilityBills.Aspire.AppHost.ServiceDefaults;
using UtilityBills.Host;
using UtilityBills.Host.BackgroundServices;
using UtilityBills.Host.Commands;
using UtilityBills.Host.Extensions;
using UtilityBills.Host.Integrations;
using UtilityBills.Host.Security;
using UtilityBills.Infrastructure;
using UtilityBills.Infrastructure.Extensions;
using UtilityBills.Services;
using UtilityBills.Telegram.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPasswordProtector, PasswordProtector>();
builder.Services.AddSingleton<ISecurityKeyProvider>(_ =>
    new SecurityKeyProvider(builder.Configuration.GetValue<string>("SecurityConfig:SecretKey")!,
        builder.Configuration.GetValue<string>("SecurityConfig:InitVector")!));

builder.Services.AddScoped<IOrientProvider, OrientProvider>();
builder.Services.AddScoped<IKvadoProvider, KvadoProvider>();
builder.Services.AddOrientClient().AddKvadoClient();

builder.Services.AddSingleton<IDebtNotificationProvider, TelegramDebtNotificationProvider>();

builder.Services
    .AddConfiguredDatabase(builder.Configuration)
    .AddApplicationLayer()
    .AddInfrastructureLayer()
    .AddTelegramBot(builder.Configuration.GetValue<string>("BotToken")!);

builder.Services.AddHangfireServer();
builder.Services.AddHangfire(configuration =>
{
    configuration.UseRecommendedSerializerSettings();
    configuration.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString(nameof(UtilityBillsDbContext))));
});

builder.Services.AddTelegramCommands(Assembly.GetExecutingAssembly());
builder.Services.AddInterceptor<ExitTelegramCommandInterceptor>();
builder.Services.AddHostedService<TelegramHostedService>();

builder.AddServiceDefaults();
builder.Services.AddLocalization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UtilityBillsDbContext>();
    await context.Database.MigrateAsync();

    var manager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    var job = scope.ServiceProvider.GetRequiredService<IDebtNotificationManager>();
    Expression<Func<Task>> jobFun = () => job.StartJob(CancellationToken.None);
    manager.AddOrUpdate("Debt", jobFun, Cron.Daily(12));
}

app.MapDefaultEndpoints();

app.Services.UseTelegramBot();

app.UseRequestLocalization();

app.Run();