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
    public class LIngreso : DIngreso
    {

        public string Insertar(DIngreso Ingreso, List<DDetalle_Ingreso> Detalle, DCuentaPagar CuentaPagar)
        {
            int ID=1;

            string respuesta = "";

            string queryID = "SELECT max(idIngreso) FROM ingreso";

            string query = @"
                        INSERT INTO ingreso(
                            idIngreso,
                            idTrabajador,
                            idProveedor,
                            fecha,
                            factura,
                            impuesto,
                            metodoPago,
                            estado
                        ) VALUES (
                            @idIngreso,
                            @idTrabajador,
                            @idProveedor,
                            @fecha,
                            @factura,
                            @impuesto,
                            @metodoPago,
                            @estado
                        );
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {
                using (SqlCommand comm4 = new SqlCommand(queryID, conn))
                {
                    try
                    {
                        conn.Open();

                        using (SqlDataReader reader = comm4.ExecuteReader())
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
                            comm.Parameters.AddWithValue("@idIngreso", ID);
                            comm.Parameters.AddWithValue("@idTrabajador", Ingreso.idTrabajador);
                            comm.Parameters.AddWithValue("@idProveedor", Ingreso.idProveedor);
                            comm.Parameters.AddWithValue("@fecha", Ingreso.fecha);
                            comm.Parameters.AddWithValue("@factura", Ingreso.factura);
                            comm.Parameters.AddWithValue("@impuesto", Ingreso.impuesto);
                            comm.Parameters.AddWithValue("@metodoPago", Ingreso.metodoPago);

                            if (Ingreso.metodoPago == 2)
                                comm.Parameters.AddWithValue("@estado", 2);
                            else
                                comm.Parameters.AddWithValue("@estado", 1);

                            //this.idIngreso = (int)comm.ExecuteScalar();

                            respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del ingreso";

                            if (respuesta.Equals("OK") && Ingreso.metodoPago == 2)
                            {
                                string query2 = @"
                                    INSERT INTO cuentaPagar(
                                        idIngreso,
                                        fechaInicio,
                                        fechaLimite,
                                        montoIngresado
                                    ) VALUES (
                                        @idIngreso,
                                        @fechaInicio,
                                        @fechaLimite,
                                        @montoIngresado
                                    );
	                            ";

                                using (SqlCommand comm2 = new SqlCommand(query2, conn))
                                {
                                    comm2.Parameters.AddWithValue("@idIngreso", ID);
                                    comm2.Parameters.AddWithValue("@fechaInicio", CuentaPagar.fechaInicio);
                                    comm2.Parameters.AddWithValue("@fechaLimite", CuentaPagar.fechaLimite);
                                    comm2.Parameters.AddWithValue("@montoIngresado", 0);

                                    respuesta = comm2.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta por pagar";
                                }
                            }

                            if (respuesta.Equals("OK"))
                            {
                                int i = 0;
                                int stock = 0;

                                foreach (DDetalle_Ingreso det in Detalle)
                                {
                                    string queryStock = @"
                                        SELECT 
                                            SUM(cantidadInicial) as cantidadActual 
                                        FROM [detalleIngreso] WHERE idArticulo= " + Detalle[i].idArticulo + " ";

                                    using (SqlCommand comm5 = new SqlCommand(queryStock, conn))
                                    {
                                        using (SqlDataReader reader = comm5.ExecuteReader())
                                        {
                                            if (reader.Read())
                                            {
                                                if (!reader.IsDBNull(0))
                                                {
                                                    stock = reader.GetInt32(0);
                                                }

                                            }

                                        }
                                    }

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
                                            @cantidadInicial + " + stock + @"
                                        );
	                                ";

                                    using (SqlCommand comm3 = new SqlCommand(query3, conn))
                                    {
                                        comm3.Parameters.AddWithValue("@idIngreso", ID);
                                        comm3.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);
                                        comm3.Parameters.AddWithValue("@precioCompra", Detalle[i].precioCompra);
                                        comm3.Parameters.AddWithValue("@precioVenta", Detalle[i].precioVenta);
                                        comm3.Parameters.AddWithValue("@cantidadInicial", Detalle[i].cantidadInicial);


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

                    comm.CommandText = "SELECT i.idIngreso, t.cedula, p.razonSocial,i.fecha, i.factura, i.metodoPago, i.estado, SUM(di.precioCompra) as precioTotal from [ingreso] i inner join [proveedor] p on i.idProveedor=p.idProveedor inner join [trabajador] t on i.idTrabajador=t.idTrabajador inner join [detalleIngreso] di on i.idIngreso=di.idIngreso where tipoComprobante = " + Buscar + " AND serieComprobante like '" + Buscar2 + "%' order by serieComprobante";


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
                                    factura = reader.GetString(4),
                                    metodoPago = reader.GetInt32(5),
                                    estado = reader.GetInt32(6),
                                    montoTotal = reader.GetDouble(7)
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


        public List<DArticulo> MostrarStockNombre(string Buscar)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = @"SELECT TOP 1
                                            a.idArticulo,
                                            a.codigo, 
                                            a.nombre, 
                                            c.nombre,
                                            di.precioVenta,
                                            di.cantidadActual
                                        from [articulo] a 
                                            inner join [detalleIngreso] di on a.idArticulo=di.idArticulo  
                                            inner join [categoria] c on a.idCategoria=c.idCategoria
										WHERE a.nombre LIKE '" + Buscar + @"%' 
                                        GROUP BY a.codigo, a.nombre, a.idArticulo, di.cantidadActual, di.idDetalleIngreso, c.nombre, di.precioVenta
                                        ORDER BY di.idDetalleIngreso DESC";

                    try
                    {
                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DArticulo
                                {
                                    idArticulo = reader.GetInt32(0),
                                    codigo = reader.GetString(1),
                                    nombre = reader.GetString(2),
                                    categoria = reader.GetString(3),
                                    precioVenta = (double)reader.GetDecimal(4),
                                    cantidadActual = reader.GetInt32(5)
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

        //todavia no se implementa, cambiarlo como stocknombre
        public List<DArticulo> MostrarStockCodigo(string Buscar)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = @"SELECT TOP 1
                                            a.idArticulo,
                                            a.codigo, 
                                            a.nombre, 
                                            (SELECT SUM(cantidadInicial) as cantidad 
												from detalleIngreso
											) as cantidadInicial,
                                            di.cantidadActual
                                        from [articulo] a 
                                            inner join [detalleIngreso] di on a.idArticulo=di.idArticulo  
										WHERE a.codigo = " + Buscar + @" 
                                        GROUP BY a.codigo, a.nombre, a.idArticulo, di.cantidadActual, di.idDetalleIngreso
                                        HAVING di.cantidadActual > 0
                                        ORDER BY di.idDetalleIngreso DESC";

                    try
                    {
                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DArticulo
                                {
                                    idArticulo = reader.GetInt32(0),
                                    codigo = reader.GetString(1),
                                    nombre = reader.GetString(2),
                                    cantidadInicial = reader.GetInt32(3),
                                    cantidadActual = reader.GetInt32(4),
                                    cantidadVendida = reader.GetInt32(5)
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


        public List<DDetalle_Ingreso> EncontrarByArticulo(int IdArticulo)
        {
            List<DDetalle_Ingreso> ListaGenerica = new List<DDetalle_Ingreso>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT idDetalleIngreso, idIngreso, idArticulo, precioCompra, precioVenta, cantidadInicial, cantidadActual from [detalleIngreso] where idArticulo = " + IdArticulo + " and cantidadActual > 0 order by idDetalleIngreso DESC";


                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DDetalle_Ingreso
                                {
                                    idDetalleIngreso = reader.GetInt32(0),
                                    idIngreso = reader.GetInt32(1),
                                    idArticulo = reader.GetInt32(2),
                                    precioCompra = (double)reader.GetDecimal(3),
                                    precioVenta = (double)reader.GetDecimal(4),
                                    cantidadInicial = reader.GetInt32(5),
                                    cantidadActual = reader.GetInt32(6)
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

        public List<DDetalle_Ingreso> Encontrar(int Id)
        {
            List<DDetalle_Ingreso> ListaGenerica = new List<DDetalle_Ingreso>();


            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * from [detalleIngreso] where idDetalleIngreso = " + Id + " order by idDetalleIngreso DESC";


                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DDetalle_Ingreso
                                {
                                    idDetalleIngreso = reader.GetInt32(0),
                                    idIngreso = reader.GetInt32(1),
                                    idArticulo = reader.GetInt32(2),
                                    precioCompra = (double)reader.GetDecimal(3),
                                    precioVenta = (double)reader.GetDecimal(4),
                                    cantidadInicial = reader.GetInt32(5),
                                    cantidadActual = reader.GetInt32(6)
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




        //cuentas por pagar
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


            string query = @"
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

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@monto", RegistroCuentaPagar.monto);
                    comm.Parameters.AddWithValue("@idCuentaPagar", IdCuentaPagar);


                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta";


                        if (respuesta.Equals("OK"))
                        {
                            string query2 = @"
                                        INSERT INTO registroCuentaPagar(
                                            idCuentaPagar,
                                            monto,
                                            fecha
                                        ) VALUES (
                                            @idCuentaPagar,
                                            @monto,
                                            @fecha
                                        );
	                        ";


                            using (SqlCommand comm2 = new SqlCommand(query2, conn))
                            {
                                comm2.Parameters.AddWithValue("@idCuentaPagar", RegistroCuentaPagar.idCuentaPagar);
                                comm2.Parameters.AddWithValue("@monto", RegistroCuentaPagar.monto);
                                comm2.Parameters.AddWithValue("@fecha", DateTime.Now);

                                respuesta = comm2.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta por pagar";


                                if (respuesta.Equals("OK"))
                                {
                                    string query3 = @"
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
										WHERE cp.idCuentaPagar = " + IdCuentaPagar + " ";

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
