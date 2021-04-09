using System;
using System.Collections.Generic;
using System.Text;
using Datos;

using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LDevolucion:DDevolucion
    {

        public string Insertar(DDevolucion Devolucion, List<DDetalle_Devolucion> Detalle)
        {
            int i = 0;

            string respuesta = "";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                try
                {
                    conn.Open();

                    LFunction MethodID = new LFunction();

                    int ID = MethodID.GetID("devolucion", "idDevolucion");

                    #region Añadir Devolucion
                    string queryAddDevolution = @"
                                INSERT INTO devolucion(
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

                    using (SqlCommand comm = new SqlCommand(queryAddDevolution, conn))
                    {
                        comm.Parameters.AddWithValue("@idDevolucion", ID);
                        comm.Parameters.AddWithValue("@idCliente", Devolucion.idCliente);
                        comm.Parameters.AddWithValue("@idTrabajador", Devolucion.idTrabajador);
                        comm.Parameters.AddWithValue("@idVenta", Devolucion.idVenta);
                        comm.Parameters.AddWithValue("@fecha", Devolucion.fecha);

                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la Devolucion";
                    }
                    #endregion

                    if (respuesta.Equals("OK"))
                    {
                        //DETALLES
                        foreach (DDetalle_Devolucion det in Detalle)
                        {
                            #region Restock Articulo si no esta Dañado
                            if (Detalle[i].dañado == 0)
                            {
                                string queryRestock = @"
                                        UPDATE detalleIngreso SET 
                                               cantidadActual = cantidadActual + @cantidad
                                        WHERE idArticulo = @idArticulo 
                                        AND idDetalleIngreso=(SELECT MAX(idDetalleIngreso) FROM detalleIngreso)
                                ";

                                using (SqlCommand comm = new SqlCommand(queryRestock, conn))
                                {
                                    comm.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);
                                    comm.Parameters.AddWithValue("@cantidad", Detalle[i].cantidad);

                                    respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso la actualizacion del stock";
                                }
                            }
                            #endregion

                            if (respuesta.Equals("OK"))
                            {
                                int detailID = MethodID.GetID("detalleDevolucion", "idDetalleDevolucion");

                                #region Añadir Detalle de Devolucion
                                string queryAddDetailDevolution = @"
                                    INSERT INTO detalleDevolucion(
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

                                using (SqlCommand comm = new SqlCommand(queryAddDetailDevolution, conn))
                                {
                                    comm.Parameters.AddWithValue("@idDetalleDevolucion", detailID);
                                    comm.Parameters.AddWithValue("@idDevolucion", ID);
                                    comm.Parameters.AddWithValue("@idDetalleVenta", Detalle[i].idDetalleVenta);
                                    comm.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);
                                    comm.Parameters.AddWithValue("@cantidad", Detalle[i].cantidad);
                                    comm.Parameters.AddWithValue("@precio", Detalle[i].precio);
                                    comm.Parameters.AddWithValue("@dañado", Detalle[i].dañado);

                                    respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingreso el Registro del Detalle de Devolucion";
                                }
                                #endregion
                            }
                            else
                                break;

                            if (respuesta.Equals("OK"))
                            {
                                #region Obtener Cantidad del Articulo Vendido
                                string queryAmountArticle = @"
											SELECT TOP 1 dv.cantidad
                                            FROM detalleVenta dv
                                                inner join detalleIngreso di ON dv.idDetalleIngreso=di.idDetalleIngreso
                                            WHERE dv.idDetalleVenta = @idDetalleVenta
                                                AND di.idArticulo = @idArticulo
											ORDER BY dv.idDetalleIngreso DESC
                                ";

                                using (SqlCommand comm = new SqlCommand(queryAmountArticle, conn))
                                {
                                    comm.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);
                                    comm.Parameters.AddWithValue("@idDetalleVenta", Detalle[i].idDetalleVenta);

                                    using (SqlDataReader reader = comm.ExecuteReader())
                                    {
                                        if (reader.Read() && !reader.IsDBNull(0))
                                            cantidad = reader.GetInt32(0);
                                        else
                                            break;
                                    }
                                }
                                #endregion

                                //si la cantidad a devolver es igual a la de la venta, se elimina el detalle venta
                                if (cantidad == Detalle[i].cantidad)
                                {
                                    #region Eliminar Detalle Venta
                                    string queryDeleteDetail = @"
											DELETE dv FROM detalleVenta dv
                                                inner join detalleIngreso di ON dv.idDetalleIngreso=di.idDetalleIngreso
                                            WHERE dv.idDetalleVenta = @idDetalleVenta
                                                AND di.idArticulo = @idArticulo
                                    ";

                                    using (SqlCommand comm = new SqlCommand(queryDeleteDetail, conn))
                                    {
                                        comm.Parameters.AddWithValue("@idDetalleVenta", Detalle[i].idDetalleVenta);
                                        comm.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);

                                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se eliminó el Detalle de la Venta";
                                    }
                                    #endregion
                                }
                                //si la cantidad a devolver es menor a la de la venta, se actualiza el detalle venta
                                else if (cantidad > Detalle[i].cantidad)
                                {
                                    #region Actualizar Detalle Venta
                                    string queryUpdateDetail = @"
                                            UPDATE detalleVenta SET
                                                cantidad = dv.cantidad - @cantidad
                                            FROM detalleVenta dv
                                                inner join detalleIngreso di ON dv.idDetalleIngreso=di.idDetalleIngreso
                                            WHERE dv.idDetalleVenta = @idDetalleVenta
                                                AND di.idArticulo = @idArticulo
                                    ";

                                    using (SqlCommand comm = new SqlCommand(queryUpdateDetail, conn))
                                    {
                                        comm.Parameters.AddWithValue("@cantidad", Detalle[i].cantidad);
                                        comm.Parameters.AddWithValue("@idDetalleVenta", Detalle[i].idDetalleVenta);
                                        comm.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);

                                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se actualizó el Detalle de la Venta";
                                    }
                                    #endregion
                                }
                            }
                            else
                                break;

                            i++;
                        }

                        #region Contar Detalles de Ventas Restantes
                        string queryCountSales = @"
								SELECT COUNT(idDetalleVenta)
                                FROM detalleVenta
                                WHERE idVenta = @idVenta
                        ;";

                        using (SqlCommand comm = new SqlCommand(queryCountSales, conn))
                        {
                            comm.Parameters.AddWithValue("@idVenta", Devolucion.idVenta);

                            using (SqlDataReader reader = comm.ExecuteReader())
                            {
                                if (reader.Read() && !reader.IsDBNull(0))
                                    cantidad = reader.GetInt32(0);
                            }
                        }
                        #endregion

                        //si queda la venta sin ningun detalle, se anula
                        if (cantidad == 0)
                        {
                            #region Anular Venta
                            string queryCancelSale = @"
                                    UPDATE venta SET estado = 3
						            WHERE idVenta = @idVenta
                            ";

                            using (SqlCommand comm = new SqlCommand(queryCancelSale, conn))
                            {
                                comm.Parameters.AddWithValue("@idVenta", Devolucion.idVenta);

                                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se anuló la Venta";
                            }
                            #endregion
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
                return respuesta;
            }
        }


    }
}
