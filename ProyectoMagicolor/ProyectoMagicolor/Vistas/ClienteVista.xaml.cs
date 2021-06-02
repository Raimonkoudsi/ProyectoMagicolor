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
    public partial class ClienteVista : Window
    {
        public VentaFrm ParentForm;

        public LCliente Metodos = new LCliente();

        public ClienteVista(VentaFrm parent)
        {
            InitializeComponent();
            ParentForm = parent;
        }
        

        public void Refresh(string typeSearch, string search)
        {
            List<DCliente> items = Metodos.Mostrar(typeSearch, search, 1);

            dgOperaciones.ItemsSource = items;

            if (items.Count == 0)
                SinRegistro.Visibility = Visibility.Visible;
            else
                SinRegistro.Visibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CbTipoDocumento.SelectedIndex = 0;

            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.Encontrar(id)[0];

            DialogResult = true;
            ParentForm.AgregarCliente(response);
            this.Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void CbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tipoDoc = CbTipoDocumento.SelectedIndex == 0 ? "V" :
                            CbTipoDocumento.SelectedIndex == 1 ? "E" :
                            CbTipoDocumento.SelectedIndex == 2 ? "J" :
                            CbTipoDocumento.SelectedIndex == 3 ? "G" : "";

            Refresh(tipoDoc, txtDocumento.Text);
        }
    }

}
