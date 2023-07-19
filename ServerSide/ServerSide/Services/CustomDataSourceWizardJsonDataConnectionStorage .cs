using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.Web;
using DevExpress.DataAccess.Wizard.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ServerSide.Code;

namespace ServerSide.Services
{
    public class CustomDataSourceWizardJsonDataConnectionStorage : IDataSourceWizardJsonConnectionStorage
    {
        public const string JsonDataConnectionsKey = "dxJsonDataConnections";
        protected IWebHostEnvironment Environment { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }

        public CustomDataSourceWizardJsonDataConnectionStorage(
            IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            Environment = env;
            HttpContextAccessor = httpContextAccessor;
        }

        public Dictionary<string, string> GetConnections()
        {
            if (HttpContextAccessor.HttpContext == null || HttpContextAccessor.HttpContext.Session == null)
            {
                return null;
            }
            var connectionStrings =
                HttpContextAccessor.HttpContext.Session
                .GetObjectFromJson<Dictionary<string, string>>(JsonDataConnectionsKey);
            if (connectionStrings == null)
            {
                connectionStrings = GetDefaults();
                UpdateSessionState(connectionStrings);
            }
            return connectionStrings;
        }

        bool IJsonConnectionStorageService.CanSaveConnection
        {
            get
            {
                return HttpContextAccessor.HttpContext != null
                    && HttpContextAccessor.HttpContext.Session != null;
            }
        }
        bool IJsonConnectionStorageService.ContainsConnection(string connectionName)
        {
            var connections = GetConnections();
            return connections == null ? false : connections.ContainsKey(connectionName);
        }

        IEnumerable<JsonDataConnection> IJsonConnectionStorageService.GetConnections()
        {
            var connections = GetConnections();
            if (connections == null)
            {
                return new List<JsonDataConnection>();
            }
            return connections.Select(x => CreateJsonDataConnectionFromString(x.Key, x.Value));

        }

        JsonDataConnection IJsonDataConnectionProviderService.GetJsonDataConnection(string name)
        {
            var connections = GetConnections();
            if (connections == null || !connections.ContainsKey(name))
                throw new InvalidOperationException();
            return CreateJsonDataConnectionFromString(name, connections[name]);
        }

        void IJsonConnectionStorageService.SaveConnection(string connectionName,
            JsonDataConnection dataConnection, bool saveCredentials)
        {
            var connections = GetConnections();
            if (connections == null)
            {
                return;
            }
            var connectionString = dataConnection.CreateConnectionString();
            if (connections.ContainsKey(connectionName))
            {
                connections[connectionName] = connectionString;
            }
            else
            {
                connections.Add(connectionName, connectionString);
            }
            UpdateSessionState(connections);
        }

        Dictionary<string, string> GetDefaults()
        {
            var connections = new Dictionary<string, string>();
            var uri = new Uri(Path.Combine(Environment.ContentRootPath, "Data", "nwind.json"),
                UriKind.Relative);
            var dataConnection = new JsonDataConnection(new UriJsonSource(uri))
            {
                StoreConnectionNameOnly = true,
                Name = "Products (JSON)"
            };
            connections.Add(dataConnection.Name, dataConnection.CreateConnectionString());
            return connections;
        }

        void UpdateSessionState(Dictionary<string, string> connectionStrings)
        {
            HttpContextAccessor.HttpContext.Session.SetObjectAsJson(JsonDataConnectionsKey,
                connectionStrings);
        }

        public static JsonDataConnection CreateJsonDataConnectionFromString(string connectionName,
            string connectionString)
        {
            return new JsonDataConnection(connectionString)
            {
                StoreConnectionNameOnly = true,
                Name = connectionName
            };
        }
    }
}