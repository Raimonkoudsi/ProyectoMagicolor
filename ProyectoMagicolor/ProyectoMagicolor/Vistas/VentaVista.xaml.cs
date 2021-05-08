﻿using System;
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
    public partial class VentaVista : Page
    {
        VentaDG ParentFrm;

        public VentaVista(DVenta venta, VentaDG par)
        {
            InitializeComponent();

            ParentFrm = par;

            this.Venta = venta;
        }

        public DVenta Venta;
        private LVenta Metodos = new LVenta();

        public List<DDetalle_Venta> DetalleVentas = new List<DDetalle_Venta>();

        public List<ModeloFactura> ArticulosADevolver = new List<ModeloFactura>();



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DetalleVentas = Metodos.MostrarDetalleVenta(Venta.idVenta);
            //factura
            txtFactura.Text = "#" + Venta.idVenta;
            txtFecha.Text = Venta.fechaString;

            //monto
            double subTotal = (Venta.montoTotal / ((Venta.impuesto / 100.00) + 1));
            txtSubtotal.Text = "Bs.S " + subTotal.ToString("0.00");
            double impuesto = (Venta.montoTotal - subTotal);
            txtImpuesto.Text = "Bs.S " + impuesto.ToString("0.00");
            double descuento = (Venta.descuento);
            txtDescuento.Text = "Bs.S " + descuento.ToString("0.00");
            double total = (Venta.montoTotal);
            txtTotal.Text = "Bs.S " + total.ToString("0.00");

            //trabajador
            txtVendedor.Text = Venta.trabajador;
            //cliente
            txtCliName.Text = Venta.cliente;
            TxtCliDoc.Text = Venta.cedulaCliente;
            TxtCliTelf.Text = Venta.telefonoCliente;
            TxtCliEmail.Text = Venta.emailCliente;

            int i = 0;
            foreach (DDetalle_Venta item in DetalleVentas)
            {
                ModeloFactura modelo = new ModeloFactura(i, item.nombre,
                                                                item.precioVenta,
                                                                item.cantidad,
                                                                (item.precioVenta * item.cantidad));
                modelo.idDetalleVenta = item.idDetalleVenta;
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
            var resp = MessageBox.Show("¿Desea Anular la Venta?", "Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (resp == MessageBoxResult.No)
                return;
            else
            {
                string metodoAnular = Metodos.Anular(Venta.idVenta, new LVenta().MostrarDetalleVenta(Venta.idVenta));
                if (metodoAnular == "OK") ParentFrm.GetBack();
            }
        }

        private void BtnFactura_Click(object sender, RoutedEventArgs e)
        {
            Reports.Reporte reporte = new Reports.Reporte();

            reporte.ExportPDFTwoArguments(Metodos.MostrarDetalleVenta(Venta.idVenta), "Venta", Metodos.MostrarVenta(Venta.idVenta), "VentaGeneral", true, Venta.idVenta.ToString());
        }
    }


    public class ModeloFactura
    {
        public ModeloFactura(int id, string nombreArticulo, double precioUnitario, int cantidad, double precioTotal)
        {
            Id = id;
            NombreArticulo = nombreArticulo;
            PrecioUnitario = precioUnitario;
            Cantidad = cantidad;
            PrecioTotal = precioTotal;
        }

        public int Id { get; set; }

        public string NombreArticulo { get; set; }
        public double PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public double PrecioTotal { get; set; }

        public int idDetalleVenta;
        public int idArticulo;
    }
}
