using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Web;
using System.Collections.Generic;
using System.Linq;

namespace ServerSide.Services
{
    public class MyDataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider
    {
        public Dictionary<string, string> GetConnectionDescriptions()
        {
            Dictionary<string, string> connections = AppConfigHelper.GetConnections().Keys.ToDictionary(x => x, x => x);

            // Customize the loaded connections list. 
            connections.Remove("Cars");
            connections.Add("Custom Connection", "Custom SQL Connection");
            return connections;
        }

        public DataConnectionParametersBase GetDataConnectionParameters(string name)
        {
            // Return custom connection parameters for the custom connection. 
            if (name == "Custom Connection")
            {
                return new MsSqlConnectionParameters("host.docker.internal", "Web_KBHM", "sa", "Buiyen123>", MsSqlAuthorizationType.SqlServer);
            }
            return AppConfigHelper.LoadConnectionParameters(name);
        }
    }
}
