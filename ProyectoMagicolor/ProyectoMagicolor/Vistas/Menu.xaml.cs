using MaterialDesignThemes.Wpf;
using ProyectoMagicolor.Vistas;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Diagnostics;
using Datos;
using Logica;

namespace ProyectoMagicolor
{

    public partial class MainWindow : Window
	{
        public Login ParentFrm;

        public bool logout = false;
        public MainWindow(DTrabajador trabajador)
        {
            InitializeComponent();

            LoggedTrabajador = trabajador;
            Globals.ACCESO_SISTEMA = trabajador.acceso;
            Globals.ID_SISTEMA = trabajador.idTrabajador;
            Globals.TRABAJADOR_SISTEMA = (trabajador.cedula + " " + trabajador.apellidos + " " + trabajador.nombre);
            Globals.USUARIO_SISTEMA = trabajador.usuario;

            DAuditoria auditoria = new DAuditoria(
                                                    Globals.ID_SISTEMA,
                                                    "Ingresar",
                                                    "Ha ingresado al Sistema"
                                                 );

            new LAuditoria().Insertar(auditoria);
            
            var menuRegister = new List<SubItem>();
            menuRegister.Add(new SubItem("Cliente", new ClienteDG()));
            if(LoggedTrabajador.acceso==0 || LoggedTrabajador.acceso == 1)
            {
                menuRegister.Add(new SubItem("Proveedor", new ProveedorDG()));
                if (LoggedTrabajador.acceso == 0)
                {
                    menuRegister.Add(new SubItem("Trabajador", new TrabajadoresDG()));
                }
                menuRegister.Add(new SubItem("Categoría", new CategoriaDG()));
            }
            var item1 = new ItemMenu("Registros", menuRegister, PackIconKind.Register);

            var menuActions = new List<SubItem>();
            menuActions.Add(new SubItem("Venta", new VentaFrm(this)));
            if (LoggedTrabajador.acceso == 0 || LoggedTrabajador.acceso == 1)
            {
                menuActions.Add(new SubItem("Compra", new CompraFrm(this)));
                menuActions.Add(new SubItem("Cuenta por Cobrar", new CuentaCobrarDG(this)));
                menuActions.Add(new SubItem("Cuenta por Pagar", new CuentaPagarDG(this)));
            }
            if (LoggedTrabajador.acceso == 0)
            {
                menuActions.Add(new SubItem("Devolución", new DevolucionInicio(this)));
                menuActions.Add(new SubItem("Inventario", new InventarioDG(this)));
            }
            var item2 = new ItemMenu("Acciones", menuActions, PackIconKind.PointOfSale);

            var menuList = new List<SubItem>();
            menuList.Add(new SubItem("Artículos", new ActualizarArticuloDG(this)));
            menuList.Add(new SubItem("Ventas", new VentaDG(this)));
            menuList.Add(new SubItem("Compras", new CompraDG(this)));
            if (LoggedTrabajador.acceso == 0)
            {
                menuList.Add(new SubItem("Devoluciones", new DevolucionDG(this)));
            }
            menuList.Add(new SubItem("Articulos por Proveedor", new ProveedorArticuloDG(this)));
            var item3 = new ItemMenu("Listados", menuList, PackIconKind.AccountBoxes);

            var menuSetting = new List<SubItem>();
            if (LoggedTrabajador.acceso == 0)
            {
                menuSetting.Add(new SubItem("Auditoria", new AuditoriaDG(this)));
                menuSetting.Add(new SubItem("Respaldo", null, true, false, false, false));
                menuSetting.Add(new SubItem("Restauración", null, false, true, false, false));
            }
                menuSetting.Add(new SubItem("Ayuda", null, false, false, true, false));
            var item4 = new ItemMenu("Opciones", menuSetting, PackIconKind.Settings);

            var item5 = new ItemMenu("Cerrar Sesión", new AuditoriaDG(this), PackIconKind.Logout);

            Menu.Children.Add(new MenuItemX(item1, this));
            Menu.Children.Add(new MenuItemX(item2, this));
            if (LoggedTrabajador.acceso == 0 || LoggedTrabajador.acceso == 1)
            {
                Menu.Children.Add(new MenuItemX(item3, this));
            }
            Menu.Children.Add(new MenuItemX(item4, this));

            Menu.Children.Add(new MenuItemX(item5, this));
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

            var resp = System.Windows.MessageBox.Show("¿Desea Cerrar la Sesión?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (resp == MessageBoxResult.Yes)
            {
                DAuditoria auditoria = new DAuditoria(
                                        Globals.ID_SISTEMA,
                                        "Salir",
                                        "Ha Cerrado la Sesión"
                                     );
                new LAuditoria().Insertar(auditoria);

                logout = true;
                Login login = new Login();
                login.Show();
                this.Close();
            }
        }

        public void Backup()
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dlg.SelectedPath;
                LFunction.Backup(path);
            }
        }

        public void Restore()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "SQL SERVER database backup files|*.bak";
            dlg.Title = "Restaurar Base de Datos";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dlg.FileName;
                LFunction.Restore(path);
            }
        }

        public void AbrirManual()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            process.StartInfo = startInfo;

            startInfo.FileName = @"C:\manual\manual-3.pdf";


            if (Globals.ACCESO_SISTEMA == 0)
                startInfo.FileName = @"C:\manual-raimon\Manual_1.pdf";
            if (Globals.ACCESO_SISTEMA == 1)
                startInfo.FileName = @"C:\manual-raimon\Manual_2.pdf";
            if (Globals.ACCESO_SISTEMA == 2)
                startInfo.FileName = @"C:\manual-raimon\Manual_3.pdf";

            process.Start();
        }

        private void ButtonPopUpLogout_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!logout)
            {
                var resp = System.Windows.MessageBox.Show("¿Desea Cerrar la Aplicación?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (resp == MessageBoxResult.Yes)
                {
                    DAuditoria auditoria = new DAuditoria(
                                            Globals.ID_SISTEMA,
                                            "Salir",
                                            "Ha Cerrado la Sesión"
                                         );
                    new LAuditoria().Insertar(auditoria);

                    Environment.Exit(0);
                }

                e.Cancel = true;
            }
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
        public SubItem(string name, Page screen = null, bool backup = false, bool restore = false, bool help = false, bool iva = false)
        {
            Name = name;
            Screen = screen;
            Backup = backup;
            Restore = restore;
            IVA = iva;
            Help = help;
        }

        public string Name { get; private set; }
		public Page Screen { get; private set; }

        public bool Backup { get; private set; }

        public bool Restore { get; private set; }

        public bool IVA { get; private set; }

        public bool Help { get; private set; }
    }

}
