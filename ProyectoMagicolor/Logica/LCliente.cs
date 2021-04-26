using System;
using System.Collections.Generic;
using System.Text;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LCliente : DCliente
    {
        #region QUERIES
        private string queryInsert = @"
            INSERT INTO [cliente] (
                idCliente,
                nombre,
                apellidos,
                tipoDocumento,
                numeroDocumento,
                direccion,
                telefono,
                email
            ) VALUES (
                @idCliente,
                @nombre,
                @apellidos,
                @tipoDocumento,
                @numeroDocumento,
                @direccion,
                @telefono,
                @email
            );
	    ";

        private string queryUpdate = @"
            UPDATE [cliente] SET 
                nombre = @nombre,
                apellidos = @apellidos,
                tipoDocumento = @tipoDocumento,
                numeroDocumento = @numeroDocumento,
                direccion = @direccion,
                telefono = @telefono,
                email = @email
            WHERE idCliente = @idCliente;
	    ";

        private string queryDelete = @"
            DELETE * FROM [cliente] 
            WHERE idCliente = @idCliente;
	    ";

        private string queryList = @"
            SELECT * FROM [cliente] 
            WHERE tipoDocumento = @tipoDocumento AND numeroDocumento LIKE @numeroDocumento + '%' 
            ORDER BY numeroDocumento;
        ";

        private string queryListID = @"
            SELECT * FROM [cliente] 
            WHERE idCliente = @idCliente;
        ";

        private string queryListDocument = @"
            SELECT * FROM [cliente] 
            WHERE tipoDocumento = @tipoDocumento AND numeroDocumento = @numeroDocumento
        ";
        #endregion


        public string Insertar(DCliente Client)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idCliente", LFunction.GetID("cliente", "idCliente"));
                comm.Parameters.AddWithValue("@nombre", Client.nombre);
                comm.Parameters.AddWithValue("@apellidos", Client.apellidos);
                comm.Parameters.AddWithValue("@tipoDocumento", Client.tipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", Client.numeroDocumento);
                comm.Parameters.AddWithValue("@direccion", Client.direccion);
                comm.Parameters.AddWithValue("@telefono", Client.telefono);
                comm.Parameters.AddWithValue("@email", Client.email);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro del Cliente";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Cliente Ingresado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public string Editar(DCliente Client)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryUpdate, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@nombre", Client.nombre);
                comm.Parameters.AddWithValue("@apellidos", Client.apellidos);
                comm.Parameters.AddWithValue("@tipoDocumento", Client.tipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", Client.numeroDocumento);
                comm.Parameters.AddWithValue("@direccion", Client.direccion);
                comm.Parameters.AddWithValue("@telefono", Client.telefono);
                comm.Parameters.AddWithValue("@email", Client.email);
                comm.Parameters.AddWithValue("@idCliente", Client.idCliente);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Registro del Cliente";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Cliente Actualizado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public string Eliminar(int IdCliente)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryDelete, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idCliente", IdCliente);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Eliminó el Registro del Cliente";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Cliente Eliminado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DCliente> Mostrar(string TipoDocumento, string NumeroDocumento)
        {
            List<DCliente> ListaGenerica = new List<DCliente>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@tipoDocumento", TipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", NumeroDocumento);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DCliente
                    {
                        idCliente = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        apellidos = reader.GetString(2),
                        tipoDocumento = reader.GetString(3),
                        numeroDocumento = reader.GetString(4),
                        direccion = reader.GetString(5),
                        telefono = reader.GetString(6),
                        email = reader.GetString(7),
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DCliente> Encontrar(int IdCliente)
        {
            List<DCliente> ListaGenerica = new List<DCliente>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListID, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idCliente", IdCliente);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DCliente
                    {
                        idCliente = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        apellidos = reader.GetString(2),
                        tipoDocumento = reader.GetString(3),
                        numeroDocumento = reader.GetString(4),
                        direccion = reader.GetString(5),
                        telefono = reader.GetString(6),
                        email = reader.GetString(7)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DCliente> EncontrarConDocumento(string TipoDocumento, string NumeroDocumento)
        {
            List<DCliente> ListaGenerica = new List<DCliente>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListDocument, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@tipoDocumento", TipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", NumeroDocumento);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DCliente
                    {
                        idCliente = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        apellidos = reader.GetString(2),
                        tipoDocumento = reader.GetString(3),
                        numeroDocumento = reader.GetString(4),
                        direccion = reader.GetString(5),
                        telefono = reader.GetString(6),
                        email = reader.GetString(7),
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }
    }
}
