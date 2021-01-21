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
    /// <summary>
    /// Interaction logic for CompraFrm.xaml
    /// </summary>
    public partial class CompraFrm : Page
    {
        public CompraFrm()
        {
            InitializeComponent();
        }

        public DProveedor Proveedor;

        private void CbMetodoPago_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbMetodoPago.SelectedIndex > -1)
                PlaceMetodoPago.Text = "";
            else
                PlaceMetodoPago.Text = "Método de pago";
        }

        private void CbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbTipoDocumento.SelectedIndex > -1)
                PlaceTipoDocumento.Text = "";
            else
                PlaceTipoDocumento.Text = "Tipo";
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtDocumento.Text == "")
            {
                PlaceDocumento.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDocumento.Text == "")
            {
                PlaceDocumento.Text = "Buscar...";
            }
        }

        private void txtDocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                MessageBox.Show("Has presionado enter");
            }
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "")
            {
                txtBucarPlaceH.Text = "";
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "")
            {
                PlaceDocumento.Text = "Codigo de Articulo";
            }
        }

        public bool ProveedorSetted = false;

        public void AgregarProveedor(DProveedor proveedor)
        {
            ProveedorSetted = true;

            Proveedor = proveedor;

            ProveedorBuscar.Visibility = Visibility.Collapsed;
            txtProvName.Text = proveedor.razonSocial;
            txtProvName.Visibility = Visibility.Visible;
            TxtProvDoc.Text = proveedor.tipoDocumento + "-" + proveedor.numeroDocumento;
            TxtProvDir.Text = proveedor.direccion;
            TxtProvTelf.Text = proveedor.telefono;
            TxtProvEmail.Text = proveedor.email;
            ProveedorDatos.Visibility = Visibility.Visible;


            BtnProveedor.Background = Brushes.ForestGreen;
            BtnProveedor.BorderBrush = Brushes.ForestGreen;
            BtnProveedor.Content = "Cambiar";
        }

        public void QuitarProveedor()
        {
            ProveedorSetted = false;

            Proveedor = null;

            ProveedorBuscar.Visibility = Visibility.Visible;
            txtProvName.Visibility = Visibility.Collapsed;
            ProveedorDatos.Visibility = Visibility.Collapsed;


            BtnProveedor.ClearValue(Button.BackgroundProperty);
            BtnProveedor.ClearValue(Button.BorderBrushProperty);
            BtnProveedor.Content = "Encontrar";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!ProveedorSetted)
            {
                if(CbTipoDocumento.SelectedIndex > -1 && txtDocumento.Text != "")
                {
                    LProveedor Metodo = new LProveedor();
                    var response = Metodo.EncontrarConDocumento(CbTipoDocumento.Text, txtDocumento.Text);

                    if(response.Count > 0)
                    {
                        AgregarProveedor(response[0]);
                        return;
                    }

                }
                else
                {
                    ProveedorVista PVFrm = new ProveedorVista(this);

                    PVFrm.ShowDialog();
                }
            }
            else
            {
                QuitarProveedor();
            }

            
        }
    }
}
