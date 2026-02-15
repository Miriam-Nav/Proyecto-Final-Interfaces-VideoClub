using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ViewModel.Services;

namespace ViewModel
{
    /// <summary>
    /// ViewModel de la ventana de configuración.
    /// Gestiona la lógica para el mantenimiento de Tarifas y Géneros de los productos.
    /// </summary>
    public class ConfiguracionViewModel : INotifyPropertyChanged
    {
        private readonly ConfiguracionService _service = new ConfiguracionService();

        // --- PROPIEDADES PARA TARIFAS ---

        private List<Tarifas> _tarifas;
        /// <summary> Lista de tarifas registradas. </summary>
        public List<Tarifas> Tarifas
        {
            get => _tarifas;
            set { _tarifas = value; OnPropertyChanged(nameof(Tarifas)); }
        }

        private Tarifas _tarifaSeleccionada;
        /// <summary> Tarifa seleccionada actualmente. </summary>
        public Tarifas TarifaSeleccionada
        {
            get => _tarifaSeleccionada;
            set
            {
                _tarifaSeleccionada = value;
                OnPropertyChanged(nameof(TarifaSeleccionada));
                CargarTarifaEnFormulario();
            }
        }

        private string _inputDescripcionTarifa;
        /// <summary> Descripción de la tarifa. </summary>
        public string InputDescripcionTarifa
        {
            get => _inputDescripcionTarifa;
            set { _inputDescripcionTarifa = value; OnPropertyChanged(nameof(InputDescripcionTarifa)); }
        }

        private string _inputPrecioDia;
        /// <summary> Precio por día de la tarifa. </summary>
        public string InputPrecioDia
        {
            get => _inputPrecioDia;
            set { _inputPrecioDia = value; OnPropertyChanged(nameof(InputPrecioDia)); }
        }

        private string _errorTarifa;
        public string ErrorTarifa
        {
            get => _errorTarifa;
            set { _errorTarifa = value; OnPropertyChanged(nameof(ErrorTarifa)); }
        }


        // --- PROPIEDADES PARA GÉNEROS ---

        private List<Generos> _generos;
        /// <summary> Lista de géneros registrados. </summary>
        public List<Generos> Generos
        {
            get => _generos;
            set { _generos = value; OnPropertyChanged(nameof(Generos)); }
        }

        private Generos _generoSeleccionado;
        /// <summary> Género seleccionado actualmente. </summary>
        public Generos GeneroSeleccionado
        {
            get => _generoSeleccionado;
            set
            {
                _generoSeleccionado = value;
                OnPropertyChanged(nameof(GeneroSeleccionado));
                CargarGeneroEnFormulario();
            }
        }

        private string _inputNombreGenero;
        /// <summary> Nombre del género. </summary>
        public string InputNombreGenero
        {
            get => _inputNombreGenero;
            set { _inputNombreGenero = value; OnPropertyChanged(nameof(InputNombreGenero)); }
        }

        private string _errorGenero;
        public string ErrorGenero
        {
            get => _errorGenero;
            set { _errorGenero = value; OnPropertyChanged(nameof(ErrorGenero)); }
        }


        // --- COMANDOS ---
        /// <summary> Comando para guardar una nueva tarifa o actualizar la seleccionada. </summary>
        public ICommand GuardarTarifaCommand { get; }

        /// <summary> Comando para registrar un nuevo género o modificar el nombre del seleccionado. </summary>
        public ICommand GuardarGeneroCommand { get; }

        /// <summary> Comando para eliminar permanentemente la tarifa seleccionada. </summary>
        public ICommand EliminarTarifaCommand { get; }

        /// <summary> Comando para borrar el género seleccionado de la base de datos. </summary>
        public ICommand EliminarGeneroCommand { get; }

        /// <summary> Comando para resetear todos los campos de texto y limpiar las selecciones de ambas tablas. </summary>
        public ICommand LimpiarCommand { get; }


        /// <summary>
        /// Constructor del ViewModel.
        /// Inicializa los servicios y carga todas las listas de configuración.
        /// </summary>
        public ConfiguracionViewModel()
        {
            GuardarTarifaCommand = new RelayCommand(GuardarTarifa);
            GuardarGeneroCommand = new RelayCommand(GuardarGenero);
            EliminarTarifaCommand = new RelayCommand(EliminarTarifa);
            EliminarGeneroCommand = new RelayCommand(EliminarGenero);
            LimpiarCommand = new RelayCommand(LimpiarTodo);

            RefrescarListas();
        }

        /// <summary>
        /// Consulta el servicio para obtener los datos actualizados de todas las tablas.
        /// </summary>
        private void RefrescarListas()
        {
            Tarifas = _service.ObtenerTarifas();
            Generos = _service.ObtenerGeneros();
        }

        /// <summary>
        /// Procesa el guardado o actualización de una tarifa.
        /// </summary>
        private void GuardarTarifa()
        {
            try
            {
                ErrorTarifa = "";
                decimal.TryParse(InputPrecioDia, out decimal precio);

                if (TarifaSeleccionada == null)
                {
                    var nueva = new Tarifas { Descripcion = InputDescripcionTarifa, PrecioDia = precio };
                    _service.GuardarTarifa(nueva);
                }
                else
                {
                    TarifaSeleccionada.Descripcion = InputDescripcionTarifa;
                    TarifaSeleccionada.PrecioDia = precio;
                    _service.ActualizarTarifa(TarifaSeleccionada);
                }

                RefrescarListas();
                LimpiarTodo();
                ErrorTarifa = "";
            }
            catch (Exception ex)
            {
                ErrorTarifa = ex.Message;
            }
        }

        /// <summary>
        /// Elimina la tarifa seleccionada de la base de datos.
        /// </summary>
        private void EliminarTarifa()
        {
            try
            {
                ErrorTarifa = "";

                if (TarifaSeleccionada != null) { 
                    _service.EliminarTarifa(TarifaSeleccionada.Id);
                    RefrescarListas();
                    LimpiarTodo();
                    ErrorTarifa = "";
                }
            }
            catch (Exception ex)
            {
                ErrorTarifa = ex.Message;
            }
        }


        /// <summary>
        /// Procesa el guardado o actualización de un género.
        /// </summary>
        private void GuardarGenero()
        {
            try
            {
                ErrorGenero = "";

                if (GeneroSeleccionado == null)
                {
                    var nuevo = new Generos { Nombre = InputNombreGenero };
                    _service.GuardarGenero(nuevo);
                }
                else
                {
                    GeneroSeleccionado.Nombre = InputNombreGenero;
                    _service.ActualizarGenero(GeneroSeleccionado);
                }

                RefrescarListas();
                LimpiarTodo();
                ErrorGenero = "";
            }
            catch (Exception ex)
            {
                ErrorGenero = ex.Message;
            }
        }

        /// <summary>
        /// Elimina el género seleccionado de la base de datos.
        /// </summary>
        private void EliminarGenero()
        {
            try
            {
                ErrorGenero = "";
                if (GeneroSeleccionado != null) 
                { 
                    _service.EliminarGenero(GeneroSeleccionado.Id);
                    RefrescarListas();
                    LimpiarTodo();
                    ErrorGenero = "";
                }
            }
            catch (Exception ex)
            {
                ErrorGenero = ex.Message;
            }
        }

        /// <summary>
        /// Carga los datos de la tarifa seleccionada.
        /// </summary>
        private void CargarTarifaEnFormulario()
        {
            if (TarifaSeleccionada != null)
            {
                InputDescripcionTarifa = TarifaSeleccionada.Descripcion;
                InputPrecioDia = TarifaSeleccionada.PrecioDia.ToString();
            }
        }

        /// <summary>
        /// Carga el nombre del género seleccionado.
        /// </summary>
        private void CargarGeneroEnFormulario()
        {
            if (GeneroSeleccionado != null)
            {
                InputNombreGenero = GeneroSeleccionado.Nombre;
            }
        }

        /// <summary>
        /// Restablece todos los campos de entrada de la pantalla.
        /// </summary>
        private void LimpiarTodo()
        {
            InputDescripcionTarifa = string.Empty;
            InputPrecioDia = string.Empty;
            InputNombreGenero = string.Empty;
            TarifaSeleccionada = null;
            GeneroSeleccionado = null;
        }

        /// <summary> Evento para notificar cambios en las propiedades a la Vista. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}