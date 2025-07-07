using System.Threading;
using System.Threading.Tasks;

namespace CMS.Server.Services
{
    public interface IExpiredOTPService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
} 