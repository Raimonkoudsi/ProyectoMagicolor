﻿using System;
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
    public partial class Login : Window
    {

        public Login()
        {
            InitializeComponent();

            txtUsuario.BorderBrush = System.Windows.Media.Brushes.LightGray;
            txtContraseña.BorderBrush = System.Windows.Media.Brushes.LightGray;
            txtUsuario.Foreground = System.Windows.Media.Brushes.LightGray;
            txtContraseña.Foreground = System.Windows.Media.Brushes.LightGray;
        }



        void Loging()
        {

            if (txtUsuario.Text == "")
            {
                System.Windows.MessageBox.Show("Debes poner un Nombre de Usuario!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
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

                var respuesta = metodosUsuario.Login(txtUsuario.Text, txtContraseña.Password);

                if (respuesta.Count > 0)
                {
                    var MainFrm = new MainWindow(respuesta[0]);

                    this.Hide();
                    MainFrm.Show();
                    MainFrm.Closing += (s, r) => { this.Close(); };
                }
                else
                {
                    ContadorIntentos();
                }
            }
            catch
            {

            }
        }

        private static int intentos;
        public static void ContadorIntentos()
        {
            intentos++;

            if (intentos < 3)
            {
                LFunction.MessageExecutor("Error", "Los Datos son Incorrectos (" + intentos + " intento)");
            }
            else if (intentos == 3)
            {
                LFunction.MessageExecutor("Error", "Los Datos son Incorrectos (" + intentos + " intento), se cerrará el programa");
                Environment.Exit(0);
            }
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            Loging();
        }

        private void cerrar_Click(object sender, RoutedEventArgs e)
        {
            var resp = MessageBox.Show("¿Desea Cerrar la Aplicación?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (resp == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void txtContraseña_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtContraseña.Password == "")
            {
                txtContraeñaPlace.Text = "";
            }

            txtContraseña.BorderBrush = System.Windows.Media.Brushes.White;
            txtContraseña.Foreground = System.Windows.Media.Brushes.White;
        }

        private void txtContraseña_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtContraseña.Password == "")
            {
                txtContraeñaPlace.Text = "Contraseña";
            }

            txtContraseña.BorderBrush = System.Windows.Media.Brushes.LightGray;
            txtContraseña.Foreground = System.Windows.Media.Brushes.LightGray;
        }

        private void txtUsuario_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsuario.Text == "")
            {
                txtUsuarioPlace.Text = "";
            }

            txtUsuario.BorderBrush = System.Windows.Media.Brushes.White;
            txtUsuario.Foreground = System.Windows.Media.Brushes.White;
        }

        private void txtUsuario_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsuario.Text == "")
            {
                txtUsuarioPlace.Text = "Usuario";
            }

            txtUsuario.BorderBrush = System.Windows.Media.Brushes.LightGray;
            txtUsuario.Foreground = System.Windows.Media.Brushes.LightGray;
        }

        private void StackPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Loging();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Preguntas_Click(object sender, RoutedEventArgs e)
        {
            if(txtUsuario.Text == "")
            {
                LFunction.MessageExecutor("Information", "Debe Proporcionar un Nombre de Usuario en el Login");
                txtUsuario.Focus();
            }
            else
            {
                LTrabajador metodosUsuario = new LTrabajador();

                var respuesta = metodosUsuario.Seguridad(txtUsuario.Text);

                if (respuesta.Count > 0)
                {
                    //var MainFrm = new PreguntasSeguridad(respuesta);
                    PreguntasSeguridad frmPreguntas = new PreguntasSeguridad(this);
                    frmPreguntas.DataFill = respuesta;
                    bool Resp = frmPreguntas.ShowDialog() ?? false;
                }
                else
                {
                    LFunction.MessageExecutor("Information", "El Usuario no está Registrado");
                }
            }
        }
    }
}
