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
                contraseña = @contraseña,
                estado = 1
            WHERE idTrabajador = @idTrabajador;
	    ";

        private string queryDelete = @"
            UPDATE [trabajador] SET
                estado = 0
            WHERE idTrabajador = @idTrabajador;
	    ";

        private string queryListIDCardAdmin = @"
            SELECT 
                idTrabajador, 
                cedula, 
                CONCAT(nombre, ' ', apellidos) as nombreCompleto, 
                direccion, 
                telefono, 
                email, 
                usuario 
            FROM [trabajador] 
                WHERE idTrabajador <> 0
            ORDER BY usuario ASC
        ";


        private string queryListEmployee = @"
            SELECT * FROM [trabajador] 
            WHERE idTrabajador = @idTrabajador;
        ";

        private string queryListEmployeeNull = @"
            SELECT * FROM [trabajador] 
            WHERE CONCAT(tipoDocumento , '-', numeroDocumento) = @cedula AND idTrabajador <> 0 AND estado = 0;
        ";

        private string queryListSecurity = @"
            SELECT * FROM [seguridad] 
            WHERE idTrabajador = @idTrabajador AND idTrabajador <> 0;
        ";

        private string queryLogin = @"
            SELECT * FROM [trabajador] 
            WHERE usuario = @usuario AND contraseña = @contraseña AND idTrabajador <> 0 AND estado <> 0;
        ";

        private string queryFormSecurity = @"
            SELECT 
                t.idTrabajador,
	            t.usuario,
	            s.pregunta,
	            s.respuesta
            FROM [seguridad] s
	            INNER JOIN [trabajador] t ON t.idTrabajador=s.idTrabajador
            WHERE t.usuario = @usuario AND t.idTrabajador <> 0 AND t.estado <> 0;
        ";

        string queryUpdatePassword = @"
            UPDATE [trabajador] SET
                contraseña = @contraseña
            WHERE usuario = @usuario;
	    ";

        private string queryUserRepeated = @"
            SELECT idTrabajador FROM [trabajador]
            WHERE usuario = @usuario;
        ";

        private string queryIDCardRepeated = @"
            SELECT idTrabajador FROM [trabajador] 
            WHERE cedula = @cedula;
        ";

        private string queryUserNull = @"
            SELECT idTrabajador FROM [trabajador]
            WHERE usuario = @usuario AND contraseña = @contraseña AND estado = 0;
        ";


        #endregion

        public string Insertar(DTrabajador Trabajador, List<DSeguridad> Seguridad = null, bool TrabajadorVacio = false)
        {
            string respuesta = "";

            Action action = () =>
            {
                int idTrabajador = TrabajadorVacio == false ? LFunction.GetID("trabajador", "idTrabajador") : 0;

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
                comm.Parameters.AddWithValue("@usuario", Encripter.Encrypt(Trabajador.usuario));
                comm.Parameters.AddWithValue("@contraseña", Encripter.Encrypt(Trabajador.contraseña));

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Trabajador";
                if (respuesta.Equals("OK") && !TrabajadorVacio)
                    respuesta = InsertarSeguridad(idTrabajador, Seguridad);
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        private string InsertarSeguridad(int IdTrabajador, List<DSeguridad> Detalle)
        {
            int i = 0;
            string respuesta = "";

            foreach (DSeguridad det in Detalle)
            {
                using SqlCommand comm = new SqlCommand(queryInsertSecurity, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idSeguridad", LFunction.GetID("seguridad", "idSeguridad"));
                comm.Parameters.AddWithValue("@idTrabajador", IdTrabajador);
                comm.Parameters.AddWithValue("@pregunta", Encripter.Encrypt(Detalle[i].pregunta));
                comm.Parameters.AddWithValue("@respuesta", Encripter.Encrypt(Detalle[i].respuesta));

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
                InsertarSeguridad(Trabajador.idTrabajador, Seguridad);

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
                comm.Parameters.AddWithValue("@usuario", Encripter.Encrypt(Trabajador.usuario));
                comm.Parameters.AddWithValue("@contraseña", Encripter.Encrypt(Trabajador.contraseña));
                comm.Parameters.AddWithValue("@idTrabajador", Trabajador.idTrabajador);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se actualizo el Registro del Trabajador";
                if (respuesta.Equals("OK"))
                    respuesta = BorrarSeguridad(Trabajador, Seguridad);
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

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Deshabilitó el Trabajador";
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DTrabajador> Mostrar(string Cedula, int Estado)
        {
            List<DTrabajador> ListaGenerica = new List<DTrabajador>();

            string queryListIDCard = @"
                SELECT 
                    idTrabajador, 
                    cedula, 
                    CONCAT(nombre, ' ', apellidos) as nombreCompleto, 
                    direccion, 
                    telefono, 
                    email, 
                    usuario,
                    acceso,
                    estado
                FROM [trabajador] 
                WHERE cedula LIKE @cedula + '%' AND acceso <> 0 AND idTrabajador <> 0  " + new LProveedor().BuscarEstado(Estado) + @"
                ORDER BY cedula ASC
            ";

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
                            direccion = reader.GetString(3),
                            telefono = reader.GetString(4),
                            email = reader.GetString(5),
                            usuario = Encripter.Decrypt(reader.GetString(6)),
                            accesoString = reader.GetInt32(7) == 1 ? "Encargado" : "Vendedor",
                            estado = reader.GetInt32(8),
                            accesoTrabajadorIngresado = Globals.ACCESO_SISTEMA,
                            nombreTrabajadorIngresado = Globals.TRABAJADOR_SISTEMA
                        });
                    }
                };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        //auditoria
        public List<DTrabajador> MostrarConAdministrador()
        {
            List<DTrabajador> ListaGenerica = new List<DTrabajador>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListIDCardAdmin, Conexion.ConexionSql);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DTrabajador
                    {
                        idTrabajador = reader.GetInt32(0),
                        cedula = reader.GetString(1),
                        nombre = reader.GetString(2),
                        direccion = reader.GetString(3),
                        telefono = reader.GetString(4),
                        email = reader.GetString(5),
                        usuario = Encripter.Decrypt(reader.GetString(6))
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
                if (reader.Read())
                {
                    ListaGenerica.Add(new DTrabajador
                    {
                        idTrabajador = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        apellidos = reader.GetString(2),
                        sexo = reader.GetString(3),
                        fechaNacimiento = reader.GetDateTime(4),
                        cedula = reader.GetString(5),
                        direccion = reader.GetString(6),
                        telefono = reader.GetString(7),
                        email = reader.GetString(8),
                        acceso = reader.GetInt32(9),
                        usuario = Encripter.Decrypt(reader.GetString(10)),
                        contraseña = Encripter.Decrypt(reader.GetString(11)),
                        estado = reader.GetInt32(12)
                    });
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
                            Encripter.Decrypt(reader.GetString(2)),
                            Encripter.Decrypt(reader.GetString(3))
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
                comm.Parameters.AddWithValue("@usuario", Encripter.Encrypt(User));
                comm.Parameters.AddWithValue("@contraseña", Encripter.Encrypt(Password));

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    ListaGenerica.Add(new DTrabajador
                    {
                        idTrabajador = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        apellidos = reader.GetString(2),
                        sexo = reader.GetString(3),
                        fechaNacimiento = reader.GetDateTime(4),
                        cedula = reader.GetString(5),
                        direccion = reader.GetString(6),
                        telefono = reader.GetString(7),
                        email = reader.GetString(8),
                        acceso = reader.GetInt32(9),
                        usuario = Encripter.Decrypt(reader.GetString(10)),
                        contraseña = Encripter.Decrypt(reader.GetString(11)),
                        estado = reader.GetInt32(12)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DTrabajador> Seguridad(string Usuario)
        {
            List<DTrabajador> ListaGenerica = new List<DTrabajador>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryFormSecurity, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@usuario", Encripter.Encrypt(Usuario));

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DTrabajador
                    {
                        idTrabajador = reader.GetInt32(0),
                        usuario = Encripter.Decrypt(reader.GetString(1)),
                        pregunta = Encripter.Decrypt(reader.GetString(2)),
                        respuesta = Encripter.Decrypt(reader.GetString(3))
                    });
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
                comm.Parameters.AddWithValue("@usuario", Encripter.Encrypt(Usuario));

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


        public List<DTrabajador> CedulaRepetidaAnulada(string Cedula)
        {
            List<DTrabajador> ListaGenerica = new List<DTrabajador>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListEmployeeNull, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@cedula", Cedula);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    ListaGenerica.Add(new DTrabajador
                    {
                        idTrabajador = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        apellidos = reader.GetString(2),
                        sexo = reader.GetString(3),
                        fechaNacimiento = reader.GetDateTime(4),
                        cedula = reader.GetString(5),
                        direccion = reader.GetString(6),
                        telefono = reader.GetString(7),
                        email = reader.GetString(8),
                        acceso = reader.GetInt32(9),
                        usuario = Encripter.Decrypt(reader.GetString(10)),
                        contraseña = Encripter.Decrypt(reader.GetString(11)),
                        estado = reader.GetInt32(12)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public bool UsuarioAnulado(string Usuario, string Contraseña)
        {
            bool respuesta = false;

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryUserNull, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@usuario", Encripter.Encrypt(Usuario));
                comm.Parameters.AddWithValue("@contraseña", Encripter.Encrypt(Contraseña));

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read()) respuesta = true;
                else respuesta = false;
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public string EditarContraseña(string Usuario, string Contraseña)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryUpdatePassword, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@usuario", Encripter.Encrypt(Usuario));
                comm.Parameters.AddWithValue("@contraseña", Encripter.Encrypt(Contraseña));

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se actualizó la contraseña";
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }
    }
}
