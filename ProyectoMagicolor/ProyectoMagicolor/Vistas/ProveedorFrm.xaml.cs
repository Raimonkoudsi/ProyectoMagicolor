using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for FormTrabajadores.xaml
    /// </summary>
    public partial class ProveedorFrm : Window
    {


        public ProveedorFrm()
        {
            InitializeComponent();
            txtDocumento.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtTelefono.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);

            LCategoria Mt = new LCategoria();

            var LCmt = Mt.Mostrar("");

            DCategoria NCT = new DCategoria(0,"Variedades", "");
            LCmt.Add(NCT);
            
            CbSectorComercial.ItemsSource = LCmt;
            CbSectorComercial.DisplayMemberPath = "nombre";
            CbSectorComercial.SelectedValuePath = "nombre";
        }

        

        public TypeForm Type;
        public DProveedor DataFill;

        public DProveedor UForm;

        public LProveedor Metodos = new LProveedor();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Type == TypeForm.Update)
                Update();
            else
                Create();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(Type == TypeForm.Read)
            {
                txtTitulo.Text = "Leer Proveedor";
                fillForm(DataFill);
                SetEnable(false);
                btnEnviar.Visibility = Visibility.Collapsed;
            }
            else if(Type == TypeForm.Update)
            {
                txtTitulo.Text = "Editar Proveedor";
                fillForm(DataFill);
            }
        }

       

        void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            string razonsocial = txtRazonSocial.txt.Text;
            string sectorcomercial = (string)CbSectorComercial.SelectedValue;
            string tipoDocumento = CbTipoDocumento.Text;
            string documento = txtDocumento.txt.Text;
            string direccion = txtDireccion.Text;
            string telefono = txtTelefono.Changed ? txtTelefono.txt.Text : "";
            string email = txtEmail.Changed ? txtEmail.txt.Text : "";
            string url = txtUrl.Changed ? txtUrl.txt.Text : "";

            UForm = new DProveedor(0,
                                   razonsocial,
                                   sectorcomercial,
                                   tipoDocumento,
                                   documento,
                                   direccion,
                                   telefono,
                                   email,
                                   url); //Datos
        }

        void Create()
        {
            fillData();
            if (UForm == null)
                return;
            string response = Metodos.Insertar(UForm);
            MessageBox.Show(response);
            if (response == "OK")
            {
                this.DialogResult = true;
                this.Close();
            }

        }

        void Update()
        {
            fillData();
            if (UForm == null)
                return;
            UForm.idProveedor = DataFill.idProveedor;
            string response = Metodos.Editar(UForm);
            MessageBox.Show(response);
            if(response == "OK")
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void PlaceDescripcion_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtDireccion.Text == "")
            {
                PlaceDireccion.Text = "";
            }
        }

        private void PlaceDescripcion_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDireccion.Text == "")
            {
                PlaceDireccion.Text = "Dirección";
            }
        }


        private void CbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbTipoDocumento.SelectedIndex > -1)
                PlaceTipoDocumento.Text = "";
            else
                PlaceTipoDocumento.Text = "Tipo";
        }
        private void CbSectorComercial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbSectorComercial.SelectedIndex > -1)
                PlaceSectorComercial.Text = "";
            else
                PlaceSectorComercial.Text = "Sector Comercial";
        }

        void SetEnable(bool Enable)
        {
            txtRazonSocial.IsEnabled = Enable;
            CbSectorComercial.IsEnabled = Enable;
            CbTipoDocumento.IsEnabled = Enable;
            txtDocumento.IsEnabled = Enable;
            txtDireccion.IsEnabled = Enable;
            txtTelefono.IsEnabled = Enable;
            txtEmail.IsEnabled = Enable;
            txtUrl.IsEnabled = Enable;
        }
        void fillForm(DProveedor Data)
        {
            if(Data != null)
            {
                txtRazonSocial.SetText(Data.razonSocial);
                CbSectorComercial.SelectedValue = Data.sectorComercial;
                CbTipoDocumento.SelectedIndex = Data.tipoDocumento == "V" ? 0 :
                                                Data.tipoDocumento == "E" ? 1 : 
                                                Data.tipoDocumento == "J" ? 2 :
                                                Data.tipoDocumento == "G" ? 3 : -1;
                txtDocumento.SetText(Data.numeroDocumento);
                txtTelefono.SetText(Data.telefono);
                txtEmail.SetText(Data.email);
                txtDireccion.Text = Data.direccion;
                PlaceDireccion.Text = Data.direccion != "" ? "" : "Dirección";
                txtUrl.SetText(Data.url);
            }
        }
        #region Validation
        bool Validate()
        {
            if (!txtRazonSocial.Changed)
            {
                MessageBox.Show("Debes llenar el campo Razón Social!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtRazonSocial.txt.Focus();
                return true;
            }
            if (CbSectorComercial.SelectedIndex == -1)
            {
                MessageBox.Show("Debes seleccionar un Sector Comercial!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                CbSectorComercial.Focus();
                return true;
            }
            if (CbTipoDocumento.SelectedIndex == -1)
            {
                MessageBox.Show("Debes seleccionar un Tipo de Documento!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                CbTipoDocumento.Focus();
                return true;
            }
            if (!txtDocumento.Changed)
            {
                MessageBox.Show("Debes llenar el campo Documento!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtDocumento.txt.Focus();
                return true;
            }
            if(txtTelefono.Changed && txtTelefono.txt.Text.Contains(" ") )
            {
                MessageBox.Show("El campo de telefono no debe tener espacios!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtTelefono.txt.Focus();
                return true;
            }
            if (txtEmail.Changed && !Validaciones.IsValidEmail(txtEmail.txt.Text))
            {
                MessageBox.Show("El correo electronico es incorrecto!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtEmail.txt.Focus();
                return true;
            }
            if(txtUrl.Changed && !Validaciones.ValidHttpURL(txtUrl.txt.Text))
            {
                MessageBox.Show("El URl es Incorrecto!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtEmail.txt.Focus();
                return true;
            }

            return false;
        }

        #endregion


    }
}
