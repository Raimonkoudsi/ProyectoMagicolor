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

            txtTitulo.Text = "Venta N° " + DataFill.idVenta.ToString();
            txtCliente.Text = DataFill.cliente;
            txtCedula.Text = DataFill.cedulaCliente;

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
                rpta = MessageBox.Show("Desea Cancelar el Monto Total Restante de " + DataFill.montoTotal + " Bs S ?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);
            }
            else
                rpta = MessageBox.Show("Desea Cancelar el Monto de " + txtMonto.Text + " Bs S para dejar un restante de " + (DataFill.montoTotal - double.Parse(txtMonto.Text)).ToString() + "Bs S ?", "Variedades Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (rpta == MessageBoxResult.No)
            {
                return;
            }


            string abonarCC = Metodos.RegistrarCxC(UForm, DataFill.idCuentaCobrar);

            if (abonarCC.Equals("TOTAL"))
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Registrar",
                    "Ha Completado la Cuenta por Cobrar N° " + DataFill.idCuentaCobrar
                );
                new LAuditoria().Insertar(auditoria);

                LFunction.MessageExecutor("Information", "Pago Total Completado");
            }
            else if (abonarCC.Equals("PARCIAL"))
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Registrar",
                    "Ha Abonado a la Cuenta por Cobrar N° " + DataFill.idCuentaCobrar
                );
                new LAuditoria().Insertar(auditoria);

                LFunction.MessageExecutor("Information", "Abono Ingresado Correctamente");
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
