using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class CategoriaDG : Page
    {

        public LCategoria Metodos = new LCategoria();

        public CategoriaDG()
        {
            InitializeComponent();
        }
        

        public void Refresh(string search)
        {

            List<DCategoria> items = Metodos.Mostrar(search);

            dgOperaciones.ItemsSource = items;


            if (items.Count == 0)
                SinRegistro.Visibility = Visibility.Visible;
            else
                SinRegistro.Visibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBuscar.Text = "";
            txtBuscar.Focus();

            Refresh(txtBuscar.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CategoriaFrm frmTrab = new CategoriaFrm();
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(txtBuscar.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.Encontrar(id);

            CategoriaFrm frm = new CategoriaFrm();
            frm.Type = TypeForm.Update;
            frm.DataFill = response[0];
            bool Resp = frm.ShowDialog() ?? false;
            Refresh(txtBuscar.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(txtBuscar.Text);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Resp = MessageBox.Show("¿Seguro que quiere deshabilitar esta Categoría?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Resp != MessageBoxResult.Yes)
                return;
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.Encontrar(id);

            string codigo = response[0].nombre;

            Metodos.Eliminar(id);
            Refresh(txtBuscar.Text);

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Deshabilitar",
                "Ha Deshabilitado la Categoría " + codigo
            );
            new LAuditoria().Insertar(auditoria);
        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.Encontrar(id);

            CategoriaFrm frmTrab = new CategoriaFrm();
            frmTrab.Type = TypeForm.Read;
            frmTrab.DataFill = response[0];
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(txtBuscar.Text);
        }
    }

}
