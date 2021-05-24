using System;
using System.Collections.Generic;
using System.Text;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LDevolucion : DDevolucion
    {
        #region QUERIES
        private string queryInsert = @"
            INSERT INTO [devolucion] (
                idDevolucion,
                idCliente,
                idTrabajador,
                idVenta,
                fecha
            ) VALUES (
                @idDevolucion,
                @idCliente,
                @idTrabajador,
                @idVenta,
                @fecha
            );
        ";

        private string queryInsertDetail = @"
        INSERT INTO [detalleDevolucion] (
                idDetalleDevolucion,
                idDevolucion,
                idDetalleVenta,
                idArticulo,
                cantidad,
                precio,
                dañado
            ) VALUES (
                @idDetalleDevolucion,
                @idDevolucion,
                @idDetalleVenta,
                @idArticulo,
                @cantidad,
                @precio,
                @dañado
            );
        ";

        //VENTAS
        private string queryRestockSale = @"
            UPDATE [detalleIngreso] SET 
                cantidadActual = cantidadActual + @cantidad
            WHERE idArticulo = @idArticulo 
            AND idDetalleIngreso = (SELECT MAX(idDetalleIngreso) FROM detalleIngreso WHERE idArticulo = @idArticulo)
        ";

        private string queryUpdateSaleDetail = @"
            UPDATE detalleVenta SET
                detalleVenta.cantidad = detalleVenta.cantidad - @cantidad
            FROM detalleVenta
                INNER JOIN detalleIngreso di ON detalleVenta.idDetalleIngreso=di.idDetalleIngreso
            WHERE detalleVenta.idDetalleVenta = @idDetalleVenta
                AND di.idArticulo = @idArticulo
        ";

        private string queryDeleteSaleDetail = @"
	        UPDATE detalleVenta SET
                detalleVenta.estado = 0
            FROM detalleVenta dv
                INNER JOIN detalleIngreso di ON dv.idDetalleIngreso=di.idDetalleIngreso
            WHERE dv.idDetalleVenta = @idDetalleVenta
                AND di.idArticulo = @idArticulo
                AND dv.cantidad = 0;
        ";

        private string queryNullSale = @"
            UPDATE [venta] SET estado = 3
			WHERE idVenta = @idVenta
			AND (SELECT SUM(cantidad)
				FROM detalleVenta
				WHERE idVenta = @idVenta) = 0
        ";

        //COMPRAS
        private string queryRestockBuy = @"
            UPDATE [detalleIngreso] SET 
                detalleIngreso.cantidadActual = detalleIngreso.cantidadActual - @cantidad
			FROM [detalleIngreso]
			INNER JOIN (
				SELECT detalleIngreso.idDetalleIngreso 
				FROM detalleIngreso 
				GROUP BY detalleIngreso.idDetalleIngreso, detalleIngreso.cantidadActual
				HAVING @cantidad <= detalleIngreso.cantidadActual
			) x
            ON x.idDetalleIngreso = detalleIngreso.idDetalleIngreso
            WHERE idArticulo = @idArticulo 
            AND detalleIngreso.idDetalleIngreso = (SELECT MAX(idDetalleIngreso) FROM detalleIngreso WHERE idArticulo = @idArticulo)
        ";

        private string queryUpdateBuyDetail = @"
            UPDATE detalleIngreso SET
                detalleIngreso.cantidadInicial = 0
            FROM detalleIngreso
            WHERE detalleIngreso.idDetalleIngreso = @idDetalleIngreso
                AND detalleIngreso.idArticulo = @idArticulo
        ";

        private string queryDeleteBuyDetail = @"
	        UPDATE detalleIngreso SET
                detalleIngreso.estado = 0
            FROM detalleIngreso di
            WHERE di.idDetalleIngreso = @idDetalleIngreso
                AND di.idArticulo = @idArticulo
                AND di.cantidadInicial = 0;
        ";

        private string queryNullBuy = @"
            UPDATE [ingreso] SET estado = 3
			WHERE idIngreso = @idIngreso
			AND (SELECT SUM(cantidadInicial)
				FROM detalleIngreso
				WHERE idIngreso = @idIngreso) = 0
        ";


        private string queryListDetail = @"
            SELECT 
                dd.idDetalleDevolucion, 
                dd.idDevolucion, 
                a.idArticulo, 
                a.codigo, 
                a.nombre, 
                dd.cantidad, 
                dd.precio 
            FROM [detalleDevolucion] dd 
                INNER JOIN [articulo] a ON dd.idArticulo = a.idArticulo 
            WHERE dd.idDevolucion = @idDevolucion AND dd.cantidad <> 0;
        ";
        #endregion


        public string Insertar(DDevolucion Devolucion, List<DDetalle_Devolucion> Detalle)
        {
            string respuesta = "";

            Action action = () =>
            {
                int ID = LFunction.GetID("devolucion", "idDevolucion");

                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idDevolucion", ID);
                comm.Parameters.AddWithValue("@idCliente", Devolucion.idCliente);
                comm.Parameters.AddWithValue("@idTrabajador", Devolucion.idTrabajador);
                comm.Parameters.AddWithValue("@idVenta", Devolucion.idVenta);
                comm.Parameters.AddWithValue("@fecha", Devolucion.fecha);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro de la Devolución";

                if (!respuesta.Equals("OK"))
                    throw new Exception("Error en el Registro de la Devolución");

                if (InsertarDetalle(ID, Detalle).Equals("OK"))
                    respuesta = AnularVenta(Devolucion.idVenta);

            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }

        private string InsertarDetalle(int IdDevolucion, List<DDetalle_Devolucion> Detalle)
        {
            int i = 0;
            string respuesta = "";

            foreach (DDetalle_Devolucion det in Detalle)
            {
                using SqlCommand comm = new SqlCommand(queryInsertDetail, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idDetalleDevolucion", LFunction.GetID("detalleDevolucion", "idDetalleDevolucion"));
                comm.Parameters.AddWithValue("@idDevolucion", IdDevolucion);
                comm.Parameters.AddWithValue("@idDetalleVenta", Detalle[i].idDetalleVenta);
                comm.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);
                comm.Parameters.AddWithValue("@cantidad", Detalle[i].cantidad);
                comm.Parameters.AddWithValue("@precio", Detalle[i].precio);
                comm.Parameters.AddWithValue("@dañado", Detalle[i].dañado);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Detalle de la Devolución";

                if (!respuesta.Equals("OK"))
                    throw new Exception("Error en el Ingreso de los Detalles de la Venta");

                if (Detalle[i].dañado == 0)
                    if (!RestockVenta(Detalle[i].idArticulo, Detalle[i].cantidad).Equals("OK"))
                        throw new Exception("Error en el Actualización del Stock");

                if (!ActualizarDetalleVenta(Detalle[i].cantidad, Detalle[i].idDetalleVenta, Detalle[i].idArticulo).Equals("OK"))
                    throw new Exception("Error en la Actualizacion de los Detalles de la Venta");

                if (!EliminarDetalleVenta(Detalle[i].idDetalleVenta, Detalle[i].idArticulo).Equals("OK"))
                    throw new Exception("Error en al Eliminar el Detalle de la Venta");

                i++;
            }

            return respuesta;
        }

        //VENTAS
        protected string RestockVenta(int IdArticulo, int Cantidad)
        {
            using SqlCommand comm = new SqlCommand(queryRestockSale, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idArticulo", IdArticulo);
            comm.Parameters.AddWithValue("@cantidad", Cantidad);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Stock";
        }

        protected string ActualizarDetalleVenta(int Cantidad, int IdDetalleVenta, int IdArticulo)
        {
            using SqlCommand comm = new SqlCommand(queryUpdateSaleDetail, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@cantidad", Cantidad);
            comm.Parameters.AddWithValue("@idDetalleVenta", IdDetalleVenta);
            comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Detalle de la Venta";
        }

        protected string EliminarDetalleVenta(int IdDetalleVenta, int IdArticulo)
        {
            using SqlCommand comm = new SqlCommand(queryDeleteSaleDetail, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idDetalleVenta", IdDetalleVenta);
            comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "OK";
        }

        protected string AnularVenta(int IdVenta)
        {
            using SqlCommand comm = new SqlCommand(queryNullSale, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idVenta", IdVenta);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "OK";
        }


        //COMPRAS
        protected string RestockIngreso(int IdArticulo, int Cantidad)
        {
            using SqlCommand comm = new SqlCommand(queryRestockBuy, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idArticulo", IdArticulo);
            comm.Parameters.AddWithValue("@cantidad", Cantidad);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Stock";
        }

        protected string ActualizarDetalleIngreso(int IdDetalleIngreso, int IdArticulo)
        {
            using SqlCommand comm = new SqlCommand(queryUpdateBuyDetail, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idDetalleIngreso", IdDetalleIngreso);
            comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Detalle de la Compra";
        }

        protected string EliminarDetalleIngreso(int IdDetalleIngreso, int IdArticulo)
        {
            using SqlCommand comm = new SqlCommand(queryDeleteBuyDetail, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idDetalleVenta", IdDetalleIngreso);
            comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "OK";
        }

        protected string AnularIngreso(int IdIngreso)
        {
            using SqlCommand comm = new SqlCommand(queryNullBuy, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idIngreso", IdIngreso);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "OK";
        }


        public List<DDevolucion> MostrarDevolucionesGenerales(DateTime? Fecha, string Nombre)
        {
            List<DDevolucion> ListaGenerica = new List<DDevolucion>();

            string queryList = @"
                SELECT 
					d.idDevolucion,
                    d.idVenta,
                    CONCAT(c.tipoDocumento, '-', c.numeroDocumento) AS cedulaCliente,
                    CONCAT(c.nombre, ' ', c.apellidos) AS nombreCliente,
                    SUM(dd.cantidad * dd.precio) AS montoDevolucion,
					SUM(dd.cantidad) AS cantidadArticulos,
                    v.fecha
                FROM [devolucion] d
                    INNER JOIN [cliente] c ON d.idCliente = c.idCliente
                    INNER JOIN [detalleDevolucion] dd ON d.idDevolucion = dd.idDevolucion
                    INNER JOIN [venta] v ON v.idVenta = d.idVenta
				WHERE v.fecha = @fecha 
                    AND CONCAT(c.nombre, ' ', c.apellidos) LIKE @nombre + '%'
			    GROUP BY 
					d.idDevolucion,
					d.idVenta,
					c.tipoDocumento,
					c.numeroDocumento,
					c.nombre,
					c.apellidos,
                    v.fecha;
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@fecha", Fecha == null ? DateTime.Today : Fecha);
                comm.Parameters.AddWithValue("@nombre", Nombre);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DDevolucion
                    {
                        idDevolucion = reader.GetInt32(0),
                        idVenta = reader.GetInt32(1),
                        cedulaCliente = reader.GetString(2),
                        nombreCliente = reader.GetString(3),
                        montoDevolucion = (double)reader.GetDecimal(4),
                        cantidad = reader.GetInt32(5),
                        fechaVentaString = reader.GetDateTime(6).ToString("MM/dd/yyyy")
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DDevolucion> MostrarDevolucion(int IdDevolucion)
        {
            List<DDevolucion> ListaGenerica = new List<DDevolucion>();

            string queryList = @"
                SELECT 
					d.idDevolucion,
                    d.idVenta,
                    CONCAT(c.tipoDocumento, '-', c.numeroDocumento) AS cedulaCliente,
                    CONCAT(c.nombre, ' ', c.apellidos) AS nombreCliente,
                    SUM(dd.cantidad * dd.precio) AS montoDevolucion,
					SUM(dd.cantidad) AS cantidadArticulos,
                    CONCAT(t.nombre, ' ', t.apellidos) AS nombreTrabajador,
                    d.fecha,
                    v.fecha,
                    c.telefono,
                    c.email
                FROM [devolucion] d
                    INNER JOIN [cliente] c ON d.idCliente = c.idCliente
                    INNER JOIN [detalleDevolucion] dd ON d.idDevolucion = dd.idDevolucion
                    INNER JOIN [trabajador] t ON d.idTrabajador = t.idTrabajador
                    INNER JOIN [venta] v ON v.idVenta = d.idVenta
				WHERE d.idDevolucion = @idDevolucion
			    GROUP BY 
					d.idDevolucion,
					d.idVenta,
					c.tipoDocumento,
					c.numeroDocumento,
					c.nombre,
					c.apellidos,
                    t.nombre,
                    t.apellidos,
                    v.fecha,
                    d.fecha,
                    c.telefono,
                    c.email;
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idDevolucion", IdDevolucion);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DDevolucion
                    {
                        idDevolucion = reader.GetInt32(0),
                        idVenta = reader.GetInt32(1),
                        cedulaCliente = reader.GetString(2),
                        nombreCliente = reader.GetString(3),
                        montoDevolucion = (double)reader.GetDecimal(4),
                        cantidad = reader.GetInt32(5),
                        trabajador = reader.GetString(6),
                        fechaString = reader.GetDateTime(7).ToShortDateString(),
                        fechaVentaString = reader.GetDateTime(8).ToShortDateString(),
                        telefono = reader.GetString(9),
                        email = reader.GetString(10)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DDetalle_Devolucion> MostrarDetalleDevolucion(int IdDevolucion)
        {
            List<DDetalle_Devolucion> ListaGenerica = new List<DDetalle_Devolucion>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListDetail, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idDevolucion", IdDevolucion);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DDetalle_Devolucion
                    {
                        idDetalleDevolucion = reader.GetInt32(0),
                        idDevolucion = reader.GetInt32(1),
                        idArticulo = reader.GetInt32(2),
                        codigo = reader.GetString(3),
                        nombre = reader.GetString(4),
                        cantidad = reader.GetInt32(5),
                        precio = (double)reader.GetDecimal(6)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }

    }
}
