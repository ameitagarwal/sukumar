using TMS_Training_Management_StatusChange_Scheduler.Interfaces;

namespace TMS_Training_Management_StatusChange_Scheduler.Services
{
	public class RepositoryService : BackgroundService
	{
		private readonly ILogger<RepositoryService> _logger;
		private readonly IConfiguration _config;
		private readonly IRegistrationService _registrationService;
		public RepositoryService(
			ILogger<RepositoryService> logger,
			IConfiguration config, 
			IRegistrationService registrationService)
		{
			_logger = logger;
			_config = config;
			_registrationService = registrationService;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					_logger.Log(LogLevel.Information, "Service Started");
					_registrationService.ResetpaymentStatus();
				}
				catch (Exception ex)
				{
					_logger.Log(LogLevel.Error, ex.Message);
				}
				await Task.Delay(Convert.ToInt32(_config["Interval"]) * 60 * 1000, stoppingToken);
			}
		}
		public override Task StartAsync(CancellationToken cancellationToken)
		{
			return base.StartAsync(cancellationToken);
		}
		public override Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.Log(LogLevel.Information, "Service Stopped");
			return base.StopAsync(cancellationToken);
		}
	}
}
