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

        private string _Motivo;
        public string motivo
        {
            get { return _Motivo; }
            set { _Motivo = value; }
        }

        private int _Cantidad;
        public int cantidad
        {
            get { return _Cantidad; }
            set { _Cantidad = value; }
        }

        private string _CedulaCliente;
        public string cedulaCliente
        {
            get { return _CedulaCliente; }
            set { _CedulaCliente = value; }
        }

        private string _NombreCliente;
        public string nombreCliente
        {
            get { return _NombreCliente; }
            set { _NombreCliente = value; }
        }

        private string _NombreArticulo;
        public string nombreArticulo
        {
            get { return _NombreArticulo; }
            set { _NombreArticulo = value; }
        }

        private double _MontoDevolucion;
        public double montoDevolucion
        {
            get { return _MontoDevolucion; }
            set { _MontoDevolucion = value; }
        }

        private string _Trabajador;
        public string trabajador
        {
            get { return _Trabajador; }
            set { _Trabajador = value; }
        }

        private DateTime _FechaVenta;
        public DateTime fechaVenta
        {
            get { return _FechaVenta; }
            set { _FechaVenta = value; }
        }

        private string _Telefono;
        public string telefono
        {
            get { return _Telefono; }
            set { _Telefono = value; }
        }

        private string _Email;
        public string email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        private string _FechaString;
        public string fechaString
        {
            get { return _FechaString; }
            set { _FechaString = value; }
        }

        private string _FechaVentaString;
        public string fechaVentaString
        {
            get { return _FechaVentaString; }
            set { _FechaVentaString = value; }
        }

        private string _NombreTrabajadorIngresado;
        public string nombreTrabajadorIngresado
        {
            get { return _NombreTrabajadorIngresado; }
            set { _NombreTrabajadorIngresado = value; }
        }

        private string _MontoDevolucionString;
        public string montoDevolucionString
        {
            get { return _MontoDevolucionString; }
            set { _MontoDevolucionString = value; }
        }

        public DDevolucion()
        {

        }

        public DDevolucion(int IdDevolucion, int IdCliente, int IdTrabajador, int IdVenta, DateTime Fecha, string Motivo)
        {
            this.idDevolucion = IdDevolucion;
            this.idCliente = IdCliente;
            this.idTrabajador = IdTrabajador;
            this.idVenta = IdVenta;
            this.fecha = Fecha;
            this.motivo = Motivo;
        }
    }
}
