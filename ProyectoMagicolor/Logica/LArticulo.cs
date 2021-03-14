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
        //Metodos

        public string Insertar(DArticulo Articulo)
        {
            string respuesta = "";

            string query = @"
                        INSERT INTO articulo(
                            codigo,
                            nombre,
                            descripcion,
                            stockMinimo,
                            stockMaximo,
                            idCategoria
                        ) VALUES(
                            @codigo,
                            @nombre,
                            @descripcion,
                            @stockMinimo,
                            @stockMaximo,
                            @idCategoria
                        );
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@codigo", Articulo.codigo);
                    comm.Parameters.AddWithValue("@nombre", Articulo.nombre);
                    comm.Parameters.AddWithValue("@descripcion", Articulo.descripcion);
                    //comm.Parameters.AddWithValue("@imagen", Articulo.imagen);
                    comm.Parameters.AddWithValue("@stockMinimo", Articulo.stockMinimo);
                    comm.Parameters.AddWithValue("@stockMaximo", Articulo.stockMinimo);
                    comm.Parameters.AddWithValue("@idCategoria", Articulo.idCategoria);
                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del Articulo";
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


        public string Editar(DArticulo Articulo)
        {
            string respuesta = "";

            string query = @"
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

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@codigo", Articulo.codigo);
                    comm.Parameters.AddWithValue("@nombre", Articulo.nombre);
                    comm.Parameters.AddWithValue("@descripcion", Articulo.descripcion);
                    //comm.Parameters.AddWithValue("@imagen", Articulo.imagen);
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

            string query = @"
                        DELETE FROM articulo WHERE idArticulo=@idArticulo
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
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



        //funcionando
        public List<DArticulo> Mostrar(string Buscar)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * from [articulo] where codigo like '" + Buscar + "%' order by codigo";


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

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
                                    //imagen = reader.GetSqlBytes(4).Buffer,
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


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

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


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

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
