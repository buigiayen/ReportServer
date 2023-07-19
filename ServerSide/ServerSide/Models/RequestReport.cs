using System;

namespace ServerSide.Models
{
    public class RequestReport
    {
        public string ReportName { get; set; }
        public string DataReport { get; set; }
        public string TableName { get; set; } = "DataTable";
        public string ReportTilte { get; set; } = Guid.NewGuid().ToString();
    }
}
