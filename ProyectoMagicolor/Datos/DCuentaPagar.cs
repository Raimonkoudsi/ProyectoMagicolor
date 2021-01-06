using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DCuentaPagar:Conexion
    {

        private int _IdCuentaPagar;
        public int idCuentaPagar
        {
            get { return _IdCuentaPagar; }
            set { _IdCuentaPagar = value; }
        }

        private int _IdIngreso;
        public int idIngreso
        {
            get { return _IdIngreso; }
            set { _IdIngreso = value; }
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

        public DCuentaPagar()
        {

        }

        public DCuentaPagar(int IdCuentaPagar, int IdIngreso, DateTime FechaInicio, DateTime FechaLimite, double MontoIngresado)
        {
            this.idCuentaPagar = IdCuentaPagar;
            this.idIngreso = IdIngreso;
            this.fechaInicio = FechaInicio;
            this.fechaLimite = FechaLimite;
            this.montoIngresado = MontoIngresado;
        }
    }
}
