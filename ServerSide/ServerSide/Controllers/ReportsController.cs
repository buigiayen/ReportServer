using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerSide.infrastructure;
using ServerSide.Models;
using System.Threading.Tasks;
using System;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services;

namespace ServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private ReportExport reportExport;
        private StreamReport streamReport;
      
        public ReportsController( StreamReport streamReport,  ReportExport reportExport)
        {
            this.streamReport = streamReport;
            this.reportExport = reportExport;
        }
        [HttpPost("ReportView/PDF")]
        public async Task<IActionResult> PDfExports([FromBody] RequestReport requestReport)
        {
            string Formater = @"{""" + requestReport.TableName + "\" : " + requestReport.DataReport + " }";   
            var reportStream = await reportExport.ExportReport(requestReport.ReportURL, Formater, requestReport.TableName, Data.FileExtension.Extension.PDF);
            return File(reportStream, "application/pdf");
        }
        [HttpPost("ReportView/Word")]
        public async Task<IActionResult> WordExport([FromBody] RequestReport requestReport)
        {
            string Formater = @"{""" + requestReport.TableName + "\" : " + requestReport.DataReport + " }";
            var reportStream = await reportExport.ExportReport(requestReport.ReportURL, Formater, requestReport.TableName, Data.FileExtension.Extension.WORD);
            return File(reportStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document","Filename.docx");
        }
    }
}
