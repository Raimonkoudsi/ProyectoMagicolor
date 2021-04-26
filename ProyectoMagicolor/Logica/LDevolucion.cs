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

        private string queryRestock = @"
            UPDATE [detalleIngreso] SET 
                cantidadActual = cantidadActual + @cantidad
            WHERE idArticulo = @idArticulo 
            AND idDetalleIngreso = (SELECT MAX(idDetalleIngreso) FROM detalleIngreso WHERE idArticulo = @idArticulo)
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
			AND (SELECT COUNT(idDetalleVenta)
				FROM detalleVenta
				WHERE idVenta = @idVenta) = 0
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
                    if (!Restock(Detalle[i]).Equals("OK"))
                        throw new Exception("Error en el Actualización del Stock");

                if (!ActualizarDetalleVenta(Detalle[i]).Equals("OK"))
                    throw new Exception("Error en la Actualizacion de los Detalles de la Venta");

                if (!EliminarDetalleVenta(Detalle[i]).Equals("OK"))
                    throw new Exception("Error en al Eliminar el Detalle de la Venta");

                i++;
            }

            return respuesta;
        }

        private string Restock(DDetalle_Devolucion Detalle)
        {
            using SqlCommand comm = new SqlCommand(queryRestock, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idArticulo", Detalle.idArticulo);
            comm.Parameters.AddWithValue("@cantidad", Detalle.cantidad);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó la Actualización del Stock";
        }

        private string ActualizarDetalleVenta(DDetalle_Devolucion Detalle)
        {
            using SqlCommand comm = new SqlCommand(queryUpdateSaleDetail, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@cantidad", Detalle.cantidad);
            comm.Parameters.AddWithValue("@idDetalleVenta", Detalle.idDetalleVenta);
            comm.Parameters.AddWithValue("@idArticulo", Detalle.idArticulo);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Detalle de la Venta";
        }

        private string EliminarDetalleVenta(DDetalle_Devolucion Detalle)
        {
            using SqlCommand comm = new SqlCommand(queryDeleteSaleDetail, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idDetalleVenta", Detalle.idDetalleVenta);
            comm.Parameters.AddWithValue("@idArticulo", Detalle.idArticulo);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "OK";
        }

        private string AnularVenta(int IdVenta)
        {
            using SqlCommand comm = new SqlCommand(queryNullSale, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idVenta", IdVenta);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "OK";
        }
    }
}
