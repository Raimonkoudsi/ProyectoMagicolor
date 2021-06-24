using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class ProveedorDG : Page
    {

        public LProveedor Metodos = new LProveedor();

        List<DProveedor> items = new List<DProveedor>();

        public ProveedorDG()
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

            CbTipoDocumento.SelectedIndex = 2;

            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProveedorFrm frmTrab = new ProveedorFrm();
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            List<DProveedor> response = Metodos.Encontrar(id);

            ProveedorFrm frm = new ProveedorFrm();
            frm.Type = TypeForm.Update;
            frm.DataFill = response[0];

            if(response[0].estado == 0)
            {
                MessageBoxResult RespHab = MessageBox.Show("¿Desea habilitar el Proveedor?" + Environment.NewLine + "(Abrirá el formulario para editar datos relevantes)", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
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
                "Ha Deshabilitado el Proveedor " + cedula
            );
            new LAuditoria().Insertar(auditoria);
        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            List<DProveedor> response = Metodos.Encontrar(id);

            ProveedorFrm frmTrab = new ProveedorFrm();
            frmTrab.Type = TypeForm.Read;
            frmTrab.DataFill = response[0];
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);

        }

        private void CbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipoDoc = CbTipoDocumento.SelectedIndex == 0 ? "V" :
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
            reporte.ExportPDF(Metodos.Mostrar(CbTipoDocumento.Text, txtDocumento.Text, TipoEstadoBusqueda()), "Proveedor");

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Generar",
                "Ha Generado un Reporte de Proveedores"
            );
            new LAuditoria().Insertar(auditoria);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(CbTipoDocumento.Text, txtDocumento.Text);
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




    public class DesactivateButtonNull : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            int estado = int.Parse(values[0].ToString());

            int acceso = int.Parse(values[1].ToString());

            if (estado == 0)
                return false;
            if (estado != 0 && acceso == 0)
                return true;

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class ChangeRedColorRowNull : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Transparent;

            int estado = int.Parse(value.ToString());

            if (estado == 0)
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
            }
            else
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("Black"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class ChangeEditButtonNull : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int estado = int.Parse(value.ToString());

            if (estado == 0)
            {
                return "AccountReactivate";
            }
            else
            {
                return "PencilOutline";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class ChangeEditTextButtonNull : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int estado = int.Parse(value.ToString());

            if (estado == 0)
            {
                return "Reactivar";
            }
            else
            {
                return "Editar";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class ChangeEditAccessButtonNull : IMultiValueConverter
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
