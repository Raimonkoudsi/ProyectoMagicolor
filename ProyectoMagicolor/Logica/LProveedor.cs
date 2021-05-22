using System;
using System.Collections.Generic;
using System.Text;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LProveedor : DProveedor
    {
        #region QUERIES 
        private string queryInsert = @"
            INSERT INTO [proveedor] (
                idProveedor,
                razonSocial,
                sectorComercial,
                tipoDocumento,
                numeroDocumento,
                direccion,
                telefono,
                email,
                url
            ) VALUES (
                @idProveedor,
                @razonSocial,
                @sectorComercial,
                @tipoDocumento,
                @numeroDocumento,
                @direccion,
                @telefono,
                @email,
                @url
            );
        ";

        private string queryUpdate = @"
            UPDATE [proveedor] SET 
                razonSocial = @razonSocial,
                sectorComercial = @sectorComercial,
                tipoDocumento = @tipoDocumento,
                numeroDocumento = @numeroDocumento,
                direccion = @direccion,
                telefono = @telefono,
                email = @email,
                url = @url
            WHERE idProveedor = @idProveedor;
        ";

        private string queryDelete = @"
            DELETE FROM [proveedor] 
            WHERE idProveedor = @idProveedor
        ";

        private string queryListGeneral = @"
            SELECT * FROM [proveedor] 
            WHERE tipoDocumento = @tipoDocumento AND numeroDocumento like @numeroDocumento + '%' 
            ORDER BY numeroDocumento
        ";

        private string queryListSpecific = @"
            SELECT * FROM [proveedor] 
            WHERE tipoDocumento = @tipoDocumento AND numeroDocumento = @numeroDocumento
        ";

        private string queryListID = @"
            SELECT * FROM [proveedor] 
            WHERE idProveedor = @idProveedor
        ";
        #endregion

        public string Insertar(DProveedor Proveedor)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idProveedor", LFunction.GetID("proveedor", "idProveedor"));
                comm.Parameters.AddWithValue("@razonSocial", Proveedor.razonSocial);
                comm.Parameters.AddWithValue("@sectorComercial", Proveedor.sectorComercial);
                comm.Parameters.AddWithValue("@tipoDocumento", Proveedor.tipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", Proveedor.numeroDocumento);
                comm.Parameters.AddWithValue("@direccion", Proveedor.direccion);
                comm.Parameters.AddWithValue("@telefono", Proveedor.telefono);
                comm.Parameters.AddWithValue("@email", Proveedor.email);
                comm.Parameters.AddWithValue("@url", Proveedor.url);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Registró el Proveedor";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Proveedor Ingresado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public string Editar(DProveedor Proveedor)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryUpdate, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@razonSocial", Proveedor.razonSocial);
                comm.Parameters.AddWithValue("@sectorComercial", Proveedor.sectorComercial);
                comm.Parameters.AddWithValue("@tipoDocumento", Proveedor.tipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", Proveedor.numeroDocumento);
                comm.Parameters.AddWithValue("@direccion", Proveedor.direccion);
                comm.Parameters.AddWithValue("@telefono", Proveedor.telefono);
                comm.Parameters.AddWithValue("@email", Proveedor.email);
                comm.Parameters.AddWithValue("@url", Proveedor.url);
                comm.Parameters.AddWithValue("@idProveedor", Proveedor.idProveedor);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Proveedor";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Proveedor Actualizado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public string Eliminar(int IdProveedor)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryDelete, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idProveedor", IdProveedor);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Eliminó el Proveedor";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Proveedor Eliminado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DProveedor> Mostrar(string TipoDocumento, string NumeroDocumento)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListGeneral, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@tipoDocumento", TipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", NumeroDocumento);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DProveedor
                    {
                        idProveedor = reader.GetInt32(0),
                        razonSocial = reader.GetString(1),
                        sectorComercial = reader.GetString(2),
                        tipoDocumento = reader.GetString(3),
                        numeroDocumento = reader.GetString(4),
                        direccion = reader.GetString(5),
                        telefono = reader.GetString(6),
                        email = reader.GetString(7),
                        url = reader.GetString(8)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DProveedor> Encontrar(int IdProveedor)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListID, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idProveedor", IdProveedor);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DProveedor
                    {
                        idProveedor = reader.GetInt32(0),
                        razonSocial = reader.GetString(1),
                        sectorComercial = reader.GetString(2),
                        tipoDocumento = reader.GetString(3),
                        numeroDocumento = reader.GetString(4),
                        direccion = reader.GetString(5),
                        telefono = reader.GetString(6),
                        email = reader.GetString(7),
                        url = reader.GetString(8)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;

        }


        public List<DProveedor> EncontrarConDocumento(string TipoDocumento, string NumeroDocumento)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListSpecific, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@tipoDocumento", TipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", NumeroDocumento);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DProveedor
                    {
                        idProveedor = reader.GetInt32(0),
                        razonSocial = reader.GetString(1),
                        sectorComercial = reader.GetString(2),
                        tipoDocumento = reader.GetString(3),
                        numeroDocumento = reader.GetString(4),
                        direccion = reader.GetString(5),
                        telefono = reader.GetString(6),
                        email = reader.GetString(7),
                        url = reader.GetString(8)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }
    }
}
