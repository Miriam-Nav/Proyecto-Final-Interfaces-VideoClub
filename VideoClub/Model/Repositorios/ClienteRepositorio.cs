using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Repositorios
{
    /// <summary>
    /// Repositorio encargado de las operaciones de persistencia para la gestión de clientes.
    /// Gestiona directamente el acceso a la base de datos para la entidad Clientes 
    /// utilizando Entity Framework, permitiendo realizar operaciones CRUD y consultas de conteo.
    /// </summary>
    public class ClienteRepositorio
    {
        /// <summary>
        /// Realiza una consulta a la base de datos para extraer la información necesaria del informe.
        /// </summary>
        /// <returns>
        /// Una lista de objetos anónimos con los datos de nombres, emails, telefonos y estados de los clientes.
        /// </returns>
        public IEnumerable<object> DatosRepo()
        {
            using (var db = new VideoClubEntities())
            {
                return db.Clientes
                    .Select(c => new
                    {
                        Nombre = c.Nombre,
                        Email = c.Email,
                        Telefono = c.Telefono,
                        Activo = c.Activo
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Obtiene la lista completa de clientes registrados en el sistema.
        /// </summary>
        /// <returns>Una lista de objetos de tipo Clientes.</returns>
        public List<Clientes> ObtenerTodos()
        {
            using (var db = new VideoClubEntities())
            {
                // Obtener todos los clientes
                return db.Clientes.ToList();
            }
        }

        /// <summary>
        /// Obtiene el número total de clientes registrados en la base de datos.
        /// </summary>
        public int ObtenerTotalClientes()
        {
            using (var db = new VideoClubEntities())
            {
                return db.Clientes.Count();
            }
        }

        /// <summary>
        /// Consulta si un cliente específico tiene productos alquilados que aún no han sido devueltos.
        /// </summary>
        /// <param name="clienteId">ID del cliente a consultar.</param>
        /// <returns>True si tiene al menos un alquiler con la fecha de devolución real pendiente (null).</returns>
        public bool TieneAlquileresPendientes(int clienteId)
        {
            using (var db = new VideoClubEntities())
            {
                // Un alquiler está pendiente si la FechaDevolucionReal es NULL
                return db.Alquileres.Any(a => a.ClienteId == clienteId && a.FechaDevolucionReal == null);
            }
        }

        /// <summary>
        /// Registra un nuevo cliente en la base de datos.
        /// </summary>
        /// <param name="nuevo">Objeto con la información del cliente a insertar.</param>
        public void Guardar(Clientes nuevo)
        {
            using (var db = new VideoClubEntities())
            {
                db.Clientes.Add(nuevo);
                // Guardar un nuevo cliente
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza la información de un cliente ya existente.
        /// </summary>
        /// <param name="clienteEditado">Objeto con los datos actualizados.</param>
        public void Actualizar(Clientes clienteEditado)
        {
            using (var db = new VideoClubEntities())
            {
                // Busca el registro original por ID
                var original = db.Clientes.Find(clienteEditado.Id);
                if (original != null)
                {
                    // Copia los valores nuevos sobre el registro encontrado
                    db.Entry(original).CurrentValues.SetValues(clienteEditado);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina un cliente de la base de datos de forma permanente.
        /// </summary>
        /// <param name="id">Identificador único del cliente a eliminar.</param>
        public void Eliminar(int id)
        {
            using (var db = new VideoClubEntities())
            {
                var cliente = db.Clientes.Find(id);
                if (cliente != null)
                {
                    db.Clientes.Remove(cliente);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Busca clientes que coincidan con el nombre.
        /// </summary>
        /// <param name="nombre">Texto a buscar en el nombre del cliente.</param>
        /// <returns>Lista de clientes filtrada.</returns>
        public List<Clientes> BuscarPorNombre(string nombre)
        {
            using (var db = new VideoClubEntities())
            {
                return db.Clientes
                    .Where(c => c.Nombre.Contains(nombre))
                    .ToList();
            }
        }

        /// <summary>
        /// Busca si ya existe un cliente con el correo electrónico especificado.
        /// </summary>
        /// <param name="email">Email a comprobar.</param>
        /// <returns>El objeto Clientes si lo encuentra, o null si no existe.</returns>
        public Clientes BuscarPorEmail(string email)
        {
            using (var db = new VideoClubEntities())
            {
                // Busca el primer cliente que coincida con el email
                return db.Clientes.FirstOrDefault(c => c.Email == email);
            }
        }
    }
}
