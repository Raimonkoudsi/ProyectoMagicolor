using System.Windows;
using System.Windows.Input;

namespace ProyectoMagicolor.Vistas
{
    public partial class DevolucionDetalleFrm : Window
    {


        public DevolucionDetalleFrm(DevolucionFrm par, ModeloDevolucion modelo)
        {
            InitializeComponent();

            txtCantidad.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);

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

            int CantidadDevolver = 0;
            if (txtCantidad.Text != "")
            {
                CantidadDevolver = int.Parse(txtCantidad.Text);
                if (CantidadDevolver <= 0)
                    CantidadDevolver = 0;
            }

            int cantidadRestante = DataFill.CantidadComprada - CantidadDevolver;
            txtCantidadRestante.Text = cantidadRestante.ToString();
        }

        void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            int CantidadDevuelta = int.Parse(txtCantidad.Text);
            int CantidadRestante = DataFill.CantidadComprada - CantidadDevuelta;
            bool Dañado = RBDamaged.IsChecked ?? false;

            DataFill.CantidadDevuelta = CantidadDevuelta;
            DataFill.CantidadRestante = CantidadRestante;
            DataFill.Dañado = Dañado;


            UForm = DataFill;
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
            txtCantidad.IsEnabled = Enable;
            RBDamaged.IsEnabled = Enable;
        }
        void fillForm(ModeloDevolucion Data)
        {
            if(Data != null)
            {
                txtNombreArticulo.Text = Data.NombreArticulo;
                txtCantidadComprada.Text = Data.CantidadComprada.ToString();
                int CantidadDevolver = Data.CantidadDevuelta;
                txtCantidad.Text = (CantidadDevolver > 0 ? CantidadDevolver.ToString() : "");
                RBDamaged.IsChecked = Data.Dañado;
            }
        }

        #region Validation
        bool Validate()
        {
            if (txtCantidad.Text == "")
            {
                MessageBox.Show("Debe ingresar la cantidad", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtCantidad.Focus();
                return true;
            }
            if (int.Parse(txtCantidad.Text) <= 0)
            {
                MessageBox.Show("La cantidad no puede ser 0", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtCantidad.Focus();
                return true;
            }
            if (int.Parse(txtCantidad.Text) > DataFill.CantidadComprada)
            {
                MessageBox.Show("La cantidad a devolver no puede ser mayor a la comprada", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtCantidad.Focus();
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
            if(txtCantidad.Text != "")
            {
                CantidadDevolver = int.Parse(txtCantidad.Text);
                if (CantidadDevolver <= 0)
                    CantidadDevolver = 0;
            }

            int cantidadRestante = DataFill.CantidadComprada - CantidadDevolver;

            txtCantidadRestante.Text = cantidadRestante.ToString();
        }
    }
}
