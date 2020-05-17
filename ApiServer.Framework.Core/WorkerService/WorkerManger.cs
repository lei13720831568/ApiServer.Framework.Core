using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApiServer.Framework.Core.WorkerService
{
    public abstract class WorkerManger : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public WorkerManger(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        protected List<Task> works=new List<Task>();
        protected abstract void ConfigWorker(CancellationToken stoppingToken);

        protected void AddWorker<T>(CancellationToken stoppingToken) where T : IWorker
        {
            works.Add(CreateWorker<T>(stoppingToken));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.ConfigWorker(stoppingToken);
            await Task.WhenAll(works.ToArray());
        }

        private async Task CreateWorker<T>(CancellationToken stoppingToken) where T : IWorker
        {

            using (var scope = scopeFactory.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<T>();
                await scopedProcessingService.DoWork(stoppingToken);
            }
        }

    }
}
