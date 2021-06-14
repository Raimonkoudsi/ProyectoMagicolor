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
            txtMonto.Text = DataFill.montoTotal.ToString();

            total = true;
            Create(true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtTitulo.Text = "Compra N° " + DataFill.idIngreso.ToString();
            txtProveedor.Text = DataFill.razonSocial;
            txtCedula.Text = DataFill.cedulaProveedor;

            double z = Math.Truncate(DataFill.monto * 100) / 100;
            txtMontoRestante.Text = z.ToString("0.00") + " Bs S";

            double y = Math.Truncate(DataFill.montoTotal * 100) / 100;
            txtMontoTotal.Text = y.ToString("0.00") + " Bs S";
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
                monto = double.Parse(txtMonto.Text);
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
                rpta = MessageBox.Show("Desea Cancelar el Monto de" + txtMonto.Text + " Bs S para dejar un restante de " + (DataFill.montoTotal - double.Parse(txtMonto.Text)).ToString() + " Bs S ?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
            }

            if (rpta == MessageBoxResult.No)
                return;

            string abonarCP = Metodos.RegistrarCxP(UForm, DataFill.idCuentaPagar);

            if (abonarCP.Equals("TOTAL"))
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Registrar",
                    "Ha Completado la Cuenta por Pagar N° " + DataFill.idCuentaPagar
                );
                new LAuditoria().Insertar(auditoria);

                MessageBox.Show("Pago Completado!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (abonarCP.Equals("PARCIAL"))
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Registrar",
                    "Ha Abonado a la Cuenta por Pagar N° " + DataFill.idCuentaPagar
                );
                new LAuditoria().Insertar(auditoria);

                MessageBox.Show("Abono Ingresado!", "Variedades Magicolor", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            this.Close();
        }



        bool Validate()
        {
            if ((txtMonto.Text == "" || double.Parse(txtMonto.Text) <= 0) && !total)
            {
                LFunction.MessageExecutor("Error", "Debe Agregar un Monto para Abonar");
                txtMonto.Focus();
                return true;
            }
            else if (double.Parse(txtMonto.Text) > DataFill.montoTotal && !total)
            {
                LFunction.MessageExecutor("Error", "El Monto no debe Exceder la Deuda");
                txtMonto.Focus();
                return true;
            }

            return false;

        }
    }
}
