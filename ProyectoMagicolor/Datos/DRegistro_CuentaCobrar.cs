using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DRegistro_CuentaCobrar:Conexion
    {

        private int _IdRegistro;
        public int idRegistro
        {
            get { return _IdRegistro; }
            set { _IdRegistro = value; }
        }

        private int _IdCuentaCobrar;
        public int idCuentaCobrar
        {
            get { return _IdCuentaCobrar; }
            set { _IdCuentaCobrar = value; }
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

        public DRegistro_CuentaCobrar()
        {

        }

        public DRegistro_CuentaCobrar(int IdRegistro, int IdCuentaCobrar, double Monto, DateTime Fecha)
        {
            this.idRegistro = IdRegistro;
            this.idCuentaCobrar = IdCuentaCobrar;
            this.monto = Monto;
            this.fecha = Fecha;
        }
    }
}
