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

            LTrabajador Mt = new LTrabajador();
            var LCmt = Mt.MostrarConAdministrador();

            DTrabajador NCT = new DTrabajador(0, "", "" ,"", DateTime.Now, "", "", "", "", 0, "Todos los Usuarios", "");
            LCmt.Add(NCT);

            CbUsuario.ItemsSource = LCmt;
            CbUsuario.DisplayMemberPath = "usuario";
            CbUsuario.SelectedValuePath = "usuario";
        }

        public MainWindow Parent;

        public void Refresh(DateTime? search, string search2, string search3)
        {
            List<DAuditoria> DisplayData = Metodos.Mostrar(search, search2, search3);

            dgOperaciones.ItemsSource = DisplayData;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh(dpFecha.SelectedDate, CambiarAccion(), CbUsuario.Text);
        }

        private void CbAcciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbAcciones.SelectedIndex > -1)
                PlaceAcciones.Text = "";
            else
                PlaceAcciones.Text = "Acciones";

            Refresh(dpFecha.SelectedDate, CambiarAccion(), CbUsuario.Text);
        }


        private void dpFecha_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            if (dpFecha.Text != "")
            {
                PlaceFecha.Text = "";
            }

            Refresh(dpFecha.SelectedDate, CambiarAccion(), CbUsuario.Text);
        }


        private string CambiarAccion()
        {
             return CbAcciones.SelectedIndex == 0 ? "Ingresar" :
                CbAcciones.SelectedIndex == 1 ? "Registrar" :
                CbAcciones.SelectedIndex == 2 ? "Mostrar" :
                CbAcciones.SelectedIndex == 3 ? "Editar" :
                CbAcciones.SelectedIndex == 4 ? "Eliminar" :
                CbAcciones.SelectedIndex == 5 ? "Generar" :
                CbAcciones.SelectedIndex == 6 ? "Salir" :
                CbAcciones.SelectedIndex == 7 ? "" : "";
        }


        private void CbUsuario_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbUsuario.SelectedIndex > -1)
                PlaceUsuario.Text = "";
            else
                PlaceUsuario.Text = "Usuario del Sistema";

            Refresh(dpFecha.SelectedDate, CambiarAccion(), CbUsuario.SelectedValue.ToString());
        }


    }
}
