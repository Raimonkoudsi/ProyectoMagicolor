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
    public partial class ProveedorDG : Page
    {

        public LProveedor Metodos = new LProveedor();

        public ProveedorDG()
        {
            InitializeComponent();
        }
        

        public void Refresh(string typeSearch, string search)
        {

            List<DProveedor> items = Metodos.Mostrar(typeSearch, search);

            foreach (DProveedor item in items)
            {
                item.numeroDocumento = item.tipoDocumento + "-" + item.numeroDocumento;
            }


            dgOperaciones.ItemsSource = items;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //contentsp.Children.Clear();

            CbTipoDocumento.SelectedIndex = 2;

            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
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
            ProveedorFrm frmTrab = new ProveedorFrm();
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.Encontrar(id);

            ProveedorFrm frm = new ProveedorFrm();
            frm.Type = TypeForm.Update;
            frm.DataFill = response[0];
            bool Resp = frm.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Resp = MessageBox.Show("¿Seguro que quieres eliminrar este item?", "Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Resp != MessageBoxResult.Yes)
                return;
            int id = (int)((Button)sender).CommandParameter;
            DProveedor item = new DProveedor()
            {
                idProveedor = id
            };
            Metodos.Eliminar(item);
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtDocumento.Text == "")
            {
                txtDocumento.Text = "";
            }
            
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtDocumento.Text == "")
            {
                txtDocumento.Text = "Buscar...";
            }
            
        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.Encontrar(id);

            ProveedorFrm frmTrab = new ProveedorFrm();
            frmTrab.Type = TypeForm.Read;
            frmTrab.DataFill = response[0];
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);

        }

        private void CbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbTipoDocumento.SelectedIndex > -1)
                PlaceTipoDocumento.Text = "";
            else
                PlaceTipoDocumento.Text = "Tipo";

            var tipoDoc = CbTipoDocumento.SelectedIndex == 0 ? "V" :
                            CbTipoDocumento.SelectedIndex == 1 ? "E" :
                            CbTipoDocumento.SelectedIndex == 2 ? "J" :
                            CbTipoDocumento.SelectedIndex == 3 ? "G" : "";

            Refresh(tipoDoc, txtDocumento.Text);
        }
    }

}
