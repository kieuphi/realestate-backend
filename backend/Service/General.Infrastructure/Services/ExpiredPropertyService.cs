using Common.Shared.Enums;
using General.Application.Interfaces;
using General.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Infrastructure.Services
{
    public class ExpiredPropertyService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpiredPropertyService> _logger;
        private Timer? _timer = null;

        public ExpiredPropertyService(IServiceProvider serviceProvider,ILogger<ExpiredPropertyService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ExpiredPropertyService start");

            _timer = new Timer(Process, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(10));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ExpiredPropertyService stopped");
        }

        private async void Process(object? state)
        {
            _logger.LogInformation("Check expired property");
            var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<IApplicationDbContext>();

            var listProperty = await dbContext.Property
                .Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove == PropertyApproveStatus.Active)
                .ToListAsync();
            _logger.LogInformation("Property active count: "+ listProperty.Count);
            DateTime now = DateTime.Now;
            if (listProperty.Count > 0)
            {
                for (int i = 0; i < listProperty.Count; i++)
                {
                    var timeForPost = await dbContext.TimeForPost.Where(x => x.Id == listProperty[i].TimeForPostId).FirstOrDefaultAsync();
                    if (timeForPost != null)
                    {
                        if (listProperty[i].ExpiredDate == null)
                        {
                            if (listProperty[i].ApproveDate != null)
                            {
                                listProperty[i].ExpiredDate = listProperty[i].ApproveDate.Value.AddDays(Convert.ToDouble(timeForPost.Value));
                            }
                        }
                        if (now > listProperty[i].ExpiredDate)
                        {
                            listProperty[i].IsApprove = PropertyApproveStatus.Expired;
                            _logger.LogInformation($"Property {listProperty[i].PropertyNumber} is expired");
                        }
                    }
                }
                
                await dbContext.SaveChangesAsync();
            }
            _logger.LogInformation("Check expired property end");
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    }
}
