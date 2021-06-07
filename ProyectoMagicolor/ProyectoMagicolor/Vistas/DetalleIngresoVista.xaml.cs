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

    public partial class DetalleIngresoVista : Window
    {

        public LArticulo Metodos = new LArticulo();
        public LIngreso MetIng = new LIngreso();

        public DArticulo Resultado;

        public List<int> Actual;
        public bool filterDG;

        public DetalleIngresoVista(List<int> ActLista, bool Filter)
        {
            InitializeComponent();

            Actual = ActLista;
            filterDG = Filter;
        }
        

        public void Refresh(string search)
        {

            List<DArticulo> items = MetIng.MostrarStockNombre(search);
            if (filterDG)
            {
                foreach (int item in Actual)
                {
                    var Res = items.FindAll((art) => art.idArticulo == item);

                    foreach (DArticulo j in Res)
                    {
                        var id = items.FindIndex((articulo) => articulo.idArticulo == j.idArticulo);


                        if (id > -1)
                            items.RemoveAt(id);
                    }
                }
            }

            dgOperaciones.ItemsSource = items;


            if (items.Count == 0)
            {
                SinRegistro.Visibility = Visibility.Visible;
            }
            else
            {
                SinRegistro.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh(txtNombre.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var resp = new LArticulo().SacarArticulo(id)[0];
            if(resp.cantidadActual == 0)
            {
                MessageBox.Show("El Producto que has seleccionado tiene un stock de 0!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var response = Metodos.Encontrar(id);

            if(response.Count > 0)
            {
                DialogResult = true;
                Resultado = response[0];
                this.Close();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(txtNombre.Text);
        }
    }

}
