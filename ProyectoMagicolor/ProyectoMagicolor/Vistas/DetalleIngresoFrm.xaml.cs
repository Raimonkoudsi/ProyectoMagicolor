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
    public partial class DetalleIngresoFrm : Window
    {

        CompraFrm ParentForm;

        public DetalleIngresoFrm(CompraFrm parentfrm, List<DDetalle_Ingreso> actualDetalle)
        {
            InitializeComponent();

            ParentForm = parentfrm;

            ActualDetalle = actualDetalle;

            txtPrecioCompra.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtPrecioVenta.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtCantidad.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
        }

        

        public TypeForm Type;
        public DDetalle_Ingreso DataFill;
        public DArticulo DataArticulo;

        public DDetalle_Ingreso UForm;

        public LArticulo Metodos = new LArticulo();

        public int idEdit;
        public List<DDetalle_Ingreso> ActualDetalle;

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
                fillForm(DataFill, DataArticulo);
                SetEnable(false);
                btnEnviar.Visibility = Visibility.Collapsed;
            }
            else if(Type == TypeForm.Update)
            {
                txtTitulo.Text = "Editar Articulo";
                fillForm(DataFill, DataArticulo);
            }
        }

       

        void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            int idArticulo = DataArticulo.idArticulo;
            double precioCompra = double.Parse(txtPrecioCompra.txt.Text);
            double precioVenta = double.Parse(txtPrecioVenta.txt.Text);
            int Cantidad = int.Parse(txtCantidad.txt.Text);


            UForm = new DDetalle_Ingreso(0,
                                         0,
                                         idArticulo,
                                         precioCompra,
                                         precioVenta,
                                         Cantidad,
                                         Cantidad);
        }

        void Create()
        {
            fillData();
            if (UForm == null)
                return;

            ParentForm.AgregarArticulo(UForm, DataArticulo);
            this.Close();
        }

        void Update()
        {
            fillData();
            if (UForm == null)
                return;
            //UForm.idArticulo = DataFill.idArticulo;
            ParentForm.EditarArticulo(UForm, DataArticulo, idEdit);
            this.Close();
        }


        void SetUpdate() // BOTON PARENT FORM
        {

        }

        bool ArticuloSetted = false;

        public void AgregarArticulo(DArticulo Articulo)
        {
            ArticuloSetted = true;
            DataArticulo = Articulo;

            GridArticulo.Visibility = Visibility.Visible;
            txtArticulo.Text = Articulo.nombre;
            txtArticuloCod.Text = Articulo.codigo;


            BtnAgregarArticulo.Content = "Cambiar";
        }

        public void QuitarArticulo()
        {
            ArticuloSetted = false;
            DataArticulo = null;

            GridArticulo.Visibility = Visibility.Collapsed;
            txtArticulo.Text = "";
            txtArticuloCod.Text = "";

            BtnAgregarArticulo.Content = "Buscar Artículo";
        }


        private void BtnAgregarArticulo_Click(object sender, RoutedEventArgs e)
        {
            if (!ArticuloSetted)
            {
                List<int> intentnt = new List<int>();

                foreach(DDetalle_Ingreso item in ActualDetalle)
                {
                    intentnt.Add(item.idArticulo);
                }

                ArticuloVista AVFrm = new ArticuloVista(intentnt, Type == TypeForm.Create);
                if ((bool)AVFrm.ShowDialog())
                {
                    AgregarArticulo(AVFrm.Resultado);
                }
            }
            else
            {
                QuitarArticulo();
            }
        }

        void SetEnable(bool Enable)
        {
            BtnAgregarArticulo.Visibility = Enable ? Visibility.Visible : Visibility.Collapsed;
            txtPrecioCompra.IsEnabled = Enable;
            txtPrecioVenta.IsEnabled = Enable;
            txtCantidad.IsEnabled = Enable;
        }
        void fillForm(DDetalle_Ingreso Data, DArticulo Articulo)
        {
            if(Data != null)
            {
                AgregarArticulo(DataArticulo);
                txtPrecioCompra.SetText(Data.precioCompra.ToString());
                txtPrecioVenta.SetText(Data.precioVenta.ToString());
                txtCantidad.SetText(Data.cantidadInicial.ToString());
            }
        }
        #region Validation
        bool Validate()
        {
            if (!txtPrecioCompra.Changed)
            {
                MessageBox.Show("Debes llenar el Precio Compra!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPrecioCompra.txt.Focus();
                return true;
            }

            if (!txtPrecioVenta.Changed)
            {
                MessageBox.Show("Debes llenar el campo Precio Venta!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPrecioVenta.txt.Focus();
                return true;
            }

            if (!txtCantidad.Changed)
            {
                MessageBox.Show("Debes llenar el campo Cantidad!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtCantidad.txt.Focus();
                return true;
            }

            if (!ArticuloSetted)
            {
                MessageBox.Show("Debes seleccionar un Articulo!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtArticulo.Focus();
                return true;
            }

            return false;
        }


        #endregion
    }
}
