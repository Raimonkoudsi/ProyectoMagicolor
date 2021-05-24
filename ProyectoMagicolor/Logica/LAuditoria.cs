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

        private string queryList = @"
            SELECT
				a.idAuditoria,
				a.fecha,
				a.accion,
				a.descripcion,
				t.usuario,
				t.acceso
			FROM [auditoria] a
				INNER JOIN [trabajador] t ON t.idTrabajador=a.idTrabajador
            WHERE a.fecha = @fecha 
				AND t.usuario LIKE @usuario + '%' 
				AND a.accion LIKE @accion + '%'
            ORDER BY a.idAuditoria DESC;
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


        public List<DAuditoria> Mostrar(DateTime? Fecha, string Usuario, string Accion)
        {
            List<DAuditoria> ListaGenerica = new List<DAuditoria>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@fecha", Fecha == null ? DateTime.Now : Fecha);
                comm.Parameters.AddWithValue("@usuario", Usuario);
                comm.Parameters.AddWithValue("@accion", Accion);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DAuditoria
                    {
                        idAuditoria = reader.GetInt32(0),
                        fecha = reader.GetDateTime(1),
                        accion = reader.GetString(2),
                        descripcion = reader.GetString(3),
                        usuario = reader.GetString(4),
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
