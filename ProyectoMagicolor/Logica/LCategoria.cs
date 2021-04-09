using System;
using System.Collections.Generic;
using System.Text;

using Datos;

using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LCategoria:DCategoria
    {

        public string Insertar(DCategoria Categoria)
        {
            string respuesta = "";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {
                try
                {
                    conn.Open();

                    LFunction getID = new LFunction();

                    int ID = getID.GetID("categoria", "idCategoria");

                    string queryAddCategory = @"
                            INSERT INTO categoria (
                                idCategoria,
                                nombre,
                                descripcion
                            ) VALUES (
                                @idCategoria,
                                @nombre,
                                @descripcion
                            );
	                ";

                    using (SqlCommand comm = new SqlCommand(queryAddCategory, conn))
                    {
                        comm.Parameters.AddWithValue("@idCategoria", ID);
                        comm.Parameters.AddWithValue("@nombre", Categoria.nombre);
                        comm.Parameters.AddWithValue("@descripcion", Categoria.descripcion);

                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la Categoria";
                    }
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


        public string Editar(DCategoria Categoria)
        {
            string respuesta = "";

            string queryEditCategory = @"
                        UPDATE categoria SET 
                            nombre = @nombre,
                            descripcion = @descripcion
                        WHERE idCategoria = @idCategoria;
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(queryEditCategory, conn))
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


        public string Eliminar(DCategoria Categoria)
        {
            string respuesta = "";

            string queryDeleteCategory = @"
                        DELETE FROM categoria 
                        WHERE idCategoria=@idCategoria
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(queryDeleteCategory, conn))
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


        public List<DCategoria> Mostrar(string Buscar)
        {
            List<DCategoria> ListaGenerica = new List<DCategoria>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = @"SELECT 
                                            idCategoria, 
                                            nombre, 
                                            descripcion 
                                        from [categoria] 
                                        where nombre like '" + Buscar + "%' order by nombre";

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                if (reader["descripcion"] != DBNull.Value)
                                {
                                    ListaGenerica.Add(new DCategoria
                                    {
                                        idCategoria = reader.GetInt32(0),
                                        nombre = reader.GetString(1),
                                        descripcion = reader.GetString(2)
                                    });
                                }
                                else
                                {
                                    ListaGenerica.Add(new DCategoria
                                    {
                                        idCategoria = reader.GetInt32(0),
                                        nombre = reader.GetString(1),
                                        descripcion = "No contiene una Descripción"
                                    });
                                }
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        MessageBox.Show(e.Message, "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
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


        public List<DCategoria> Encontrar(int Buscar)
        {
            List<DCategoria> ListaGenerica = new List<DCategoria>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * from [categoria] where idCategoria = " + Buscar + "";


                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DCategoria
                                {
                                    idCategoria = reader.GetInt32(0),
                                    nombre = reader.GetString(1),
                                    descripcion = reader.GetString(2)
                                });
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        MessageBox.Show(e.Message, "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
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
