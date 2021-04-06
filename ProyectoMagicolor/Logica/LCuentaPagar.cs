using System;
using System.Collections.Generic;
using System.Text;
using Datos;

using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LCuentaPagar:DCuentaPagar
    {

        public List<DIngreso> MostrarCxP(string Buscar)
        {
            List<DIngreso> ListaGenerica = new List<DIngreso>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = @"SELECT 
                                            i.idIngreso,
                                            p.razonSocial, 
                                            i.factura, 
                                            i.fecha, 
                                            ((SUM(di.precioCompra) * di.cantidadInicial) - cp.montoIngresado) as montoActual,
                                            t.cedula
                                        from [ingreso] i 
                                            inner join [proveedor] p on i.idProveedor=p.idProveedor 
                                            inner join [trabajador] t on i.idTrabajador=t.idTrabajador 
                                            inner join [detalleIngreso] di on i.idIngreso=di.idIngreso 
                                            inner join [cuentaPagar] cp on i.idIngreso=cp.idIngreso 
                                        where i.estado = 2 AND p.razonSocial LIKE '" + Buscar + "%' group by i.idIngreso, p.razonSocial, i.factura, i.fecha, t.cedula, cp.montoIngresado, di.cantidadInicial";

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
                                    razonSocial = reader.GetString(1),
                                    factura = reader.GetString(2),
                                    fecha = reader.GetDateTime(3),
                                    montoTotal = (double)reader.GetDecimal(4),
                                    cedulaTrabajador = reader.GetString(5)
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


        public string RegistrarCxP(DRegistro_CuentaPagar RegistroCuentaPagar, int IdCuentaPagar)
        {
            string respuesta = "";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                try
                {
                    conn.Open();

                    #region Actualizar Monto Cuenta Pagar
                    string queryUpdateCP = @"
                                  UPDATE cuentaPagar SET
                                       cuentaPagar.montoIngresado = cuentaPagar.montoIngresado + @monto
                                  FROM cuentaPagar
                                  inner join (
							           SELECT ingreso.idIngreso 
							           FROM ingreso 
							           inner join [cuentaPagar] cp on ingreso.idIngreso=cp.idIngreso
							           inner join [detalleIngreso] di on ingreso.idIngreso=di.idIngreso
							           WHERE ingreso.idIngreso = cp.idIngreso AND ingreso.estado = 2
							           GROUP BY ingreso.idIngreso , cp.montoIngresado, di.cantidadInicial
							           HAVING ((SUM(di.precioCompra)*di.cantidadInicial) - (cp.montoIngresado + @monto)) >= 0
						          ) X
                                  ON x.idIngreso = cuentaPagar.idIngreso
                                  WHERE cuentaPagar.idCuentaPagar = @idCuentaPagar;
	                ";

                    using (SqlCommand comm = new SqlCommand(queryUpdateCP, conn))
                    {
                        comm.Parameters.AddWithValue("@monto", RegistroCuentaPagar.monto);
                        comm.Parameters.AddWithValue("@idCuentaPagar", IdCuentaPagar);


                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta";
                    }
                    #endregion

                    if (respuesta.Equals("OK"))
                    {

                        LID getID = new LID();

                        int ID = getID.ID("registroCuentaPagar", "idRegistro");

                        #region Registrar Cuenta Pagar
                        string queryAddRegisterCP = @"
                                    INSERT INTO registroCuentaPagar (
                                        idRegistro,
                                        idCuentaPagar,
                                        monto,
                                        fecha
                                    ) VALUES (
                                        @idRegistro,
                                        @idCuentaPagar,
                                        @monto,
                                        @fecha
                                    );
	                    ";

                        using (SqlCommand comm = new SqlCommand(queryAddRegisterCP, conn))
                        {
                            comm.Parameters.AddWithValue("@idRegistro", ID);
                            comm.Parameters.AddWithValue("@idCuentaPagar", RegistroCuentaPagar.idCuentaPagar);
                            comm.Parameters.AddWithValue("@monto", RegistroCuentaPagar.monto);
                            comm.Parameters.AddWithValue("@fecha", DateTime.Now);

                            respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta por pagar";
                        }
                        #endregion

                        if (respuesta.Equals("OK"))
                        {
                            #region Cambiar Estado Venta
                            string queryUpdateSale = @"
                                    UPDATE ingreso SET ingreso.estado = 1
                                    from [ingreso]
                                    inner join [cuentaPagar] cp on ingreso.idIngreso=cp.idIngreso
									inner join (
										SELECT ingreso.idIngreso 
										FROM ingreso 
										inner join [cuentaPagar] cp on ingreso.idIngreso=cp.idIngreso
										inner join [detalleIngreso] di on ingreso.idIngreso=di.idIngreso
										WHERE ingreso.idIngreso = cp.idIngreso AND ingreso.estado = 2
										GROUP BY ingreso.idIngreso , cp.montoIngresado, di.cantidadInicial
										HAVING ((SUM(di.precioCompra)*di.cantidadInicial) - cp.montoIngresado) = 0
									) X
									ON x.idIngreso=cp.idIngreso
									WHERE cp.idCuentaPagar = " + IdCuentaPagar;

                            using (SqlCommand comm3 = new SqlCommand(queryUpdateSale, conn))
                            {
                                respuesta = comm3.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Actualizo el registro";
                            }
                            #endregion
                        }
                        else
                            return "No se ingreso el Registro de la cuenta por pagar";
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


        public List<DIngreso> EncontrarCxP(int Buscar)
        {
            List<DIngreso> ListaGenerica = new List<DIngreso>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = @"SELECT 
                                            i.idIngreso,
                                            cp.idCuentaPagar,
                                            p.razonSocial, 
                                            i.factura, 
                                            i.fecha, 
                                            (SUM(di.precioCompra) * di.cantidadInicial) as montoTotal,
                                            (SUM(di.precioCompra) * di.cantidadInicial - SUM(cp.montoIngresado)) as monto
                                        from [ingreso] i 
                                            inner join [proveedor] p on i.idProveedor=p.idProveedor 
                                            inner join [trabajador] t on i.idTrabajador=t.idTrabajador 
                                            inner join [detalleIngreso] di on i.idIngreso=di.idIngreso 
                                            inner join [cuentaPagar] cp on i.idIngreso=cp.idIngreso 
                                        where i.estado = 2 AND i.idIngreso = " + Buscar + " group by i.idIngreso,di.cantidadInicial, cp.idCuentaPagar, p.razonSocial, i.factura, i.fecha order by cp.idCuentaPagar ASC";

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
                                    idCuentaPagar = reader.GetInt32(1),
                                    razonSocial = reader.GetString(2),
                                    factura = reader.GetString(3),
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
    }
}
