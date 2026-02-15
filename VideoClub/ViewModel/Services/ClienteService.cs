using Model;
using Model.Repositorios;
using System;
using System.Collections.Generic;

namespace ViewModel.Services
{
    /// <summary>
    /// Servicio encargado de la lógica de negocio para la gestión de clientes.
    /// Valida los datos antes de permitir su persistencia a través del repositorio.
    /// </summary>
    public class ClienteService
    {
        private readonly ClienteRepositorio _repo = new ClienteRepositorio();

        /// <summary>
        /// Obtiene los datos procesados de los clientes desde el repositorio para su visualización en informes.
        /// </summary>
        /// <returns>
        /// Una colección de objetos que contiene la información detallada de los clientes.
        /// </returns>
        public IEnumerable<object> SeleccionarDatosRepo()
        {
            return _repo.DatosRepo();
        }

        /// <summary>
        /// Recupera todos los clientes registrados.
        /// </summary>
        /// <returns>Una lista completa de objetos <see cref="Clientes"/> existentes en la base de datos.</returns>
        public List<Clientes> ObtenerTodos()
        {
            return _repo.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene el conteo total de clientes para el dashboard.
        /// </summary>
        /// <returns>Un número entero que representa la cantidad total de clientes registrados.</returns>
        public int ObtenerTotalClientes()
        {
            return _repo.ObtenerTotalClientes();
        }

        /// <summary>
        /// Realiza una búsqueda de clientes por nombre.
        /// </summary>
        /// <param name="nombre">El nombre o fragmento de texto a buscar.</param>
        /// <returns>Una lista de <see cref="Clientes"/> cuyos nombres coincidan con el criterio de búsqueda.</returns>
        public List<Clientes> Buscar(string nombre)
        {
            return _repo.BuscarPorNombre(nombre);
        }

        /// <summary>
        /// Valida y registra un nuevo cliente.
        /// </summary>
        /// <param name="nuevo">Datos del cliente a registrar.</param>
        /// <exception cref="Exception">Lanzada si el nombre o el DNI no son válidos.</exception>
        public void Guardar(Clientes nuevo)
        {
            ValidarCliente(nuevo);
            _repo.Guardar(nuevo);
        }

        /// <summary>
        /// Actualiza los datos de un cliente existente tras validar los cambios.
        /// </summary>
        public void Actualizar(Clientes clienteEditado)
        {
            ValidarCliente(clienteEditado);
            _repo.Actualizar(clienteEditado);
        }

        /// <summary>
        /// Elimina un cliente del sistema.
        /// </summary>
        /// <param name="id">ID único del cliente a eliminar.</param>
        /// <exception cref="Exception">
        /// Se lanza si el cliente tiene alquileres activos (productos sin devolver), 
        /// para evitar la pérdida de trazabilidad del stock.
        /// </exception>
        public void Eliminar(int id)
        {
            if (_repo.TieneAlquileresPendientes(id))
            {
                throw new Exception("No se puede eliminar al cliente: tiene productos pendientes de devolución.");
            }
            _repo.Eliminar(id);
        }

        /// <summary>
        /// Comprueba el formato básico de un email.
        /// </summary>
        public bool ValidarFormatoEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return email.Contains("@") && email.Contains(".");
        }

        /// <summary>
        /// Valida que el teléfono tenga 9 dígitos y sean numéricos.
        /// </summary>
        public bool ValidarTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return true; // Es opcional
            return telefono.Length >= 9 && long.TryParse(telefono, out _);
        }

        /// <summary>
        /// Centraliza las reglas de negocio para los campos del formulario de Clientes.
        /// </summary>
        /// <param name="c">Instancia del cliente a validar.</param>
        /// <exception cref="Exception">
        /// Se lanza un mensaje descriptivo en los siguientes casos:
        /// <list type="bullet">
        /// <item><description>Nombre o Email están vacíos o nulos.</description></item>
        /// <item><description>El formato del Email no contiene caracteres esenciales ('@' o '.').</description></item>
        /// <item><description>El Email ya pertenece a otro cliente registrado (duplicidad).</description></item>
        /// <item><description>El teléfono contiene caracteres no numéricos o tiene una longitud insuficiente.</description></item>
        /// </list>
        /// </exception>
        private void ValidarCliente(Clientes c)
        {
            // Validación de Nombre Completo
            if (string.IsNullOrWhiteSpace(c.Nombre))
            {
                throw new Exception("El nombre completo es obligatorio.");
            }

            if (!ValidarFormatoEmail(c.Email))
            {
                throw new Exception("El formato del correo electrónico no es válido.");
            }
            if (!ValidarTelefono(c.Telefono))
            {
                throw new Exception("El teléfono debe ser numérico y tener al menos 9 dígitos.");
            }

            var clienteExistente = _repo.BuscarPorEmail(c.Email);

            // Si encuentra un cliente y NO es el mismo que se está editando
            if (clienteExistente != null && clienteExistente.Id != c.Id)
            {
                throw new Exception("Este correo electrónico ya está registrado por otro cliente.");
            }
        }
    }
}
