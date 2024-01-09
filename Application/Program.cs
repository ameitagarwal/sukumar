using TMS_Traning_Management.Models;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using TMS_Traning_Management.Interfaces;
using TMS_Traning_Management.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Localization.Routing;
using System.Reflection;
using TMS_Traning_Management;
using TMS_Traning_Management.Security;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Adding Db Context
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
});

// Adding Localization
builder.Services.AddLocalization(options =>
{
	options.ResourcesPath = "Resources";
});
builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
	.AddDataAnnotationsLocalization(opts =>
    {
        opts.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(ValidationResources).GetTypeInfo().Assembly.FullName!);
            return factory.Create(nameof(ValidationResources), assemblyName.Name!);
        };
    });

builder.Services.AddDbContext<AppDbContext>(); // Add your ApplicationDbContext here
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// Repository dependancy injection.
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IRegisterUserService, RegisterUserService>();
builder.Services.AddScoped<ITrainingsService, TrainingsService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddSingleton<DataProtectionPurposeStrings>();

// configuring localization
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	options.DefaultRequestCulture = new RequestCulture("en");

	var AllowedLanguages = builder.Configuration["AllowedLanguages"]?.Split(",");
	var cultures = new CultureInfo[AllowedLanguages.Length];
	for (var i = 0; i < AllowedLanguages.Length; i++)
	{
		cultures[i] = new CultureInfo(AllowedLanguages[i]);
	}

	options.SupportedCultures = cultures;
	options.SupportedUICultures = cultures;
	options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());
});


// Logging file configeration
builder.Host.UseNLog();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
	app.UseStatusCodePages();
	//app.UseStatusCodePagesWithRedirects("/Error/{0}");
}
app.UseStaticFiles();
app.UseRouting();
app.UseRequestLocalization();
app.MapControllerRoute(
    name: "default",
    pattern: "{culture=en}/{controller=Home}/{action=Index}/{id?}");

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    var context = services.GetRequiredService<AppDbContext>();
//    context.Database.EnsureCreated();
//    context.Database.Migrate();
//}

app.Run();