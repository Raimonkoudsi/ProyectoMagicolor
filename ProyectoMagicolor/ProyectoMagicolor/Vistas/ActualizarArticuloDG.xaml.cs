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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class ActualizarArticuloDG : Page
    {
        public LArticulo Metodos = new LArticulo();


        public MainWindow Parent;

        public ActualizarArticuloDG(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;
        }


        public void Refresh(string Busqueda ,int TipoBusqueda)
        {
            string busqueda = Busqueda == "" ? "-" : Busqueda;

            List<DArticulo> items = Metodos.ActualizarArticulo(busqueda, TipoBusqueda);
            dgOperaciones.ItemsSource = items;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);
        }

        private string CambiarBusqueda()
        {
            return CbTipoBusqueda.SelectedIndex == 0 ? "Codigo" :
               CbTipoBusqueda.SelectedIndex == 1 ? "Nombre" : "";
        }
        private void txtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);
        }


        private void CbUsuario_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtBusqueda.Text = "";

            Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.EncontrarActualizarArticulo(id);

            ArticuloFrm frm = new ArticuloFrm();
            frm.Type = TypeForm.Update;
            frm.DataFill = response[0];
            bool Resp = frm.ShowDialog() ?? false;
            Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);
        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.EncontrarActualizarArticulo(id);

            ArticuloFrm frmTrab = new ArticuloFrm();
            frmTrab.Type = TypeForm.Read;
            frmTrab.DataFill = response[0];
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Resp = MessageBox.Show("¿Seguro que quiere anular este artículo?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Resp != MessageBoxResult.Yes)
                return;
            int id = (int)((Button)sender).CommandParameter;
            string codigo = dgOperaciones.Items[1].ToString();
            Metodos.Eliminar(id);
            Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Eliminar",
                "Ha Eliminado el Artículo Código " + codigo
             );
            new LAuditoria().Insertar(auditoria);
        }

        private void CbTipoBusqueda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);
        }
    }
}
