using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace AdvertisingSystem.Infrastructure.Jobs;

public static class ConfigureServices
{
    public static void RunSyncReadDatabaseJob(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            var jobKey = JobKey.Create(nameof(SyncReadDatabaseJob));
            options.AddJob<SyncReadDatabaseJob>(jobKey)
                .AddTrigger(trigger =>
                {
                    trigger.ForJob(jobKey)
                        .WithSimpleSchedule(schedule =>
                        {
                            schedule.WithIntervalInMinutes(20)
                                .RepeatForever();
                        });
                });
        });

        services.AddQuartzHostedService();
    }
}