using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneMedify.Services.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.BackgroundServices
{
    public class DoctorChangeService : BackgroundService
    {
        public IServiceProvider Services { get; }
        public DoctorChangeService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {           
            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IDoctorChangeForPrescriptionService>();

                await scopedProcessingService.DoWork(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}
