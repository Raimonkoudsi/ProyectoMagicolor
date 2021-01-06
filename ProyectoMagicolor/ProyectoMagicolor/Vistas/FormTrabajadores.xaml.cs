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
        Update
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
    }
    /// <summary>
    /// Interaction logic for FormTrabajadores.xaml
    /// </summary>
    public partial class FormTrabajadores : Window
    {


        public FormTrabajadores()
        {
            InitializeComponent();
            txtCedula.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtTelefono.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
        }

        

        public TypeForm Type;

        public DTrabajador UForm;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //DTrabajador trabajador = new DTrabajador(0,
            //                                        txt1.Text,
            //                                        txt2.Text,
            //                                        txt3.Text,
            //                                        txt4.DisplayDate,
            //                                        txt5.Text,
            //                                        txt6.Text,
            //                                        txt7.Text,
            //                                        txt8.Text,
            //                                        txt9.Text,
            //                                        txt10.Text,
            //                                        txt11.Text,
            //                                        txt12.Text,
            //                                        txt13.Text,
            //                                        "");
            //LTrabajador lTrabajador = new LTrabajador();
            //MessageBox.Show(customtxt.txt.Text);
            Create();
            //string respuesta = lTrabajador.Insertar(trabajador);

            //MessageBox.Show(respuesta);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtNombre.SetText("");
        }

        void Create()
        {
            if (!(txtNombre.Changed && txtApellidos.Changed
                && CbSexo.SelectedIndex > -1 && DpNacimiento.SelectedDate != null
                && txtCedula.Changed
                && txtUsuario.Changed && CbAcceso.SelectedIndex > -1 && txtPassword.Password != "" && txtCPassword.Password != "" 
                && txtPregunta.Changed && txtRespuesta.Changed
                && Validaciones.IsValidEmail(txtEmail.txt.Text)))
                MessageBox.Show("Validación");
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

        
    }
}
