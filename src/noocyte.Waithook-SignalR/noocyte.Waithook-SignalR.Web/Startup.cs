using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using noocyte.Waithook_SignalR.Web.Controllers;

namespace noocyte.Waithook_SignalR.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddControllers();
            if (env.IsDevelopment())
            {
                services.AddSignalR();
            }
            else
            {
                services.AddSignalR().AddAzureSignalR(); // Azure:SignalR:ConnectionString
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<WaithookHub>("/waithookhub");
            });
        }
    }
}
