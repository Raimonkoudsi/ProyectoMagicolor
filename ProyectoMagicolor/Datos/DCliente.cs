using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DCliente : Conexion
    {
        private int _idCliente;
        public int idCliente
        {
            get { return _idCliente; }
            set { _idCliente = value; }
        }

        private string _Nombre;
        public string nombre
        {
            get { return _Nombre; }
            set { _Nombre = value; }
        }

        private string _Apellidos;
        public string apellidos
        {
            get { return _Apellidos; }
            set { _Apellidos = value; }
        }

        private string _TipoDocumento;
        public string tipoDocumento
        {
            get { return _TipoDocumento; }
            set { _TipoDocumento = value; }
        }

        private string _NumeroDocumento;
        public string numeroDocumento
        {
            get { return _NumeroDocumento; }
            set { _NumeroDocumento = value; }
        }

        private string _Direccion;
        public string direccion
        {
            get { return _Direccion; }
            set { _Direccion = value; }
        }

        private string _Telefono;
        public string telefono
        {
            get { return _Telefono; }
            set { _Telefono = value; }
        }

        private string _Email;
        public string email
        {
            get { return _Email; }
            set { _Email = value; }
        }


        public DCliente()
        {

        }

        public DCliente(int IdCliente, string Nombre, string Apellidos, string TipoDocumento, string NumeroDocumento, string Direccion, string Telefono, string Email)
        {
            this.idCliente = IdCliente;
            this.nombre = Nombre;
            this.apellidos = Apellidos;
            this.tipoDocumento = TipoDocumento;
            this.numeroDocumento = NumeroDocumento;
            this.direccion = Direccion;
            this.telefono = Telefono;
            this.email = Email;
        }
    }
}
