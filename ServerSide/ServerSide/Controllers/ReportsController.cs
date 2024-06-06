
using DevExpress.CodeParser.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using ServerSide.infrastructure;
using ServerSide.Models;
using System.Net.Mime;
using System.Threading.Tasks;


namespace ServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private ReportExport reportExport;
        private ILogger<ReportsController> logger;

        public ReportsController(ReportExport reportExport, ILogger<ReportsController> logger)
        {
            this.reportExport = reportExport;
            this.logger = logger;
        }
        [HttpPost("ReportView/Create")]
        public async Task<IActionResult> CreateReport([FromBody] RequestReport requestReport)
        {
            logger.LogDebug("Create Report");
            var reportStream = await reportExport.CreateReport(requestReport.DataReport);
            return File(reportStream, "application/octet-stream", "ReportBlank.repx");
        }

        [HttpPost("ReportView/PDF/Preview")]
        public async Task<IActionResult> PDfPreview([FromBody] RequestReport requestReport)
        {
            logger.LogDebug("Preview Report");
            var reportStream = await reportExport.ExportReport(requestReport, Data.FileExtension.Extension.PDF);
            return File(reportStream, MediaTypeNames.Application.Pdf);
        }

        [HttpPost("ReportView/PDF")]
        public async Task<IActionResult> PDfExports([FromBody] RequestReport requestReport)
        {
            logger.LogDebug("Export PDF");
            var reportStream = await reportExport.ExportReport(requestReport, Data.FileExtension.Extension.PDF);
            return File(reportStream, "application/octet-stream", "Filename.pdf");
        }

        [HttpPost("ReportView/Word")]
        public async Task<IActionResult> WordExport([FromBody] RequestReport requestReport)
        {
            logger.LogDebug("Export Word");
            var reportStream = await reportExport.ExportReport(requestReport, Data.FileExtension.Extension.WORD);
            return File(reportStream, "application/octet-stream", "Filename.docx");
        }

        [HttpPost("ReportView/Excel")]
        public async Task<IActionResult> ExcelExport([FromBody] RequestReport requestReport)
        {
            logger.LogDebug("Export Excel");
            var reportStream = await reportExport.ExportReport(requestReport, Data.FileExtension.Extension.EXCEL);
            return File(reportStream, "application/octet-stream", "Filename.xlsx");
        }

    }
}
