﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class Login : Window
    {
        List<DTrabajador> respuesta = new List<DTrabajador>();
        LTrabajador metodosUsuario = new LTrabajador();


        public static Random randComp;
        public static int idSecretQ, idSecretQ2;

        public Login()
        {
            InitializeComponent();

            txtContraseña.Foreground = System.Windows.Media.Brushes.White;

            if (randComp == null)
            {
                randComp = new Random(DateTime.Now.Second + DateTime.Now.Millisecond);
                idSecretQ = randComp.Next(0, 3);
                idSecretQ2 = randComp.Next(0, 3);

                while(idSecretQ == idSecretQ2)
                    idSecretQ2 = randComp.Next(0, 3);
            }
        }



        private void Loging()
        {

                if (txtUsuario.Text == "")
                {
                    LFunction.MessageExecutor("Error", "Debe ingresar un nombre de Usuario");
                    txtUsuario.Focus();
                    return;
                }
                if (txtContraseña.Password == "")
                {
                    LFunction.MessageExecutor("Error", "Debe ingresar la Contraseña");
                    txtContraseña.Focus();
                    return;
                }

                if (metodosUsuario.UsuarioAnulado(txtUsuario.Text.ToLower(), txtContraseña.Password))
                {
                    LFunction.MessageExecutor("Error", "El Usuario está deshabilitado, cerrando el Sistema");
                    Environment.Exit(0);
                }
                else
                {
                    respuesta = metodosUsuario.Login(txtUsuario.Text.ToLower(), txtContraseña.Password);

                    if (respuesta.Count > 0)
                    {
                        var MainFrm = new MainWindow(respuesta[0]);
                        intentos = 0;
                        this.Hide();
                        MainFrm.Show();

                        VistaPrincipal Frm = new VistaPrincipal(MainFrm);
                        MainFrm.SwitchScreen(Frm);

                        MainFrm.Closing += (s, r) => { this.Close(); };
                    }
                    else
                    {
                        ContadorIntentos();
                    }
                }
        }

        private static int intentos;
        public static void ContadorIntentos()
        {
            intentos++;

            if (intentos == 1)
                LFunction.MessageExecutor("Error", "Los Datos son Incorrectos (primer intento)");
            else if (intentos == 2)
                LFunction.MessageExecutor("Error", "Los Datos son Incorrectos (segundo intento)");
            else if (intentos == 3)
            {
                LFunction.MessageExecutor("Error", "Los Datos son Incorrectos (tercer intento), se cerrará el programa");
                Environment.Exit(0);
            }
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
             Loging();
        }

        private void cerrar_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
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
            else if (metodosUsuario.UsuarioAnulado(txtUsuario.Text, txtContraseña.Password))
            {
                LFunction.MessageExecutor("Error", "El Usuario está deshabilitado, cerrando el Sistema");
                Environment.Exit(0);
            }
            else
            {
                respuesta = metodosUsuario.Seguridad(txtUsuario.Text);

                if (respuesta.Count > 0)
                {
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
