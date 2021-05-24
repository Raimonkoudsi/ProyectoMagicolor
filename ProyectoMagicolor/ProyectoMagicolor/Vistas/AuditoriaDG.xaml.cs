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
    public partial class AuditoriaDG : Page
    {

        public LAuditoria Metodos = new LAuditoria();

        public AuditoriaDG(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;
        }

        public MainWindow Parent;

        public void Refresh(DateTime? search, string search2, string search3)
        {
            List<DAuditoria> DisplayData = Metodos.Mostrar(search, search2, search3);

            dgOperaciones.ItemsSource = DisplayData;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh(dpFecha.SelectedDate, CambiarAccion(), txtUsuario.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(dpFecha.SelectedDate, CambiarAccion(), txtUsuario.Text);
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsuario.Text == "")
            {
                txtBuscarPlaceUsuario.Text = "";
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsuario.Text == "")
            {
                txtBuscarPlaceUsuario.Text = "Ingresar Usuario ...";
            }
        }

        private void CbAcciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbAcciones.SelectedIndex > -1)
                PlaceAcciones.Text = "";
            else
                PlaceAcciones.Text = "Acciones";

            Refresh(dpFecha.SelectedDate, CambiarAccion(), txtUsuario.Text);
        }


        private void dpFecha_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            if (PlaceFecha.Text != "")
            {
                PlaceFecha.Text = "";
            }

            Refresh(dpFecha.SelectedDate, CambiarAccion(), txtUsuario.Text);
        }


        private string CambiarAccion()
        {
             return CbAcciones.SelectedIndex == 0 ? "Ingresar" :
                CbAcciones.SelectedIndex == 1 ? "Registrar" :
                CbAcciones.SelectedIndex == 2 ? "Mostrar" :
                CbAcciones.SelectedIndex == 3 ? "Editar" :
                CbAcciones.SelectedIndex == 4 ? "Eliminar" :
                CbAcciones.SelectedIndex == 5 ? "Generar" :
                CbAcciones.SelectedIndex == 6 ? "Salir" : "";
        }
    }
}
