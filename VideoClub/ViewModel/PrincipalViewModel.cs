using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ViewModel.Services;

namespace ViewModel
{
    /// <summary>
    /// ViewModel de la ventana principal.
    /// Gestiona la lógica de la pantalla principal y 
    /// la navegación hacia las diferentes pantallas del VideoClub.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ClienteService _clientesService = new ClienteService();
        private readonly CatalogoService _catalogoService = new CatalogoService();
        private readonly AlquilerService _alquileresService = new AlquilerService();

        /// <summary> Evento que notifica a la Vista cuando se requiere abrir una nueva sección. </summary>
        public event Action OnAccionSolicitada;

        /// <summary> Almacena el identificador de la vista o acción que se desea ejecutar. </summary>
        public string AccionSolicitada { get; private set; }

        private int _totalClientes;
        /// <summary>
        /// Obtiene o establece el número total de clientes registrados en la base de datos.
        /// </summary>
        public int TotalClientes
        {
            get => _totalClientes;
            set { _totalClientes = value; OnPropertyChanged(nameof(TotalClientes)); }
        }

        private int _totalProductos;
        /// <summary>
        /// Obtiene o establece la cantidad de títulos disponibles en el catálogo.
        /// </summary>
        public int TotalProductos
        {
            get => _totalProductos;
            set { _totalProductos = value; OnPropertyChanged(nameof(TotalProductos)); }
        }

        private List<Model.Alquileres> _actividadReciente;
        /// <summary>
        /// Lista de los últimos movimientos para mostrar en el DataGrid del Dashboard.
        /// </summary>
        public List<Model.Alquileres> ActividadReciente
        {
            get => _actividadReciente;
            set { _actividadReciente = value; OnPropertyChanged(nameof(ActividadReciente)); }
        }

        private int _alquileresActivos;
        /// <summary>
        /// Cantidad de alquileres activos actualmente.
        /// </summary>
        public int AlquileresActivos
        {
            get => _alquileresActivos;
            set { _alquileresActivos = value; OnPropertyChanged(nameof(AlquileresActivos)); }
        }

        private int _stockBajo;
        /// <summary>
        /// Cantidad de productos con stock bajo.
        /// </summary>
        public int StockBajo
        {
            get => _stockBajo;
            set { _stockBajo = value; OnPropertyChanged(nameof(StockBajo)); }
        }


        /// <summary> Obtiene la fecha actual formateada para la cabecera. </summary>
        public string FechaActual => DateTime.Now.ToString("dddd, dd MMMM yyyy");


        /// <summary> Comando para abrir la gestión de clientes. </summary>
        public ICommand AbrirClientesCommand { get; }

        /// <summary> Comando para abrir el catálogo. </summary>
        public ICommand AbrirCatalogoCommand { get; }

        /// <summary> Comando para abrir los alquileres. </summary>
        public ICommand AbrirAlquileresCommand { get; }

        /// <summary> Comando para abrir la configuración. </summary>
        public ICommand AbrirConfigCommand { get; }

        /// <summary> Comando para cerrar la aplicación. </summary>
        public ICommand SalirCommand { get; }


        /// <summary>
        /// Constructor del ViewModel. Inicializa servicios, comandos y carga datos reales.
        /// </summary>
        public MainWindowViewModel()
        {

            // Inicialización de Comandos
            AbrirClientesCommand = new RelayCommand(() => SolicitarAccion("Clientes"));
            AbrirCatalogoCommand = new RelayCommand(() => SolicitarAccion("Catalogo"));
            AbrirAlquileresCommand = new RelayCommand(() => SolicitarAccion("Alquileres"));
            AbrirConfigCommand = new RelayCommand(() => SolicitarAccion("Configuracion"));

            // Carga de datos desde el servicio
            RefrescarDatos();
        }

        /// <summary>
        /// Gestiona la navegación actualizando la acción solicitada y disparando el evento.
        /// </summary>
        /// <param name="accion">Identificador de la vista destino.</param>
        private void SolicitarAccion(string accion)
        {
            AccionSolicitada = accion;
            OnAccionSolicitada?.Invoke();
        }

        /// <summary>
        /// Consulta el servicio para obtener los datos actualizados de la base de datos.
        /// </summary>
        public void RefrescarDatos()
        {
            TotalClientes = _clientesService.ObtenerTotalClientes();
            TotalProductos = _catalogoService.ObtenerTotalProductos();
            ActividadReciente = _alquileresService.ObtenerActividadReciente();
            AlquileresActivos = _alquileresService.ObtenerAlquileresActivos();
            StockBajo = _catalogoService.ObtenerStockBajo();
        }

        /// <summary> Evento para notificar cambios en las propiedades a la Vista. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}