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
                tipoDocumento,
                numeroDocumento,
                direccion,
                telefono,
                email,
                estado
            ) VALUES (
                @idCliente,
                @nombre,
                @tipoDocumento,
                @numeroDocumento,
                @direccion,
                @telefono,
                @email,
                1
            );
	    ";

        private string queryUpdate = @"
            UPDATE [cliente] SET 
                nombre = @nombre,
                tipoDocumento = @tipoDocumento,
                numeroDocumento = @numeroDocumento,
                direccion = @direccion,
                telefono = @telefono,
                email = @email,
                estado = 1
            WHERE idCliente = @idCliente;
	    ";

        private string queryDelete = @"
            UPDATE [cliente] SET 
                estado = 0
            WHERE idCliente = @idCliente;
	    ";

        private string queryListID = @"
            SELECT * FROM [cliente] 
            WHERE idCliente = @idCliente;
        ";

        private string queryListDocument = @"
            SELECT * FROM [cliente] 
            WHERE tipoDocumento = @tipoDocumento AND numeroDocumento = @numeroDocumento
        ";

        private string queryIDCardRepeated = @"
            SELECT idCliente FROM [cliente] 
            WHERE CONCAT(tipoDocumento , '-', numeroDocumento) = @cedula AND estado <> 0;
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
                comm.Parameters.AddWithValue("@tipoDocumento", Client.tipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", Client.numeroDocumento);
                comm.Parameters.AddWithValue("@direccion", Client.direccion);
                comm.Parameters.AddWithValue("@telefono", Client.telefono);
                comm.Parameters.AddWithValue("@email", Client.email);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro del Cliente";
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
                comm.Parameters.AddWithValue("@tipoDocumento", Client.tipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", Client.numeroDocumento);
                comm.Parameters.AddWithValue("@direccion", Client.direccion);
                comm.Parameters.AddWithValue("@telefono", Client.telefono);
                comm.Parameters.AddWithValue("@email", Client.email);
                comm.Parameters.AddWithValue("@idCliente", Client.idCliente);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Registro del Cliente";
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

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Deshabilitó el Registro del Cliente";
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DCliente> Mostrar(string TipoDocumento, string NumeroDocumento, int Estado)
        {
            List<DCliente> ListaGenerica = new List<DCliente>();

            string queryList = @"
                SELECT * FROM [cliente] 
                WHERE tipoDocumento = @tipoDocumento AND numeroDocumento LIKE @numeroDocumento + '%' " + new LProveedor().BuscarEstado(Estado) + @"
                ORDER BY numeroDocumento;
            ";

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
                            numeroDocumento = reader.GetString(2) + "-" + reader.GetString(3),
                            direccion = reader.GetString(4) == null ? "Sin Dirección" : reader.GetString(4),
                            telefono = reader.GetString(5) == null ? "Sin Teléfono" : reader.GetString(5),
                            email = reader.GetString(6) == null ? "Sin Correo" : reader.GetString(6),
                            estado = reader.GetInt32(7),
                            accesoTrabajadorIngresado = Globals.ACCESO_SISTEMA,
                            nombreTrabajadorIngresado = Globals.TRABAJADOR_SISTEMA
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
                        tipoDocumento = reader.GetString(2),
                        numeroDocumento = reader.GetString(3),
                        direccion = reader.GetString(4),
                        telefono = reader.GetString(5),
                        email = reader.GetString(6),
                        estado = reader.GetInt32(7)
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
                        tipoDocumento = reader.GetString(2),
                        numeroDocumento = reader.GetString(3),
                        direccion = reader.GetString(4),
                        telefono = reader.GetString(5),
                        email = reader.GetString(6),
                        estado = reader.GetInt32(7)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public bool CedulaRepetida(string Cedula)
        {
            bool respuesta = false;

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryIDCardRepeated, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@cedula", Cedula);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read()) respuesta = true;
                else respuesta = false;
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }



        public List<DCliente> CedulaRepetidaAnulada(string Cedula)
        {
            List<DCliente> ListaGenerica = new List<DCliente>();

            string queryIDCardRepeatedNull = @"
                SELECT * FROM [cliente]
                WHERE CONCAT(tipoDocumento , '-', numeroDocumento) = @cedula 
                    AND estado = 0;
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryIDCardRepeatedNull, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@cedula", Cedula);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    ListaGenerica.Add(new DCliente
                    {
                        idCliente = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        tipoDocumento = reader.GetString(2),
                        numeroDocumento = reader.GetString(3),
                        direccion = reader.GetString(4),
                        telefono = reader.GetString(5),
                        email = reader.GetString(6)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }
    }
}
