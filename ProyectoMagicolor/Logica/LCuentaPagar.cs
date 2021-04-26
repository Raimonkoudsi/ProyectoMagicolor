using System;
using System.Collections.Generic;
using System.Text;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LCuentaPagar : DCuentaPagar
    {
        #region QUERIES

        private string queryList = @"
            SELECT 
                i.idIngreso,
                p.razonSocial, 
                i.factura, 
                i.fecha, 
                ((SUM(di.precioCompra) * di.cantidadInicial) - cp.montoIngresado) AS montoActual,
                t.cedula
            FROM [ingreso] i 
                INNER JOIN [proveedor] p ON i.idProveedor = p.idProveedor 
                INNER JOIN [trabajador] t ON i.idTrabajador = t.idTrabajador 
                INNER JOIN [detalleIngreso] di ON i.idIngreso = di.idIngreso 
                INNER JOIN [cuentaPagar] cp ON i.idIngreso = cp.idIngreso 
            WHERE i.estado = 2 AND p.razonSocial LIKE @razonSocial + '%' 
            GROUP BY 
                i.idIngreso, 
                p.razonSocial, 
                i.factura, 
                i.fecha, 
                t.cedula, 
                cp.montoIngresado, 
                di.cantidadInicial;
        ";

        private string queryInsert = @"
            INSERT INTO [registroCuentaPagar] (
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

        private string queryUpdate = @"
            UPDATE [cuentaPagar] SET
                cuentaPagar.montoIngresado = cuentaPagar.montoIngresado + @monto
            FROM [cuentaPagar]
            INNER JOIN (
				SELECT ingreso.idIngreso 
				FROM [ingreso] 
				inner join [cuentaPagar] cp on ingreso.idIngreso=cp.idIngreso
				inner join [detalleIngreso] di on ingreso.idIngreso=di.idIngreso
				WHERE ingreso.idIngreso = cp.idIngreso AND ingreso.estado = 2
				GROUP BY ingreso.idIngreso , cp.montoIngresado, di.cantidadInicial, di.precioCompra
				HAVING ((di.precioCompra*di.cantidadInicial) - cp.montoIngresado) >= 0
			) X
            ON x.idIngreso = cuentaPagar.idIngreso
            WHERE cuentaPagar.idCuentaPagar = @idCuentaPagar;
        ";

        private string queryComplete = @"
            UPDATE ingreso SET ingreso.estado = 1
            FROM [ingreso]
            INNER JOIN [cuentaPagar] cp on ingreso.idIngreso=cp.idIngreso
			INNER JOIN (
				SELECT ingreso.idIngreso 
				FROM ingreso 
				INNER JOIN [cuentaPagar] cp ON ingreso.idIngreso=cp.idIngreso
				INNER JOIN [detalleIngreso] di ON ingreso.idIngreso=di.idIngreso
				WHERE ingreso.idIngreso = cp.idIngreso AND ingreso.estado = 2
				GROUP BY ingreso.idIngreso , cp.montoIngresado, di.cantidadInicial, di.precioCompra
				HAVING ((di.precioCompra*di.cantidadInicial) - cp.montoIngresado) = 0
			) X
			ON x.idIngreso=cp.idIngreso
			WHERE cp.idCuentaPagar = @idCuentaPagar;
        ";

        private string queryListCP = @"
            SELECT 
                i.idIngreso,
                cp.idCuentaPagar,
                p.razonSocial, 
                i.factura, 
                i.fecha, 
                (SUM(di.precioCompra) * di.cantidadInicial) AS montoTotal,
                (SUM(di.precioCompra) * di.cantidadInicial - SUM(cp.montoIngresado)) AS monto
            FROM [ingreso] i 
                INNER JOIN [proveedor] p ON i.idProveedor=p.idProveedor 
                INNER JOIN [trabajador] t ON i.idTrabajador=t.idTrabajador 
                INNER JOIN [detalleIngreso] di ON i.idIngreso=di.idIngreso 
                INNER JOIN [cuentaPagar] cp ON i.idIngreso=cp.idIngreso 
            WHERE i.estado = 2 AND i.idIngreso = @idIngreso 
            GROUP BY 
                i.idIngreso,
                di.cantidadInicial, 
                cp.idCuentaPagar, 
                p.razonSocial, 
                i.factura, 
                i.fecha 
            ORDER BY cp.idCuentaPagar ASC;
        ";
        #endregion


        public List<DIngreso> MostrarCxP(string RazonSocial)
        {
            List<DIngreso> ListaGenerica = new List<DIngreso>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@razonSocial", RazonSocial);

                using SqlDataReader reader = comm.ExecuteReader();
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public string RegistrarCxP(DRegistro_CuentaPagar RegistroCuentaPagar, int IdCuentaPagar)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idRegistro", LFunction.GetID("registroCuentaPagar", "idRegistro"));
                comm.Parameters.AddWithValue("@idCuentaPagar", RegistroCuentaPagar.idCuentaPagar);
                comm.Parameters.AddWithValue("@monto", RegistroCuentaPagar.monto);
                comm.Parameters.AddWithValue("@fecha", DateTime.Now);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro de la Cuenta por Pagar";
                if (!respuesta.Equals("OK"))
                    throw new Exception("Error en el Registro de la Cuenta por Pagar");

                respuesta = ActualizarCxP(RegistroCuentaPagar.monto, IdCuentaPagar);
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }

        private string ActualizarCxP(double Monto, int IdCuentaPagar)
        {
            using SqlCommand comm = new SqlCommand(queryUpdate, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@monto", Monto);
            comm.Parameters.AddWithValue("@idCuentaPagar", IdCuentaPagar);

            if (comm.ExecuteNonQuery() != 1)
                throw new Exception("Error al Actualizar Cuenta por Pagar");

            return CompletarCxP(IdCuentaPagar);
        }

        private string CompletarCxP(int IdCuentaPagar)
        {
            using SqlCommand comm = new SqlCommand(queryComplete, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idCuentaPagar", IdCuentaPagar);

            return comm.ExecuteNonQuery() == 1 ? "TOTAL" : "PARCIAL";
        }


        public List<DIngreso> EncontrarCxP(int IdIngreso)
        {
            List<DIngreso> ListaGenerica = new List<DIngreso>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListCP, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idIngreso", IdIngreso);

                using SqlDataReader reader = comm.ExecuteReader();
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }
    }
}
