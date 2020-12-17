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
    /// Interaction logic for FormTrabajadores.xaml
    /// </summary>
    public partial class FormTrabajadores : Window
    {
        public FormTrabajadores()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DTrabajador trabajador = new DTrabajador(0,
                                                    txt1.Text,
                                                    txt2.Text,
                                                    txt3.Text,
                                                    txt4.DisplayDate,
                                                    txt5.Text,
                                                    txt6.Text,
                                                    txt7.Text,
                                                    txt8.Text,
                                                    txt9.Text,
                                                    txt10.Text,
                                                    txt11.Text,
                                                    txt12.Text,
                                                    txt13.Text,
                                                    txt14.Text);

            LTrabajador lTrabajador = new LTrabajador();

            lTrabajador.Insertar(trabajador); 
        }
    }
}
