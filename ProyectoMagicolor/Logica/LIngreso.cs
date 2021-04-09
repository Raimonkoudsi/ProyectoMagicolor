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

            string respuesta = "";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                try
                {
                    conn.Open();

                    LFunction getID = new LFunction();

                    int IDCompra = getID.GetID("ingreso", "idIngreso");

                    #region Insertar Ingreso
                    string queryAddBuy = @"
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

                    using (SqlCommand comm = new SqlCommand(queryAddBuy, conn))
                    {
                        comm.Parameters.AddWithValue("@idIngreso", IDCompra);
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

                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del ingreso";

                    }
                    #endregion

                    if (respuesta.Equals("OK") && Ingreso.metodoPago == 2)
                    {
                        int IDCP = getID.GetID("cuentaPagar", "idCuentaPagar");

                        #region Insertar Cuenta Pagar
                        string queryAddCP = @"
                                INSERT INTO cuentaPagar (
                                    idCuentaPagar,
                                    idIngreso,
                                    fechaInicio,
                                    fechaLimite,
                                    montoIngresado
                                ) VALUES (
                                    @idCuentaPagar,
                                    @idIngreso,
                                    @fechaInicio,
                                    @fechaLimite,
                                    0
                                );
	                    ";

                        using (SqlCommand comm = new SqlCommand(queryAddCP, conn))
                        {
                            comm.Parameters.AddWithValue("@idCuentaPagar", IDCP);
                            comm.Parameters.AddWithValue("@idIngreso", CuentaPagar.idIngreso);
                            comm.Parameters.AddWithValue("@fechaInicio", CuentaPagar.fechaInicio);
                            comm.Parameters.AddWithValue("@fechaLimite", CuentaPagar.fechaLimite);

                            respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta por pagar";
                        }
                        #endregion
                    }
                    else if (!respuesta.Equals("OK"))
                        return "No se ingreso el Registro del ingreso";


                    if (respuesta.Equals("OK"))
                    {
                        int i = 0, stock = 0;

                        foreach (DDetalle_Ingreso det in Detalle)
                        {
                            #region Obtener Stock
                            string querySelectStock = @"
                                        SELECT TOP 1
                                            cantidadActual
                                        FROM [detalleIngreso] WHERE idArticulo= " + Detalle[i].idArticulo + @"
                                        ORDER BY idDetalleIngreso DESC";

                            using (SqlCommand comm = new SqlCommand(querySelectStock, conn))
                            {
                                using (SqlDataReader reader = comm.ExecuteReader())
                                {
                                    if (reader.Read() && !reader.IsDBNull(0))
                                        stock = reader.GetInt32(0);
                                }
                            }
                            #endregion

                            int IDDetCompra = getID.GetID("detalleIngreso", "idDetalleIngreso");

                            #region Insertar Detalle Ingreso
                            string queryAddDetailBuy = @"
                                    INSERT INTO detalleIngreso (
                                        idDetalleIngreso,
                                        idIngreso,
                                        idArticulo,
                                        precioCompra,
                                        precioVenta,
                                        cantidadInicial,
                                        cantidadActual
                                    ) VALUES (
                                        @idDetalleIngreso,
                                        @idIngreso,
                                        @idArticulo,
                                        @precioCompra,
                                        @precioVenta,
                                        @cantidadInicial,
                                        @cantidadInicial + " + stock + @"
                                    )
	                         ";

                            using (SqlCommand comm = new SqlCommand(queryAddDetailBuy, conn))
                            {
                                comm.Parameters.AddWithValue("@idDetalleIngreso", IDDetCompra);
                                comm.Parameters.AddWithValue("@idIngreso", IDCompra);
                                comm.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);
                                comm.Parameters.AddWithValue("@precioCompra", Detalle[i].precioCompra);
                                comm.Parameters.AddWithValue("@precioVenta", Detalle[i].precioVenta);
                                comm.Parameters.AddWithValue("@cantidadInicial", Detalle[i].cantidadInicial);

                                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del detalle ingreso";
                                i++;
                            }
                            #endregion

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


        public string Eiminar(DIngreso Ingreso)
        {
            string respuesta = "";

            string query = @"
                        UPDATE ingreso SET estado = 3
						WHERE idIngreso = @idIngreso
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


        //ATENCION
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


    }
}
