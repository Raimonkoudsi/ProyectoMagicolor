using MaterialDesignThemes.Wpf;
using ProyectoMagicolor.Vistas;
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

using Datos;
using Logica;

namespace ProyectoMagicolor
{
	public partial class MainWindow : Window
	{
        public Login ParentFrm;
        
		public MainWindow(DTrabajador trabajador)
		{
			InitializeComponent();

            LoggedTrabajador = trabajador;

            var menuRegister = new List<SubItem>();
            menuRegister.Add(new SubItem("Cliente", new ClienteDG()));
            menuRegister.Add(new SubItem("Proveedor", new ProveedorDG()));
            menuRegister.Add(new SubItem("Trabajador", new TrabajadoresDG()));
            menuRegister.Add(new SubItem("Artículo", new ArticuloDG()));
            menuRegister.Add(new SubItem("Categoría", new CategoriaDG()));
            var item1 = new ItemMenu("Registros", menuRegister, PackIconKind.Register);

            var menuActions = new List<SubItem>();
            menuActions.Add(new SubItem("Venta", new VentaFrm(this)));
            menuActions.Add(new SubItem("Compra", new CompraFrm(this)));
            menuActions.Add(new SubItem("Cuenta por Cobrar", new CuentaCobrarDG(this)));
            menuActions.Add(new SubItem("Cuenta por Pagar", new CuentaPagarDG(this)));
            menuActions.Add(new SubItem("Devolución", new DevolucionInicio(this)));
            var item2 = new ItemMenu("Acciones", menuActions, PackIconKind.PointOfSale);

            var menuList = new List<SubItem>();
            menuList.Add(new SubItem("Ventas", new VentaDG(this)));
            menuList.Add(new SubItem("Compras")); //falta
            menuList.Add(new SubItem("Cuentas por Cobrar")); //falta
            menuList.Add(new SubItem("Cuentas por Pagar")); //falta
            menuList.Add(new SubItem("Devoluciones")); //falta
            menuList.Add(new SubItem("Inventario", new InventarioDG()));
            var item3 = new ItemMenu("Listados", menuList, PackIconKind.AccountBoxes);

            var item4 = new ItemMenu("Cerrar Sesión", new ArticuloDG(), PackIconKind.Logout);

            Menu.Children.Add(new MenuItemX(item1, this));
            Menu.Children.Add(new MenuItemX(item2, this));
            Menu.Children.Add(new MenuItemX(item3, this));
            Menu.Children.Add(new MenuItemX(item4, this));

        }

        public static DTrabajador LoggedTrabajador;

        public void SwitchScreen(object sender)
        {
            var screen = (Page)sender;

            if(screen != null)
            {
                ContentFrame.Content = screen;
            }
        }

        public void LogOut()
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

		private void ButtonPopUpLogout_Click(object sender, RoutedEventArgs e)
        {
			Application.Current.Shutdown();
        }

		
	}
	public class ItemMenu
    {
        public ItemMenu(string header, List<SubItem> subItems, PackIconKind icon)
        {
            Header = header;
            Icon = icon;
            SubItems = subItems;
        }
        
        public ItemMenu(string header, Page screen, PackIconKind icon)
        {
            Header = header;
            Icon = icon;
            Screen = screen;
        }

        public string Header { get; private set; }
		public PackIconKind Icon { get; private set; }
		public List<SubItem> SubItems { get; private set; }
		public Page Screen { get; private set; }
    }

	public class SubItem
    {
        public SubItem(string name, Page screen = null)
        {
            Name = name;
            Screen = screen;
        }

        public string Name { get; private set; }
		public Page Screen { get; private set; }
    }

}
