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
    public class LVenta : LDevolucion
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
            INNER JOIN (
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

        private string queryListActive = @"
            SELECT 
                v.idVenta,
                v.idCliente,
                CONCAT(t.nombre, ' ', t.apellidos) AS nombreTrabajador,
                CONCAT(c.tipoDocumento, '-', c.numeroDocumento) AS cedulaCliente,
                CONCAT(c.nombre, ' ', c.apellidos) AS nombreCliente,
                c.telefono,
                c.email,
                (SUM(dv.precioVenta * dv.cantidad) - v.descuento) AS montoTotal,
                v.descuento,
                v.impuesto,
                v.fecha,
                v.metodoPago,
                v.estado
            FROM [venta] v
                INNER JOIN [cliente] c ON v.idCliente = c.idCliente
                INNER JOIN [trabajador] t ON t.idTrabajador = v.idTrabajador
                INNER JOIN [detalleVenta] dv ON v.idVenta = dv.idVenta
            WHERE v.idVenta = @idVenta
            GROUP BY 
                v.idVenta, 
                t.nombre, 
                t.apellidos, 
                c.tipoDocumento, 
                c.numeroDocumento, 
                c.nombre, 
                c.apellidos, 
                c.telefono, 
                c.email, 
                v.descuento,
                v.impuesto,
                v.fecha,
                v.metodoPago,
                v.estado,
                v.idCliente
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
                INNER JOIN [venta] v ON v.idVenta = dv.idVenta
            WHERE dv.idVenta = @idVenta AND dv.estado <> 0 AND dv.cantidad <> 0;
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
                Venta.idVenta = IDVenta;

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la venta";

                if (!respuesta.Equals("OK"))
                    throw new Exception("Error en el Registro de la Venta");

                if (Venta.metodoPago == 2)
                    if (!InsertarCxC(CuentaCobrar, IDVenta).Equals("OK"))
                        throw new Exception("Error en el Registro de la Cuenta a Cobrar");

                Venta.idVenta = IDVenta;
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


        public string Anular(int IdVenta, List<DDetalle_Venta> Detalle)
        {
            string respuesta = "";
            int i = 0;
            Action action = () =>
            {
                foreach (DDetalle_Venta det in Detalle)
                {
                    if (!RestockVenta(Detalle[i].idArticulo, Detalle[i].cantidad).Equals("OK"))
                        throw new Exception("Error en el Actualización del Stock");

                    if (!ActualizarDetalleVenta(Detalle[i].cantidad, Detalle[i].idDetalleVenta, Detalle[i].idArticulo).Equals("OK"))
                        throw new Exception("Error en la Actualizacion de los Detalles de la Venta");

                    if (!EliminarDetalleVenta(Detalle[i].idDetalleVenta, Detalle[i].idArticulo).Equals("OK"))
                        throw new Exception("Error en al Eliminar el Detalle de la Venta");

                    i++;
                }

                respuesta = AnularVenta(IdVenta);
                if(respuesta.Equals("OK"))
                    LFunction.MessageExecutor("Information", "La Venta ha sido Anulada, regresando al Listado de Ventas");
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
                if (reader.Read())
                {
                    ListaGenerica.Add(new DVenta
                    {
                        idVenta = reader.GetInt32(0),
                        idCliente = reader.GetInt32(1),
                        trabajador = reader.GetString(2),
                        cedulaCliente = reader.GetString(3),
                        cliente = reader.GetString(4),
                        telefonoCliente = reader.GetString(5),
                        emailCliente = reader.GetString(6),
                        montoTotal = (double)reader.GetDecimal(7),
                        descuento = (double)reader.GetDecimal(8),
                        impuesto = reader.GetInt32(9),
                        fechaString = reader.GetDateTime(10).ToShortDateString(),
                        metodoPago = reader.GetInt32(11),
                        metodoPagoString = MetodoPagoToString(reader.GetInt32(11)),
                        estado = reader.GetInt32(12),
                        estadoString = EstadoToString(reader.GetInt32(12)),
                        nombreTrabajadorIngresado = Globals.TRABAJADOR_SISTEMA
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


        public List<DVenta> MostrarVentasGenerales(DateTime? Fecha, string Nombre, int MetodoPago)
        {
            List<DVenta> ListaGenerica = new List<DVenta>();

            string queryList = @"
                SELECT 
                    v.idVenta,
                    CONCAT(c.tipoDocumento, '-', c.numeroDocumento) AS cedulaCliente,
                    CONCAT(c.nombre, ' ', c.apellidos) AS nombreCliente,
                    SUM(dv.precioVenta * dv.cantidad) AS montoTotal,
                    v.descuento,
                    v.impuesto,
                    v.fecha,
                    v.metodoPago,
                    v.estado,
                    CONCAT(t.nombre, ' ', t.apellidos) AS nombreTrabajador
                FROM [venta] v
                    INNER JOIN [cliente] c ON v.idCliente = c.idCliente
                    INNER JOIN [detalleVenta] dv ON v.idVenta = dv.idVenta
                    INNER JOIN [trabajador] t ON v.idTrabajador = t.idTrabajador
                WHERE v.fecha = @fecha 
                    AND CONCAT(c.nombre, ' ', c.apellidos) LIKE @nombre + '%'
                    " + QueryMetodoPago(MetodoPago) + @"
			    GROUP BY v.idVenta, c.tipoDocumento, c.numeroDocumento, c.nombre, c.apellidos, v.fecha, v.metodoPago, v.estado, v.descuento, v.impuesto, t.nombre, t.apellidos;
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@fecha", Fecha == null ? DateTime.Today : Fecha);
                comm.Parameters.AddWithValue("@nombre", Nombre);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DVenta
                    {
                        idVenta = reader.GetInt32(0),
                        cedulaCliente = reader.GetString(1),
                        cliente = reader.GetString(2),
                        montoTotal = (double)reader.GetDecimal(3),
                        descuento = (double)reader.GetDecimal(4),
                        impuesto = reader.GetInt32(5),
                        fechaString = reader.GetDateTime(6).ToShortDateString(),
                        metodoPagoString = MetodoPagoToString(reader.GetInt32(7)),
                        estadoString = EstadoToString(reader.GetInt32(8)),
                        trabajador = reader.GetString(9),
                        nombreTrabajadorIngresado = Globals.TRABAJADOR_SISTEMA
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }

        private string QueryMetodoPago(int MetodoPago)
        {
            if (MetodoPago != 0)
                return " AND v.metodoPago = " + MetodoPago;

            return null;
        }

        public string MetodoPagoToString(int MetodoPago)
        {
            if (MetodoPago == 1)
                return "Contado";
            else
                return "Crédito";
        }

        public string EstadoToString(int Estado)
        {
            if (Estado == 1)
                return "Completada";
            else if (Estado == 2)
                return "Faltante";
            else
                return "Anulada";
        }

    }
}
