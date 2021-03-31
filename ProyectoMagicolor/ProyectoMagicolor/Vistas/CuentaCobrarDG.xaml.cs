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
    /// <summary>
    /// Lógica de interacción para CuentaCobrarDG.xaml
    /// </summary>
    public partial class CuentaCobrarDG : Page
    {

        public LVenta Metodos = new LVenta();

        public CuentaCobrarDG(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;
        }


        public List<DRegistro_CuentaCobrar> ListaCC = new List<DRegistro_CuentaCobrar>();

        public MainWindow Parent;

        public void Refresh(string search, string search2)
        {

            List<DVenta> DisplayData = Metodos.MostrarCxC(search, search2);

            //dgOperaciones.ItemsSource = null;
            dgOperaciones.ItemsSource = DisplayData;

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void btnAgregarPago_Click(object sender, RoutedEventArgs e)
        {

            int idIngreso = (int)((Button)sender).CommandParameter;
            var response = Metodos.EncontrarCxC(idIngreso);

            //implementar lo de la ListaCP
            DetalleCuentaCobrarFrm frmDetalleCC = new DetalleCuentaCobrarFrm(this, ListaCC);
            frmDetalleCC.DataFill = response[0];
            bool Resp = frmDetalleCC.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtDocumento.Text == "")
            {
                txtBucarPlaceH.Text = "";
            }

        }
        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDocumento.Text == "")
            {
                txtBucarPlaceH.Text = "Buscar...";
            }

        }

        private void CbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbTipoDocumento.SelectedIndex > -1)
                PlaceTipoDocumento.Text = "";
            else
                PlaceTipoDocumento.Text = "Tipo";

            var tipoDoc = CbTipoDocumento.SelectedIndex == 0 ? "V" :
                            CbTipoDocumento.SelectedIndex == 1 ? "E" :
                            CbTipoDocumento.SelectedIndex == 2 ? "J" :
                            CbTipoDocumento.SelectedIndex == 3 ? "G" : "";

            Refresh(tipoDoc, txtDocumento.Text);
        }
    }
}
