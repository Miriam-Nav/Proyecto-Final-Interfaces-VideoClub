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
    /// ViewModel de la ventana de gestión de clientes.
    /// Gestiona la lógica de registro, actualización, eliminación y 
    /// visualización de los clientes del VideoClub.
    /// </summary>
    public class ClientesViewModel : INotifyPropertyChanged
    {
        private readonly ClienteService _service = new ClienteService();

        // Lista para el DataGrid
        private List<Clientes> _clientes;
        /// <summary>
        /// Lista de clientes registrados.
        /// </summary>
        public List<Clientes> Clientes
        {
            // Devuelve la colección privada _clientes
            get => _clientes;
            set
            {
                // Guarda el nuevo valor y notifica el cambio
                _clientes = value;
                OnPropertyChanged(nameof(Clientes));
            }
        }

        // Cliente seleccionado en la tabla
        private Clientes _clienteSeleccionado;
        /// <summary>
        /// Obtiene o establece el cliente seleccionado actualmente en la lista.
        /// </summary>
        public Clientes ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set
            {
                _clienteSeleccionado = value;
                OnPropertyChanged(nameof(ClienteSeleccionado));

                // Al pulsar en la tabla se llenan los campos
                CargarClienteEnFormulario(); 
            }
        }

        // Propiedades para los TextBox (Inputs)
        private string _inputNombre;
        /// <summary> Nombre completo del cliente para el formulario. </summary>
        public string InputNombre 
        { 
            get => _inputNombre; 
            set { _inputNombre = value; OnPropertyChanged(nameof(InputNombre)); } 
        }


        private string _inputEmail;
        /// <summary> Correo electrónico de contacto del cliente. </summary>
        public string InputEmail 
        { 
            get => _inputEmail; 
            set { _inputEmail = value; OnPropertyChanged(nameof(InputEmail)); } 
        }

        private string _inputTelefono;
        /// <summary> Número de teléfono del cliente. </summary>    
        public string InputTelefono 
        { 
            get => _inputTelefono; 
            set { _inputTelefono = value; OnPropertyChanged(nameof(InputTelefono)); } 
        }

        private bool _esActivo = true;
        /// <summary> Determina si el cliente tiene una cuenta activa. </summary>
        public bool EsActivo 
        { 
            get => _esActivo; 
            set { _esActivo = value; OnPropertyChanged(nameof(EsActivo)); } 
        }

        private string _errorNombre;
        public string ErrorNombre
        {
            get => _errorNombre;
            set { _errorNombre = value; OnPropertyChanged(nameof(ErrorNombre)); }
        }

        private string _errorEmail;
        public string ErrorEmail
        {
            get => _errorEmail;
            set { _errorEmail = value; OnPropertyChanged(nameof(ErrorEmail)); }
        }

        private string _errorTelefono;
        public string ErrorTelefono
        {
            get => _errorTelefono;
            set { _errorTelefono = value; OnPropertyChanged(nameof(ErrorTelefono)); }
        }

        // Comandos
        /// <summary> Comando para registrar un nuevo cliente. </summary>
        public ICommand CrearCommand { get; }
        /// <summary> Comando para modificar los datos del cliente seleccionado. </summary>
        public ICommand ModificarCommand { get; }
        /// <summary> Comando para eliminar un cliente de la base de datos. </summary>
        public ICommand EliminarCommand { get; }
        /// <summary> Comando para resetear los campos del formulario. </summary>
        public ICommand LimpiarCommand { get; }
        /// <summary> Comando para generar un informe con los datos de los clientes. </summary>
        public ICommand GenerarInformeCommand { get; }

        /// <summary>
        /// Constructor del ViewModel. 
        /// Inicializa los servicios, comandos y carga la lista inicial.
        /// </summary>
        public ClientesViewModel()
        {
            CrearCommand = new RelayCommand(Registrar);   
            ModificarCommand = new RelayCommand(Actualizar);
            EliminarCommand = new RelayCommand(Borrar);
            LimpiarCommand = new RelayCommand(Limpiar);
            GenerarInformeCommand = new RelayCommand(GenerarInforme);

            // Carga inicial de datos
            RefrescarLista();
        }

        /// <summary>
        /// Consulta el servicio para obtener la lista actualizada de clientes.
        /// </summary>
        private void RefrescarLista()
        {
            Clientes = _service.ObtenerTodos();
        }

        /// <summary>
        /// Limpia los campos de errores.
        /// </summary>
        private void LimpiarErrores() {
            ErrorNombre = "";
            ErrorEmail = "";
            ErrorTelefono = "";
        }

        /// <summary>
        /// Inserta un nuevo registro de cliente utilizando los datos del formulario.
        /// </summary>
        private void Registrar()
        {
            LimpiarErrores();

            try
            {
                var nuevo = new Clientes
                {
                    Nombre = InputNombre,
                    Email = InputEmail,
                    Telefono = InputTelefono,
                    Activo = EsActivo,
                    FechaAlta = DateTime.Now
                };
                _service.Guardar(nuevo);
                RefrescarLista();
                Limpiar();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("nombre"))
                {
                    ErrorNombre = ex.Message;
                }
                else if (ex.Message.Contains("correo"))
                {
                    ErrorEmail = ex.Message;
                }
                else 
                {
                    ErrorTelefono = ex.Message;
                }
            }
        }

        /// <summary>
        /// Actualiza la información del cliente seleccionado con los cambios realizados.
        /// </summary>
        private void Actualizar()
        {
            try
            {
                LimpiarErrores();

                if (ClienteSeleccionado != null)
                {
                    ClienteSeleccionado.Nombre = InputNombre;
                    ClienteSeleccionado.Email = InputEmail;
                    ClienteSeleccionado.Telefono = InputTelefono;
                    ClienteSeleccionado.Activo = EsActivo;

                    _service.Actualizar(ClienteSeleccionado);
                    RefrescarLista();
                    Limpiar();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("nombre"))
                {
                    ErrorNombre = ex.Message;
                }
                else if (ex.Message.Contains("correo"))
                {
                    ErrorEmail = ex.Message;
                }
                else
                {
                    ErrorTelefono = ex.Message;
                }
            }
        }

        /// <summary>
        /// Elimina de forma permanente el cliente seleccionado.
        /// </summary>
        private void Borrar()
        {
            try
            {
                ErrorNombre = "";

                if (ClienteSeleccionado != null)
                {
                    _service.Eliminar(ClienteSeleccionado.Id);
                    RefrescarLista();
                    Limpiar();
                    ErrorNombre = "";
                }
            }
            catch (Exception ex)
            {
                ErrorNombre = ex.Message;
            }
        }

        /// <summary>
        /// Genera y muestra un informe visual con el listado de clientes.
        /// </summary>
        private void GenerarInforme()
        {
            try
            {
                // Reporte
                var miReporte = new ClientesReport();

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
        /// Limpia todas las propiedades del formulario.
        /// </summary>
        private void Limpiar()
        {
            InputNombre = string.Empty;
            InputEmail = string.Empty;
            InputTelefono = string.Empty;
            EsActivo = true;
            ClienteSeleccionado = null;
            LimpiarErrores();
        }

        /// <summary>
        /// Muestra los datos del cliente seleccionado en los campos del formulario.
        /// </summary>
        private void CargarClienteEnFormulario()
        {
            if (ClienteSeleccionado != null) 
            {
                InputNombre = ClienteSeleccionado.Nombre;
                InputEmail = ClienteSeleccionado.Email;
                InputTelefono = ClienteSeleccionado.Telefono;
                EsActivo = ClienteSeleccionado.Activo;
            }
        }

        /// <summary> Evento para notificar cambios en las propiedades a la Vista. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}