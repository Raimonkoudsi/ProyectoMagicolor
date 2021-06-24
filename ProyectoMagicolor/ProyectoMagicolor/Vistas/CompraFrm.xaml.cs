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
    public partial class CompraFrm : Page
    {
        public CompraFrm(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;

            txtImpuesto.KeyDown += new KeyEventHandler(Validaciones.TextBoxValidatePrices);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Limpiar();

            Refresh();

            txtImpuesto.IsEnabled = false;
            txtImpuesto.Text = LFunction.MostrarIVA().ToString();

            CbTipoDocumento.SelectedIndex = 2;
            dpFechaLimite.DisplayDateStart = DateTime.Now.Date.AddDays(1);
        }

        public new MainWindow Parent;

        public DProveedor Proveedor;

        bool Validate()
        {
            if (!ProveedorSetted)
            {
                MessageBox.Show("Debe seleccionar el proveedor de la compra", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                TxtProvDoc.Focus();
                return true;
            }

            if (ListaCompra.Count == 0)
            {
                MessageBox.Show("Debe agregar articulos", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtBuscar.Focus();
                return true;
            }

            if (CbMetodoPago.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un método de pago", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                CbMetodoPago.Focus();
                return true;
            }

            if (CbMetodoPago.SelectedIndex == 1 && dpFechaLimite.SelectedDate == null)
            {
                MessageBox.Show("Debe seleccionar una fecha límite de cuenta a pagar", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                dpFechaLimite.Focus();
                return true;
            }

            if (txtFactura.Text == "")
            {
                MessageBox.Show("Debe insertar la factura de compra!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtFactura.Focus();
                return true;
            }

            return false;

        }

        private void Limpiar()
        {
            QuitarProveedor();
            CbMetodoPago.SelectedIndex = -1;
            dpFechaLimite.SelectedDate = null;
            txtBuscar.Text = "";
            txtFactura.Text = "";
            DisplayData.Clear();
            ListaCompra.Clear();
            Refresh();
        }


        private void BtnProcesar_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
                return;

            LIngreso Metodo = new LIngreso();

            DIngreso dIngreso = new DIngreso(0,
                                             MainWindow.LoggedTrabajador.idTrabajador,
                                             Proveedor.idProveedor,
                                             DateTime.Now,
                                             txtFactura.Text,
                                             impuestos,
                                             CbMetodoPago.SelectedIndex + 1,
                                             0);

            DCuentaPagar CP = new DCuentaPagar(0,
                                               0,
                                               DateTime.Now,
                                               dpFechaLimite.SelectedDate ?? DateTime.Now,
                                               0,
                                               0);

            string res = Metodo.Insertar(dIngreso, ListaCompra, CP);

            if (res.Equals("OK"))
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Insertar",
                    "Ha Registrado la Compra N° " + dIngreso.idIngreso
                );
                new LAuditoria().Insertar(auditoria);

                Limpiar();
            }
        }

        #region VALIDACIONES
        private void CbMetodoPago_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GrdFechaLimite.Visibility = CbMetodoPago.SelectedIndex == 1 ?
                                            Visibility.Visible : Visibility.Collapsed;
        }

        private void txtDocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                setProveedor();
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
            TxtProvTelf.Text = proveedor.telefono == "" ? "Sin Teléfono" : proveedor.telefono;
            TxtProvEmail.Text = proveedor.email == "" ? "Sin Email" : proveedor.email;
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

        public void setProveedor()
        {
            if (CbTipoDocumento.SelectedIndex > -1 && txtDocumento.Text != "")
            {
                LProveedor Metodo = new LProveedor();

                var response = Metodo.EncontrarConDocumento(CbTipoDocumento.Text, txtDocumento.Text);
                if (response.Count > 0)
                {
                    AgregarProveedor(response[0]);
                    txtBuscar.Focus();
                    return;
                }

                response = Metodo.CedulaRepetidaAnulada((CbTipoDocumento.Text + "-" + txtDocumento.Text));
                if (response.Count > 0)
                {
                    if (Globals.ACCESO_SISTEMA == 0)
                    {
                        var respuesta = MessageBox.Show("El proveedor está deshabilitado en el sistema ¿Desea habilitarlo?" + Environment.NewLine + "(Abrirá el formulario para editar datos relevantes)", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (respuesta == MessageBoxResult.Yes)
                        {
                            ProveedorFrm frm = new ProveedorFrm(this, CbTipoDocumento.Text, txtDocumento.Text);
                            frm.Type = TypeForm.Update;
                            frm.DataFill = response[0];

                            bool res = frm.ShowDialog() ?? false;
                            return;
                        }
                    }
                    LFunction.MessageExecutor("Error", "El proveedor está deshabilitado, no se puede asignar");
                    txtDocumento.Focus();
                    return;
                }

                var resp = MessageBox.Show("El proveedor no está registrado ¿Desea agregarlo?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (resp == MessageBoxResult.Yes)
                {
                    ProveedorFrm Frm = new ProveedorFrm(this, CbTipoDocumento.Text, txtDocumento.Text);
                    bool res = Frm.ShowDialog() ?? false;
                    return;
                }
            }
            ProveedorVista PVFrm = new ProveedorVista(this);

            PVFrm.ShowDialog();
        }

        public void SetNuevoProveedor(string TipoDocumento, string Documento)
        {
            CbTipoDocumento.Text = TipoDocumento;
            txtDocumento.Text = Documento;
            setProveedor();
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

            if (DisplayData.Count == 0)
            {
                SinRegistro.Visibility = Visibility.Visible;
            }
            else
            {
                SinRegistro.Visibility = Visibility.Collapsed;
            }
        }

        public List<ModeloCompra> DisplayData = new List<ModeloCompra>();

        public List<DDetalle_Ingreso> ListaCompra = new List<DDetalle_Ingreso>();


        public void AgregarArticulo(DDetalle_Ingreso DDI, DArticulo DA)
        {

            //ACA VA LO DE SUMAR LAS CANTIDADES
            ModeloCompra MCN = new ModeloCompra(DisplayData.Count,
                                                DA.nombre,
                                                DDI.precioCompra,
                                                DDI.precioVenta,
                                                DDI.cantidadActual);
            DisplayData.Add(MCN);

            ListaCompra.Add(DDI);

            txtBuscar.Text = "";
            txtBuscar.Focus();
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

        public void SetNuevoArticulo(string Codigo)
        {
            txtBuscar.Text = Codigo;
            SetArticulo();
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
                    var SearchId = ListaCompra.FindIndex((comp) => comp.idArticulo == response[0].idArticulo);
                    if (SearchId != -1)
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
                else if (response.Count == 0)
                {
                    if (txtBuscar.Text.Length <= 5)
                    {
                        LFunction.MessageExecutor("Error", "El código debe ser mayor a 5 dígitos");
                        txtBuscar.Focus();
                    }
                    else
                    {
                        LIngreso MetodoIngreso = new LIngreso();
                        var responseIngreso = MetodoIngreso.EncontrarByArticuloAnulado(txtBuscar.Text);

                        if (responseIngreso.Count > 0)
                        {
                            if (Globals.ACCESO_SISTEMA == 0)
                            {
                                var resp = MessageBox.Show("El artículo está deshabilitado" + Environment.NewLine + "¿Desea Activarlo?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                                if (resp == MessageBoxResult.Yes)
                                {
                                    ArticuloFrm frm = new ArticuloFrm(this, txtBuscar.Text);
                                    frm.Type = TypeForm.Update;
                                    frm.DataFill = responseIngreso[0];

                                    bool res = frm.ShowDialog() ?? false;

                                    responseIngreso = Metodo.EncontrarConCodigo(txtBuscar.Text);
                                    if (responseIngreso.Count > 0)
                                    {
                                        DA = responseIngreso[0];
                                        WillInclude = true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("El artículo está deshabilitado, debe contactar al administrador para reactivarlo", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                                txtBuscar.Text = "";
                                txtBuscar.Focus();
                                return;
                            }
                        }
                        else
                        {
                            var resp = MessageBox.Show("El artículo no se encuentra registrado" + Environment.NewLine + "¿Desea Agregarlo?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                            if (resp == MessageBoxResult.Yes)
                            {
                                ArticuloFrm Frm = new ArticuloFrm(this, txtBuscar.Text);
                                bool? res = Frm.ShowDialog();

                                response = Metodo.EncontrarConCodigo(txtBuscar.Text);
                                if (response.Count > 0)
                                {
                                    DA = response[0];
                                    WillInclude = true;
                                }

                                txtBuscar.Text = "";
                            }
                        }
                    }
                }

            }
            DetalleIngresoFrm DIFrm = new DetalleIngresoFrm(this, ListaCompra);
            DIFrm.Type = TypeForm.Create;
            if (WillInclude)
            {
                DIFrm.AgregarArticulo(DA);
                DIFrm.ShowDialog();
            }
            else
            {
                DIFrm.AgregarArticulo(null);
                DIFrm.ShowDialog();
            }
        }

        void OpenToEdit(int id)
        {
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

        public double subtotal, total;

        public int impuestos;

        public void RefreshMoney()
        {
            double Subtotal = 0;
            foreach (DDetalle_Ingreso item in ListaCompra)
            {
                Subtotal += item.precioCompra * item.cantidadInicial;
            }

            double SubTCImp = Subtotal;


            double impPer = double.Parse(txtImpuesto.Text, CultureInfo.InvariantCulture);
            double impDec = (impPer / 100);
            Subtotal = Subtotal / (impDec + 1);

            double imp = SubTCImp - Subtotal;
            double Total = Subtotal + imp;

            subtotal = Subtotal;
            impuestos = int.Parse(txtImpuesto.Text);
            total = Total;



            double x = Math.Truncate(Subtotal * 100) / 100;
            txtSubtotal.Text = "Bs.S " + x.ToString("0.00");

            double y = Math.Truncate(imp * 100) / 100;
            txtImpuestoP.Text = "Bs.S " + y.ToString("0.00");

            double z = Math.Truncate(Total * 100) / 100;
            txtTotal.Text = "Bs.S " + z.ToString("0.00");
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
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
        public ModeloCompra(int Id, string nombreArticulo, double precioCompra, double precioVenta, int cantidad)
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
