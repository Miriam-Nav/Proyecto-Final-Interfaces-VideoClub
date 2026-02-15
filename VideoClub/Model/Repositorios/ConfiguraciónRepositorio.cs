using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Repositorios
{
    /// <summary>
    /// Repositorio encargado de las operaciones de persistencia para la configuración del sistema.
    /// Gestiona directamente el acceso a la base de datos para las entidades Tarifas y Géneros 
    /// utilizando Entity Framework.
    /// </summary>
    public class ConfiguraciónRepositorio
    {
        // --- GESTIÓN DE TARIFAS ---

        /// <summary>
        /// Obtiene la lista completa de tarifas registradas en el sistema.
        /// </summary>
        /// <returns>Una lista de objetos de tipo Tarifas.</returns>
        public List<Tarifas> ObtenerTarifas()
        {
            using (var db = new VideoClubEntities())
            {
                // Obtener todas las tarifas
                return db.Tarifas.ToList();
            }
        }

        /// <summary>
        /// Comprueba si una tarifa específica está siendo utilizada por algún producto del catálogo.
        /// </summary>
        /// <param name="tarifaId">ID de la tarifa a consultar.</param>
        /// <returns>True si hay productos con esta tarifa; False en caso contrario.</returns>
        public bool TarifaEnUso(int tarifaId)
        {
            using (var db = new VideoClubEntities())
            {
                return db.Productos.Any(p => p.TarifaId == tarifaId);
            }
        }

        /// <summary>
        /// Registra una nueva tarifa en la base de datos.
        /// </summary>
        /// <param name="nueva">Objeto con la información de la tarifa a insertar.</param>
        public void GuardarTarifa(Tarifas nueva)
        {
            using (var db = new VideoClubEntities())
            {
                db.Tarifas.Add(nueva);
                // Guardar una nueva tarifa
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza la información de una tarifa ya existente.
        /// </summary>
        /// <param name="tarifaEditada">Objeto con los datos actualizados (Descripcion, PrecioDia).</param>
        public void ActualizarTarifa(Tarifas tarifaEditada)
        {
            using (var db = new VideoClubEntities())
            {
                // Busca el registro original por ID
                var original = db.Tarifas.Find(tarifaEditada.Id);
                if (original != null)
                {
                    // Copia los valores nuevos sobre el registro encontrado
                    db.Entry(original).CurrentValues.SetValues(tarifaEditada);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina una tarifa de la base de datos de forma permanente.
        /// </summary>
        /// <param name="id">Identificador único de la tarifa a eliminar.</param>
        public void EliminarTarifa(int id)
        {
            using (var db = new VideoClubEntities())
            {
                var tarifa = db.Tarifas.Find(id);
                if (tarifa != null)
                {
                    db.Tarifas.Remove(tarifa);
                    db.SaveChanges();
                }
            }
        }


        // --- GESTIÓN DE GÉNEROS ---

        /// <summary>
        /// Obtiene la lista completa de géneros registrados en el sistema.
        /// </summary>
        /// <returns>Una lista de objetos de tipo Generos.</returns>
        public List<Generos> ObtenerGeneros()
        {
            using (var db = new VideoClubEntities())
            {
                // Obtener todos los géneros
                return db.Generos.ToList();
            }
        }

        /// <summary>
        /// Verifica si un género está asignado a algún producto del catálogo.
        /// </summary>
        /// <param name="generoId">ID del género a comprobar.</param>
        /// <returns>True si existen productos de este género.</returns>
        public bool GeneroEnUso(int generoId)
        {
            using (var db = new VideoClubEntities())
            {
                return db.Productos.Any(p => p.GeneroId == generoId);
            }
        }

        /// <summary>
        /// Registra un nuevo género en la base de datos.
        /// </summary>
        /// <param name="nuevo">Objeto con la información del género a insertar.</param>
        public void GuardarGenero(Generos nuevo)
        {
            using (var db = new VideoClubEntities())
            {
                db.Generos.Add(nuevo);
                // Guardar un nuevo género
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza la información de un género ya existente.
        /// </summary>
        /// <param name="generoEditado">Objeto con el nombre actualizado.</param>
        public void ActualizarGenero(Generos generoEditado)
        {
            using (var db = new VideoClubEntities())
            {
                // Busca el registro original por ID
                var original = db.Generos.Find(generoEditado.Id);
                if (original != null)
                {
                    // Copia los valores nuevos sobre el registro encontrado
                    db.Entry(original).CurrentValues.SetValues(generoEditado);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina un género de la base de datos de forma permanente.
        /// </summary>
        /// <param name="id">Identificador único del género a eliminar.</param>
        public void EliminarGenero(int id)
        {
            using (var db = new VideoClubEntities())
            {
                var genero = db.Generos.Find(id);
                if (genero != null)
                {
                    db.Generos.Remove(genero);
                    db.SaveChanges();
                }
            }
        }
    }
}
