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
    public partial class CategoriaFrm : Window
    {


        public CategoriaFrm()
        {
            InitializeComponent();
        }

        

        public TypeForm Type;
        public DCategoria DataFill;

        public DCategoria UForm;

        public LCategoria Metodos = new LCategoria();

        private string nombre = "";

        private void Button_Click(object sender, RoutedEventArgs e)
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
                txtTitulo.Text = "Leer Categoria";
                fillForm(DataFill);
                SetEnable(false);
                btnEnviar.Visibility = Visibility.Collapsed;

                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Mostrar",
                    "Ha Visto la Categoria " + nombre
                 );
                new LAuditoria().Insertar(auditoria);
            }
            else if(Type == TypeForm.Update)
            {
                txtTitulo.Text = "Editar Categoria";
                fillForm(DataFill);
            }
        }

       

        void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            nombre = txtNombre.txt.Text;
            string descripcion = txtDescripcion.Text;

            UForm = new DCategoria(0, nombre, descripcion);
        }

        void Create()
        {
            fillData();
            if (UForm == null)
                return;
            string response = Metodos.Insertar(UForm);
            if (response == "OK")
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Insertar",
                    "Ha Registrado la Categoria " + nombre
                 );
                new LAuditoria().Insertar(auditoria);

                this.DialogResult = true;
                this.Close();
            }

        }

        void Update()
        {
            fillData();
            if (UForm == null)
                return;
            UForm.idCategoria = DataFill.idCategoria;
            string response = Metodos.Editar(UForm);

            if(response == "OK")
            {
                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Editar",
                    "Ha Editado la Categoria " + nombre
                 );
                new LAuditoria().Insertar(auditoria);

                this.DialogResult = true;
                this.Close();
            }
        }

        private void PlaceDescripcion_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtDescripcion.Text == "")
            {
                PlaceDescripcion.Text = "";
            }
        }

        private void PlaceDescripcion_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDescripcion.Text == "")
            {
                PlaceDescripcion.Text = "Descripción";
            }
        }

        void SetEnable(bool Enable)
        {
            txtNombre.IsEnabled = Enable;
            txtDescripcion.IsEnabled = Enable;
        }
        void fillForm(DCategoria Data)
        {
            if(Data != null)
            {
                txtNombre.SetText(Data.nombre);
                txtDescripcion.Text = Data.descripcion;
                PlaceDescripcion.Text = "";
            }
        }
        #region Validation
        bool Validate()
        {
            if (!txtNombre.Changed)
            {
                MessageBox.Show("Debes llenar el campo Nombre!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtNombre.txt.Focus();
                return true;
            }

            return false;
        }
        #endregion


    }
}
