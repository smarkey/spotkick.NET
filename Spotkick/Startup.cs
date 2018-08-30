using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Spotkick.Models;

namespace Spotkick
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
            services.AddMvc();

            var optionsBuilder = new DbContextOptionsBuilder<SpotkickContext>();
            optionsBuilder.UseSqlServer(Properties.Resources.DbConnectionString);

            using (var db = new SpotkickContext(optionsBuilder.Options))
            {
                if (!db.Database.EnsureCreated())
                    db.Database.Migrate();
            }

            services.AddDbContext<SpotkickContext>(options => options.UseSqlServer(Properties.Resources.DbConnectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Spotkick/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Spotkick}/{action=Index}/{id?}");
            });
        }
    }
}
