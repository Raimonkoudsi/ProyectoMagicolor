using System;
using System.Collections.Generic;
using System.Text;

using Datos;

using System.Data;
using System.Data.SqlClient;

namespace Logica
{
    public class LCategoria:DCategoria
    {

        //Metodos

        public string Insertar(DCategoria Categoria)
        {
            string respuesta = "";

            string query = @"
                        INSERT INTO categoria(
                            nombre,
                            descripcion
                        ) VALUES(
                            @nombre,
                            @descripcion
                        );
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@nombre", Categoria.nombre);
                    comm.Parameters.AddWithValue("@descripcion", Categoria.descripcion);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la Categoria";
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


        public string Editar(DCategoria Categoria)
        {
            string respuesta = "";

            string query = @"
                        UPDATE cliente SET (
                            nombre,
                            descripcion
                        ) VALUES(
                            @nombre,
                            @descripcion
                        ) WHERE idCategoria = @idCategoria;
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@nombre", Categoria.nombre);
                    comm.Parameters.AddWithValue("@descripcion", Categoria.descripcion);

                    comm.Parameters.AddWithValue("@idCategoria", Categoria.idCategoria);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se actualizo el Registro de la Categoria";
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


        public string Eiminar(DCategoria Categoria)
        {
            string respuesta = "";

            string query = @"
                        DELETE FROM cliente WHERE idCategoria=@idCategoria
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {

                    comm.Parameters.AddWithValue("@idCategoria", Categoria.idCategoria);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se elimino el Registro de la Categoria";
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
        public List<DCategoria> Mostrar(string Buscar)
        {
            List<DCategoria> ListaGenerica = new List<DCategoria>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT nombre, descripcion from [categoria] where nombre like '" + Buscar + "%' order by nombre";


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DCategoria
                                {
                                    nombre = reader.GetString(0),
                                    descripcion = reader.GetString(1),
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
