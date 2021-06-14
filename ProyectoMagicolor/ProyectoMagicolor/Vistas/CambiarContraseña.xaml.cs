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
        private int idTrabajador;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            usuario = DataFill.usuario;
            idTrabajador = DataFill.idTrabajador;
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

            if (StrongPassword())
            {
                return true;
            }

            if (txtConfirmar.Password == "")
            {
                LFunction.MessageExecutor("Error", "Debe confirmar la contraseña");
                txtConfirmar.Focus();
                return true;
            }

            if(txtContraseña.Password.Length <= 5)
            {
                LFunction.MessageExecutor("Error", "La contraseña debe ser mínino 6 carácteres");
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


        private bool StrongPassword()
        {
            if (txtContraseña.Password.Length < 6)
            {
                MessageBox.Show("La contraseña no puede ser menor de 6 carácteres!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
                txtContraseña.Focus();
                return true;
            }
            if (txtContraseña.Password.Length > 24)
            {
                MessageBox.Show("La contraseña no puede ser mayor de 24 carácteres!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
                txtContraseña.Focus();
                return true;
            }
            if (!txtContraseña.Password.Any(char.IsUpper))
            {
                MessageBox.Show("La contraseña debe contener al menos una mayúscula!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
                txtContraseña.Focus();
                return true;
            }
            if (!txtContraseña.Password.Any(char.IsLower))
            {
                MessageBox.Show("La contraseña debe contener al menos una minúscula!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
                txtContraseña.Focus();
                return true;
            }
            if (!txtContraseña.Password.Any(char.IsDigit))
            {
                MessageBox.Show("La contraseña debe contener al menos un digito!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtContraseña.Focus();
                return true;
            }
            if (txtContraseña.Password.Contains(" "))
            {
                MessageBox.Show("La contraseña no debe contener espacios en blanco!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
                txtContraseña.Focus();
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
                DAuditoria auditoria = new DAuditoria(
                    idTrabajador,
                    "Editar",
                    "Ha Restaurado su Contraseña"
                 );
                new LAuditoria().Insertar(auditoria);

                LFunction.MessageExecutor("Information", "Contraseña cambiada correctamente, regresando al Login");
            }
            else
            {
                LFunction.MessageExecutor("Information", respuesta);
            }

            this.Hide();
        }

    }
}
