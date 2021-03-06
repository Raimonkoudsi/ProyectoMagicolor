﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DDetalle_Venta:Conexion
    {

        private int _IdDetalleVenta;
        public int idDetalleVenta
        {
            get { return _IdDetalleVenta; }
            set { _IdDetalleVenta = value; }
        }

        private int _IdVenta;
        public int idVenta
        {
            get { return _IdVenta; }
            set { _IdVenta = value; }
        }

        private int _IdDetalleIngreso;
        public int idDetalleIngreso
        {
            get { return _IdDetalleIngreso; }
            set { _IdDetalleIngreso = value; }
        }

        private int _Cantidad;
        public int cantidad
        {
            get { return _Cantidad; }
            set { _Cantidad = value; }
        }

        private double _PrecioVenta;
        public double precioVenta
        {
            get { return _PrecioVenta; }
            set { _PrecioVenta = value; }
        }

        private double _Impuesto;
        public double impuesto
        {
            get { return _Impuesto; }
            set { _Impuesto = value; }
        }

        private string _Nombre;
        public string nombre
        {
            get { return _Nombre; }
            set { _Nombre = value; }
        }

        private string _Codigo;
        public string codigo
        {
            get { return _Codigo; }
            set { _Codigo = value; }
        }

        private int _IdArticulo;
        public int idArticulo
        {
            get { return _IdArticulo; }
            set { _IdArticulo = value; }
        }

        public DDetalle_Venta()
        {

        }

        public DDetalle_Venta(int IdDetalleVenta, int IdVenta, int IdDetalleIngreso, int Cantidad, double PrecioVenta, double Impuesto, int Estado)
        {
            this.idDetalleVenta = IdDetalleVenta;
            this.idVenta = IdVenta;
            this.idDetalleIngreso = IdDetalleIngreso;
            this.cantidad = Cantidad;
            this.precioVenta = PrecioVenta;
            this.impuesto = Impuesto;
        }
    }
}
