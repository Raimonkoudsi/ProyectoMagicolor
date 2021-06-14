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

        public new MainWindow Parent;

        public void Refresh(DateTime? search, DateTime? search2, string search3, string search4)
        {
            List<DAuditoria> DisplayData = Metodos.Mostrar(search, search2, search3, search4);

            dgOperaciones.ItemsSource = DisplayData;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dpHasta.IsEnabled = false;
            CbAcciones.IsEnabled = false;
            CbUsuario.IsEnabled = false;

            dpDesde.Text = "";
            dpHasta.Text = "";
            CbAcciones.Text = "";
            CbUsuario.Text = "";

            dpDesde.Focus();
        }


        private void dpDesde_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            if (dpHasta.SelectedDate != null)
            {
                Refresh(dpDesde.SelectedDate, dpHasta.SelectedDate, CambiarAccion(), CbUsuario.Text);
                return;
            }

            dpHasta.IsEnabled = true;
            dpHasta.Focus();


            dpHasta.DisplayDateStart = dpDesde.SelectedDate.Value.AddDays(-1);
        }


        private void dpHasta_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            Refresh(dpDesde.SelectedDate, dpHasta.SelectedDate, CambiarAccion(), CbUsuario.Text);

            CbAcciones.IsEnabled = true;
            CbUsuario.IsEnabled = true;
            CbAcciones.Focus();

            dpDesde.DisplayDateEnd = dpHasta.SelectedDate.Value.AddDays(1);
        }


        private void CbAcciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh(dpDesde.SelectedDate, dpHasta.SelectedDate, CambiarAccion(), CbUsuario.Text);
            CbUsuario.Focus();
        }

        private void CbUsuario_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh(dpDesde.SelectedDate, dpHasta.SelectedDate, CambiarAccion(), CbUsuario.SelectedValue.ToString());
        }


        private string CambiarAccion()
        {
             return CbAcciones.SelectedIndex == 0 ? "Ingresar" :
                CbAcciones.SelectedIndex == 1 ? "Registrar" :
                CbAcciones.SelectedIndex == 2 ? "Mostrar" :
                CbAcciones.SelectedIndex == 3 ? "Editar" :
                CbAcciones.SelectedIndex == 4 ? "Deshabilitar" :
                CbAcciones.SelectedIndex == 5 ? "Generar" :
                CbAcciones.SelectedIndex == 6 ? "Salir" :
                CbAcciones.SelectedIndex == 7 ? "" : "";
        }


    }
}
