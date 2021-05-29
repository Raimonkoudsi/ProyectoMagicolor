using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
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

namespace ProyectoMagicolor.Vistas
{
    public partial class ArticuloFrm : Window
    {
        public CompraFrm ParentFrm;

        public TypeForm Type;

        public DArticulo DataFill;
        public DArticulo UForm;
        public LArticulo Metodos = new LArticulo();

        public DDetalle_Ingreso UFormPrecios;
        public LIngreso MetodosIngreso = new LIngreso();

        private string codigo = "";
        public string codigoParaEnviarCompra = "";


        public ArticuloFrm(CompraFrm parent = null, string codigo = "")
        {
            InitializeComponent();

            ParentFrm = parent;
            codigoParaEnviarCompra = codigo;

            txtCodigo.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtStockMinimo.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtStockMaximo.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtPrecioCompra.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
            txtPrecioVenta.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);

            LCategoria Mt = new LCategoria();
            var LCmt = Mt.Mostrar("");

            CbCategoria.ItemsSource = LCmt;
            CbCategoria.DisplayMemberPath = "nombre";
            CbCategoria.SelectedValuePath = "idCategoria";

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(Type == TypeForm.Read)
            {
                txtTitulo.Text = "Leer Artículo";
                fillForm(DataFill);
                SetEnable(false);
                btnEnviar.Content = "Sólo Lectura";

                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Mostrar",
                    "Ha Visto el Artículo Código " + codigo
                 );
                new LAuditoria().Insertar(auditoria);
            }
            else if(Type == TypeForm.Update)
            {
                SetEnable(true);

                txtTitulo.Text = "Editar Artículo";
                fillForm(DataFill);
            }
            if(ParentFrm != null)
            {
                txtCodigo.Text = codigoParaEnviarCompra;
                txtCodigo.IsEnabled = false;
                txtNombre.Focus();
            }
        }


        private void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            codigo = txtCodigo.Text;
            string nombre = txtNombre.Text;
            int idCategoria = (int)CbCategoria.SelectedValue;
            int stockminimo = int.Parse(txtStockMinimo.Text);
            int stockmaximo = txtStockMaximo.Text == "" ? 9999 : int.Parse(txtStockMaximo.Text);
            string descripcion = txtDescripcion.Text;

            UForm = new DArticulo(0, 
                                  codigo,
                                  nombre,
                                  descripcion,
                                  stockminimo,
                                  stockmaximo,
                                  idCategoria);

        }

        private void Create()
        {
            fillData();
            if (UForm == null)
                return;

            if (Metodos.Insertar(UForm).Equals("OK"))
            {
                int precioCompra = int.Parse(txtPrecioCompra.Text);
                int precioVenta = int.Parse(txtPrecioVenta.Text);

                UFormPrecios = new DDetalle_Ingreso(0,
                                    0,
                                    UForm.idArticulo,
                                    precioCompra,
                                    precioVenta,
                                    0,
                                    0);

                if (MetodosIngreso.InsertarDetallePrecios(UFormPrecios).Equals("OK"))
                {
                    DAuditoria auditoria = new DAuditoria(
                            Globals.ID_SISTEMA,
                            "Registrar",
                            "Ha Registrado el Artículo Código " + codigo
                    );
                    new LAuditoria().Insertar(auditoria);

                    if (ParentFrm != null)
                        ParentFrm.SetNuevoArticulo(UForm.codigo);

                    this.DialogResult = true;
                    this.Close();
                }
            }
        }

        private void Update()
        {
            fillData();
            if (UForm == null)
                return;
            UForm.idArticulo = DataFill.idArticulo;

            if (Metodos.Editar(UForm).Equals("OK"))
            {
                int precioCompra = int.Parse(txtPrecioCompra.Text);
                int precioVenta = int.Parse(txtPrecioVenta.Text);

                UFormPrecios = new DDetalle_Ingreso(0,
                                    0,
                                    DataFill.idArticulo,
                                    precioCompra,
                                    precioVenta,
                                    0,
                                    0);

                if (MetodosIngreso.InsertarDetallePrecios(UFormPrecios).Equals("OK"))
                {
                    DAuditoria auditoria = new DAuditoria(
                            Globals.ID_SISTEMA,
                            "Editar",
                            "Ha Editado el Artículo Código " + codigo
                     );
                    new LAuditoria().Insertar(auditoria);

                    this.DialogResult = true;
                    this.Close();
                }
            }
        }

        private void SetEnable(bool Enable)
        {
            txtCodigo.IsEnabled = false;
            txtNombre.IsEnabled = Enable;
            CbCategoria.IsEnabled = Enable;
            txtStockMinimo.IsEnabled = Enable;
            txtStockMaximo.IsEnabled = Enable;
            txtPrecioCompra.IsEnabled = Enable;
            txtPrecioVenta.IsEnabled = Enable;
            txtDescripcion.IsEnabled = Enable;
            btnCategoria.IsEnabled = Enable;
            btnEnviar.IsEnabled = Enable;
        }

        private void fillForm(DArticulo Data)
        {
            if(Data != null)
            {
                txtCodigo.Text = Data.codigo;
                txtNombre.Text = Data.nombre;
                CbCategoria.Text = Data.categoria;
                txtStockMinimo.Text = Data.stockMinimo.ToString();
                txtStockMaximo.Text = Data.stockMaximo.ToString();
                txtPrecioCompra.Text = Data.precioCompra.ToString();
                txtPrecioVenta.Text = Data.precioVenta.ToString();
                txtDescripcion.Text = Data.descripcion;
            }
        }

        public void SetCategoria(string nombre)
        {
            LCategoria Mt = new LCategoria();
            var LCmt = Mt.Mostrar("");

            CbCategoria.ItemsSource = LCmt;
            CbCategoria.DisplayMemberPath = "nombre";
            CbCategoria.SelectedValuePath = "idCategoria";

            CbCategoria.Text = nombre;
        }


        #region Validation
        bool Validate()
        {
            if (txtCodigo.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe colocar el código del artículo");
                txtCodigo.Focus();
                return true;
            }

            if (txtCodigo.Text.Length <= 5)
            {
                LFunction.MessageExecutor("Error", "El código debe ser mayor a 5 dígitos");
                txtCodigo.Focus();
                return true;
            }

            if (Type != TypeForm.Update)
            {
                if (Metodos.CodigoRepetido(txtCodigo.Text))
                {
                    LFunction.MessageExecutor("Error", "El código del artículo ya está registrado en el sistema");
                    txtCodigo.Focus();
                    return true;
                }
            }

            if (txtNombre.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe colocar el nombre del artículo");
                txtNombre.Focus();
                return true;
            }

            if (txtNombre.Text.Length <= 5)
            {
                LFunction.MessageExecutor("Error", "El nombre debe ser mayor a 5 carácteres");
                txtNombre.Focus();
                return true;
            }

            if (CbCategoria.SelectedIndex < 0)
            {
                LFunction.MessageExecutor("Error", "Debe seleccionar una categoría");
                CbCategoria.Focus();
                return true;
            }

            int minValue, maxValue, precioCompra, precioVenta;
            int.TryParse(txtStockMinimo.Text, out minValue);
            int.TryParse(txtStockMaximo.Text, out maxValue);
            int.TryParse(txtPrecioCompra.Text, out precioCompra);
            int.TryParse(txtPrecioVenta.Text, out precioVenta);

            if (txtStockMinimo.Text == "" || minValue == 0)
            {
                LFunction.MessageExecutor("Error", "Debe colocar un stock mínimo para el artículo");
                txtStockMinimo.Focus();
                return true;
            }

            if (minValue >= maxValue)
            {
                LFunction.MessageExecutor("Error", "El stock mínimo no puede ser mayor o igual al stock máximo");
                txtStockMinimo.Focus();
                return true;
            }

            if (txtPrecioCompra.Text == "" || precioCompra == 0)
            {
                LFunction.MessageExecutor("Error", "Debe colocar un precio de compra para el artículo");
                txtPrecioCompra.Focus();
                return true;
            }

            if (txtPrecioVenta.Text == "" || precioVenta == 0)
            {
                LFunction.MessageExecutor("Error", "Debe colocar un precio de venta para el artículo");
                txtPrecioVenta.Focus();
                return true;
            }

            if (precioCompra >= precioVenta)
            {
                LFunction.MessageExecutor("Error", "El precio de compra no puede ser mayor o igual al precio de venta");
                txtPrecioVenta.Focus();
                return true;
            }


            if (txtDescripcion.Text.Length <= 5 && txtDescripcion.Text != "")
            {
                LFunction.MessageExecutor("Error", "La descripción debe ser mayor a 5 carácteres");
                txtNombre.Focus();
                return true;
            }

            return false;
        }
        #endregion

        private void Categoria_Click(object sender, RoutedEventArgs e)
        {
            CategoriaFrm Frm = new CategoriaFrm(this);
            bool? res = Frm.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Type == TypeForm.Update)
                Update();
            else
                Create();
        }
    }
}
