using System.Collections.Generic;
using System.Windows;
using Datos;
using Logica;


namespace ProyectoMagicolor.Vistas
{
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
            if (txtRespuesta.Password == "")
            {
                LFunction.MessageExecutor("Information", "Debe Responder la Primera Pregunta");
                txtRespuesta.Focus();
                return true;
            }
            if (txtRespuesta2.Password == "")
            {
                LFunction.MessageExecutor("Information", "Debe Responder la Segunda Pregunta");
                txtRespuesta2.Focus();
                return true;
            }
            if (txtRespuesta3.Password == "")
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

            if ((txtRespuesta.Password == DataFill[0].respuesta) && (txtRespuesta2.Password == DataFill[1].respuesta) && (txtRespuesta3.Password == DataFill[2].respuesta))
            {
                CambiarContraseña frmContraseña = new CambiarContraseña(this);
                frmContraseña.DataFill = DataFill[0];
                LFunction.MessageExecutor("Information", "Datos correctos, introducir nueva contraseña");
                this.Hide();
                bool Resp = frmContraseña.ShowDialog() ?? false;
            }
            else
            {
                Login.ContadorIntentos();
                this.Hide();
            }
        }
    }
}
