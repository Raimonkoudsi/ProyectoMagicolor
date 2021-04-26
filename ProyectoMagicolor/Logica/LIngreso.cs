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
                cantidadActual
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
                ),0))
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

        private string queryList = @"
            SELECT 
                i.idIngreso, 
                t.cedula, 
                p.razonSocial,
                i.fecha, 
                i.factura, 
                i.metodoPago, 
                i.estado, 
                SUM(di.precioCompra) as precioTotal 
            FROM [ingreso] i 
                INNER JOIN [proveedor] p ON i.idProveedor = p.idProveedor 
                INNER JOIN [trabajador] t ON i.idTrabajador = t.idTrabajador 
                INNER JOIN [detalleIngreso] di ON i.idIngreso = di.idIngreso 
            WHERE tipoComprobante = @tipoComprobante AND serieComprobante LIKE @serieComprobante + '%' 
            ORDER BY serieComprobante";

        private string queryListStockName = @"
            SELECT 
                a.idArticulo,
				a.codigo,
				a.nombre,
				c.nombre,
				ISNULL((
                    SELECT TOP 1 
                        di.precioVenta 
                    FROM [detalleIngreso] di 
                    WHERE a.idArticulo = di.idArticulo 
                    ORDER BY di.idDetalleIngreso DESC), 0) AS PrecioVenta,
				ISNULL((
                    SELECT TOP 1 
                        di.cantidadActual 
                    FROM [detalleIngreso] di 
                    WHERE a.idArticulo = di.idArticulo 
                    ORDER BY di.idDetalleIngreso DESC), 0) AS cantidad
            FROM [articulo] a  
                INNER JOIN [categoria] c ON a.idCategoria = c.idCategoria
			WHERE a.nombre LIKE @nombre + '%'
            ORDER BY a.nombre;
        ";

        private string queryListID = @"
            SELECT 
                idDetalleIngreso, 
                idIngreso, 
                idArticulo, 
                precioCompra, 
                precioVenta, 
                cantidadInicial, 
                cantidadActual 
            FROM [detalleIngreso] 
            where idArticulo = @idArticulo AND cantidadActual > 0 
            ORDER BY idDetalleIngreso DESC;
        ";

        private string queryListDetail = @"
            SELECT * FROM [detalleIngreso] 
            WHERE idDetalleIngreso = @idDetalleIngreso 
            ORDER BY idDetalleIngreso DESC
        ";

        #endregion


        public string Insertar(DIngreso Ingreso, List<DDetalle_Ingreso> Detalle, DCuentaPagar CuentaPagar)
        {
            string respuesta = "";

            Action action = () =>
            {
                int IDCompra = LFunction.GetID("ingreso", "idIngreso");

                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idIngreso", IDCompra);
                comm.Parameters.AddWithValue("@idTrabajador", Ingreso.idTrabajador);
                comm.Parameters.AddWithValue("@idProveedor", Ingreso.idProveedor);
                comm.Parameters.AddWithValue("@fecha", Ingreso.fecha);
                comm.Parameters.AddWithValue("@factura", Ingreso.factura);
                comm.Parameters.AddWithValue("@impuesto", Ingreso.impuesto);
                comm.Parameters.AddWithValue("@metodoPago", Ingreso.metodoPago);
                comm.Parameters.AddWithValue("@estado", Ingreso.metodoPago);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Registró la Compra";

                if (!respuesta.Equals("OK"))
                    throw new Exception("Error en el Registro de la Compra");

                if (Ingreso.metodoPago == 2)
                    if(!InsertarCxP(CuentaPagar, IDCompra).Equals("OK"))
                        throw new Exception("Error en el Registro de la Cuenta a Pagar");

                respuesta = InsertarDetalle(Detalle, IDCompra);
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


        public List<DIngreso> Mostrar(string TipoComprobante, string SerieComprobante)
        {
            List<DIngreso> ListaGenerica = new List<DIngreso>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@tipoComprobante", TipoComprobante);
                comm.Parameters.AddWithValue("@serieComprobante", SerieComprobante);

                using SqlDataReader reader = comm.ExecuteReader();
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DArticulo> MostrarStockNombre(string Nombre)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListStockName, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@nombre", Nombre);

                using SqlDataReader reader = comm.ExecuteReader();
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DDetalle_Ingreso> EncontrarByArticulo(int IdArticulo)
        {
            List<DDetalle_Ingreso> ListaGenerica = new List<DDetalle_Ingreso>();

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


        public List<DDetalle_Ingreso> Encontrar(int IdArticulo)
        {
            List<DDetalle_Ingreso> ListaGenerica = new List<DDetalle_Ingreso>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListDetail, Conexion.ConexionSql);
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
    }
}
