using System.Linq.Expressions;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using KvadoClient.Clients;
using KvadoClient.Extensions;
using Microsoft.EntityFrameworkCore;
using OrientClient.Extensions;
using UtilityBills.Abstractions.Services;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Application.Extensions;
using UtilityBills.Aspire.AppHost.ServiceDefaults;
using UtilityBills.Host;
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

builder.AddServiceDefaults();
builder.Services.AddLocalization();

builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UtilityBillsDbContext>();
    context.Database.Migrate();
    if (!await context.ReadingPlatforms.AnyAsync())
    {
        context.ReadingPlatforms.AddRange([
            ReadingPlatform.Create("Ориетн", ReadingPlatformType.Orient, "Ориент бридж"),
            ReadingPlatform.Create("Квадо", ReadingPlatformType.Kvado, "квадо"),
            ReadingPlatform.Create("Рус энерго", ReadingPlatformType.RusEnergy, "Рус энерго сбыт"),
        ]);
        context.SaveChanges();
    }

    var manager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    var job = scope.ServiceProvider.GetRequiredService<IDebtNotificationManager>();
    Expression<Func<Task>> jobFun = () => job.StartJob(default);
    manager.AddOrUpdate("Debt", jobFun, Cron.Daily(12));
}

app.MapDefaultEndpoints();

app.Services.UseTelegramBot();

app.UseRequestLocalization();

app.Run();