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
    /// <summary>
    /// Interaction logic for FTrabajadores.xaml
    /// </summary>
    public partial class ProveedorVista : Window
    {

        public CompraFrm ParentForm;

        public LProveedor Metodos = new LProveedor();

        public ProveedorVista(CompraFrm parent)
        {
            InitializeComponent();
            ParentForm = parent;
        }
        

        public void Refresh(string search)
        {

            List<DProveedor> items = Metodos.Mostrar(search);

            foreach(DProveedor item in items)
            {
                item.numeroDocumento = item.tipoDocumento + "-" + item.numeroDocumento;
            }


            dgOperaciones.ItemsSource = items;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //contentsp.Children.Clear();

            Refresh(txtBuscar.Text);
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    ProveedorFrm frmTrab = new ProveedorFrm();
        //    bool Resp = frmTrab.ShowDialog() ?? false;
        //    Refresh(txtBuscar.Text);
        //}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.Encontrar(id)[0];

            DialogResult = true;
            ParentForm.AgregarProveedor(response);
            this.Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(txtBuscar.Text);
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtBuscar.Text == "")
            {
               txtBucarPlaceH.Text = "";
            }
            
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtBuscar.Text == "")
            {
                txtBucarPlaceH.Text = "Buscar...";
            }
            
        }
    }

}
