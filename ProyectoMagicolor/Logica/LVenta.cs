using System;
using System.Collections.Generic;
using System.Text;

using Datos;

using System.Data;
using System.Data.SqlClient;

//estado 1=activo, 2=cuentaxcobrar, 3=anulado

namespace Logica
{
    public class LVenta:DVenta
    {


        public string Insertar(DVenta Venta, List<DDetalle_Venta> Detalle, DCuentaCobrar CuentaCobrar)
        {
            string respuesta = "";

            string query = @"
                        INSERT INTO venta(
                            idCliente,
                            idTrabajador,
                            fecha,
                            tipoComprobante,
                            serieComprobante,
                            descuento,
                            metodoPago,
                            estado
                        ) VALUES(
                            @idCliente,
                            @idTrabajador,
                            @fecha,
                            @tipoComprobante,
                            @serieComprobante,
                            @descuento,
                            @metodoPago
                            @estado
                        );
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@idCliente", Venta.idCliente);
                    comm.Parameters.AddWithValue("@idTrabajador", Venta.idTrabajador);
                    comm.Parameters.AddWithValue("@fecha", Venta.fecha);
                    comm.Parameters.AddWithValue("@tipoComprobante", Venta.tipoComprobante);
                    comm.Parameters.AddWithValue("@serieComprobante", Venta.serieComprobante);
                    comm.Parameters.AddWithValue("@descuento", Venta.descuento);
                    comm.Parameters.AddWithValue("@metodoPago", Venta.metodoPago);

                    if (Venta.metodoPago == 2)
                        comm.Parameters.AddWithValue("@estado", 2);
                    else
                        comm.Parameters.AddWithValue("@estado", 1);


                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la venta";

                        this.idVenta = Convert.ToInt32(comm.Parameters["@idVenta"].Value);

                        if (respuesta.Equals("OK") && Venta.metodoPago == 2)
                        {

                            string query2 = @"
                                INSERT INTO cuentaCobrar(
                                    idVenta,
                                    fechaInicio,
                                    fechaLimite,
                                    montoIngresado
                                ) VALUES(
                                    @idVenta,
                                    @fechaInicio,
                                    @fechaLimite,
                                    @montoIngresado
                                );
	                        ";

                            using (SqlCommand comm2 = new SqlCommand(query2, conn))
                            {
                                comm2.Parameters.AddWithValue("@idVenta", this.idVenta);
                                comm2.Parameters.AddWithValue("@fechaInicio", CuentaCobrar.fechaInicio);
                                comm2.Parameters.AddWithValue("@fechaLimite", CuentaCobrar.fechaLimite);
                                comm2.Parameters.AddWithValue("@montoIngresado", CuentaCobrar.montoIngresado);

                                respuesta = comm2.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta por pagar";
                            }
                        }

                        if (respuesta.Equals("OK"))
                        {
                            int i = 0;
                            foreach (DDetalle_Venta det in Detalle)
                            {

                                string query3 = @"
                                    INSERT INTO detalleVenta(
                                        idVenta,
                                        idDetalleIngreso,
                                        cantidad,
                                        precioVenta,
                                        impuesto
                                    ) VALUES(
                                        @idVenta,
                                        @idDetalleIngreso,
                                        @cantidad,
                                        @precioVenta,
                                        @impuesto
                                    );
	                            ";

                                using (SqlCommand comm3 = new SqlCommand(query3, conn))
                                {
                                    comm3.Parameters.AddWithValue("@idVenta", this.idVenta);
                                    comm3.Parameters.AddWithValue("@idDetalleIngreso", Detalle[i].idDetalleIngreso);
                                    comm3.Parameters.AddWithValue("@cantidad", Detalle[i].cantidad);
                                    comm3.Parameters.AddWithValue("@precioVenta", Detalle[i].precioVenta);
                                    comm3.Parameters.AddWithValue("@impuesto", Detalle[i].impuesto);

                                    respuesta = comm3.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del detalle";
                                    i++;
                                }

                                if (!respuesta.Equals("OK"))
                                {
                                    break;
                                }
                            }
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
        }





        public string Eiminar(DVenta Venta)
        {
            string respuesta = "";

            string query = @"
                        DELETE FROM venta WHERE idVenta=@idVenta
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



        //funcionando
        public List<DVenta> Mostrar(string Buscar, string Buscar2)
        {
            List<DVenta> ListaGenerica = new List<DVenta>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT v.idVenta, t.cedula, c.cedula "+"-"+" c.nombre, v.fecha, v.tipoComprobante, v.serieComprobante, v.metodoPago, v.estado, SUM(dv.precioVenta) as precioTotal from [venta] v inner join [cliente] c on v.idCliente=v.idCliente inner join [trabajador] t on v.idTrabajador=t.idTrabajador inner join [detalleVenta] dv on v.idVenta=dv.idVenta where tipoComprobante = " + Buscar + " AND serieComprobante like '" + Buscar2 + "%' order by serieComprobante";


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

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
                                    cedulaTrabajador = reader.GetString(1),
                                    cliente = reader.GetString(2), 
                                    fecha = reader.GetDateTime(3),
                                    tipoComprobante = reader.GetString(4),
                                    serieComprobante = reader.GetString(5),
                                    metodoPago = reader.GetInt32(7),
                                    estado = reader.GetInt32(8),
                                    montoTotal = reader.GetDouble(9)
                                });
                            }
                        }
                    }
                    catch
                    {
                        //error
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
