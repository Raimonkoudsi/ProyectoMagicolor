using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{

    public partial class VentaFrm : Page
    {
        public VentaFrm(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;

            txtImpuesto.KeyDown += new KeyEventHandler(Validaciones.TextBoxValidatePrices);
            txtDescuento.KeyDown += new KeyEventHandler(Validaciones.TextBoxValidatePrices);
            txtBuscar.KeyDown += new KeyEventHandler(Validaciones.TextBoxValidatePrices);
            txtDocumento.KeyDown += new KeyEventHandler(Validaciones.TextBoxValidatePrices);
        }

        public new MainWindow Parent;

        public DCliente Cliente;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Limpiar();

            Refresh();
            txtDocumento.Focus();
            dpFechaLimite.DisplayDateStart = DateTime.Now.Date.AddDays(1);

            CbTipoDocumento.SelectedIndex = 0;

            txtImpuesto.IsEnabled = false;
            txtImpuesto.Text = LFunction.MostrarIVA().ToString();
        }

        private bool Validate()
        {
            if (!ClienteSetted)
            {
                MessageBox.Show("Debe Seleccionar un Cliente", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtDocumento.Focus();
                return true;
            }

            if(ListaVenta.Count == 0)
            {
                MessageBox.Show("Debe Agregar Articulos", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtBuscar.Focus();
                return true;
            }

            if(CbMetodoPago.SelectedIndex == -1)
            {
                MessageBox.Show("Debe Seleccionar un Metodo de Pago", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                CbMetodoPago.Focus();
                return true;
            }

            if(CbMetodoPago.SelectedIndex == 1 && dpFechaLimite.SelectedDate == null)
            {
                MessageBox.Show("Debe Seleccionar una Fecha Limite", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                dpFechaLimite.Focus();
                return true;
            }

            return false;

        }


        private void Limpiar()
        {
            QuitarCliente();
            subtotal = 0;
            impuestos = 0;
            total = 0;
            descuento = 0;
            CbMetodoPago.SelectedIndex = -1;
            dpFechaLimite.SelectedDate = null;
            txtBuscar.Text = "";
            txtDocumento.Text = "";

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

            if (res.Equals("OK"))
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Insertar",
                    "Ha Registrado la Venta N° " + dVenta.idVenta
                );
                new LAuditoria().Insertar(auditoria);

                if (dVenta.estado == 2)
                {
                    var resp = MessageBox.Show("¡Venta a Crédito Ingresada!", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                }
                else
                {
                    var resp = MessageBox.Show("¡Venta Completada!" + Environment.NewLine + "¿Desea Mostrar la Factura de la Venta?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (resp == MessageBoxResult.Yes)
                    {
                        Reports.Reporte reporte = new Reports.Reporte();
                        reporte.ExportPDFTwoArguments(Metodo.MostrarDetalleVenta(dVenta.idVenta), "Venta", Metodo.MostrarVenta(dVenta.idVenta), "VentaGeneral", false, dVenta.idVenta.ToString());

                        auditoria = new DAuditoria(
                            Globals.ID_SISTEMA,
                            "Generar",
                            "Ha Impreso la Factura de la Venta N° " + dVenta.idVenta
                        );
                        new LAuditoria().Insertar(auditoria);
                    }
                }

                Limpiar();
            }
        }

        private void CbMetodoPago_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GrdFechaLimite.Visibility = CbMetodoPago.SelectedIndex == 1 ?
                                            Visibility.Visible : Visibility.Collapsed;
        }

        private void txtDocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                setCliente();
            }
        }

        public bool ClienteSetted = false;

        public void AgregarCliente(DCliente cliente)
        {
            ClienteSetted = true;

            Cliente = cliente;

            ClienteBuscar.Visibility = Visibility.Collapsed;
            txtCliName.Text = cliente.nombre;
            txtCliName.Visibility = Visibility.Visible;
            TxtCliDoc.Text = cliente.tipoDocumento + "-" + cliente.numeroDocumento;
            TxtCliTelf.Text = cliente.telefono == "" ? "Sin Teléfono" : cliente.telefono;
            TxtCliEmail.Text = cliente.email == "" ? "Sin Correo" : cliente.email;
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

        private void setCliente()
        {
            if (CbTipoDocumento.SelectedIndex > -1 && txtDocumento.Text != "")
            {
                LCliente Metodo = new LCliente();

                var response = Metodo.EncontrarConDocumento(CbTipoDocumento.Text, txtDocumento.Text);
                if (response.Count > 0)
                {
                    AgregarCliente(response[0]);
                    txtBuscar.Focus();
                    return;
                }

                response = Metodo.CedulaRepetidaAnulada((CbTipoDocumento.Text + "-" + txtDocumento.Text));
                if (response.Count > 0)
                {
                    if (Globals.ACCESO_SISTEMA == 0)
                    {
                        var respuesta = MessageBox.Show("El cliente está deshabilitado en el sistema ¿Desea habilitarlo?" + Environment.NewLine + "(Abrirá el formulario para editar datos relevantes)", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (respuesta == MessageBoxResult.Yes)
                        {
                            ClienteFrm frm = new ClienteFrm(this, CbTipoDocumento.Text, txtDocumento.Text);
                            frm.Type = TypeForm.Update;
                            frm.DataFill = response[0];

                            bool res = frm.ShowDialog() ?? false;
                            return;
                        }
                    }
                    LFunction.MessageExecutor("Error", "El cliente está deshabilitado, no se puede asignar");
                    txtDocumento.Focus();
                    return;
                }

                var resp = MessageBox.Show("El cliente no está registrado ¿Desea agregarlo?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (resp == MessageBoxResult.Yes)
                {
                    ClienteFrm Frm = new ClienteFrm(this, CbTipoDocumento.Text, txtDocumento.Text);
                    bool res = Frm.ShowDialog() ?? false;
                    return;
                }
            }
            ClienteVista PVFrm = new ClienteVista(this);

            PVFrm.ShowDialog();
        }

        public void SetNuevoCliente(string TipoDocumento, string Documento)
        {
            CbTipoDocumento.Text = TipoDocumento;
            txtDocumento.Text = Documento;
            setCliente();
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

            if (DisplayData.Count == 0)
            {
                SinRegistro.Visibility = Visibility.Visible;
            }
            else
            {
                SinRegistro.Visibility = Visibility.Collapsed;
            }
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

                if (response.Count == 0)
                {
                    MessageBox.Show("El artículo no existe o esta deshabilitado", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtBuscar.Text = "";
                    txtBuscar.Focus();

                    return;
                }

                if(response[0].cantidadActual <= 0)
                {
                    MessageBox.Show("El artículo no posee disponibilidad", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtBuscar.Text = "";
                    txtBuscar.Focus();

                    return;
                }

                if (response.Count > 0)
                {
                    Resp2 = MEt.EncontrarByArticulo(response[0].idArticulo);
                }

                if (Resp2.Count > 0)
                {
                    DA = response[0];
                    DDI = Resp2[0];
                    var SearchId = ListaVenta.FindIndex((vent) => vent.idDetalleIngreso == Resp2[0].idDetalleIngreso);
                    if(SearchId != -1)
                    {
                        OpenToEdit(DisplayData[SearchId].id);
                        txtBuscar.Text = "";
                        txtBuscar.Focus();
                        return;
                    }
                    else
                    {
                        WillInclude = true;
                    }
                    
                }
                else
                {
                    LIngreso MetodoIngreso = new LIngreso();
                    var responseIngreso = MetodoIngreso.EncontrarByArticuloAnulado(txtBuscar.Text);

                    if (responseIngreso.Count > 0)
                    {
                        MessageBox.Show("El artículo está deshabilitado, no se puede realizar ventas", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtBuscar.Text = "";
                        txtBuscar.Focus();
                        return;
                    }
                    else
                    {
                        var resp = MessageBox.Show("El Codigo que ingresó no se encuentra ¿Desea buscarlo manualmente?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (resp == MessageBoxResult.No)
                            return;
                        else
                            OpenProducts = true;
                    }
                }
            }
            DetalleVentaFrm DVFrm = new DetalleVentaFrm(this, ListaVenta);
            DVFrm.Type = TypeForm.Create;
            DVFrm.OpenProducts = OpenProducts;
            if (WillInclude)
            {
                DVFrm.AgregarArticulo(DDI, DA);
                DVFrm.ShowDialog();
            }
            else
            {
                DVFrm.AgregarArticulo(DDI, null);
                DVFrm.ShowDialog();
            }
        }


        void OpenToEdit(int id)
        {
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SetArticulo();
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


            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

            nfi.CurrencyDecimalSeparator = ",";
            nfi.CurrencyGroupSeparator = ".";
            nfi.CurrencySymbol = "";


            //double x = Math.Truncate(Subtotal * 100) / 100;

            string x = Convert.ToDecimal(Subtotal).ToString("C3", nfi);
            txtSubtotal.Text = "Bs.S " + x;


            //double y = Math.Truncate(imp * 100) / 100;

            string y = Convert.ToDecimal(imp).ToString("C3", nfi);
            txtImpuestoP.Text = "Bs.S " + y;



            //double z = Math.Truncate(Total * 100) / 100;

            string z = Convert.ToDecimal(Total).ToString("C3", nfi);
            txtTotal.Text = "Bs.S " + z;

        }

        private void txtDescuento_LostFocus(object sender, RoutedEventArgs e)
        {
            double Monto = 0;

            if(txtDescuento.Text != "")
                Monto = Convert.ToDouble(txtDescuento.Text);

            if(Monto > subtotal)
            {
                MessageBox.Show("El Monto de descuento no puede ser mayor al subtotal!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                Monto = 0;
                subtotal = 0;
                impuestos = 0;
                total = 0;
                descuento = 0;

                txtDescuento.Text = "";
                txtDescuento.Focus();
            }

            txtDescuento.Text = Monto.ToString("0.00");
        }

        private void txtDescuento_KeyUp(object sender, KeyEventArgs e)
        {
            double Monto = 0;

            if(txtDescuento.Text != "")
                Monto = Convert.ToDouble(txtDescuento.Text);

            if(Monto > subtotal)
            {
                MessageBox.Show("El Monto de descuento no puede ser mayor al subtotal!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                Monto = 0;
                subtotal = 0;
                impuestos = 0;
                total = 0;
                descuento = 0;

                txtDescuento.Text = "";
                txtDescuento.Focus();
            }

            RefreshMoney();

            txtDescuento.Text = Monto.ToString("0.00");
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
