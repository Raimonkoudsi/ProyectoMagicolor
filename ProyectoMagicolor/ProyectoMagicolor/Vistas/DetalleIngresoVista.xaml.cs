using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Datos;
using Logica;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

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
                MessageBox.Show("El Producto que has seleccionado tiene un stock de 0", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
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






    public class DesactivateButtonNoStock : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            int cantidadActual = int.Parse(values[0].ToString());

            if (cantidadActual == 0)
                return false;
            if (cantidadActual > 0)
                return true;

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class ChangeRedColorRowNoStock : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Transparent;

            int cantidadActual = int.Parse(value.ToString());

            if (cantidadActual == 0)
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
            }
            else
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("Black"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
