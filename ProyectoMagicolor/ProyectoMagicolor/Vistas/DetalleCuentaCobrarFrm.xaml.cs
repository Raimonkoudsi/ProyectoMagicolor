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
            txtMonto.txt.Text = DataFill.montoTotal.ToString();

            total = true;
            Create(true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            txtIdVenta.Text = DataFill.idVenta.ToString();
            txtCliente.Text = DataFill.cliente;
            txtCedula.Text = DataFill.cedulaCliente;
            txtFecha.Text = DataFill.fecha.ToString();
            txtMontoRestante.Text = DataFill.monto.ToString();
            txtMontoTotal.Text = DataFill.montoTotal.ToString();

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
                monto = double.Parse(txtMonto.txt.Text);
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
                rpta = MessageBox.Show("Desea Cancelar el Monto de " + txtMonto.txt.Text + " $ para dejar un restante de " + (DataFill.montoTotal - double.Parse(txtMonto.txt.Text)).ToString() + "$ ?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
            }

            if (rpta == MessageBoxResult.No)
                return;

            string ok = Metodos.RegistrarCxC(UForm, DataFill.idCuentaCobrar);

            if (ok.Equals("OK"))
            {
                if (total)
                {
                    MessageBox.Show("Pago Completado!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Abono Completado!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            this.Close();
        }



        bool Validate()
        {
            if ((txtMonto.txt.Text == "Monto a Abonar" || double.Parse(txtMonto.txt.Text) <= 0) && !total)
            {
                MessageBox.Show("Debe Agregar un Monto para Abonar!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtMonto.Focus();
                return true;
            }
            else if (double.Parse(txtMonto.txt.Text) > DataFill.montoTotal && !total)
            {
                MessageBox.Show("El Monto no debe Exceder la Deuda!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtMonto.Focus();
                return true;
            }

            return false;

        }
    }
}
