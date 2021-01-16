using System;
using System.Collections.Generic;
using System.Text;
using Datos;

using System.Data;
using System.Data.SqlClient;

namespace Logica
{
    public class LCliente : DCliente
    {


        //Metodos

        public string Insertar(DCliente Cliente)
        {
            string respuesta = "";

            string query = @"
                        INSERT INTO cliente(
                            nombre,
                            apellidos,
                            tipoDocumento,
                            numeroDocumento,
                            direccion,
                            telefono,
                            email
                        ) VALUES(
                            @nombre,
                            @apellidos,
                            @tipoDocumento,
                            @numeroDocumento,
                            @direccion,
                            @telefono,
                            @email
                        );
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@nombre", Cliente.nombre);
                    comm.Parameters.AddWithValue("@apellidos", Cliente.apellidos);
                    comm.Parameters.AddWithValue("@tipoDocumento", Cliente.tipoDocumento);
                    comm.Parameters.AddWithValue("@numeroDocumento", Cliente.numeroDocumento);
                    comm.Parameters.AddWithValue("@direccion", Cliente.direccion);
                    comm.Parameters.AddWithValue("@telefono", Cliente.telefono);
                    comm.Parameters.AddWithValue("@email", Cliente.email);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del Cliente";
                    }
                    catch (SqlException e)
                    {
                        respuesta = e.Message;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return respuesta;
                }
            }
        }


        public string Editar(DCliente Cliente)
        {
            string respuesta = "";

            string query = @"
                        UPDATE cliente SET 
                            nombre = @nombre,
                            apellidos = @apellidos,
                            tipoDocumento = @tipoDocumento,
                            numeroDocumento = @numeroDocumento,
                            direccion = @direccion,
                            telefono = @telefono,
                            email = @email
                            WHERE idCliente = @idCliente;
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@nombre", Cliente.nombre);
                    comm.Parameters.AddWithValue("@apellidos", Cliente.apellidos);
                    comm.Parameters.AddWithValue("@tipoDocumento", Cliente.tipoDocumento);
                    comm.Parameters.AddWithValue("@numeroDocumento", Cliente.numeroDocumento);
                    comm.Parameters.AddWithValue("@direccion", Cliente.direccion);
                    comm.Parameters.AddWithValue("@telefono", Cliente.telefono);
                    comm.Parameters.AddWithValue("@email", Cliente.email);

                    comm.Parameters.AddWithValue("@idCliente", Cliente.idCliente);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se actualizo el Registro del Cliente";
                    }
                    catch (SqlException e)
                    {
                        respuesta = e.Message;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return respuesta;
                }
            }
        }


        public string Eliminar(DCliente Cliente)
        {
            string respuesta = "";

            string query = @"
                        DELETE FROM cliente WHERE idCliente=@idCliente
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {

                    comm.Parameters.AddWithValue("@idCliente", Cliente.idCliente);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se elimino el Registro del Cliente";
                    }
                    catch (SqlException e)
                    {
                        respuesta = e.Message;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return respuesta;
                }
            }
        }



        //funcionando
        public List<DCliente> Mostrar(string Buscar)
        {
            List<DCliente> ListaGenerica = new List<DCliente>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * from [cliente] where numeroDocumento like '" + Buscar + "%' order by numeroDocumento";


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

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
                        }
                    }
                    catch (SqlException e)
                    {
                        //error
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return ListaGenerica;
                }
            }

        }

        public List<DCliente> Encontrar(int Buscar)
        {
            List<DCliente> ListaGenerica = new List<DCliente>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * from [cliente] WHERE idCliente= " + Buscar + "";


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

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
                        }
                    }
                    catch (SqlException e)
                    {
                        //error
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return ListaGenerica;
                }
            }

        }
    }
}
