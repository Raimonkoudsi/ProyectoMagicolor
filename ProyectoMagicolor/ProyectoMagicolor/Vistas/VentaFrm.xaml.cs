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
    public partial class VentaFrm : Page
    {
        public VentaFrm(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;

            txtImpuesto.KeyDown += new KeyEventHandler(Validaciones.TextBoxValidatePrices);
            txtDescuento.KeyDown += new KeyEventHandler(Validaciones.TextBoxValidatePrices);
        }

        public MainWindow Parent;

        public DCliente Cliente;

        bool Validate()
        {
            if (!ClienteSetted)
            {
                MessageBox.Show("Debes Seleccionar un Cliente!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtDocumento.Focus();
                return true;
            }

            if(ListaVenta.Count == 0)
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


        void Limpiar()
        {
            QuitarCliente();
            double Monto = 0;
            subtotal = 0;
            impuestos = 0;
            total = 0;
            descuento = 0;
            txtDescuento.Text = Monto.ToString("0.00");
            CbMetodoPago.SelectedIndex = -1;
            dpFechaLimite.SelectedDate = null;
            txtImpuesto.Text = "15";
            txtBuscar.Text = "";
            txtDocumento.Text = "";
            txtDescuento.Text = "0,00";

            DisplayData.Clear();
            ListaVenta.Clear();

            RefreshMoney();
            Refresh();

        }

        private void BtnProcesar_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
                return;

            LVenta Metodo = new LVenta();

            DVenta dVenta = new DVenta(0, Cliente.idCliente,
                                             MainWindow.LoggedTrabajador.idTrabajador,
                                             DateTime.Now,
                                             descuento,
                                             impuesto,
                                             CbMetodoPago.SelectedIndex + 1,
                                             0);

            DCuentaCobrar CC = new DCuentaCobrar(0,
                                               0,
                                               DateTime.Now,
                                               dpFechaLimite.SelectedDate ?? DateTime.Now,
                                               total,
                                               0);

            string res = Metodo.Insertar(dVenta, ListaVenta, CC);
            MessageBox.Show(res);

            if (res.Equals("OK"))
            {
                Limpiar();
            }
        }

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
                setCliente();
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

        public bool ClienteSetted = false;

        public void AgregarCliente(DCliente cliente)
        {
            ClienteSetted = true;

            Cliente = cliente;

            ClienteBuscar.Visibility = Visibility.Collapsed;
            txtCliName.Text = cliente.nombre + " " + cliente.apellidos;
            txtCliName.Visibility = Visibility.Visible;
            TxtCliDoc.Text = cliente.tipoDocumento + "-" + cliente.numeroDocumento;
            //TxtProvDir.Text = proveedor.direccion;
            TxtCliTelf.Text = cliente.telefono;
            TxtCliEmail.Text = cliente.email;
            ClienteDatos.Visibility = Visibility.Visible;


            BtnBuscar.Background = Brushes.ForestGreen;
            BtnBuscar.BorderBrush = Brushes.ForestGreen;
            BtnBuscar.Content = "Cambiar";
            BtnAbrir.Visibility = Visibility.Collapsed;

            txtBuscar.Focus();
        }

        public void QuitarCliente()
        {
            ClienteSetted = false;

            Cliente = null;

            ClienteBuscar.Visibility = Visibility.Visible;
            txtCliName.Visibility = Visibility.Collapsed;
            ClienteDatos.Visibility = Visibility.Collapsed;


            BtnBuscar.ClearValue(Button.BackgroundProperty);
            BtnBuscar.ClearValue(Button.BorderBrushProperty);
            BtnBuscar.Content = "Buscar";
            BtnAbrir.Visibility = Visibility.Visible;
        }

        void setCliente()
        {
            if (CbTipoDocumento.SelectedIndex > -1 && txtDocumento.Text != "")
            {
                LCliente Metodo = new LCliente();
                var response = Metodo.EncontrarConDocumento(CbTipoDocumento.Text, txtDocumento.Text);

                if (response.Count > 0)
                {
                    AgregarCliente(response[0]);
                    return;
                }
                else
                {
                    var resp = MessageBox.Show("Esta cedula no está en la base de datos! ¿Desea agregarlo?", "Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if(resp == MessageBoxResult.Yes)
                    {
                        ClienteFrmVista Frm = new ClienteFrmVista(this, CbTipoDocumento.Text, txtDocumento.Text);
                        bool? res = Frm.ShowDialog();
                    }
                }

            }
        }

        private void BtnAbrir_Click(object sender, RoutedEventArgs e)
        {
            ClienteVista PVFrm = new ClienteVista(this);

            PVFrm.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!ClienteSetted)
            {
                setCliente();
            }
            else
            {
                QuitarCliente();
            }

            
        }

        public void Refresh()
        {
            dgOperaciones.ItemsSource = null;
            dgOperaciones.ItemsSource = DisplayData;
            RefreshMoney();
        }

        public List<ModeloVenta> DisplayData = new List<ModeloVenta>();

        public List<DDetalle_Venta> ListaVenta = new List<DDetalle_Venta>();

        public void AgregarArticulo(DDetalle_Venta DDV, DArticulo DA)
        {
            ModeloVenta MCN = new ModeloVenta(DisplayData.Count,
                                                DA.nombre,
                                                DDV.precioVenta,
                                                DDV.cantidad);
            DisplayData.Add(MCN);

            ListaVenta.Add(DDV);

            txtBuscar.Text = "";
            txtBuscar.Focus();

            Refresh();
        }

        public void EditarArticulo(DDetalle_Venta DDV, DArticulo DA, int id)
        {
            int ind = DisplayData.FindIndex((Modelo) => Modelo.id == id);

            ModeloVenta MCN = new ModeloVenta(DisplayData.Count,
                                                DA.nombre,
                                                DDV.precioVenta,
                                                DDV.cantidad);

            DisplayData[ind] = MCN;

            ListaVenta[ind] = DDV;
            Refresh();

        }

        public void EliminarArticulo(int id)
        {
            int ind = DisplayData.FindIndex((Modelo) => Modelo.id == id);
            DisplayData.RemoveAt(ind);
            ListaVenta.RemoveAt(ind);
            Refresh();
        }

        public void SetArticulo()
        {
            bool WillInclude = false;

            bool OpenProducts = false;

            DArticulo DA = new DArticulo();
            DDetalle_Ingreso DDI = new DDetalle_Ingreso();
            if (txtBuscar.Text != "")
            {
                LArticulo Metodo = new LArticulo();
                LIngreso MEt = new LIngreso();
                var response = Metodo.EncontrarConCodigo(txtBuscar.Text);
                List<DDetalle_Ingreso> Resp2 = new List<DDetalle_Ingreso>();

                if(response.Count > 0)
                {
                    Resp2 = MEt.EncontrarByArticulo(response[0].idArticulo);
                }

                if (Resp2.Count > 0)
                {
                    DA = response[0];
                    DDI = Resp2[0];
                    WillInclude = true;
                }
                else
                {
                    var resp = MessageBox.Show("El Codigo que ingresaste no se encuentra! ¿Deseas buscarlo manualmente?", "Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (resp == MessageBoxResult.No)
                        return;
                    else
                        OpenProducts = true;
                }
            }
            DetalleVentaFrm DVFrm = new DetalleVentaFrm(this, ListaVenta);
            DVFrm.Type = TypeForm.Create;
            DVFrm.OpenProducts = OpenProducts;
            if(WillInclude)
            {
                DVFrm.AgregarArticulo(DDI, DA);
            }
            DVFrm.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //SetArticulo();

            DetalleVentaFrm DVFrm = new DetalleVentaFrm(this, ListaVenta);
            DVFrm.Type = TypeForm.Create;
            DVFrm.ShowDialog();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;

            int ind = DisplayData.FindIndex((Modelo) => Modelo.id == id);


            DetalleVentaFrm DIFrm = new DetalleVentaFrm(this, ListaVenta);
            DIFrm.Type = TypeForm.Update;
            DIFrm.idEdit = id;

            DIFrm.DataFill = ListaVenta[ind];

            LArticulo Metodo = new LArticulo();
            LIngreso MetIng = new LIngreso();

            var Resp = MetIng.Encontrar(ListaVenta[ind].idDetalleIngreso);

            var response = Metodo.Encontrar(Resp[0].idArticulo);

            DIFrm.DataArticulo = response[0];
            DIFrm.DataDIngreso = Resp[0];

            DIFrm.ShowDialog();


        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;

            EliminarArticulo(id);
        }

        public double subtotal, impuestos, descuento, total;

        public int impuesto;

        public void RefreshMoney()
        {
            double Subtotal = 0;
            foreach(DDetalle_Venta item in ListaVenta)
            {
                Subtotal += item.precioVenta * item.cantidad;
            }
            double SubTCImp = Subtotal;

            double impPer = double.Parse(txtImpuesto.Text, CultureInfo.InvariantCulture);
            double impDec = (impPer / 100);
            Subtotal = Subtotal / (impDec + 1);

            double imp = SubTCImp - Subtotal;

            subtotal = Subtotal;
            impuestos = impPer;

            impuesto = int.Parse(txtImpuesto.Text);

            double Total = Subtotal + imp;
            double Desc = 0;

            if (txtDescuento.Text != "")
                Desc = Convert.ToDouble(txtDescuento.Text);

            if (Desc > total)
                return;

            double Descuento = Desc;
            Total = Total - Descuento;


            descuento = Descuento;
            total = Total;


            double x = Math.Truncate(Subtotal * 100) / 100;
            txtSubtotal.Text = "Bs.S " + x.ToString("0.00");

            double y = Math.Truncate(imp * 100) / 100;
            txtImpuestoP.Text = "Bs.S " + y.ToString("0.00");

            double z = Math.Truncate(Total * 100) / 100;
            txtTotal.Text = "Bs.S " + z.ToString("0.00");

        }

        private void txtDescuento_LostFocus(object sender, RoutedEventArgs e)
        {
            double Monto = 0;

            if(txtDescuento.Text != "")
                Monto = Convert.ToDouble(txtDescuento.Text);

            if(Monto > subtotal)
            {
                MessageBox.Show("El Monto de descuento no puede ser mayor al subtotal!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                Monto = 0;
                subtotal = 0;
                impuestos = 0;
                total = 0;
                descuento = 0;
            }

            txtDescuento.Text = Monto.ToString("0.00");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
            txtDocumento.Focus();
            dpFechaLimite.DisplayDateStart = DateTime.Now.Date.AddDays(1);
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

        private void txtDescuento_KeyUp(object sender, KeyEventArgs e)
        {
            RefreshMoney();
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

    public class ModeloVenta
    {
        public ModeloVenta(int id, string nombreArticulo, double precio, int cantidad)
        {
            this.id = id;
            NombreArticulo = nombreArticulo;
            Precio = precio;
            Cantidad = cantidad;
        }

        public int id { get; set; }
        public string NombreArticulo { get; set; }
        public double Precio { get; set; }
        public int Cantidad { get; set; }


    }
}
