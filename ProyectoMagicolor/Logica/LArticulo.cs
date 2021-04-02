using System;
using System.Collections.Generic;
using System.Text;
using Datos;

using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LArticulo:DArticulo
    {

        public string Insertar(DArticulo Articulo)
        {
            string respuesta = "";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {
                try
                {
                    conn.Open();

                    LID getID = new LID();

                    int ID = getID.ID("articulo", "idArticulo");

                    string queryAddArticle = @"
                            INSERT INTO articulo (
                                idArticulo,
                                codigo,
                                nombre,
                                descripcion,
                                stockMinimo,
                                stockMaximo,
                                idCategoria
                            ) VALUES (
                                @idArticulo,
                                @codigo,
                                @nombre,
                                @descripcion,
                                @stockMinimo,
                                @stockMaximo,
                                @idCategoria
                            )
	                ";

                    using (SqlCommand comm = new SqlCommand(queryAddArticle, conn))
                    {
                        comm.Parameters.AddWithValue("@idArticulo", ID);
                        comm.Parameters.AddWithValue("@codigo", Articulo.codigo);
                        comm.Parameters.AddWithValue("@nombre", Articulo.nombre);
                        comm.Parameters.AddWithValue("@descripcion", Articulo.descripcion);
                        comm.Parameters.AddWithValue("@stockMinimo", Articulo.stockMinimo);
                        comm.Parameters.AddWithValue("@stockMaximo", Articulo.stockMinimo);
                        comm.Parameters.AddWithValue("@idCategoria", Articulo.idCategoria);

                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del Articulo";
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


        public string Editar(DArticulo Articulo)
        {
            string respuesta = "";

            string queryEditArticle = @"
                        UPDATE articulo SET
                            codigo = @codigo,
                            nombre = @nombre,
                            descripcion = @descripcion,
                            stockMinimo = @stockMinimo,
                            stockMaximo = @stockMaximo,
                            idCategoria = @idCategoria
                        WHERE idArticulo = @idArticulo;
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(queryEditArticle, conn))
                {
                    comm.Parameters.AddWithValue("@codigo", Articulo.codigo);
                    comm.Parameters.AddWithValue("@nombre", Articulo.nombre);
                    comm.Parameters.AddWithValue("@descripcion", Articulo.descripcion);
                    comm.Parameters.AddWithValue("@stockMinimo", Articulo.stockMinimo);
                    comm.Parameters.AddWithValue("@stockMaximo", Articulo.stockMinimo);
                    comm.Parameters.AddWithValue("@idCategoria", Articulo.idCategoria);

                    comm.Parameters.AddWithValue("@idArticulo", Articulo.idArticulo);

                    try
                    {
                        conn.Open();

                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se actualizo el Registro del Articulo";
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


        public string Eliminar(DArticulo Articulo)
        {
            string respuesta = "";

            string queryDeleteArticle = @"
                        DELETE FROM articulo 
                        WHERE idArticulo=@idArticulo
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(queryDeleteArticle, conn))
                {

                    comm.Parameters.AddWithValue("@idArticulo", Articulo.idArticulo);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se elimino el Registro del Articulo";
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


        public List<DArticulo> Mostrar(string Buscar)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * FROM [articulo] WHERE codigo LIKE '" + Buscar + "%' ORDER BY codigo";

                    try
                    {
                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DArticulo
                                {
                                    idArticulo = reader.GetInt32(0),
                                    codigo = reader.GetString(1),
                                    nombre = reader.GetString(2),
                                    descripcion = reader.GetString(3),
                                    stockMinimo = reader.GetInt32(4),
                                    stockMaximo = reader.GetInt32(5),
                                    idCategoria = reader.GetInt32(6)
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


        public List<DArticulo> Encontrar(int Buscar)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * from [articulo] WHERE idArticulo= " + Buscar + " ";

                    try
                    {
                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DArticulo
                                {
                                    idArticulo = reader.GetInt32(0),
                                    codigo = reader.GetString(1),
                                    nombre = reader.GetString(2),
                                    descripcion = reader.GetString(3),
                                    stockMinimo = reader.GetInt32(4),
                                    stockMaximo = reader.GetInt32(5),
                                    idCategoria = reader.GetInt32(6)
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


        public List<DArticulo> EncontrarConCodigo(string Buscar)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * from [articulo] WHERE codigo= '" + Buscar + "' ";

                    try
                    {
                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DArticulo
                                {
                                    idArticulo = reader.GetInt32(0),
                                    codigo = reader.GetString(1),
                                    nombre = reader.GetString(2),
                                    descripcion = reader.GetString(3),
                                    stockMinimo = reader.GetInt32(4),
                                    stockMaximo = reader.GetInt32(5),
                                    idCategoria = reader.GetInt32(6)
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
