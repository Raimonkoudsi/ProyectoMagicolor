using System;
using System.Collections.Generic;
using System.Text;
using Datos;

using System.Data;
using System.Data.SqlClient;

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
                            imagen,
                            stockMinimo,
                            stockMaximo,
                            idCategoria
                        ) VALUES(
                            @codigo,
                            @nombre,
                            @descripcion,
                            @imagen,
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
                    comm.Parameters.AddWithValue("@imagen", Articulo.imagen);
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
                        UPDATE articulo SET (
                            codigo,
                            nombre,
                            descripcion,
                            imagen,
                            stockMinimo,
                            stockMaximo,
                            idCategoria
                        ) VALUES(
                            @codigo,
                            @nombre,
                            @descripcion,
                            @imagen,
                            @stockMinimo,
                            @stockMaximo,
                            @idCategoria
                        ) WHERE idArticulo = @idArticulo;
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@codigo", Articulo.codigo);
                    comm.Parameters.AddWithValue("@nombre", Articulo.nombre);
                    comm.Parameters.AddWithValue("@descripcion", Articulo.descripcion);
                    comm.Parameters.AddWithValue("@imagen", Articulo.imagen);
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


        public string Eiminar(DArticulo Articulo)
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
                                    codigo = reader.GetString(1),
                                    nombre = reader.GetString(2),
                                    descripcion = reader.GetString(3),
                                    stockMinimo = reader.GetInt32(5),
                                    stockMaximo = reader.GetInt32(6),
                                    idCategoria = reader.GetInt32(7)
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
