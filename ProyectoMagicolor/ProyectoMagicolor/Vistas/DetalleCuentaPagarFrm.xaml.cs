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
    public partial class DetalleCuentaPagarFrm : Window
    {
        CuentaPagarDG ParentForm;

        public DetalleCuentaPagarFrm(CuentaPagarDG parentfrm, List<DRegistro_CuentaPagar> actualDetalle)
        {
            InitializeComponent();

            ParentForm = parentfrm;

            ActualDetalle = actualDetalle;

            txtMonto.KeyDown += new KeyEventHandler(Validaciones.TextBox_KeyDown);
        }


        public DIngreso DataFill;

        public DRegistro_CuentaPagar UForm;

        public LCuentaPagar Metodos = new LCuentaPagar();

        public int idEdit;
        public bool total;
        public List<DRegistro_CuentaPagar> ActualDetalle;


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

            txtIdIngreso.Text = "ID " + DataFill.idIngreso.ToString();
            txtRazonSocial.Text = DataFill.razonSocial;
            txtFactura.Text = DataFill.factura;
            txtFecha.Text = DataFill.fecha.ToShortDateString();
            txtMontoRestante.Text = "Total " + DataFill.monto.ToString();
            txtMontoTotal.Text = "Restante " + DataFill.montoTotal.ToString();
            txtCedulaProveedor.Text = DataFill.cedulaProveedor;
        }

        void fillData(bool total)
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            int idCuentaPagar = DataFill.idCuentaPagar;

            double monto = 0F;

            if (!total)
                monto = double.Parse(txtMonto.txt.Text);
            else
                monto = DataFill.monto;


            UForm = new DRegistro_CuentaPagar(0,
                                         idCuentaPagar,
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
                rpta = MessageBox.Show("Desea Cancelar el Monto Total Restante de " + DataFill.montoTotal + " Bs S ?", " Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
            }
            else
            {
                rpta = MessageBox.Show("Desea Cancelar el Monto de" + txtMonto.txt.Text + " Bs S para dejar un restante de " + (DataFill.montoTotal - double.Parse(txtMonto.txt.Text)).ToString() + " Bs S ?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
            }

            if (rpta == MessageBoxResult.No)
                return;

            string abonarCP = Metodos.RegistrarCxP(UForm, DataFill.idCuentaPagar);

            if (abonarCP.Equals("TOTAL"))
            {
                MessageBox.Show("Pago Completado!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (abonarCP.Equals("PARCIAL"))
            {
                MessageBox.Show("Abono Ingresado!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
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
