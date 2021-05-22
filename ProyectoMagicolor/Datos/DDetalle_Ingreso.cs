using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DDetalle_Ingreso:Conexion
    {

        private int _IdDetalleIngreso;
        public int idDetalleIngreso
        {
            get { return _IdDetalleIngreso; }
            set { _IdDetalleIngreso = value; }
        }

        private int _IdIngreso;
        public int idIngreso
        {
            get { return _IdIngreso; }
            set { _IdIngreso = value; }
        }

        private int _IdArticulo;
        public int idArticulo
        {
            get { return _IdArticulo; }
            set { _IdArticulo = value; }
        }

        private double _PrecioCompra;
        public double precioCompra
        {
            get { return _PrecioCompra; }
            set { _PrecioCompra = value; }
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

        private int _Cantidad;
        public int cantidad
        {
            get { return _Cantidad; }
            set { _Cantidad = value; }
        }

        public DDetalle_Ingreso()
        {

        }

        public DDetalle_Ingreso(int IdDetalleIngreso, int IdIngreso, int IdArticulo, double PrecioCompra, double PrecioVenta, int CantidadInicial, int CantidadActual)
        {
            this.idDetalleIngreso = IdDetalleIngreso;
            this.idIngreso = IdIngreso;
            this.idArticulo = IdArticulo;
            this.precioCompra = PrecioCompra;
            this.precioVenta = PrecioVenta;
            this.cantidadInicial = CantidadInicial;
            this.cantidadActual = CantidadActual;
        }

    }
}
