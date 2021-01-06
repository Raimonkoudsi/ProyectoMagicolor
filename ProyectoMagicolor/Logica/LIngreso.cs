﻿using System;
using System.Collections.Generic;
using System.Text;
using Datos;

using System.Data;
using System.Data.SqlClient;


namespace Logica
{
    public class LIngreso:DIngreso
    {
        //Metodos

        public string Insertar(DIngreso Ingreso, List<DDetalle_Ingreso> Detalle, DCuentaPagar CuentaPagar)
        {
            string respuesta = "";

            string query = @"
                        INSERT INTO ingreso(
                            idTrabajador,
                            idProveedor,
                            fecha,
                            tipoComprobante,
                            serieComprobante,
                            impuesto,
                            metodoPago,
                            estado
                        ) VALUES(
                            @idTrabajador,
                            @idProveedor,
                            @fecha,
                            @tipoComprobante,
                            @serieComprobante,
                            @impuesto,
                            @metodoPago
                            @estado
                        );
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@idTrabajador", Ingreso.idTrabajador);
                    comm.Parameters.AddWithValue("@idProveedor", Ingreso.idProveedor);
                    comm.Parameters.AddWithValue("@fecha", Ingreso.fecha);
                    comm.Parameters.AddWithValue("@tipoComprobante", Ingreso.tipoComprobante);
                    comm.Parameters.AddWithValue("@serieComprobante", Ingreso.serieComprobante);
                    comm.Parameters.AddWithValue("@impuesto", Ingreso.impuesto);
                    comm.Parameters.AddWithValue("@metodoPago", Ingreso.metodoPago);
                    comm.Parameters.AddWithValue("@estado", 1);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del ingreso";

                        this.idIngreso = Convert.ToInt32(comm.Parameters["@idIngreso"].Value);

                        if (respuesta.Equals("OK") && Ingreso.metodoPago == 2)
                        {

                            string query2 = @"
                                INSERT INTO cuentaPagar(
                                    idIngreso,
                                    fechaInicio,
                                    fechaLimite,
                                    montoIngresado
                                ) VALUES(
                                    @idIngreso,
                                    @fechaInicio,
                                    @fechaLimite,
                                    @montoIngresado
                                );
	                        ";

                            using (SqlCommand comm2 = new SqlCommand(query2, conn))
                            {
                                comm2.Parameters.AddWithValue("@idIngreso", this.idIngreso);
                                comm2.Parameters.AddWithValue("@fechaInicio", CuentaPagar.fechaInicio);
                                comm2.Parameters.AddWithValue("@fechaLimite", CuentaPagar.fechaLimite);
                                comm2.Parameters.AddWithValue("@montoIngresado", CuentaPagar.montoIngresado);

                                respuesta = comm2.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta por cobrar";
                            }
                        }

                        if(respuesta.Equals("OK"))
                        {
                            int i=0;
                            foreach (DDetalle_Ingreso det in Detalle)
                            {

                                string query3 = @"
                                    INSERT INTO detalleIngreso(
                                        idIngreso,
                                        idArticulo,
                                        precioCompra,
                                        precioVenta,
                                        cantidadInicial,
                                        cantidadActual
                                    ) VALUES(
                                        @idIngreso,
                                        @idArticulo,
                                        @precioCompra,
                                        @precioVenta,
                                        @cantidadInicial,
                                        @cantidadActual
                                    );
	                            ";

                                using (SqlCommand comm3 = new SqlCommand(query3, conn))
                                {
                                    comm3.Parameters.AddWithValue("@idIngreso", this.idIngreso);
                                    comm3.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);
                                    comm3.Parameters.AddWithValue("@precioCompra", Detalle[i].precioCompra);
                                    comm3.Parameters.AddWithValue("@precioVenta", Detalle[i].precioVenta);
                                    comm3.Parameters.AddWithValue("@cantidadInicial", Detalle[i].cantidadInicial);
                                    comm3.Parameters.AddWithValue("@cantidadActual", Detalle[i].cantidadActual);


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





        public string Eiminar(DIngreso Ingreso)
        {
            string respuesta = "";

            string query = @"
                        DELETE FROM ingreso WHERE idIngreso=@idIngreso
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {

                    comm.Parameters.AddWithValue("@idIngreso", Ingreso.idIngreso);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se elimino el Registro del ingreso";
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
        public List<DIngreso> Mostrar(string Buscar, string Buscar2)
        {
            List<DIngreso> ListaGenerica = new List<DIngreso>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT i.idIngreso, t.cedula, p.razonSocial,i.fecha, i.tipoComprobante, i.serieComprobante, i.metodoPago, i.estado, SUM(di.precioCompra) as precioTotal from [ingreso] i inner join [proveedor] p on i.idProveedor=p.idProveedor inner join [trabajador] t on i.idTrabajador=t.idTrabajador inner join [detalleIngreso] dt on i.idIngreso=dt.idIngreso where tipoComprobante = " + Buscar +" AND serieComprobante like '" + Buscar2 + "%' order by serieComprobante";


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DIngreso
                                {
                                    idIngreso = reader.GetInt32(0),
                                    cedulaTrabajador = reader.GetString(1),
                                    razonSocial = reader.GetString(2),
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