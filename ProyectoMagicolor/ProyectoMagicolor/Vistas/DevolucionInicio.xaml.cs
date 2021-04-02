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

using Logica;
using Datos;

namespace ProyectoMagicolor.Vistas
{
    /// <summary>
    /// Interaction logic for DevolucionInicio.xaml
    /// </summary>
    public partial class DevolucionInicio : Page
    {
        public DevolucionInicio(MainWindow parent)
        {
            InitializeComponent();

            this.ParentMenu = parent;
            txtFactura.txt.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
        }

        MainWindow ParentMenu;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txtFactura.txt.Focus();
        }

        void GetVentas()
        {
            if(!txtFactura.Changed)
            {
                MessageBox.Show("Debes llenar el campo Numero de Factura!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtFactura.txt.Focus();
                return;
            }

            int FrmIdVentas = int.Parse(txtFactura.txt.Text);

            var VentaResp = new LVenta().MostrarVenta(FrmIdVentas);
            

            if(VentaResp.Count > 0)
            {
                DVenta Venta = VentaResp[0];
                int Devuelto = 3;
                if (Venta.estado == Devuelto)
                {
                    MessageBox.Show("La venta que está buscando ya se ha devuelto!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                DevolucionFrm Frm = new DevolucionFrm(Venta, this);
                ParentMenu.SwitchScreen(Frm);
                
            }
            else
            {
                MessageBox.Show("La venta que está buscando no se encuentra!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void GetBack()
        {
            ParentMenu.SwitchScreen(this);
        }

        public void Limpiar()
        {
            txtFactura.SetText("");
        }

        private void StackPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                GetVentas();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetVentas();
        }
    }
}
