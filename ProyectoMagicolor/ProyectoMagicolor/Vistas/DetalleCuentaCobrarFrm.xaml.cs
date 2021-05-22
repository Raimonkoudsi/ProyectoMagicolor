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
using Microsoft.Win32;


namespace ProyectoMagicolor.Vistas
{
    /// <summary>
    /// Lógica de interacción para DetalleCuentaCobrarFrm.xaml
    /// </summary>
    public partial class DetalleCuentaCobrarFrm : Window
    {
        CuentaCobrarDG ParentForm;

        public DetalleCuentaCobrarFrm(CuentaCobrarDG parentfrm, List<DRegistro_CuentaCobrar> actualDetalle)
        {
            InitializeComponent();

            ParentForm = parentfrm;

            ActualDetalle = actualDetalle;

            txtMonto.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
        }


        public DVenta DataFill;
        //public DCuentaPagar DataCxP;

        public DRegistro_CuentaCobrar UForm;

        public LCuentaCobrar Metodos = new LCuentaCobrar();

        public int idEdit;
        public bool total;
        public List<DRegistro_CuentaCobrar> ActualDetalle;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            total = false;
            Create(false);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            txtMonto.Text = DataFill.montoTotal.ToString();

            total = true;
            Create(true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            txtIdVenta.Text = "ID " + DataFill.idVenta.ToString();
            txtFecha.Text = DataFill.fecha.ToShortDateString();
            txtCliente.Text = DataFill.cliente;
            txtCedula.Text = DataFill.cedulaCliente;
            txtMontoRestante.Text = "Total " + DataFill.monto.ToString();
            txtMontoTotal.Text = "Restante " + DataFill.montoTotal.ToString();

        }

        void fillData(bool total)
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            int idCuentaCobrar = DataFill.idCuentaCobrar;

            double monto = 0F;

            if (!total)
                monto = double.Parse(txtMonto.Text);
            else
                monto = DataFill.montoTotal;


            UForm = new DRegistro_CuentaCobrar(0,
                                         idCuentaCobrar,
                                         monto,
                                         DateTime.Now);
        }

        void Create(bool total)
        {
            fillData(total);
            if (UForm == null)
                return;

            MessageBoxResult rpta;

            if (total)
            {
                rpta = MessageBox.Show("Desea Cancelar el Monto Total Restante de " + DataFill.montoTotal + " $ ?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
            }
            else
            {
                rpta = MessageBox.Show("Desea Cancelar el Monto de " + txtMonto.Text + " $ para dejar un restante de " + (DataFill.montoTotal - double.Parse(txtMonto.Text)).ToString() + "$ ?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
            }

            if (rpta == MessageBoxResult.No)
                return;

            string abonarCC = Metodos.RegistrarCxC(UForm, DataFill.idCuentaCobrar);

            if (abonarCC.Equals("TOTAL"))
            {
                MessageBox.Show("Pago Completado!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (abonarCC.Equals("PARCIAL"))
            {
                MessageBox.Show("Abono Ingresado!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            this.Close();
        }



        bool Validate()
        {
            if ((txtMonto.Text == "Monto a Abonar" || double.Parse(txtMonto.Text) <= 0) && !total)
            {
                MessageBox.Show("Debe Agregar un Monto para Abonar!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtMonto.Focus();
                return true;
            }
            else if (double.Parse(txtMonto.Text) > DataFill.montoTotal && !total)
            {
                MessageBox.Show("El Monto no debe Exceder la Deuda!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtMonto.Focus();
                return true;
            }

            return false;

        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtMonto.Text == "")
            {
                txtBucarPlaceH.Text = "";
            }

        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMonto.Text == "")
            {
                txtBucarPlaceH.Text = "Monto a Abonar";
            }

        }
    }
}
