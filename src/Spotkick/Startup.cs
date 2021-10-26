using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Spotkick.Interfaces;
using Spotkick.Models;
using Spotkick.Properties;
using Spotkick.Services;

namespace Spotkick
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddRouting();
            services.AddDbContext<SpotkickContext>(options =>
            {
                options.UseSqlServer(Resources.DbConnectionString);
                options.LogTo(_ => Debug.WriteLine(_));
            });
            services.AddSwaggerGen();

            // var optionsBuilder = new DbContextOptionsBuilder<SpotkickContext>();
            // optionsBuilder.UseSqlServer(Resources.DbConnectionString);
            //
            // using var db = new SpotkickContext(optionsBuilder.Options);
            // if (!db.Database.EnsureCreated())
            //     db.Database.Migrate();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Spotkick/Error");
            }

            app.UseStaticFiles()
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                })
                .UseMvc(routes =>
                {
                    routes.MapRoute("default", "{controller=Spotkick}/{action=Index}/{id?}");
                    routes.MapRoute("callback", "{controller=Spotkick}/{action=Callback}");
                });
        }
    }
}