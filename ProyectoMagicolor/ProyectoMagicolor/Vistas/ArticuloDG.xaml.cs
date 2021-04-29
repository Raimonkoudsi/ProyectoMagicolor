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
    public partial class ArticuloDG : Page
    {

        public LArticulo Metodos = new LArticulo();

        public ArticuloDG()
        {
            InitializeComponent();
        }
        

        public void Refresh(string search)
        {

            List<DArticulo> items = Metodos.Mostrar(search);


            dgOperaciones.ItemsSource = items;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //contentsp.Children.Clear();

            Refresh(txtBuscar.Text);
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
            ArticuloFrm frmTrab = new ArticuloFrm();
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(txtBuscar.Text);
        }

        private void Reporte_Click(object sender, RoutedEventArgs e)
        {
            Reports.Reporte reporte = new Reports.Reporte();

            reporte.ExportPDF(Metodos.MostrarConCategoria(txtBuscar.Text), "Articulo");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.Encontrar(id);

            ArticuloFrm frm = new ArticuloFrm();
            frm.Type = TypeForm.Update;
            frm.DataFill = response[0];
            bool Resp = frm.ShowDialog() ?? false;
            Refresh(txtBuscar.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(txtBuscar.Text);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Resp = MessageBox.Show("¿Seguro que quieres eliminar este item?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Resp != MessageBoxResult.Yes)
                return;
            int id = (int)((Button)sender).CommandParameter;
            Metodos.Eliminar(id);
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
            var response = Metodos.Encontrar(id);

            ArticuloFrm frmTrab = new ArticuloFrm();
            frmTrab.Type = TypeForm.Read;
            frmTrab.DataFill = response[0];
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(txtBuscar.Text);

            //MessageBox.Show(response[0].fechaNacimiento.ToString());
        }

    }

}
