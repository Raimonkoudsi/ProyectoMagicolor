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
    public partial class ClienteFrmVista : Window
    {
        public ClienteFrmVista(VentaFrm parent, string tipo, string documento)
        {
            InitializeComponent();
            txtDocumento.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtTelefono.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);

            ParentFrm = parent;

            TipoDocumento = tipo;
            Documento = documento;
        }

        public string TipoDocumento;
        public string Documento;

        public VentaFrm ParentFrm;

        public DCliente UForm;

        public LCliente Metodos = new LCliente();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Create();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            fillForm(TipoDocumento, Documento);
        }

       

        void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            string nombre = txtNombre.txt.Text;
            string apellidos = txtApellidos.txt.Text;
            string tipoDocumento = CbTipoDocumento.Text;
            string documento = txtDocumento.txt.Text;
            string direccion = txtDireccion.Text;
            string telefono = txtTelefono.Changed ? txtTelefono.txt.Text : "";
            string email = txtEmail.Changed ? txtEmail.txt.Text : "";

            UForm = new DCliente(0,
                                 nombre,
                                 apellidos,
                                 tipoDocumento,
                                 documento,
                                 direccion,
                                 telefono,
                                 email); //Datos
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
                    "Registrar",
                    "Ha Registrado el Cliente " + TipoDocumento + "-" + Documento
                 );
                new LAuditoria().Insertar(auditoria);

                ParentFrm.AgregarCliente(UForm);
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

        void SetEnable(bool Enable)
        {
            txtNombre.IsEnabled = Enable;
            txtApellidos.IsEnabled = Enable;
            CbTipoDocumento.IsEnabled = Enable;
            txtDocumento.IsEnabled = Enable;
            txtDireccion.IsEnabled = Enable;
            txtTelefono.IsEnabled = Enable;
            txtEmail.IsEnabled = Enable;
        }
        void fillForm(string tipo, string documento)
        {
                CbTipoDocumento.SelectedIndex = tipo == "V" ? 0 :
                                                tipo == "E" ? 1 :
                                                tipo == "J" ? 2 :
                                                tipo == "G" ? 3 : -1;
                txtDocumento.SetText(documento);
        }

        #region Validation
        bool Validate()
        {
            if (!txtNombre.Changed)
            {
                MessageBox.Show("Debes llenar el campo Nombre!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtNombre.txt.Focus();
                return true;
            }
            if (!txtApellidos.Changed)
            {
                MessageBox.Show("Debes llenar el campo Apellidos!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtApellidos.txt.Focus();
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
            if(txtTelefono.txt.Text.Contains(" ") && txtTelefono.Changed)
            {
                MessageBox.Show("El campo de telefono no debe tener espacios!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtTelefono.txt.Focus();
                return true;
            }
            if ((txtTelefono.txt.Text.Length <= 10 && txtTelefono.txt.Text.Length > 0) || txtTelefono.txt.Text.Length >= 14)
            {
                MessageBox.Show("Debe agregar un teléfono válido!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtTelefono.txt.Focus();
                return true;
            }
            if (txtDocumento.txt.Text.Length <= 6 || txtDocumento.txt.Text.Length >= 9)
            {
                MessageBox.Show("El campo del documento debe ser válido!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtDocumento.txt.Focus();
                return true;
            }
            if (txtEmail.Changed && !Validaciones.IsValidEmail(txtEmail.txt.Text))
            {
                MessageBox.Show("El correo electronico es incorrecto!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtEmail.txt.Focus();
                return true;
            }

            if (Metodos.CedulaRepetida(CbTipoDocumento.Text + "-" + txtDocumento.Text))
            {
                LFunction.MessageExecutor("Error", "La Cédula ya está Registrada en el Sistema");
                txtDocumento.txt.Focus();
                return true;
            }

            return false;
        }

        #endregion

    }
}
