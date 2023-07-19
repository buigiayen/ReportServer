using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ServerSide.Services;
using System;

namespace ServerSide
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
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddDevExpressControls();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.ConfigureReportingServices(configurator =>
            {
                // ...
                configurator.ConfigureReportDesigner(designerConfigurator =>
                {
                    // ...
                    //designerConfigurator.RegisterDataSourceWizardConfigFileJsonConnectionStringsProvider();
                    designerConfigurator.RegisterDataSourceWizardJsonConnectionStorage<CustomDataSourceWizardJsonDataConnectionStorage>(true);
                    //designerConfigurator.RegisterDataSourceWizardConnectionStringsProvider<MyDataSourceWizardConnectionStringsProvider>();
                    designerConfigurator.RegisterDataSourceWizardConnectionStringsProvider<MyDataSourceWizardConnectionStringsProvider>(true);
                    // ...
                });
                configurator.ConfigureWebDocumentViewer(viewerConfigurator =>
                {
                    // ...
                    viewerConfigurator.RegisterJsonDataConnectionProviderFactory<CustomJsonDataConnectionProviderFactory>();
                });

            });
            // ..
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "System.api", Version = "v1" });
            });
            services.AddCors();
            services.AddMvc(options => options.EnableEndpointRouting = false);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "System.api v1"));
            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();
            app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials()); // allow credentials
            app.UseDevExpressControls();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new CustomReportStorageWebExtension(env));
        }
    }
}
