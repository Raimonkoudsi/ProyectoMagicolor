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
using System.Windows.Shapes;

using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    /// <summary>
    /// Interaction logic for FTrabajadores.xaml
    /// </summary>
    public partial class TrabajadoresDG : Page
    {

        public LTrabajador MetodosUsuario = new LTrabajador();

        public TrabajadoresDG()
        {
            InitializeComponent();
        }
        

        public void Refresh(string search)
        {

            List<DTrabajador> items = MetodosUsuario.Mostrar(search);

            List<TablaTrabajadores> tablaTrabajadores = new List<TablaTrabajadores>();

            foreach (DTrabajador it in items)
            {
                tablaTrabajadores.Add(new TablaTrabajadores(it.idTrabajador, it.cedula, it.nombre, it.apellidos, it.direccion, it.telefono, it.email));
            }

            dgOperaciones.ItemsSource = tablaTrabajadores;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //contentsp.Children.Clear();

            Refresh(txtBuscar.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TrabajadoresFrm frmTrab = new TrabajadoresFrm();
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(txtBuscar.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = MetodosUsuario.Encontrar(id);

            TrabajadoresFrm frmTrab = new TrabajadoresFrm();
            frmTrab.Type = TypeForm.Update;
            frmTrab.DataFill = response[0];
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(txtBuscar.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(txtBuscar.Text);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Resp = MessageBox.Show("¿Seguro que quieres eliminar este item?", "Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Resp != MessageBoxResult.Yes)
                return;
            int id = (int)((Button)sender).CommandParameter;
            MetodosUsuario.Eliminar(id);
            Refresh(txtBuscar.Text);
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtBuscar.Text == "")
            {
               txtBucarPlaceH.Text = "";
            }
            
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtBuscar.Text == "")
            {
                txtBucarPlaceH.Text = "Buscar...";
            }
            
        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = MetodosUsuario.Encontrar(id);

            TrabajadoresFrm frmTrab = new TrabajadoresFrm();
            frmTrab.Type = TypeForm.Read;
            frmTrab.DataFill = response[0];
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(txtBuscar.Text);
        }

        private void Reporte_Click(object sender, RoutedEventArgs e)
        {
            if (dgOperaciones.Items.Count == 0)
            {
                LFunction.MessageExecutor("Error", "No se puede realizar un Reporte vacio!");
                return;
            }

            Reports.Reporte reporte = new Reports.Reporte();
            reporte.ExportPDF(MetodosUsuario.MostrarNombre(txtBuscar.Text), "Trabajador");
        }
    }

    public class TablaTrabajadores
    {
        public TablaTrabajadores(int id, string cedula, string nombre, string apellidos, string direccion, string telefono, string email)
        {
            Id = id;
            Cedula = cedula;
            Nombre = nombre;
            Apellidos = apellidos;
            Direccion = direccion;
            Telefono = telefono;
            Email = email;
        }

        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }

}
