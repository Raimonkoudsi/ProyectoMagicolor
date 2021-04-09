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

            ChBAlfabeticoOrdenar.IsChecked = true;
            CbColumnas.SelectedIndex = 0;
        }
        

        public void Refresh()
        {

            int TipoBuscarPor = 1;

            DateTime FechaInicio = DateTime.Now;
            DateTime FechaFinal = DateTime.Now;

            if (RBDiaBuscar.IsChecked ?? false)
                TipoBuscarPor = 1;
            else if (RBSemanaBuscar.IsChecked ?? false)
                TipoBuscarPor = 2;
            else if (RBMesBuscar.IsChecked ?? false)
                TipoBuscarPor = 3;
            else if (RBAñoBuscar.IsChecked ?? false)
                TipoBuscarPor = 4;
            else if (RBFechaBuscar.IsChecked ?? false)
            {
                //entre 2 Fechas
                if(ChBEntreFechas.IsChecked ?? false)
                {
                    if(DpFechaInicio.SelectedDate != null && DpFechaFinal.SelectedDate != null)
                    {
                        FechaInicio = DpFechaInicio.SelectedDate ?? DateTime.Now;
                        FechaFinal = DpFechaFinal.SelectedDate ?? DateTime.Now;
                        TipoBuscarPor = 6;
                    }
                    else
                    {
                        //Mensaje de Error
                    }
                }
                //Solo 1 Fecha
                else
                {
                    if(DpFechaInicio.SelectedDate != null)
                    {
                        FechaInicio = DpFechaInicio.SelectedDate ?? DateTime.Now;
                        TipoBuscarPor = 5;
                    }
                    else
                    {
                        //mensaje de Error
                    }
                }
            }

            int TipoMostrar = 1;

            if (RBStockMostrar.IsChecked ?? false)
                TipoMostrar = 2;
            else if (RBSinStockMostrar.IsChecked ?? false)
                TipoMostrar = 3;



            int TipoOrdenar = 1;

            if (ChBAlfabeticoOrdenar.IsChecked ?? false)
            {
                //Ordenar por Articulos
                if (CbColumnas.SelectedIndex == 0)
                    TipoOrdenar = 1;
                //Ordenar por Categorías
                else if (CbColumnas.SelectedIndex == 1)
                    TipoOrdenar = 2;
                else
                {
                    //mensaje de Error
                }
            }
            else if (ChBMayoresVentasOrdenar.IsChecked ?? false)
                TipoOrdenar = 3;
            else if (ChBMayorStockOrdenar.IsChecked ?? false)
                TipoOrdenar = 4;


            List<DArticulo> items = Metodos.Inventario(TipoBuscarPor, FechaInicio, FechaFinal, TipoMostrar, TipoOrdenar);


            dgOperaciones.ItemsSource = items;
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

        void OpenSidebar()
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

        void CloseSidebar()
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

        private void btnOpenFilters_Click(object sender, RoutedEventArgs e)
        {
            OpenSidebar();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseSidebar();   
        }

        private void RBFechaBuscar_Checked(object sender, RoutedEventArgs e)
        {
            if (RBFechaBuscar.IsChecked ?? false)
            {
                FechasPanel.Visibility = Visibility.Visible;
            }
            else
            {
                FechasPanel.Visibility = Visibility.Collapsed;

            }
        }

        private void ChBEntreFechas_Checked(object sender, RoutedEventArgs e)
        {
            if (ChBEntreFechas.IsChecked ?? false)
            {
                DpFechaFinal.IsEnabled = true;
            }
            else
            {
                DpFechaFinal.IsEnabled = false;

            }
        }

        private void ChBAlfabeticoOrdenar_Checked(object sender, RoutedEventArgs e)
        {
            if (ChBAlfabeticoOrdenar.IsChecked ?? false)
            {
                ValoresPanel.Visibility = Visibility.Visible;
            }
            else
            {
                ValoresPanel.Visibility = Visibility.Collapsed;

            }
        }

        private void DpFechaInicio_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DpFechaInicio.SelectedDate != null)
            {
                DpFechaInicioPlace.Text = "";
                DpFechaFinal.DisplayDateStart = DpFechaInicio.SelectedDate?.Date.AddDays(1);
            }
            else
            {
                DpFechaInicioPlace.Text = "Fecha Inicio";
                DpFechaFinal.DisplayDateStart = null;
            }
        }

        private void DpFechaFinal_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DpFechaFinal.SelectedDate != null)
            {
                DpFechaFinalPlace.Text = "";

                DpFechaInicio.DisplayDateEnd = DpFechaFinal.SelectedDate?.Date.AddDays(-1);
            }
            else
            {
                DpFechaFinalPlace.Text = "Fecha Final";

                DpFechaInicio.DisplayDateEnd = null;

            }
        }

        private void CbColumnas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbColumnas.SelectedIndex > -1)
                CbColumnasPlaceholder.Text = "";
            else
                CbColumnasPlaceholder.Text = "Columnas";
        }

        private void BtnAplicarFiltro_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            CloseSidebar();
        }
    }

}
