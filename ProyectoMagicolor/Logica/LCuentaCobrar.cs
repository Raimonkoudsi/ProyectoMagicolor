using System;
using System.Collections.Generic;
using System.Text;
using Datos;

using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LCuentaCobrar:DCuentaCobrar
    {

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

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                try
                {
                    conn.Open();

                    #region Actualizar Monto Cuenta Cobrar
                    string queryUpdateCC = @"
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

                    using (SqlCommand comm = new SqlCommand(queryUpdateCC, conn))
                    {
                        comm.Parameters.AddWithValue("@monto", RegistroCuentaCobrar.monto);
                        comm.Parameters.AddWithValue("@idCuentaCobrar", IdCuentaCobrar);

                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se actualizó la cuenta por cobrar";
                    }
                    #endregion

                    if (respuesta.Equals("OK"))
                    {
                        LFunction getID = new LFunction();

                        int ID = getID.GetID("registroCuentaCobrar", "idRegistro");

                        #region Añadir Registro Cuenta Cobrar
                        string queryAddRegCC = @"
                                INSERT INTO registroCuentaCobrar(
                                    idRegistro,
                                    idCuentaCobrar,
                                    monto,
                                    fecha
                                ) VALUES (
                                    @idRegistro,
                                    @idCuentaCobrar,
                                    @monto,
                                    @fecha
                                );
	                    ";


                        using (SqlCommand comm = new SqlCommand(queryAddRegCC, conn))
                        {
                            comm.Parameters.AddWithValue("@idRegistro", ID);
                            comm.Parameters.AddWithValue("@idCuentaCobrar", RegistroCuentaCobrar.idCuentaCobrar);
                            comm.Parameters.AddWithValue("@monto", RegistroCuentaCobrar.monto);
                            comm.Parameters.AddWithValue("@fecha", DateTime.Now);

                            respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta por cobrar";
                        }
                        #endregion
                    }
                    else
                        return "No se ingreso el Registro de la cuenta por cobrar";


                    if (respuesta.Equals("OK"))
                    {
                        #region Actualizar Venta si se Completa
                        string queryUpdateSale = @"
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

                        using (SqlCommand comm = new SqlCommand(queryUpdateSale, conn))
                        {
                            respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Actualizo el registro";
                        }
                        #endregion
                    }
                    else
                        return "No se actualizó la cuenta por cobrar";

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


        public List<DCuentaCobrar> BuscarCxC(int Buscar)
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
