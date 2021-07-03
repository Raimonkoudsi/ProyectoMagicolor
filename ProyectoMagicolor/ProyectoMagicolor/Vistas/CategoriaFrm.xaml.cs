using System.Windows;
using Datos;
using Logica;

namespace ProyectoMagicolor.Vistas
{
    public partial class CategoriaFrm : Window
    {
        public ArticuloFrm ParentFrm;

        public TypeForm Type;

        public DCategoria DataFill;
        public DCategoria UForm;
        public LCategoria Metodos = new LCategoria();

        private string nombre = "";


        public CategoriaFrm(ArticuloFrm parent = null)
        {
            InitializeComponent();

            ParentFrm = parent;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(Type == TypeForm.Read)
            {
                txtTitulo.Text = "Leer Categoria";
                fillForm(DataFill);
                SetEnable(false);
                btnEnviar.Content = "Sólo Lectura";

                DAuditoria auditoria = new DAuditoria(
                    Globals.ID_SISTEMA,
                    "Mostrar",
                    "Ha Visto la Categoria " + nombre
                 );
                new LAuditoria().Insertar(auditoria);
            }
            else if(Type == TypeForm.Update)
            {
                SetEnable(true);

                txtTitulo.Text = "Editar Categoria";
                fillForm(DataFill);
            }

            txtNombre.Focus();
        }

       

        private void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            nombre = txtNombre.Text;
            string descripcion = txtDescripcion.Text;

            UForm = new DCategoria(0, nombre, descripcion);
        }

        private void Create()
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

                if (ParentFrm != null)
                    ParentFrm.SetCategoria(UForm.nombre);

                this.DialogResult = true;
                this.Close();
            }

        }

        private void Update()
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

        private void SetEnable(bool Enable)
        {
            txtNombre.IsEnabled = false;
            txtDescripcion.IsEnabled = Enable;
            btnEnviar.IsEnabled = Enable;
        }

        private void fillForm(DCategoria Data)
        {
            if(Data != null)
            {
                txtNombre.Text = Data.nombre;
                txtDescripcion.Text = Data.descripcion;
            }
        }

        #region Validation
        bool Validate()
        {
            if (txtNombre.Text == "")
            {
                LFunction.MessageExecutor("Error", "Debe colocar el nombre de la categoría");
                txtNombre.Focus();
                return true;
            }

            if (txtNombre.Text.Length <= 3)
            {
                LFunction.MessageExecutor("Error", "El nombre de la categoría debe ser mayor de 3 carácteres");
                txtNombre.Focus();
                return true;
            }

            if (txtDescripcion.Text.Length <= 5 && txtDescripcion.Text != "")
            {
                LFunction.MessageExecutor("Error", "La descripción de la categoría debe ser mayor de 5 carácteres");
                txtNombre.Focus();
                return true;
            }

            if (Type != TypeForm.Update)
            {
                if (Metodos.CategoriaRepetida(txtNombre.Text))
                {
                    LFunction.MessageExecutor("Error", "La categoría ya está registrada en el sistema");
                    txtNombre.Text = ""; 
                    txtNombre.Focus();
                    return true;
                }
            }

            return false;
        }
        #endregion


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Type == TypeForm.Update)
                Update();
            else
                Create();
        }

        private void txtNombre_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Metodos.CategoriaRepetida(txtNombre.Text))
            {
                LFunction.MessageExecutor("Error", "La categoría ya está registrada en el sistema");
                txtNombre.Text = "";
                txtNombre.Focus();
            }
        }
    }
}
