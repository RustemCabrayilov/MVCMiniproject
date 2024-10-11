using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OMMS.DAL.Data;
using OMMS.DAL.Entities;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using Serilog.Core;
using Microsoft.AspNetCore.HttpLogging;
using OMMS.DAL.Repository.Interface;
using OMMS.DAL.Repository;
using OMMS.SignalR.HubServices.Interfaces;
using OMMS.SignalR.HubServices;
using OMMS.UI.Configuration;
using OMMS.UI.Services.Interfaces;
using OMMS.UI.Services;
using NToastNotify;
using FluentValidation.AspNetCore;
using OMMS.UI.Validation;

namespace OMMS.UI.ServiceRegistration
{
	public static class ServiceRegister
	{
		public static void Register(this IServiceCollection services, IConfiguration configuration, WebApplicationBuilder builder)
		{
			var toastrOptions = new ToastrOptions()
			{
				ProgressBar = false,
				PositionClass = ToastPositions.TopRight,
				ShowDuration = 500,

			};

			services.AddControllersWithViews()
				  .AddNToastNotifyToastr(toastrOptions);
			services.AddSignalR();

			services.AddCors(setup =>
			{

				setup.AddDefaultPolicy(policy =>
				{
					policy.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials()
					.SetIsOriginAllowed(orgin => true);

				});

			});
			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
			services.AddScoped<IEmailService, EmailService>();
			services.AddTransient<IProductHubService, ProductHubService>();
			Logger log = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File("Logs/log.txt")
	.WriteTo.MSSqlServer(configuration.GetConnectionString("SqlConnectionString"),
   sinkOptions: new MSSqlServerSinkOptions
   {
	   TableName = "Logs",
	   AutoCreateSqlTable = true

   },
	columnOptions: new Serilog.Sinks.MSSqlServer.ColumnOptions
	{
		AdditionalColumns = new List<SqlColumn>
		{
						new SqlColumn("user_name", System.Data.SqlDbType.NVarChar)
		}
	})
	.Enrich.FromLogContext()
	.MinimumLevel.Information()
	.CreateLogger();
			builder.Host.UseSerilog(log);
			services.AddHttpLogging(logging =>
			{
				logging.LoggingFields = HttpLoggingFields.All;
				logging.RequestHeaders.Add("sec-ch-ua");
				logging.ResponseHeaders.Add("OMMS");
				logging.MediaTypeOptions.AddText("application/javascript");
				logging.RequestBodyLogLimit = 4096;
				logging.ResponseBodyLogLimit = 4096;
				logging.CombineLogs = true;
			});




			services.AddDbContext<OMMSDbContext>(
			opts => opts.UseSqlServer(configuration.GetConnectionString("SqlConnectionString")));
			services.AddIdentity<AppUser, AppRole>(opts =>
				{

					opts.Password.RequireDigit = true;
					opts.Password.RequireLowercase = false;
					opts.Password.RequireNonAlphanumeric = false;
					opts.Password.RequireUppercase = false;
					opts.Password.RequiredLength = 6;

				}).AddEntityFrameworkStores<OMMSDbContext>().AddDefaultTokenProviders();
			services.Configure<DataProtectionTokenProviderOptions>(opts =>
			{
				opts.TokenLifespan = TimeSpan.FromHours(2);
			});
			services.ConfigureApplicationCookie(cf =>
			{

				CookieBuilder cookieBuilder = new CookieBuilder()
				{
					Name = "OMMSOperationUI",
					Path = "/",

				};
				cf.Cookie = cookieBuilder;
				cf.ExpireTimeSpan = TimeSpan.FromDays(1);
				cf.LoginPath = "/Account/LogIn";

			});
			services.AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining(typeof(MerchantValidator)));
			services.AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining(typeof(BranchValidator)));
			services.AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining(typeof(EmployeeValidator)));
			services.AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining(typeof(SignUpValidator)));

		}
	}
}
