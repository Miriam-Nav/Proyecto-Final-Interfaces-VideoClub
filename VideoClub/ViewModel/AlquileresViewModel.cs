using Informes;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Windows.Input;
using ViewModel.Services;

namespace ViewModel
{
    /// <summary>
    /// ViewModel para la gestión de alquileres.
    /// Controla el registro de nuevos alquileres, devoluciones y el historial
    /// de transacciones de los clientes.
    /// </summary>
    public class AlquileresViewModel : INotifyPropertyChanged
    {
        private readonly AlquilerService _service = new AlquilerService();
        private readonly CatalogoService _productoService = new CatalogoService();
        private readonly ClienteService _clienteService = new ClienteService();

        // --- PROPIEDADES DE DATOS ---
        private List<Alquileres> _alquileres;
        /// <summary> Lista completa del historial de alquileres. </summary>
        public List<Alquileres> Alquileres
        {
            get
            {
                if (ClienteSeleccionado == null && ProductoSeleccionado == null)
                {
                    return _alquileres;
                }

                IEnumerable<Alquileres> filtrados = _alquileres;

                // Si hay un cliente seleccionado, filtra por ClienteId
                if (ClienteSeleccionado != null)
                {
                    filtrados = filtrados.Where(a => a.ClienteId == ClienteSeleccionado.Id);
                }

                // Si hay un producto seleccionado, filtra por ProductoId
                if (ProductoSeleccionado != null)
                {
                    filtrados = filtrados.Where(a => a.ProductoId == ProductoSeleccionado.Id);
                }

                return filtrados.ToList();
            }
            set { _alquileres = value; OnPropertyChanged(nameof(Alquileres)); }
        }

        private List<Clientes> _listaClientes;
        /// <summary> Lista de clientes registrados para el ComboBox. </summary>
        public List<Clientes> ListaClientes
        {
            get => _listaClientes;
            set { _listaClientes = value; OnPropertyChanged(nameof(ListaClientes)); }
        }

        private List<Productos> _listaProductosDisponibles;
        /// <summary> Lista de productos con stock disponible para alquilar. </summary>
        public List<Productos> ListaProductosDisponibles
        {
            get => _listaProductosDisponibles;
            set { _listaProductosDisponibles = value; OnPropertyChanged(nameof(ListaProductosDisponibles)); }
        }

        // --- PROPIEDADES DE SELECCIÓN ---

        private Clientes _clienteSeleccionado;
        /// <summary> Cliente seleccionado para realizar un nuevo alquiler. </summary>
        public Clientes ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set 
            { 
                _clienteSeleccionado = value; 
                OnPropertyChanged(nameof(ClienteSeleccionado));
                OnPropertyChanged(nameof(Alquileres));
            }
        }

        private Productos _productoSeleccionado;
        /// <summary> Producto seleccionado para realizar un nuevo alquiler. </summary>
        public Productos ProductoSeleccionado
        {
            get => _productoSeleccionado;
            set 
            { 
                _productoSeleccionado = value; 
                OnPropertyChanged(nameof(ProductoSeleccionado));
                OnPropertyChanged(nameof(Alquileres));
            }
        }

        private DateTime? _fechaDevolucionPrevista = DateTime.Now.AddDays(1);
        /// <summary> Fecha en la que se espera que el cliente devuelva el producto. </summary>
        public DateTime? FechaDevolucionPrevista
        {
            get => _fechaDevolucionPrevista;
            set { _fechaDevolucionPrevista = value; OnPropertyChanged(nameof(FechaDevolucionPrevista)); }
        }

        private Alquileres _alquilerSeleccionado;
        /// <summary> Alquiler seleccionado en la tabla para registrar devolución o imprimir. </summary>
        public Alquileres AlquilerSeleccionado
        {
            get => _alquilerSeleccionado;
            set { _alquilerSeleccionado = value; OnPropertyChanged(nameof(AlquilerSeleccionado)); }
        }

        private string _errorCliente;
        public string ErrorCliente
        {
            get => _errorCliente;
            set { _errorCliente = value; OnPropertyChanged(nameof(ErrorCliente)); }
        }

        private string _errorProducto;
        public string ErrorProducto
        {
            get => _errorProducto;
            set { _errorProducto = value; OnPropertyChanged(nameof(ErrorProducto)); }
        }

        private string _errorFecha;
        public string ErrorFecha
        {
            get => _errorFecha;
            set { _errorFecha = value; OnPropertyChanged(nameof(ErrorFecha)); }
        }


        // --- COMANDOS ---
        /// <summary> Comando para registrar un nuevo alquiler en el sistema. </summary>
        public ICommand AlquilarCommand { get; }

        /// <summary> Comando para procesar la devolución de un producto alquilado. </summary>
        public ICommand DevolverCommand { get; }

        /// <summary> Comando para eliminar el registro de un producto alquilado. </summary>
        public ICommand EliminarCommand { get; }

        /// <summary> Comando para limpiar los campos de selección del formulario. </summary>
        public ICommand LimpiarCommand { get; }

        /// <summary> Comando para generar el reporte de alquileres. </summary>
        public ICommand GenerarInformeCommand { get; }

        /// <summary>
        /// Constructor del ViewModel.
        /// Inicializa comandos y carga la información inicial de clientes y productos.
        /// </summary>
        public AlquileresViewModel()
        {
            AlquilarCommand = new RelayCommand(RegistrarAlquiler);
            DevolverCommand = new RelayCommand(RegistrarDevolucion);
            EliminarCommand = new RelayCommand(EliminarAlquiler);
            LimpiarCommand = new RelayCommand(LimpiarCampos);
            GenerarInformeCommand = new RelayCommand(GenerarInforme);

            RefrescarListas();
        }

        /// <summary>
        /// Actualiza todas las colecciones de datos desde la base de datos.
        /// </summary>
        private void RefrescarListas()
        {
            _alquileres = _service.ObtenerTodos();
            ListaClientes = _clienteService.ObtenerTodos();

            ListaProductosDisponibles = _productoService.ObtenerTodos().ToList();
            OnPropertyChanged(nameof(Alquileres));
        }

        /// <summary>
        /// Crea un nuevo registro de alquiler y descuenta el stock del producto.
        /// </summary>
        private void RegistrarAlquiler()
        {
            try { 
                if (ClienteSeleccionado == null) 
                {
                    throw new Exception("Selecciona un cliente");
                }
                if (ProductoSeleccionado == null ) 
                {
                    throw new Exception("Selecciona un producto");
                }
                if (FechaDevolucionPrevista == null) 
                {
                    throw new Exception("Selecciona una fecha");
                }
                
                var nuevoAlquiler = new Alquileres
                {
                    ClienteId = ClienteSeleccionado.Id,
                    ProductoId = ProductoSeleccionado.Id,
                    FechaSalida = DateTime.Now,
                    FechaPrevistaDevolucion = FechaDevolucionPrevista.Value,
                    FechaDevolucionReal = null,

                    // Coste total en el Service
                };

                _service.GuardarAlquiler(nuevoAlquiler);
                RefrescarListas();
                LimpiarCampos();
                
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("cliente"))
                {
                    ErrorCliente = ex.Message;
                }
                else if (ex.Message.Contains("producto"))
                {
                    ErrorProducto = ex.Message;
                }
                else
                {
                    ErrorFecha = ex.Message;
                }
            }
        }

        /// <summary>
        /// Marca el alquiler seleccionado como devuelto y repone el stock.
        /// </summary>
        private void RegistrarDevolucion()
        {
            if (AlquilerSeleccionado != null && AlquilerSeleccionado.FechaDevolucionReal == null) {
                
                _service.MarcarComoDevuelto(AlquilerSeleccionado.Id);
                RefrescarListas();
            }
        }

        /// <summary>
        /// Elimina el alquiler seleccionado y repone el stock.
        /// </summary>
        private void EliminarAlquiler()
        {
            if (AlquilerSeleccionado != null)
            {
                _service.EliminarAlquiler(AlquilerSeleccionado.Id);
                RefrescarListas();
            }
        }

        /// <summary>
        /// Genera y muestra un informe visual con el listado de alquileres.
        /// </summary>
        private void GenerarInforme()
        {
            try
            {
                // Reporte de Crystal
                var miReporte = new PrestamosPorCliente();

                var datos = _service.SeleccionarDatosRepo();

                miReporte.SetDataSource(datos);

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
        /// Reinicia los valores de selección del formulario.
        /// </summary>
        private void LimpiarCampos()
        {
            ClienteSeleccionado = null;
            ProductoSeleccionado = null;
            FechaDevolucionPrevista = DateTime.Now.AddDays(1);
            AlquilerSeleccionado = null;
        }

        /// <summary> Evento para notificar cambios en las propiedades a la Vista. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}