using System;
using System.Collections.Generic;
using System.Text;

using Datos;

using System.Data;
using System.Data.SqlClient;
using System.Windows;

//estado 1=activo, 2=cuentaxcobrar, 3=anulado

namespace Logica
{
    public class LVenta : DVenta
    {
        #region QUERIES
        private string queryInsert = @"
            INSERT INTO [venta] (
                idVenta,
                idCliente,
                idTrabajador,
                fecha,
                metodoPago,
                descuento,
                impuesto,
                estado
            ) VALUES (
                @idVenta,
                @idCliente,
                @idTrabajador,
                @fecha,
                @metodoPago,
                @descuento,
                @impuesto,
                @estado
            );
        ";

        private string queryInserDetail = @"
            INSERT INTO [detalleVenta] (
                idDetalleVenta,
                idVenta,
                idDetalleIngreso,
                cantidad,
                precioVenta,
                estado
            ) VALUES (
                @idDetalleVenta,
                @idVenta,
                @idDetalleIngreso,
                @cantidad,
                @precioVenta,
                1
            );
        ";

        private string queryUpdateStock = @"
            UPDATE detalleIngreso SET 
                    detalleIngreso.cantidadActual = detalleIngreso.cantidadActual - @cantidad
            FROM [detalleIngreso]
            inner join (
					SELECT detalleIngreso.idDetalleIngreso 
					FROM detalleIngreso 
					GROUP BY detalleIngreso.idDetalleIngreso, detalleIngreso.cantidadActual
					HAVING @cantidad <= detalleIngreso.cantidadActual
			) x
            ON x.idDetalleIngreso = detalleIngreso.idDetalleIngreso
            WHERE detalleIngreso.idDetalleIngreso = @idDetalleIngreso; 
        ";

        private string queryInsertCC = @"
            INSERT INTO [cuentaCobrar] (
                idCuentaCobrar,
                idVenta,
                fechaInicio,
                fechaLimite,
                montoIngresado
            ) VALUES (
                @idCuentaCobrar,
                @idVenta,
                @fechaInicio,
                @fechaLimite,
                @montoIngresado
            );
        ";

        private string queryNull = @"
            UPDATE [venta] SET
                estado = 3
            WHERE idVenta = @idVenta;
        ";

        private string queryListActive = @"
            SELECT * FROM [venta] 
            WHERE idVenta = @idVenta AND estado = 1
        ";

        private string queryListDetail = @"
            SELECT 
                dv.idDetalleVenta, 
                dv.idVenta, 
                a.idArticulo, 
                a.codigo, 
                a.nombre, 
                dv.cantidad, 
                dv.precioVenta 
            FROM [detalleVenta] dv 
                INNER JOIN [detalleIngreso] di ON di.idDetalleIngreso = dv.idDetalleIngreso 
                INNER JOIN [articulo] a ON di.idArticulo = a.idArticulo 
            WHERE dv.idVenta = @idVenta;
        ";

        private string queryList = @"
            SELECT 
                v.idVenta,
                CONCAT(c.tipoDocumento, '-', c.numeroDocumento) AS cedulaCliente,
                CONCAT(c.nombre, ' ', c.apellidos) AS nombreCliente,
                SUM(dv.precioVenta * dv.cantidad) AS montoTotal,
                v.fecha,
                v.metodoPago,
                v.estado
            FROM [venta] v
                INNER JOIN [cliente] c ON v.idCliente = c.idCliente
                INNER JOIN [detalleVenta] dv ON v.idVenta = dv.idVenta
            WHERE v.fecha = @fecha AND CONCAT(c.nombre, ' ', c.apellidos) LIKE @nombre + '%'
			GROUP BY v.idVenta, c.tipoDocumento, c.numeroDocumento, c.nombre, c.apellidos, v.fecha, v.metodoPago, v.estado;
        ";

        #endregion


        public string Insertar(DVenta Venta, List<DDetalle_Venta> Detalle, DCuentaCobrar CuentaCobrar)
        {
            string respuesta = "";

            Action action = () =>
            {
                int IDVenta = LFunction.GetID("venta", "idVenta");

                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idVenta", IDVenta);
                comm.Parameters.AddWithValue("@idCliente", Venta.idCliente);
                comm.Parameters.AddWithValue("@idTrabajador", Venta.idTrabajador);
                comm.Parameters.AddWithValue("@fecha", Venta.fecha);
                comm.Parameters.AddWithValue("@descuento", Venta.descuento);
                comm.Parameters.AddWithValue("@impuesto", Venta.impuesto);
                comm.Parameters.AddWithValue("@metodoPago", Venta.metodoPago);
                comm.Parameters.AddWithValue("@estado", Venta.metodoPago);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la venta";

                if (!respuesta.Equals("OK"))
                    throw new Exception("Error en el Registro de la Venta");

                if (Venta.metodoPago == 2)
                    if (!InsertarCxC(CuentaCobrar, IDVenta).Equals("OK"))
                        throw new Exception("Error en el Registro de la Cuenta a Cobrar");

                respuesta = InsertarDetalle(Detalle, IDVenta);
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }

        private string InsertarDetalle(List<DDetalle_Venta> Detalle, int IdVenta)
        {
            int i = 0;
            string respuesta = "";

            foreach (DDetalle_Venta det in Detalle)
            {
                using SqlCommand comm = new SqlCommand(queryInserDetail, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idDetalleVenta", LFunction.GetID("detalleVenta", "idDetalleVenta"));
                comm.Parameters.AddWithValue("@idVenta", IdVenta);
                comm.Parameters.AddWithValue("@idDetalleIngreso", Detalle[i].idDetalleIngreso);
                comm.Parameters.AddWithValue("@cantidad", Detalle[i].cantidad);
                comm.Parameters.AddWithValue("@precioVenta", Detalle[i].precioVenta);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro del Detalle";
                if (!respuesta.Equals("OK")) break;

                respuesta = ActualizarStock(Detalle[i]);
                if (!respuesta.Equals("OK")) break;

                i++;
            }

            return respuesta;
        }

        private string ActualizarStock(DDetalle_Venta Detalle)
        {
            using SqlCommand comm = new SqlCommand(queryUpdateStock, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@cantidad", Detalle.cantidad);
            comm.Parameters.AddWithValue("@idDetalleIngreso", Detalle.idDetalleIngreso);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó la Cantidad de Articulos en Almacén";
        }

        private string InsertarCxC(DCuentaCobrar CuentaCobrar, int IdVenta)
        {
            using SqlCommand comm = new SqlCommand(queryInsertCC, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idCuentaCobrar", LFunction.GetID("cuentaCobrar", "idCuentaCobrar"));
            comm.Parameters.AddWithValue("@idVenta", IdVenta);
            comm.Parameters.AddWithValue("@fechaInicio", CuentaCobrar.fechaInicio);
            comm.Parameters.AddWithValue("@fechaLimite", CuentaCobrar.fechaLimite);
            comm.Parameters.AddWithValue("@montoIngresado", 0);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro de la Cuenta por Pagar";
        }


        public string Anular(int IdVenta)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryNull, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idVenta", IdVenta);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Anuló la Venta";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Venta Anulada Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DVenta> MostrarVenta(int IdVenta)
        {
            List<DVenta> ListaGenerica = new List<DVenta>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListActive, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idVenta", IdVenta);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DVenta
                    {
                        idVenta = reader.GetInt32(0),
                        idCliente = reader.GetInt32(1),
                        idTrabajador = reader.GetInt32(2),
                        fecha = reader.GetDateTime(3),
                        metodoPago = reader.GetInt32(4),
                        descuento = (double)reader.GetDecimal(5),
                        impuesto = reader.GetInt32(6),
                        estado = reader.GetInt32(7)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DDetalle_Venta> MostrarDetalleVenta(int IdVenta)
        {
            List<DDetalle_Venta> ListaGenerica = new List<DDetalle_Venta>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListDetail, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idVenta", IdVenta);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DDetalle_Venta
                    {
                        idDetalleVenta = reader.GetInt32(0),
                        idVenta = reader.GetInt32(1),
                        idArticulo = reader.GetInt32(2),
                        codigo = reader.GetString(3),
                        nombre = reader.GetString(4),
                        cantidad = reader.GetInt32(5),
                        precioVenta = (double)reader.GetDecimal(6)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }



        public List<DVenta> MostrarGenerales(DateTime? Fecha, string Nombre)
        {
            List<DVenta> ListaGenerica = new List<DVenta>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@fecha", Fecha == null ? DateTime.Today : Fecha);
                comm.Parameters.AddWithValue("@nombre", Nombre);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    string metodoPago = "", estado = "";

                    if (reader.GetInt32(5) == 1)
                        metodoPago = "Contado";
                    else if (reader.GetInt32(5) == 2)
                        metodoPago = "Crédito";

                    if (reader.GetInt32(6) == 1)
                        estado = "Completada";
                    else if (reader.GetInt32(6) == 2)
                        estado = "Faltante";
                    else if (reader.GetInt32(6) == 3)
                        estado = "Anulada";

                    ListaGenerica.Add(new DVenta
                    {
                        idVenta = reader.GetInt32(0),
                        cedulaCliente = reader.GetString(1),
                        cliente = reader.GetString(2),
                        montoTotal = (double)reader.GetDecimal(3),
                        fechaString = reader.GetDateTime(4).ToShortDateString(),
                        metodoPagoString = metodoPago,
                        estadoString = estado
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }
    }
}
