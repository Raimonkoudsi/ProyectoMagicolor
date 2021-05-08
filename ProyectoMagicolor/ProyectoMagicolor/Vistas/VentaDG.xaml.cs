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
    public partial class VentaDG : Page
    {
        public LVenta Metodos = new LVenta();


        public MainWindow Parent;

        public VentaDG(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;
        }


        public void Refresh(DateTime? Fecha ,string Nombre)
        {
            List<DVenta> items = Metodos.MostrarVentasGenerales(Fecha, Nombre);

            dgOperaciones.ItemsSource = items;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh(dpFecha.SelectedDate, txtNombre.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ArticuloFrm frmTrab = new ArticuloFrm();
            bool Resp = frmTrab.ShowDialog() ?? false;
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

            reporte.ExportPDF(Metodos.MostrarVentasGenerales(dpFecha.SelectedDate, txtNombre.Text), "VentasGenerales", dpFecha.SelectedDate.Value.ToShortDateString());
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
            var VentaResp = new LVenta().MostrarVenta(id);

            DVenta Venta = VentaResp[0];
            VentaVista Frm = new VentaVista(Venta, this);
            Parent.SwitchScreen(Frm);
        }

        public void GetBack()
        {
            Parent.SwitchScreen(this);
        }
    }
}
