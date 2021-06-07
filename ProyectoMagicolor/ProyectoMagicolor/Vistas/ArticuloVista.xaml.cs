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
    public partial class ArticuloVista : Window
    {

        public LIngreso Metodos = new LIngreso();

        public LArticulo MetodosArticulos = new LArticulo();

        public DArticulo Resultado;

        public List<int> Actual;
        public bool filterDG;

        public ArticuloVista(List<int> ActLista, bool Filter)
        {
            InitializeComponent();

            Actual = ActLista;
            filterDG = Filter;
        }
        

        public void Refresh()
        {

            List<DArticulo> items = new LArticulo().MostrarConCategoria(txtNombre.Text);
            if (filterDG)
            {
                foreach (int item in Actual)
                {
                    var id = items.FindIndex((articulo) => articulo.idArticulo == item);

                    items.RemoveAt(id);
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
            Refresh();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = MetodosArticulos.Encontrar(id);

            if(response.Count > 0)
            {
                DialogResult = true;
                Resultado = response[0];
                this.Close();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh();
        }
    }

}
