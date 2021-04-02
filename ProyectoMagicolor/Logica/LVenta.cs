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
            int ID = 1;

            string respuesta = "";

            string queryID = "SELECT max(idVenta) FROM venta";

            string query = @"
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

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand commID = new SqlCommand(queryID, conn))
                {

                    try
                    {
                        conn.Open();

                        using (SqlDataReader reader = commID.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    ID = reader.GetInt32(0);
                                    ID++;
                                }

                            }

                        }

                        using (SqlCommand comm = new SqlCommand(query, conn))
                        {
                            comm.Parameters.AddWithValue("@idVenta", ID);
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
                                    comm2.Parameters.AddWithValue("@idVenta", ID);
                                    comm2.Parameters.AddWithValue("@fechaInicio", CuentaCobrar.fechaInicio);
                                    comm2.Parameters.AddWithValue("@fechaLimite", CuentaCobrar.fechaLimite);
                                    comm2.Parameters.AddWithValue("@montoIngresado", 0);

                                    respuesta = comm2.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta por pagar";
                                }
                            }

                            if (respuesta.Equals("OK"))
                            {
                                int i = 0;

                                foreach (DDetalle_Venta det in Detalle)
                                {
                                    string queryStock = @"
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
 
                                    using (SqlCommand comm3 = new SqlCommand(queryStock, conn))
                                    {
                                            comm3.Parameters.AddWithValue("@cantidad", Detalle[i].cantidad);
                                            comm3.Parameters.AddWithValue("@idDetalleIngreso", Detalle[i].idDetalleIngreso);

                                            respuesta = comm3.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la Venta";

                                            if (respuesta.Equals("OK"))
                                            {
                                                string query4 = @"
                                                    INSERT INTO detalleVenta(
                                                        idVenta,
                                                        idDetalleIngreso,
                                                        cantidad,
                                                        precioVenta
                                                    ) VALUES(
                                                        @idVenta,
                                                        @idDetalleIngreso,
                                                        @cantidad,
                                                        @precioVenta
                                                    );
	                                            ";

                                                using (SqlCommand comm4 = new SqlCommand(query4, conn))
                                                {
                                                    comm4.Parameters.AddWithValue("@idVenta", ID);
                                                    comm4.Parameters.AddWithValue("@idDetalleIngreso", Detalle[i].idDetalleIngreso);
                                                    comm4.Parameters.AddWithValue("@cantidad", Detalle[i].cantidad);
                                                    comm4.Parameters.AddWithValue("@precioVenta", Detalle[i].precioVenta);

                                                    respuesta = comm4.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del detalle";
                                                    i++;
                                                }

                                                if (!respuesta.Equals("OK"))
                                                {
                                                    break;
                                                }
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

                    comm.CommandText = "SELECT dv.idDetalleVenta, dv.idVenta, a.idArticulo, a.codigo, a.nombre, dv.cantidad, dv.precioVenta from [detalleVenta] dv inner join [detalleIngreso] di on di.idDetalleIngreso = dv.idDetalleIngreso inner join [articulo] a on di.idArticulo = a.idArticulo where dv.idVenta = " + Buscar;

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




        //cuentas por cobrar
        public List<DVenta> MostrarCxC(string Buscar, string Buscar2)
        {
            List<DVenta> ListaGenerica = new List<DVenta>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = @"SELECT 
                                            v.idVenta,
                                            c.tipoDocumento + '-' + c.numeroDocumento,
                                            c.apellidos + ' ' + c.nombre, 
                                            v.fecha, 
                                            ((SUM(dv.precioVenta) * dv.cantidad) - cc.montoIngresado - v.descuento) as montoTotal
                                        from [venta] v 
                                            inner join [cliente] c on v.idCliente=c.idCliente 
                                            inner join [trabajador] t on v.idTrabajador=t.idTrabajador 
                                            inner join [detalleVenta] dv on v.idVenta=dv.idVenta 
                                            inner join [cuentaCobrar] cc on v.idVenta=cc.idVenta 
                                        where v.estado = 2 AND c.tipoDocumento LIKE '" + Buscar + "%' AND c.numeroDocumento LIKE '" + Buscar2 + "%' group by v.idVenta, c.tipoDocumento, c.numeroDocumento, c.apellidos, c.nombre, v.fecha, cc.montoIngresado, dv.cantidad, v.descuento";


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
                                    cedulaCliente = reader.GetString(1),
                                    cliente = reader.GetString(2),
                                    fecha = reader.GetDateTime(3),
                                    montoTotal = (double)reader.GetDecimal(4)
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



        public string RegistrarCxC(DRegistro_CuentaCobrar RegistroCuentaCobrar, int IdCuentaCobrar)
        {
            string respuesta = "";


            string query = @"
                          UPDATE cuentaCobrar SET
                               cuentaCobrar.montoIngresado = cuentaCobrar.montoIngresado + @monto
                          FROM cuentaCobrar
                          inner join (
							   SELECT venta.idVenta 
							   FROM venta 
							       inner join [cuentaCobrar] cc on venta.idVenta=cc.idVenta
                                   inner join [detalleVenta] dv on dv.idVenta=venta.idVenta
							       inner join [detalleIngreso] di on di.idDetalleIngreso=dv.idDetalleIngreso
							   WHERE venta.idVenta = cc.idVenta AND venta.estado = 2
							   GROUP BY venta.idVenta , cc.montoIngresado, dv.cantidad
							   HAVING ((SUM(dv.precioVenta)*dv.cantidad) - (cc.montoIngresado + @monto)) >= 0
						  ) X
                          ON x.idVenta = cuentaCobrar.idVenta
                          WHERE cuentaCobrar.idCuentaCobrar = @idCuentaCobrar;
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@monto", RegistroCuentaCobrar.monto);
                    comm.Parameters.AddWithValue("@idCuentaCobrar", IdCuentaCobrar);


                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta";


                        if (respuesta.Equals("OK"))
                        {
                            string query2 = @"
                                        INSERT INTO registroCobrar(
                                            idCuentaCobrar,
                                            monto,
                                            fecha
                                        ) VALUES (
                                            @idCuentaCobrar,
                                            @monto,
                                            @fecha
                                        );
	                        ";


                            using (SqlCommand comm2 = new SqlCommand(query2, conn))
                            {
                                comm2.Parameters.AddWithValue("@idCuentaCobrar", RegistroCuentaCobrar.idCuentaCobrar);
                                comm2.Parameters.AddWithValue("@monto", RegistroCuentaCobrar.monto);
                                comm2.Parameters.AddWithValue("@fecha", DateTime.Now);

                                respuesta = comm2.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta por cobrar";


                                if (respuesta.Equals("OK"))
                                {
                                    string query3 = @"
                                        UPDATE venta SET venta.estado = 1
                                        from [venta]
                                        inner join [cuentaCobrar] cc on venta.idVenta=cc.idVenta
										inner join (
											SELECT venta.idVenta 
											FROM venta 
											inner join [cuentaCobrar] cc on venta.idVenta=cc.idVenta
											inner join [detalleVenta] dv on venta.idVenta=dv.idVenta
											WHERE venta.idVenta = cc.idVenta AND venta.estado = 2
											GROUP BY venta.idVenta , cc.montoIngresado, dv.cantidad, venta.descuento, dv.precioVenta
											HAVING ((dv.precioVenta * dv.cantidad) - cc.montoIngresado - venta.descuento) = 0
										) X
										ON x.idVenta=cc.idVenta
										WHERE cc.idCuentaCobrar = " + IdCuentaCobrar + " ";

                                    using (SqlCommand comm3 = new SqlCommand(query3, conn))
                                    {
                                        respuesta = comm3.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Actualizo el registro";
                                    }
                                }
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





        public List<DVenta> EncontrarCxC(int Buscar)
        {
            List<DVenta> ListaGenerica = new List<DVenta>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = @"SELECT 
                                            v.idVenta,
                                            cc.idCuentaCobrar,
                                            c.tipoDocumento + '-' + c.numeroDocumento,
                                            c.apellidos + ' ' + c.nombre, 
                                            v.fecha, 
                                            ((SUM(dv.precioVenta) * dv.cantidad) - v.descuento) as montoTotal,
                                            (SUM(dv.precioVenta) * dv.cantidad - cc.montoIngresado - v.descuento) as monto
                                        from [venta] v 
                                            inner join [cliente] c on v.idCliente=c.idCliente 
                                            inner join [detalleVenta] dv on v.idVenta=dv.idVenta 
                                            inner join [cuentaCobrar] cc on v.idVenta=cc.idVenta 
                                        where v.estado = 2 AND v.idVenta = " + Buscar + " group by v.idVenta,dv.cantidad, cc.idCuentaCobrar, cc.montoIngresado, c.tipoDocumento, c.numeroDocumento, c.apellidos, c.nombre, v.fecha, v.descuento order by cc.idCuentaCobrar ASC";


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
                                    idCuentaCobrar = reader.GetInt32(1),
                                    cedulaCliente = reader.GetString(2),
                                    cliente = reader.GetString(3),
                                    fecha = reader.GetDateTime(4),
                                    monto = (double)reader.GetDecimal(5),
                                    montoTotal = (double)reader.GetDecimal(6)

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

        public List<DCuentaCobrar> EncontrarCuentaCobrar(int Buscar)
        {
            List<DCuentaCobrar> ListaGenerica = new List<DCuentaCobrar>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "Select * from [cuentaCobrar] where idVenta = " + Buscar;


                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DCuentaCobrar
                                {
                                    idCuentaCobrar = reader.GetInt32(0),
                                    idVenta = reader.GetInt32(1),
                                    fechaInicio = reader.GetDateTime(2),
                                    fechaLimite = reader.GetDateTime(3),
                                    montoIngresado = (double)reader.GetDecimal(4)
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
