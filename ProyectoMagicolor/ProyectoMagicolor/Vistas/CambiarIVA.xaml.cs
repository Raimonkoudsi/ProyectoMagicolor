using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    /// <summary>
    /// Interaction logic for FormTrabajadores.xaml
    /// </summary>
    public partial class CambiarIVA : Window
    {

        public CambiarIVA()
        {
            InitializeComponent();

            txtIVA.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
        }

       

        public LArticulo Metodos = new LArticulo();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Send();
        }

        public void Send()
        {
            if (Validate())
                return;

            string respuesta = LFunction.EditarIVA(int.Parse(txtIVA.Text));

            if(respuesta.Equals("OK"))
            {
                LFunction.MessageExecutor("Information", "El IVA se ha actualizado correctamente");
                this.Hide();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtIVA.Text = LFunction.MostrarIVA().ToString();
            txtIVA.Focus();
        } 

        #region Validation
        bool Validate()
        {
            if (txtIVA.Text == "")
            {
                MessageBox.Show("Debe ingresar el IVA", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtIVA.Focus();
                return true;
            }

            return false;
        }


        #endregion

        private void txtIVA_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Send();
            }
        }
    }
}
