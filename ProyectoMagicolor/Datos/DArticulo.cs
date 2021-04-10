using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DArticulo:Conexion
    {

        private int _IdArticulo;
        public int idArticulo
        {
            get { return _IdArticulo; }
            set { _IdArticulo = value; }
        }

        private string _Codigo;
        public string codigo
        {
            get { return _Codigo; }
            set { _Codigo = value; }
        }

        private string _Nombre;
        public string nombre
        {
            get { return _Nombre; }
            set { _Nombre = value; }
        }

        private string _Descripcion;
        public string descripcion
        {
            get { return _Descripcion; }
            set { _Descripcion = value; }
        }

        private int _StockMinimo;
        public int stockMinimo
        {
            get { return _StockMinimo; }
            set { _StockMinimo = value; }
        }

        private int _StockMaximo;
        public int stockMaximo
        {
            get { return _StockMaximo; }
            set { _StockMaximo = value; }
        }

        private int _IdCategoria;
        public int idCategoria
        {
            get { return _IdCategoria; }
            set { _IdCategoria = value; }
        }

        private double _PrecioVenta;
        public double precioVenta
        {
            get { return _PrecioVenta; }
            set { _PrecioVenta = value; }
        }

        private int _CantidadInicial;
        public int cantidadInicial
        {
            get { return _CantidadInicial; }
            set { _CantidadInicial = value; }
        }

        private int _CantidadActual;
        public int cantidadActual
        {
            get { return _CantidadActual; }
            set { _CantidadActual = value; }
        }

        private int _CantidadVendida;
        public int cantidadVendida
        {
            get { return _CantidadVendida; }
            set { _CantidadVendida = value; }
        }

        private string _Categoria;
        public string categoria
        {
            get { return _Categoria; }
            set { _Categoria = value; }
        }

        private int _CantidadComprada;
        public int cantidadComprada
        {
            get { return _CantidadComprada; }
            set { _CantidadComprada = value; }
        }

        private int _CantidadDevuelta;
        public int cantidadDevuelta
        {
            get { return _CantidadDevuelta; }
            set { _CantidadDevuelta = value; }
        }

        private int _CantidadCliente;
        public int cantidadCliente
        {
            get { return _CantidadCliente; }
            set { _CantidadCliente = value; }
        }

        private double _Subtotal;
        public double subtotal
        {
            get { return _Subtotal; }
            set { _Subtotal = value; }
        }

        private double _Impuesto;
        public double impuesto
        {
            get { return _Impuesto; }
            set { _Impuesto = value; }
        }

        private double _Total;
        public double total
        {
            get { return _Total; }
            set { _Total = value; }
        }

        private double _SubtotalDevolucion;
        public double subtotalDevolucion
        {
            get { return _SubtotalDevolucion; }
            set { _SubtotalDevolucion = value; }
        }

        private double _TotalDevolucion;
        public double totalDevolucion
        {
            get { return _TotalDevolucion; }
            set { _TotalDevolucion = value; }
        }

        private double _PrecioUnidad;
        public double precioUnidad
        {
            get { return _PrecioUnidad; }
            set { _PrecioUnidad = value; }
        }

        private double _CompraVendida;
        public double compraVendida
        {
            get { return _CompraVendida; }
            set { _CompraVendida = value; }
        }

        private double _TotalNeto;
        public double totalNeto
        {
            get { return _TotalNeto; }
            set { _TotalNeto = value; }
        }


        public DArticulo()
        {

        }

        public DArticulo(int IdArticulo,string Codigo, string Nombre, string Descripcion, int StockMinimo, int StockMaximo, int IdCategoria)
        {
            this.idArticulo = IdArticulo;
            this.codigo = Codigo;
            this.nombre = Nombre;
            this.descripcion = Descripcion;
            this.stockMinimo = StockMinimo;
            this.stockMaximo = StockMaximo;
            this.idCategoria = IdCategoria;
        }
    }
}
