using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Datos
{
    public class Conexion
    {

        protected static string conexionRaimon = "DESKTOP-2185U8G\\SQLEXPRESS";
        protected static string conexionJose = "DESKTOP-KOFID31\\SQLEXPRESS01";
        protected static string CadenaConexion = "Data Source= " + conexionRaimon + "; Initial Catalog= dbMagicolor; Integrated Security= true";

        public static SqlConnection ConexionSql = new SqlConnection(CadenaConexion);
    }
}
