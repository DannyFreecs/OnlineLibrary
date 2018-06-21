using System;
using Library.Persistence;
using LibraryMVC.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bead1
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
            // Konfiguráció olvasása
            DbType dbType = Configuration.GetSection("CustomSettings").GetValue<DbType>("DbType");

            // Adatbázis kontextus függőségi befecskendezése
            switch (dbType)
            {
                case DbType.SqlServer:
                    services.AddDbContext<LibraryContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")));
                    break;
                case DbType.Sqlite:
                    services.AddDbContext<LibraryContext>(options =>
                        options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")));
                    break;
            }

            services.AddMvc();

            // Munkamenetkezelés beállítésa
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15); // max. 15 percig él a munkamenet
            });
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
                app.UseExceptionHandler("/Home/Error");
            }

            // Munkamentek használata
            app.UseSession();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Adatbázis inicializálása
            DbInitializer.Initialize(app.ApplicationServices.GetRequiredService<LibraryContext>(), "App_Data");
        }
    }
}
