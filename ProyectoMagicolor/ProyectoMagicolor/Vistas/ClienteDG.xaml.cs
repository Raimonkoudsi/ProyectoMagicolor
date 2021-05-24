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

namespace ProyectoMagicolor.Vistas
{
    public partial class ClienteDG : Page
    {

        public LCliente Metodos = new LCliente();

        public ClienteDG()
        {
            InitializeComponent();
        }
        

        public void Refresh(string typeSearch, string search)
        {

            List<DCliente> items = Metodos.Mostrar(typeSearch, search);

            foreach(DCliente item in items)
            {
                item.numeroDocumento = item.tipoDocumento + "-" + item.numeroDocumento;
            }

            dgOperaciones.ItemsSource = items;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.ACCESO_SISTEMA != 0)
            {
                btnReport.ToolTip = "Sólo el Administrador puede Generar Reportes";
                btnReport.IsEnabled = false;
            }

            CbTipoDocumento.SelectedIndex = 0;
            Refresh(CbTipoDocumento.Text ,txtDocumento.Text);
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
            bool Resp = frm.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.ACCESO_SISTEMA == 2)
            {
                LFunction.MessageExecutor("Information", "Los Vendedores no pueden Eliminar Clientes!");
            }
            else
            {
                MessageBoxResult Resp = MessageBox.Show("¿Seguro que quieres Eliminar el Cliente?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (Resp != MessageBoxResult.Yes)
                    return;
                int id = (int)((Button)sender).CommandParameter;
                string cedula = dgOperaciones.Items[1].ToString();
                Metodos.Eliminar(id);
                Refresh(CbTipoDocumento.Text, txtDocumento.Text);

                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Eliminar",
                    "Ha Eliminado el Cliente " + cedula
                );
                new LAuditoria().Insertar(auditoria);
            }
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtDocumento.Text == "")
            {
                txtBucarPlaceH.Text = "";
            }
            
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtDocumento.Text == "")
            {
                txtBucarPlaceH.Text = "Documento";
            }
            
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
            if (CbTipoDocumento.SelectedIndex > -1)
                PlaceTipoDocumento.Text = "";
            else
                PlaceTipoDocumento.Text = "Tipo";

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
            reporte.ExportPDF(Metodos.Mostrar(CbTipoDocumento.Text, txtDocumento.Text), "Cliente");

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Generar",
                "Ha Generado el Reporte de Clientes"
            );
            new LAuditoria().Insertar(auditoria);
        }
    }

}
