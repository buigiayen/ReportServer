
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Data;
using System.Dynamic;
using System.Linq;
using DevExpress.Xpo.DB.Helpers;

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
                    dr[jprop.Name] = (object)(jprop.Value);
                    jtoken = jtoken.Next;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }
        public void TestJson()
        {
            string json = " {\r\n  name : 'table',\r\n  arr:[\r\n    {\r\n      tagert: \"paer\"\r\n    },\r\n     {\r\n      tagert: \"paer2\"\r\n    }\r\n  ]\r\n}";
            dynamic myData = JsonConvert.DeserializeAnonymousType(json, new ExpandoObject());

            Dictionary<string, Object> myDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            var dic = myDict.ToList();

        }
    }
}
