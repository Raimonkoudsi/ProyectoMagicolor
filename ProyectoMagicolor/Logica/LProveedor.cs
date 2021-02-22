using System;
using System.Collections.Generic;
using System.Text;

using Datos;

using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LProveedor : DProveedor
    {

        //Metodos

        public string Insertar(DProveedor Proveedor)
        {
            string respuesta = "";

            string query = @"
                        INSERT INTO proveedor(
                            razonSocial,
                            sectorComercial,
                            tipoDocumento,
                            numeroDocumento,
                            direccion,
                            telefono,
                            email,
                            url
                        ) VALUES(
                            @razonSocial,
                            @sectorComercial,
                            @tipoDocumento,
                            @numeroDocumento,
                            @direccion,
                            @telefono,
                            @email,
                            @url
                        );
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@razonSocial", Proveedor.razonSocial);
                    comm.Parameters.AddWithValue("@sectorComercial", Proveedor.sectorComercial);
                    comm.Parameters.AddWithValue("@tipoDocumento", Proveedor.tipoDocumento);
                    comm.Parameters.AddWithValue("@numeroDocumento", Proveedor.numeroDocumento);
                    comm.Parameters.AddWithValue("@direccion", Proveedor.direccion);
                    comm.Parameters.AddWithValue("@telefono", Proveedor.telefono);
                    comm.Parameters.AddWithValue("@email", Proveedor.email);
                    comm.Parameters.AddWithValue("@url", Proveedor.url);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingreso el Registro del Proveedor";
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


        public string Editar(DProveedor Proveedor)
        {
            string respuesta = "";

            string query = @"
                        UPDATE proveedor SET 
                            razonSocial = @razonSocial,
                            sectorComercial = @sectorComercial,
                            tipoDocumento = @tipoDocumento,
                            numeroDocumento = @numeroDocumento,
                            direccion = @direccion,
                            telefono = @telefono,
                            email = @email,
                            url = @url
                            WHERE idProveedor = @idProveedor;
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("@razonSocial", Proveedor.razonSocial);
                    comm.Parameters.AddWithValue("@sectorComercial", Proveedor.sectorComercial);
                    comm.Parameters.AddWithValue("@tipoDocumento", Proveedor.tipoDocumento);
                    comm.Parameters.AddWithValue("@numeroDocumento", Proveedor.numeroDocumento);
                    comm.Parameters.AddWithValue("@direccion", Proveedor.direccion);
                    comm.Parameters.AddWithValue("@telefono", Proveedor.telefono);
                    comm.Parameters.AddWithValue("@email", Proveedor.email);
                    comm.Parameters.AddWithValue("@url", Proveedor.url);

                    comm.Parameters.AddWithValue("@idProveedor", Proveedor.idProveedor);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se actualizo el Registro del Proveedor";
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


        public string Eliminar(DProveedor Proveedor)
        {
            string respuesta = "";

            string query = @"
                        DELETE FROM proveedor WHERE idProveedor=@idProveedor
	        ";

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand(query, conn))
                {

                    comm.Parameters.AddWithValue("@idProveedor", Proveedor.idProveedor);

                    try
                    {
                        conn.Open();
                        respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se elimino el Registro del Proveedor";
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
        public List<DProveedor> Mostrar(string Buscar, string Buscar2)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * from [proveedor] where tipoDocumento = '" + Buscar + "' AND numeroDocumento like '" + Buscar2 + "%' order by numeroDocumento";

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DProveedor
                                {
                                    idProveedor = reader.GetInt32(0),
                                    razonSocial = reader.GetString(1),
                                    sectorComercial = reader.GetString(2),
                                    tipoDocumento = reader.GetString(3),
                                    numeroDocumento = reader.GetString(4),
                                    direccion = reader.GetString(5),
                                    telefono = reader.GetString(6),
                                    email = reader.GetString(7),
                                    url = reader.GetString(8)
                                });
                            }
                        }
                    }
                    catch (SqlException e)
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
        public List<DProveedor> Encontrar(int Buscar)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * from [proveedor] WHERE idProveedor= " + Buscar + "";


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DProveedor
                                {
                                    idProveedor = reader.GetInt32(0),
                                    razonSocial = reader.GetString(1),
                                    sectorComercial = reader.GetString(2),
                                    tipoDocumento = reader.GetString(3),
                                    numeroDocumento = reader.GetString(4),
                                    direccion = reader.GetString(5),
                                    telefono = reader.GetString(6),
                                    email = reader.GetString(7),
                                    url = reader.GetString(8)
                                });
                            }
                        }
                    }
                    catch (SqlException e)
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

        public List<DProveedor> EncontrarConDocumento(string Tipo, string NroDocumento)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaConexion))
            {

                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;

                    comm.CommandText = "SELECT * from [proveedor] WHERE tipoDocumento = '" + Tipo + "' AND numeroDocumento = '" + NroDocumento + "'";


                    //comm.Parameters.AddWithValue("@textoBuscar", "");

                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ListaGenerica.Add(new DProveedor
                                {
                                    idProveedor = reader.GetInt32(0),
                                    razonSocial = reader.GetString(1),
                                    sectorComercial = reader.GetString(2),
                                    tipoDocumento = reader.GetString(3),
                                    numeroDocumento = reader.GetString(4),
                                    direccion = reader.GetString(5),
                                    telefono = reader.GetString(6),
                                    email = reader.GetString(7),
                                    url = reader.GetString(8)
                                });
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        //error
                        MessageBox.Show(e.Message);
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
    }
}
