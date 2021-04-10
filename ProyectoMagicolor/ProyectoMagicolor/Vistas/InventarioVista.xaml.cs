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
    /// <summary>
    /// Interaction logic for InventarioVista.xaml
    /// </summary>
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

            string bs = "Bs. ";

            //articulo
            txtCodigoArticulo.Text = InventarioDatos.codigo;
            txtNombreArticulo.Text = InventarioDatos.nombre;
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
            txtVentasMonto.Text = bs + InventarioDatos.total.ToString();
            txtImpuestoVentas.Text = bs + (InventarioDatos.total - InventarioDatos.subtotal).ToString();
            txtVentasTotal.Text = bs + InventarioDatos.subtotal.ToString();

            txtDevolucionesMonto.Text = bs + InventarioDatos.subtotalDevolucion.ToString();
            txtImpuestosDevoluciones.Text = bs + (InventarioDatos.totalDevolucion - InventarioDatos.subtotalDevolucion).ToString();
            txtDevolucionesTotal.Text = bs + InventarioDatos.totalDevolucion.ToString();

            txtCompraUnitaria.Text = bs + InventarioDatos.precioUnidad.ToString();
            txtCompraVendido.Text = bs + InventarioDatos.compraVendida.ToString();
            txtGananciasNetas.Text = bs + (InventarioDatos.subtotal - InventarioDatos.compraVendida).ToString(); ;
        }
    }
}
