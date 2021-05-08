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
using System.Globalization;

namespace ProyectoMagicolor.Vistas
{

    public class BooleanToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Transparent;

            int idArticulo = int.Parse(value.ToString());

            var resp = new LArticulo().SacarArticulo(idArticulo)[0];

            if(resp.cantidadActual < resp.stockMinimo)
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFD8D8"));
            }
            else
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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

        private int TipoBuscarPor = 1;
        private DateTime FechaInicio = DateTime.Now;
        private DateTime FechaFinal = DateTime.Now;
        private int TipoMostrar = 1;
        private int TipoOrdenar = 1;

        public void Refresh()
        {

            TipoBuscarPor = 1;

            FechaInicio = DateTime.Now;
            FechaFinal = DateTime.Now;

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
                        MessageBox.Show("Debes seleccionar las dos fechas!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                        DpFechaInicio.Focus();
                        return;
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
                        MessageBox.Show("Debes seleccionar una fecha!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                        DpFechaInicio.Focus();
                        return;
                    }
                }
            }

            TipoMostrar = 1;

            if (RBStockMostrar.IsChecked ?? false) TipoMostrar = 2;
            else if (RBSinStockMostrar.IsChecked ?? false) TipoMostrar = 3;

            TipoOrdenar = 1;

            if (ChBAlfabeticoOrdenar.IsChecked ?? false)
            {
                //Ordenar por Articulos
                if (CbColumnas.SelectedIndex == 0) TipoOrdenar = 1;
                //Ordenar por Categorías
                else if (CbColumnas.SelectedIndex == 1) TipoOrdenar = 2;
                else
                {
                    MessageBox.Show("Debes seleccionar una columna!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                    CbColumnas.Focus();
                    return;
                }
            }
            else if (ChBMayoresVentasOrdenar.IsChecked ?? false) TipoOrdenar = 3;
            else if (ChBMayorStockOrdenar.IsChecked ?? false) TipoOrdenar = 4;

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

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var response = Metodos.DetalleInventario(id)[0];

            InventarioVista vista = new InventarioVista(response);
            vista.ShowDialog();
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

        private void Reporte_Click(object sender, RoutedEventArgs e)
        {
            if (dgOperaciones.Items.Count == 0)
            {
                LFunction.MessageExecutor("Error", "No se puede realizar un Reporte vacio!");
                return;
            }

            Reports.Reporte reporte = new Reports.Reporte();
            reporte.ExportPDF(Metodos.Inventario(TipoBuscarPor, FechaInicio, FechaFinal, TipoMostrar, TipoOrdenar), "Inventario");
        }
    }

}
