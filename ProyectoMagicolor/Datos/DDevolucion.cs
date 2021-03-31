using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DDevolucion:Conexion
    {

        private int _IdDevolucion;
        public int idDevolucion
        {
            get { return _IdDevolucion; }
            set { _IdDevolucion = value; }
        }

        private int _IdCliente;
        public int idCliente
        {
            get { return _IdCliente; }
            set { _IdCliente = value; }
        }

        private int _IdTrabajador;
        public int idTrabajador
        {
            get { return _IdTrabajador; }
            set { _IdTrabajador = value; }
        }

        private int _IdVenta;
        public int idVenta
        {
            get { return _IdVenta; }
            set { _IdVenta = value; }
        }

        private DateTime _Fecha;
        public DateTime fecha
        {
            get { return _Fecha; }
            set { _Fecha = value; }
        }

        private int _Cantidad;
        public int cantidad
        {
            get { return _Cantidad; }
            set { _Cantidad = value; }
        }

        public DDevolucion()
        {

        }

        public DDevolucion(int IdDevolucion, int IdCliente, int IdTrabajador, int IdVenta, DateTime Fecha)
        {
            this.idDevolucion = IdDevolucion;
            this.idCliente = IdCliente;
            this.idTrabajador = IdTrabajador;
            this.idVenta = IdVenta;
            this.fecha = Fecha;
        }
    }
}
