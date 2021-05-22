﻿using System;
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
    /// <summary>
    /// Interaction logic for FormTrabajadores.xaml
    /// </summary>
    public partial class TrabajadoresFrm : Window
    {


        public TrabajadoresFrm()
        {
            InitializeComponent();
            txtDocumento.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtTelefono.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
        }

        

        public TypeForm Type;
        public DTrabajador DataFill;
        public List<DSeguridad> DataFillSeguridad;

        public DTrabajador UForm;

        public LTrabajador MetodosUsuario = new LTrabajador();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Type == TypeForm.Update)
                Update();
            else
                Create();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Type == TypeForm.Read)
            {
                txtTitulo.Text = "Leer Trabajador";
                fillForm(DataFill, ListaSeguridad);
                SetEnable(false);
                btnEnviar.Visibility = Visibility.Collapsed;

            }
            else if (Type == TypeForm.Update)
            {
                txtTitulo.Text = "Editar Trabajador";
                fillForm(DataFill, ListaSeguridad);
            }
        } 

       

        void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            string nombre = txtNombre.txt.Text;
            string apellido = txtApellidos.txt.Text;
            string sexo = CbSexo.SelectedIndex == 0 ? "H" : "M";
            DateTime Nacimiento = DpNacimiento.SelectedDate ?? DateTime.Now;
            string cedula = CbTipoDocumento.Text + "-" + txtDocumento.txt.Text;
            string direccion = txtDireccion.txt.Text;
            string telefono = txtTelefono.txt.Text;
            string email = txtEmail.txt.Text;
            int acceso = CbAcceso.SelectedIndex;
            string usuario = txtUsuario.txt.Text;
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
                                    contraseña,
                                    "");

            AgregarSeguridad();
        }

        void Create()
        {
            fillData();
            if (UForm == null)
                return;

            AgregarSeguridad();

            string response = MetodosUsuario.Insertar(UForm, ListaSeguridad);
            if (response == "OK")
            {
                LFunction.MessageExecutor("Information", "Trabajador Ingresado Correctamente");
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                LFunction.MessageExecutor("Error", response);
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

        void Update()
        {
            fillData();
            if (UForm == null)
                return;
            UForm.idTrabajador = DataFill.idTrabajador;

            string response = MetodosUsuario.Editar(UForm, ListaSeguridad);
            if(response == "OK")
            {
                LFunction.MessageExecutor("Information", "Trabajador Editado Correctamente");
                this.DialogResult = true;
                this.Close();
            } 
            else
            {
                LFunction.MessageExecutor("Error", response);
            }
        }

        private void DpNacimiento_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DpNacimiento.SelectedDate != null)
            {
                dpNacPlaceholder.Text = "";
            }
            else
            {
                dpNacPlaceholder.Text = "Fecha de Nacimiento";
            }

        }

        private void CbSexo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbSexo.SelectedIndex > -1)
                CbSexoPlaceholder.Text = "";
            else
                CbSexoPlaceholder.Text = "Sexo";
        }

        private void CbAcceso_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbAcceso.SelectedIndex > -1)
                CbAccesoPlaceholder.Text = "";
            else
                CbAccesoPlaceholder.Text = "Acceso";
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtPassword.Password == "")
            {
                txtPassPlaceholder.Text = "";
            }
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtPassword.Password == "")
            {
                txtPassPlaceholder.Text = "Contraseña";
            }
        }

        private void txtCPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            
            if (txtCPassword.Password == "")
            {
                txtCPassPlaceholder.Text = "Confirmar Contraseña";
            }
        }

        private void txtCPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtCPassword.Password == "")
            {
                txtCPassPlaceholder.Text = "";
            }
        }

        private void txtUsuario_LostFocus(object sender, RoutedEventArgs e)
        {

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
            CbSexo.IsEnabled = Enable;
            DpNacimiento.IsEnabled = Enable;
            txtDocumento.IsEnabled = Enable;
            txtDireccion.IsEnabled = Enable;
            txtTelefono.IsEnabled = Enable;
            txtEmail.IsEnabled = Enable;
            CbAcceso.IsEnabled = Enable;
            CbTipoDocumento.IsEnabled = Enable;
            txtUsuario.IsEnabled = Enable;
            txtPassword.IsEnabled = Enable;
            txtCPassword.IsEnabled = Enable;
            txtPregunta.IsEnabled = Enable;
            txtRespuesta.IsEnabled = Enable;
            txtPregunta2.IsEnabled = Enable;
            txtRespuesta2.IsEnabled = Enable;
            txtPregunta3.IsEnabled = Enable;
            txtRespuesta3.IsEnabled = Enable;
        }
        void fillForm(DTrabajador Data, List<DSeguridad> DataSeguridad)
        {
            if(Data != null)
            {
                txtNombre.SetText(Data.nombre);
                txtApellidos.SetText(Data.apellidos);
                CbSexo.SelectedIndex = Data.sexo == "H" ? 0 : 1;
                DpNacimiento.SelectedDate = Data.fechaNacimiento;

                CbTipoDocumento.SelectedIndex = Data.cedula.Contains("V") == true ? 0 : 1;
                txtDocumento.SetText(Data.cedula.Remove(0,2));

                if(Data.direccion != "")
                    txtDireccion.SetText(Data.direccion);
                if(Data.telefono != "")
                    txtTelefono.SetText(Data.telefono);
                if(Data.email != "")
                    txtEmail.SetText(Data.email);

                CbAcceso.SelectedIndex = Data.acceso.Equals("Admin") ? 0 :
                                         Data.acceso.Equals("Encargado") ? 1 : 2;
                txtUsuario.SetText(Data.usuario);

                txtPregunta.SetText(DataSeguridad[0].pregunta);
                txtRespuesta.SetText(DataSeguridad[0].respuesta);

                txtPregunta2.SetText(DataSeguridad[1].pregunta);
                txtRespuesta2.SetText(DataSeguridad[1].respuesta);

                txtPregunta3.SetText(DataSeguridad[2].pregunta);
                txtRespuesta3.SetText(DataSeguridad[2].respuesta);

                if (Type == TypeForm.Read)
                {
                    Password.Visibility = Visibility.Collapsed;
                    CPassword.Visibility = Visibility.Collapsed;
                }
            }
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

            if (CbSexo.SelectedIndex == -1)
            {
                MessageBox.Show("Debes seleccionar el Sexo!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                CbSexo.Focus();
                return true;
            }

            if (DpNacimiento.SelectedDate == null)
            {
                MessageBox.Show("Debes llenar la Fecha de Nacimiento!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                DpNacimiento.Focus();
                return true;
            }

            if (!txtDocumento.Changed)
            {
                MessageBox.Show("Debes llenar el campo Cedula!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtDocumento.txt.Focus();
                return true;
            }

            if (!txtUsuario.Changed)
            {
                MessageBox.Show("Debes llenar el campo Usuario!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtUsuario.txt.Focus();
                return true;
            }

            if (CbAcceso.SelectedIndex == -1)
            {
                MessageBox.Show("Debes seleccionar el Acceso!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                CbAcceso.Focus();
                return true;
            }


            if (txtPassword.Password == "" && Type != TypeForm.Update)
            {
                MessageBox.Show("Debes llenar el campo Contraseña!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPassword.Focus();
                return true;
            }

            if (((txtCPassword.Password == "" && Type != TypeForm.Update) || (Type == TypeForm.Update && txtCPassword.Password == "" && txtPassword.Password != "")))
            {
                MessageBox.Show("Debes llenar el campo de Confirmar Contraseña!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtCPassword.Focus();
                return true;
            }

            if (!txtPregunta.Changed)
            {
                MessageBox.Show("Debes llenar el primer campo Pregunta!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPregunta.txt.Focus();
                return true;
            }

            if (!txtRespuesta.Changed)
            {
                MessageBox.Show("Debes llenar el primer campo Respuesta!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtRespuesta.txt.Focus();
                return true;
            }

            if (!txtPregunta2.Changed)
            {
                MessageBox.Show("Debes llenar el segundo campo Pregunta!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPregunta.txt.Focus();
                return true;
            }

            if (!txtRespuesta2.Changed)
            {
                MessageBox.Show("Debes llenar el segundo campo Respuesta!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtRespuesta.txt.Focus();
                return true;
            }

            if (!txtPregunta3.Changed)
            {
                MessageBox.Show("Debes llenar el tercer campo Pregunta!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPregunta.txt.Focus();
                return true;
            }

            if (!txtRespuesta3.Changed)
            {
                MessageBox.Show("Debes llenar el tercer campo Respuesta!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtRespuesta.txt.Focus();
                return true;
            }

            if (txtEmail.txt.Text != "" && !Validaciones.IsValidEmail(txtEmail.txt.Text))
            {
                MessageBox.Show("El correo electronico es incorrecto!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtEmail.txt.Focus();
                return true;
            }

            if (txtPassword.Password != txtCPassword.Password)
            {
                MessageBox.Show("Los campos Contraseña y su confirmación no son iguales!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPassword.Focus();
                return true;
            }

            if(Type != TypeForm.Update)
            {
                if (MetodosUsuario.CedulaRepetida(CbTipoDocumento.Text + "-" + txtDocumento.Text))
                {
                    MessageBox.Show("La Cedula ingresada está Repetida!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtDocumento.SetText("");
                    txtDocumento.txt.Focus();
                }
            }


            return false;
        }
        #endregion

    }
}
