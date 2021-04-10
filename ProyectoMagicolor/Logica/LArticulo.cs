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




        public List<DArticulo> Inventario(int typeDate, DateTime firstDate, DateTime secondDate, int typeStock, int typeOrder)
        {

            string dateQuery = "", orderQuery = "";

            //FECHAS
            if (typeDate <= 0 || typeDate >= 6) return null;
            else
            {
                //diaria
                if (typeDate == 1)
                    dateQuery = " AND v.fecha = ('" + DateTime.Now.Date.ToShortDateString() + "')";
                //semanal
                else if (typeDate == 2)
                    dateQuery = " AND v.fecha BETWEEN ('" + DateTime.Now.Date.StartOfWeek(DayOfWeek.Monday).ToShortDateString() + "') AND ('" + DateTime.Now.Date.ToShortDateString() + "')";
                //mensual
                else if (typeDate == 3)
                    dateQuery = " AND v.fecha BETWEEN ('" + new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString() + "') AND ('" + DateTime.Now.Date.ToShortDateString() + "')";
                //anual
                else if (typeDate == 4)
                    dateQuery = " AND v.fecha BETWEEN ('" + new DateTime(DateTime.Now.Year, 1, 1).ToShortDateString() + "') AND ('" + DateTime.Now.Date.ToShortDateString() + "')";
                //Por Fecha
                else if (typeDate == 5)
                    dateQuery = " AND v.fecha = ('" + firstDate.ToShortDateString() + "')";
                //entre fechas
                else if (typeDate == 6)
                    dateQuery = " AND v.fecha BETWEEN ('" + firstDate.ToShortDateString() + "') AND ('" + secondDate.ToShortDateString() + "')";
            }

            //ORDEN
            if (typeOrder <= 0 || typeOrder >= 5) return null;
            else
            {
                //alfabeticamente por Articulo
                if (typeOrder == 1)
                    orderQuery = " a.nombre ASC";
                //Alfabeticamente por Categoría
                else if (typeOrder == 2)
                    orderQuery = " c.nombre, a.nombre ASC";
                //mayores ventas
                else if (typeOrder == 3)
                    orderQuery = " vendido DESC";
                //mayor stock
                else if (typeOrder == 4)
                    orderQuery = " cantidad DESC";
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
                                        WHERE a.idArticulo = di.idArticulo 
                                        ORDER BY di.idDetalleIngreso DESC), 0) AS cantidad, 
                                    ISNULL((
                                        SELECT 
                                            SUM(dv.cantidad) 
                                        FROM [detalleVenta] dv 
		                                    INNER JOIN [detalleIngreso] di ON dv.idDetalleIngreso = di.idDetalleIngreso 
		                                    INNER JOIN [venta] v ON v.idVenta=dv.idVenta
                                        WHERE a.idArticulo = di.idArticulo " + dateQuery + @" ), 0) as vendido,
                                    a.stockMinimo
                                FROM [articulo] a 
	                                INNER JOIN [categoria] c ON a.idCategoria=c.idCategoria
                                ORDER BY " + orderQuery + @"
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
                            while (reader.Read())
                            {
                                int CantidadActual = reader.GetInt32(4);
                                int StockMinimo = reader.GetInt32(6);
                                if ((typeStock == 2 && CantidadActual < StockMinimo) || (typeStock == 3 && CantidadActual >= StockMinimo))
                                    continue;
                                ListaGenerica.Add(new DArticulo
                                {
                                    idArticulo = reader.GetInt32(0),
                                    codigo = reader.GetString(1),
                                    nombre = reader.GetString(2),
                                    categoria = reader.GetString(3),
                                    cantidadActual = CantidadActual,
                                    cantidadVendida = reader.GetInt32(5),
                                    stockMinimo = StockMinimo
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


        public List<DArticulo> SacarArticulo(int Buscar)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "Select a.stockMinimo, ISNULL((SELECT TOP 1 di.cantidadActual FROM [detalleIngreso] di WHERE a.idArticulo = di.idArticulo ORDER BY di.idDetalleIngreso DESC), 0) AS cantidad from [articulo] a WHERE a.idArticulo= " + Buscar + " ";

                    try
                    {
                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                int CantidadActual = reader.GetInt32(1);
                                int StockMinimo = reader.GetInt32(0);
                                ListaGenerica.Add(new DArticulo
                                {
                                    stockMinimo = StockMinimo,
                                    cantidadActual = CantidadActual
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



        public List<DArticulo> DetalleInventario(int idArticle)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    string weekDate = " AND v.fecha BETWEEN ('" + DateTime.Now.Date.StartOfWeek(DayOfWeek.Monday).ToShortDateString() + "') AND ('" + DateTime.Now.Date.ToShortDateString() + "')";

                    comm.Connection = conn;

                    comm.CommandText = @"
                                        SELECT
                                            a.idArticulo,
                                            a.codigo, 
                                            a.nombre,
									        a.descripcion,
									        a.stockMinimo,
									        a.stockMaximo,
                                            c.nombre,
                                            ISNULL((
										        SELECT 
											        SUM(dv.cantidad) 
										        FROM [detalleVenta] dv 
											        INNER JOIN [detalleIngreso] di ON dv.idDetalleIngreso = di.idDetalleIngreso
										        WHERE a.idArticulo = di.idArticulo), 0) AS cantidadVendida, 
									        ISNULL((
										        SELECT 
											        SUM(di.cantidadInicial) 
										        FROM [detalleIngreso] di 
										        WHERE a.idArticulo = di.idArticulo), 0) AS cantidadComprada, 
									        ISNULL((
										        SELECT 
											        SUM(dd.cantidad) 
										        FROM [detalleDevolucion] dd 
										        WHERE a.idArticulo = dd.idArticulo), 0) AS cantidadDevuelta, 
									        ISNULL((
										        SELECT 
											        COUNT(DISTINCT c.idCliente) 
										        FROM [cliente] c 
											        INNER JOIN [venta] v ON v.idCliente = c.idCliente
											        INNER JOIN [detalleVenta] dv ON dv.idVenta = v.idVenta
											        INNER JOIN [detalleIngreso] di ON di.idDetalleIngreso=dv.idDetalleIngreso
										        WHERE a.idArticulo = di.idArticulo), 0) AS cantidadCliente, 
                                            ISNULL((
										        SELECT 
										          CAST((SUM(dv.precioVenta * dv.cantidad/((v.impuesto/100.0)+1))) AS NUMERIC(38,2))
										        FROM [detalleVenta] dv 
										          INNER JOIN [detalleIngreso] di ON dv.idDetalleIngreso = di.idDetalleIngreso 
										          INNER JOIN [venta] v ON v.idVenta=dv.idVenta
										        WHERE a.idArticulo = di.idArticulo ), 0) AS subtotal,
									        ISNULL((
										        SELECT 
											        (SUM(dv.precioVenta * dv.cantidad)) AS total
										        FROM [detalleVenta] dv 
											        INNER JOIN [detalleIngreso] di ON dv.idDetalleIngreso = di.idDetalleIngreso 
										        WHERE a.idArticulo = di.idArticulo " + weekDate + @" ), 0) AS total
                                        FROM [articulo] a 
	                                        INNER JOIN [categoria] c ON a.idCategoria=c.idCategoria
								        WHERE a.idArticulo= " + idArticle + @"
                    ";

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
                                    categoria = reader.GetString(6),
                                    cantidadVendida = reader.GetInt32(7),
                                    cantidadComprada = reader.GetInt32(8),
                                    cantidadDevuelta = reader.GetInt32(9),
                                    cantidadCliente = reader.GetInt32(10),
                                    subtotal = (double)reader.GetDecimal(11),
                                    impuesto = (double)reader.GetDecimal(12),
                                    total = (double)reader.GetDecimal(13)
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
