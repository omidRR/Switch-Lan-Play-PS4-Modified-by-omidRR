using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Switch_Lan_Play_Modified_by_omidRR
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc(options => options.EnableEndpointRouting = false);

                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.WriteIndented = true;
                    });

                services.AddSingleton(Program._slpServer);

                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            }
            catch
            {
                throw;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) // IHostingEnvironment is obsolete since .NET Core 3.0
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                    app.UseHttpsRedirection(); // Moved inside the else block
                }

                app.UseMvc();
            }
            catch
            {
                throw;
            }
        }
    }
}