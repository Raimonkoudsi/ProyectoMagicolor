using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class CompraDG : Page
    {
        public LIngreso Metodos = new LIngreso();


        public new MainWindow Parent;

        public CompraDG(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;
        }

        private int metodoPago()
        {
            if ((RBContadoMostrar.IsChecked ?? true) && (RBCreditoMostrar.IsChecked ?? true))
                return 0;
            if (RBContadoMostrar.IsChecked ?? true)
                return 1;
            if (RBCreditoMostrar.IsChecked ?? true)
                return 2;

            return 3;
        }


        public void Refresh(DateTime? Fecha ,string Nombre)
        {
            List<DIngreso> items = Metodos.MostrarComprasGenerales(Fecha, Nombre, metodoPago());
            dgOperaciones.ItemsSource = items;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.ACCESO_SISTEMA != 0)
            {
                btnReport.ToolTip = "Sólo el Administrador puede Generar Reportes";
                btnReport.IsEnabled = false;
            }

            Refresh(dpFecha.SelectedDate, txtNombre.Text);
        }

        private void Reporte_Click(object sender, RoutedEventArgs e)
        {
            if (dgOperaciones.Items.Count == 0)
            {
                LFunction.MessageExecutor("Error", "No se puede realizar un Reporte vacio!");
                return;
            }

            if(dpFecha.SelectedDate.HasValue)
            {
                Reports.Reporte reporte = new Reports.Reporte();
                reporte.ExportPDF(Metodos.MostrarComprasGenerales(dpFecha.SelectedDate, txtNombre.Text, metodoPago()), "ComprasGenerales", dpFecha.SelectedDate.Value.ToString("dd-MM-yyyy"));
            } 
            else
            {
                Reports.Reporte reporte = new Reports.Reporte();
                reporte.ExportPDF(Metodos.MostrarComprasGenerales(DateTime.Today, txtNombre.Text, metodoPago()), "ComprasGenerales", DateTime.Today.ToString("dd-MM-yyyy"));
            }

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Generar",
                "Ha Generado el Reporte de Compras"
            );
            new LAuditoria().Insertar(auditoria);
        }

        private void MetodoPago_Click(object sender, RoutedEventArgs e)
        {
            Refresh(dpFecha.SelectedDate, txtNombre.Text);
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(dpFecha.SelectedDate, txtNombre.Text);
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtNombre.Text == "")
            {
                txtBucarPlaceH.Text = "";
            }

        }

        private void dpFecha_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            if (PlaceFecha.Text != "")
            {
                PlaceFecha.Text = "";
            }

            Refresh(dpFecha.SelectedDate, txtNombre.Text);
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtNombre.Text == "")
            {
                txtBucarPlaceH.Text = "Buscar...";
            }

        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var CompraResp = new LIngreso().MostrarCompra(id);

            DIngreso Compra = CompraResp[0];

            if (Compra.estadoString == "Anulada")
                LFunction.MessageExecutor("Information", "La Compra ha sido Anulada, no posee detalles");
            else
            {
                CompraVista Frm = new CompraVista(Compra, this);
                Parent.SwitchScreen(Frm);
            }
        }

        public void GetBack()
        {
            Parent.SwitchScreen(this);
        }
    }
}
