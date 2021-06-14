using System;
using System.Collections.Generic;
using System.Text;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LAuditoria : DAuditoria
    {
        #region QUERIES
        private string queryInsert = @"
            INSERT INTO [auditoria] (
                idAuditoria,
                idTrabajador,
                accion,
                descripcion,
                fecha
            ) VALUES (
                @idAuditoria,
                @idTrabajador,
                @accion,
                @descripcion,
                @fecha
            );
	    ";

        #endregion


        public string Insertar(DAuditoria Auditoria)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idAuditoria", LFunction.GetID("auditoria", "idAuditoria"));
                comm.Parameters.AddWithValue("@idTrabajador", Auditoria.idTrabajador);
                comm.Parameters.AddWithValue("@accion", Auditoria.accion);
                comm.Parameters.AddWithValue("@descripcion", Auditoria.descripcion);
                comm.Parameters.AddWithValue("@fecha", DateTime.Now);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "Error en el proceso de Auditoria";
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DAuditoria> Mostrar(DateTime? Fecha, DateTime? Fecha2, string Accion, string Usuario)
        {
            List<DAuditoria> ListaGenerica = new List<DAuditoria>();

            DateTime? fecha = Fecha.HasValue ? Fecha : DateTime.Now;
            DateTime? fecha2 = Fecha2.HasValue ? Fecha2 : fecha.Value.AddDays(1).AddTicks(-1);

            string stringUsuario = Usuario == "" ? "" :
                                   Usuario == "Todos los Usuarios" ? "" :
                                   " AND t.usuario = '" + Encripter.Encrypt(Usuario) + "'";

            string queryList = @"
                SELECT
				    a.idAuditoria,
				    a.fecha,
				    a.accion,
				    a.descripcion,
				    t.usuario,
				    t.acceso
			    FROM [auditoria] a
				    INNER JOIN [trabajador] t ON t.idTrabajador=a.idTrabajador
                WHERE a.accion LIKE @accion + '%'
                    AND a.fecha BETWEEN @desde AND @hasta
                    " + stringUsuario + @"
                ORDER BY a.fecha DESC;
            ";

            Action action = () =>
                {

                    using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                    comm.Parameters.AddWithValue("@desde", fecha);
                    comm.Parameters.AddWithValue("@hasta", fecha2.Value.AddDays(1).AddTicks(-1));
                    comm.Parameters.AddWithValue("@accion", Accion);

                    using SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        ListaGenerica.Add(new DAuditoria
                        {
                            idAuditoria = reader.GetInt32(0),
                            fechaString = reader.GetDateTime(1).TimeOfDay.ToString("dd/MM/yyyy"),
                            fechaTime = reader.GetDateTime(1).TimeOfDay.ToString(@"hh\:mm\:ss"),
                            accion = reader.GetString(2),
                            descripcion = reader.GetString(3),
                            usuario = Encripter.Decrypt(reader.GetString(4)),
                            accesoString = AccesoString(reader.GetInt32(5))
                        });
                    }
                };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }

        private string AccesoString(int Acceso)
        {
            if (Acceso == 0)
                return "Administrador";
            if (Acceso == 1)
                return "Encargado";
            if (Acceso == 2)
                return "Vendedor";

            return null;
        }
    }
}
