using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        void Loging()
        {
            if (!txtUsuario.Changed)
            {
                MessageBox.Show("Debes poner un Nombre de Usuario!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtUsuario.Focus();
                return;
            }
            if (txtContraseña.Password == "")
            {
                MessageBox.Show("Debes poner la Contraseña!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtContraseña.Focus();
                return;
            }
            try
            {

                LTrabajador metodosUsuario = new LTrabajador();

                String[] respuesta = metodosUsuario.Login(txtUsuario.txt.Text, txtContraseña.Password);

                if(respuesta[0] != "")
                {
                    var MainFrm = new MainWindow();
                    this.Hide();
                    MainFrm.ShowDialog();
                    this.Close();
                }

                //MessageBox.Show(respuesta[0] + respuesta[1] + respuesta[2] + respuesta[3]);
            }
            catch
            {

            }
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            Loging();
        }

        private void txtContraseña_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtContraseña.Password == "")
            {
                txtContraeñaPlace.Text = "";
            }
        }

        private void txtContraseña_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtContraseña.Password == "")
            {
                txtContraeñaPlace.Text = "Contraseña";
            }
        }

        private void StackPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Loging();
            }
        }
    }
}
