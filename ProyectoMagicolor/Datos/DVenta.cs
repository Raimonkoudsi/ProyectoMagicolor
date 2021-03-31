using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DVenta : Conexion
    {
        private int _IdVenta;
        public int idVenta
        {
            get { return _IdVenta; }
            set { _IdVenta = value; }
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

        private DateTime _Fecha;
        public DateTime fecha
        {
            get { return _Fecha; }
            set { _Fecha = value; }
        }

        private double _Descuento;
        public double descuento
        {
            get { return _Descuento; }
            set { _Descuento = value; }
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

        private string _CedulaCliente;
        public string cedulaCliente
        {
            get { return _CedulaCliente; }
            set { _CedulaCliente = value; }
        }

        private string _Cliente;
        public string cliente
        {
            get { return _Cliente; }
            set { _Cliente = value; }
        }

        private double _Impuesto;
        public double impuesto
        {
            get { return _Impuesto; }
            set { _Impuesto = value; }
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

        private int _IdCuentaCobrar;
        public int idCuentaCobrar
        {
            get { return _IdCuentaCobrar; }
            set { _IdCuentaCobrar = value; }
        }



        public DVenta()
        {

        }

        public DVenta(int IdVenta,int IdCliente, int IdTrabajador, DateTime Fecha, double Descuento, double Impuesto,int MetodoPago, int Estado)
        {
            this.idVenta = IdVenta;
            this.idCliente = IdCliente;
            this.idTrabajador = IdTrabajador;
            this.fecha = Fecha;
            this.descuento = Descuento;
            this.impuesto = Impuesto;
            this.metodoPago = MetodoPago;
            this.estado = Estado;
        }

    }
}
