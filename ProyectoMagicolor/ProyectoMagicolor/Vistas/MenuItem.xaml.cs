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
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ProyectoMagicolor.Vistas
{
    /// <summary>
    /// Interaction logic for MenuItem.xaml
    /// </summary>
    public partial class MenuItemX : System.Windows.Controls.UserControl
    {
        MainWindow _context;
        public MenuItemX(ItemMenu itemMenu, MainWindow context)
        {
            InitializeComponent();

            _context = context;

            ExpanderMenu.Visibility = itemMenu.SubItems == null ? Visibility.Collapsed : Visibility.Visible;
            ListViewItemMenu.Visibility = itemMenu.SubItems == null ? Visibility.Visible : Visibility.Collapsed;
            ListViewItemMenu.Content = itemMenu.Header;


            if (itemMenu.SubItems != null)
            {
                if (itemMenu.SubItems[1].Backup == true)
                {
                    ListViewItemMenu.Click += (s, r) =>
                    {
                        //backup
                        _context.Backup();
                    };
                }
                if (itemMenu.SubItems[2].Restore == true)
                {
                    ListViewItemMenu.Click += (s, r) =>
                    {
                        //restore
                        _context.Restore();
                    };
                }
            }
            

            if (itemMenu.SubItems == null)
            {
                ListViewItemMenu.Click += (s, r) =>
                {
                    //_context.SwitchScreen(itemMenu.Screen);
                    _context.LogOut();
                };
            }

            this.DataContext = itemMenu;
        }

        private void LisViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LisViewMenu.SelectedIndex < 0) return;
            _context.SwitchScreen(((SubItem)((System.Windows.Controls.ListView)sender).SelectedItem).Screen);
            LisViewMenu.SelectedItem = 0;
            LisViewMenu.SelectedIndex = -1;
        }
    }
}
