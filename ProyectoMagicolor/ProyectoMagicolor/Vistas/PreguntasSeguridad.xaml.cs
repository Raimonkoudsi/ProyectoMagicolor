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
    /// <summary>
    /// Lógica de interacción para DetalleCuentaCobrarFrm.xaml
    /// </summary>
    public partial class PreguntasSeguridad : Window
    {
        Login ParentForm;

        public PreguntasSeguridad(Login parentfrm)
        {
            InitializeComponent();

            ParentForm = parentfrm;
        }



        public List<DTrabajador> DataFill;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Create();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtTitulo.Text = DataFill[0].usuario;
            txtPregunta.Text = DataFill[0].pregunta;
            txtPregunta2.Text = DataFill[1].pregunta;
            txtPregunta3.Text = DataFill[2].pregunta;
        }

        private bool Validate()
        {
            if (txtRespuesta.Text == "")
            {
                LFunction.MessageExecutor("Information", "Debe Responder la Primera Pregunta");
                txtRespuesta.Focus();
                return true;
            }
            if (txtRespuesta2.Text == "")
            {
                LFunction.MessageExecutor("Information", "Debe Responder la Segunda Pregunta");
                txtRespuesta2.Focus();
                return true;
            }
            if (txtRespuesta3.Text == "")
            {
                LFunction.MessageExecutor("Information", "Debe Responder la Tercera Pregunta");
                txtRespuesta3.Focus();
                return true;
            }

            return false;
        }

        void Create()
        {
            if (Validate()) return;

            if ((txtRespuesta.Text == DataFill[0].respuesta) && (txtRespuesta2.Text == DataFill[1].respuesta) && (txtRespuesta3.Text == DataFill[2].respuesta))
            {
                CambiarContraseña frmContraseña = new CambiarContraseña(this);
                frmContraseña.DataFill.usuario = DataFill[0].usuario;
                this.Hide();
                bool Resp = frmContraseña.ShowDialog() ?? false;
            }
            else
            {
                LFunction.MessageExecutor("Error", "Respuestas incorrectas, regresando al Login");
                this.Hide();
            }
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
