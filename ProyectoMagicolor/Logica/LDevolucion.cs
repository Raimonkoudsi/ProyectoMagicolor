﻿using System;
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
            int ID = 1;

            string respuesta = "";

            string queryID = "SELECT max(idDevolucion) FROM devolucion";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {
                using (SqlCommand commID = new SqlCommand(queryID, conn))
                {

                    try
                    {
                        conn.Open();

                        using (SqlDataReader reader = commID.ExecuteReader())
                        {
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                ID = reader.GetInt32(0);
                                ID++;
                            }
                        }

                        string query = @"
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

                        using (SqlCommand comm = new SqlCommand(query, conn))
                        {
                            comm.Parameters.AddWithValue("@idDevolucion", ID);
                            comm.Parameters.AddWithValue("@idCliente", Devolucion.idCliente);
                            comm.Parameters.AddWithValue("@idTrabajador", Devolucion.idTrabajador);
                            comm.Parameters.AddWithValue("@idVenta", Devolucion.idVenta);
                            comm.Parameters.AddWithValue("@fecha", Devolucion.fecha);

                            respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la Devolucion";


                            if (respuesta.Equals("OK"))
                            {
                                int i = 0;

                                foreach (DDetalle_Devolucion det in Detalle)
                                {
                                    if (Detalle[i].dañado == 1)
                                    {
                                        string queryStock = @"
                                            UPDATE detalleIngreso SET 
                                                   cantidadActual = detalleIngreso.cantidadActual + @cantidad
                                            WHERE idArticulo = @idArticulo 
                                            AND idDetalleIngreso=(SELECT MAX(idDetalleIngreso) FROM detalleIngreso)";

                                        using (SqlCommand comm2 = new SqlCommand(queryStock, conn))
                                        {
                                            comm2.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);
                                            comm2.Parameters.AddWithValue("@precioCompra", Detalle[i].cantidad);


                                            respuesta = comm2.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso la actualizacion del ingreso";
                                        }
                                    }

                                    if (respuesta.Equals("OK"))
                                    {
                                        string query3 = @"
                                            INSERT INTO detalleDevolucion(
                                                    idDevolucion,
                                                    idDetalleVenta,
                                                    idArticulo,
                                                    cantidad,
                                                    precio,
                                                    dañado
                                                ) VALUES (
                                                    @idDevolucion,
                                                    @idDetalleVenta,
                                                    @idArticulo,
                                                    @cantidad,
                                                    @precio,
                                                    @dañado
                                                );
	                                    ";

                                        using (SqlCommand comm3 = new SqlCommand(query3, conn))
                                        {
                                            comm3.Parameters.AddWithValue("@idDevolucion", ID);
                                            comm3.Parameters.AddWithValue("@idDetalleVenta", Detalle[i].idDetalleVenta);
                                            comm3.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);
                                            comm3.Parameters.AddWithValue("@cantidad", Detalle[i].cantidad);
                                            comm3.Parameters.AddWithValue("@precio", Detalle[i].precio);
                                            comm3.Parameters.AddWithValue("@dañado", Detalle[i].dañado);

                                            respuesta = comm3.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del detalle";


                                            if(respuesta.Equals("OK"))
                                            {
                                                string query4 = @"
														SELECT TOP 1 dv.cantidad
                                                        FROM detalleVenta dv
                                                            inner join detalleIngreso di ON dv.idDetalleIngreso=di.idDetalleIngreso
                                                        WHERE dv.idDetalleVenta = @idDetalleVenta
                                                        AND di.idArticulo = @idArticulo
														ORDER BY dv.idDetalleIngreso DESC
                                                ;";

                                                using (SqlCommand comm4 = new SqlCommand(query4, conn))
                                                {
                                                    comm4.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);
                                                    comm4.Parameters.AddWithValue("@idDetalleVenta", Detalle[i].idDetalleVenta);

                                                    using (SqlDataReader reader = comm4.ExecuteReader())
                                                    {
                                                        if (reader.Read() && !reader.IsDBNull(0))
                                                            cantidad = reader.GetInt32(0);
                                                        else
                                                            break;
                                                    }
                                                }

                                                //si la cantidad a devolver es igual a la de la venta, se anula el detalle venta
                                                if(cantidad == Detalle[i].cantidad)
                                                {
                                                    string query5 = @"
														    DELETE dv FROM detalleVenta dv
                                                            inner join detalleIngreso di ON dv.idDetalleIngreso=di.idDetalleIngreso
                                                            WHERE dv.idDetalleVenta = @idDetalleVenta
                                                            AND di.idArticulo = @idArticulo
                                                    ;";

                                                    using (SqlCommand comm5 = new SqlCommand(query5, conn))
                                                    {
                                                        comm5.Parameters.AddWithValue("@idDetalleVenta", Detalle[i].idDetalleVenta);
                                                        comm5.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);

                                                        respuesta = comm5.ExecuteNonQuery() == 1 ? "OK" : "No se actualizó el Registro del detalle";
                                                    }
                                                }
                                                //si la cantidad a devolver es menor a la de la venta, se actualiza el detalle venta
                                                else if (cantidad > Detalle[i].cantidad)
                                                {
                                                    string query6 = @"
                                                            UPDATE detalleVenta SET
                                                                cantidad = dv.cantidad - @cantidad
                                                            FROM detalleVenta dv
                                                                inner join detalleIngreso di ON dv.idDetalleIngreso=di.idDetalleIngreso
                                                            WHERE dv.idDetalleVenta = @idDetalleVenta
                                                            AND di.idArticulo = @idArticulo
                                                    ;";

                                                    using (SqlCommand comm6 = new SqlCommand(query6, conn))
                                                    {
                                                        comm6.Parameters.AddWithValue("@cantidad", Detalle[i].cantidad);
                                                        comm6.Parameters.AddWithValue("@idDetalleVenta", Detalle[i].idDetalleVenta);
                                                        comm6.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);

                                                        respuesta = comm6.ExecuteNonQuery() == 1 ? "OK" : "No se actualizó el Registro del detalle";
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    i++;

                                    if (!respuesta.Equals("OK"))
                                    {
                                        break;
                                    }
                                }

                                string query7 = @"
											SELECT COUNT(idDetalleVenta)
                                            FROM detalleVenta
                                            WHERE idVenta = @idVenta
                                ;";

                                using (SqlCommand comm7 = new SqlCommand(query7, conn))
                                {
                                    comm7.Parameters.AddWithValue("@idVenta", Devolucion.idVenta);

                                    using (SqlDataReader reader = comm7.ExecuteReader())
                                    {
                                        if (reader.Read() && !reader.IsDBNull(0))
                                            cantidad = reader.GetInt32(0);
                                    }

                                    //anula la venta si devuelve todos los articulos
                                    if(cantidad==0)
                                    {
                                        string query8 = @"
                                                    UPDATE venta SET estado = 3
						                            WHERE idVenta = @idVenta
                                        ";

                                        using (SqlCommand comm8 = new SqlCommand(query8, conn))
                                        {
                                            comm7.Parameters.AddWithValue("@idVenta", Devolucion.idVenta);

                                            respuesta = comm7.ExecuteNonQuery() == 1 ? "OK" : "No se anulo la venta en devolucion";
                                        }
                                    }
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



        public List<DVenta> MostrarVenta(string Buscar)
        {
            List<DVenta> ListaGenerica = new List<DVenta>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT " +
                            "v.idVenta, " +
                            "c.cedula " + "-" + " c.nombre, " +
                            "v.fecha, " +
                            "v.metodoPago, " +
                            "v.estado, " +
                            "SUM(dv.precioVenta) as precioTotal " +
                        "from [venta] v " +
                        "inner join [cliente] c on v.idCliente=v.idCliente " +
                        "inner join [trabajador] t on v.idTrabajador=t.idTrabajador " +
                        "inner join [detalleVenta] dv on v.idVenta=dv.idVenta " +
                        "where v.idVenta = " + Buscar + " ";

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
                                    cliente = reader.GetString(1),
                                    fecha = reader.GetDateTime(2),
                                    metodoPago = reader.GetInt32(3),
                                    estado = reader.GetInt32(4),
                                    montoTotal = reader.GetDouble(5)
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



        public List<DDetalle_Venta> MostrarDetalleVenta(string Buscar)
        {
            List<DDetalle_Venta> ListaGenerica = new List<DDetalle_Venta>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT " +
                            "dv.idDetalleVenta, " +
                            "dv.idVenta, " +
                            "a.codigo, " +
                            "a.nombre, " +
                            "dv.cantidad, " +
                            "dv.precioVenta " +
                        "from [detalleVenta] dv " +
                        "inner join [detalleIngreso] di on di.idDetalleIngreso = dv.idDetalleIngreso " +
                        "inner join [articulo] a on di.idArticulo = a.idArticulo " +
                        "where dv.idVenta = " + Buscar;

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DDetalle_Venta
                                {
                                    idDetalleIngreso = reader.GetInt32(0),
                                    idVenta = reader.GetInt32(1),
                                    codigo = reader.GetString(2),
                                    nombre = reader.GetString(3),
                                    cantidad = reader.GetInt32(4),
                                    precioVenta = (double)reader.GetDecimal(5)
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
