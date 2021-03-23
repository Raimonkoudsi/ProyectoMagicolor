using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// <summary>
    /// Interaction logic for CompraFrm.xaml
    /// </summary>
    public partial class CompraFrm : Page
    {
        public CompraFrm(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;

            txtImpuesto.KeyDown += new KeyEventHandler(Validaciones.TextBoxValidatePrices);
        }

        public MainWindow Parent;

        public DProveedor Proveedor;

        bool Validate()
        {
            if (!ProveedorSetted)
            {
                MessageBox.Show("Debes Seleccionar un Proveedor!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                TxtProvDoc.Focus();
                return true;
            }

            if(ListaCompra.Count == 0)
            {
                MessageBox.Show("Debes Agregar Articulos!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtBuscar.Focus();
                return true;
            }

            if(CbMetodoPago.SelectedIndex == -1)
            {
                MessageBox.Show("Debes Seleccionar un Metodo de Pago!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                CbMetodoPago.Focus();
                return true;
            }

            if(CbMetodoPago.SelectedIndex == 1 && dpFechaLimite.SelectedDate == null)
            {
                MessageBox.Show("Debes Seleccionar una Fecha Limite!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                dpFechaLimite.Focus();
                return true;
            }

            return false;

        }


        private void BtnProcesar_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
                return;

            LIngreso Metodo = new LIngreso();

            DIngreso dIngreso = new DIngreso(0,
                                             Parent.LoggedTrabajador.idTrabajador,
                                             Proveedor.idProveedor,
                                             DateTime.Now,
                                             "",
                                             impuestos,
                                             CbMetodoPago.SelectedIndex + 1,
                                             0, "", "", 0, 0, 0);

            DCuentaPagar CP = new DCuentaPagar(0,
                                               0,
                                               DateTime.Now,
                                               dpFechaLimite.SelectedDate ?? DateTime.Now,
                                               0,
                                               0);

            string res = Metodo.Insertar(dIngreso, ListaCompra, CP);
            MessageBox.Show(res);
        }

        #region VALIDACIONES
        private void CbMetodoPago_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GrdFechaLimite.Visibility = CbMetodoPago.SelectedIndex == 1 ?
                                            Visibility.Visible : Visibility.Collapsed;
            if (CbMetodoPago.SelectedIndex > -1)
                PlaceMetodoPago.Text = "";
            else
                PlaceMetodoPago.Text = "Método de pago";
        }

        private void CbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbTipoDocumento.SelectedIndex > -1)
                PlaceTipoDocumento.Text = "";
            else
                PlaceTipoDocumento.Text = "Tipo";
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtDocumento.Text == "")
            {
                PlaceDocumento.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDocumento.Text == "")
            {
                PlaceDocumento.Text = "Buscar...";
            }
        }

        private void txtDocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                setProveedor();
            }
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "")
            {
                txtBucarPlaceH.Text = "";
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "")
            {
                PlaceDocumento.Text = "Codigo de Articulo";
            }
        }
#endregion

        public bool ProveedorSetted = false;

        public void AgregarProveedor(DProveedor proveedor)
        {
            ProveedorSetted = true;

            Proveedor = proveedor;

            ProveedorBuscar.Visibility = Visibility.Collapsed;
            txtProvName.Text = proveedor.razonSocial;
            txtProvName.Visibility = Visibility.Visible;
            TxtProvDoc.Text = proveedor.tipoDocumento + "-" + proveedor.numeroDocumento;
            //TxtProvDir.Text = proveedor.direccion;
            TxtProvTelf.Text = proveedor.telefono;
            TxtProvEmail.Text = proveedor.email;
            ProveedorDatos.Visibility = Visibility.Visible;


            BtnProveedor.Background = Brushes.ForestGreen;
            BtnProveedor.BorderBrush = Brushes.ForestGreen;
            BtnProveedor.Content = "Cambiar";
        }

        public void QuitarProveedor()
        {
            ProveedorSetted = false;

            Proveedor = null;

            ProveedorBuscar.Visibility = Visibility.Visible;
            txtProvName.Visibility = Visibility.Collapsed;
            ProveedorDatos.Visibility = Visibility.Collapsed;


            BtnProveedor.ClearValue(Button.BackgroundProperty);
            BtnProveedor.ClearValue(Button.BorderBrushProperty);
            BtnProveedor.Content = "Encontrar";
        }

        void setProveedor()
        {
            if (CbTipoDocumento.SelectedIndex > -1 && txtDocumento.Text != "")
            {
                LProveedor Metodo = new LProveedor();
                var response = Metodo.EncontrarConDocumento(CbTipoDocumento.Text, txtDocumento.Text);

                if (response.Count > 0)
                {
                    AgregarProveedor(response[0]);
                    return;
                }

            }
            ProveedorVista PVFrm = new ProveedorVista(this);

            PVFrm.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!ProveedorSetted)
            {
                setProveedor();
            }
            else
            {
                QuitarProveedor();
            }

            
        }

        public void Refresh()
        {
            dgOperaciones.ItemsSource = null;
            dgOperaciones.ItemsSource = DisplayData;
            RefreshMoney();
        }

        public List<ModeloCompra> DisplayData = new List<ModeloCompra>();

        public List<DDetalle_Ingreso> ListaCompra = new List<DDetalle_Ingreso>();


        public void AgregarArticulo(DDetalle_Ingreso DDI, DArticulo DA)
        {
            ModeloCompra MCN = new ModeloCompra(DisplayData.Count,
                                                DA.nombre,
                                                DDI.precioCompra,
                                                DDI.precioVenta,
                                                DDI.cantidadActual);
            DisplayData.Add(MCN);

            ListaCompra.Add(DDI);
            Refresh();
        }

        public void EditarArticulo(DDetalle_Ingreso DDI, DArticulo DA, int id)
        {
            int ind = DisplayData.FindIndex((Modelo) => Modelo.id == id);

            ModeloCompra MCN = new ModeloCompra(DisplayData.Count,
                                                DA.nombre,
                                                DDI.precioCompra,
                                                DDI.precioVenta,
                                                DDI.cantidadActual);

            DisplayData[ind] = MCN;

            ListaCompra[ind] = DDI;
            Refresh();

        }

        public void EliminarArticulo(int id)
        {
            int ind = DisplayData.FindIndex((Modelo) => Modelo.id == id);
            DisplayData.RemoveAt(ind);
            ListaCompra.RemoveAt(ind);
            Refresh();
        }

        public void SetArticulo()
        {
            bool WillInclude = false;

            DArticulo DA = new DArticulo();
            if (txtBuscar.Text != "")
            {
                LArticulo Metodo = new LArticulo();
                var response = Metodo.EncontrarConCodigo(txtBuscar.Text);

                if (response.Count > 0)
                {
                    DA = response[0];
                    WillInclude = true;
                }

            }
            DetalleIngresoFrm DIFrm = new DetalleIngresoFrm(this, ListaCompra);
            DIFrm.Type = TypeForm.Create;
            if(WillInclude)
            {
                DIFrm.AgregarArticulo(DA);
            }
            DIFrm.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SetArticulo();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;

            int ind = DisplayData.FindIndex((Modelo) => Modelo.id == id);


            DetalleIngresoFrm DIFrm = new DetalleIngresoFrm(this, ListaCompra);
            DIFrm.Type = TypeForm.Update;
            DIFrm.idEdit = id;

            DIFrm.DataFill = ListaCompra[ind];

            LArticulo Metodo = new LArticulo();

            var response = Metodo.Encontrar(ListaCompra[ind].idArticulo);

            DIFrm.DataArticulo = response[0];

            DIFrm.ShowDialog();


        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;

            EliminarArticulo(id);
        }

        public double subtotal, impuestos, total;

        public void RefreshMoney()
        {
            double Subtotal = 0;
            foreach(DDetalle_Ingreso item in ListaCompra)
            {
                Subtotal += item.precioCompra * item.cantidadInicial;
            }
            double x = Math.Truncate(Subtotal * 100) / 100;
            txtSubtotal.Text = "Bs.S " + x.ToString("0.00");



            double impPer = double.Parse(txtImpuesto.Text, CultureInfo.InvariantCulture);

            double imp = Subtotal * (impPer / 100);

            double y = Math.Truncate(imp * 100) / 100;

            txtImpuestoP.Text = "Bs.S " + y.ToString("0.00");



            double Total = Subtotal + imp;

            double z = Math.Truncate(Total * 100) / 100;

            txtTotal.Text = "Bs.S " + z.ToString("0.00");

            subtotal = Subtotal;
            impuestos = imp;
            total = Total;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void dpFechaLimite_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpFechaLimite.SelectedDate != null)
            {
                PlaceFechaLimite.Text = "";
            }
            else
            {
                PlaceFechaLimite.Text = "Fecha Limite";
            }
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SetArticulo();
            }
        }

        private void txtImpuesto_KeyUp(object sender, KeyEventArgs e)
        {
            RefreshMoney();
        }


    }

    public class ModeloCompra
    {
        public ModeloCompra(int Id ,string nombreArticulo, double precioCompra, double precioVenta, int cantidad)
        {
            id = Id;
            NombreArticulo = nombreArticulo;
            PrecioCompra = precioCompra;
            PrecioVenta = precioVenta;
            Cantidad = cantidad;
        }

        public int id { get; set; }
        public string NombreArticulo { get; set; }
        public double PrecioCompra { get; set; }
        public double PrecioVenta { get; set; }
        public int Cantidad { get; set; }


    }
}
