using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DIngreso:Conexion
    {

        private int _IdIngreso;
        public int idIngreso
        {
            get { return _IdIngreso; }
            set { _IdIngreso = value; }
        }

        private int _IdTrabajador;
        public int idTrabajador
        {
            get { return _IdTrabajador; }
            set { _IdTrabajador = value; }
        }

        private int _IdProveedor;
        public int idProveedor
        {
            get { return _IdProveedor; }
            set { _IdProveedor = value; }
        }

        private DateTime _Fecha;
        public DateTime fecha
        {
            get { return _Fecha; }
            set { _Fecha = value; }
        }

        private string _Factura;
        public string factura
        {
            get { return _Factura; }
            set { _Factura = value; }
        }

        private double _Impuesto;
        public double impuesto
        {
            get { return _Impuesto; }
            set { _Impuesto = value; }
        }

        private int _MetodoPago;
        public int metodoPago
        {
            get { return _MetodoPago; }
            set { _MetodoPago = value; }
        }

        private int _Estado;
        public int estado
        {
            get { return _Estado; }
            set { _Estado = value; }
        }

        private string _CedulaTrabajador;
        public string cedulaTrabajador
        {
            get { return _CedulaTrabajador; }
            set { _CedulaTrabajador = value; }
        }

        private string _RazonSocial;
        public string razonSocial
        {
            get { return _RazonSocial; }
            set { _RazonSocial = value; }
        }

        private double _Monto;
        public double monto
        {
            get { return _Monto; }
            set { _Monto = value; }
        }

        private double _MontoTotal;
        public double montoTotal
        {
            get { return _MontoTotal; }
            set { _MontoTotal = value; }
        }

        private int _IdCuentaPagar;
        public int idCuentaPagar
        {
            get { return _IdCuentaPagar; }
            set { _IdCuentaPagar = value; }
        }

        private string _CedulaProveedor;
        public string cedulaProveedor
        {
            get { return _CedulaProveedor; }
            set { _CedulaProveedor = value; }
        }

        private string _FechaString;
        public string fechaString
        {
            get { return _FechaString; }
            set { _FechaString = value; }
        }

        private string _MetodoPagoString;
        public string metodoPagoString
        {
            get { return _MetodoPagoString; }
            set { _MetodoPagoString = value; }
        }

        private string _EstadoString;
        public string estadoString
        {
            get { return _EstadoString; }
            set { _EstadoString = value; }
        }

        private string _Trabajador;
        public string trabajador
        {
            get { return _Trabajador; }
            set { _Trabajador = value; }
        }

        private string _TelefonoProveedor;
        public string telefonoProveedor
        {
            get { return _TelefonoProveedor; }
            set { _TelefonoProveedor = value; }
        }

        private string _EmailProveedor;
        public string emailProveedor
        {
            get { return _EmailProveedor; }
            set { _EmailProveedor = value; }
        }

        private string _NombreTrabajadorIngresado;
        public string nombreTrabajadorIngresado
        {
            get { return _NombreTrabajadorIngresado; }
            set { _NombreTrabajadorIngresado = value; }
        }

        public DIngreso()
        {

        }

        public DIngreso(int IdIngreso, int IdTrabajador, int IdProveedor, DateTime Fecha, string Factura, double Impuesto, int MetodoPago, int Estado)
        {
            this.idIngreso = IdIngreso;
            this.idTrabajador = IdTrabajador;
            this.idProveedor = IdProveedor;
            this.fecha = Fecha;
            this.factura = Factura;
            this.impuesto = Impuesto;
            this.metodoPago = MetodoPago;
            this.estado = Estado;
        }


    }
}
