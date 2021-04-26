using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Datos;
using Logica;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using Microsoft.Reporting.WinForms;
using System.Windows.Forms;
using System.IO;

namespace ProyectoMagicolor.Reports
{
    public class Reporte : Conexion
    {
        LCliente Cliente = new LCliente();

        public void ExportPDF()
        {
            string deviceInfo = ""; //tamaño de la pagina, default
            string[] streamIds;
            Warning[] warnings;

            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;

            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportPath = @"Reports\rptCliente.rdlc";
            viewer.LocalReport.DataSources.Add(new ReportDataSource("ClientDS", Cliente.Mostrar("V", "")));

            viewer.RefreshReport();
            var bytes = viewer.LocalReport.Render("PDF", deviceInfo, out mimeType, out encoding, out extension, out streamIds, out warnings);

            string fileName = RouteSavePDF("Clients");
            File.WriteAllBytes(fileName, bytes);
            System.Diagnostics.Process.Start(fileName);
        }

        private string RouteSavePDF(string Name)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos de Pdf|*.pdf";
            saveFileDialog.Title = "Guardar archivo";
            saveFileDialog.FileName = Name;
            saveFileDialog.ShowDialog();

            return saveFileDialog.FileName;
        }
    }
}
