using System;
using System.Collections.Generic;
using System.Text;
using Datos;
using System.Data;
using System.Data.SqlClient;

namespace Logica
{
    public class LArticulo : DArticulo
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
                idCategoria,
                estado
            ) VALUES (
                @idArticulo,
                @codigo,
                @nombre,
                @descripcion,
                @stockMinimo,
                @stockMaximo,
                @idCategoria,
                1
            );
        ";

        private string queryUpdate = @"
            UPDATE [articulo] SET
                codigo = @codigo,
                nombre = @nombre,
                descripcion = @descripcion,
                stockMinimo = @stockMinimo,
                stockMaximo = @stockMaximo,
                idCategoria = @idCategoria,
                estado = 1
            WHERE idArticulo = @idArticulo;
        ";

        private string queryListCategory = @"
            SELECT
                a.idArticulo, 
                a.codigo, 
                a.nombre, 
                c.nombre,
                a.stockMinimo,
                a.stockMaximo
            FROM [articulo] a 
                INNER JOIN [categoria] c ON a.idCategoria = c.idCategoria 
            WHERE a.nombre LIKE @nombre + '%' 
                AND a.estado <> 0
            ORDER BY a.nombre ASC
        ";



        private string queryListCode = @"
            SELECT
                    a.idArticulo,
                    a.codigo,
                    a.nombre,
                    a.descripcion,
                    a.stockMinimo,
                    a.stockMaximo,
                    a.idCategoria,
                    ISNULL((
		                SELECT TOP 1 
			                di.cantidadActual 
		                FROM [detalleIngreso] di 
		                WHERE a.idArticulo = di.idArticulo AND di.estado <> 0
		                ORDER BY di.idDetalleIngreso DESC), 0) AS cantidad,
                    ISNULL((
                        SELECT TOP 1
                            di.precioCompra
                        FROM [detalleIngreso] di
                        WHERE a.idArticulo = di.idArticulo AND di.estado <> 0
                        ORDER BY idDetalleIngreso DESC
                    ),0) AS precioCompra,
                    ISNULL((
                        SELECT TOP 1
                            di.precioVenta
                        FROM [detalleIngreso] di
                        WHERE a.idArticulo = di.idArticulo AND di.estado <> 0
                        ORDER BY idDetalleIngreso DESC
                    ),0) AS precioVenta
            FROM [articulo] a
            WHERE a.codigo = @codigo
                AND a.estado <> 0
        ";

        private string queryInventaryArticle = @"
            SELECT 
                a.stockMinimo, 
                ISNULL((
                    SELECT TOP 1 
                        di.cantidadActual 
                    FROM [detalleIngreso] di 
                    WHERE a.idArticulo = di.idArticulo AND di.estado <> 0
                    ORDER BY di.idDetalleIngreso DESC)
                , 0) AS cantidad 
            FROM [articulo] a 
            WHERE a.idArticulo = @idArticulo
        ";

        private string queryNotificationStock = @"
            SELECT
	            a.stockMinimo,
	            ISNULL((
		            SELECT TOP 1 
			            di.cantidadActual 
		            FROM [detalleIngreso] di 
		            WHERE a.idArticulo = di.idArticulo AND di.estado <> 0
		            ORDER BY di.idDetalleIngreso DESC), 0) AS cantidad
            FROM [articulo] a
        ";


        //parte de deshabilitar articulo
        private string queryDeshabilitarArticulo = @"
            UPDATE [articulo] SET
                estado = 0
            WHERE idArticulo = @idArticulo;
        ";

        private string queryDeshabilitarDetalleIngresoArticulo = @"
            UPDATE [detalleIngreso] SET
                estadoArticulo = 0
            WHERE idArticulo = @idArticulo;
        ";

        private string queryInsertarDetalleIngresoVacio = @"
            INSERT INTO [detalleIngreso] (
                idDetalleIngreso,
                idIngreso,
                idArticulo,
                precioCompra,
                precioVenta,
                cantidadInicial,
                cantidadActual,
                estado,
                estadoArticulo
            ) VALUES (
                @idDetalleIngreso,
                0,
                @idArticulo,
                (ISNULL((
                    SELECT TOP 1
                        precioCompra
                    FROM [detalleIngreso] WHERE idArticulo = @idArticulo AND estado <> 0
                    ORDER BY idDetalleIngreso DESC
                ),0)),
                (ISNULL((
                    SELECT TOP 1
                        precioVenta
                    FROM [detalleIngreso] WHERE idArticulo = @idArticulo AND estado <> 0
                    ORDER BY idDetalleIngreso DESC
                ),0)),
                0,
                (ISNULL((
                    SELECT TOP 1
                        cantidadActual
                    FROM [detalleIngreso] WHERE idArticulo = @idArticulo AND estado <> 0
                    ORDER BY idDetalleIngreso DESC
                ),0)),
                1,
                1
            );
	    ";
        #endregion


        public string Insertar(DArticulo Articulo)
        {
            string respuesta = "";

            Action action = () =>
            {
                int idArticulo = LFunction.GetID("articulo", "idArticulo");

                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idArticulo", idArticulo);
                comm.Parameters.AddWithValue("@codigo", Articulo.codigo);
                comm.Parameters.AddWithValue("@nombre", Articulo.nombre);
                comm.Parameters.AddWithValue("@descripcion", Articulo.descripcion);
                comm.Parameters.AddWithValue("@stockMinimo", Articulo.stockMinimo);
                comm.Parameters.AddWithValue("@stockMaximo", Articulo.stockMaximo);
                comm.Parameters.AddWithValue("@idCategoria", Articulo.idCategoria);
                Articulo.idArticulo = idArticulo;

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro el Articulo";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Artículo Ingresado Correctamente");
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
                Articulo.idArticulo = idArticulo;

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Registro del Articulo";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Artículo Actualizado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
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
                        stockMinimo = reader.GetInt32(4)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DArticulo> Encontrar(int IdArticulo)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            string queryListID = @"
                SELECT
                    a.idArticulo,
                    a.codigo,
                    a.nombre,
                    a.descripcion,
                    a.stockMinimo,
                    a.stockMaximo,
                    a.idCategoria,
                    ISNULL((
		                SELECT TOP 1 
			                di.cantidadActual 
		                FROM [detalleIngreso] di 
		                WHERE a.idArticulo = di.idArticulo 
		                ORDER BY di.idDetalleIngreso DESC), 0) AS cantidad,
                    ISNULL((
                        SELECT TOP 1
                            precioVenta
                        FROM [detalleIngreso] WHERE idArticulo = @idArticulo AND estado <> 0
                        ORDER BY idDetalleIngreso DESC
                    ),0) AS precioVenta
                FROM [articulo] a
                WHERE a.idArticulo = @idArticulo
            ";

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
                            idCategoria = reader.GetInt32(6),
                            cantidadActual = reader.GetInt32(7),
                            precioVenta = (double)reader.GetDecimal(8)
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
                        idCategoria = reader.GetInt32(6),
                        cantidadActual = reader.GetInt32(7),
                        precioCompra = (double)reader.GetDecimal(8),
                        precioVenta = (double)reader.GetDecimal(9)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DArticulo> Inventario(int typeDate, DateTime firstDate, DateTime secondDate, int typeStock, int typeOrder, bool verNoVendidos)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            Action action = () =>
            {
                string dateQuery = InventarioFecha(typeDate, firstDate, secondDate);
                string dateQueryDevolution = InventarioFechaDevolucion(typeDate, firstDate, secondDate);
                string orderQuery = InventarioOrden(typeOrder);

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
                                AND di.estado <> 0 
                            ORDER BY di.idDetalleIngreso DESC), -1) AS cantidad, 
                        ISNULL((
                            SELECT 
                                SUM(dv.cantidad) 
                            FROM [detalleVenta] dv 
		                        INNER JOIN [detalleIngreso] di ON dv.idDetalleIngreso = di.idDetalleIngreso 
		                        INNER JOIN [venta] v ON v.idVenta=dv.idVenta
                            WHERE a.idArticulo = di.idArticulo AND di.estado <> 0  " + dateQuery + @"), 0) AS vendido,
                        ISNULL((
                            SELECT
                                SUM(dd.cantidad)
                            FROM [detalleDevolucion] dd
		                        INNER JOIN [devolucion] d ON dd.idDevolucion = d.idDevolucion 
                            WHERE a.idArticulo = dd.idArticulo " + dateQueryDevolution + @"), 0) AS cantidadDevuelta,
                        a.stockMinimo,
                        ISNULL((
                            SELECT TOP 1 
                                di.precioVenta 
                            FROM [detalleIngreso] di 
                            WHERE a.idArticulo = di.idArticulo AND di.estado <> 0
                            ORDER BY di.idDetalleIngreso DESC), 0) AS precio
                    FROM [articulo] a 
	                    INNER JOIN [categoria] c ON a.idCategoria=c.idCategoria
                    WHERE a.estado <> 0
                    ORDER BY " + orderQuery + @"
                ";

                using SqlCommand comm = new SqlCommand(queryInventary, Conexion.ConexionSql);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    int CantidadVendida = reader.GetInt32(5);

                    if((CantidadVendida == 0 && verNoVendidos) || CantidadVendida != 0)
                    {

                        int CantidadActual = reader.GetInt32(4);
                        int CantidadDevuelta = reader.GetInt32(6);
                        int StockMinimo = reader.GetInt32(7);
                        if (CantidadActual == -1)
                            continue;
                        if ((typeStock == 2 && CantidadActual < StockMinimo) || (typeStock == 3 && CantidadActual >= StockMinimo))
                            continue;
                        if (((typeOrder == 3 || typeOrder == 4) && CantidadActual == 0) || (typeOrder == 5 && CantidadDevuelta == 0))
                            continue;

                        ListaGenerica.Add(new DArticulo
                        {
                            idArticulo = reader.GetInt32(0),
                            codigo = reader.GetString(1),
                            nombre = reader.GetString(2),
                            categoria = reader.GetString(3),
                            cantidadActual = CantidadActual,
                            cantidadVendida = CantidadVendida,
                            cantidadDevuelta = CantidadDevuelta,
                            stockMinimo = StockMinimo,
                            precioVenta = (double)reader.GetDecimal(8),
                            nombreTrabajadorIngresado = Globals.TRABAJADOR_SISTEMA
                        });

                    }
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }
        private string InventarioFecha(int typeDate, DateTime firstDate, DateTime secondDate)
        {
            if (typeDate <= 0 && typeDate > 6)
                throw new NullReferenceException("Error en la Búsqueda de Fechas");

            //dia
            if (typeDate == 1) return " AND v.fecha = ('" + DateTime.Today.ToString("MM-dd-yyyy") + "')";
            //semana
            if (typeDate == 2) return " AND v.fecha BETWEEN ('" + DateTime.Now.Date.StartOfWeek(DayOfWeek.Monday).ToString("MM/dd/yyyy") + "') AND ('" + DateTime.Today.ToString("MM/dd/yyyy") + "')";
            //mes
            if (typeDate == 3) return " AND v.fecha BETWEEN ('" + new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("MM/dd/yyyy") + "') AND ('" + DateTime.Today.ToString("MM/dd/yyyy") + "')";
            //año
            if (typeDate == 4) return " AND v.fecha BETWEEN ('" + new DateTime(DateTime.Now.Year, 1, 1).ToString("MM/dd/yyyy") + "') AND ('" + DateTime.Today.ToString("MM/dd/yyyy") + "')";
            //fecha
            if (typeDate == 5) return " AND v.fecha = ('" + firstDate.ToString("MM-dd-yyyy") + "')";
            //entre fechas
            if (typeDate == 6) return " AND v.fecha BETWEEN ('" + firstDate.ToString("MM-dd-yyyy") + "') AND ('" + secondDate.ToString("MM-dd-yyyy") + "')";

            throw new NullReferenceException("Error en la Búsqueda de Fechas");
        }

        private string InventarioFechaDevolucion(int typeDate, DateTime firstDate, DateTime secondDate)
        {
            if (typeDate <= 0 && typeDate > 6)
                throw new NullReferenceException("Error en la Búsqueda de Fechas");

            //dia
            if (typeDate == 1) return " AND d.fecha = ('" + DateTime.Today.ToString("MM-dd-yyyy") + "')";
            //semana
            if (typeDate == 2) return " AND d.fecha BETWEEN ('" + DateTime.Now.Date.StartOfWeek(DayOfWeek.Monday).ToString("MM/dd/yyyy") + "') AND ('" + DateTime.Today.ToString("MM/dd/yyyy") + "')";
            //mes
            if (typeDate == 3) return " AND d.fecha BETWEEN ('" + new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("MM/dd/yyyy") + "') AND ('" + DateTime.Today.ToString("MM/dd/yyyy") + "')";
            //año
            if (typeDate == 4) return " AND d.fecha BETWEEN ('" + new DateTime(DateTime.Now.Year, 1, 1).ToString("MM/dd/yyyy") + "') AND ('" + DateTime.Today.ToString("MM/dd/yyyy") + "')";
            //fecha
            if (typeDate == 5) return " AND d.fecha = ('" + firstDate.ToString("MM-dd-yyyy") + "')";
            //entre fechas
            if (typeDate == 6) return " AND d.fecha BETWEEN ('" + firstDate.ToString("MM-dd-yyyy") + "') AND ('" + secondDate.ToString("MM-dd-yyyy") + "')";

            throw new NullReferenceException("Error en la Búsqueda de Fechas");
        }
        private string InventarioOrden(int typeOrder)
        {
            if (typeOrder <= 0 || typeOrder >= 7)
                throw new NullReferenceException("Error en los Ordenes de Búsqueda");

            //alfabeticamente por Articulo
            if (typeOrder == 1) return " a.nombre ASC";
            //Alfabeticamente por Categoría
            if (typeOrder == 2) return " c.nombre, a.nombre ASC";
            //mayores Ventas
            else if (typeOrder == 3) return " vendido DESC";
            //menores Ventas
            else if (typeOrder == 4) return " vendido ASC";
            //menores Ventas
            else if (typeOrder == 5) return " cantidadDevuelta DESC";
            //mayor Stock
            else if (typeOrder == 6) return " cantidad DESC";

            throw new NullReferenceException("Error en los Ordenes de Búsqueda");
        }


        public List<DArticulo> DetalleInventario(int IdArticulo)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            string weekDate = InventarioFecha(3, DateTime.Now, DateTime.Now);

            string queryInventaryDetail = @"
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

                        WHERE a.idArticulo = di.idArticulo AND dv.estado <> 0), 0) AS cantidadVendida,
                    ISNULL((
                        SELECT
                            SUM(di.cantidadInicial)
                        FROM [detalleIngreso] di
                        WHERE a.idArticulo = di.idArticulo AND di.estado <> 0), 0) AS cantidadComprada,
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
                        WHERE a.idArticulo = di.idArticulo AND dv.estado <> 0), 0) AS cantidadCliente,
                    ISNULL((
                        SELECT
                            CAST(SUM(dv.precioVenta * dv.cantidad / ((v.impuesto / 100.0) + 1)) AS NUMERIC(38, 2))
                        FROM[detalleVenta] dv
                            INNER JOIN [detalleIngreso] di ON dv.idDetalleIngreso = di.idDetalleIngreso
                            INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                        WHERE a.idArticulo = di.idArticulo
							AND v.estado <> 0 AND dv.estado <> 0 AND di.estado <> 0 " + weekDate + @"
						), 0) AS subtotal,
                    ISNULL((
                        SELECT
                            (SUM(dv.precioVenta * dv.cantidad)) AS total
                        FROM[detalleVenta] dv
                            INNER JOIN [detalleIngreso] di ON dv.idDetalleIngreso = di.idDetalleIngreso
                            INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                        WHERE a.idArticulo = di.idArticulo 
							AND v.estado <> 0 AND dv.estado <> 0 AND di.estado <> 0" + weekDate + @"
						), 0) AS total,
                    ISNULL((
                        SELECT
                            CAST(SUM(dd.precio * dd.cantidad / ((v.impuesto / 100.0) + 1)) AS NUMERIC(38, 2))
                        FROM[detalleDevolucion] dd
                            INNER JOIN [detalleVenta] dv ON dv.idDetalleVenta = dd.idDetalleVenta
                            INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                        WHERE a.idArticulo = dd.idArticulo  
							AND v.estado <> 0 AND dv.estado <> 0 " + weekDate + @"
						), 0) AS subtotalDevolucion,
                    ISNULL((
                        SELECT
                            (SUM(dd.precio * dd.cantidad))
                        FROM[detalleDevolucion] dd
                            INNER JOIN [detalleVenta] dv ON dv.idDetalleVenta = dd.idDetalleVenta
                            INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                        WHERE a.idArticulo = dd.idArticulo AND v.estado <> 0 AND dv.estado <> 0 " + weekDate + @"
						), 0) AS totalDevolucion,
                    ISNULL((
                        SELECT
                            CAST((SUM(di.precioCompra * dv.cantidad / ((i.impuesto / 100.0) + 1))) AS NUMERIC(38, 2))
                        FROM[detalleVenta] dv
                            INNER JOIN [detalleIngreso] di ON dv.idDetalleIngreso = di.idDetalleIngreso
                            INNER JOIN [ingreso] i ON i.idIngreso = di.idIngreso
                            INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                        WHERE a.idArticulo = di.idArticulo " + weekDate + @"
						), 0) AS subtotalCompraVendida,
                    ISNULL((
                        SELECT
                            (SUM(di.precioCompra * dv.cantidad))
                        FROM[detalleIngreso] di
                            INNER JOIN [detalleVenta] dv ON dv.idDetalleIngreso = di.idDetalleIngreso
                            INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                            INNER JOIN [ingreso] i ON i.idIngreso = di.idIngreso
                        WHERE a.idArticulo = di.idArticulo " + weekDate + @"
						), 0) AS totalCompraVendida,
					ISNULL((
                        SELECT TOP 1
                            di.precioCompra
                        FROM[detalleIngreso] di
                            INNER JOIN [detalleVenta] dv ON dv.idDetalleIngreso = di.idDetalleIngreso
                            INNER JOIN [venta] v ON v.idVenta = dv.idVenta
                        WHERE a.idArticulo = di.idArticulo
                        ORDER BY di.idDetalleIngreso DESC), 0) AS costoUnidad,
                    ISNULL((
                        SELECT TOP 1 
                            di.cantidadActual 
                        FROM [detalleIngreso] di 
                        WHERE a.idArticulo = di.idArticulo 
                        ORDER BY di.idDetalleIngreso DESC), 0) AS cantidadActual
                FROM [articulo] a
                    INNER JOIN[categoria] c ON a.idCategoria=c.idCategoria
                WHERE a.idArticulo = @idArticulo;
            ";

            Action action = () =>
                {
                    using SqlCommand comm = new SqlCommand(queryInventaryDetail, Conexion.ConexionSql);
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
                            subtotalCompra = (double)reader.GetDecimal(15),
                            totalCompra = (double)reader.GetDecimal(16),
                            precioUnidad = (double)reader.GetDecimal(17),
                            cantidadActual = reader.GetInt32(18)
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

        public int NotificacionStock()
        {
            int cantidad = 0;

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryNotificationStock, Conexion.ConexionSql);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetInt32(0) >= reader.GetInt32(1))
                        cantidad++;
                }
            };
            LFunction.SafeExecutor(action);

            return cantidad;
        }

        public bool CodigoRepetido(string Codigo, int IdArticulo)
        {
            bool respuesta = false;

            string queryCodeRepeated = @"
                SELECT idArticulo FROM [articulo] 
                WHERE codigo = @codigo
                    AND (idArticulo <> @idArticulo OR (idArticulo = @idArticulo AND estado = 0));
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryCodeRepeated, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@codigo", Codigo);
                comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read()) respuesta = true;
                else respuesta = false;
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DArticulo> CodigoRepetidoAnulado(string Codigo)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();


            string queryCodeRepeatedNull = @"
                    SELECT 
                        a.idArticulo,
                        a.codigo, 
                        a.nombre,
                        c.nombre,
						a.stockMinimo,
						a.stockMaximo,
                        ISNULL((
                            SELECT TOP 1 
                                di.precioVenta 
                            FROM [detalleIngreso] di 
                            WHERE a.idArticulo = di.idArticulo 
                            ORDER BY di.idDetalleIngreso DESC), 0) AS precioVenta,
                        ISNULL((
                            SELECT TOP 1 
                                di.precioCompra
                            FROM [detalleIngreso] di 
                            WHERE a.idArticulo = di.idArticulo 
                            ORDER BY di.idDetalleIngreso DESC), 0) AS precioCompra,
                        a.descripcion,
                        a.estado
					FROM [articulo] a
                        INNER JOIN [categoria] c ON c.idCategoria=a.idCategoria
                    WHERE a.codigo = @codigo 
                        AND a.idArticulo <> 0 
                        AND a.estado = 0;
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryCodeRepeatedNull, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@codigo", Codigo);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    ListaGenerica.Add(new DArticulo
                    {
                        idArticulo = reader.GetInt32(0),
                        codigo = reader.GetString(1),
                        nombre = reader.GetString(2),
                        categoria = reader.GetString(3),
                        stockMinimo = reader.GetInt32(4),
                        stockMaximo = reader.GetInt32(5),
                        precioVenta = (double)reader.GetDecimal(6),
                        precioCompra = (double)reader.GetDecimal(7),
                        descripcion = reader.GetString(8),
                        estado = reader.GetInt32(9)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DArticulo> ActualizarArticulo(string Busqueda, int TipoBusqueda, int Estado)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            Action action = () =>
            {
                string searchQuery = "";

                if (TipoBusqueda == 0 && Busqueda != "-")
                    searchQuery = " WHERE a.codigo LIKE '" + Busqueda + "' + '%' ";
                else if (TipoBusqueda == 1 && Busqueda != "-")
                    searchQuery = " WHERE a.nombre LIKE '" + Busqueda + "' + '%' ";
                    

                string queryListArticlee = @"
                    SELECT 
                        a.idArticulo,
                        a.codigo, 
                        a.nombre,
                        c.nombre,
                        ISNULL((
                            SELECT TOP 1 
                                di.precioVenta 
                            FROM [detalleIngreso] di 
                            WHERE a.idArticulo = di.idArticulo 
                            ORDER BY di.idDetalleIngreso DESC), 0) AS precioVenta,
                        ISNULL((
                            SELECT TOP 1 
                                di.cantidadActual 
                            FROM [detalleIngreso] di 
                            WHERE a.idArticulo = di.idArticulo 
                            ORDER BY di.idDetalleIngreso DESC), 0) AS cantidadActual,
                        ISNULL((
                            SELECT TOP 1 
                                i.fecha 
                            FROM [ingreso] i
								INNER JOIN [detalleIngreso] di ON i.idIngreso=di.idIngreso
                            WHERE a.idArticulo = di.idArticulo 
                            ORDER BY di.idDetalleIngreso DESC), null) AS ultimaActualizacion,
                        a.estado
					FROM [articulo] a
                        INNER JOIN [categoria] c ON c.idCategoria = a.idCategoria
					" + searchQuery + BuscarEstado(Estado) + @"
					ORDER BY ultimaActualizacion DESC
                ";

                using SqlCommand comm = new SqlCommand(queryListArticlee, Conexion.ConexionSql);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    int cantidadActual = reader.GetInt32(5);
                    int estado = reader.GetInt32(7);
                    if (cantidadActual != 0 || cantidadActual == 0 || (cantidadActual == 0 && estado == 0))
                    {
                        ListaGenerica.Add(new DArticulo
                        {
                            idArticulo = reader.GetInt32(0),
                            codigo = reader.GetString(1),
                            nombre = reader.GetString(2),
                            categoria = reader.GetString(3),
                            precioVenta = (double)reader.GetDecimal(4),
                            precioVentaString = ((double)reader.GetDecimal(4)).ToString() + " Bs S",
                            cantidadActual = cantidadActual,
                            ultimaActualizacion = reader.GetDateTime(6) == null ? "Sin Actualización" : reader.GetDateTime(6).ToString("dd-MM-yyyy"),
                            estado = estado,
                            accesoTrabajadorIngresado = Globals.ACCESO_SISTEMA,
                            nombreTrabajadorIngresado = Globals.TRABAJADOR_SISTEMA
                        });
                    }
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DArticulo> EncontrarActualizarArticulo(int IdArticulo)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            string queryListArticlee = @"
                    SELECT 
                        a.idArticulo,
                        a.codigo, 
                        a.nombre,
                        c.nombre,
						a.stockMinimo,
						a.stockMaximo,
                        ISNULL((
                            SELECT TOP 1 
                                di.precioVenta 
                            FROM [detalleIngreso] di 
                            WHERE a.idArticulo = di.idArticulo 
                            ORDER BY di.idDetalleIngreso DESC), 0) AS precioVenta,
                        ISNULL((
                            SELECT TOP 1 
                                di.precioCompra
                            FROM [detalleIngreso] di 
                            WHERE a.idArticulo = di.idArticulo 
                            ORDER BY di.idDetalleIngreso DESC), 0) AS precioCompra,
                        a.descripcion,
                        a.estado
					FROM [articulo] a
                        INNER JOIN [categoria] c ON c.idCategoria=a.idCategoria
					WHERE a.idArticulo = @idArticulo
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListArticlee, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DArticulo
                    {
                        idArticulo = reader.GetInt32(0),
                        codigo = reader.GetString(1),
                        nombre = reader.GetString(2),
                        categoria = reader.GetString(3),
                        stockMinimo = reader.GetInt32(4),
                        stockMaximo = reader.GetInt32(5),
                        precioVenta = (double)reader.GetDecimal(6),
                        precioCompra = (double)reader.GetDecimal(7),
                        descripcion = reader.GetString(8),
                        estado = reader.GetInt32(9)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }



        public string DeshabilitarArticulo(int IdArticulo, string tipoAnulacion)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryDeshabilitarArticulo, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Registro del Articulo";

                if (respuesta.Equals("OK") && tipoAnulacion == "Completo")
                    respuesta = DeshabilitarArticuloEnIngreso(IdArticulo);
                else if (respuesta.Equals("OK") && tipoAnulacion == "Parcial")
                    respuesta = InsertarDetalleIngresoVacio(IdArticulo);
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        private string DeshabilitarArticuloEnIngreso(int IdArticulo)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryDeshabilitarDetalleIngresoArticulo, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "OK";

            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        private string InsertarDetalleIngresoVacio(int IdArticulo)
        {
            string respuesta = "";

            //proveedor vacio
            if (new LProveedor().Encontrar(0).Count == 0)
            {
                DProveedor UForm = new DProveedor(0,
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0.ToString());

                new LProveedor().Insertar(UForm, true);
            }
            //trabajador vacio
            if (new LTrabajador().Encontrar(0).Count == 0)
            {
                DTrabajador UForm = new DTrabajador(0,
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                DateTime.Now,
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0,
                                0.ToString(),
                                0.ToString());

                new LTrabajador().Insertar(UForm, null, true);
            }
            //ingreso vacio
            if (new LIngreso().MostrarIngresoVacio() == false)
            {
                DIngreso UForm = new DIngreso(0,
                                0,
                                0,
                                DateTime.Now,
                                0.ToString(),
                                LFunction.MostrarIVA(),
                                0,
                                0);

                new LIngreso().Insertar(UForm, null, null, true);
            }

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryInsertarDetalleIngresoVacio, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idDetalleIngreso", LFunction.GetID("detalleIngreso", "idDetalleIngreso"));
                comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el articulo";

                if (!respuesta.Equals("OK"))
                    throw new Exception("Error en actualización del artículo");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        private string BuscarEstado(int Estado)
        {
            if (Estado == 1)
                return "AND a.estado = 1";
            if (Estado == 2)
                return "AND a.estado = 0";
            if (Estado == 3)
                return "";

            return "AND a.estado = 2";
        }



        public Tuple<int, int> ContadorStockVistaPrincipal()
        {
            int CantidadConStock = 0;
            int CantidadSinStock = 0;

            string queryList = @"
					SELECT 
                        ISNULL((
                            SELECT TOP 1 
                                di.cantidadActual 
                            FROM [detalleIngreso] di 
                            WHERE a.idArticulo = di.idArticulo 
                                AND di.estado <> 0 
                            ORDER BY di.idDetalleIngreso DESC), -1) AS cantidad, 
                        a.stockMinimo
                    FROM [articulo] a 
					WHERE a.estado <> 0
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    if(reader.GetInt32(0) != -1)
                    {
                        if (reader.GetInt32(0) < reader.GetInt32(1))
                            CantidadSinStock++;
                        else
                            CantidadConStock++;
                    }
                }
            };
            LFunction.SafeExecutor(action);

            return Tuple.Create(CantidadConStock, CantidadSinStock);
        }

        public Tuple<int, int> ContadorCxCVistaPrincipal()
        {
            int CantidadConTiempo = 0;
            int CantidadSinTiempo = 0;

            string queryListCCIncomplete = @"
					SELECT 
						cc.fechaLimite
                    FROM [cuentaCobrar] cc
						INNER JOIN [venta] v ON v.idVenta=cc.idVenta
					WHERE v.estado = 2
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListCCIncomplete, Conexion.ConexionSql);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetDateTime(0) < DateTime.Today)
                        CantidadSinTiempo++;
                    else
                        CantidadConTiempo++;
                }
            };
            LFunction.SafeExecutor(action);

            return Tuple.Create(CantidadConTiempo, CantidadSinTiempo);
        }

        public Tuple<int, int> ContadorCxPVistaPrincipal()
        {
            int CantidadConTiempo = 0;
            int CantidadSinTiempo = 0;

            string queryListCPIncomplete = @"
					SELECT 
						cp.fechaLimite
                    FROM [cuentaPagar] cp
						INNER JOIN [ingreso] i ON i.idIngreso=cp.idIngreso
					WHERE i.estado = 2
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListCPIncomplete, Conexion.ConexionSql);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetDateTime(0) < DateTime.Today)
                        CantidadSinTiempo++;
                    else
                        CantidadConTiempo++;
                }
            };
            LFunction.SafeExecutor(action);

            return Tuple.Create(CantidadConTiempo, CantidadSinTiempo);
        }
    }
}