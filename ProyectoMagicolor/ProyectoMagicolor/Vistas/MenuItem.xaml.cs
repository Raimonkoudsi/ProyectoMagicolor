using System.Windows;
using System.Windows.Controls;

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


            if (itemMenu.SubItems == null && itemMenu.Principal == false)
            {
                ListViewItemMenu.Click += (s, r) =>
                {
                    _context.LogOut();
                };
            }
            else if (itemMenu.SubItems == null && itemMenu.Principal == true)
            {
                ListViewItemMenu.Click += (s, r) =>
                {
                    _context.Home();
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
            else if (SI.Help)
            {
                _context.AbrirManual();
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
