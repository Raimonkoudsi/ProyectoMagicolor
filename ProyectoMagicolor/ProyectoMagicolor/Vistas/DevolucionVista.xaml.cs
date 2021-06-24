using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class DevolucionVista : Page
    {
        DevolucionDG ParentFrm;

        public DevolucionVista(DDevolucion devolucion, DevolucionDG par)
        {
            InitializeComponent();

            ParentFrm = par;

            this.Devolucion = devolucion;
        }

        public DDevolucion Devolucion;
        private LDevolucion Metodos = new LDevolucion();

        public List<DDetalle_Devolucion> DetalleDevoluciones = new List<DDetalle_Devolucion>();

        public List<ModeloFactura> ArticulosADevolver = new List<ModeloFactura>();



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DetalleDevoluciones = Metodos.MostrarDetalleDevolucion(Devolucion.idDevolucion);
            //factura
            txtFactura.Text = "#" + Devolucion.idVenta;
            txtFecha.Text = Devolucion.fechaVentaString;
            txtFechaDevolucion.Text = Devolucion.fechaString;

            //monto
            txtCantidad.Text = Devolucion.cantidad.ToString();
            double total = (Devolucion.montoDevolucion);
            txtTotal.Text = "Bs.S " + total.ToString("0.00");

            //trabajador
            txtVendedor.Text = Devolucion.trabajador;
            //cliente
            txtCliName.Text = Devolucion.nombreCliente;
            TxtCliDoc.Text = Devolucion.cedulaCliente;
            TxtCliTelf.Text = Devolucion.telefono == "" ? "Sin Teléfono" : Devolucion.telefono;
            TxtCliEmail.Text = Devolucion.email == "" ? "Sin Email" : Devolucion.email;

            int i = 0;
            foreach (DDetalle_Devolucion item in DetalleDevoluciones)
            {
                ModeloFactura modelo = new ModeloFactura(i, item.nombre,
                                                                item.precio,
                                                                item.cantidad,
                                                                (item.precio * item.cantidad));
                modelo.idDetalleVenta = item.idDetalleDevolucion;
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

        private void BtnFactura_Click(object sender, RoutedEventArgs e)
        {
            Reports.Reporte reporte = new Reports.Reporte();
            reporte.ExportPDFTwoArguments(Metodos.MostrarDetalleDevolucion(Devolucion.idDevolucion), "Devolucion", Metodos.MostrarDevolucion(Devolucion.idDevolucion), "DevolucionGeneral", true, Devolucion.idDevolucion.ToString());

            DAuditoria auditoria = new DAuditoria(
                Globals.ID_SISTEMA,
                "Generar",
                "Ha Generado el Comprobante de Reporte N° " + Devolucion.idDevolucion
            );
            new LAuditoria().Insertar(auditoria);
        }
    }

}
