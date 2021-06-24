using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class DetalleIngresoFrm : Window
    {
        CompraFrm ParentForm;

        public TypeForm Type;
        public DDetalle_Ingreso DataFill;
        public DArticulo DataArticulo;

        public DDetalle_Ingreso UForm;

        public LArticulo Metodos = new LArticulo();

        public int idEdit;
        public List<DDetalle_Ingreso> ActualDetalle;

        public static double PrecioCompra, PrecioVenta;

        public DetalleIngresoFrm(CompraFrm parentfrm, List<DDetalle_Ingreso> actualDetalle)
        {
            InitializeComponent();

            ParentForm = parentfrm;

            ActualDetalle = actualDetalle;

            txtPrecioCompra.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtPrecioVenta.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtCantidad.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
        }


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


        private void BtnRegistrarArticulo_Click(object sender, RoutedEventArgs e)
        {
            ArticuloFrm Frm = new ArticuloFrm(null, "", this);
            bool? res = Frm.ShowDialog();
        }


        private void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            int idArticulo = DataArticulo.idArticulo;
            double precioCompra = double.Parse(txtPrecioCompra.Text);
            double precioVenta = double.Parse(txtPrecioVenta.Text);
            int Cantidad = int.Parse(txtCantidad.Text);


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
            ParentForm.EditarArticulo(UForm, DataArticulo, idEdit);
            this.Close();
        }


        bool ArticuloSetted = false;

        public void AgregarArticulo(DArticulo Articulo = null)
        {
            if(Articulo != null)
            {
                ArticuloSetted = true;
                DataArticulo = Articulo;

                gridArticuloLleno.Visibility = Visibility.Visible;
                gridArticuloVacio.Visibility = Visibility.Collapsed;
                txtArticulo.Text = Articulo.nombre;
                txtArticuloCod.Text = Articulo.codigo;
                txtCantidadActual.Text = Articulo.cantidadActual.ToString();
                txtCantidadMaxima.Text = Articulo.stockMaximo.ToString();

                if (PrecioCompra != 0 && PrecioVenta != 0)
                {
                    txtPrecioCompra.Text = PrecioCompra.ToString();
                    txtPrecioVenta.Text = PrecioVenta.ToString();
                }

                BtnAgregarArticulo.Background = System.Windows.Media.Brushes.OrangeRed;
                BtnAgregarArticulo.BorderBrush = System.Windows.Media.Brushes.OrangeRed;

                BtnRegistrarArticulo.Visibility = Visibility.Collapsed;
                IconoBoton.Visibility = Visibility.Collapsed;

                NombreBoton.Text = "Cancelar Selección";
                IconoBoton.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cancel;
            }
            else
            {
                QuitarArticulo();
            }
        }

        public void QuitarArticulo()
        {
            ArticuloSetted = false;
            DataArticulo = null;

            gridArticuloLleno.Visibility = Visibility.Collapsed;
            gridArticuloVacio.Visibility = Visibility.Visible;
            txtArticulo.Text = "";
            txtArticuloCod.Text = "";

            BtnAgregarArticulo.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3194F7"));
            BtnAgregarArticulo.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3194F7"));

            BtnRegistrarArticulo.Visibility = Visibility.Visible;
            IconoBoton.Visibility = Visibility.Visible;

            NombreBoton.Text = "  Buscar Artículo  ";
            IconoBoton.Kind = MaterialDesignThemes.Wpf.PackIconKind.Search;
        }


        private void BtnAgregarArticulo_Click(object sender, RoutedEventArgs e)
        {
            if (!ArticuloSetted)
            {
                List<int> intentnt = new List<int>();

                if (ActualDetalle != null)
                {
                    foreach (DDetalle_Ingreso item in ActualDetalle)
                    {
                        intentnt.Add(item.idArticulo);
                    }
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

        private void SetEnable(bool Enable)
        {
            BtnAgregarArticulo.Visibility = Enable ? Visibility.Visible : Visibility.Collapsed;
            txtPrecioCompra.IsEnabled = Enable;
            txtPrecioVenta.IsEnabled = Enable;
            txtCantidad.IsEnabled = Enable;
        }

        private void fillForm(DDetalle_Ingreso Data, DArticulo Articulo)
        {
            if(Data != null)
            {
                AgregarArticulo(DataArticulo);
                txtPrecioCompra.Text = Data.precioCompra.ToString();
                txtPrecioVenta.Text = Data.precioVenta.ToString();
                txtCantidad.Text = Data.cantidadInicial.ToString();
            }
        }


        #region Validation
        bool Validate()
        {
            if (txtPrecioCompra.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar el precio de compra");
                txtPrecioCompra.Focus();
                return true;
            }

            if (Int32.Parse(txtPrecioCompra.Text) <= 0)
            {
                LFunction.MessageExecutor("Error", "El precio de la compra no puede ser 0");
                txtPrecioCompra.Focus();
                return true;
            }

            if (txtPrecioVenta.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar el precio de venta");
                txtPrecioVenta.Focus();
                return true;
            }


            if (Int32.Parse(txtPrecioVenta.Text) <= 0)
            {
                LFunction.MessageExecutor("Error", "El precio de la venta no puede ser 0");
                txtPrecioCompra.Focus();
                return true;
            }

            if (Int32.Parse(txtPrecioVenta.Text) <= Int32.Parse(txtPrecioCompra.Text))
            {
                LFunction.MessageExecutor("Error", "El precio de la venta no puede ser menor a la de compra");
                txtPrecioVenta.Focus();
                return true;
            }

            if (txtCantidad.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar la cantidad de la compra");
                txtCantidad.Focus();
                return true;
            }

            if (Int32.Parse(txtCantidad.Text) <= 0)
            {
                LFunction.MessageExecutor("Error", "La cantidad de la compra no puede ser 0");
                txtCantidad.Focus();
                return true;
            }

            if ((Int32.Parse(txtCantidad.Text) + Int32.Parse(txtCantidadActual.Text)) > Int32.Parse(txtCantidadMaxima.Text))
            {
                LFunction.MessageExecutor("Error", "El artículo no puede sobrepasarse de la cantidad máxima");
                txtCantidad.Focus();
                return true;
            }

            if (!ArticuloSetted)
            {
                LFunction.MessageExecutor("Error", "Debe seleccionar un articulo");
                txtArticulo.Focus();
                return true;
            }

            return false;
        }


        #endregion
    }
}
