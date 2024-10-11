using Azure;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NToastNotify;
using OMMS.DAL.Data;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository;
using OMMS.DAL.Repository.Interface;
using OMMS.SignalR.Hubs;
using OMMS.SignalR.HubServices;
using OMMS.SignalR.HubServices.Interfaces;
using OMMS.UI.Configuration;
using OMMS.UI.ServiceRegistration;
using OMMS.UI.Services;
using OMMS.UI.Services.Interfaces;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.MSSqlServer;

namespace OMMS.UI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Services.Register(builder.Configuration,builder);


			builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
			var app = builder.Build();


			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseStaticFiles();
			app.UseSerilogRequestLogging();
			app.UseHttpLogging();
			app.UseHttpsRedirection();
			app.UseRouting();
			/*app.UseAuthentication();*/
			app.UseAuthorization();
			app.Use(async (context, next) =>
			{
				var userName = context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : null;
				LogContext.PushProperty("user_name", userName);
				await next();
			});
			app.UseNToastNotify();
			app.MapControllerRoute(
					  name: "areas",
					  pattern: "{area:exists}/{controller=Account}/{action=LogIn}/{id?}"
					);
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Account}/{action=LogIn}/{id?}");
			app.UseCors();
			app.MapHubs();
			app.Run();
		}
	}
}
