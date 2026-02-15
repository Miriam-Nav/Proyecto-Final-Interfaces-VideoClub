using Model;
using Model.Repositorios;
using System;
using System.Collections.Generic;

namespace ViewModel.Services
{
    /// <summary>
    /// Servicio encargado de la lógica de negocio para el catálogo de productos.
    /// Gestiona las reglas de inventario antes de persistir los datos.
    /// </summary>
    public class CatalogoService
    {
        private readonly CatalogoRepositorio _repo = new CatalogoRepositorio();

        /// <summary>
        /// Obtiene los datos procesados de los productos desde el repositorio para su visualización en informes.
        /// </summary>
        /// <returns>
        /// Una colección de objetos que contiene la información detallada de los productos.
        /// </returns>
        public IEnumerable<object> SeleccionarDatosRepo()
        {
            return _repo.DatosRepo();
        }

        /// <summary> Recupera todos los productos incluyendo sus relaciones (Género y Tarifa). </summary>
        /// <returns>Una lista de objetos <see cref="Productos"/> con sus datos maestros cargados.</returns>
        public List<Productos> ObtenerTodos() 
        { 
            return _repo.ObtenerTodos(); 
        }

        /// <summary> Obtiene la lista de géneros disponibles para asignar a los productos. </summary>
        /// <returns>Colección completa de la tabla <see cref="Generos"/>.</returns>
        public List<Generos> ObtenerGeneros()
        {
            return _repo.ObtenerGeneros();
        }

        /// <summary> Obtiene la lista de tarifas disponibles para asignar a los productos. </summary>
        /// /// <returns>Colección completa de la tabla <see cref="Tarifas"/>.</returns>
        public List<Tarifas> ObtenerTarifas()
        {
            return _repo.ObtenerTarifas();
        }

        /// <summary> Devuelve el número de productos con stock crítico (menor o igual a 2). </summary>
        /// <returns>Un número entero que indica cuántos productos requieren reposición inmediata.</returns>
        public int ObtenerStockBajo()
        {
            return _repo.ObtenerStockBajo();
        }

        /// <summary> Devuelve el total de productos. </summary>
        /// <returns>La cantidad total de registros en la tabla de productos.</returns>
        public int ObtenerTotalProductos()
        {
            return _repo.ObtenerTotalProductos();
        }


        /// <summary>
        /// Valida y guarda un nuevo producto en el catálogo.
        /// </summary>
        /// <param name="nuevo">Producto a registrar.</param>
        public void Guardar(Productos nuevo)
        {
            ValidarProducto(nuevo);
            _repo.Guardar(nuevo);
        }

        /// <summary>
        /// Actualiza los datos de un producto existente.
        /// </summary>
        /// <param name="editado">Producto con cambios.</param>
        public void Actualizar(Productos editado)
        {
            ValidarProducto(editado);
            _repo.Actualizar(editado);
        }

        /// <summary>
        /// Elimina un producto del catálogo.
        /// </summary>
        /// <param name="id">ID del producto.</param>
        public void Eliminar(int id)
        {
            _repo.Eliminar(id);
        }

        /// <summary>
        /// Valida que el título del producto no sea nulo ni vacío.
        /// </summary>
        public bool ValidarTitulo(string titulo)
        {
            return !string.IsNullOrWhiteSpace(titulo);
        }

        /// <summary>
        /// Verifica que el stock sea un valor positivo o cero.
        /// </summary>
        public bool ValidarStock(int stock)
        {
            return stock >= 0;
        }

        /// <summary>
        /// Comprueba que se haya seleccionado una relación válida (Género o Tarifa).
        /// </summary>
        public bool ValidarRelacionId(int id)
        {
            return id > 0;
        }

        /// <summary>
        /// Reglas de negocio para los productos.
        /// </summary>
        /// <param name="p">El objeto <see cref="Productos"/> a validar.</param>
        /// <exception cref="Exception">
        /// Se lanza un mensaje de error descriptivo en los siguientes casos:
        /// <list type="bullet">
        /// <item><description>Si el título está vacío o solo contiene espacios en blanco.</description></item>
        /// <item><description>Si el valor de stock es inferior a cero.</description></item>
        /// <item><description>Si no se ha seleccionado un género válido (ID menor o igual a 0).</description></item>
        /// <item><description>Si no se ha seleccionado una tarifa válida (ID menor o igual a 0).</description></item>
        /// </list>
        /// </exception>
        private void ValidarProducto(Productos p)
        {
            if (!ValidarTitulo(p.Titulo))
            {
                throw new Exception("El título del producto es obligatorio.");
            }
            if (!ValidarStock(p.Stock))
            {
                throw new Exception("El stock no puede ser un número negativo.");
            }
            if (!ValidarRelacionId(p.GeneroId))
            {
                throw new Exception("Debes seleccionar un género para el producto.");
            }
            if (!ValidarRelacionId(p.TarifaId))
            {
                throw new Exception("Debes asignar una tarifa al producto.");
            }
        }
    }
}