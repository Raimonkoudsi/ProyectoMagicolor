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

        private void login_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                LTrabajador metodosUsuario = new LTrabajador();

                String[] respuesta = metodosUsuario.Login(txtUsuario.Text, txtContraseña.Password);

                MessageBox.Show(respuesta[0] + respuesta[1] + respuesta[2]+ respuesta[3]);
            }
            catch
            {

            }



        }
    }
}
