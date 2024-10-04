using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NToastNotify;
using OMMS.DAL.Data;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Configuration;
using OMMS.UI.Services;
using OMMS.UI.Services.Interfaces;

namespace OMMS.UI
{
    public class Program
    {
        public static  void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var toastrOptions = new ToastrOptions()
            {
                ProgressBar = false,
                PositionClass = ToastPositions.TopRight,
                ShowDuration = 500,
                
            };
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IEmailService, EmailService>();
            // Add services to the container.
            builder.Services.AddControllersWithViews()
              .AddNToastNotifyToastr(toastrOptions);
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            builder.Services.AddDbContext<OMMSDbContext>(
                opts=>opts.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnectionString")));
            builder.Services.AddIdentity<AppUser, AppRole>(opts =>
            {

                opts.Password.RequireDigit = true;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequiredLength = 6;

            }).AddEntityFrameworkStores<OMMSDbContext>().AddDefaultTokenProviders();
            builder.Services.Configure<DataProtectionTokenProviderOptions>(opts =>
            {
                opts.TokenLifespan = TimeSpan.FromHours(2);
            });


            builder.Services.ConfigureApplicationCookie(cf =>
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

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseNToastNotify();
            app.MapControllerRoute(
                      name: "areas",
                      pattern: "{area:exists}/{controller=Account}/{action=LogIn}/{id?}"
                    );
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=LogIn}/{id?}");

            app.Run();
        }
    }
}
