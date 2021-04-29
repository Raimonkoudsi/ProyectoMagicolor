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
