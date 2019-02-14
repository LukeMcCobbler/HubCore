using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HubCore.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HubCore
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddScoped<IInfoRepository, DBInfoRepository>();
            services.AddScoped<IQueryLogicResolverFactory, CompositeLogicResolverFactory>();
            services.AddScoped<IDataLayer, CommonDataLayer>(srv => { return new CommonDataLayer(srv.GetService<ISettingsManager>(), srv.GetService<ApplicationInfo>(), "HubParameterDB"); });
            services.AddScoped<ISettingsManager, XMLFileSettingsManager>();
            services.AddScoped((srv) => { return new ApplicationInfo() { ApplicationName = "HubCore",GlobalSettingsFilePath=getGlobalSettingsPath(),InstanceSettingsFilePath=getInstanceSettingsPath() }; });
            services.AddScoped<ILogger, PassThroughLogger>();
        }

        private string getInstanceSettingsPath()
        {

            return getDataFilePath($"InstanceSettings_{Environment.MachineName}.xml");
        }

        private string getDataFilePath(string DataFileName)
        {
            var dataDir=AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            return Path.Combine(dataDir, DataFileName);
        }

        private string getGlobalSettingsPath()
        {
            return getDataFilePath("GlobalSettings.xml");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            string baseDir = env.WebRootPath;
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(baseDir, "App_Data"));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
            app.UseMvcWithDefaultRoute();
        }
    }
}
