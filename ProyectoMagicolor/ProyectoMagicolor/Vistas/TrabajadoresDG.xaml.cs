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

    public partial class TrabajadoresDG : Page
    {

        public LTrabajador MetodosUsuario = new LTrabajador();

        public TrabajadoresDG()
        {
            InitializeComponent();
        }
        

        public void Refresh(string search, string search2)
        {

            List<DTrabajador> items = MetodosUsuario.Mostrar((search + "-" + search2));

            List<TablaTrabajadores> tablaTrabajadores = new List<TablaTrabajadores>();

            foreach (DTrabajador it in items)
            {
                tablaTrabajadores.Add(new TablaTrabajadores(it.idTrabajador, it.cedula, it.nombre, it.apellidos, it.direccion, it.telefono, it.email));
            }

            dgOperaciones.ItemsSource = tablaTrabajadores;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
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
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Resp = MessageBox.Show("¿Seguro que quieres Eliminar este Trabajador?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Resp != MessageBoxResult.Yes)
                return;
            int id = (int)((Button)sender).CommandParameter;
            string cedula = dgOperaciones.Items[1].ToString();
            MetodosUsuario.Eliminar(id);
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Eliminar",
                "Ha Eliminado el Tarabajador " + cedula
            );
            new LAuditoria().Insertar(auditoria);
        }

        private void CbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbTipoDocumento.SelectedIndex > -1)
                PlaceTipoDocumento.Text = "";
            else
                PlaceTipoDocumento.Text = "Tipo";

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
            reporte.ExportPDF(MetodosUsuario.Mostrar((CbTipoDocumento.Text + "-" + txtDocumento.Text)), "Trabajador");

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Generar",
                "Ha Generado un Reporte de Trabajadores"
            );
            new LAuditoria().Insertar(auditoria);
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtDocumento.Text == "")
            {
                txtBucarPlaceH.Text = "";
            }

        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDocumento.Text == "")
            {
                txtBucarPlaceH.Text = "Ingresar Cédula del Trabajador";
            }

        }
    }

    public class TablaTrabajadores
    {
        public TablaTrabajadores(int id, string cedula, string nombre, string apellidos, string direccion, string telefono, string email)
        {
            Id = id;
            Cedula = cedula;
            Nombre = nombre;
            Apellidos = apellidos;
            Direccion = direccion;
            Telefono = telefono;
            Email = email;
        }

        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }

}
