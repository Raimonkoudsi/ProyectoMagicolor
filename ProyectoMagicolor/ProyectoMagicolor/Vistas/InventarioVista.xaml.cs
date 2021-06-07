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

using Logica;
using Datos;

namespace ProyectoMagicolor.Vistas
{
    public partial class InventarioVista : Window
    {
        public InventarioVista(DArticulo Datos)
        {
            InitializeComponent();

            InventarioDatos = Datos;
        }

        DArticulo InventarioDatos;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            string bs = " Bs. S";

            //articulo
            txtCodigoArticulo.Text = InventarioDatos.codigo;
            txtNombreArticulo.Text = InventarioDatos.nombre;
            txtCantidadArticulo.Text = "(" + InventarioDatos.cantidadActual.ToString() + " Unidades Restantes)";
            txtDescripcionArticulo.Text = InventarioDatos.descripcion;
            txtCategoriaArticulo.Text = InventarioDatos.categoria;
            txtStockMinArticulo.Text = InventarioDatos.stockMinimo.ToString();
            txtStockMaxArticulo.Text = InventarioDatos.stockMaximo.ToString();


            //Cuadros de Unidades
            txtUnidadesVentas.Text = InventarioDatos.cantidadVendida.ToString();
            txtUnidadesCompras.Text = InventarioDatos.cantidadComprada.ToString();
            txtUnidadesDevoluciones.Text = InventarioDatos.cantidadDevuelta.ToString();
            txtClientes.Text = InventarioDatos.cantidadCliente.ToString();


            //Montos
            txtVentasMonto.Text = InventarioDatos.total.ToString() + bs;
            txtImpuestoVentas.Text = (InventarioDatos.total - InventarioDatos.subtotal).ToString() + bs;
            txtVentasTotal.Text = InventarioDatos.subtotal.ToString() + bs;

            txtComprasMonto.Text = InventarioDatos.subtotalCompra.ToString() + bs;
            txtImpuestoCompras.Text = (InventarioDatos.totalCompra - InventarioDatos.subtotalCompra).ToString() + bs;
            txtComprasTotal.Text = InventarioDatos.totalCompra.ToString() + bs;

            txtDevolucionesMonto.Text = InventarioDatos.subtotalDevolucion.ToString() + bs;
            txtImpuestoDevoluciones.Text = (InventarioDatos.totalDevolucion - InventarioDatos.subtotalDevolucion).ToString() + bs;
            txtDevolucionesTotal.Text = InventarioDatos.totalDevolucion.ToString() + bs;

            double total = InventarioDatos.subtotal - InventarioDatos.subtotalCompra - InventarioDatos.subtotalDevolucion + (InventarioDatos.totalDevolucion - InventarioDatos.subtotalDevolucion);

            txtGananciasNetas.Text = total.ToString() + bs;

            LProveedor Metodos = new LProveedor();

            List<DProveedor> items = Metodos.ListadoProveedorArticulo(InventarioDatos.idArticulo);
            dgOperaciones.ItemsSource = items;

            List<DProveedor> items2 = Metodos.ListadoProveedorCategoria(InventarioDatos.categoria);
            dgOperaciones2.ItemsSource = items2;
        }

        private void btnVer_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
