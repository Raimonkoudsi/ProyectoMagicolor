using System;
using System.Collections.Generic;
using System.Text;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Security.Cryptography;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace Logica
{
    public static class Globals
    {
        public static Int32 ACCESO_SISTEMA = 3;
        public static Int32 ID_SISTEMA = 0;
        public static String TRABAJADOR_SISTEMA = "";
        public static String USUARIO_SISTEMA = "";
    }

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

        public static string EditarIVA(int IVA)
        {
            string respuesta = "";

            string queryUpdate = @"
                UPDATE [configuracion] SET 
                    iva = @iva
	        ";

            Action action = () =>
                {
                    using SqlCommand comm = new SqlCommand(queryUpdate, Conexion.ConexionSql);
                    comm.Parameters.AddWithValue("@iva", IVA);

                    respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el IVA";
                };
            LFunction.SafeExecutor(action);

            return respuesta;
        }

        public static int MostrarIVA()
        {
            int IVA = 0;

            string queryListID = @"
                SELECT iva FROM [configuracion] 
            ";

            Action action = () =>
                {
                    using SqlCommand comm = new SqlCommand(queryListID, Conexion.ConexionSql);

                    using SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        IVA = reader.GetInt32(0);
                    };
                };
            LFunction.SafeExecutor(action);

            return IVA;
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

        public async static void SafeExecutor(Action action)
        {
            await SafeExecutor(() =>
            {
                action();
                return 0;
            });
        }

        private async static Task<T> SafeExecutor<T>(Func<T> action)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                Conexion.ConexionSql.OpenAsync(token).Wait();

                return action();
            }
            catch (SqlException errorSql)
            {
                MessageExecutor("Error", errorSql.Message);
            }
            catch (IndexOutOfRangeException errorIndex)
            {
                MessageExecutor("Error", errorIndex.Message);
            }
            catch (ArgumentNullException errorNull)
            {
                MessageExecutor("Error", errorNull.Message);
            }
            catch (Exception error)
            {
                MessageExecutor("Error", error.Message);
            }
            finally
            {
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

    public class Encripter
    {
        // set permutations
        public const String strPermutation = "ouiveyxaqtd";
        public const Int32 bytePermutation1 = 0x19;
        public const Int32 bytePermutation2 = 0x59;
        public const Int32 bytePermutation3 = 0x17;
        public const Int32 bytePermutation4 = 0x41;

        // encoding
        public static string Encrypt(string strData)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(strData)));
        }


        // decoding
        public static string Decrypt(string strData)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(strData)));
        }

        // encrypt
        public static byte[] Encrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(Encripter.strPermutation,
            new byte[] { Encripter.bytePermutation1,
                         Encripter.bytePermutation2,
                         Encripter.bytePermutation3,
                         Encripter.bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

        // decrypt
        public static byte[] Decrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(Encripter.strPermutation,
            new byte[] { Encripter.bytePermutation1,
                         Encripter.bytePermutation2,
                         Encripter.bytePermutation3,
                         Encripter.bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }
    }



    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
    }
}
