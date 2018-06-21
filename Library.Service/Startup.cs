using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Persistence;
using Library.Service.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Library.Service
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvc();

            // Adatbázis inicializálása
            DbInitializer.Initialize(app.ApplicationServices.GetRequiredService<LibraryContext>(), "App_Data");
        }
    }
}
