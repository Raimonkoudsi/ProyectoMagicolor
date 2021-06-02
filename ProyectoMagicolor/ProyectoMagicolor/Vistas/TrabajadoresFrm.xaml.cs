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
    public enum TypeForm
    {
        Create,
        Read,
        Update,
        Delete
    }

    public class Validaciones
    {
        #region CharConverter
        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(
         uint wVirtKey,
         uint wScanCode,
         byte[] lpKeyState,
         [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
         int cchBuff,
         uint wFlags);
        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        public static char GetCharFromKey(Key key)
        {
            char ch = ' ';

            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
            }
            return ch;
        }
        #endregion
        public static bool IsValidEmail(string email)
        {
            var Foo = new EmailAddressAttribute();
            bool Valid = Foo.IsValid(email);
            return Valid;
        }

        public static bool ValidHttpURL(string s)
        {
            Uri resultURI;

            if (!Regex.IsMatch(s, @"^https?:\/\/", RegexOptions.IgnoreCase))
                s = "http://" + s;

            if (Uri.TryCreate(s, UriKind.Absolute, out resultURI))
                return (resultURI.Scheme == Uri.UriSchemeHttp ||
                        resultURI.Scheme == Uri.UriSchemeHttps);

            return false;
        }

        public static void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                TextBox s = e.Source as TextBox;
                if (s != null)
                {
                    s.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }

                e.Handled = true;
            }

            char ch = GetCharFromKey(e.Key);

            if (!Char.IsDigit(ch))
            {
                e.Handled = true;
            }
        }


        public static void DontPressNumber(object sender, KeyEventArgs e)
        {
            if (!Regex.IsMatch(GetCharFromKey(e.Key).ToString(), @"[a-zA-Z/]"))
            {
                e.Handled = true;
            }
        }

        public static void TextBoxValidatePrices(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                TextBox s = e.Source as TextBox;
                if (s != null)
                {
                    s.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }

                e.Handled = true;
            }

            char ch = GetCharFromKey(e.Key);

            TextBox txtBox = (TextBox)sender;

            if (ch == 46 && txtBox.Text.IndexOf('.') != -1)
            {
                e.Handled = true;
                return;
            }

            if (Regex.IsMatch(txtBox.Text, @"\.\d\d"))
            {
                e.Handled = true;
                return;
            }

            if (!Char.IsDigit(ch) && ch != 9 && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }
    }


    public partial class TrabajadoresFrm : Window
    {
        public TypeForm Type;
        public DTrabajador DataFill;
        public List<DSeguridad> DataFillSeguridad;

        public DTrabajador UForm;

        public LTrabajador MetodosUsuario = new LTrabajador();

        private string cedula = "";


        public TrabajadoresFrm()
        {
            InitializeComponent();

            txtDocumento.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtTelefono.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);

            txtNombre.KeyDown += new KeyEventHandler(Validaciones.DontPressNumber);
            txtApellidos.KeyDown += new KeyEventHandler(Validaciones.DontPressNumber);
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
            DpNacimiento.DisplayDateEnd = DateTime.Today.AddYears(-18);

            if (Type == TypeForm.Read)
            {
                txtTitulo.Text = "Leer Trabajador";
                fillForm(DataFill, ListaSeguridad);
                SetEnable(false);
                btnEnviar.IsEnabled = false;
                btnEnviar.Content = "Sólo Lectura";

                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Mostrar",
                    "Ha Visto al Trabajador " + cedula
                 );
                new LAuditoria().Insertar(auditoria);
            }
            else if (Type == TypeForm.Update)
            {
                txtTitulo.Text = "Editar Trabajador";
                fillForm(DataFill, ListaSeguridad);

                SetEnable(true);
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
            string apellido = txtApellidos.Text;
            string sexo = CbSexo.SelectedIndex == 0 ? "H" : "M";
            DateTime Nacimiento = DpNacimiento.SelectedDate ?? DateTime.Now;
            cedula = CbTipoDocumento.Text + "-" + txtDocumento.Text;
            string direccion = txtDireccion.Text;
            string telefono = txtTelefono.Text;
            string email = txtEmail.Text;
            int acceso = (CbAcceso.SelectedIndex + 1);
            string usuario = txtUsuario.Text;
            string contraseña = txtPassword.Password;

            UForm = new DTrabajador(0,
                                    nombre,
                                    apellido,
                                    sexo,
                                    Nacimiento,
                                    cedula,
                                    direccion,
                                    telefono,
                                    email,
                                    acceso,
                                    usuario,
                                    contraseña);

            AgregarSeguridad();
        }

        private void Create()
        {
            fillData();
            if (UForm == null)
                return;

            AgregarSeguridad();

            string response = MetodosUsuario.Insertar(UForm, ListaSeguridad);
            if (response == "OK")
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Insertar",
                    "Ha Registrado al Trabajador " + cedula
                );
                new LAuditoria().Insertar(auditoria);

                this.DialogResult = true;
                this.Close();
            }

        }


        public List<DSeguridad> ListaSeguridad = new List<DSeguridad>();

        public void AgregarSeguridad()
        {
            ListaSeguridad.Clear();

            DSeguridad DS = new DSeguridad();
            DSeguridad DS2 = new DSeguridad();
            DSeguridad DS3 = new DSeguridad();

            DS.pregunta = txtPregunta.Text;
            DS.respuesta = txtRespuesta.Text;
            ListaSeguridad.Add(DS);

            DS2.pregunta = txtPregunta2.Text;
            DS2.respuesta = txtRespuesta2.Text;
            ListaSeguridad.Add(DS2);

            DS3.pregunta = txtPregunta3.Text;
            DS3.respuesta = txtRespuesta3.Text;
            ListaSeguridad.Add(DS3);
    
        }

        private void Update()
        {
            fillData();
            if (UForm == null)
                return;
            UForm.idTrabajador = DataFill.idTrabajador;

            string response = MetodosUsuario.Editar(UForm, ListaSeguridad);
            if(response == "OK")
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Editar",
                    "Ha Editado al Trabajador " + cedula
                );
                new LAuditoria().Insertar(auditoria);

                this.DialogResult = true;
                this.Close();
            } 
        }

        private void SetEnable(bool Enable)
        {
            txtNombre.IsEnabled = Enable;
            txtApellidos.IsEnabled = Enable;
            CbSexo.IsEnabled = Enable;
            DpNacimiento.IsEnabled = Enable;
            txtDocumento.IsEnabled = false;
            txtDireccion.IsEnabled = Enable;
            txtTelefono.IsEnabled = Enable;
            txtEmail.IsEnabled = Enable;
            CbAcceso.IsEnabled = Enable;
            CbTipoDocumento.IsEnabled = false;
            txtUsuario.IsEnabled = false;
            txtPassword.IsEnabled = Enable;
            txtCPassword.IsEnabled = Enable;
            txtPregunta.IsEnabled = Enable;
            txtRespuesta.IsEnabled = Enable;
            txtPregunta2.IsEnabled = Enable;
            txtRespuesta2.IsEnabled = Enable;
            txtPregunta3.IsEnabled = Enable;
            txtRespuesta3.IsEnabled = Enable;
        }

        private void fillForm(DTrabajador Data, List<DSeguridad> DataSeguridad)
        {
            if(Data != null)
            {
                txtNombre.Text = Data.nombre;
                txtApellidos.Text = Data.apellidos;
                CbSexo.SelectedIndex = Data.sexo == "H" ? 0 : 1;
                DpNacimiento.SelectedDate = Data.fechaNacimiento;

                CbTipoDocumento.SelectedIndex = Data.cedula.Contains("V") == true ? 0 : 1;
                txtDocumento.Text = Data.cedula.Remove(0,2);

                txtDireccion.Text = Data.direccion;
                txtTelefono.Text = Data.telefono;
                txtEmail.Text = Data.email;

                if (Data.acceso == 1)
                    CbAcceso.SelectedIndex = 0;
                else
                    CbAcceso.SelectedIndex = 1;

                txtUsuario.Text = Data.usuario;
                txtPassword.Password = Data.contraseña;
                txtCPassword.Password = Data.contraseña;

                txtPregunta.Text = DataSeguridad[0].pregunta;
                txtRespuesta.Text = DataSeguridad[0].respuesta;

                txtPregunta2.Text = DataSeguridad[1].pregunta;
                txtRespuesta2.Text = DataSeguridad[1].respuesta;

                txtPregunta3.Text = DataSeguridad[2].pregunta;
                txtRespuesta3.Text = DataSeguridad[2].respuesta;
            }
        }

        #region Validation
        private bool Validate()
        {

            if (CbTipoDocumento.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe seleccionar el tipo de cédula");
                txtDocumento.Focus();
                return true;
            }

            if (txtDocumento.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar el campo de cédula");
                txtDocumento.Focus();
                return true;
            }

            if (txtDocumento.Text.Length < 6 || txtDocumento.Text.Length >= 9)
            {
                LFunction.MessageExecutor("Error", "El campo del documento debe ser válido");
                txtDocumento.Focus();
                return true;
            }

            if (txtNombre.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar el campo de nombre");
                txtNombre.Focus();
                return true;
            }

            if (txtApellidos.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar el campo de apellido");
                txtApellidos.Focus();
                return true;
            }

            if (CbSexo.SelectedIndex == -1)
            {
                LFunction.MessageExecutor("Error", "Debe seleccionar el sexo");
                CbSexo.Focus();
                return true;
            }

            if (DpNacimiento.SelectedDate == null)
            {
                LFunction.MessageExecutor("Error", "Debe seleccionar la fecha de nacimiento");
                DpNacimiento.Focus();
                return true;
            }

            if ((txtTelefono.Text.Length <= 10 && txtTelefono.Text != "") || txtTelefono.Text.Length >= 14)
            {
                LFunction.MessageExecutor("Error", "Debe ingresar un teléfono váido");
                txtTelefono.Focus();
                return true;
            }

            if (txtTelefono.Text == "" && txtEmail.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar un teléfono o correo del trabajador");
                txtTelefono.Focus();
                return true;
            }

            if (txtUsuario.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar el campo de usuario");
                txtUsuario.Focus();
                return true;
            }

            if (CbAcceso.SelectedIndex == -1)
            {
                LFunction.MessageExecutor("Error", "Debe seleccionar el nivel de acceso");
                CbAcceso.Focus();
                return true;
            }

            if (txtPassword.Password == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar la contraseña");
                MessageBox.Show("Debes llenar el campo Contraseña!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPassword.Focus();
                return true;
            }

            if (txtPassword.Password.Length <= 5)
            {
                LFunction.MessageExecutor("Error", "Las contraseña debe ser mayor a 6 dígitos");
                txtCPassword.Focus();
                return true;
            }

            if (txtCPassword.Password == "")
            {
                LFunction.MessageExecutor("Error", "Debe confirmar la contraseña");
                txtCPassword.Focus();
                return true;
            }

            if (txtCPassword.Password != txtPassword.Password)
            {
                LFunction.MessageExecutor("Error", "Las contraseñas deben coincidir");
                txtCPassword.Focus();
                return true;
            }

            if (txtPregunta.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar la primera pregunta");
                txtPregunta.Focus();
                return true;
            }

            if (txtRespuesta.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar la primera respuesta");
                txtRespuesta.Focus();
                return true;
            }

            if (txtPregunta2.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar la segunda pregunta");
                txtPregunta.Focus();
                return true;
            }

            if (txtRespuesta2.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar la segunda respuesta");
                txtRespuesta.Focus();
                return true;
            }

            if (txtPregunta3.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar la tercera pregunta");
                txtPregunta.Focus();
                return true;
            }

            if (txtRespuesta3.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar la tercera respuesta");
                txtRespuesta.Focus();
                return true;
            }

            if (txtEmail.Text != "" && !Validaciones.IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("El correo electronico es incorrecto!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtEmail.Focus();
                return true;
            }

            if(Type != TypeForm.Update)
            {
                if (ActivarAnulado())
                {
                    return true;
                }

                if (MetodosUsuario.CedulaRepetida(CbTipoDocumento.Text + "-" + txtDocumento.Text))
                {
                    LFunction.MessageExecutor("Error", "La Cédula ya está Registrada en el Sistema");
                    txtDocumento.Focus();
                    return true;
                }

                if (MetodosUsuario.UsuarioRepetido(txtUsuario.Text))
                {
                    LFunction.MessageExecutor("Error", "El Nombre de Usuario ya está Registrado en el Sistema");
                    txtUsuario.Focus();
                    return true;
                }
            }


            return false;
        }
        #endregion


        private bool ActivarAnulado()
        {
            List<DTrabajador> response = MetodosUsuario.CedulaRepetidaAnulada((CbTipoDocumento.Text + "-" + txtDocumento.Text));

            if (response.Count > 0)
            {
                MessageBoxResult Resp = MessageBox.Show("El Trabajador está Anulado" + Environment.NewLine + "¿Desea reactivarlo?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (Resp == MessageBoxResult.Yes)
                {
                    Type = TypeForm.Read;

                    txtTitulo.Text = "Editar Trabajador";
                    txtDocumento.IsEnabled = false;
                    CbTipoDocumento.IsEnabled = false;
                    txtUsuario.IsEnabled = false;
                    fillForm(response[0], new LTrabajador().EncontrarSeguridad(response[0].idTrabajador));

                    txtNombre.Focus();
                }
                else
                {
                    txtDocumento.Text = "";
                    CbTipoDocumento.Text = "";

                    CbTipoDocumento.Focus();
                }

                return true;
            }
            return false;
        }


        private void txtDocumento_LostFocus(object sender, RoutedEventArgs e)
        {
            if(CbTipoDocumento.Text != "" && txtDocumento.Text != "")
                if(!ActivarAnulado())
                    if (MetodosUsuario.CedulaRepetida(CbTipoDocumento.Text + "-" + txtDocumento.Text))
                    {
                        LFunction.MessageExecutor("Error", "La Cédula ya está Registrada en el Sistema");
                        txtDocumento.Focus();
                    }
        }

        private void CbTipoDocumento_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CbTipoDocumento.Text != "" && txtDocumento.Text != "")
                if (!ActivarAnulado())
                    if (MetodosUsuario.CedulaRepetida(CbTipoDocumento.Text + "-" + txtDocumento.Text))
                    {
                        LFunction.MessageExecutor("Error", "La Cédula ya está Registrada en el Sistema");
                        txtDocumento.Focus();
                    }
        }

        private void txtUsuario_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Type != TypeForm.Update)
            {
                if (MetodosUsuario.UsuarioRepetido(txtUsuario.Text))
                {
                    LFunction.MessageExecutor("Error", "El Nombre de Usuario ya está Registrado en el Sistema");
                    txtUsuario.Focus();
                }
            }
        }
    }
}
