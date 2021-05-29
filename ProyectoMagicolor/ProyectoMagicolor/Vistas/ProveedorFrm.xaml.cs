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

        private string tipoDocumento = "";
        private string documento = "";

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

                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Mostrar",
                    "Ha Visto al Proveedor " + tipoDocumento + "-" + documento
                 );
                new LAuditoria().Insertar(auditoria);
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

            string razonsocial = txtRazonSocial.Text;
            string sectorcomercial = (string)CbSectorComercial.SelectedValue;
            tipoDocumento = CbTipoDocumento.Text;
            documento = txtDocumento.Text;
            string direccion = txtDireccion.Text;
            string telefono = txtTelefono.Text;
            string email = txtEmail.Text;
            string url = txtUrl.Text;

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

            if (response == "OK")
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Insertar",
                    "Ha Registrado al Proveedor " + tipoDocumento + "-" + documento
                );
                new LAuditoria().Insertar(auditoria);

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

            if(response == "OK")
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Editar",
                    "Ha Editado al Proveedor " + tipoDocumento + "-" + documento
                );
                new LAuditoria().Insertar(auditoria);

                this.DialogResult = true;
                this.Close();
            }
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
                txtRazonSocial.Text = Data.razonSocial;
                CbSectorComercial.SelectedValue = Data.sectorComercial;
                CbTipoDocumento.SelectedIndex = Data.tipoDocumento == "V" ? 0 :
                                                Data.tipoDocumento == "E" ? 1 : 
                                                Data.tipoDocumento == "J" ? 2 :
                                                Data.tipoDocumento == "G" ? 3 : -1;
                txtDocumento.Text = Data.numeroDocumento;
                txtTelefono.Text = Data.telefono;
                txtEmail.Text = Data.email;
                txtDireccion.Text = Data.direccion;
                txtUrl.Text = Data.url;
            }
        }
        #region Validation
        bool Validate()
        {
            if (txtRazonSocial.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar la razón social");
                txtRazonSocial.Focus();
                return true;
            }
            if (txtRazonSocial.Text.Length <= 4)
            {
                LFunction.MessageExecutor("Error", "La razón social debe ser mayor a 4 carácteres");
                txtRazonSocial.Focus();
                return true;
            }
            if (CbSectorComercial.SelectedIndex == -1)
            {
                LFunction.MessageExecutor("Error", "Debe seleccionar un sector comercial");
                CbSectorComercial.Focus();
                return true;
            }
            if (CbTipoDocumento.SelectedIndex == -1)
            {
                LFunction.MessageExecutor("Error", "Debe seleccionar un tipo de documento");
                CbTipoDocumento.Focus();
                return true;
            }
            if (txtDocumento.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar el campo cocumento");
                txtDocumento.Focus();
                return true;
            }
            if(txtTelefono.Text != "" && txtTelefono.Text.Contains(" ") )
            {
                LFunction.MessageExecutor("Error", "El campo de telefono no debe contener espacios en blanco");
                txtTelefono.Focus();
                return true;
            }
            if (txtEmail.Text != "" && !Validaciones.IsValidEmail(txtEmail.Text))
            {
                LFunction.MessageExecutor("Error", "El correo electronico no contiene el formato correcto");
                txtEmail.Focus();
                return true;
            }
            if(txtUrl.Text != "" && !Validaciones.ValidHttpURL(txtUrl.Text))
            {
                LFunction.MessageExecutor("Error", "La dirección de la página web no contiene el formato correcto");
                txtEmail.Focus();
                return true;
            }
            if ((txtTelefono.Text.Length <= 10 && txtTelefono.Text != "") || txtTelefono.Text.Length >= 14)
            {
                LFunction.MessageExecutor("Error", "El campo del teléfono debe ser válido");
                txtTelefono.Focus();
                return true;
            }
            if (txtDocumento.Text.Length <= 6 || txtDocumento.Text.Length >= 9)
            {
                LFunction.MessageExecutor("Error", "El campo del documento debe ser válido");
                txtDocumento.Focus();
                return true;
            }

            if(txtTelefono.Text == "" && txtEmail.Text == "") 
            {
                LFunction.MessageExecutor("Error", "Debe proporcionar un teléfono o correo electrónico del proveedor");
                txtTelefono.Focus();
                return true;
            }

            if (Type != TypeForm.Update)
            {
                if (Metodos.CedulaRepetida(CbTipoDocumento.Text + "-" + txtDocumento.Text))
                {
                    LFunction.MessageExecutor("Error", "El documento ya está registrado en el sistema");
                    txtDocumento.Focus();
                    return true;
                }
            }

            return false;
        }

        #endregion


    }
}
