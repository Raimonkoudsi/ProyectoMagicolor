using System;
using System.Collections.Generic;
using System.Text;
using Datos;

using System.Data;
using System.Data.SqlClient;

namespace Logica
{
    public class LTrabajador:DTrabajador
    {


        //Metodos

        public string Insertar(DTrabajador Trabajador)
        {
            string respuesta = "";

            string query = @"
                        INSERT INTO trabajador(
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
                            pregunta,
                            respuesta,
                            estado
                        ) VALUES(
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
                            @pregunta,
                            @respuesta,
                            @estado
                        );
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {

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
                    comm.Parameters.AddWithValue("@pregunta", Trabajador.pregunta);
                    comm.Parameters.AddWithValue("@respuesta", Trabajador.respuesta);
                    comm.Parameters.AddWithValue("@estado", 1);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del Trabajador";
                    } 
                    catch(SqlException e)
                    {
                        respuesta = e.Message;
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return respuesta;
                }
            }
        }


        public string Editar(DTrabajador Trabajador)
        {
            string respuesta = "";

            string query = @"
                        UPDATE trabajador SET
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
                            pregunta = @pregunta,
                            respuesta = @respuesta
                        WHERE idTrabajador = @idTrabajador;
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {

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
                    comm.Parameters.AddWithValue("@pregunta", Trabajador.pregunta);
                    comm.Parameters.AddWithValue("@respuesta", Trabajador.respuesta);
                    comm.Parameters.AddWithValue("@idTrabajador", Trabajador.idTrabajador);

                    try
                    {
                        conn.Open();
                        var Resp = comm.ExecuteNonQuery();
                        respuesta = Resp == 1 ? "OK" : "No se actualizo el Registro del Trabajador";
                        Console.WriteLine(Resp);
                    }
                    catch (SqlException e)
                    {
                        respuesta = e.Message;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return respuesta;
                }
            }
        }


        public string Eliminar(DTrabajador Trabajador)
        {
            string respuesta = "";

            string query = @"
                        DELETE FROM trabajador WHERE idTrabajador=@idTrabajador
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {

                    comm.Parameters.AddWithValue("@idTrabajador", Trabajador.idTrabajador);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se elimino el Registro del Trabajador";
                    }
                    catch (SqlException e)
                    {
                        respuesta = e.Message;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return respuesta;
                }
            }
        }



        //funcionando
        public List<DTrabajador> Mostrar(string Buscar)
        {
            List<DTrabajador> ListaGenerica = new List<DTrabajador>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT idTrabajador, cedula, nombre, apellidos, direccion, telefono, email, usuario from [trabajador] where cedula like '" + Buscar + "%' order by cedula";


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

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
                        }
                    }
                    catch (SqlException e)
                    {
                        respuesta = e.Message;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return ListaGenerica;
                }
            }

        }

        public List<DTrabajador> MostrarNombre(string Buscar)
        {
            List<DTrabajador> ListaGenerica = new List<DTrabajador>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT cedula, nombre, apellidos, direccion, telefono, email, usuario from trabajador where usuario like '" + Buscar + "%' order by usuario";


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

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
                        }
                    }
                    catch
                    {
                        //error
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return ListaGenerica;
                }
            }

        }
        public List<DTrabajador> Encontrar(int Buscar)
        {
            List<DTrabajador> ListaGenerica = new List<DTrabajador>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * from [trabajador] where idTrabajador = " + Buscar + " ";


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                int state = reader.GetInt32(14);
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
                                        reader.GetString(9),
                                        reader.GetString(10),
                                        reader.GetString(11),
                                        reader.GetString(12),
                                        reader.GetString(13),
                                        state.ToString()
                                    ));
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        respuesta = e.Message;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return ListaGenerica;
                }
            }

        }

        public String[] Login(string Usuario, string Contraseña)
        {
            String[] respuesta = new String[4];

            string error = "";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = @"
                        SELECT cedula, usuario, nombre, acceso
                        FROM [trabajador] 
                        WHERE usuario = @usuario AND contraseña = @contraseña
	                ;";

                    comm.Parameters.AddWithValue("@usuario", Usuario);
                    comm.Parameters.AddWithValue("@contraseña", Contraseña);

                    try
                    {
                        conn.Open();
                        //respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "Usuario no Existe";

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                respuesta[0] = String.Format("{0}", reader["cedula"]);
                                respuesta[1] = String.Format("{0}", reader["usuario"]);
                                respuesta[2] = String.Format("{0}", reader["nombre"]);
                                respuesta[3] = String.Format("{0}", reader["acceso"]);
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        error = e.Message;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return respuesta;
                }
            }
        }
    }
}
