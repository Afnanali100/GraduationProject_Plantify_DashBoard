using ControlPanel.DAL.Context;
using ControlPanel.DAL.Models;
using ControlPanel.PLL.Helper;
using ControlPanel.PLL.Profiles;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity;

namespace ControlPanel.PLL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // DBConnection
            builder.Services.AddDbContext<dbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("defaultconnection"));
            });

            // AutoMapper
            builder.Services.AddAutoMapper(m => m.AddProfile(new UserProfile()));

            // Identity & Google
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireUppercase = true;
            }).AddEntityFrameworkStores<dbContext>().AddDefaultTokenProviders();

            builder.Services.AddAuthentication( CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Home/error";
            });
           
            // MailKit
            builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSetting"));
            builder.Services.AddScoped<IEmailSetting, EmailSetting>();

            // Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();  
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=User}/{action=Index}/{id?}");
            });

            app.Run();
        }
    }
}
