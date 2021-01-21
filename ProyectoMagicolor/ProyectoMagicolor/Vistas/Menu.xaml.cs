﻿using MaterialDesignThemes.Wpf;
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

namespace ProyectoMagicolor
{
	/// <summary>
	/// Lógica de interacción para MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        public Login ParentFrm;
        
		public MainWindow()
		{
			InitializeComponent();

            var menuRegister = new List<SubItem>();
            menuRegister.Add(new SubItem("Customer"));
            menuRegister.Add(new SubItem("Providers"));
            menuRegister.Add(new SubItem("Employees"));
            menuRegister.Add(new SubItem("Products"));
            var item5 = new ItemMenu("Register", menuRegister, PackIconKind.Register);
            
            
            var menuSchedule = new List<SubItem>();
            menuSchedule.Add(new SubItem("Articulos", new ArticuloDG()));
            menuSchedule.Add(new SubItem("Categorias", new CategoriaDG()));
            var item1 = new ItemMenu("Articulos", menuSchedule, PackIconKind.Schedule);

            var menuReports = new List<SubItem>();
            menuReports.Add(new SubItem("Trabajadores", new TrabajadoresDG()));
            menuReports.Add(new SubItem("Cliente", new ClienteDG()));
            menuReports.Add(new SubItem("Proveedor", new ProveedorDG()));
            menuReports.Add(new SubItem("Stock"));
            menuReports.Add(new SubItem("Sales"));
            var item2 = new ItemMenu("Pruebas", menuReports, PackIconKind.FileReport);

            var menuExpenses = new List<SubItem>();
            menuExpenses.Add(new SubItem("Fixed"));
            menuExpenses.Add(new SubItem("Variable"));
            var item3 = new ItemMenu("Expenses", menuExpenses, PackIconKind.ShoppingBasket);

            var menuFinancial = new List<SubItem>();
            menuFinancial.Add(new SubItem("Cash Flow"));
            var item4 = new ItemMenu("Expenses", menuFinancial, PackIconKind.ScaleBalance);

            var item0 = new ItemMenu("Dashboard", new ArticuloDG(), PackIconKind.ViewDashboard);

            Menu.Children.Add(new MenuItemX(item0, this));
            Menu.Children.Add(new MenuItemX(item1, this));
            Menu.Children.Add(new MenuItemX(item2, this));
            Menu.Children.Add(new MenuItemX(item3, this));
            Menu.Children.Add(new MenuItemX(item4, this));
            Menu.Children.Add(new MenuItemX(item5, this));

            var Frm = new CompraFrm();
            ContentFrame.Content = Frm;
        }

        public void SwitchScreen(object sender)
        {
            var screen = (Page)sender;

            if(screen != null)
            {
                ContentFrame.Content = screen;
            }
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
