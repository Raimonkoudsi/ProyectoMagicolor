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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class CompraVista : Page
    {
        CompraDG ParentFrm;

        public CompraVista(DIngreso compra, CompraDG par)
        {
            InitializeComponent();

            ParentFrm = par;

            this.Compra = compra;
        }

        public DIngreso Compra;
        private LIngreso Metodos = new LIngreso();

        public List<DDetalle_Ingreso> DetalleCompras = new List<DDetalle_Ingreso>();

        public List<ModeloFactura> ArticulosADevolver = new List<ModeloFactura>();



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Mostrar",
                "Ha Visto la Compra N° " + Compra.idIngreso
             );
            new LAuditoria().Insertar(auditoria);

            if (Globals.ACCESO_SISTEMA != 0)
            {
                BtnFactura.ToolTip = "Sólo el Administrador puede reimprimir Facturas";
                BtnFactura.IsEnabled = false;

                BtnAnular.ToolTip = "Sólo el Administrador puede Anular Ventas";
                BtnAnular.IsEnabled = false;
            }

            DetalleCompras = Metodos.MostrarDetalleCompra(Compra.idIngreso);
            //factura
            txtFactura.Text = "#" + Compra.factura;
            txtFecha.Text = Compra.fechaString;

            //monto
            double subTotal = (Compra.montoTotal / ((Compra.impuesto / 100.00) + 1));
            txtSubtotal.Text = "Bs.S " + subTotal.ToString("0.00");
            double impuesto = (Compra.montoTotal - subTotal);
            txtImpuesto.Text = "Bs.S " + impuesto.ToString("0.00");
            double total = (Compra.montoTotal);
            txtTotal.Text = "Bs.S " + total.ToString("0.00");

            //trabajador
            txtVendedor.Text = Compra.trabajador;
            //cliente
            txtProveedor.Text = Compra.razonSocial;
            TxtProveedorDocumento.Text = Compra.cedulaProveedor;
            TxtProveedorTelefono.Text = Compra.telefonoProveedor;
            TxtProveedorEmail.Text = Compra.emailProveedor;

            int i = 0;
            foreach (DDetalle_Ingreso item in DetalleCompras)
            {
                ModeloFactura modelo = new ModeloFactura(i, item.nombre,
                                                                item.precioCompra,
                                                                item.cantidad,
                                                                (item.precioCompra * item.cantidad));
                modelo.idDetalleVenta = item.idDetalleIngreso;
                modelo.idArticulo = item.idArticulo;
                ArticulosADevolver.Add(modelo);

                i++;
            }

            dgOperaciones.ItemsSource = null;
            dgOperaciones.ItemsSource = ArticulosADevolver;
        }

        private void BtnAtras_Click(object sender, RoutedEventArgs e)
        {
            ParentFrm.GetBack();
        }

        private void BtnAnular_Click(object sender, RoutedEventArgs e)
        {
            var resp = MessageBox.Show("¿Desea Anular la Compra?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (resp == MessageBoxResult.No)
                return;
            else
            {
                string metodoAnular = Metodos.Anular(Compra.idIngreso, new LIngreso().MostrarDetalleCompra(Compra.idIngreso));
                if (metodoAnular == "OK")
                {
                    DAuditoria auditoria = new DAuditoria(
                        Globals.ID_SISTEMA,
                        "Anular",
                        "Ha Anulado la Compra N° " + Compra.idIngreso
                    );
                    new LAuditoria().Insertar(auditoria);

                    ParentFrm.GetBack();
                }
            }
        }

        private void BtnComprobante_Click(object sender, RoutedEventArgs e)
        {
            Reports.Reporte reporte = new Reports.Reporte();
            reporte.ExportPDFTwoArguments(Metodos.MostrarDetalleCompra(Compra.idIngreso), "Compra", Metodos.MostrarCompra(Compra.idIngreso), "CompraGeneral", true, Compra.idIngreso.ToString());

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Generar",
                "Ha Reimpreso la Compra N° " + Compra.idIngreso
            );
            new LAuditoria().Insertar(auditoria);
        }
    }
}
