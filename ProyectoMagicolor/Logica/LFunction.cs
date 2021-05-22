using System;
using System.Collections.Generic;
using System.Text;

using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

using System.Reflection;

namespace Logica
{
    public class LFunction : Conexion
    {
        public static int GetID(string Table, string Parameter)
        {
            int ID = 1;

            string queryGetID = "SELECT max(" + Parameter + ") FROM " + Table;
            using SqlCommand comm = new SqlCommand(queryGetID, Conexion.ConexionSql);

            using SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read() && !reader.IsDBNull(0)) ID = reader.GetInt32(0) + 1;

            return ID;
        }


        public static MessageBoxResult MessageExecutor(string TypeError, string Message)
        {
            if (TypeError == null && Message == null)
                throw new NullReferenceException("Error en el mensaje de Información");
            if (TypeError == "Error")
                return MessageBox.Show(Message, "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
            if (TypeError == "Information")
                return MessageBox.Show(Message, "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
            if (TypeError == "Warning")
                return MessageBox.Show(Message, "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Warning);

            return MessageBox.Show(Message, "Variedades Magicolor", MessageBoxButton.OK);
        }

        public static void SafeExecutor(Action action)
        {
            SafeExecutor(() => { 
                action(); 
                return 0; 
            });
        }

        private static T SafeExecutor<T>(Func<T> action)
        {
            try
            {
                Conexion.ConexionSql.Open();
                return action();
            }
            catch (SqlException errorSql) { 
                MessageExecutor("Error", errorSql.Message); 
            }
            catch (IndexOutOfRangeException errorIndex) { 
                MessageExecutor("Error", errorIndex.Message); 
            }
            catch (ArgumentNullException errorNull) { 
                MessageExecutor("Error", errorNull.Message); 
            }
            catch (Exception error) { 
                MessageExecutor("Error", error.Message); 
            }
            finally { 
                if (Conexion.ConexionSql.State == ConnectionState.Open) 
                    Conexion.ConexionSql.Close(); 
            }

            return default(T);
        }



        public static void Backup(string Path)
        {
            Action action = () =>
            {
                string database = Conexion.ConexionSql.Database.ToString();

                if (Path == string.Empty)
                    LFunction.MessageExecutor("Error", "Necesita una Ruta de Almacenamiento");
                else
                {
                    string cmd = "BACKUP DATABASE [" + database + "] TO DISK='" + Path + "\\" + "dbMagicolor" + "-" + DateTime.Now.ToString("yyyy-MM-dd") + ".bak'";

                    using (SqlCommand command = new SqlCommand(cmd, Conexion.ConexionSql))
                    {
                        command.ExecuteNonQuery();
                        LFunction.MessageExecutor("Information", "Backup Almacenado Correctamente");
                    }
                }
            };
            LFunction.SafeExecutor(action);
        }

        public static void Restore(string Path)
        {
            Action action = () =>
            {
                string database = Conexion.ConexionSql.Database.ToString();

                if (Path == string.Empty)
                    LFunction.MessageExecutor("Error", "Necesita Seleccionar un Respaldo");
                else
                {
                    string sqlStmt2 = string.Format("ALTER DATABASE [" + database + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                    SqlCommand bu2 = new SqlCommand(sqlStmt2, Conexion.ConexionSql);
                    bu2.ExecuteNonQuery();

                    string sqlStmt3 = "USE MASTER RESTORE DATABASE [" + database + "] FROM DISK='" + Path + "'WITH REPLACE;";
                    SqlCommand bu3 = new SqlCommand(sqlStmt3, Conexion.ConexionSql);
                    bu3.ExecuteNonQuery();

                    string sqlStmt4 = string.Format("ALTER DATABASE [" + database + "] SET MULTI_USER");
                    SqlCommand bu4 = new SqlCommand(sqlStmt4, Conexion.ConexionSql);
                    bu4.ExecuteNonQuery();

                    LFunction.MessageExecutor("Information", "Bas de Datos Restaurada Correctamente");
                }
            };
            LFunction.SafeExecutor(action);
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
