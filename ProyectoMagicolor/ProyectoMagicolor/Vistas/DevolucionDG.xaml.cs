using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class DevolucionDG : Page
    {
        public LDevolucion Metodos = new LDevolucion();

        public new MainWindow Parent;

        public DevolucionDG(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;
        }


        private void MetodoPago_Click(object sender, RoutedEventArgs e)
        {
            Refresh(dpFecha.SelectedDate, txtNombre.Text);
        }

        public void Refresh(DateTime? Fecha, string Nombre)
        {
            List<DDevolucion> items = Metodos.MostrarDevolucionesGenerales(Fecha, Nombre);

            dgOperaciones.ItemsSource = items;

            if (items.Count == 0)
            {
                btnReport.IsEnabled = false;
                SinRegistro.Visibility = Visibility.Visible;
            }
            else
            {
                SinRegistro.Visibility = Visibility.Collapsed;
            }

            if (Globals.ACCESO_SISTEMA == 0 && items.Count != 0)
            {
                btnReport.IsEnabled = true;
            }
            else if (Globals.ACCESO_SISTEMA != 0)
            {
                btnReport.IsEnabled = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtNombre.Text = "";
            txtNombre.Focus();

            dpFecha.DisplayDateEnd = DateTime.Today;

            Refresh(dpFecha.SelectedDate, txtNombre.Text);
        }

        private void Reporte_Click(object sender, RoutedEventArgs e)
        {
            if (dgOperaciones.Items.Count == 0)
            {
                LFunction.MessageExecutor("Error", "No se puede realizar un Reporte vacio!");
                return;
            }

            Reports.Reporte reporte = new Reports.Reporte();
            reporte.ExportPDF(Metodos.MostrarDevolucionesGenerales(dpFecha.SelectedDate, txtNombre.Text), "DevolucionesGenerales", dpFecha.SelectedDate.Value.ToString("dd-MM-yyyy"));

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Generar",
                "Ha Generado un Listado de Devoluciones"
            );
            new LAuditoria().Insertar(auditoria);
        }



        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(dpFecha.SelectedDate, txtNombre.Text);
        }

        private void dpFecha_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            Refresh(dpFecha.SelectedDate, txtNombre.Text);
        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var DevolucionResp = new LDevolucion().MostrarDevolucion(id);

            DDevolucion Devolucion = DevolucionResp[0];
            DevolucionVista Frm = new DevolucionVista(Devolucion, this);

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Mostrar",
                "Ha Visto el Detalle de la Venta N° " + DevolucionResp[0].idVenta
            );
            new LAuditoria().Insertar(auditoria);

            Parent.SwitchScreen(Frm);
        }

        public void GetBack()
        {
            Parent.SwitchScreen(this);
        }
    }
}
