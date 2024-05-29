using DevExpress.DataAccess.ObjectBinding;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit.Commands;
using Microsoft.Extensions.FileProviders;
using ServerSide.Models;
using System;
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

        public async Task<Stream> ExportReport(string UrlReport, string JsonFormater, string TableName, Extension extension )
        {
            switch (extension)
            {
                case Extension.PDF:
                    return await ExportPDF(UrlReport , JsonFormater , TableName);
                case Extension.WORD:
                    return await ExportToDocx(UrlReport, JsonFormater, TableName);
                case Extension.EXCEL:
                    return await ExportToDocx(UrlReport, JsonFormater, TableName);
                default:
                    throw new Exception("Extension not found");
            }
        }
        private  async Task<Stream> ExportPDF(string UrlReport, string JsonFormater, string TableName)
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
        private async Task<XtraReport> ExportXtraReport(string UrlPath, string JsonFormater, string TableName)
        {
            var FileReport = streamReport.GetFileStreamFromUrl(UrlPath);
            JsonToDataTable jsonToDataTable = new JsonToDataTable();
            XtraReport xtraReport = new XtraReport();
            xtraReport.LoadLayout(FileReport);
            ObjectDataSource objectDataSource = new ObjectDataSource();
            objectDataSource.DataSource = jsonToDataTable.JsonToDataTableNew(JsonFormater, TableName);
            xtraReport.DataSource = objectDataSource;
            xtraReport.CreateDocument();
            return xtraReport;
        }

    }
}






