using System.Diagnostics;
using System.Linq;
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
using Spotkick.Models.Songkick;
using Spotkick.Models.Spotify;
using Spotkick.Properties;
using Spotkick.Services;

namespace Spotkick
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddRouting();
            services.AddSwaggerGen();
            services.AddScoped<IArtistService, ArtistService>();
            services.AddScoped<IUserService, UserService>();
            
            services.AddOptions();
            services.Configure<SpotifyConfig>(Configuration.GetSection("Spotify"));
            services.Configure<SongkickConfig>(Configuration.GetSection("Songkick"));

            services.AddDbContext<SpotkickDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Default"));
                options.LogTo(_ => Debug.WriteLine(_));
            });

            var optionsBuilder = new DbContextOptionsBuilder<SpotkickDbContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("Default"));

            using var db = new SpotkickDbContext(optionsBuilder.Options);
            var migrationsToRun = db.Database.GetPendingMigrations();
            
            if (migrationsToRun.Any())
                db.Database.Migrate();
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
                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseStaticFiles()
                .UseSwagger()
                .UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Spotkick API V1"); })
                .UseMvc(routes =>
                {
                    routes.MapRoute("default", "{controller=Spotkick}/{action=Index}/{id?}");
                    routes.MapRoute("callback", "{controller=Spotkick}/{action=Callback}");
                });
        }
    }
}