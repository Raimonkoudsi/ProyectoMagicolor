using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DProveedor : Conexion
    {
        private int _idProveedor;
        public int idProveedor
        {
            get { return _idProveedor; }
            set { _idProveedor = value; }
        }

        private string _RazonSocial;
        public string razonSocial
        {
            get { return _RazonSocial; }
            set { _RazonSocial = value; }
        }

        private string _SectorComercial;
        public string sectorComercial
        {
            get { return _SectorComercial; }
            set { _SectorComercial = value; }
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

        private string _Url;
        public string url
        {
            get { return _Url; }
            set { _Url = value; }
        }


        public DProveedor()
        {

        }

        public DProveedor(int IdProveedor, string RazonSocial, string SectorComercial, string TipoDocumento, string NumeroDocumento, string Direccion, string Telefono, string Email, string Url)
        {
            this.idProveedor = IdProveedor;
            this.razonSocial = RazonSocial;
            this.sectorComercial = SectorComercial;
            this.tipoDocumento = TipoDocumento;
            this.numeroDocumento = NumeroDocumento;
            this.direccion = Direccion;
            this.telefono = Telefono;
            this.email = Email;
            this.url = Url;
        }
    }
}
