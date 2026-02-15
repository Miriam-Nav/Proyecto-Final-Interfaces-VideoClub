using Model;
using Model.Repositorios;
using System;
using System.Collections.Generic;

namespace ViewModel.Services
{
    /// <summary>
    /// Servicio encargado de la lógica de negocio para la gestión de alquileres.
    /// Coordina las transacciones entre clientes y productos, validando fechas 
    /// y disponibilidad de stock antes de confirmar el préstamo.
    /// </summary>
    public class AlquilerService
    {
        private readonly AlquilerRepositorio _repo = new AlquilerRepositorio();

        /// <summary>
        /// Obtiene los datos procesados de préstamos desde el repositorio para su visualización en informes.
        /// </summary>
        /// <returns>
        /// Una colección de objetos que contiene la información detallada de los clientes y sus productos alquilados.
        /// </returns>
        public IEnumerable<object> SeleccionarDatosRepo()
        {
            return _repo.DatosRepo();
        }

        /// <summary>
        /// Recupera el historial completo de alquileres registrados en el sistema.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="Alquileres"/> con sus relaciones cargadas.</returns>
        public List<Alquileres> ObtenerTodos()
        {
            return _repo.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene los últimos movimientos de alquiler realizados.
        /// </summary>
        /// <returns>Una lista con los 10 alquileres más recientes, incluyendo datos del cliente y el producto.</returns>
        public List<Alquileres> ObtenerActividadReciente()
        {
            return _repo.ObtenerActividadReciente();
        }

        /// <summary>
        /// Calcula el número total de alquileres que aún están pendientes de devolución.
        /// </summary>
        /// <returns>Un número entero que representa la cantidad de alquileres sin fecha de devolución real.</returns>
        public int ObtenerAlquileresActivos()
        {
            return _repo.ObtenerAlquileresActivos();
        }

        /// <summary>
        /// Registra un nuevo alquiler tras validar disponibilidad y coherencia de fechas.
        /// </summary>
        public void GuardarAlquiler(Alquileres nuevo)
        {
            ValidarAlquiler(nuevo);
            _repo.GuardarAlquiler(nuevo);
        }

        /// <summary>
        /// Finaliza un alquiler devolviendo el producto al stock.
        /// </summary>
        public void MarcarComoDevuelto(int id)
        {
            if (id <= 0) 
            { 
                throw new Exception("ID de alquiler no válido."); 
            }
            _repo.MarcarComoDevuelto(id);
        }

        /// <summary>
        /// Elimina un registro de alquiler.
        /// </summary>
        public void EliminarAlquiler(int id)
        {
            _repo.Eliminar(id);
        }

        /// <summary>
        /// Lógica de negocio para alquileres.
        /// <param name="a">Objeto <see cref="Alquileres"/> con los datos a validar.</param>
        /// <exception cref="Exception">
        /// Se lanza en los siguientes casos:
        /// <list type="bullet">
        /// <item><description>Si el cliente no ha sido seleccionado.</description></item>
        /// <item><description>Si el producto no ha sido seleccionado.</description></item>
        /// <item><description>Si la fecha es incorrecta.</description></item>
        /// <item><description>Si se intenta registrar un alquiler con una fecha de inicio pasada.</description></item>
        /// </list>
        /// </exception>
        /// </summary>
        private void ValidarAlquiler(Alquileres a)
        {
            // Validación de selección de entidades
            if (a.ClienteId <= 0)
            {
                throw new Exception("Debe seleccionar un cliente para el alquiler.");
            }
            if (a.ProductoId <= 0)
            {
                throw new Exception("Debe seleccionar un producto del catálogo.");
            }

            // Validación de fechas
            if (a.FechaSalida > a.FechaPrevistaDevolucion)
            {
                throw new Exception("La fecha de devolución no puede ser anterior a la de salida.");
            }
            if (a.FechaSalida < DateTime.Today.AddDays(-1))
            {
                throw new Exception("La fecha de salida no puede ser en el pasado.");
            }
        }
    }
}