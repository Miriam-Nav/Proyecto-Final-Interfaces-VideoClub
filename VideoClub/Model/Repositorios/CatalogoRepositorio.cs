using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Repositorios
{
    /// <summary>
    /// Repositorio encargado de las operaciones de persistencia para el catálogo de productos.
    /// Gestiona directamente el acceso a la base de datos para la entidad Productos, 
    /// incluyendo la carga de relaciones con Géneros y Tarifas, y el control de niveles de stock.
    /// </summary>
    public class CatalogoRepositorio
    {
        /// <summary>
        /// Realiza una consulta a la base de datos para extraer la información necesaria del informe.
        /// </summary>
        /// <returns>
        /// Una lista de objetos anónimos con los datos de nombres, titulos, precios y stock de los productos.
        /// </returns>
        public IEnumerable<object> DatosRepo()
        {
            using (var db = new VideoClubEntities())
            {
                return db.Productos.Select(p => new
                {
                    Titulo = p.Titulo,
                    GeneroNombre = p.Generos.Nombre,
                    Stock = p.Stock,
                    PrecioTarifa = p.Tarifas.PrecioDia,
                    IdProducto = p.Id
                }).ToList();
            }
        }

        /// <summary>
        /// Obtiene la lista completa de productos registrados en el sistema.
        /// </summary>
        /// <returns>Una lista de objetos de tipo Productos.</returns>
        public List<Productos> ObtenerTodos()
        {
            using (var db = new VideoClubEntities())
            {
                // Obtener todos los productos
                return db.Productos.Include("Generos").Include("Tarifas").ToList();
            }
        }

        /// <summary>
        /// Obtiene el total de productos en inventario.
        /// </summary>
        public int ObtenerTotalProductos()
        {
            using (var db = new VideoClubEntities())
            {
                return db.Productos.Count();
            }
        }

        /// <summary>
        /// Calcula cuántos productos tienen un stock menor o igual a 2.
        /// </summary>
        public int ObtenerStockBajo()
        {
            using (var db = new VideoClubEntities())
            {
                return db.Productos.Count(p => p.Stock <= 2);
            }
        }

        /// <summary>
        /// Obtiene la lista completa de generos registrados en el sistema.
        /// </summary>
        /// <returns>Una lista de objetos de tipo Generos.</returns>
        public List<Generos> ObtenerGeneros()
        {
            using (var db = new VideoClubEntities())
            {
                return db.Generos.ToList();
            }
        }

        /// <summary>
        /// Obtiene la lista completa de tarifas registradas en el sistema.
        /// </summary>
        /// <returns>Una lista de objetos de tipo Tarifas.</returns>
        public List<Tarifas> ObtenerTarifas()
        {
            using (var db = new VideoClubEntities())
            {
                return db.Tarifas.ToList();
            }
        }

        /// <summary>
        /// Registra un nuevo producto en la base de datos.
        /// </summary>
        /// <param name="nuevo">Objeto con la información del producto a insertar.</param>
        public void Guardar(Productos nuevo)
        {
            using (var db = new VideoClubEntities())
            {
                db.Productos.Add(nuevo);
                // Guardar un nuevo producto
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza la información de un producto ya existente.
        /// </summary>
        /// <param name="productoEditado">Objeto con los datos actualizados.</param>
        public void Actualizar(Productos productoEditado)
        {
            using (var db = new VideoClubEntities())
            {
                // Busca el registro original por ID
                var original = db.Productos.Find(productoEditado.Id);
                if (original != null)
                {
                    // Copia los valores nuevos sobre el registro encontrado
                    db.Entry(original).CurrentValues.SetValues(productoEditado);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina un producto de la base de datos de forma permanente.
        /// </summary>
        /// <param name="id">Identificador único del producto a eliminar.</param>
        public void Eliminar(int id)
        {
            using (var db = new VideoClubEntities())
            {
                var producto = db.Productos.Find(id);
                if (producto != null)
                {
                    db.Productos.Remove(producto);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Busca productos que coincidan con el título.
        /// </summary>
        /// <param name="titulo">Texto a buscar en el título del producto.</param>
        /// <returns>Lista de productos filtrada.</returns>
        public List<Productos> BuscarPorTitulo(string titulo)
        {
            using (var db = new VideoClubEntities())
            {
                return db.Productos
                    .Where(p => p.Titulo.Contains(titulo))
                    .ToList();
            }
        }
    }
}
