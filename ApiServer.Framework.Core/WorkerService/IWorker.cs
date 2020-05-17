using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApiServer.Framework.Core.WorkerService
{
    public interface IWorker
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}
