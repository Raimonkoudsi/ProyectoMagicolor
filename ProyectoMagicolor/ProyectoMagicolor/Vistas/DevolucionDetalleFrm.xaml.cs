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

namespace ProyectoMagicolor.Vistas
{
    /// <summary>
    /// Interaction logic for FormTrabajadores.xaml
    /// </summary>
    public partial class DevolucionDetalleFrm : Window
    {


        public DevolucionDetalleFrm(DevolucionFrm par, ModeloDevolucion modelo)
        {
            InitializeComponent();

            txtcantidad.txt.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);

            ParentFrm = par;
            DataFill = modelo;
        }

        
        public DevolucionFrm ParentFrm;
        public TypeForm Type;
        public ModeloDevolucion DataFill;

        public ModeloDevolucion UForm;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Create();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fillForm(DataFill);
        }

        void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            int CantidadDevuelta = int.Parse(txtcantidad.txt.Text);
            bool Dañado = RBDamaged.IsChecked ?? false;

            UForm = new ModeloDevolucion(DataFill.Id,
                                         DataFill.NombreArticulo,
                                         DataFill.Precio,
                                         DataFill.CantidadComprada,
                                         CantidadDevuelta,
                                         DataFill.CantidadComprada - CantidadDevuelta,
                                         Dañado);
        }

        void Create()
        {
            fillData();
            if (UForm == null)
                return;
            
            ParentFrm.DevolucionArticulo(UForm);
            this.DialogResult = true;
            this.Close();
        }

        void SetEnable(bool Enable)
        {
            txtcantidad.IsEnabled = Enable;
            RBDamaged.IsEnabled = Enable;
        }
        void fillForm(ModeloDevolucion Data)
        {
            if(Data != null)
            {
                txtNombreArticulo.Text = Data.NombreArticulo;
                txtCantidadComprada.Text = Data.CantidadComprada.ToString();
                int CantidadDevolver = Data.CantidadDevuelta;
                txtcantidad.SetText(CantidadDevolver > 0 ? CantidadDevolver.ToString() : "");
                RBDamaged.IsChecked = Data.Dañado;
            }
        }
        #region Validation
        bool Validate()
        {
            if (!txtcantidad.Changed)
            {
                MessageBox.Show("Debes llenar el campo Cantidad!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtcantidad.txt.Focus();
                return true;
            }
            if (int.Parse(txtcantidad.txt.Text) <= 0)
            {
                MessageBox.Show("El campo Cantidad no puede ser cero ni negativo!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtcantidad.txt.Focus();
                return true;
            }
            if (int.Parse(txtcantidad.txt.Text) > DataFill.CantidadComprada)
            {
                MessageBox.Show("El campo Cantidad a Devolver no puede ser mayor a la Cantidad Comprada!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtcantidad.txt.Focus();
                return true;
            }

            return false;
        }

        #endregion

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ParentFrm.CancelarDevolucion(DataFill.Id);
            this.DialogResult = true;
            this.Close();
        }

        private void txtcantidad_KeyUp(object sender, KeyEventArgs e)
        {
            int CantidadDevolver = 0;
            if(txtcantidad.txt.Text != "")
            {
                CantidadDevolver = int.Parse(txtcantidad.txt.Text);
                if (CantidadDevolver <= 0)
                    CantidadDevolver = 0;
            }

            int cantidadRestante = DataFill.CantidadComprada - CantidadDevolver;

            txtCantidadRestante.Text = cantidadRestante.ToString();
        }
    }
}
