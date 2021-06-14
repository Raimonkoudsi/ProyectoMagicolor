using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class VistaPrincipal : Page
    {
        public LArticulo Metodos = new LArticulo();

        public new MainWindow Parent;

        public VistaPrincipal(MainWindow parent)
        {
            InitializeComponent();

            Parent = parent;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtUsuario.Text = Globals.USUARIO_SISTEMA;
            if (Globals.ACCESO_SISTEMA == 0)
                txtAcceso.Text = "Administrador";
            else if (Globals.ACCESO_SISTEMA == 1)
                txtAcceso.Text = "Encargado";
            else if (Globals.ACCESO_SISTEMA == 2)
                txtAcceso.Text = "Vendedor";

            Tuple<int, int> Stocks = Metodos.ContadorStockVistaPrincipal();
            txtConStock.Text = Stocks.Item1.ToString();
            txtSinStock.Text = Stocks.Item2.ToString();

            Tuple<int, int> CxC = Metodos.ContadorCxCVistaPrincipal();
            txtCxCConTiempo.Text = CxC.Item1.ToString();
            txtCxCSinTiempo.Text = CxC.Item2.ToString();

            Tuple<int, int> CxP = Metodos.ContadorCxPVistaPrincipal();
            txtCxPConTiempo.Text = CxP.Item1.ToString();
            txtCxPSinTiempo.Text = CxP.Item2.ToString();
        }


        private void ArticulosDisponibles_Click(object sender, RoutedEventArgs e)
        {
            InventarioDG Frm = new InventarioDG(Parent, 1);
            Parent.SwitchScreen(Frm);
        }

        private void ArticulosFaltantes_Click(object sender, RoutedEventArgs e)
        {
            InventarioDG Frm = new InventarioDG(Parent, 2);
            Parent.SwitchScreen(Frm);
        }

        private void CuentasCobrar_Click(object sender, RoutedEventArgs e)
        {
            CuentaCobrarDG Frm = new CuentaCobrarDG(Parent, 1);
            Parent.SwitchScreen(Frm);
        }

        private void CuentasCobrarFaltantes_Click(object sender, RoutedEventArgs e)
        {
            CuentaCobrarDG Frm = new CuentaCobrarDG(Parent, 2);
            Parent.SwitchScreen(Frm);
        }

        private void CuentasPagar_Click(object sender, RoutedEventArgs e)
        {
            CuentaPagarDG Frm = new CuentaPagarDG(Parent, 1);
            Parent.SwitchScreen(Frm);
        }

        private void CuentasPagarFaltantes_Click(object sender, RoutedEventArgs e)
        {
            CuentaPagarDG Frm = new CuentaPagarDG(Parent, 2);
            Parent.SwitchScreen(Frm);
        }
    }
}
