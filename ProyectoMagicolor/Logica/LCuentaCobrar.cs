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
        #region QUERIES

        private string queryList = @"
            SELECT 
                v.idVenta,
                c.tipoDocumento + '-' + c.numeroDocumento,
                c.apellidos + ' ' + c.nombre, 
                v.fecha, 
                (select (sum(precioVenta * cantidad) - cc.montoIngresado - v.descuento) from detalleVenta where idVenta = v.idVenta)
            FROM [cuentaCobrar] cc
                INNER JOIN [venta] v ON v.idVenta=cc.idVenta 
                INNER JOIN [cliente] c ON v.idCliente=c.idCliente 
                INNER JOIN [trabajador] t ON v.idTrabajador=t.idTrabajador 
            WHERE v.estado = 2 AND c.tipoDocumento LIKE @tipoDocumento + '%' AND c.numeroDocumento LIKE @numeroDocumento + '%' 
        ";

        private string queryInsert = @"
            INSERT INTO [registroCuentaCobrar] (
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

        private string queryUpdate = @"
            UPDATE [cuentaCobrar] SET
                cuentaCobrar.montoIngresado = cuentaCobrar.montoIngresado + @monto
            FROM [cuentaCobrar]
            INNER JOIN (
	            SELECT v.idVenta 
	            FROM [venta] v
		            inner join [cuentaCobrar] cc on v.idVenta=cc.idVenta
                    inner join [detalleVenta] dv on dv.idVenta=v.idVenta
		            inner join [detalleIngreso] di on di.idDetalleIngreso=dv.idDetalleIngreso
	            WHERE v.idVenta = cc.idVenta AND v.estado = 2
	            GROUP BY v.idVenta , cc.montoIngresado, dv.cantidad
	            HAVING ((SUM(dv.precioVenta)*dv.cantidad) - (cc.montoIngresado + @monto)) >= 0
            ) X
            ON x.idVenta = cuentaCobrar.idVenta
            WHERE cuentaCobrar.idCuentaCobrar = @idCuentaCobrar;
            ";

        private string queryComplete = @"
            UPDATE [venta] SET venta.estado = 1
            FROM [venta]
            INNER JOIN [cuentaCobrar] cc ON venta.idVenta=cc.idVenta
            INNER JOIN (
            	SELECT v.idVenta 
            	FROM [venta] v
            	INNER JOIN [cuentaCobrar] cc ON v.idVenta=cc.idVenta
            	INNER JOIN [detalleVenta] dv ON v.idVenta=dv.idVenta
            	WHERE v.idVenta = cc.idVenta AND v.estado = 2
            	GROUP BY v.idVenta , cc.montoIngresado, dv.cantidad, v.descuento, dv.precioVenta
            	HAVING ((dv.precioVenta * dv.cantidad) - cc.montoIngresado - v.descuento) = 0
            ) X
            ON x.idVenta=cc.idVenta
            WHERE cc.idCuentaCobrar = @idCuentaCobrar
        ";

        private string queryListID = @"
            SELECT
                v.idVenta,
                cc.idCuentaCobrar,
                c.tipoDocumento + '-' + c.numeroDocumento,
                c.apellidos + ' ' + c.nombre,
                v.fecha,
                ((SUM(dv.precioVenta) * dv.cantidad) - v.descuento) AS montoTotal,
                (SUM(dv.precioVenta) * dv.cantidad - cc.montoIngresado - v.descuento) AS monto
            FROM [venta] v
                INNER JOIN [cliente] c ON v.idCliente = c.idCliente
                INNER JOIN [detalleVenta] dv ON v.idVenta = dv.idVenta
                INNER JOIN [cuentaCobrar] cc ON v.idVenta = cc.idVenta
            WHERE v.estado = 2 AND v.idVenta = @idVenta 
            GROUP BY 
                v.idVenta, 
                dv.cantidad, 
                cc.idCuentaCobrar, 
                cc.montoIngresado, 
                c.tipoDocumento, 
                c.numeroDocumento, 
                c.apellidos, 
                c.nombre, 
                v.fecha, 
                v.descuento 
            ORDER BY cc.idCuentaCobrar ASC;
        ";

        private string queryListCC = @"
            SELECT * FROM [cuentaCobrar] 
            where idVenta = @idVenta
        ";

        #endregion

        public List<DVenta> MostrarCxC(string TipoDocumento, string NumeroDocumento)
        {
            List<DVenta> ListaGenerica = new List<DVenta>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@tipoDocumento", TipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", NumeroDocumento);

                using SqlDataReader reader = comm.ExecuteReader();
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public string RegistrarCxC(DRegistro_CuentaCobrar RegistroCuentaCobrar, int IdCuentaCobrar)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idRegistro", LFunction.GetID("registroCuentaCobrar", "idRegistro"));
                comm.Parameters.AddWithValue("@idCuentaCobrar", RegistroCuentaCobrar.idCuentaCobrar);
                comm.Parameters.AddWithValue("@monto", RegistroCuentaCobrar.monto);
                comm.Parameters.AddWithValue("@fecha", DateTime.Now);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro de la cuenta por cobrar";
                if (!respuesta.Equals("OK"))
                    throw new Exception("Error en el Registro de la Cuenta a Cobrar");

                respuesta = ActualizarCxC(RegistroCuentaCobrar.monto, IdCuentaCobrar);
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }

        private string ActualizarCxC(double Monto, int IdCuentaCobrar)
        {
            using SqlCommand comm = new SqlCommand(queryUpdate, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@monto", Monto);
            comm.Parameters.AddWithValue("@idCuentaCobrar", IdCuentaCobrar);

            if(comm.ExecuteNonQuery() != 1) 
                throw new Exception("Error al Actualizar Cuenta por Cobrar");

            return CompletarCxC(IdCuentaCobrar);
        }

        private string CompletarCxC(int IdCuentaCobrar)
        {
            using SqlCommand comm = new SqlCommand(queryComplete, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idCuentaCobrar", IdCuentaCobrar);

            return comm.ExecuteNonQuery() == 1 ? "TOTAL" : "PARCIAL";
        }


        public List<DVenta> EncontrarCxC(int IdVenta)
        {
            List<DVenta> ListaGenerica = new List<DVenta>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListID, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idVenta", IdVenta);

                using SqlDataReader reader = comm.ExecuteReader();
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DCuentaCobrar> BuscarCxC(int IdVenta)
        {
            List<DCuentaCobrar> ListaGenerica = new List<DCuentaCobrar>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListCC, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idVenta", IdVenta);

                using SqlDataReader reader = comm.ExecuteReader();
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }
    }
}
