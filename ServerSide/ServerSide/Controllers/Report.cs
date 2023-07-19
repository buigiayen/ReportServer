using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Reporting_ObjectDS_AspNetCore;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Collections;
using DevExpress.Office.Utils;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerSide.Models;
using System.Buffers.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace ServerSide.Controllers
{

    public class Report : Controller
    {
        readonly string ReportDirectory;
        const string FileExtension = ".repx";
        const string PDFExtension = ".pdf";
        private IWebHostEnvironment webHostEnvironment;
        public Report(IWebHostEnvironment env)
        {
            ReportDirectory = Path.Combine(env.ContentRootPath.ToString(), "Reports");
            if (!Directory.Exists(ReportDirectory))
            {
                Directory.CreateDirectory(ReportDirectory);
            }
        }
        [HttpPost("ReportView")]
        public async Task<IActionResult> Index([FromBody] RequestReport requestReport)
        {
            string base64Convert = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(requestReport.DataReport));
            string Formater = @"{""" + requestReport.TableName + "\" : " + base64Convert + " }";
            string PDFFilePath = Path.Combine(ReportDirectory, requestReport.ReportTilte + PDFExtension);
            string PathRepx = Path.Combine(ReportDirectory, requestReport.ReportName + FileExtension);
            XtraReport xtraReport = new XtraReport();
            if (!System.IO.File.Exists(PathRepx))
            {
                xtraReport.SaveLayoutToXml(PathRepx);
            }
            xtraReport.LoadLayout(PathRepx);
            ObjectDataSource objectDataSource = new ObjectDataSource();
            objectDataSource.DataSource = JsonToDataTableNew(Formater, requestReport.TableName);
            xtraReport.DataSource = objectDataSource;
            xtraReport.CreateDocument();
            xtraReport.ExportToPdf(PDFFilePath);
            IFileProvider provider = new PhysicalFileProvider(ReportDirectory);
            IFileInfo fileInfo = provider.GetFileInfo(requestReport.ReportTilte + PDFExtension);
            var readStream = fileInfo.CreateReadStream();
            return File(readStream, "application/pdf");
        }

        public DataTable JsonToDataTableNew(string json, string tableName)
        {
            var dt = new DataTable(tableName);
            var root = JObject.Parse(json);
            var items = root[tableName] as JArray;

            if (items == null || items.Count == 0)
            {
                return dt;
            }

            // We know we have at least the first item, we'll use that to create the
            // properties on the DataTable.
            JObject item = items[0] as JObject;
            JProperty jprop;

            // Create the columns
            foreach (var p in item.Properties())
            {
                dt.Columns.Add(new DataColumn(p.Name));
            }

            JToken jtoken;
            JObject obj;

            for (int i = 0; i <= items.Count - 1; i++)
            {
                // Create the new row, put the values into the columns then add the row to the DataTable
                var dr = dt.NewRow();

                // Add each of the columns into a new row then put that new row into the DataTable
                obj = items[i] as JObject;
                jtoken = obj.First;

                while (jtoken != null)
                {
                    jprop = jtoken as JProperty;
                    dr[jprop.Name] = jprop.Value.ToString();
                    jtoken = jtoken.Next;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

    }
}
