
using Newtonsoft.Json.Linq;
using System.Data;

namespace ServerSide.infrastructure
{


    public class JsonToDataTable
    {
        public DataTable JsonToDataTableNew(string json, string tableName)
        {
            var dt = new DataTable(tableName);
            var root = JObject.Parse(json);
            var items = root[tableName] as JArray;

            if (items == null || items.Count == 0)
            {
                return dt;
            }

            JObject item = items[0] as JObject;

            foreach (var p in item.Properties())
            {
                dt.Columns.Add(new DataColumn(p.Name));
            }

            for (int i = 0; i < items.Count; i++)
            {
                var dr = dt.NewRow();
                var obj = items[i] as JObject;
                var jtoken = obj.First;

                while (jtoken != null)
                {
                    var jprop = jtoken as JProperty;
                    dr[jprop.Name] = jprop.Value.ToString();
                    jtoken = jtoken.Next;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}
