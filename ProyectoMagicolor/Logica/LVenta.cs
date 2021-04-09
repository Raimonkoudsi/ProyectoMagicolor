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
    public class LVenta:DVenta
    {
        public string Insertar(DVenta Venta, List<DDetalle_Venta> Detalle, DCuentaCobrar CuentaCobrar)
        {
            string respuesta = "";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {
                try
                {
                    conn.Open();

                    LFunction getID = new LFunction();

                    int IDVenta = getID.GetID("venta", "idVenta");

                    #region Añadir Venta
                    string queryAddSale = @"
                            INSERT INTO venta(
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


                    using (SqlCommand comm = new SqlCommand(queryAddSale, conn))
                    {
                        comm.Parameters.AddWithValue("@idVenta", IDVenta);
                        comm.Parameters.AddWithValue("@idCliente", Venta.idCliente);
                        comm.Parameters.AddWithValue("@idTrabajador", Venta.idTrabajador);
                        comm.Parameters.AddWithValue("@fecha", Venta.fecha);
                        comm.Parameters.AddWithValue("@descuento", Venta.descuento);
                        comm.Parameters.AddWithValue("@impuesto", Venta.impuesto);
                        comm.Parameters.AddWithValue("@metodoPago", Venta.metodoPago);

                        if (Venta.metodoPago == 2)
                            comm.Parameters.AddWithValue("@estado", 2);
                        else
                            comm.Parameters.AddWithValue("@estado", 1);


                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la venta";

                    }
                    #endregion

                    if (respuesta.Equals("OK") && Venta.metodoPago == 2)
                    {
                        int IDCC = getID.GetID("cuentaCobrar", "idCuentaCobrar");

                        #region Añadir Cuenta x Cobrar
                        string queryAddCC = @"
                            INSERT INTO cuentaCobrar(
                                idCuentaCobrar,
                                idVenta,
                                fechaInicio,
                                fechaLimite,
                                montoIngresado
                            ) VALUES(
                                @idCuentaCobrar,
                                @idVenta,
                                @fechaInicio,
                                @fechaLimite,
                                @montoIngresado
                            );
	                    ";

                        using (SqlCommand comm = new SqlCommand(queryAddCC, conn))
                        {
                            comm.Parameters.AddWithValue("@idCuentaCobrar", IDCC);
                            comm.Parameters.AddWithValue("@idVenta", IDVenta);
                            comm.Parameters.AddWithValue("@fechaInicio", CuentaCobrar.fechaInicio);
                            comm.Parameters.AddWithValue("@fechaLimite", CuentaCobrar.fechaLimite);
                            comm.Parameters.AddWithValue("@montoIngresado", 0);

                            respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta por pagar";
                        }
                        #endregion
                    }
                    else if(!respuesta.Equals("OK"))
                        return "No se ingreso el Registro de la venta";


                    if (respuesta.Equals("OK"))
                    {
                        int i = 0;

                        foreach (DDetalle_Venta det in Detalle)
                        {

                            #region Actualizar Stock
                            string queryUpdateStock = @"
                                        UPDATE detalleIngreso SET 
                                               detalleIngreso.cantidadActual = detalleIngreso.cantidadActual - @cantidad
                                        FROM [detalleIngreso]
                                        inner join (
							                   SELECT detalleIngreso.idDetalleIngreso 
							                   FROM detalleIngreso 
							                   GROUP BY detalleIngreso.idDetalleIngreso, detalleIngreso.cantidadActual
							                   HAVING @cantidad <= detalleIngreso.cantidadActual
						                ) X
                                        ON x.idDetalleIngreso = detalleIngreso.idDetalleIngreso
                                        WHERE detalleIngreso.idDetalleIngreso = @idDetalleIngreso ";

                            using (SqlCommand comm = new SqlCommand(queryUpdateStock, conn))
                            {
                                comm.Parameters.AddWithValue("@cantidad", Detalle[i].cantidad);
                                comm.Parameters.AddWithValue("@idDetalleIngreso", Detalle[i].idDetalleIngreso);

                                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la Venta";
                            }
                            #endregion


                            if (respuesta.Equals("OK"))
                            {
                                int IDDetVenta = getID.GetID("detalleVenta", "idDetalleVenta");

                                #region Añadir Detalle Venta
                                string queryAddDetSale = @"
                                        INSERT INTO detalleVenta(
                                            idDetalleVenta,
                                            idVenta,
                                            idDetalleIngreso,
                                            cantidad,
                                            precioVenta
                                        ) VALUES(
                                            @idDetalleVenta,
                                            @idVenta,
                                            @idDetalleIngreso,
                                            @cantidad,
                                            @precioVenta
                                        );
	                            ";

                                using (SqlCommand comm = new SqlCommand(queryAddDetSale, conn))
                                {
                                    comm.Parameters.AddWithValue("@idDetalleVenta", IDDetVenta);
                                    comm.Parameters.AddWithValue("@idVenta", IDVenta);
                                    comm.Parameters.AddWithValue("@idDetalleIngreso", Detalle[i].idDetalleIngreso);
                                    comm.Parameters.AddWithValue("@cantidad", Detalle[i].cantidad);
                                    comm.Parameters.AddWithValue("@precioVenta", Detalle[i].precioVenta);

                                    respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del detalle";
                                    i++;
                                }
                                #endregion
                            }
                            else
                                return "No se actualizó el stock";

                            if (!respuesta.Equals("OK"))
                            {
                                break;
                            }
                        }
                    }
                    else
                        return "No se ingreso el Registro de la cuenta por pagar";
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


        public string Eliminar(DVenta Venta)
        {
            string respuesta = "";

            string query = @"
                        UPDATE venta SET estado = 3
						WHERE idVenta = @idVenta
            ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {

                    comm.Parameters.AddWithValue("@idVenta", Venta.idVenta);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se elimino el Registro de la venta";
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


        public List<DVenta> MostrarVenta(int Buscar)
        {
            List<DVenta> ListaGenerica = new List<DVenta>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * FROM [venta] where idVenta = " + Buscar + "";

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

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
                                    impuesto = (double)reader.GetDecimal(6),
                                    estado = reader.GetInt32(7)
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


        public List<DDetalle_Venta> MostrarDetalleVenta(int Buscar)
        {
            List<DDetalle_Venta> ListaGenerica = new List<DDetalle_Venta>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = @"SELECT 
                                            dv.idDetalleVenta, 
                                            dv.idVenta, 
                                            a.idArticulo, 
                                            a.codigo, 
                                            a.nombre, 
                                            dv.cantidad, 
                                            dv.precioVenta 
                                        from [detalleVenta] dv 
                                            inner join [detalleIngreso] di on di.idDetalleIngreso = dv.idDetalleIngreso 
                                            inner join [articulo] a on di.idArticulo = a.idArticulo 
                                        where dv.idVenta = " + Buscar;

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

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
