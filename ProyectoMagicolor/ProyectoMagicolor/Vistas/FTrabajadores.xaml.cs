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
using System.Windows.Shapes;

using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    /// <summary>
    /// Interaction logic for FTrabajadores.xaml
    /// </summary>
    public partial class FTrabajadores : Window
    {
        public FTrabajadores()
        {
            InitializeComponent();
        }
        
        public Border CreateCell(string text)
        {

            Border bord = new Border();
            bord.BorderBrush = Brushes.White;

            TextBlock txtblock = new TextBlock();
            txtblock.Text = text;
            txtblock.Foreground = Brushes.White;
            txtblock.FontSize = 8;
            txtblock.HorizontalAlignment = HorizontalAlignment.Center;
            txtblock.VerticalAlignment = VerticalAlignment.Center;
            txtblock.Margin = new Thickness(0, 10, 0, 10);
            txtblock.TextWrapping = TextWrapping.Wrap;

            bord.Child = txtblock;

            return bord;
        }

        public Grid CreateRow(string cedula, string nombre, string apellidos, string direccion, string telefono, string email, string usuario, bool LastRow)
        {
            Grid grd = new Grid();

            for(int i = 0; i < 8; i++)
            {
                grd.ColumnDefinitions.Add(new ColumnDefinition() { 
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            Border brd1 = CreateCell(cedula);
            brd1.SetValue(Grid.ColumnProperty, 0);
            brd1.BorderThickness = new Thickness(0, 1, 0, LastRow ? 1 : 0);
            grd.Children.Add(brd1);

            Border brd2 = CreateCell(nombre);
            brd2.SetValue(Grid.ColumnProperty, 1);
            brd2.BorderThickness = new Thickness(1, 1, 0, LastRow ? 1 : 0);
            grd.Children.Add(brd2);

            Border brd3 = CreateCell(apellidos);
            brd3.SetValue(Grid.ColumnProperty, 2);
            brd3.BorderThickness = new Thickness(1, 1, 0, LastRow ? 1 : 0);
            grd.Children.Add(brd3);

            Border brd4 = CreateCell(direccion);
            brd4.SetValue(Grid.ColumnProperty, 3);
            brd4.BorderThickness = new Thickness(1, 1, 0, LastRow ? 1 : 0);
            grd.Children.Add(brd4);

            Border brd5 = CreateCell(telefono);
            brd5.SetValue(Grid.ColumnProperty, 4);
            brd5.BorderThickness = new Thickness(1, 1, 0, LastRow ? 1 : 0);
            grd.Children.Add(brd5);

            Border brd6 = CreateCell(email);
            brd6.SetValue(Grid.ColumnProperty, 5);
            brd6.BorderThickness = new Thickness(1, 1, 0, LastRow ? 1 : 0);
            grd.Children.Add(brd6);

            Border brd7 = CreateCell(usuario);
            brd7.SetValue(Grid.ColumnProperty, 6);
            brd7.BorderThickness = new Thickness(1, 1, 0, LastRow ? 1 : 0);
            grd.Children.Add(brd7);

            return grd;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            contentsp.Children.Clear();

            int i = 0;

            LTrabajador metodosUsuario = new LTrabajador();

            foreach (DTrabajador it in metodosUsuario.Mostrar(""))
            {
                contentsp.Children.Add(CreateRow(it.cedula,
                                                it.nombre,
                                                it.apellidos,
                                                it.direccion,
                                                it.telefono,
                                                it.email,
                                                it.usuario,
                                                i == metodosUsuario.Mostrar("").Count - 1));
                i++;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormTrabajadores frmTrab = new FormTrabajadores();
            frmTrab.ShowDialog();
        }
    }

}
