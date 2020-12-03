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

namespace ProyectoMagicolor.Vistas
{
    /// <summary>
    /// Interaction logic for FTrabajadores.xaml
    /// </summary>
    public partial class FTrabajadores : Window
    {
        public List<Trabajador> Trabajadores = new List<Trabajador>();
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

        public Grid CreateRow(string id, string nombre, string apellidos, string sexo, string FechaNacimiento, string cedula, string direccion, string telefono, bool LastRow)
        {
            Grid grd = new Grid();

            for(int i = 0; i < 8; i++)
            {
                grd.ColumnDefinitions.Add(new ColumnDefinition() { 
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            Border brd1 = CreateCell(id);
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

            Border brd4 = CreateCell(sexo);
            brd4.SetValue(Grid.ColumnProperty, 3);
            brd4.BorderThickness = new Thickness(1, 1, 0, LastRow ? 1 : 0);
            grd.Children.Add(brd4);

            Border brd5 = CreateCell(FechaNacimiento);
            brd5.SetValue(Grid.ColumnProperty, 4);
            brd5.BorderThickness = new Thickness(1, 1, 0, LastRow ? 1 : 0);
            grd.Children.Add(brd5);

            Border brd6 = CreateCell(cedula);
            brd6.SetValue(Grid.ColumnProperty, 5);
            brd6.BorderThickness = new Thickness(1, 1, 0, LastRow ? 1 : 0);
            grd.Children.Add(brd6);

            Border brd7 = CreateCell(direccion);
            brd7.SetValue(Grid.ColumnProperty, 6);
            brd7.BorderThickness = new Thickness(1, 1, 0, LastRow ? 1 : 0);
            grd.Children.Add(brd7);

            Border brd8 = CreateCell(telefono);
            brd8.SetValue(Grid.ColumnProperty, 7);
            brd8.BorderThickness = new Thickness(1, 1, 0, LastRow ? 1 : 0);
            grd.Children.Add(brd8);

            return grd;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            contentsp.Children.Clear();

            Trabajadores.Clear();

            Trabajadores.Add(new Trabajador()
            {
                id = 0,
                nombre = "José",
                apellidos = "Pereira",
                sexo = "M",
                FechaNacimiento = "18/06/2002",
                cedula = "26866008",
                direccion = "Maracay",
                telefono = "04243094204"
            });
            Trabajadores.Add(new Trabajador()
            {
                id = 1,
                nombre = "Raimon",
                apellidos = "Koudsi",
                sexo = "M",
                FechaNacimiento = "18/06/2002",
                cedula = "26866008",
                direccion = "Maracay",
                telefono = "04243094204"
            });
            int i = 0;
            foreach(Trabajador it in Trabajadores)
            {
                contentsp.Children.Add(CreateRow(it.id.ToString(),
                                                it.nombre,
                                                it.apellidos,
                                                it.sexo,
                                                it.FechaNacimiento,
                                                it.cedula,
                                                it.direccion,
                                                it.direccion,
                                                i == Trabajadores.Count - 1));
                i++;
            }
        }
    }
    public class Trabajador
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public string sexo { get; set; }
        public string FechaNacimiento { get; set; }
        public string cedula { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
    }
}
