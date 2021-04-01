using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DDetalle_Devolucion:Conexion
    {
        private int _IdDetalleDevolucion;
        public int idDetalleDevolucion
        {
            get { return _IdDetalleDevolucion; }
            set { _IdDetalleDevolucion = value; }
        }

        private int _IdDevolucion;
        public int idDevolucion
        {
            get { return _IdDevolucion; }
            set { _IdDevolucion = value; }
        }

        private int _IdDetalleVenta;
        public int idDetalleVenta
        {
            get { return _IdDetalleVenta; }
            set { _IdDetalleVenta = value; }
        }

        private int _IdArticulo;
        public int idArticulo
        {
            get { return _IdArticulo; }
            set { _IdArticulo = value; }
        }

        private int _Cantidad;
        public int cantidad
        {
            get { return _Cantidad; }
            set { _Cantidad = value; }
        }

        private double _Precio;
        public double precio
        {
            get { return _Precio; }
            set { _Precio = value; }
        }

        private int _Dañado;
        public int dañado
        {
            get { return _Dañado; }
            set { _Dañado = value; }
        }
        public DDetalle_Devolucion()
        {

        }

        public DDetalle_Devolucion(int IdDetalleDevolucion, int IdDevolucion, int IdDetalleVenta, int IdArticulo, int Cantidad, double Precio, int Dañado)
        {
            this.idDetalleDevolucion = IdDetalleDevolucion;
            this.idDevolucion = IdDevolucion;
            this.idDetalleVenta = IdDetalleVenta;
            this.idArticulo = IdArticulo;
            this.cantidad = Cantidad;
            this.precio = Precio;
            this.dañado = Dañado;
        }
    }
}
