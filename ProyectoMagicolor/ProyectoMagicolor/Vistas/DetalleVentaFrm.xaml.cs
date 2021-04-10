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
    public partial class DetalleVentaFrm : Window
    {

        VentaFrm ParentForm;

        public DetalleVentaFrm(VentaFrm parentfrm, List<DDetalle_Venta> actualDetalle)
        {
            InitializeComponent();

            ParentForm = parentfrm;

            ActualDetalle = actualDetalle;

            txtCantidad.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
        }

        

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

            txtCantidad.txt.Focus();

            if (OpenProducts)
            {
                AbrirIngresos();
            }
        }

       

        void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            int id = DataDIngreso.idDetalleIngreso;
            int Cantidad = int.Parse(txtCantidad.txt.Text);


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
            ArticuloSetted = true;
            DataDIngreso = DDI;
            DataArticulo = Articulo;

            GridArticulo.Visibility = Visibility.Visible;
            txtArticulo.Text = Articulo.nombre;
            txtArtPrecio.Text = DDI.precioVenta.ToString("0.00");
            txtStock.Text = "En Stock: " + DDI.cantidadActual;


            BtnAgregarArticulo.Content = "Cambiar";
        }

        public void QuitarArticulo()
        {
            ArticuloSetted = false;
            DataDIngreso = null;
            DataArticulo = null;

            GridArticulo.Visibility = Visibility.Collapsed;
            txtArticulo.Text = "";
            txtArtPrecio.Text = "";
            txtStock.Text = "";

            BtnAgregarArticulo.Content = "Buscar Artículo";
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
                txtCantidad.SetText(Data.cantidad.ToString());
            }
        }
        #region Validation
        bool Validate()
        {
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

            if(DataDIngreso.cantidadActual < int.Parse(txtCantidad.txt.Text))
            {
                MessageBox.Show("No puedes superar la cantidad que hay en Stock!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtCantidad.txt.Focus();
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
