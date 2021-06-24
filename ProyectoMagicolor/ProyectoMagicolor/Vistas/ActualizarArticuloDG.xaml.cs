using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Globalization;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class ActualizarArticuloDG : Page
    {
        public LArticulo Metodos = new LArticulo();

        public new MainWindow Parent;

        public ActualizarArticuloDG(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;
        }


        public void Refresh(string Busqueda ,int TipoBusqueda)
        {
            string busqueda = Busqueda == "" ? "-" : Busqueda;

            List<DArticulo> items = Metodos.ActualizarArticulo(busqueda, TipoBusqueda, TipoEstadoBusqueda());
            dgOperaciones.ItemsSource = items;

            if (items.Count == 0)
            {
                btnReport.IsEnabled = false;
                SinRegistro.Visibility = Visibility.Visible;
            }
            else
            {
                SinRegistro.Visibility = Visibility.Collapsed;
            }

            if (Globals.ACCESO_SISTEMA == 0 && items.Count != 0)
            {
                btnReport.IsEnabled = true;
            }
            else if (Globals.ACCESO_SISTEMA != 0)
            {
                btnReport.IsEnabled = false;
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CbTipoBusqueda.SelectedIndex = 0;
            txtBusqueda.Text = "";
            txtBusqueda.Focus();

            Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.EncontrarActualizarArticulo(id);

            ArticuloFrm frm = new ArticuloFrm();
            frm.Type = TypeForm.Update;
            frm.DataFill = response[0];

            if (response[0].estado == 0)
            {
                MessageBoxResult RespHab = MessageBox.Show("¿Desea habilitar el Artículo?" + Environment.NewLine + "(Abrirá el formulario para editar datos relevantes)", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (RespHab == MessageBoxResult.No)
                    return;
            }

            bool Resp = frm.ShowDialog() ?? false;
            Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);
        }


        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            string respuesta = "";

            MessageBoxResult Resp = MessageBox.Show("¿Seguro que quiere deshabilitar este artículo?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Resp != MessageBoxResult.Yes)
                return;
            int id = (int)((Button)sender).CommandParameter;
            string codigo = dgOperaciones.Items[1].ToString();

            respuesta = Metodos.DeshabilitarArticulo(id, "Parcial");

            if (respuesta.Equals("OK"))
            {
                LFunction.MessageExecutor("Information", "Artículo Deshabilitado Correctamente");

                Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);

                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Deshabilitar",
                    "Ha Deshabilitado el Artículo Código " + codigo
                 );
                new LAuditoria().Insertar(auditoria);
            }
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


        private void Reporte_Click(object sender, RoutedEventArgs e)
        {
            string busqueda = txtBusqueda.Text == "" ? "-" : txtBusqueda.Text;

            if (dgOperaciones.Items.Count == 0)
            {
                LFunction.MessageExecutor("Error", "No se puede realizar un Reporte vacio!");
                return;
            }

            Reports.Reporte reporte = new Reports.Reporte();
            reporte.ExportPDF(Metodos.ActualizarArticulo(busqueda, CbTipoBusqueda.SelectedIndex, TipoEstadoBusqueda()), "Articulo");

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Generar",
                "Ha Generado el Reporte de Clientes"
            );
            new LAuditoria().Insertar(auditoria);
        }


        private void txtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);
        }


        private void RBHabilitado_Click(object sender, RoutedEventArgs e)
        {
            Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);
        }


        private void CbTipoBusqueda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh(txtBusqueda.Text, CbTipoBusqueda.SelectedIndex);
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
    }

    public class DesactivateButtonNullArticlee : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            int estado = int.Parse(values[0].ToString());

            int acceso = int.Parse(values[1].ToString());

            if (estado == 0)
                return false;
            if (estado == 1 && acceso == 0)
                return true;

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class ChangeEditAccessButtonNullArticlee : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            int estado = int.Parse(values[0].ToString());

            int acceso = int.Parse(values[1].ToString());

            if (estado == 0 && acceso == 0)
            {
                return true;
            }
            else if (estado == 0 && acceso != 0)
            {
                return false;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
