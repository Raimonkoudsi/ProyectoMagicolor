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
    public partial class ArticuloVista : Window
    {

        public LArticulo Metodos = new LArticulo();

        public DArticulo Resultado;

        public List<int> Actual;
        public bool filterDG;

        public ArticuloVista(List<int> ActLista, bool Filter)
        {
            InitializeComponent();

            Actual = ActLista;
            filterDG = Filter;
        }
        

        public void Refresh(string search)
        {

            List<DArticulo> items = Metodos.Mostrar(search);
            if (filterDG)
            {
                foreach (int item in Actual)
                {
                    var id = items.FindIndex((articulo) => articulo.idArticulo == item);

                    items.RemoveAt(id);
                }
            }


            dgOperaciones.ItemsSource = items;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //contentsp.Children.Clear();

            Refresh(txtBuscar.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
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
    }

}
