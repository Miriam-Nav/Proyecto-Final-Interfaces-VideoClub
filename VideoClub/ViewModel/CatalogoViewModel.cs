using Informes;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using ViewModel.Services;

namespace ViewModel
{
    /// <summary>
    /// ViewModel de la ventana del catálogo de productos.
    /// Gestiona la lógica de registro, actualización, eliminación y 
    /// visualización de los títulos disponibles en el VideoClub.
    /// </summary>
    public class CatalogoViewModel : INotifyPropertyChanged
    {
        private readonly CatalogoService _service = new CatalogoService();

        // --- LISTAS PARA DATAGRID Y COMBOBOX ---
        private List<Productos> _productos;
        /// <summary>
        /// Lista de productos registrados.
        /// </summary>
        public List<Productos> Productos
        {
            get => _productos;
            set
            {
                _productos = value;
                OnPropertyChanged(nameof(Productos));
            }
        }

        private List<Generos> _generos;
        /// <summary>
        /// Lista de generos registrados.
        /// </summary>
        public List<Generos> Generos
        {
            get => _generos;
            set { _generos = value; OnPropertyChanged(nameof(Generos)); }
        }

        private List<Tarifas> _tarifas;
        /// <summary>
        /// Lista de tarifas registradas.
        /// </summary>
        public List<Tarifas> Tarifas
        {
            get => _tarifas;
            set { _tarifas = value; OnPropertyChanged(nameof(Tarifas)); }
        }


        // --- SELECCIONES ---

        // Producto seleccionado en la tabla
        private Productos _productoSeleccionado;
        /// <summary>
        /// Obtiene el producto seleccionado actualmente en la lista.
        /// </summary>
        public Productos ProductoSeleccionado
        {
            get => _productoSeleccionado;
            set
            {
                _productoSeleccionado = value;
                OnPropertyChanged(nameof(ProductoSeleccionado));

                // Al pulsar en la tabla se llenan los campos
                CargarProductoEnFormulario();
            }
        }

        private Generos _generoSeleccionado;
        /// <summary> Género del producto. </summary>
        public Generos GeneroSeleccionado
        {
            get => _generoSeleccionado;
            set { _generoSeleccionado = value; OnPropertyChanged(nameof(GeneroSeleccionado)); }
        }

        private Tarifas _tarifaSeleccionada;
        /// <summary> Tarifa de alquiler aplicada. </summary>
        public Tarifas TarifaSeleccionada
        {
            get => _tarifaSeleccionada;
            set { _tarifaSeleccionada = value; OnPropertyChanged(nameof(TarifaSeleccionada)); }
        }


        // --- INPUTS DE TEXTO ---

        // Propiedades para los TextBox 
        private string _inputTitulo;
        /// <summary> Título de la película o producto para el formulario. </summary>
        public string InputTitulo
        {
            get => _inputTitulo;
            set { _inputTitulo = value; OnPropertyChanged(nameof(InputTitulo)); }
        }

        private string _inputTipo;
        /// <summary> Tipo de producto. </summary>
        public string InputTipo
        {
            get => _inputTipo;
            set { 
                _inputTipo = value; 
                OnPropertyChanged(nameof(InputTipo));
                OnPropertyChanged(nameof(EsPelicula));
                OnPropertyChanged(nameof(EsJuego));
            }
        }

        // --- LÓGICA PARA RADIOBUTTONS ---
        /// <summary> 
        /// Indica si el tipo de producto seleccionado es una Película. 
        /// </summary>
        public bool EsPelicula
        {
            get => InputTipo == "Pelicula";
            set 
            { 
                if (value) 
                { 
                    InputTipo = "Pelicula"; 
                } 
            }
        }

        /// <summary> 
        /// Indica si el tipo de producto seleccionado es un Juego. 
        /// </summary>
        public bool EsJuego
        {
            get => InputTipo == "Juego";
            set 
            { 
                if (value) 
                { 
                    InputTipo = "Juego"; 
                } 
            }
        }

        private string _inputStock;
        /// <summary> Cantidad de unidades disponibles en inventario. </summary>
        public string InputStock
        {
            get => _inputStock;
            set { _inputStock = value; OnPropertyChanged("InputStock"); }
        }

        private string _errorTitulo;
        public string ErrorTitulo
        {
            get => _errorTitulo;
            set { _errorTitulo = value; OnPropertyChanged(nameof(ErrorTitulo)); }
        }

        private string _errorStock;
        public string ErrorStock
        {
            get => _errorStock;
            set { _errorStock = value; OnPropertyChanged(nameof(ErrorStock)); }
        }

        // Comandos
        /// <summary> Comando para registrar un nuevo producto en el catálogo. </summary>
        public ICommand CrearCommand { get; }
        /// <summary> Comando para modificar los datos del producto seleccionado. </summary>
        public ICommand ModificarCommand { get; }
        /// <summary> Comando para eliminar un producto del catálogo. </summary>
        public ICommand EliminarCommand { get; }
        /// <summary> Comando para resetear los campos del formulario. </summary>
        public ICommand LimpiarCommand { get; }
        /// <summary> Comando para generar un informe con los datos de los productos. </summary>
        public ICommand GenerarInformeCommand { get; }

        /// <summary>
        /// Constructor del ViewModel. 
        /// Inicializa los servicios, comandos y carga la lista inicial.
        /// </summary>
        public CatalogoViewModel()
        {
            CrearCommand = new RelayCommand(Registrar);
            ModificarCommand = new RelayCommand(Actualizar);
            EliminarCommand = new RelayCommand(Borrar);
            LimpiarCommand = new RelayCommand(Limpiar);
            GenerarInformeCommand = new RelayCommand(GenerarInforme);

            // Carga inicial de datos
            RefrescarLista();
            CargarCombos();

            // Valor inicial
            InputTipo = "Pelicula"; 
        }

        /// <summary>
        /// Consulta el servicio para obtener la lista actualizada de productos.
        /// </summary>
        private void RefrescarLista()
        {
            Productos = _service.ObtenerTodos();
        }

        /// <summary>
        /// Inserta un nuevo registro de producto utilizando los datos del formulario.
        /// </summary>
        private void Registrar()
        {
            try
            {
                ErrorTitulo = "";
                ErrorStock = "";

                if (GeneroSeleccionado == null) 
                { 
                    throw new Exception("Selecciona un genero"); 
                }

                if (TarifaSeleccionada == null)
                {
                    throw new Exception("Selecciona una tarifa");
                }

                // Convierte los inputs de texto a los tipos numéricos correspondientes
                int.TryParse(InputStock, out int stock);

                var nuevo = new Productos
                {
                    Titulo = InputTitulo,
                    Tipo = InputTipo,
                    Stock = stock,
                    GeneroId = GeneroSeleccionado.Id,
                    TarifaId = TarifaSeleccionada.Id
                };

                _service.Guardar(nuevo);
                RefrescarLista();
                Limpiar();

                ErrorTitulo = "";
                ErrorStock = "";

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("titulo"))
                {
                    ErrorTitulo = ex.Message;
                }
                else if (ex.Message.Contains("stock"))
                {
                    ErrorTitulo = ex.Message;
                }
                else
                {
                    ErrorStock = ex.Message;
                }
            }
        }

        /// <summary>
        /// Actualiza la información del producto seleccionado con los cambios realizados.
        /// </summary>
        private void Actualizar()
        {
            try
            {
                ErrorTitulo = "";
                ErrorStock = "";

                if (ProductoSeleccionado != null && GeneroSeleccionado != null && TarifaSeleccionada != null)
                {
                    int.TryParse(InputStock, out int stock);

                    ProductoSeleccionado.Titulo = InputTitulo;
                    ProductoSeleccionado.Tipo = InputTipo;
                    ProductoSeleccionado.Stock = stock;
                    ProductoSeleccionado.GeneroId = GeneroSeleccionado.Id;
                    ProductoSeleccionado.TarifaId = TarifaSeleccionada.Id;

                    _service.Actualizar(ProductoSeleccionado);
                    RefrescarLista();
                    Limpiar();

                    ErrorTitulo = "";
                    ErrorStock = "";
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("titulo"))
                {
                    ErrorTitulo = ex.Message;
                }
                else if (ex.Message.ToLower().Contains("stock"))
                {
                    ErrorStock = ex.Message;
                }
                else
                {
                    ErrorTitulo = ex.Message;
                }
            }
        }

        /// <summary>
        /// Elimina de forma permanente el producto seleccionado.
        /// </summary>
        private void Borrar()
        {
            if (ProductoSeleccionado != null) 
            {  
                _service.Eliminar(ProductoSeleccionado.Id);
                RefrescarLista();
                Limpiar();
            }
        }

        /// <summary>
        /// Genera y muestra un informe visual con el listado de productos.
        /// </summary>
        private void GenerarInforme()
        {
            try
            {
                // Reporte
                var miReporte = new ProductosPorGenero();

                var datos = _service.SeleccionarDatosRepo();

                miReporte.Database.Tables["ProductosPorGenero"].SetDataSource(datos);

                var ventanaVisor = new Informes.InformesView();
                ventanaVisor.reportViewer.ViewerCore.ReportSource = miReporte;
                ventanaVisor.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Limpia todas las propiedades del formulario.
        /// </summary>
        private void Limpiar()
        {
            InputTitulo = string.Empty;
            InputTipo = "Pelicula";
            InputStock = string.Empty;
            GeneroSeleccionado = null;
            TarifaSeleccionada = null;
            ProductoSeleccionado = null;
            ErrorTitulo = "";
            ErrorStock = "";
        }

        /// <summary>
        /// Muestra los datos del producto seleccionado en los campos del formulario.
        /// </summary>
        private void CargarProductoEnFormulario()
        {
            if (ProductoSeleccionado != null)
            {
                InputTitulo = ProductoSeleccionado.Titulo;
                InputTipo = ProductoSeleccionado.Tipo;
                InputStock = ProductoSeleccionado.Stock.ToString();

                if (Generos != null)
                {
                    GeneroSeleccionado = Generos.Find(g => g.Id == ProductoSeleccionado.GeneroId);
                }
                if (Tarifas != null)
                {
                    TarifaSeleccionada = Tarifas.Find(t => t.Id == ProductoSeleccionado.TarifaId);
                }
            }
        }

        /// <summary>
        /// Carga las listas de géneros y tarifas al ComboBox del formulario.
        /// </summary>
        private void CargarCombos()
        {
            Generos = _service.ObtenerGeneros();
            Tarifas = _service.ObtenerTarifas();
        }

        /// <summary> Evento para notificar cambios en las propiedades a la Vista. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}