using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraReports.UI;
using ServerSide.Models;
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

        public async Task<Stream> ExportReport(RequestReport requestReport, Extension extension)
        {
            var xtraReport = await ExportXtraReport(requestReport.ReportURL, requestReport.DataReport, requestReport.TableName);
            if(!string.IsNullOrEmpty(requestReport.Password) && extension != Extension.PDF ) { throw new InvalidDataException("Password only allowed in PDF");}
            switch (extension)
            {
                case Extension.PDF:
                    xtraReport.ExportOptions.Pdf.PasswordSecurityOptions.OpenPassword = requestReport.Password;
                    return await ExportPDF(xtraReport);
                case Extension.WORD:
                    return await ExportWord(xtraReport);
                case Extension.EXCEL:             
                    return await ExportExcel(xtraReport);
                default:
                    throw new Exception("Extension not found");
            }
        }
        public async Task<string> CreateReport(string JsonFormater)
        {
            string fileTemp = Path.GetTempFileName().Replace(".tmp", ".repx");
            XtraReport xtraReport = new XtraReport();
            xtraReport.CreateDocument();
            JsonDataSource JsonDataSource = new JsonDataSource();
            JsonDataSource.JsonSource = new CustomJsonSource(JsonFormater);
            JsonDataSource.Fill();
            xtraReport.DataSource = JsonDataSource;
            xtraReport.SaveLayoutToXml(fileTemp);
            return fileTemp;
        }
        private async Task<Stream> ExportPDF(XtraReport xtraReport)
        {
            Stream memoryStream = new MemoryStream();
            xtraReport.ExportToPdf(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private async Task<Stream> ExportWord(XtraReport xtraReport)
        {
            Stream memoryStream = new MemoryStream();
            xtraReport.ExportToDocx(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private async Task<Stream> ExportExcel(XtraReport xtraReport)
        {   
            Stream memoryStream = new MemoryStream();
            xtraReport.ExportToXlsx(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private async Task<XtraReport> ExportXtraReport(string UrlPath, string JsonFormater, string TableName)
        {
            var FileReport = streamReport.GetFileStreamFromUrl(UrlPath);
            XtraReport xtraReport = new XtraReport();
            xtraReport.LoadLayout(FileReport);
            JsonDataSource JsonDataSource = new JsonDataSource();
            JsonDataSource.JsonSource = new CustomJsonSource(JsonFormater);
            JsonDataSource.Fill();
            xtraReport.DataSource = JsonDataSource;
            xtraReport.DataMember = TableName;
            xtraReport.CreateDocument();
            return xtraReport;
        }

    }
}






