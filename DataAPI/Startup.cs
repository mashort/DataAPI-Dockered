using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DataAPI.Models;
using System.Text;

namespace DataAPI
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
            services.AddControllers();

            var config = new StringBuilder (Configuration["ConnectionStrings:MagsConnectionMssql"]);
            string conn = config.Replace("ENVSRV", Configuration["DB_Server"])
                                .Replace("ENVID", Configuration["DB_UserId"])
                                .Replace("ENVPW", Configuration["DB_PW"])
                                .ToString();

            services.AddDbContext<MagContext>(options =>
                //options.UseSqlServer(Configuration.GetConnectionString("MagsConnectionMssql")));
                //options.UseSqlite(Configuration.GetConnectionString("MagsConnectionSqlite")));
                options.UseSqlServer(conn));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
