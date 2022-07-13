using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts
{
    public interface IDoctorChangeForPrescriptionService
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}
