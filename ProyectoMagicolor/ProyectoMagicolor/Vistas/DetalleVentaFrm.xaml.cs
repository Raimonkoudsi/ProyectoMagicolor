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
    public partial class DetalleVentaFrm : Window
    {

        VentaFrm ParentForm;

        public TypeForm Type;
        public DDetalle_Venta DataFill;

        //Datos seleccionados
        public DArticulo DataArticulo;
        public DDetalle_Ingreso DataDIngreso;

        public DDetalle_Venta UForm;

        public LArticulo Metodos = new LArticulo();

        public bool OpenProducts;
        public int idEdit;
        public List<DDetalle_Venta> ActualDetalle;

        public DetalleVentaFrm(VentaFrm parentfrm, List<DDetalle_Venta> actualDetalle)
        {
            InitializeComponent();

            ParentForm = parentfrm;

            ActualDetalle = actualDetalle;

            txtCantidad.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Type == TypeForm.Read)
            {
                txtTitulo.Text = "Leer Articulo";
                fillForm(DataFill, DataArticulo);
                SetEnable(false);
                btnEnviar.Visibility = Visibility.Collapsed;
            }
            else if (Type == TypeForm.Update)
            {
                txtTitulo.Text = "Editar Articulo";
                fillForm(DataFill, DataArticulo);
            }

            txtCantidad.Focus();

            if (OpenProducts)
            {
                AbrirIngresos();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Send();
        }

        public void Send()
        {
            if (Type == TypeForm.Update)
                Update();
            else
                Create();
        }

       

        void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            int id = DataDIngreso.idDetalleIngreso;
            int Cantidad = int.Parse(txtCantidad.Text);


            UForm = new DDetalle_Venta( 0,
                                        0,
                                        id,
                                        Cantidad,
                                        DataDIngreso.precioVenta,
                                        0,0);
            UForm.idArticulo = DataArticulo.idArticulo;
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

        bool ArticuloSetted = false;

        public void AgregarArticulo(DDetalle_Ingreso DDI,DArticulo Articulo)
        {
            if (Articulo != null)
            {
                ArticuloSetted = true;
                DataDIngreso = DDI;
                DataArticulo = Articulo;

                gridArticuloLleno.Visibility = Visibility.Visible;
                gridArticuloVacio.Visibility = Visibility.Collapsed;
                txtArticulo.Text = Articulo.nombre;
                txtArticuloCod.Text = Articulo.codigo;
                txtCantidadActual.Text = Articulo.cantidadActual.ToString();
                txtPrecio.Text = Articulo.precioVenta.ToString() + " Bs S";

                BtnAgregarArticulo.Background = System.Windows.Media.Brushes.OrangeRed;
                BtnAgregarArticulo.BorderBrush = System.Windows.Media.Brushes.OrangeRed;

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

            IconoBoton.Visibility = Visibility.Visible;

            NombreBoton.Text = "  Buscar Artículo  ";
            IconoBoton.Kind = MaterialDesignThemes.Wpf.PackIconKind.Search;
        }

        public void AbrirIngresos()
        {
            List<int> intentnt = new List<int>();

            foreach (DDetalle_Venta item in ActualDetalle)
            {
                intentnt.Add(item.idArticulo);
            }

            DetalleIngresoVista AVFrm = new DetalleIngresoVista(intentnt, Type == TypeForm.Create);
            if ((bool)AVFrm.ShowDialog())
            {
                LIngreso Met = new LIngreso();
                var Resp = Met.EncontrarByArticulo(AVFrm.Resultado.idArticulo)[0];

                AgregarArticulo(Resp, AVFrm.Resultado);
            }
        }


        private void BtnAgregarArticulo_Click(object sender, RoutedEventArgs e)
        {
            if (!ArticuloSetted)
            {
                AbrirIngresos();
            }
            else
            {
                QuitarArticulo();
            }
        }

        void SetEnable(bool Enable)
        {
            BtnAgregarArticulo.Visibility = Enable ? Visibility.Visible : Visibility.Collapsed;
            txtCantidad.IsEnabled = Enable;
        }
        void fillForm(DDetalle_Venta Data, DArticulo Articulo)
        {
            if(Data != null)
            {
                AgregarArticulo(DataDIngreso, DataArticulo);
                txtCantidad.Text = Data.cantidad.ToString();
            }
        }
        #region Validation
        bool Validate()
        {
            if (txtCantidad.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe ingresar la cantidad de la compra");
                txtCantidad.Focus();
                return true;
            }

            if (!ArticuloSetted)
            {
                LFunction.MessageExecutor("Error", "Debe seleccionar un articulo");
                txtArticulo.Focus();
                return true;
            }

            if(DataDIngreso.cantidadActual < int.Parse(txtCantidad.Text))
            {
                LFunction.MessageExecutor("Error", "La cantidad de venta no puede superar la cantidad que hay disponible");
                txtCantidad.Focus();
                return true;
            }

            return false;
        }


        #endregion

        private void StackPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Send();
            }
        }
    }
}
