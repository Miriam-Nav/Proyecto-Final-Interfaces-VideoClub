using Model;
using Model.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel.Services
{
    /// <summary>
    /// Servicio encargado de coordinar la lógica de negocio para la configuración del sistema.
    /// Actúa como intermediario entre la vista y el repositorio, gestionando validaciones
    /// de Tarifas y Géneros.
    /// </summary>
    public class ConfiguracionService
    {
        
        /// <summary> Referencia al repositorio encargado del acceso a datos de configuración. </summary>
        private readonly ConfiguraciónRepositorio _repo = new ConfiguraciónRepositorio();

        // --- GESTIÓN DE TARIFAS ---
        /// <summary>
        /// Obtiene el catálogo completo de tarifas disponibles para los alquileres.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Tarifas"/>.</returns>
        public List<Tarifas> ObtenerTarifas()
        {
            return _repo.ObtenerTarifas();
        }


        /// <summary>
        /// Valida y registra una nueva tarifa en el sistema.
        /// </summary>
        /// <param name="nueva">Objeto con la descripción y precio de la nueva tarifa.</param>
        /// <exception cref="Exception">Lanzada cuando el precio es menor o igual a cero o falta la descripción.</exception>
        public void GuardarTarifa(Tarifas nueva)
        {
            // No permitir precios negativos o cero
            if (nueva.PrecioDia <= 0)
            {
                throw new Exception("El precio de la tarifa debe ser mayor que 0.");
            }

            if (string.IsNullOrWhiteSpace(nueva.Descripcion))
            {
                throw new Exception("La descripción de la tarifa es obligatoria.");
            }
            _repo.GuardarTarifa(nueva);
        }

        /// <summary>
        /// Procesa la actualización de una tarifa existente previo control de valores.
        /// </summary>
        /// <param name="tarifaEditada">Objeto con los datos actualizados.</param>
        /// <exception cref="Exception">Lanzada si el nuevo precio no cumple con el mínimo requerido.</exception>
        public void ActualizarTarifa(Tarifas tarifaEditada)
        {
            if (tarifaEditada.PrecioDia <= 0)
            { 
                throw new Exception("El precio actualizado debe ser mayor que 0.");
            }
            _repo.ActualizarTarifa(tarifaEditada);
        }

        /// <summary>
        /// Elimina una tarifa del sistema siempre que no esté asignada a ningún producto.
        /// </summary>
        /// <param name="id">ID de la tarifa a eliminar.</param>
        /// <exception cref="Exception">Se lanza si hay productos vinculados a esta tarifa.</exception>
        public void EliminarTarifa(int id)
        {
            if (_repo.TarifaEnUso(id))
            {
                throw new Exception("No se puede eliminar la tarifa: hay productos en el catálogo que la están utilizando.");
            }

            _repo.EliminarTarifa(id);
        }


        // --- GESTIÓN DE GÉNEROS ---
        /// <summary>
        /// Recupera todos los géneros cinematográficos registrados.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Generos"/>.</returns>
        public List<Generos> ObtenerGeneros()
        {
            return _repo.ObtenerGeneros();
        }

        /// <summary>
        /// Valida y guarda un nuevo género en la base de datos.
        /// </summary>
        /// <param name="nuevo">Objeto con la información del género.</param>
        /// <exception cref="Exception">Lanzada si el nombre del género es nulo o vacío.</exception>
        public void GuardarGenero(Generos nuevo)
        {
            if (string.IsNullOrWhiteSpace(nuevo.Nombre))
            { 
                throw new Exception("El nombre del género no puede estar vacío.");
            }
            _repo.GuardarGenero(nuevo);
        }

        /// <summary>
        /// Actualiza un género existente asegurando que el nombre siga siendo válido.
        /// </summary>
        /// <param name="generoEditado">Objeto de género con los nuevos datos.</param>
        /// <exception cref="Exception">Lanzada si el nuevo nombre no es válido.</exception>
        public void ActualizarGenero(Generos generoEditado)
        {
            if (string.IsNullOrWhiteSpace(generoEditado.Nombre))
            {
                throw new Exception("El nombre del género no puede estar vacío.");
            }
            _repo.ActualizarGenero(generoEditado);
        }

        /// <summary>
        /// Elimina un género del sistema siempre que no existan películas o juegos vinculados a él.
        /// </summary>
        /// <param name="id">ID del género a eliminar.</param>
        /// <exception cref="Exception">Se lanza si el género tiene productos asociados.</exception>
        public void EliminarGenero(int id)
        {
            if (_repo.GeneroEnUso(id))
            {
                throw new Exception("No se puede eliminar el género: existen productos en el catálogo clasificados en esta categoría.");
            }

            _repo.EliminarGenero(id);
        }
    }
}