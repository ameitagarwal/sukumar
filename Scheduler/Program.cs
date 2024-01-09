using NLog.Web;
using TMS_Training_Management_StatusChange_Scheduler.Interfaces;
using TMS_Training_Management_StatusChange_Scheduler.Services;
var host = Host.CreateDefaultBuilder(args)
	.UseWindowsService()
	.ConfigureServices(services =>
	{
		services.AddTransient<IRegistrationService, RegistrationService>();
		services.AddHostedService<RepositoryService>();
	})
	.ConfigureLogging(logging => {
		logging.ClearProviders();
		logging.SetMinimumLevel(LogLevel.Trace);
	}).UseNLog()
	.Build();
host.Run();
