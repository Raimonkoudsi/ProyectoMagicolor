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
    public partial class ClienteFrm : Window
    {
        public TypeForm Type;
        public DCliente DataFill;

        public DCliente UForm;

        public LCliente Metodos = new LCliente();

        private string tipoDocumento = "";
        private string documento = "";

        public VentaFrm ParentFrm;
        public string TipoDocumento;
        public string Documento;

        public ClienteFrm(VentaFrm parent = null, string tipo = null, string documento = null)
        {
            InitializeComponent();
            txtDocumento.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtTelefono.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);


            ParentFrm = parent;
            TipoDocumento = tipo;
            Documento = documento;
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
                txtTitulo.Text = "Leer Cliente";
                fillForm(DataFill);
                SetEnable(false);

                btnEnviar.IsEnabled = false;
                btnEnviar.Content = "Sólo Lectura";

                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Mostrar",
                    "Ha Visto al Cliente " + tipoDocumento + "-" + documento
                 );
                new LAuditoria().Insertar(auditoria);
            }
            else if(Type == TypeForm.Update)
            {
                txtTitulo.Text = "Editar Cliente";
                fillForm(DataFill);
            }
            if (ParentFrm != null)
            {
                CbTipoDocumento.Text = TipoDocumento;
                txtDocumento.Text = Documento;
                CbTipoDocumento.IsEnabled = false;
                txtDocumento.IsEnabled = false;
                txtNombre.Focus();
            }
        }

       

        private void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            string nombre = txtNombre.Text;
            tipoDocumento = CbTipoDocumento.Text;
            documento = txtDocumento.Text;
            string direccion = txtDireccion.Text;
            string telefono = txtTelefono.Text;
            string email = txtEmail.Text;

            UForm = new DCliente(0,
                                 nombre,
                                 tipoDocumento,
                                 documento,
                                 direccion,
                                 telefono,
                                 email); //Datos
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
                    "Ha Registrado al Cliente " + tipoDocumento + "-" + documento
                );
                new LAuditoria().Insertar(auditoria);

                if(ParentFrm != null)
                    ParentFrm.AgregarCliente(UForm);

                this.DialogResult = true;
                this.Close();
            }

        }

        private void Update()
        {
            fillData();
            if (UForm == null)
                return;
            UForm.idCliente = DataFill.idCliente;
            string response = Metodos.Editar(UForm);
            if(response == "OK")
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Editar",
                    "Ha Editado al Cliente " + tipoDocumento + "-" + documento
                );
                new LAuditoria().Insertar(auditoria);

                this.DialogResult = true;
                this.Close();
            }
        }


        void SetEnable(bool Enable)
        {
            txtNombre.IsEnabled = Enable;
            CbTipoDocumento.IsEnabled = Enable;
            txtDocumento.IsEnabled = Enable;
            txtDireccion.IsEnabled = Enable;
            txtTelefono.IsEnabled = Enable;
            txtEmail.IsEnabled = Enable;
        }
        void fillForm(DCliente Data)
        {
            if(Data != null)
            {
                txtNombre.Text = Data.nombre;
                CbTipoDocumento.SelectedIndex = Data.tipoDocumento == "V" ? 0 :
                                                Data.tipoDocumento == "E" ? 1 : 
                                                Data.tipoDocumento == "J" ? 2 :
                                                Data.tipoDocumento == "G" ? 3 : - 1;

                txtDocumento.Text = Data.numeroDocumento;
                txtTelefono.Text = Data.telefono;
                txtEmail.Text = Data.email;
                txtDireccion.Text = Data.direccion;
            }
        }


        #region Validation
        private bool Validate()
        {
            if (CbTipoDocumento.SelectedIndex == -1)
            {
                MessageBox.Show("Debes seleccionar un Tipo de Documento!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                CbTipoDocumento.Focus();
                return true;
            }
            if (txtDocumento.Text == "")
            {
                MessageBox.Show("Debes llenar el campo Documento!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtDocumento.Focus();
                return true;
            }
            if (txtDocumento.Text.Length <= 6 || txtDocumento.Text.Length >= 11)
            {
                LFunction.MessageExecutor("Error", "El campo del documento debe ser válido");
                txtDocumento.Focus();
                return true;
            }
            if (txtNombre.Text == "")
            {
                MessageBox.Show("Debes llenar el campo Nombre!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtNombre.Focus();
                return true;
            }
            if(txtTelefono.Text.Contains(" ") && txtTelefono.Text != "")
            {
                MessageBox.Show("El campo de telefono no debe tener espacios!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("El correo electronico es incorrecto!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtEmail.Focus();
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
                        LFunction.MessageExecutor("Error", "El Cliente ya está Registrado en el Sistema");
                        txtDocumento.Focus();
                    }
        }

        private void CbTipoDocumento_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CbTipoDocumento.Text != "" && txtDocumento.Text != "")
                if (!ActivarAnulado())
                    if (Metodos.CedulaRepetida(CbTipoDocumento.Text + "-" + txtDocumento.Text))
                    {
                        LFunction.MessageExecutor("Error", "El Cliente ya está Registrado en el Sistema");
                        txtDocumento.Focus();
                    }
        }


        private bool ActivarAnulado()
        {
            List<DCliente> response = Metodos.CedulaRepetidaAnulada(CbTipoDocumento.Text + "-" + txtDocumento.Text);

            if (response.Count > 0)
            {
                if (Globals.ACCESO_SISTEMA == 0)
                {
                    MessageBoxResult Resp = MessageBox.Show("El Cliente está Deshabilitado" + Environment.NewLine + "¿Desea reactivarlo?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (Resp == MessageBoxResult.Yes)
                    {
                        Type = TypeForm.Read;

                        txtTitulo.Text = "Editar Cliente";
                        txtDocumento.IsEnabled = false;
                        CbTipoDocumento.IsEnabled = false;
                        fillForm(response[0]);

                        txtNombre.Focus();
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
                    LFunction.MessageExecutor("Error", "El Cliente está Deshabilitado" + Environment.NewLine + "Sólo el Administrador puede reactivarlo");

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
