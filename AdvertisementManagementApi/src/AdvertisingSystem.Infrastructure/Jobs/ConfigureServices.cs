using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace AdvertisingSystem.Infrastructure.Jobs;

public static class ConfigureServices
{
    public static void RunSyncReadDatabaseJob(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            var syncJobKey = JobKey.Create(nameof(SyncReadDatabaseJob));
            options.AddJob<SyncReadDatabaseJob>(syncJobKey)
                .AddTrigger(trigger =>
                {
                    trigger.ForJob(syncJobKey)
                        .WithSimpleSchedule(schedule =>
                        {
                            schedule.WithIntervalInMinutes(20)
                                .RepeatForever();
                        });
                });

            //=====================================================================
            var outBoxJobKey = new JobKey(nameof(OutboxProcessorJob));

            options.AddJob<OutboxProcessorJob>(opts => opts.WithIdentity(outBoxJobKey));

            options.AddTrigger(opts => opts
                .ForJob(outBoxJobKey)
                .WithIdentity("OutboxProcessorJob-trigger")
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromSeconds(3))
                    .RepeatForever()));
        });

        services.AddQuartzHostedService();
    }
}