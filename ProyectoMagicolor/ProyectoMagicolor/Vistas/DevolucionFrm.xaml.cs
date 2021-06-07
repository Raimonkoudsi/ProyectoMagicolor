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
    public partial class DevolucionFrm : Page
    {
        DevolucionInicio ParentFrm;

        public DevolucionFrm(DVenta venta, DevolucionInicio par)
        {
            InitializeComponent();

            ParentFrm = par;

            this.Venta = venta;
        }

        public DVenta Venta;
        public List<DDetalle_Venta> DetalleVentas = new List<DDetalle_Venta>();

        public List<ModeloDevolucion> ArticulosaDevolver = new List<ModeloDevolucion>();
        public List<DDetalle_Devolucion> Devoluciones = new List<DDetalle_Devolucion>();


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DetalleVentas = new LVenta().MostrarDetalleVenta(Venta.idVenta);
            //factura
            txtFactura.Text = "#" + Venta.idVenta;
            txtFecha.Text = Venta.fecha.ToShortDateString();
            //trabajador
            txtVendedor.Text = Venta.trabajador;
            //cliente
            txtCliName.Text = Venta.cliente;
            TxtCliDoc.Text = Venta.cedulaCliente;
            TxtCliTelf.Text = Venta.telefonoCliente;

            CbMetodoPago.SelectedIndex = Venta.metodoPago - 1;
            int MetodoCredito = 2;
            if (Venta.metodoPago == MetodoCredito)
            {
                var CuentaCobrar = new LCuentaCobrar().BuscarCxC(Venta.idVenta)[0];
                dpFechaLimite.SelectedDate = CuentaCobrar.fechaLimite;
            }

            int i = 0;
            foreach (DDetalle_Venta item in DetalleVentas)
            {
                ModeloDevolucion modelo = new ModeloDevolucion(i, item.nombre,
                                                                item.precioVenta,
                                                                item.cantidad,
                                                                0, item.cantidad, false);
                modelo.idDetalleVenta = item.idDetalleVenta;
                modelo.idArticulo = item.idArticulo;
                ArticulosaDevolver.Add(modelo);
                i++;
            }


            Refresh();
        }


        public void Refresh()
        {
            double MontoTotal = 0;
            double MontoDevolver = 0;
            Devoluciones.Clear();
            foreach (ModeloDevolucion item in ArticulosaDevolver)
            {
                MontoTotal += item.Precio * item.CantidadRestante;
                MontoDevolver += !item.Dañado ? item.Precio * item.CantidadDevuelta : 0;
                if (item.CantidadDevuelta != 0)
                {
                    DDetalle_Devolucion Devolucion = new DDetalle_Devolucion(0,
                                                                             0,
                                                                             item.idDetalleVenta,
                                                                             item.idArticulo,
                                                                             item.CantidadDevuelta,
                                                                             item.Precio,
                                                                             item.Dañado ? 1 : 0);
                    Devoluciones.Add(Devolucion);

                    
                }
            }

            double MontoTotalFormatted = Math.Truncate(MontoTotal * 100) / 100;
            txtTotal.Text = "Bs.S " + MontoTotalFormatted.ToString("0.00");

            double MontoDevolverFormatted = Math.Truncate(MontoDevolver * 100) / 100;
            txtMontoDevolucion.Text = "Bs.S " + MontoDevolverFormatted.ToString("0.00");

            dgOperaciones.ItemsSource = null;
            dgOperaciones.ItemsSource = ArticulosaDevolver;

            if(Devoluciones.Count <= 0)
            {
                BtnProcesarDevolucion.IsEnabled = false;
            }
            else
            {
                BtnProcesarDevolucion.IsEnabled = true;
            }
        }

        bool ProcessAll = false;

        private void BtnProcesarTodo_Click(object sender, RoutedEventArgs e)
        {
            if (!ProcessAll)
            {
                BtnProcesarTodo.Content = "Cancelar Devolución Completa";
                dgOperaciones.IsEnabled = false;
                ProcessAll = true;
                foreach(ModeloDevolucion item in ArticulosaDevolver)
                {
                    item.CantidadDevuelta = item.CantidadComprada;
                    item.CantidadRestante = 0;
                    item.Dañado = false;
                }
                Refresh();
            }
            else
            {
                BtnProcesarTodo.Content = "Devolución Completa";
                dgOperaciones.IsEnabled = true;
                ProcessAll = false;
                foreach (ModeloDevolucion item in ArticulosaDevolver)
                {
                    item.CantidadDevuelta = 0;
                    item.CantidadRestante = item.CantidadComprada;
                    item.Dañado = false;
                }
                Refresh();
            }
        }

        private void BtnDevolver_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;

            DevolucionDetalleFrm frm = new DevolucionDetalleFrm(this, ArticulosaDevolver[id]);
            frm.ShowDialog();
        }

        public void DevolucionArticulo(ModeloDevolucion devolucion)
        {
            ArticulosaDevolver[devolucion.Id] = devolucion;
            Refresh();
        }
        public void CancelarDevolucion(int id)
        {
            ArticulosaDevolver[id].CantidadDevuelta = 0;
            ArticulosaDevolver[id].CantidadRestante = ArticulosaDevolver[id].CantidadComprada;
            ArticulosaDevolver[id].Dañado = false;
            Refresh();
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


        bool Validate()
        {
            if(txtMotivo.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar el motivo de la Devolución");
                txtMotivo.Focus();

                return true;
            }

            if (txtMotivo.Text.Length <= 5)
            {
                LFunction.MessageExecutor("Error", "El motivo debe tener más de 5 carácteres");
                txtMotivo.Focus();

                return true;
            }

            return false;
        }

        private void BtnProcesarDevolucion_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
                return;

            DDevolucion Devolucion = new DDevolucion(0,
                                                     Venta.idCliente,
                                                     MainWindow.LoggedTrabajador.idTrabajador,
                                                     Venta.idVenta,
                                                     DateTime.Now,
                                                     txtMotivo.Text);

            var resp = new LDevolucion().Insertar(Devolucion, Devoluciones);
            if(resp == "OK")
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Registrar",
                    "Ha Devuelto Artículos de la Venta N° " + Venta.idVenta
                );
                new LAuditoria().Insertar(auditoria);

                ParentFrm.Limpiar();
                ParentFrm.GetBack();
            }
        }

        private void BtnAtras_Click(object sender, RoutedEventArgs e)
        {
            ParentFrm.GetBack();
        }
    }
    public class ModeloDevolucion
    {
        public ModeloDevolucion(int id, string nombreArticulo, double precio, int cantidadComprada, int cantidadDevuelta, int cantidadRestante, bool dañado)
        {
            Id = id;
            NombreArticulo = nombreArticulo;
            Precio = precio;
            CantidadComprada = cantidadComprada;
            CantidadDevuelta = cantidadDevuelta;
            CantidadRestante = cantidadRestante;
            Dañado = dañado;
        }

        public int Id { get; set; }

        public string NombreArticulo { get; set; }
        public double Precio { get; set; }
        public int CantidadComprada { get; set; }
        public int CantidadDevuelta { get; set; }
        public int CantidadRestante { get; set; }
        public bool Dañado { get; set; }
        public int idDetalleVenta;
        public int idArticulo;
    }
}
