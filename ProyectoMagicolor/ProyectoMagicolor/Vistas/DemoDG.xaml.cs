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
    public partial class DemoDG : Window
    {

        public LTrabajador MetodosUsuario = new LTrabajador();

        public DemoDG()
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

            Refresh("");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (dgOperaciones.SelectedItems.Count > 0)
            //{
            //    for(int i = 0; i < dgOperaciones.SelectedItems.Count; i++)
            //    {
            //        MessageBox.Show(((TablaTrabajadores)dgOperaciones.SelectedItems[i]).Nombre);
            //    }
            //}
            //else
            //    MessageBox.Show("no hay");
            FormTrabajadores frmTrab = new FormTrabajadores();
            frmTrab.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            MessageBox.Show(btn.Content.ToString() +"("+ ((int)btn.CommandParameter).ToString() + ")");
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(txtBuscar.Text);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            DTrabajador item = new DTrabajador()
            {
                idTrabajador = id
            };
            MetodosUsuario.Eliminar(item);
            Refresh(txtBuscar.Text);
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBuscar.Text = "";
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            txtBuscar.Text = "Buscar...";
        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = MetodosUsuario.Encontrar(id);

            MessageBox.Show(response[0].fechaNacimiento.ToString());
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
