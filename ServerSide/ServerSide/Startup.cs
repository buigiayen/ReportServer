using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ServerSide.infrastructure;
using ServerSide.Interfaces;
using ServerSide.Services;
using System;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDevExpressControls();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.ConfigureReportingServices(configurator =>
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
builder.Services.AddTransient<StreamReport>();
builder.Services.AddTransient<JsonToDataTable>();
builder.Services.AddTransient<ReportExport>();

builder.Services.AddTransient<IRequire, SendMessage2>();

// ..
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "System.api", Version = "v1" });
});
builder.Services.AddCors();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

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
DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new CustomReportStorageWebExtension(app.Environment));
app.Run();