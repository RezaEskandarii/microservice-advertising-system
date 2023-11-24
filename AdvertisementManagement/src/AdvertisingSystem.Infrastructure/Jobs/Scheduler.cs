using Quartz;
using Quartz.Impl;

namespace AdvertisingSystem.Infrastructure.Jobs;

public class Scheduler
{
    private IScheduler _scheduler;

    public async Task Start()
    {
        // Create a Quartz scheduler
        var schedulerFactory = new StdSchedulerFactory();
        _scheduler = await schedulerFactory.GetScheduler();

        // Start the scheduler
        await _scheduler.Start();

        // Define the job and tie it to the job type
        var job = JobBuilder.Create<SyncReadDatabaseJob>()
            .WithIdentity("everyMinuteJob", "group1")
            .Build();

        // Trigger the job to run every minute
        var trigger = TriggerBuilder.Create()
            .WithIdentity("everyMinuteTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(1)
                .RepeatForever())
            .Build();

        // Schedule the job using the trigger
        await _scheduler.ScheduleJob(job, trigger);
    }

    public async Task Stop()
    {
        if (_scheduler != null && !_scheduler.IsShutdown)
        {
            await _scheduler.Shutdown();
        }
    }
}