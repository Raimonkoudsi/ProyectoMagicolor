using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DSeguridad:Conexion
    {

        private int _IdSeguridad;
        public int idSeguridad
        {
            get { return _IdSeguridad; }
            set { _IdSeguridad = value; }
        }

        private int _IdTrabajador;
        public int idTrabajador
        {
            get { return _IdTrabajador; }
            set { _IdTrabajador = value; }
        }

        private string _Pregunta;
        public string pregunta
        {
            get { return _Pregunta; }
            set { _Pregunta = value; }
        }

        private string _Respuesta;
        public string respuesta
        {
            get { return _Respuesta; }
            set { _Respuesta = value; }
        }

        public DSeguridad()
        {

        }

        public DSeguridad(int IdTrabajador, string Pregunta, string Respuesta)
        {
            this.idTrabajador = IdTrabajador;
            this.pregunta = Pregunta;
            this.respuesta = Respuesta;
        }
    }
}
