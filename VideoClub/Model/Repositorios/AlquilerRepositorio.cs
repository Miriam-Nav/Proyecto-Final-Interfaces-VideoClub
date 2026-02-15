using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
namespace Model.Repositorios
{
    /// <summary>
    /// Repositorio encargado de las operaciones de persistencia para la gestión de alquileres.
    /// Administra las transacciones de préstamo y devolución, coordinando la relación 
    /// entre clientes y productos, y asegurando la integridad del stock durante cada operación.
    /// </summary>
    public class AlquilerRepositorio
    {
        /// <summary>
        /// Realiza una consulta proyectada a la base de datos para extraer la información necesaria del informe.
        /// </summary>
        /// <returns>
        /// Una lista de objetos anónimos con los datos de alquileres, productos y clientes.
        /// </returns>
        public IEnumerable<object> DatosRepo()
        {
            using (var db = new VideoClubEntities())
            {
                return db.Alquileres
                    .Select(a => new
                    {
                        NombreCliente = a.Clientes.Nombre,
                        NombreProducto = a.Productos.Titulo,
                        Importe = a.CosteTotal,
                        IdAlquiler = a.Id
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Obtiene el historial completo de alquileres, incluyendo datos de clientes y productos.
        /// </summary>
        /// <returns>Lista de alquileres con sus relaciones cargadas.</returns>
        public List<Alquileres> ObtenerTodos()
        {
            using (var db = new VideoClubEntities())
            {
                return db.Alquileres
                         .Include(a => a.Clientes)
                         .Include(a => a.Productos)
                         .OrderByDescending(a => a.FechaSalida)
                         .ToList();
            }
        }

        /// <summary>
        /// Obtiene el conteo de alquileres que aún no tienen fecha de devolución real.
        /// </summary>
        public int ObtenerAlquileresActivos()
        {
            using (var db = new VideoClubEntities())
            {
                // Si FechaDevolucionReal es null, es que el cliente aún tiene el producto
                return db.Alquileres.Count(a => a.FechaDevolucionReal == null);
            }
        }

        /// <summary>
        /// Obtiene los 10 últimos alquileres realizados.
        /// </summary>
        public List<Alquileres> ObtenerActividadReciente()
        {
            using (var db = new VideoClubEntities())
            {
                return db.Alquileres
                         .Include("Clientes")  
                         .Include("Productos") 
                         .OrderByDescending(a => a.FechaSalida)
                         .Take(10)
                         .ToList();
            }
        }

        /// <summary>
        /// Registra un nuevo alquiler, calcula el precio y reduce el stock del producto.
        /// </summary>
        /// <param name="nuevo">Objeto con los datos básicos del alquiler.</param>
        public void GuardarAlquiler(Alquileres nuevo)
        {
            using (var db = new VideoClubEntities())
            {
                // Obtener el producto para acceder a su tarifa y stock
                var producto = db.Productos.Include(p => p.Tarifas).FirstOrDefault(p => p.Id == nuevo.ProductoId);

                if (producto != null && producto.Stock > 0)
                {
                    // Calcular días de alquiler 
                    TimeSpan diferencia = nuevo.FechaPrevistaDevolucion - nuevo.FechaSalida;

                    // Mínimo 1 día
                    int dias = 1;

                    if (diferencia.Days > 0)
                    {
                        dias = diferencia.Days;
                    }

                    // Calcular precio total 
                    decimal total = dias * producto.Tarifas.PrecioDia;

                    if (nuevo.FechaDevolucionReal != null && nuevo.FechaPrevistaDevolucion != nuevo.FechaDevolucionReal)
                    {
                        nuevo.CosteTotal = total * 1.50m;
                    }
                    else
                    {
                        nuevo.CosteTotal = total;
                    }

                    // Restar una unidad al stock del producto
                    producto.Stock = producto.Stock - 1;

                    db.Alquileres.Add(nuevo);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Procesa la devolución de un producto, marcándolo como entregado y reponiendo el stock.
        /// </summary>
        /// <param name="alquilerId">ID del registro de alquiler a cerrar.</param>
        public void MarcarComoDevuelto(int alquilerId)
        {
            using (var db = new VideoClubEntities())
            {
                var alquiler = db.Alquileres.Find(alquilerId);

                if (alquiler != null && alquiler.FechaDevolucionReal == null)
                {
                    // Marcar como entregado
                    alquiler.FechaDevolucionReal = DateTime.Now;

                    // Devolver la unidad al stock del producto
                    var producto = db.Productos.Find(alquiler.ProductoId);
                    if (producto != null)
                    {
                        producto.Stock = producto.Stock + 1;
                    }

                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina un registro de alquiler de la base de datos.
        /// </summary>
        /// <param name="id">ID del alquiler a eliminar.</param>
        public void Eliminar(int id)
        {
            using (var db = new VideoClubEntities())
            {
                var alquiler = db.Alquileres.Find(id);
                if (alquiler != null)
                {
                    // Si el alquiler NO tiene fecha de devolución real no a sido devuelto.
                    // Hay devolverlo al stock.
                    if (alquiler.FechaDevolucionReal == null)
                    {
                        var producto = db.Productos.Find(alquiler.ProductoId);
                        if (producto != null)
                        {
                            producto.Stock += 1;
                        }
                    }

                    db.Alquileres.Remove(alquiler);
                    db.SaveChanges();
                }
            }
        }
    }
}
