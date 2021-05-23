using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DAuditoria:Conexion
    {

        private int _IdAuditoria;
        public int idAuditoria
        {
            get { return _IdAuditoria; }
            set { _IdAuditoria = value; }
        }

        private int _IdTrabajador;
        public int idTrabajador
        {
            get { return _IdTrabajador; }
            set { _IdTrabajador = value; }
        }

        private string _Accion;
        public string accion
        {
            get { return _Accion; }
            set { _Accion = value; }
        }

        private string _Descripcion;
        public string descripcion
        {
            get { return _Descripcion; }
            set { _Descripcion = value; }
        }

        private DateTime _Fecha;
        public DateTime fecha
        {
            get { return _Fecha; }
            set { _Fecha = value; }
        }

        private string _Usuario;
        public string usuario
        {
            get { return _Usuario; }
            set { _Usuario = value; }
        }

        private string _AccesoString;
        public string accesoString
        {
            get { return _AccesoString; }
            set { _AccesoString = value; }
        }


        public DAuditoria()
        {

        }

        public DAuditoria(int IdTrabajador,string Accion, string Descripcion, DateTime Fecha)
        {
            this.idAuditoria = IdTrabajador;
            this.accion = Accion;
            this.descripcion = Descripcion;
            this.fecha = Fecha;
        }
    }
}
