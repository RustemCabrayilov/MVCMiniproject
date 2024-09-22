using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OMMS.DAL.Data;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository;
using OMMS.DAL.Repository.Interface;

namespace OMMS.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<OMMSDbContext>(
                opts=>opts.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnectionString")));
            builder.Services.AddIdentity<AppUser, AppRole>(opts =>
            {

                opts.Password.RequireDigit = true;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequiredLength = 6;

            }).AddEntityFrameworkStores<OMMSDbContext>();
            
            
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
            app.MapControllerRoute(
                      name: "areas",
                      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                    );
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=LogIn}/{id?}");

            app.Run();
        }
    }
}
