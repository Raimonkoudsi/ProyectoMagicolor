using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class ProveedorArticuloDG : Page
    {
        private LProveedor Metodos = new LProveedor();
        private LProveedor Mt = new LProveedor();

        private new MainWindow Parent;

        private int IdProveedor;

        public ProveedorArticuloDG(MainWindow parent = null, int idProveedor = 0)
        {
            InitializeComponent();

            Parent = parent;
            IdProveedor = idProveedor;

            var LCmt = Mt.Mostrar("", "", 1);

            CbProveedor.ItemsSource = LCmt;
            CbProveedor.DisplayMemberPath = "razonSocial";
            CbProveedor.SelectedValuePath = "idProveedor";

            CbCategoria.IsEnabled = false;
            txtNombre.IsEnabled = false;
        }


        public void Refresh()
        {
            if (CbProveedor.SelectedIndex == -1)
                return;

            List<DArticulo> items = Metodos.ListadoArticuloPorProveedor((int)CbProveedor.SelectedValue, (string)CbCategoria.SelectedValue, txtNombre.Text);
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
            txtEmail.Text = "";
            txtTelefono.Text = "";

            Refresh();

            if (IdProveedor != 0)
            {
                CbProveedor.SelectedValue = IdProveedor;

                List<DArticulo> items = Metodos.ListadoArticuloPorProveedor(IdProveedor, (string)CbCategoria.SelectedValue, txtNombre.Text);
                dgOperaciones.ItemsSource = items;

                CbCategoria.IsEnabled = true;
                txtNombre.IsEnabled = true;

                List<DProveedor> DatosProveedor = Mt.Encontrar(IdProveedor);
                txtEmail.Text = DatosProveedor[0].email == "" ? "Sin Correo" : DatosProveedor[0].email;
                txtTelefono.Text = DatosProveedor[0].telefono == "" ? "Sin Teléfono" : DatosProveedor[0].telefono;

                SinRegistro.Visibility = Visibility.Collapsed;
            }
            else
            {
                CbProveedor.SelectedIndex = -1;
                CbCategoria.SelectedIndex = -1;
                txtNombre.IsEnabled = false;

                txtEmail.Text = "";
                txtTelefono.Text = "";

                CbCategoria.IsEnabled = false;
                txtNombre.IsEnabled = false;
            }
        }


        public void GetBack()
        {
            Parent.SwitchScreen(this);
        }

        private void CbCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtNombre.Text = "";

            Refresh();
        }

        private void CbProveedor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();

            CbCategoria.SelectedIndex = -1;
            txtNombre.Text = "";

            if(CbProveedor.SelectedIndex != -1)
            {
                List<DArticulo> LCmt = Mt.CategoriasPorProveedor((int)CbProveedor.SelectedValue);

                CbCategoria.ItemsSource = LCmt;
                CbCategoria.DisplayMemberPath = "categoria";
                CbCategoria.SelectedValuePath = "categoria";

                CbCategoria.IsEnabled = true;
                txtNombre.IsEnabled = true;

                List<DProveedor> DatosProveedor = Mt.Encontrar((int)CbProveedor.SelectedValue);
                txtEmail.Text = DatosProveedor[0].email == "" ? "Sin Correo" : DatosProveedor[0].email;
                txtTelefono.Text = DatosProveedor[0].telefono == "" ? "Sin Teléfono" : DatosProveedor[0].telefono;
            } 
            else
            {
                txtEmail.Text = "";
                txtTelefono.Text = "";

                CbCategoria.IsEnabled = false;
                txtNombre.IsEnabled = false;
            }
        }

        private void txtNombre_KeyUp(object sender, KeyEventArgs e)
        {
            Refresh();
        }
    }
}
