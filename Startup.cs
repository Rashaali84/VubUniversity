using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VubUniversity.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;

namespace VubUniversity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>(

                options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();
        

            //register the new db context for dependency injection 
            services.AddDbContext<SchoolContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //samesite cookie
            services.Configure<CookiePolicyOptions>
                (options => { 
                    options.MinimumSameSitePolicy = SameSiteMode.Strict; 
                    options.Secure =  CookieSecurePolicy.Always; 
                    options.HttpOnly = HttpOnlyPolicy.Always; });

            
            services.AddControllersWithViews();
            //services.AddRazorPages();
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            })
            .AddRazorPagesOptions(options =>
            {
                options.Conventions
                       .ConfigureFilter(new AutoValidateAntiforgeryTokenAttribute());
            });

            // here to allow our fix to be applied we need to ruen this option to false
            services.AddAntiforgery(options =>
            {
                options.SuppressXFrameOptionsHeader = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //the configure function here act as a middle ware layer to add necessery tags security headers
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add(" X-Frame-Options", "DENY");
                //Using a content security policy ! 
                /*string scriptSrc = "script-src 'self' https:// code.jquery.com;"; 
                string styleSrc = "style-src 'self' 'unsafe-inline';"; 
                string imgSrc = "img-src 'self' https:// www.packtpub.com/;";
                string objSrc = "object-src 'none'"; 
                string defaultSrc = "default-src 'self';";
                string csp = $"{ defaultSrc}{ scriptSrc}{scriptSrc}{ styleSrc} {imgSrc}{ objSrc}";
                context.Response.Headers.Add($" Content-Security- Policy", csp);*/

            
                await next();
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
