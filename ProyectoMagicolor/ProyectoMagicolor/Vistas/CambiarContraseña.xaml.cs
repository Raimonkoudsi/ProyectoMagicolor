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
            if (txtRespuesta.Text == "")
            {
                LFunction.MessageExecutor("Information", "Debe responder la primera pregunta");
                txtRespuesta.Focus();
                return true;
            }
            if (txtRespuesta2.Text == "")
            {
                LFunction.MessageExecutor("Information", "Debe responder la segunda pregunta");
                txtRespuesta2.Focus();
                return true;
            }

            if(txtRespuesta.Text.Length <= 5)
            {
                LFunction.MessageExecutor("Information", "La contraseña debe ser mínino 6 dígitos");
                txtRespuesta.Focus();
                return true;
            }

            if (txtRespuesta.Text != txtRespuesta2.Text)
            {
                LFunction.MessageExecutor("Error", "Las contraseñas deben coincidir");
                txtRespuesta2.Focus();
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


            this.Close();
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtRespuesta.Text == "")
            {
                txtBucarPlaceH.Text = "";
            }

        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtRespuesta.Text == "")
            {
                txtBucarPlaceH.Text = "Monto a Abonar";
            }

        }
    }
}
