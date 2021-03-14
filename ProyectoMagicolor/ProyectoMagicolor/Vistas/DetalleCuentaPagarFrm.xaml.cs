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
    /// Lógica de interacción para DetalleCuentaPagarFrm.xaml
    /// </summary>
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
        public DCuentaPagar DataCxP;

        public DRegistro_CuentaPagar UForm;

        public LArticulo Metodos = new LArticulo();

        public int idEdit;
        public List<DRegistro_CuentaPagar> ActualDetalle;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Create();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(DataFill.idIngreso.ToString());
            //txtTitulo.Text = "Agregar Pago";
            //fillForm(DataFill, DataCxP);
            //SetEnable(false);
            //btnEnviar.Visibility = Visibility.Collapsed;
            
        }

        void fillData()
        {
            //if (Validate())
            //{
            //    UForm = null;
            //    return;
            //}

            int idCuentaPagar = DataCxP.idCuentaPagar;
            double monto = double.Parse(txtMonto.txt.Text);


            UForm = new DRegistro_CuentaPagar(0,
                                         idCuentaPagar,
                                         monto,
                                         DateTime.Now);
        }

        void Create()
        {
            fillData();
            if (UForm == null)
                return;

            //ParentForm.AgregarArticulo(UForm, DataArticulo);
            this.Close();
        }


    }
}
