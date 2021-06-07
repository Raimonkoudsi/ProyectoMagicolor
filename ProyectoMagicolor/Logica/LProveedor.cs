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
        #region QUERIES 
        private string queryInsert = @"
            INSERT INTO [proveedor] (
                idProveedor,
                razonSocial,
                sectorComercial,
                tipoDocumento,
                numeroDocumento,
                direccion,
                telefono,
                email,
                url,
                estado
            ) VALUES (
                @idProveedor,
                @razonSocial,
                @sectorComercial,
                @tipoDocumento,
                @numeroDocumento,
                @direccion,
                @telefono,
                @email,
                @url,
                1
            );
        ";

        private string queryUpdate = @"
            UPDATE [proveedor] SET 
                razonSocial = @razonSocial,
                sectorComercial = @sectorComercial,
                tipoDocumento = @tipoDocumento,
                numeroDocumento = @numeroDocumento,
                direccion = @direccion,
                telefono = @telefono,
                email = @email,
                url = @url,
                estado = 1
            WHERE idProveedor = @idProveedor;
        ";

        private string queryDelete = @"
            UPDATE [proveedor] SET 
                estado = 0
            WHERE idProveedor = @idProveedor;
        ";

        private string queryListSpecific = @"
            SELECT * FROM [proveedor] 
            WHERE tipoDocumento = @tipoDocumento AND numeroDocumento = @numeroDocumento
        ";

        private string queryListID = @"
            SELECT * FROM [proveedor] 
            WHERE idProveedor = @idProveedor
        ";

        private string queryIDCardRepeated = @"
            SELECT idProveedor FROM [proveedor] 
            WHERE CONCAT(tipoDocumento , '-', numeroDocumento) = @cedula AND estado <> 0;
        ";

        private string queryIDCardRepeatedNull = @"
            SELECT * FROM [proveedor]
            WHERE CONCAT(tipoDocumento , '-', numeroDocumento) = @cedula AND estado = 0;
        ";
        #endregion

        public string Insertar(DProveedor Proveedor, bool ProveedorVacio = false)
        {
            string respuesta = "";

            Action action = () =>
            {
                int idProveedor = ProveedorVacio == false ? LFunction.GetID("proveedor", "idProveedor") : 0;

                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idProveedor", idProveedor);
                comm.Parameters.AddWithValue("@razonSocial", Proveedor.razonSocial);
                comm.Parameters.AddWithValue("@sectorComercial", Proveedor.sectorComercial);
                comm.Parameters.AddWithValue("@tipoDocumento", Proveedor.tipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", Proveedor.numeroDocumento);
                comm.Parameters.AddWithValue("@direccion", Proveedor.direccion);
                comm.Parameters.AddWithValue("@telefono", Proveedor.telefono);
                comm.Parameters.AddWithValue("@email", Proveedor.email);
                comm.Parameters.AddWithValue("@url", Proveedor.url);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Registró el Proveedor";
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public string Editar(DProveedor Proveedor)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryUpdate, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@razonSocial", Proveedor.razonSocial);
                comm.Parameters.AddWithValue("@sectorComercial", Proveedor.sectorComercial);
                comm.Parameters.AddWithValue("@tipoDocumento", Proveedor.tipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", Proveedor.numeroDocumento);
                comm.Parameters.AddWithValue("@direccion", Proveedor.direccion);
                comm.Parameters.AddWithValue("@telefono", Proveedor.telefono);
                comm.Parameters.AddWithValue("@email", Proveedor.email);
                comm.Parameters.AddWithValue("@url", Proveedor.url);
                comm.Parameters.AddWithValue("@idProveedor", Proveedor.idProveedor);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Proveedor";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Proveedor Actualizado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public string Eliminar(int IdProveedor)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryDelete, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idProveedor", IdProveedor);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Deshabilitó el Proveedor";
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DProveedor> Mostrar(string TipoDocumento, string NumeroDocumento, int Estado)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            string queryListGeneral = @"
                SELECT * FROM [proveedor] 
                WHERE tipoDocumento = @tipoDocumento AND numeroDocumento LIKE @numeroDocumento + '%' " + BuscarEstado(Estado) + @"
                ORDER BY numeroDocumento
            ";

            Action action = () =>
                {
                    using SqlCommand comm = new SqlCommand(queryListGeneral, Conexion.ConexionSql);
                    comm.Parameters.AddWithValue("@tipoDocumento", TipoDocumento);
                    comm.Parameters.AddWithValue("@numeroDocumento", NumeroDocumento);

                    using SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        ListaGenerica.Add(new DProveedor
                        {
                            idProveedor = reader.GetInt32(0),
                            razonSocial = reader.GetString(1),
                            sectorComercial = reader.GetString(2),
                            numeroDocumento = reader.GetString(3) + "-" + reader.GetString(4),
                            direccion = reader.GetString(5),
                            telefono = reader.GetString(6) == null ? "Sin Teléfono" : reader.GetString(6),
                            email = reader.GetString(7),
                            url = reader.GetString(8),
                            estado = reader.GetInt32(9),
                            accesoTrabajadorIngresado = Globals.ACCESO_SISTEMA,
                            nombreTrabajadorIngresado = Globals.TRABAJADOR_SISTEMA
                        });
                    }
                };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }

        public string BuscarEstado(int Estado)
        {
            if (Estado == 1)
                return "AND estado = 1";
            if (Estado == 2)
                return "AND estado = 0";
            if (Estado == 3)
                return "";

            return "AND estado = 2";
        }


        public List<DProveedor> Encontrar(int IdProveedor)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListID, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idProveedor", IdProveedor);

                using SqlDataReader reader = comm.ExecuteReader();
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
                        url = reader.GetString(8),
                        estado = reader.GetInt32(9)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;

        }


        public List<DProveedor> EncontrarConDocumento(string TipoDocumento, string NumeroDocumento)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListSpecific, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@tipoDocumento", TipoDocumento);
                comm.Parameters.AddWithValue("@numeroDocumento", NumeroDocumento);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
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
                        url = reader.GetString(8),
                        estado = reader.GetInt32(9)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
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

        public List<DProveedor> CedulaRepetidaAnulada(string Cedula)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryIDCardRepeatedNull, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@cedula", Cedula);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
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
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }



        public List<DProveedor> ListadoProveedorArticulo(int IdArticulo)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            string queryListArticlee = @"
                SELECT 
	                p.razonSocial,
	                (ISNULL((
                        SELECT TOP 1
                            i.fecha
                        FROM [ingreso] i 
			                INNER JOIN [detalleIngreso] di ON i.idIngreso = di.idIngreso
			                INNER JOIN [articulo] a ON di.idArticulo = a.idArticulo
		                WHERE a.idArticulo = @idArticulo
			                AND i.idProveedor = p.idProveedor
                            AND i.estado <> 0
                        ORDER BY di.idDetalleIngreso DESC
                    ), '01/01/2000')) AS ultimaCompra,
	                (ISNULL((
                        SELECT TOP 1
                            di.precioCompra
                        FROM [ingreso] i 
			                INNER JOIN [detalleIngreso] di ON i.idIngreso = di.idIngreso
			                INNER JOIN [articulo] a ON di.idArticulo = a.idArticulo
		                WHERE a.idArticulo = @idArticulo
			                AND i.idProveedor = p.idProveedor
                            AND i.estado <> 0
                        ORDER BY di.idDetalleIngreso DESC
                    ), 0)) AS ultimoPrecio
                FROM [proveedor] p
                WHERE p.razonSocial <> '0'
	                AND p.estado <> 0
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListArticlee, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idArticulo", IdArticulo);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    if(reader.GetDecimal(2) != 0)
                    {
                        ListaGenerica.Add(new DProveedor
                        {
                            razonSocial = reader.GetString(0),
                            ultimaCompra = reader.GetDateTime(1).ToString("dd/MM/yyyy"),
                            ultimoPrecio = (double)reader.GetDecimal(2)
                        });
                    }
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DProveedor> ListadoProveedorCategoria(string Categoria)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            string queryListArticlee = @"
                SELECT 
	                p.razonSocial,
	                p.sectorComercial
                FROM [proveedor] p
                WHERE p.razonSocial <> '0'
	                AND p.estado <> 0
	                AND (p.sectorComercial = @categoria OR p.sectorComercial = 'Variedades')
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListArticlee, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@categoria", Categoria);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DProveedor
                    {
                        razonSocial = reader.GetString(0),
                        sectorComercial = reader.GetString(1)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }
    }
}
