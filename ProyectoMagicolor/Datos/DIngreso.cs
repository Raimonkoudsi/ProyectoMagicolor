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

        private double _MontoTotal;
        public double montoTotal
        {
            get { return _MontoTotal; }
            set { _MontoTotal = value; }
        }


        public DIngreso()
        {

        }

        public DIngreso(int IdIngreso, int IdTrabajador, int IdProveedor, DateTime Fecha, string Factura, double Impuesto, int MetodoPago, int Estado, string Cedula, string RazonSocial, double MontoTotal)
        {
            this.idIngreso = IdIngreso;
            this.idTrabajador = IdTrabajador;
            this.idProveedor = IdProveedor;
            this.fecha = Fecha;
            this.factura = Factura;
            this.impuesto = Impuesto;
            this.metodoPago = MetodoPago;
            this.estado = Estado;
            this.cedulaTrabajador = Cedula;
            this.razonSocial = RazonSocial;
            this.montoTotal = MontoTotal;
        }


    }
}
