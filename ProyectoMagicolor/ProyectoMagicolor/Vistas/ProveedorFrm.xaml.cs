using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class ProveedorFrm : Window
    {
        public TypeForm Type;
        public DProveedor DataFill;

        public DProveedor UForm;

        public LProveedor Metodos = new LProveedor();

        private string tipoDocumento = "";
        private string documento = "";

        public CompraFrm ParentFrm;
        public string tipoDocumentoCompra = "";
        public string documentoCompra = "";


        public ProveedorFrm(CompraFrm parent = null, string tipoDocumento = "", string numeroDocumento = "")
        {
            InitializeComponent();

            ParentFrm = parent;
            tipoDocumentoCompra = tipoDocumento;
            documentoCompra = numeroDocumento;

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
                btnEnviar.Content = "Sólo Lectura";

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
            if (ParentFrm != null)
            {
                CbTipoDocumento.Text = tipoDocumentoCompra;
                txtDocumento.Text = documentoCompra;

                CbTipoDocumento.IsEnabled = false;
                txtDocumento.IsEnabled = false;

                txtRazonSocial.Focus();
            }
        }

       

        private void fillData()
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

        private void Create()
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

                if (ParentFrm != null)
                    ParentFrm.SetNuevoProveedor(UForm.tipoDocumento, UForm.numeroDocumento);

                this.DialogResult = true;
                this.Close();
            }

        }

        private void Update()
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

                if (ParentFrm != null)
                    ParentFrm.setProveedor();

                this.DialogResult = true;
                this.Close();
            }
        }

        private void SetEnable(bool Enable)
        {
            txtRazonSocial.IsEnabled = Enable;
            CbSectorComercial.IsEnabled = Enable;
            CbTipoDocumento.IsEnabled = false;
            txtDocumento.IsEnabled = false;
            txtDireccion.IsEnabled = Enable;
            txtTelefono.IsEnabled = Enable;
            txtEmail.IsEnabled = Enable;
            txtUrl.IsEnabled = Enable;
            btnEnviar.IsEnabled = Enable;
        }

        private void fillForm(DProveedor Data)
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
        private bool Validate()
        {
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
            if (txtDocumento.Text.Length <= 6 || txtDocumento.Text.Length >= 11)
            {
                LFunction.MessageExecutor("Error", "El campo del documento debe ser válido");
                txtDocumento.Focus();
                return true;
            }
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
            if (txtTelefono.Text != "" && txtTelefono.Text.Contains(" ") )
            {
                LFunction.MessageExecutor("Error", "El campo de telefono no debe contener espacios en blanco");
                txtTelefono.Focus();
                return true;
            }
            if ((txtTelefono.Text.Length <= 9 && txtTelefono.Text != "") || txtTelefono.Text.Length >= 14)
            {
                LFunction.MessageExecutor("Error", "El campo del teléfono debe ser válido");
                txtTelefono.Focus();
                return true;
            }
            if (txtEmail.Text != "" && !Validaciones.IsValidEmail(txtEmail.Text))
            {
                LFunction.MessageExecutor("Error", "El correo electronico no contiene el formato correcto");
                txtEmail.Focus();
                return true;
            }
            if (txtTelefono.Text == "" && txtEmail.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe proporcionar un teléfono o correo electrónico del proveedor");
                txtTelefono.Focus();
                return true;
            }
            if (txtUrl.Text != "" && !Validaciones.ValidHttpURL(txtUrl.Text))
            {
                LFunction.MessageExecutor("Error", "La dirección de la página web no contiene el formato correcto");
                txtEmail.Focus();
                return true;
            }
            if (txtDireccion.Text != "" && txtDireccion.Text.Length <= 4)
            {
                LFunction.MessageExecutor("Error", "La dirección debe ser mayor a 4 carácteres");
                txtDireccion.Focus();
                return true;
            }

            if (Type != TypeForm.Update)
            {
                if (ActivarAnulado())
                {
                    return true;
                }

                if (Metodos.CedulaRepetida(CbTipoDocumento.Text + "-" + txtDocumento.Text))
                {
                    LFunction.MessageExecutor("Error", "El Documento ya está registrado en el sistema");
                    txtDocumento.Text = "";
                    txtDocumento.Focus();
                    return true;
                }
            }

            return false;
        }


        #endregion

        private void txtDocumento_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CbTipoDocumento.Text != "" && txtDocumento.Text != "")
                if (!ActivarAnulado())
                    if (Metodos.CedulaRepetida(CbTipoDocumento.Text + "-" + txtDocumento.Text))
                    {
                        LFunction.MessageExecutor("Error", "El Proveedor ya está Registrado en el Sistema");
                        txtDocumento.Text = "";
                        txtDocumento.Focus();
                    }
        }

        private void CbTipoDocumento_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CbTipoDocumento.Text != "" && txtDocumento.Text != "")
                if (!ActivarAnulado())
                    if (Metodos.CedulaRepetida(CbTipoDocumento.Text + "-" + txtDocumento.Text))
                    {
                        LFunction.MessageExecutor("Error", "El Proveedor ya está Registrado en el Sistema");
                        txtDocumento.Text = "";
                        txtDocumento.Focus();
                    }
        }

        private bool ActivarAnulado ()
        {
            List<DProveedor> response = Metodos.CedulaRepetidaAnulada(CbTipoDocumento.Text + "-" + txtDocumento.Text);

            if (response.Count > 0)
            {
                if(Globals.ACCESO_SISTEMA == 0)
                {
                    MessageBoxResult Resp = MessageBox.Show("El Proveedor está Anulado" + Environment.NewLine + "¿Desea reactivarlo?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (Resp == MessageBoxResult.Yes)
                    {
                        Type = TypeForm.Read;

                        txtTitulo.Text = "Editar Proveedor";
                        txtDocumento.IsEnabled = false;
                        CbTipoDocumento.IsEnabled = false;
                        fillForm(response[0]);

                        txtRazonSocial.Focus();
                    }
                    else
                    {
                        txtDocumento.Text = "";
                        CbTipoDocumento.Text = "";

                        CbTipoDocumento.Focus();
                    }
                }
                else
                {
                    LFunction.MessageExecutor("Error", "El Proveedor está Anulado" + Environment.NewLine + "Sólo el Administrador puede reactivarlo");

                    txtDocumento.Text = "";
                    CbTipoDocumento.Text = "";

                    CbTipoDocumento.Focus();
                }

                return true;
            }
            return false;
        }
    }
}
