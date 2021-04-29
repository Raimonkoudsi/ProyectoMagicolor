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
	/// <summary>
	/// Lógica de interacción para MainWindow.xaml
	/// </summary>
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
            var item1 = new ItemMenu("Registro", menuRegister, PackIconKind.Register);

            var menuSale = new List<SubItem>();
            menuSale.Add(new SubItem("Nueva", new VentaFrm(this)));
            menuSale.Add(new SubItem("Listado", new VentaDG(this))); //FALTA
            var item2 = new ItemMenu("Venta", menuSale, PackIconKind.PointOfSale);

            var menuCC = new List<SubItem>();
            menuCC.Add(new SubItem("Abono", new CuentaCobrarDG(this)));
            menuCC.Add(new SubItem("Listado")); //FALTA
            var item3 = new ItemMenu("Cuenta por Cobrar", menuCC, PackIconKind.Salesforce);

            var menuBuy = new List<SubItem>();
            menuBuy.Add(new SubItem("Nueva", new CompraFrm(this)));
            menuBuy.Add(new SubItem("Listado")); //FALTA
            var item4 = new ItemMenu("Compra", menuBuy, PackIconKind.ShoppingOutline);

            var menuCP = new List<SubItem>();
            menuCP.Add(new SubItem("Abono", new CuentaPagarDG(this)));
            menuCP.Add(new SubItem("Listado")); //FALTA
            var item5 = new ItemMenu("Cuenta por Pagar", menuCP, PackIconKind.Salesforce);

            var menuRefund = new List<SubItem>();
            menuRefund.Add(new SubItem("Nueva", new DevolucionInicio(this)));
            menuRefund.Add(new SubItem("Listado")); //FALTA
            var item6 = new ItemMenu("Devolución", menuRefund, PackIconKind.CashRefund);

            var item7 = new ItemMenu("Inventario", new InventarioDG(), PackIconKind.CartCheck);

            var item0 = new ItemMenu("Cerrar Sesión", new ArticuloDG(), PackIconKind.Logout);

            Menu.Children.Add(new MenuItemX(item0, this));
            Menu.Children.Add(new MenuItemX(item1, this));
            Menu.Children.Add(new MenuItemX(item2, this));
            Menu.Children.Add(new MenuItemX(item3, this));
            Menu.Children.Add(new MenuItemX(item4, this));
            Menu.Children.Add(new MenuItemX(item5, this));
            Menu.Children.Add(new MenuItemX(item6, this));
            Menu.Children.Add(new MenuItemX(item7, this));

            var Frm = new CompraFrm(this);
            ContentFrame.Content = Frm;
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
