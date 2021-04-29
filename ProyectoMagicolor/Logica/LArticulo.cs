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
        #region QUERIES
        private string queryInsert = @"
            INSERT INTO [articulo] (
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
            );
        ";

        private string queryUpdate = @"
            UPDATE [articulo] SET
                codigo = @codigo,
                nombre = @nombre,
                descripcion = @descripcion,
                stockMinimo = @stockMinimo,
                stockMaximo = @stockMaximo,
                idCategoria = @idCategoria
            WHERE idArticulo = @idArticulo;
        ";

        private string queryDelete = @"
            DELETE * FROM [articulo] 
            WHERE idArticulo = @idArticulo;
        ";

        private string queryList = @"
            SELECT * FROM [articulo] 
            WHERE codigo LIKE @codigo + '%' 
            ORDER BY codigo
        ";

        private string queryListCategory = @"
            SELECT
                a.idArticulo, 
                a.codigo, 
                a.nombre, 
                c.nombre,
                a.descripcion
            FROM [articulo] a 
                INNER JOIN [categoria] c ON a.idCategoria = c.idCategoria 
            WHERE a.nombre LIKE @nombre + '%' 
            ORDER BY a.codigo
        ";

        private string queryListID = @"
            SELECT * FROM [articulo] 
            WHERE idArticulo = @idArticulo
        ";

        private string queryListCode = @"
            SELECT * FROM [articulo] 
            WHERE codigo = @codigo
        ";

        private string queryInventaryDetail = @"
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
                        INNER JOIN[detalleIngreso] di ON dv.idDetalleIngreso = di.idDetalleIngreso

                    WHERE a.idArticulo = di.idArticulo), 0) AS cantidadVendida,
                ISNULL((
                    SELECT
                        SUM(di.cantidadInicial)
                    FROM[detalleIngreso] di
                    WHERE a.idArticulo = di.idArticulo), 0) AS cantidadComprada,
                ISNULL((
                    SELECT
                        SUM(dd.cantidad)
                    FROM[detalleDevolucion] dd
                    WHERE a.idArticulo = dd.idArticulo), 0) AS cantidadDevuelta,
                ISNULL((
                    SELECT
                        COUNT(DISTINCT c.idCliente)
                    FROM[cliente] c
                        INNER JOIN [venta] v ON v.idCliente = c.idCliente
                        INNER JOIN [detalleVenta] dv ON dv.idVenta = v.idVenta
                        INNER JOIN [detalleIngreso] di ON di.idDetalleIngreso = dv.idDetalleIngreso
                    WHERE a.idArticulo = di.idArticulo), 0) AS cantidadCliente,
                ISNULL((
                    SELECT
                        CAST((SUM(dv.precioVenta * dv.cantidad / ((v.impuesto / 100.0) + 1)) - v.descuento) AS NUMERIC(38, 2))
                    FROM[detalleVenta] dv
                        INNER JOIN [detalleIngreso] di ON dv.idDetalleIngreso = di.idDetalleIngreso
                        INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                    WHERE a.idArticulo = di.idArticulo @weekDate ), 0) AS subtotal,
                ISNULL((
                    SELECT
                        (SUM(dv.precioVenta * dv.cantidad) - v.descuento) AS total
                    FROM[detalleVenta] dv
                        INNER JOIN [detalleIngreso] di ON dv.idDetalleIngreso = di.idDetalleIngreso
                        INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                    WHERE a.idArticulo = di.idArticulo @weekDate ), 0) AS total,
                ISNULL((
                    SELECT
                        CAST((SUM(dd.precio * dd.cantidad / ((v.impuesto / 100.0) + 1)) - v.descuento) AS NUMERIC(38, 2))
                    FROM[detalleDevolucion] dd
                        INNER JOIN [detalleVenta] dv ON dv.idDetalleVenta = dd.idDetalleVenta
                        INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                    WHERE a.idArticulo = dd.idArticulo  @weekDate  ), 0) AS subtotalDevolucion,
                ISNULL((
                    SELECT
                        (SUM(dd.precio * dd.cantidad) - v.descuento)
                    FROM[detalleDevolucion] dd
                        INNER JOIN [detalleVenta] dv ON dv.idDetalleVenta = dd.idDetalleVenta
                        INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                    WHERE a.idArticulo = dd.idArticulo  @weekDate ), 0) AS totalDevolucion,
                ISNULL((
                    SELECT TOP 1
                        di.precioCompra
                    FROM[detalleIngreso] di
                        INNER JOIN [detalleVenta] dv ON dv.idDetalleIngreso = di.idDetalleIngreso
                        INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                    WHERE a.idArticulo = di.idArticulo @weekDate
                    ORDER BY di.idDetalleIngreso DESC), 0) AS costoUnidad,
                ISNULL((
                    SELECT
                        (SUM(di.precioCompra * dv.cantidad))
                    FROM[detalleIngreso] di
                        INNER JOIN [detalleVenta] dv ON dv.idDetalleIngreso = di.idDetalleIngreso
                        INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                    WHERE a.idArticulo = di.idArticulo @weekDate ), 0) AS compraVendida,
                ISNULL((
                    SELECT
                        (SUM(dv.precioVenta * dv.cantidad) - SUM(di.precioCompra * dv.cantidad) - v.descuento)
                    FROM[detalleIngreso] di
                        INNER JOIN [detalleVenta] dv ON dv.idDetalleIngreso = di.idDetalleIngreso
                        INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                    WHERE a.idArticulo = di.idArticulo @weekDate ), 0) AS totalNeto
            FROM[articulo] a
                INNER JOIN[categoria] c ON a.idCategoria=c.idCategoria
            WHERE a.idArticulo = @idArticulo;
        ";

        private string queryInventaryArticle = @"
            SELECT 
                a.stockMinimo, 
                ISNULL((
                    SELECT TOP 1 
                        di.cantidadActual 
                    FROM [detalleIngreso] di 
                    WHERE a.idArticulo = di.idArticulo 
                    ORDER BY di.idDetalleIngreso DESC)
                , 0) AS cantidad 
            FROM [articulo] a 
            WHERE a.idArticulo = @idArticulo
        ";
        #endregion


        public string Insertar(DArticulo Articulo)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idArticulo", LFunction.GetID("articulo", "idArticulo"));
                comm.Parameters.AddWithValue("@codigo", Articulo.codigo);
                comm.Parameters.AddWithValue("@nombre", Articulo.nombre);
                comm.Parameters.AddWithValue("@descripcion", Articulo.descripcion == String.Empty ? "No Contiene una Descripción" : Articulo.descripcion);
                comm.Parameters.AddWithValue("@stockMinimo", Articulo.stockMinimo);
                comm.Parameters.AddWithValue("@stockMaximo", Articulo.stockMinimo);
                comm.Parameters.AddWithValue("@idCategoria", Articulo.idCategoria);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro el Articulo";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Articulo Ingresado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public string Editar(DArticulo Articulo)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryUpdate, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@codigo", Articulo.codigo);
                comm.Parameters.AddWithValue("@nombre", Articulo.nombre);
                comm.Parameters.AddWithValue("@descripcion", Articulo.descripcion);
                comm.Parameters.AddWithValue("@stockMinimo", Articulo.stockMinimo);
                comm.Parameters.AddWithValue("@stockMaximo", Articulo.stockMaximo);
                comm.Parameters.AddWithValue("@idCategoria", Articulo.idCategoria);
                comm.Parameters.AddWithValue("@idArticulo", Articulo.idArticulo);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Registro del Articulo";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Articulo Actualizado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public string Eliminar(int IdArticulo)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryDelete, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Eliminó el Registro del Articulo";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Articulo Eliminado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DArticulo> Mostrar(string Codigo)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@codigo", Codigo);

                using SqlDataReader reader = comm.ExecuteReader();
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DArticulo> MostrarConCategoria(string Nombre)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListCategory, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@nombre", Nombre);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DArticulo
                    {
                        idArticulo = reader.GetInt32(0),
                        codigo = reader.GetString(1),
                        nombre = reader.GetString(2),
                        categoria = reader.GetString(3),
                        descripcion = reader.GetString(4)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DArticulo> Encontrar(int IdArticulo)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListID, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

                using SqlDataReader reader = comm.ExecuteReader();
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DArticulo> EncontrarConCodigo(string Codigo)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListCode, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@codigo", Codigo);

                using SqlDataReader reader = comm.ExecuteReader();
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DArticulo> Inventario(int typeDate, DateTime firstDate, DateTime secondDate, int typeStock, int typeOrder)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            Action action = () =>
            {
                string dateQuery = InventarioFecha(typeDate, firstDate, secondDate);
                string orderQuery = InventarioOrden(typeOrder);

                LFunction.MessageExecutor("Information", dateQuery);

                if (dateQuery == null || orderQuery == null)
                    throw new NullReferenceException("Error en los Métodos de Búsqueda, ingreselos nuevamente");

                string queryInventary = @"
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
                            WHERE a.idArticulo = di.idArticulo " + dateQuery + @"), 0) AS vendido,
                        a.stockMinimo
                    FROM [articulo] a 
	                    INNER JOIN [categoria] c ON a.idCategoria=c.idCategoria
                    ORDER BY " + orderQuery + @"
                ";

                using SqlCommand comm = new SqlCommand(queryInventary, Conexion.ConexionSql);

                using SqlDataReader reader = comm.ExecuteReader();
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }
        private string InventarioFecha(int typeDate, DateTime? firstDate, DateTime? secondDate)
        {
            if (typeDate <= 0 && typeDate > 6)
                throw new NullReferenceException("Error en la Búsqueda de Fechas");

            //dia
            if (typeDate == 1) return " AND v.fecha = ('" + DateTime.Today + "')";
            //semana
            if (typeDate == 2) return " AND v.fecha BETWEEN ('" + DateTime.Now.Date.StartOfWeek(DayOfWeek.Monday) + "') AND ('" + DateTime.Today + "')";
            //mes
            if (typeDate == 3) return " AND v.fecha BETWEEN ('" + new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) + "') AND ('" + DateTime.Today + "')";
            //año
            if (typeDate == 4) return " AND v.fecha BETWEEN ('" + new DateTime(DateTime.Now.Year, 1, 1) + "') AND ('" + DateTime.Today + "')";
            //fecha
            if (typeDate == 5) return " AND v.fecha = ('" + firstDate + "')";
            //entre fechas
            if (typeDate == 6) return " AND v.fecha BETWEEN ('" + firstDate + "') AND ('" + secondDate+ "')";

            throw new NullReferenceException("Error en la Búsqueda de Fechas");
        }
        private string InventarioOrden(int typeOrder)
        {
            if (typeOrder <= 0 || typeOrder >= 5)
                throw new NullReferenceException("Error en los Ordenes de Búsqueda");

            //alfabeticamente por Articulo
            if (typeOrder == 1) return " a.nombre ASC";
            //Alfabeticamente por Categoría
            if (typeOrder == 2) return " c.nombre, a.nombre ASC";
            //mayores Ventas
            else if (typeOrder == 3) return " vendido DESC";
            //mayor Stock
            else if (typeOrder == 4) return " cantidad DESC";

            throw new NullReferenceException("Error en los Ordenes de Búsqueda");
        }


        public List<DArticulo> DetalleInventario(int IdArticulo)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryInventaryDetail, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@weekDate", InventarioFecha(2, null, null));
                comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

                using SqlDataReader reader = comm.ExecuteReader();
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
                        total = (double)reader.GetDecimal(12),
                        subtotalDevolucion = (double)reader.GetDecimal(13),
                        totalDevolucion = (double)reader.GetDecimal(14),
                        precioUnidad = (double)reader.GetDecimal(15),
                        compraVendida = (double)reader.GetDecimal(16),
                        totalNeto = (double)reader.GetDecimal(17)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DArticulo> SacarArticulo(int IdArticulo)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryInventaryArticle, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DArticulo
                    {
                        stockMinimo = reader.GetInt32(0),
                        cantidadActual = reader.GetInt32(1)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }
    }
}
