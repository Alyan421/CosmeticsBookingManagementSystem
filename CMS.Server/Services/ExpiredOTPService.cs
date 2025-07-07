using CMS.Server.Models;
using CMS.Server.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CMS.Server.Services
{
    public class ExpiredOTPService : IHostedService, IExpiredOTPService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public ExpiredOTPService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckForExpiredOTPs, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void CheckForExpiredOTPs(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IGenericRepository<User>>();
                var users = repository.GetAllAsync().Result;
                var expiredUsers = users.Where(u => !u.IsVerified && u.OTPExpiry.HasValue && u.OTPExpiry.Value < DateTime.Now).ToList();

                foreach (var user in expiredUsers)
                {
                    repository.DeleteAsync(user).Wait();
                }

                repository.SaveChangesAsync().Wait();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
} 