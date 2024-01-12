using WorkService.NetCore6._0.Helpers;

namespace WorkService.NetCore6._0.Workers
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SqlHelper _sqlHelpers;
        public Worker(ILogger<Worker> logger, SqlHelper sqlHelpers)
        {
            _logger = logger;
            _sqlHelpers = sqlHelpers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Todo();

                _logger.LogInformation("Service started successfully: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private void Todo()
        {
            try
            {
                var now = DateTime.Now;
                if (now.Hour == 9)
                {

                    //

                    _logger.LogInformation("Action performed at 16 PM: {time}", DateTimeOffset.Now);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error when todo ~ " + ex.Message, DateTimeOffset.Now);
            }
        }
    }
}