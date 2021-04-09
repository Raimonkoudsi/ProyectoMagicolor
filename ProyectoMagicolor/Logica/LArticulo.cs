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

                    LFunction getID = new LFunction();

                    int ID = getID.GetID("articulo", "idArticulo");

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

        public List<DArticulo> MostrarConCategoria(string Buscar)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT a.idArticulo, a.codigo, a.nombre, c.nombre as Categoria FROM [articulo] a inner join [categoria] c on a.idCategoria = c.idCategoria Where a.nombre like  '" + Buscar + "%' ORDER BY codigo";

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
                                    categoria = reader.GetString(3)
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




        public List<DArticulo> Inventario(int typeDate, string firstDate, string secondDate, int typeStock, int typeOrder)
        {

            string dateQuery = "", stockQuery = "", orderQuery = "";

            //FECHAS
            if (typeDate <= 0 || typeDate >= 6) return null;
            else
            {
                //diaria
                if (typeDate == 1)
                    dateQuery = " AND v.fecha = " + DateTime.Now.Date;
                //semanal
                else if (typeDate == 2)
                    dateQuery = " AND v.fecha BETWEEN " + DateTime.Now.Date.StartOfWeek(DayOfWeek.Monday) + " AND " + DateTime.Now.Date;
                //mensual
                else if (typeDate == 3)
                    dateQuery = " AND v.fecha BETWEEN " + new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) + " AND " + DateTime.Now.Date;
                //anual
                else if (typeDate == 4)
                    dateQuery = " AND v.fecha BETWEEN " + new DateTime(DateTime.Now.Year, 1, 1) + " AND " + DateTime.Now.Date;
                //entre fechas
                else if (typeDate == 5)
                    dateQuery = " AND v.fecha BETWEEN " + firstDate + " AND " + secondDate;
            }

            //STOCKS
            if (typeStock <= 1 || typeStock >= 4) return null;
            else
            {
                //con stock
                if (typeStock == 2)
                    stockQuery = " AND di.cantidadActual >= a.stockMinimo ";
                //sin stock
                else if (typeStock == 3)
                    stockQuery = " AND di.cantidadActual < a.stockMinimo ";
            }

            //ORDEN
            if (typeOrder <= 0 || typeOrder >= 4) return null;
            else
            {
                //alfabeticamente
                if (typeOrder == 1)
                    orderQuery = " a.nombre ";
                //mayores ventas
                else if (typeOrder == 2)
                    orderQuery = " vendido ";
                //mayor stock
                else if (typeOrder == 3)
                    orderQuery = " cantidad ";
            }


            string inventaryQuery = @"
                                SELECT 
                                    a.idArticulo,
                                    a.codigo, 
                                    a.nombre, 
                                    c.nombre,
                                    ISNULL((
                                    SELECT TOP 1 
                                        di.cantidadActual 
                                    FROM [detalleIngreso] di 
		                                INNER JOIN [detalleVenta] dv ON dv.idDetalleIngreso=di.idDetalleIngreso
		                                INNER JOIN [venta] v ON v.idVenta=dv.idVenta
                                    WHERE a.idArticulo = di.idArticulo 
                                    ORDER BY di.idDetalleIngreso DESC), 0) AS cantidad, 
                                    ISNULL((
                                    SELECT 
                                        SUM(dv.cantidad) 
                                    FROM [detalleVenta] dv 
		                                INNER JOIN [detalleIngreso] di ON dv.idDetalleIngreso = di.idDetalleIngreso 
		                                INNER JOIN [venta] v ON v.idVenta=dv.idVenta
                                    WHERE a.idArticulo = di.idArticulo " + dateQuery + stockQuery + @" ), 0) as vendido,
                                    a.stockMinimo
                                FROM [articulo] a 
	                                INNER JOIN [categoria] c ON a.idCategoria=c.idCategoria
                                ORDER BY " + orderQuery + @"DESC
            ";

            List<DArticulo> ListaGenerica = new List<DArticulo>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = inventaryQuery;

                    try
                    {
                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            //no agrega las filas en busquedas con o sin stock si esta en 0
                            if ((typeStock == 2 || typeStock == 3) && reader.GetInt32(5)!=0)
                            {
                                while (reader.Read())
                                {
                                    ListaGenerica.Add(new DArticulo
                                    {
                                        idArticulo = reader.GetInt32(0),
                                        codigo = reader.GetString(1),
                                        nombre = reader.GetString(2),
                                        categoria = reader.GetString(3),
                                        cantidadActual = reader.GetInt32(4),
                                        cantidadVendida = reader.GetInt32(5),
                                        stockMinimo = reader.GetInt32(6)
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


    }
}
