using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DCategoria:Conexion
    {

        private int _idCategoria;
        public int idCategoria
        {
            get { return _idCategoria; }
            set { _idCategoria = value; }
        }

        private string _Nombre;
        public string nombre
        {
            get { return _Nombre; }
            set { _Nombre = value; }
        }

        private string _Descripcion;
        public string descripcion
        {
            get { return _Descripcion; }
            set { _Descripcion = value; }
        }

        private int _Estado;
        public int estado
        {
            get { return _Estado; }
            set { _Estado = value; }
        }


        public DCategoria()
        {

        }

        public DCategoria(int IdCategoria, string Nombre, string Descripcion)
        {
            this.idCategoria = IdCategoria;
            this.nombre = Nombre;
            this.descripcion = Descripcion;
        }
    }
}
