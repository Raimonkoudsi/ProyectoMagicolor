using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DRegistro_CuentaPagar:Conexion
    {
        private int _IdRegistro;
        public int idRegistro
        {
            get { return _IdRegistro; }
            set { _IdRegistro = value; }
        }

        private int _IdCuentaPagar;
        public int idCuentaPagar
        {
            get { return _IdCuentaPagar; }
            set { _IdCuentaPagar = value; }
        }

        private double _Monto;
        public double monto
        {
            get { return _Monto; }
            set { _Monto = value; }
        }

        private DateTime _Fecha;
        public DateTime fecha
        {
            get { return _Fecha; }
            set { _Fecha = value; }
        }

        public DRegistro_CuentaPagar()
        {

        }

        public DRegistro_CuentaPagar(int IdRegistro, int IdCuentaPagar, double Monto, DateTime Fecha)
        {
            this.idRegistro = IdRegistro;
            this.idCuentaPagar = IdCuentaPagar;
            this.monto = Monto;
            this.fecha = Fecha;
        }
    }
}
