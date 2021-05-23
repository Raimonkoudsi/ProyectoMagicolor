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
using Microsoft.Win32;


namespace ProyectoMagicolor.Vistas
{

    public partial class CambiarContraseña : Window
    {
        PreguntasSeguridad ParentForm;

        public CambiarContraseña(PreguntasSeguridad parentfrm)
        {
            InitializeComponent();

            ParentForm = parentfrm;
        }

        public DTrabajador DataFill;

        LTrabajador Metodos = new LTrabajador();

        private string usuario;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            usuario = DataFill.usuario;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Create();
        }

        private bool Validate()
        {
            if (txtContraseña.Password == "")
            {
                LFunction.MessageExecutor("Information", "Debe ingresar la contraseña");
                txtContraseña.Focus();
                return true;
            }
            if (txtConfirmar.Password == "")
            {
                LFunction.MessageExecutor("Information", "Debe confirmar la contraseña");
                txtConfirmar.Focus();
                return true;
            }

            if(txtContraseña.Password.Length <= 5)
            {
                LFunction.MessageExecutor("Information", "La contraseña debe ser mínino 6 dígitos");
                txtContraseña.Focus();
                return true;
            }

            if (txtContraseña.Password != txtConfirmar.Password)
            {
                LFunction.MessageExecutor("Error", "Las contraseñas deben coincidir");
                txtConfirmar.Focus();
                return true;
            }

            return false;
        }

        void Create()
        {
            if (Validate()) return;

            MessageBoxResult rpta;
            rpta = MessageBox.Show("¿Está seguro que desea implementar esta contraseña?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (rpta == MessageBoxResult.No)
                return;

            string respuesta = Metodos.EditarContraseña(usuario, txtContraseña.Password);

            if (respuesta.Equals("OK"))
            {
                LFunction.MessageExecutor("Information", "Contraseña cambiada correctamente, regresando al Login");
            }
            else
            {
                LFunction.MessageExecutor("Information", respuesta);
            }

            this.Hide();
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtContraseña.Password == "")
            {
                txtBucarPlaceH.Text = "";
            }

        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtContraseña.Password == "")
            {
                txtBucarPlaceH.Text = "Introducir Contraseña";
            }

        }


        private void txtBuscar2_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtConfirmar.Password == "")
            {
                txtBucarPlaceH2.Text = "";
            }

        }

        private void txtBuscar2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtContraseña.Password == "")
            {
                txtBucarPlaceH2.Text = "Introducir Contraseña";
            }

        }
    }
}
