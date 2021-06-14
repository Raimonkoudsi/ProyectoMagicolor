using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class ClienteDG : Page
    {
        public LCliente Metodos = new LCliente();

        List<DCliente> items = new List<DCliente>();

        public ClienteDG()
        {
            InitializeComponent();

            txtDocumento.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
        }
        

        public void Refresh(string typeSearch, string search)
        {

            items = Metodos.Mostrar(typeSearch, search, TipoEstadoBusqueda());

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


            CbTipoDocumento.SelectedIndex = 0;
            txtDocumento.Focus();

            Refresh(CbTipoDocumento.Text, txtDocumento.Text);


            if (Globals.ACCESO_SISTEMA != 0)
            {
                btnReport.ToolTip = "Sólo el Administrador puede Generar Reportes";
                btnReport.IsEnabled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClienteFrm frmTrab = new ClienteFrm();
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.Encontrar(id);

            ClienteFrm frm = new ClienteFrm();
            frm.Type = TypeForm.Update;
            frm.DataFill = response[0];

            if (response[0].estado == 0)
            {
                MessageBoxResult RespHab = MessageBox.Show("¿Desea habilitar el Cliente?" + Environment.NewLine + "(Abrirá el formulario para editar datos relevantes)", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (RespHab == MessageBoxResult.No)
                    return;
            }

            bool Resp = frm.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Resp = MessageBox.Show("¿Seguro que quiere deshabilitar este Proveedor?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Resp != MessageBoxResult.Yes)
                return;
            int id = (int)((Button)sender).CommandParameter;
            string cedula = dgOperaciones.Items[1].ToString();
            Metodos.Eliminar(id);
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);

            DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Deshabilitar",
                    "Ha Deshabilitado el Cliente " + cedula
                );
            new LAuditoria().Insertar(auditoria);

        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.Encontrar(id);

            ClienteFrm frmTrab = new ClienteFrm();
            frmTrab.Type = TypeForm.Read;
            frmTrab.DataFill = response[0];
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);

        }

        private void CbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var tipoDoc = CbTipoDocumento.SelectedIndex == 0 ? "V" :
                            CbTipoDocumento.SelectedIndex == 1 ? "E" :
                            CbTipoDocumento.SelectedIndex == 2 ? "J" :
                            CbTipoDocumento.SelectedIndex == 3 ? "G" : "";

            Refresh(tipoDoc, txtDocumento.Text);
        }


        private void Reporte_Click(object sender, RoutedEventArgs e)
        {
            if (dgOperaciones.Items.Count == 0)
            {
                LFunction.MessageExecutor("Error", "No se puede realizar un Reporte vacio!");
                return;
            }

            Reports.Reporte reporte = new Reports.Reporte();
            reporte.ExportPDF(Metodos.Mostrar(CbTipoDocumento.Text, txtDocumento.Text, TipoEstadoBusqueda()), "Cliente");

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Generar",
                "Ha Generado el Reporte de Clientes"
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

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void RBHabilitado_Click(object sender, RoutedEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }
    }

}
