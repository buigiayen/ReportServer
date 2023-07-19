using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ServerSide.Code;
using ServerSide.Code;
using ServerSide.Services;

namespace ServerSide.Services
{
    public class CustomJsonDataConnectionProviderFactory : IJsonDataConnectionProviderFactory
    {
        protected IWebHostEnvironment Environment { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }
        Dictionary<string, string> connectionStrings;

        public CustomJsonDataConnectionProviderFactory(IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            Environment = env;
            HttpContextAccessor = httpContextAccessor;
            if (HttpContextAccessor.HttpContext == null ||
                HttpContextAccessor.HttpContext.Session == null)
            {
                connectionStrings = null;
            }
            else
            {
                connectionStrings =
                    HttpContextAccessor.HttpContext.Session.
                    GetObjectFromJson<Dictionary<string, string>>(
                        CustomDataSourceWizardJsonDataConnectionStorage.JsonDataConnectionsKey);
            }
        }

        public IJsonDataConnectionProviderService Create()
        {
            return new WebDocumentViewerJsonDataConnectionProvider(connectionStrings);
        }
    }

    public class WebDocumentViewerJsonDataConnectionProvider : IJsonDataConnectionProviderService
    {
        readonly Dictionary<string, string> jsonDataConnections;
        public WebDocumentViewerJsonDataConnectionProvider(Dictionary<string, string> jsonDataConnections)
        {
            this.jsonDataConnections = jsonDataConnections;
        }
        public JsonDataConnection GetJsonDataConnection(string name)
        {
            if (jsonDataConnections == null)
                return null;
            return CustomDataSourceWizardJsonDataConnectionStorage.CreateJsonDataConnectionFromString(
                name, jsonDataConnections[name]);
        }
    }
}