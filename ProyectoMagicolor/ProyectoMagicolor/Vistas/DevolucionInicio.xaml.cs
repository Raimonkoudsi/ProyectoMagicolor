using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Logica;
using Datos;

namespace ProyectoMagicolor.Vistas
{
    public partial class DevolucionInicio : Page
    {
        public DevolucionInicio(MainWindow parent)
        {
            InitializeComponent();

            this.ParentMenu = parent;
            txtFactura.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
        }

        MainWindow ParentMenu;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txtFactura.Text = "";
            txtFactura.Focus();
        }

        void GetVentas()
        {
            if(txtFactura.Text == "")
            {
                MessageBox.Show("Debe ingresar el número de factura de la venta", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtFactura.Focus();
                return;
            }

            int FrmIdVentas = int.Parse(txtFactura.Text);

            var VentaResp = new LVenta().MostrarVenta(FrmIdVentas);
            

            if(VentaResp.Count > 0)
            {
                DVenta Venta = VentaResp[0];
                int Devuelto = 3;
                if (Venta.estado == Devuelto)
                {
                    MessageBox.Show("La venta que está buscando ya se ha devuelto", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                DevolucionFrm Frm = new DevolucionFrm(Venta, this);
                ParentMenu.SwitchScreen(Frm);
                
            }
            else
            {
                MessageBox.Show("La venta que está buscando no se encuentra", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void GetBack()
        {
            ParentMenu.SwitchScreen(this);
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
