using System;
using System.Collections.Generic;
using System.Text;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Logica
{
    public class LCategoria : DCategoria
    {
        #region QUERIES
        private string queryInsert = @"
            INSERT INTO [categoria] (
                idCategoria,
                nombre,
                descripcion,
                estado
            ) VALUES (
                @idCategoria,
                @nombre,
                @descripcion,
                1
            );
	    ";

        private string queryUpdate = @"
            UPDATE [categoria] SET 
                nombre = @nombre,
                descripcion = @descripcion
            WHERE idCategoria = @idCategoria;
	    ";

        private string queryDelete = @"
            UPDATE [categoria] SET 
                estado = 0
            WHERE idCategoria = @idCategoria;
	    ";

        private string queryList = @"
            SELECT
                idCategoria,
                nombre,
                descripcion
            FROM [categoria]
            WHERE nombre LIKE @nombre + '%' AND  estado <> 0
            ORDER BY nombre;
        ";

        private string queryListID = @"
            SELECT * FROM [categoria] 
            WHERE idCategoria = @idCategoria AND estado <> 0;
        ";
        #endregion


        public string Insertar(DCategoria Category)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idCategoria", LFunction.GetID("categoria", "idCategoria"));
                comm.Parameters.AddWithValue("@nombre", Category.nombre);
                comm.Parameters.AddWithValue("@descripcion", Category.descripcion);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro de la Categoria";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Categoría Ingresada Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public string Editar(DCategoria Category)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryUpdate, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@nombre", Category.nombre);
                comm.Parameters.AddWithValue("@descripcion", Category.descripcion);
                comm.Parameters.AddWithValue("@idCategoria", Category.idCategoria);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Registro de la Categoria";
                if (respuesta.Equals("OK")) LFunction.MessageExecutor("Information", "Categoría Actualizada Correctamente");
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public string Eliminar(int IdCategory)
        {
            string respuesta = "";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryDelete, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idCategoria", IdCategory);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Eliminó el Registro de la Categoria";
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }


        public List<DCategoria> Mostrar(string Name)
        {
            List<DCategoria> ListaGenerica = new List<DCategoria>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@nombre", Name);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DCategoria
                    {
                        idCategoria = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        descripcion = reader.GetString(2) == null ? "No Contiene una Descripción" : reader.GetString(2)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public List<DCategoria> Encontrar(int IdCategoria)
        {
            List<DCategoria> ListaGenerica = new List<DCategoria>();

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryListID, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idCategoria", IdCategoria);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DCategoria
                    {
                        idCategoria = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        descripcion = reader.GetString(2)
                    });
                }
            };
            LFunction.SafeExecutor(action);

            return ListaGenerica;
        }


        public bool CategoriaRepetida(string Categoria, int IdCategoria)
        {
            bool respuesta = false;

            string queryCategoryRepeated = @"
                SELECT idCategoria FROM [categoria]
                WHERE nombre = @nombre 
                    AND estado <> 0
                    AND idCategoria <> 0
                    AND idCategoria <> @idCategoria;
            ";

            Action action = () =>
            {
                using SqlCommand comm = new SqlCommand(queryCategoryRepeated, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@nombre", Categoria);
                comm.Parameters.AddWithValue("@idCategoria", IdCategoria);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read()) respuesta = true;
                else respuesta = false;
            };
            LFunction.SafeExecutor(action);

            return respuesta;
        }
    }
}
