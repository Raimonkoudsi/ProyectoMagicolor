using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{

    public partial class TrabajadoresDG : Page
    {

        public LTrabajador MetodosUsuario = new LTrabajador();

        List<DTrabajador> items = new List<DTrabajador>();

        public TrabajadoresDG()
        {
            InitializeComponent();
        }
        

        public void Refresh(string search, string search2)
        {

            items = MetodosUsuario.Mostrar((search + "-" + search2), TipoEstadoBusqueda());

            dgOperaciones.ItemsSource = items;


            if (items.Count == 0)
            {
                btnReport.IsEnabled = false;
                SinRegistro.Visibility = Visibility.Visible;
            }
            else
            {
                btnReport.IsEnabled = true;
                SinRegistro.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);

            CbTipoDocumento.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TrabajadoresFrm frmTrab = new TrabajadoresFrm();
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = MetodosUsuario.Encontrar(id);
            var responseSecurity = MetodosUsuario.EncontrarSeguridad(id);

            TrabajadoresFrm frmTrab = new TrabajadoresFrm();
            frmTrab.Type = TypeForm.Update;
            frmTrab.ListaSeguridad = responseSecurity;
            frmTrab.DataFill = response[0];

            if (response[0].estado == 0)
            {
                MessageBoxResult RespHab = MessageBox.Show("¿Desea habilitar el Trabajador?" + Environment.NewLine + "(Abrirá el formulario para editar datos relevantes)", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (RespHab == MessageBoxResult.No)
                    return;
            }

            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Resp = MessageBox.Show("¿Seguro que quiere Deshabilitar este Trabajador?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Resp != MessageBoxResult.Yes)
                return;
            int id = (int)((Button)sender).CommandParameter;
            string cedula = dgOperaciones.Items[1].ToString();
            MetodosUsuario.Eliminar(id);
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Deshabilitar",
                "Ha Deshabilitado el Tarabajador " + cedula
            );
            new LAuditoria().Insertar(auditoria);
        }

        private void CbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var tipoDoc = CbTipoDocumento.SelectedIndex == 0 ? "V" :
                            CbTipoDocumento.SelectedIndex == 1 ? "E" : "";

            Refresh(tipoDoc, txtDocumento.Text);
        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = MetodosUsuario.Encontrar(id);
            var responseSecurity = MetodosUsuario.EncontrarSeguridad(id);

            TrabajadoresFrm frmTrab = new TrabajadoresFrm();
            frmTrab.Type = TypeForm.Read;
            frmTrab.ListaSeguridad = responseSecurity;
            frmTrab.DataFill = response[0];
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void Reporte_Click(object sender, RoutedEventArgs e)
        {
            if (dgOperaciones.Items.Count == 0)
            {
                LFunction.MessageExecutor("Error", "No se puede realizar un Reporte vacio!");
                return;
            }

            Reports.Reporte reporte = new Reports.Reporte();
            reporte.ExportPDF(MetodosUsuario.Mostrar((CbTipoDocumento.Text + "-" + txtDocumento.Text), TipoEstadoBusqueda()), "Trabajador");

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Generar",
                "Ha Generado un Reporte de Trabajadores"
            );
            new LAuditoria().Insertar(auditoria);
        }

        private int TipoEstadoBusqueda()
        {
            if (RBHabilitado.IsChecked == true && RBDeshabilitado.IsChecked == true)
                return 3;
            if (RBHabilitado.IsChecked == true && RBDeshabilitado.IsChecked == false)
                return 1;
            if (RBHabilitado.IsChecked == false && RBDeshabilitado.IsChecked == true)
                return 2;

            return 0;
        }

        private void RBHabilitado_Click(object sender, RoutedEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }
    }

}
