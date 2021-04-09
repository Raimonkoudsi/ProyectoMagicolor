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
    public class LFunction:Conexion
    {
        public int GetID(string Table, string Parameter)
        {
            int ID = 1;

            string queryGetID = "SELECT max(" + Parameter + ") FROM " + Table;

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {
                using (SqlCommand comm = new SqlCommand(queryGetID, conn))
                {
                    try
                    {
                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                ID = reader.GetInt32(0);
                                ID++;
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

                    return ID;
                }
            }
        }

        /*
        public bool Insert(string Table, DArticulo Articulo, int CountValue)
        {
            bool respuesta = false;

            string finalAddQuery = "INSERT INTO " + Table + " VALUES (";

            PropertyInfo[] properties = Articulo.GetType().GetProperties();

            int iteracion=0;

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand comm = new SqlCommand(finalAddQuery, conn))
                    {


                        for (int i = 0; i <= (CountValue - 1); i++)
                        {
                            finalAddQuery += properties[i].GetValue(Articulo, null).ToString();


                            if (iteracion < (CountValue - 1))
                            {
                                finalAddQuery += ",";
                            }
                            else
                            {
                                finalAddQuery += ");";
                            }

                            comm.Parameters.AddWithValue("@" + properties[i].Name, 
                                                        properties[i].GetValue(Articulo, null));

                            iteracion++;
                        }

                        respuesta = comm.ExecuteNonQuery() == 1 ? true : false;

                        MessageBox.Show(finalAddQuery, "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);

                        if (respuesta)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                }
                catch (SqlException e)
                {
                    MessageBox.Show(e.Message, "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);

                    return false;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            //foreach (PropertyInfo pi in properties)
            //{
            //    //MessageBox.Show(pi.Name, "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
            //    ///MessageBox.Show(pi.GetValue(Articulo, null).ToString(), "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
            //    finalAddQuery += pi.GetValue(Articulo, null).ToString();


            //    if(iteracion < properties.Length)
            //    {
            //        finalAddQuery += ",";
            //    } 
            //    else
            //    {
            //        finalAddQuery += ");";
            //    }

            //    iteracion++;
            //}
        }
        */

        
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
