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
using System.Windows.Media.Animation;

using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    /// <summary>
    /// Interaction logic for FTrabajadores.xaml
    /// </summary>
    public partial class InventarioDG : Page
    {

        public LArticulo Metodos = new LArticulo();

        public InventarioDG()
        {
            InitializeComponent();
        }
        

        public void Refresh()
        {

            //List<DArticulo> items = Metodos.Mostrar("");


            //dgOperaciones.ItemsSource = items;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //contentsp.Children.Clear();

            Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (dgOperaciones.SelectedItems.Count > 0)
            //{
            //    for(int i = 0; i < dgOperaciones.SelectedItems.Count; i++)
            //    {
            //        MessageBox.Show(((TablaTrabajadores)dgOperaciones.SelectedItems[i]).Nombre);
            //    }
            //}
            //else
            //    MessageBox.Show("no hay");
            ArticuloFrm frmTrab = new ArticuloFrm();
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.Encontrar(id);

            ArticuloFrm frm = new ArticuloFrm();
            frm.Type = TypeForm.Update;
            frm.DataFill = response[0];
            bool Resp = frm.ShowDialog() ?? false;
            Refresh();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Resp = MessageBox.Show("¿Seguro que quieres eliminrar este item?", "Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Resp != MessageBoxResult.Yes)
                return;
            int id = (int)((Button)sender).CommandParameter;
            DArticulo item = new DArticulo()
            {
                idArticulo = id
            };
            Metodos.Eliminar(item);
            Refresh();
        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.Encontrar(id);

            ArticuloFrm frmTrab = new ArticuloFrm();
            frmTrab.Type = TypeForm.Read;
            frmTrab.DataFill = response[0];
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh();
            //MessageBox.Show(response[0].fechaNacimiento.ToString());
        }

        bool isAnimationCurrent = false;

        double AnimationDuration = .3;

        private void btnOpenFilters_Click(object sender, RoutedEventArgs e)
        {
            if (isAnimationCurrent)
                return;

            GridFilters.Width = 0;
            SideBar.Visibility = Visibility.Visible;

            var easing = new CircleEase();
            //easing.EasingMode = EasingMode.EaseOut;

            DoubleAnimation DAGrid = new DoubleAnimation();
            DAGrid.From = 0;
            DAGrid.To = 250;
            DAGrid.EasingFunction = easing;
            DAGrid.Duration = TimeSpan.FromSeconds(AnimationDuration);

            DAGrid.Completed += (s, r) =>
            {
                isAnimationCurrent = false;
            };

            GridFilters.BeginAnimation(WidthProperty, DAGrid);

            DoubleAnimation BlackDA = new DoubleAnimation();
            BlackDA.From = 0;
            BlackDA.To = 0.7f;
            BlackDA.EasingFunction = easing;
            BlackDA.Duration = TimeSpan.FromSeconds(AnimationDuration);

            BlackPanel.BeginAnimation(OpacityProperty, BlackDA);

            isAnimationCurrent = true;

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //SideBar.Visibility = Visibility.Collapsed;

            if (isAnimationCurrent)
                return;

            var easing = new CircleEase();
            //easing.EasingMode = EasingMode.EaseOut;

            DoubleAnimation DAGrid = new DoubleAnimation();
            DAGrid.From = 250;
            DAGrid.To = 0;
            DAGrid.EasingFunction = easing;
            DAGrid.Duration = TimeSpan.FromSeconds(AnimationDuration);

            DAGrid.Completed += (s, r) =>
            {
                SideBar.Visibility = Visibility.Collapsed;
                isAnimationCurrent = false;
            };


            GridFilters.BeginAnimation(WidthProperty, DAGrid);

            DoubleAnimation BlackDA = new DoubleAnimation();
            BlackDA.From = 0.7f;
            BlackDA.To = 0;
            BlackDA.EasingFunction = easing;
            BlackDA.Duration = TimeSpan.FromSeconds(AnimationDuration);

            BlackPanel.BeginAnimation(OpacityProperty, BlackDA);
        }
    }

}
