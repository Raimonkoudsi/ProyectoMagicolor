using System;
using System.Collections.Generic;
using System.Text;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LTrabajador:DTrabajador
    {

        #region QUERIES
        private string queryInsert = @"
            INSERT INTO [trabajador] (
                idTrabajador,
                nombre,
                apellidos,
                sexo,
                fechaNacimiento,
                cedula,
                direccion,
                telefono,
                email,
                acceso,
                usuario,
                contraseña,
                estado
            ) VALUES (
                @idTrabajador,
                @nombre,
                @apellidos,
                @sexo,
                @fechaNacimiento,
                @cedula,
                @direccion,
                @telefono,
                @email,
                @acceso,
                @usuario,
                @contraseña,
                1
            );
        ";

        private string queryInsertSecurity = @"
            INSERT INTO [seguridad] (
                idSeguridad,
                idTrabajador,
                pregunta,
                respuesta
            ) VALUES (
                @idSeguridad,
                @idTrabajador,
                @pregunta,
                @respuesta
            );
        ";

        private string queryDeleteSecurity = @"
            DELETE FROM [seguridad]
            WHERE idTrabajador = @idTrabajador
	    ";

        string queryUpdate = @"
            UPDATE [trabajador] SET
                nombre = @nombre,
                apellidos = @apellidos,
                sexo = @sexo,
                fechaNacimiento = @fechaNacimiento,
                cedula = @cedula,
                direccion = @direccion,
                telefono = @telefono,
                email = @email,
                acceso = @acceso,
                usuario = @usuario,
                contraseña = @contraseña
            WHERE idTrabajador = @idTrabajador;
	    ";

        private string queryDelete = @"
            DELETE FROM [trabajador]
            WHERE idTrabajador = @idTrabajador
	    ";

        private string queryListIDCard = @"
            SELECT 
                idTrabajador, 
                cedula, 
                nombre, 
                apellidos, 
                direccion, 
                telefono, 
                email, 
                usuario 
            FROM [trabajador] 
            WHERE cedula LIKE @cedula + '%' 
            ORDER BY cedula
        ";

        private string queryListUser = @"
            SELECT 
                cedula, 
                nombre, 
                apellidos, 
                direccion, 
                telefono, 
                email, 
                usuario 
            FROM [trabajador] 
            WHERE usuario LIKE @usuario + '%' 
            ORDER BY usuario
        ";

        private string queryListEmployee = @"
            SELECT * FROM [trabajador] 
            WHERE idTrabajador = @idTrabajador;
        ";

        private string queryListSecurity = @"
            SELECT * FROM [seguridad] 
            WHERE idTrabajador = @idTrabajador;
        ";

        private string queryLogin = @"
            SELECT * FROM [trabajador] 
            WHERE usuario = @usuario AND contraseña = @contraseña;
        ";

        private string queryUserRepeated = @"
            SELECT idTrabajador FROM [trabajador]
            WHERE usuario = @usuario;
        ";

        private string queryIDCardRepeated = @"
            SELECT idTrabajador FROM [trabajador] 
            WHERE cedula = @cedula;
        ";


        #endregion

        public string Insertar(DTrabajador Trabajador, List<DSeguridad> Seguridad)
        {
            string respuesta = "";

            Action action = () =>
            {
                int idTrabajador = LFunction.GetID("trabajador", "idTrabajador");

                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idTrabajador", idTrabajador);
                comm.Parameters.AddWithValue("@nombre", Trabajador.nombre);
                comm.Parameters.AddWithValue("@apellidos", Trabajador.apellidos);
                comm.Parameters.AddWithValue("@sexo", Trabajador.sexo);
                comm.Parameters.AddWithValue("@fechaNacimiento", Trabajador.fechaNacimiento);
                comm.Parameters.AddWithValue("@cedula", Trabajador.cedula);
                comm.Parameters.AddWithValue("@direccion", Trabajador.direccion);
                comm.Parameters.AddWithValue("@telefono", Trabajador.telefono);
                comm.Parameters.AddWithValue("@email", Trabajador.email);
                comm.Parameters.AddWithValue("@acceso", Trabajador.acceso);
                comm.Parameters.AddWithValue("@usuario", Trabajador.usuario);
                comm.Parameters.AddWithValue("@contraseña", Trabajador.contraseña);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Trabajador";
                if (respuesta.Equals("OK"))
                    respuesta = InsertarSeguridad(Seguridad, idTrabajador);
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        private string InsertarSeguridad(List<DSeguridad> Detalle, int IdTrabajador)
        {
            int i = 0;
            string respuesta = "";

            foreach (DSeguridad det in Detalle)
            {
                using SqlCommand comm = new SqlCommand(queryInsertSecurity, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idSeguridad", LFunction.GetID("seguridad", "idSeguridad"));
                comm.Parameters.AddWithValue("@idTrabajador", IdTrabajador);
                comm.Parameters.AddWithValue("@pregunta", Detalle[i].pregunta);
                comm.Parameters.AddWithValue("@respuesta", Detalle[i].respuesta);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro de la Seguridad";
                if (!respuesta.Equals("OK")) break;

                i++;
            }

            return respuesta;
        }

        private string BorrarSeguridad(DTrabajador Trabajador, List<DSeguridad> Seguridad)
        {
            using SqlCommand comm = new SqlCommand(queryDeleteSecurity, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idTrabajador", Trabajador.idTrabajador);

            string respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "OK";

            if (respuesta.Equals("OK"))
                InsertarSeguridad(Seguridad, Trabajador.idTrabajador);

            return respuesta;
        }

        public string Editar(DTrabajador Trabajador, List<DSeguridad> Seguridad)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryUpdate, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@nombre", Trabajador.nombre);
                comm.Parameters.AddWithValue("@apellidos", Trabajador.apellidos);
                comm.Parameters.AddWithValue("@sexo", Trabajador.sexo);
                comm.Parameters.AddWithValue("@fechaNacimiento", Trabajador.fechaNacimiento);
                comm.Parameters.AddWithValue("@cedula", Trabajador.cedula);
                comm.Parameters.AddWithValue("@direccion", Trabajador.direccion);
                comm.Parameters.AddWithValue("@telefono", Trabajador.telefono);
                comm.Parameters.AddWithValue("@email", Trabajador.email);
                comm.Parameters.AddWithValue("@acceso", Trabajador.acceso);
                comm.Parameters.AddWithValue("@usuario", Trabajador.usuario);
                comm.Parameters.AddWithValue("@contraseña", Trabajador.contraseña);
                comm.Parameters.AddWithValue("@idTrabajador", Trabajador.idTrabajador);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se actualizo el Registro del Trabajador";
                if (respuesta.Equals("OK"))
                    BorrarSeguridad(Trabajador, Seguridad);
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public string Eliminar(int IdTrabajador)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryDelete, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idTrabajador", IdTrabajador);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Eliminó el Trabajador";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Trabajador Eliminado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DTrabajador> Mostrar(string Cedula)
        {
            List<DTrabajador> ListaGenerica = new List<DTrabajador>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListIDCard, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@cedula", Cedula);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DTrabajador
                    {
                        idTrabajador = reader.GetInt32(0),
                        cedula = reader.GetString(1),
                        nombre = reader.GetString(2),
                        apellidos = reader.GetString(3),
                        direccion = reader.GetString(4),
                        telefono = reader.GetString(5),
                        email = reader.GetString(6),
                        usuario = reader.GetString(7)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DTrabajador> MostrarNombre(string Usuario)
        {
            List<DTrabajador> ListaGenerica = new List<DTrabajador>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListUser, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@usuario", Usuario);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    ListaGenerica.Add(new DTrabajador
                    {
                        cedula = reader.GetString(0),
                        nombre = reader.GetString(1),
                        apellidos = reader.GetString(2),
                        direccion = reader.GetString(3),
                        telefono = reader.GetString(4),
                        email = reader.GetString(5),
                        usuario = reader.GetString(6)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DTrabajador> Encontrar(int IdTrabajador)
        {
            List<DTrabajador> ListaGenerica = new List<DTrabajador>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListEmployee, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idTrabajador", IdTrabajador);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    int state = reader.GetInt32(12);
                    ListaGenerica.Add(new DTrabajador
                        (
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetDateTime(4),
                            reader.GetString(5),
                            reader.GetString(6),
                            reader.GetString(7),
                            reader.GetString(8),
                            reader.GetInt32(9),
                            reader.GetString(10),
                            reader.GetString(11),
                            state.ToString()
                        ));
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }

        public List<DSeguridad> EncontrarSeguridad(int IdTrabajador)
        {
            List<DSeguridad> ListaGenerica = new List<DSeguridad>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListSecurity, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idTrabajador", IdTrabajador);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DSeguridad
                        (
                            reader.GetInt32(1),
                            reader.GetString(2),
                            reader.GetString(3)
                        ));
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DTrabajador> Login(string User, string Password)
        {
            List<DTrabajador> ListaGenerica = new List<DTrabajador>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryLogin, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@usuario", User);
                comm.Parameters.AddWithValue("@contraseña", Password);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    ListaGenerica.Add(new DTrabajador
                        (
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetDateTime(4),
                            reader.GetString(5),
                            reader.GetString(6),
                            reader.GetString(7),
                            reader.GetString(8),
                            reader.GetInt32(9),
                            reader.GetString(10),
                            reader.GetString(11),
                            reader.GetInt32(12).ToString()
                        ));
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public bool UsuarioRepetido(string Usuario)
        {
            bool respuesta = false;

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryUserRepeated, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@usuario", Usuario);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read()) respuesta = true;
                else respuesta = false;
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public bool CedulaRepetida(string Cedula)
        {
            bool respuesta = false;

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryIDCardRepeated, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@cedula", Cedula);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read()) respuesta = true;
                else respuesta = false;
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }
    }
}
