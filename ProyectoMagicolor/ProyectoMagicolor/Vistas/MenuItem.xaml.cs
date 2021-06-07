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


            if (itemMenu.SubItems == null)
            {
                ListViewItemMenu.Click += (s, r) =>
                {
                    _context.LogOut();
                };
            }

            this.DataContext = itemMenu;
        }

        private void LisViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LisViewMenu.SelectedIndex < 0) return;
            SubItem SI = ((SubItem)((System.Windows.Controls.ListView)sender).SelectedItem);

            if (SI.Backup)
            {
                _context.Backup();
            }
            else if (SI.Restore)
            {
                _context.Restore();
            }
            else if (SI.IVA)
            {
                _context.ChangeIVA();
            }
            else
            {
                _context.SwitchScreen(SI.Screen);

            }

            LisViewMenu.SelectedItem = 0;
            LisViewMenu.SelectedIndex = -1;
        }
    }
}
