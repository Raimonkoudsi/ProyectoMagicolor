using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.Win32;

namespace ProyectoMagicolor.Vistas
{
    /// <summary>
    /// Interaction logic for FormTrabajadores.xaml
    /// </summary>
    public partial class ArticuloFrm : Window
    {


        public ArticuloFrm()
        {
            InitializeComponent();
            txtStockMinimo.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtStockMaximo.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);

            LCategoria Mt = new LCategoria();

            var LCmt = Mt.Mostrar("");

            CbCategoria.ItemsSource = LCmt;
            CbCategoria.DisplayMemberPath = "nombre";
            CbCategoria.SelectedValuePath = "idCategoria";
            //txtCodigo.KeyDown += new KeyEventHandler(Validaciones.TextBox_CheckSpace);
        }

        

        public TypeForm Type;
        public DArticulo DataFill;

        public DArticulo UForm;

        public LArticulo Metodos = new LArticulo();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Type == TypeForm.Update)
                Update();
            else
                Create();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(Type == TypeForm.Read)
            {
                txtTitulo.Text = "Leer Articulo";
                fillForm(DataFill);
                SetEnable(false);
                btnEnviar.Visibility = Visibility.Collapsed;
            }
            else if(Type == TypeForm.Update)
            {
                txtTitulo.Text = "Editar Articulo";
                fillForm(DataFill);
            }
        }

       

        void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            string codigo = txtCodigo.txt.Text;
            string nombre = txtNombre.txt.Text;
            int idCategoria = (int)CbCategoria.SelectedValue;
            int stockminimo = int.Parse(txtStockMinimo.txt.Text);
            int stockmaximo = int.Parse(txtStockMaximo.txt.Text);
            string descripcion = txtDescripcion.Text;

            UForm = new DArticulo(0, 
                                  codigo,
                                  nombre,
                                  descripcion,
                                  stockminimo,
                                  stockmaximo,
                                  idCategoria);
        }

        void Create()
        {
            fillData();
            if (UForm == null)
                return;

            if (Metodos.Insertar(UForm).Equals("OK"))
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        void Update()
        {
            fillData();
            if (UForm == null)
                return;
            UForm.idArticulo = DataFill.idArticulo;
            Metodos.Editar(UForm);
            //MessageBox.Show(response);
            //if(response == "OK")
            //{
            //    this.DialogResult = true;
            //    this.Close();
            //}
        }

        private void PlaceDescripcion_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtDescripcion.Text == "")
            {
                PlaceDescripcion.Text = "";
            }
        }

        private void PlaceDescripcion_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDescripcion.Text == "")
            {
                PlaceDescripcion.Text = "Descripción";
            }
        }


        private void CbCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbCategoria.SelectedIndex > -1)
                PlaceCategoria.Text = "";
            else
                PlaceCategoria.Text = "Sexo";
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        void SetEnable(bool Enable)
        {
            txtCodigo.IsEnabled = Enable;
            txtNombre.IsEnabled = Enable;
            CbCategoria.IsEnabled = Enable;
            txtStockMinimo.IsEnabled = Enable;
            txtStockMaximo.IsEnabled = Enable;
            txtDescripcion.IsEnabled = Enable;
        }
        void fillForm(DArticulo Data)
        {
            if(Data != null)
            {
                txtCodigo.SetText(Data.codigo);
                txtNombre.SetText(Data.nombre);
                CbCategoria.SelectedValue = Data.idCategoria;
                txtStockMinimo.SetText(Data.stockMinimo.ToString());
                txtStockMaximo.SetText(Data.stockMaximo.ToString());
                txtDescripcion.Text = Data.descripcion;
                PlaceDescripcion.Text = "";
            }
        }
        #region Validation
        bool Validate()
        {
            if (!txtCodigo.Changed)
            {
                MessageBox.Show("Debes llenar el Código!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtCodigo.txt.Focus();
                return true;
            }

            if (!txtNombre.Changed)
            {
                MessageBox.Show("Debes llenar el campo Nombre!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtNombre.txt.Focus();
                return true;
            }

            if(CbCategoria.SelectedIndex < 0)
            {
                MessageBox.Show("Debes Seleccionar una Categoría!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                CbCategoria.Focus();
                return true;
            }

            if (!txtStockMinimo.Changed)
            {
                MessageBox.Show("Debes llenar el campo Stock Minimo!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStockMinimo.txt.Focus();
                return true;
            }

            if (!txtStockMaximo.Changed)
            {
                MessageBox.Show("Debes llenar el campo Stock Maximo!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStockMaximo.txt.Focus();
                return true;
            }

            if (txtCodigo.txt.Text.Contains(" "))
            {
                MessageBox.Show("El campo Código no puede tener espacios!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtCodigo.txt.Focus();
                return true;
            }

            return false;
        }


        #endregion

    }
}
