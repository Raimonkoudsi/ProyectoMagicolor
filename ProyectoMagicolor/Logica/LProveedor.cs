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
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Proveedor Ingresado Correctamente");
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
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Proveedor Deshabilitado Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DProveedor> Mostrar(string TipoDocumento, string NumeroDocumento, int Estado)
        {
            List<DProveedor> ListaGenerica = new List<DProveedor>();

            string queryListGeneral = @"
                SELECT * FROM [proveedor] 
                WHERE tipoDocumento LIKE  @tipoDocumento + '%' 
                    AND razonSocial <> '0'
                    AND numeroDocumento LIKE @numeroDocumento + '%' " + BuscarEstado(Estado) + @"
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


        public bool CedulaRepetida(string Cedula, int IdProveedor)
        {
            bool respuesta = false;

            string queryIDCardRepeated = @"
                SELECT idProveedor FROM [proveedor] 
                WHERE CONCAT(tipoDocumento , '-', numeroDocumento) = @cedula 
                    AND estado <> 0
                    AND idProveedor <> @idProveedor;
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryIDCardRepeated, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@cedula", Cedula);
                comm.Parameters.AddWithValue("@idProveedor", IdProveedor);

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

            string queryIDCardRepeatedNull = @"
                SELECT * FROM [proveedor]
                WHERE CONCAT(tipoDocumento , '-', numeroDocumento) = @cedula 
                    AND idProveedor <> 0
                    AND estado = 0;
            ";

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
                    p.idProveedor,
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
                    if(reader.GetDecimal(3) != 0)
                    {
                        ListaGenerica.Add(new DProveedor
                        {
                            idProveedor = reader.GetInt32(0),
                            razonSocial = reader.GetString(1),
                            ultimaCompra = reader.GetDateTime(2).ToString("dd/MM/yyyy"),
                            ultimoPrecio = (double)reader.GetDecimal(3)
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
                    p.idProveedor,
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
                        idProveedor = reader.GetInt32(0),
                        razonSocial = reader.GetString(1),
                        sectorComercial = reader.GetString(2)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }



        public List<DArticulo> ListadoArticuloPorProveedor(int IdProveedor, string Categoria, string Nombre)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            string queryListArticlee = @"
                SELECT
					a.codigo,
					a.nombre,
                    c.nombre,
					ISNULL((
						SELECT TOP 1 
							di.cantidadActual 
						FROM [detalleIngreso] di 
						WHERE a.idArticulo = di.idArticulo
						ORDER BY di.idDetalleIngreso DESC
					), 0) AS cantidadActual, 
					(ISNULL((
                        SELECT TOP 1
                            i.fecha
                        FROM [ingreso] i 
			                INNER JOIN [detalleIngreso] di ON i.idIngreso = di.idIngreso
		                WHERE di.idArticulo = a.idArticulo
			                AND i.idProveedor = @idProveedor
                            AND i.estado <> 0
                        ORDER BY di.idDetalleIngreso DESC
                    ), '01/01/2000')) AS ultimaCompra,
	                (ISNULL((
                        SELECT TOP 1
                            di.precioCompra
                        FROM [ingreso] i 
			                INNER JOIN [detalleIngreso] di ON i.idIngreso = di.idIngreso
		                WHERE di.idArticulo = a.idArticulo
			                AND i.idProveedor = @idProveedor
                            AND i.estado <> 0
                        ORDER BY di.idDetalleIngreso DESC
                    ), 0)) AS ultimoPrecio
					FROM [articulo] a
                        INNER JOIN [categoria] c ON a.idCategoria = c.idCategoria
					WHERE a.estado <> 0 
                        AND c.nombre LIKE @categoria + '%'
                        AND a.nombre LIKE @nombre + '%'
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListArticlee, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idProveedor", IdProveedor);
                comm.Parameters.AddWithValue("@categoria", Categoria == null ? "" : Categoria);
                comm.Parameters.AddWithValue("@nombre", Nombre);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetDecimal(5) != 0)
                    {
                        ListaGenerica.Add(new DArticulo
                        {
                            codigo = reader.GetString(0),
                            nombre = reader.GetString(1),
                            categoria = reader.GetString(2),
                            cantidadActual = reader.GetInt32(3),
                            ultimaActualizacion = reader.GetDateTime(4).ToString("dd/MM/yyyy"),
                            precioCompra = (double)reader.GetDecimal(5)
                        });
                    }
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DArticulo> CategoriasPorProveedor(int IdProveedor)
        {
            List<DArticulo> ListaGenerica = new List<DArticulo>();

            string queryListArticlee = @"
                SELECT DISTINCT
	                c.nombre
                FROM [proveedor] p
	                INNER JOIN [ingreso] i ON i.idProveedor = p.idProveedor
	                INNER JOIN [detalleIngreso] di ON di.idIngreso = i.idIngreso
	                INNER JOIN [articulo] a ON a.idArticulo = di.idArticulo
	                INNER JOIN [categoria] c ON a.idCategoria = c.idCategoria
                WHERE p.idProveedor = @idProveedor
	                AND i.estado <> 0
	                AND di.estado <> 0
	                AND a.estado <> 0
	                AND p.estado <> 0
	                AND c.estado <> 0
                    AND p.estado <> 0
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListArticlee, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idProveedor", IdProveedor);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DArticulo
                    {
                        categoria = reader.GetString(0)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }
    }
}
