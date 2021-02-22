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

        private byte[] _Imagen;
        public byte[] imagen
        {
            get { return _Imagen; }
            set { _Imagen = value; }
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

        public DArticulo()
        {

        }

        public DArticulo(int IdArticulo,string Codigo, string Nombre, string Descripcion, byte[] Imagen, int StockMinimo, int StockMaximo, int IdCategoria, double PrecioVenta, int CantidadInicial, int CantidadActual, int CantidadVendida)
        {
            this.idArticulo = IdArticulo;
            this.codigo = Codigo;
            this.nombre = Nombre;
            this.descripcion = Descripcion;
            this.imagen = Imagen;
            this.stockMinimo = StockMinimo;
            this.stockMaximo = StockMaximo;
            this.idCategoria = IdCategoria;
            this.precioVenta = PrecioVenta;
            this.cantidadInicial = CantidadInicial;
            this.cantidadActual = CantidadActual;
            this.cantidadVendida = CantidadVendida;
        }
    }
}
