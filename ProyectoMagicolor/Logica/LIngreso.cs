using System;
using System.Collections.Generic;
using Datos;
using System.Data.SqlClient;

namespace Logica
{
    public class LIngreso : LDevolucion
    {
        #region QUERIES
        private string queryInsert = @"
            INSERT INTO [ingreso] (
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

        private string queryInsertDetail = @"
            INSERT INTO [detalleIngreso] (
                idDetalleIngreso,
                idIngreso,
                idArticulo,
                precioCompra,
                precioVenta,
                cantidadInicial,
                cantidadActual,
                estado,
                estadoArticulo
            ) VALUES (
                @idDetalleIngreso,
                @idIngreso,
                @idArticulo,
                @precioCompra,
                @precioVenta,
                @cantidadInicial,
                @cantidadInicial + (ISNULL((
                    SELECT TOP 1
                        cantidadActual
                    FROM [detalleIngreso] WHERE idArticulo = @idArticulo
                    ORDER BY idDetalleIngreso DESC
                ),0)),
                1,
                1
            );
	    ";

        private string queryInsertCP = @"
            INSERT INTO [cuentaPagar] (
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

        private string queryListDetail = @"
            SELECT 
                di.idDetalleIngreso, 
                di.idIngreso, 
                a.idArticulo, 
                a.codigo, 
                a.nombre, 
                di.cantidadInicial, 
                di.precioCompra
            FROM [detalleIngreso] di 
                INNER JOIN [articulo] a ON di.idArticulo = a.idArticulo 
				INNER JOIN [ingreso] i ON di.idIngreso = i.idIngreso
            WHERE di.idIngreso = @idIngreso AND i.estado <> 3 AND di.cantidadInicial <> 0;
        ";


        private string queryListDetailID = @"
            SELECT * FROM [detalleIngreso] 
            WHERE idDetalleIngreso = @idDetalleIngreso AND estado <> 0
            ORDER BY idDetalleIngreso DESC
        ";
        #endregion


        public string Insertar(DIngreso Ingreso, List<DDetalle_Ingreso> Detalle = null, DCuentaPagar CuentaPagar = null, bool IngresoVacio = false)
        {
            string respuesta = "";

            Action action = () =>
            {
                int idIngreso = IngresoVacio == false ? LFunction.GetID("ingreso", "idIngreso") : 0;

                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idIngreso", idIngreso);
                comm.Parameters.AddWithValue("@idTrabajador", Ingreso.idTrabajador);
                comm.Parameters.AddWithValue("@idProveedor", Ingreso.idProveedor);
                comm.Parameters.AddWithValue("@fecha", Ingreso.fecha);
                comm.Parameters.AddWithValue("@factura", Ingreso.factura);
                comm.Parameters.AddWithValue("@impuesto", Ingreso.impuesto);
                comm.Parameters.AddWithValue("@metodoPago", Ingreso.metodoPago);
                comm.Parameters.AddWithValue("@estado", Ingreso.metodoPago);
                Ingreso.idIngreso = idIngreso;

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Registró la Compra";

                if (!respuesta.Equals("OK") && !IngresoVacio)
                    throw new Exception("Error en el Registro de la Compra");

                if (Ingreso.metodoPago == 2 && !IngresoVacio)
                    if (!InsertarCxP(CuentaPagar, idIngreso).Equals("OK"))
                        throw new Exception("Error en el Registro de la Cuenta a Pagar");

                if (!IngresoVacio)
                    respuesta = InsertarDetalle(Detalle, idIngreso);
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }

        private string InsertarDetalle(List<DDetalle_Ingreso> Detalle, int IdCompra)
        {
            int i = 0;
            string respuesta = "";

            foreach (DDetalle_Ingreso det in Detalle)
            {
                using SqlCommand comm = new SqlCommand(queryInsertDetail, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idDetalleIngreso", LFunction.GetID("detalleIngreso", "idDetalleIngreso"));
                comm.Parameters.AddWithValue("@idIngreso", IdCompra);
                comm.Parameters.AddWithValue("@idArticulo", Detalle[i].idArticulo);
                comm.Parameters.AddWithValue("@precioCompra", Detalle[i].precioCompra);
                comm.Parameters.AddWithValue("@precioVenta", Detalle[i].precioVenta);
                comm.Parameters.AddWithValue("@cantidadInicial", Detalle[i].cantidadInicial);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Registró el Detalle de la Compra";
                if (!respuesta.Equals("OK")) break;

                i++;
            }

            if (respuesta.Equals("OK"))
                LFunction.MessageExecutor("Information", "Compra Registrada Correctamente");
            else
                LFunction.MessageExecutor("Error", respuesta);

            return respuesta;
        }

        private string InsertarCxP(DCuentaPagar CuentaPagar, int IdCompra)
        {
            using SqlCommand comm = new SqlCommand(queryInsertCP, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idCuentaPagar", LFunction.GetID("cuentaPagar", "idCuentaPagar"));
            comm.Parameters.AddWithValue("@idIngreso", IdCompra);
            comm.Parameters.AddWithValue("@fechaInicio", CuentaPagar.fechaInicio);
            comm.Parameters.AddWithValue("@fechaLimite", CuentaPagar.fechaLimite);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta por pagar";
        }


        public string Anular(int IdCompra, List<DDetalle_Ingreso> Detalle)
        {
            string respuesta = "";
            int i = 0, j = 0;
            string fueraStock = "";
            Action action = () =>
            {
                foreach (DDetalle_Ingreso det in Detalle)
                {
                    string fueraStockFunction = ComprobarCantidadDisponibleParaRestock(Detalle[j].idArticulo, Detalle[j].cantidad);

                    if (!fueraStockFunction.Equals(""))
                        fueraStock += fueraStockFunction + Environment.NewLine;

                    j++;
                }

                if(fueraStock != "")
                    throw new Exception("La Compra no puede ser Anulada, existen Artículos sin Disponibilidad para la Devolución. Los cuales son:" + Environment.NewLine + Environment.NewLine + fueraStock);

                foreach (DDetalle_Ingreso det in Detalle)
                {
                    if (!RestockIngreso(Detalle[i].idArticulo, Detalle[i].cantidad).Equals("OK"))
                        throw new Exception("Error en el Actualización del Stock");

                    if (!ActualizarDetalleIngreso(Detalle[i].idDetalleIngreso, Detalle[i].idArticulo).Equals("OK"))
                        throw new Exception("Error en la Actualizacion de los Detalles de la Venta");

                    if (!EliminarDetalleIngreso(Detalle[i].idDetalleIngreso, Detalle[i].idArticulo).Equals("OK"))
                        throw new Exception("Error en al Eliminar el Detalle de la Venta");

                    i++;
                }

                respuesta = AnularIngreso(IdCompra);
                if (respuesta.Equals("OK"))
                    LFunction.MessageExecutor("Information", "La Compra ha sido Anulada, regresando al Listado de Compras");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DIngreso> MostrarCompra(int IdIngreso)
        {
            List<DIngreso> ListaGenerica = new List<DIngreso>();

            string queryListActive = @"
                SELECT 
                    i.idIngreso,
				    i.factura,
                    CONCAT(t.nombre, ' ', t.apellidos) AS nombreTrabajador,
                    CONCAT(p.tipoDocumento, '-', p.numeroDocumento) AS cedulaProveedor,
                    p.razonSocial,
                    p.telefono,
                    p.email,
                    SUM(di.precioCompra * di.cantidadInicial) AS montoTotal,
                    i.impuesto,
                    i.fecha,
                    i.metodoPago,
                    i.estado,
				    ISNULL((
                        SELECT TOP 1 
                            cp.montoIngresado 
                        FROM [cuentaPagar] cp
                        WHERE cp.idIngreso = i.idIngreso
                        ORDER BY i.idIngreso DESC), -1) AS cuentaPagar
                FROM [ingreso] i
                    INNER JOIN [proveedor] p ON i.idProveedor = p.idProveedor
                    INNER JOIN [trabajador] t ON t.idTrabajador = i.idTrabajador
                    INNER JOIN [detalleIngreso] di ON i.idIngreso = di.idIngreso
                WHERE i.idIngreso = @idIngreso AND di.cantidadInicial <> 0
                GROUP BY 
                    i.idIngreso, 
                    i.factura,
                    t.nombre, 
                    t.apellidos, 
                    p.tipoDocumento, 
                    p.numeroDocumento, 
                    p.razonSocial, 
                    p.telefono, 
                    p.email, 
                    i.impuesto,
                    i.fecha,
                    i.metodoPago,
                    i.estado
            ";

        Action action = () =>
                {
                    using SqlCommand comm = new SqlCommand(queryListActive, Conexion.ConexionSql);
                    comm.Parameters.AddWithValue("@idIngreso", IdIngreso);

                    using SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        ListaGenerica.Add(new DIngreso
                        {
                            idIngreso = reader.GetInt32(0),
                            factura = reader.GetString(1),
                            trabajador = reader.GetString(2),
                            cedulaProveedor = reader.GetString(3),
                            razonSocial = reader.GetString(4),
                            telefonoProveedor = reader.GetString(5),
                            emailProveedor = reader.GetString(6),
                            montoTotal = (double)reader.GetDecimal(7),
                            impuesto = reader.GetInt32(8),
                            fechaString = reader.GetDateTime(9).ToShortDateString(),
                            metodoPago = reader.GetInt32(10),
                            metodoPagoString = new LVenta().MetodoPagoToString(reader.GetInt32(10)),
                            estado = reader.GetInt32(11),
                            estadoString = new LVenta().EstadoToString(reader.GetInt32(11)),
                            montoIngresado = (double)reader.GetDecimal(12),
                            nombreTrabajadorIngresado = Globals.TRABAJADOR_SISTEMA
                        });
                    }
                };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DDetalle_Ingreso> MostrarDetalleCompra(int IdIngreso)
        {
            List<DDetalle_Ingreso> ListaGenerica = new List<DDetalle_Ingreso>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListDetail, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idIngreso", IdIngreso);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DDetalle_Ingreso
                    {
                        idDetalleIngreso = reader.GetInt32(0),
                        idIngreso = reader.GetInt32(1),
                        idArticulo = reader.GetInt32(2),
                        codigo = reader.GetString(3),
                        nombre = reader.GetString(4),
                        cantidad = reader.GetInt32(5),
                        precioCompra = (double)reader.GetDecimal(6)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DArticulo> MostrarStockNombre(string Nombre)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            string queryListStockName = @"
                SELECT 
                    a.idArticulo,
				    a.codigo,
				    a.nombre,
				    c.nombre,
				    ISNULL((
                        SELECT TOP 1 
                            di.precioVenta 
                        FROM [detalleIngreso] di 
                        WHERE a.idArticulo = di.idArticulo AND di.estado <> 0
                        ORDER BY di.idDetalleIngreso DESC), 0) AS precioVenta,
				    ISNULL((
                        SELECT TOP 1 
                            di.cantidadActual 
                        FROM [detalleIngreso] di 
                        WHERE a.idArticulo = di.idArticulo AND di.estado <> 0
                        ORDER BY di.idDetalleIngreso DESC), 0) AS cantidad
                FROM [articulo] a  
                    INNER JOIN [categoria] c ON a.idCategoria = c.idCategoria
			    WHERE a.nombre LIKE @nombre + '%'
                    AND a.estado <> 0
                ORDER BY a.nombre;
            ";

            Action action = () =>
                {
                    using SqlCommand comm = new SqlCommand(queryListStockName, Conexion.ConexionSql);
                    comm.Parameters.AddWithValue("@nombre", Nombre);

                    using SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.GetDecimal(4) != 0)
                        {
                            ListaGenerica.Add(new DArticulo
                            {
                                idArticulo = reader.GetInt32(0),
                                codigo = reader.GetString(1),
                                nombre = reader.GetString(2),
                                categoria = reader.GetString(3),
                                precioVenta = (double)reader.GetDecimal(4),
                                precioVentaString = ((double)reader.GetDecimal(4)).ToString() + " Bs S",
                                cantidadActual = reader.GetInt32(5)
                            });
                        }
                    }
                };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DDetalle_Ingreso> EncontrarByArticulo(int IdArticulo)
        {
            List<DDetalle_Ingreso> ListaGenerica = new List<DDetalle_Ingreso>();

            string queryListID = @"
                SELECT 
                    di.idDetalleIngreso, 
                    di.idIngreso, 
                    di.idArticulo, 
                    di.precioCompra, 
                    di.precioVenta, 
                    di.cantidadInicial, 
                    di.cantidadActual
                FROM [detalleIngreso] di
					INNER JOIN [articulo] a ON a.idArticulo = di.idArticulo
                WHERE di.idArticulo = @idArticulo 
					AND a.estado <> 0
                    AND di.estado <> 0
                ORDER BY idDetalleIngreso DESC;
            ";

            Action action = () =>
                {
                    using SqlCommand comm = new SqlCommand(queryListID, Conexion.ConexionSql);
                    comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

                    using SqlDataReader reader = comm.ExecuteReader();
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
                };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }

        public List<DArticulo> EncontrarByArticuloAnulado(string Codigo)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            string queryIDCardRepeatedNull = @"
                    SELECT 
                        a.idArticulo,
                        a.codigo, 
                        a.nombre,
                        c.nombre,
						a.stockMinimo,
						a.stockMaximo,
                        ISNULL((
                            SELECT TOP 1 
                                di.precioVenta 
                            FROM [detalleIngreso] di 
                            WHERE a.idArticulo = di.idArticulo 
                            ORDER BY di.idDetalleIngreso DESC), 0) AS precioVenta,
                        ISNULL((
                            SELECT TOP 1 
                                di.precioCompra
                            FROM [detalleIngreso] di 
                            WHERE a.idArticulo = di.idArticulo 
                            ORDER BY di.idDetalleIngreso DESC), 0) AS precioCompra,
                        a.descripcion,
                        a.estado
					FROM [articulo] a
                        INNER JOIN [categoria] c ON c.idCategoria=a.idCategoria
					WHERE a.codigo = @codigo
                        AND a.estado = 0
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryIDCardRepeatedNull, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@codigo", Codigo);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    ListaGenerica.Add(new DArticulo
                    {
                        idArticulo = reader.GetInt32(0),
                        codigo = reader.GetString(1),
                        nombre = reader.GetString(2),
                        categoria = reader.GetString(3),
                        stockMinimo = reader.GetInt32(4),
                        stockMaximo = reader.GetInt32(5),
                        precioVenta = (double)reader.GetDecimal(6),
                        precioCompra = (double)reader.GetDecimal(7),
                        descripcion = reader.GetString(8),
                        estado = reader.GetInt32(9)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DDetalle_Ingreso> Encontrar(int IdArticulo)
        {
            List<DDetalle_Ingreso> ListaGenerica = new List<DDetalle_Ingreso>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListDetailID, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idDetalleIngreso", IdArticulo);

                using SqlDataReader reader = comm.ExecuteReader();
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }



        public List<DIngreso> MostrarComprasGenerales(DateTime? Fecha, string RazonSocial, int MetodoPago)
        {
            List<DIngreso> ListaGenerica = new List<DIngreso>();

            string queryListGeneral = @"
                SELECT 
                    i.idIngreso,
				    i.factura,
                    CONCAT(p.tipoDocumento, '-', p.numeroDocumento) AS cedulaProveedor,
                    p.razonSocial,
                    SUM(di.precioCompra * di.cantidadInicial) AS montoTotal,
				    i.impuesto,
                    i.fecha,
                    i.metodoPago,
                    i.estado
                FROM [ingreso] i
                    INNER JOIN [proveedor] p ON p.idProveedor = i.idProveedor
                    INNER JOIN [detalleIngreso] di ON i.idIngreso = di.idIngreso
                WHERE i.fecha = @fecha 
                    AND i.idIngreso <> 0
                    AND p.razonSocial LIKE @razonSocial + '%'
                    " + QueryMetodoPago(MetodoPago) + @"
			    GROUP BY 
				    i.idIngreso, 
				    i.factura, 
				    p.tipoDocumento, 
				    p.numeroDocumento, 
				    p.razonSocial, 
				    i.impuesto, 
				    i.fecha, 
				    i.metodoPago, 
				    i.estado;
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListGeneral, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@fecha", Fecha == null ? DateTime.Today : Fecha);
                comm.Parameters.AddWithValue("@razonSocial", RazonSocial);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DIngreso
                    {
                        idIngreso = reader.GetInt32(0),
                        factura = reader.GetString(1),
                        cedulaProveedor = reader.GetString(2),
                        razonSocial = reader.GetString(3),
                        montoTotal = (double)reader.GetDecimal(4),
                        montoTotalString = (double)reader.GetDecimal(4) == 0 ? "S/M" : ((double)reader.GetDecimal(4)).ToString() + " Bs S",
                        impuesto = reader.GetInt32(5),
                        fechaString = reader.GetDateTime(6).ToShortDateString(),
                        metodoPagoString = new LVenta().MetodoPagoToString(reader.GetInt32(7)),
                        estado = reader.GetInt32(8),
                        estadoString = new LVenta().EstadoToString(reader.GetInt32(8)),
                        nombreTrabajadorIngresado = Globals.TRABAJADOR_SISTEMA,
                        accesoTrabajadorIngresado = Globals.ACCESO_SISTEMA
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }

        private string QueryMetodoPago(int MetodoPago)
        {
            if (MetodoPago != 0)
                return " AND i.metodoPago = " + MetodoPago;

            return null;
        }


        public string InsertarDetallePrecios(DDetalle_Ingreso Detalle)
        {
            string respuesta = "";

            //proveedor vacio
            if (new LProveedor().Encontrar(0).Count == 0)
            {
                DProveedor UForm = new DProveedor(0,
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0.ToString());

                new LProveedor().Insertar(UForm, true);
            }
            //trabajador vacio
            if (new LTrabajador().Encontrar(0).Count == 0)
            {
                DTrabajador UForm = new DTrabajador(0,
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                DateTime.Now,
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0.ToString(),
                                0,
                                0.ToString(),
                                0.ToString());

                new LTrabajador().Insertar(UForm, null, true);
            }
            //ingreso vacio
            if (!MostrarIngresoVacio())
            {
                DIngreso UForm = new DIngreso(0,
                                0,
                                0,
                                DateTime.Now,
                                0.ToString(),
                                0,
                                0,
                                0);

                Insertar(UForm, null, null, true);
            }

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryInsertDetail, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idDetalleIngreso", LFunction.GetID("detalleIngreso", "idDetalleIngreso"));
                comm.Parameters.AddWithValue("@idIngreso", 0);
                comm.Parameters.AddWithValue("@idArticulo", Detalle.idArticulo);
                comm.Parameters.AddWithValue("@precioCompra", Detalle.precioCompra);
                comm.Parameters.AddWithValue("@precioVenta", Detalle.precioVenta);
                comm.Parameters.AddWithValue("@cantidadInicial", 0);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizaron los Precios";

                if (!respuesta.Equals("OK"))
                    throw new Exception("Error en el Registro de la Compra");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }

        public bool MostrarIngresoVacio()
        {
            bool respuesta = false;

            string queryList = @"
                SELECT 
                    i.idIngreso
                FROM [ingreso] i
                WHERE i.idIngreso = 0
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read()) respuesta = true;
                else respuesta = false;
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }
    }
}
