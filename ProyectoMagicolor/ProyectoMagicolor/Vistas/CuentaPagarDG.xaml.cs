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
    /// Lógica de interacción para CuentaPagarDG.xaml
    /// </summary>
    public partial class CuentaPagarDG : Page
    {

        public LIngreso Metodos = new LIngreso();

        public CuentaPagarDG(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;


        }

        public List<DRegistro_CuentaPagar> ListaCP = new List<DRegistro_CuentaPagar>();

        public MainWindow Parent;

        public void Refresh(string search)
        {

            List<DIngreso> items = Metodos.MostrarCxP(search);


            dgOperaciones.ItemsSource = items;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh(txtBuscar.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(txtBuscar.Text);
        }

        private void btnAgregarPago_Click(object sender, RoutedEventArgs e)
        {

            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.EncontrarCxP(id);

            //implementar lo de la ListaCP
            DetalleCuentaPagarFrm frmTrab = new DetalleCuentaPagarFrm(this, ListaCP);
            frmTrab.DataFill = response[0];
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(txtBuscar.Text);
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "")
            {
                txtBucarPlaceH.Text = "";
            }

        }
        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "")
            {
                txtBucarPlaceH.Text = "Buscar...";
            }

        }

    }
}
