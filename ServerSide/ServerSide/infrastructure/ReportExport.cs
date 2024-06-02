using DevExpress.DataAccess.Json;
using DevExpress.XtraReports.UI;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using static ServerSide.Data.FileExtension;

namespace ServerSide.infrastructure
{
    public class ReportExport
    {
        private StreamReport streamReport;
        public ReportExport(StreamReport streamReport)
        {
            this.streamReport = streamReport;
        }

        public async Task<Stream> ExportReport(string UrlReport, string JsonFormater, string TableName, Extension extension)
        {
            switch (extension)
            {
                case Extension.PDF:
                    return await ExportPDF(UrlReport, JsonFormater, TableName);
                case Extension.WORD:
                    return await ExportToDocx(UrlReport, JsonFormater, TableName);
                case Extension.EXCEL:
                    return await ExportToExcel(UrlReport, JsonFormater, TableName);
                default:
                    throw new Exception("Extension not found");
            }
        }
        private async Task<Stream> ExportPDF(string UrlReport, string JsonFormater, string TableName)
        {
            var xtraReport = await ExportXtraReport(UrlReport, JsonFormater, TableName);
            Stream memoryStream = new MemoryStream();
            xtraReport.ExportToPdf(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private async Task<Stream> ExportToDocx(string UrlReport, string JsonFormater, string TableName)
        {
            var xtraReport = await ExportXtraReport(UrlReport, JsonFormater, TableName);
            Stream memoryStream = new MemoryStream();
            xtraReport.ExportToDocx(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private async Task<Stream> ExportToExcel(string UrlReport, string JsonFormater, string TableName)
        {
            var xtraReport = await ExportXtraReport(UrlReport, JsonFormater, TableName);
            Stream memoryStream = new MemoryStream();
            xtraReport.ExportToXlsx(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private async Task<XtraReport> ExportXtraReport(string UrlPath, string JsonFormater, string TableName)
        {
            var FileReport = streamReport.GetFileStreamFromUrl(UrlPath);
            JsonToDataTable jsonToDataTable = new JsonToDataTable();
            XtraReport xtraReport = new XtraReport();
            xtraReport.LoadLayout(FileReport);
            JsonDataSource JsonDataSource = new JsonDataSource();
            JsonDataSource.JsonSource = new CustomJsonSource(JsonFormater);
            xtraReport.DataSource = JsonDataSource;
            xtraReport.DataMember = TableName;
            xtraReport.CreateDocument();
            return xtraReport;
        }
     
    }
}






