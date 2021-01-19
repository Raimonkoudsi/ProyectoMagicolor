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

namespace ProyectoMagicolor.Vistas
{
    /// <summary>
    /// Interaction logic for TextInput.xaml
    /// </summary>
    public partial class TextInput : UserControl
    {
        public TextInput()
        {
            InitializeComponent();
            this.DataContext = this;
            Text = "";
            
        }

        public void SetText(string text)
        {
            Changed = text != "";
            if(!Changed)
            {
                txt.Foreground = Brushes.Gray;
                txt.Text = Placeholder;
            }
            else
            {
                txt.Foreground = Brushes.Black;
                txt.Text = text;
            }
        }

        public string Placeholder { get; set; }

        string _Placeholder;

        public string Text { get; set; }

        public bool Changed = false;

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            txt.Foreground = Brushes.Black;
            if(!Changed)
                txt.Text = "";
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //Changed = txt.Text != "";
            if (!Changed)
            {
                txt.Foreground = Brushes.Gray;
                txt.Text = _Placeholder;
            }
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = txt.Text;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _Placeholder = Placeholder;
        }

        private void txt_KeyUp(object sender, KeyEventArgs e)
        {
            Changed = txt.Text != "";
        }
    }
}
