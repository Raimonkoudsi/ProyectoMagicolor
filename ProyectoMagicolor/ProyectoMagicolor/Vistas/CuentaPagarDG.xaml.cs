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
    public partial class CuentaPagarDG : Page
    {

        public LCuentaPagar Metodos = new LCuentaPagar();

        public List<DRegistro_CuentaPagar> ListaCP = new List<DRegistro_CuentaPagar>();

        public new MainWindow Parent;

        private int TipoFecha;
        private string tipoDoc;

        public CuentaPagarDG(MainWindow parent = null, int tipoFecha = 0)
        {
            InitializeComponent();

            Parent = parent;
            TipoFecha = tipoFecha;

        }

        public void Refresh(string search, string search2)
        {

            List<DIngreso> DisplayData = Metodos.MostrarCxP(search, search2, TipoFechaBusqueda());
            dgOperaciones.ItemsSource = DisplayData;

            if (DisplayData.Count == 0)
            {
                SinRegistro.Visibility = Visibility.Visible;
            }
            else
            {
                SinRegistro.Visibility = Visibility.Collapsed;
            }
        }

        private int TipoFechaBusqueda()
        {
            if (RBFecha.IsChecked == true && RBFechaLimite.IsChecked == true)
                return 3;
            if (RBFecha.IsChecked == true && RBFechaLimite.IsChecked == false)
                return 1;
            if (RBFecha.IsChecked == false && RBFechaLimite.IsChecked == true)
                return 2;

            return 0;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (TipoFecha == 1)
            {
                RBFecha.IsChecked = true;
                RBFechaLimite.IsChecked = false;
            }
            if (TipoFecha == 2)
            {
                RBFecha.IsChecked = false;
                RBFechaLimite.IsChecked = true;
            }

            CbTipoDocumento.SelectedIndex = 4;

            Refresh(tipoDoc, txtDocumento.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(tipoDoc, txtDocumento.Text);
        }

        private void btnAgregarPago_Click(object sender, RoutedEventArgs e)
        {

            int idIngreso = (int)((Button)sender).CommandParameter;
            var response = Metodos.EncontrarCxP(idIngreso);

            DetalleCuentaPagarFrm frmDetalleCP = new DetalleCuentaPagarFrm(this, ListaCP);
            frmDetalleCP.DataFill = response[0];
            bool Resp = frmDetalleCP.ShowDialog() ?? false;
            Refresh(tipoDoc, txtDocumento.Text);
        }

        private void CbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            tipoDoc = CbTipoDocumento.SelectedIndex == 0 ? "V" :
                            CbTipoDocumento.SelectedIndex == 1 ? "E" :
                            CbTipoDocumento.SelectedIndex == 2 ? "J" :
                            CbTipoDocumento.SelectedIndex == 3 ? "G" : "";

            if (CbTipoDocumento.SelectedIndex == 4)
                txtDocumento.IsEnabled = false;
            else
                txtDocumento.IsEnabled = true;

            Refresh(tipoDoc, txtDocumento.Text);
        }

        private void RBFecha_Click(object sender, RoutedEventArgs e)
        {
            Refresh(tipoDoc, txtDocumento.Text);
        }

    }
}
