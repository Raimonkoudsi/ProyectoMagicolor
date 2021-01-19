using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DCuentaCobrar:Conexion
    {


        private int _IdCuentaCobrar;
        public int idCuentaCobrar
        {
            get { return _IdCuentaCobrar; }
            set { _IdCuentaCobrar = value; }
        }

        private int _IdVenta;
        public int idVenta
        {
            get { return _IdVenta; }
            set { _IdVenta = value; }
        }

        private DateTime _FechaInicio;
        public DateTime fechaInicio
        {
            get { return _FechaInicio; }
            set { _FechaInicio = value; }
        }

        private DateTime _FechaLimite;
        public DateTime fechaLimite
        {
            get { return _FechaLimite; }
            set { _FechaLimite = value; }
        }

        private double _MontoIngresado;
        public double montoIngresado
        {
            get { return _MontoIngresado; }
            set { _MontoIngresado = value; }
        }

        private int _Estado;
        public int estado
        {
            get { return _Estado; }
            set { _Estado = value; }
        }

        public DCuentaCobrar()
        {

        }

        public DCuentaCobrar(int IdCuentaCobrar, int IdVenta, DateTime FechaInicio, DateTime FechaLimite, double MontoIngresado, int Estado)
        {
            this.idCuentaCobrar = IdCuentaCobrar;
            this.idVenta = IdVenta;
            this.fechaInicio = FechaInicio;
            this.fechaLimite = FechaLimite;
            this.montoIngresado = MontoIngresado;
            this.estado = Estado;
        }
    }
}
