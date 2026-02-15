using System.Data.SqlClient;
using ViewModel.Services;

namespace Test
{
    /// <summary>
    /// Clase de pruebas para validar la lógica de negocio y la integración del sistema VideoClub.
    /// Contiene pruebas unitarias de validación y pruebas de persistencia de datos.
    /// </summary>
    [TestClass]
    public sealed class TestVideoClub
    {
        /// <summary>
        /// Prueba Unitaria: Verifica que el sistema rechace valores de stock negativos.
        /// Requisito: El stock de los productos siempre debe ser mayor o igual a cero.
        /// </summary>
        [TestMethod]
        public void TestStockNegativo()
        {
            var servicio = new CatalogoService();
            bool resultado = servicio.ValidarStock(-5);
            Assert.IsFalse(resultado, "Error. El stock no puede ser negativo.");
        }

        /// <summary>
        /// Prueba Unitaria: Valida que el formato del correo electrónico de los clientes sea correcto.
        /// Comprueba que se detecten emails mal formados (sin el símbolo '@' o punto).
        /// </summary>
        [TestMethod]
        public void TestEmailFormatoIncorrecto()
        {
            var servicio = new ClienteService();
            bool resultado = servicio.ValidarFormatoEmail("usuario.com"); 
            Assert.IsFalse(resultado, "Error. Formato de email incorrecto.");
        }


        /// <summary>
        /// Prueba de Integración: Verifica la conexión directa con la base de datos de Clientes.
        /// </summary>
        [TestMethod]
        public void TestConexionDirectaBD()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=VideoClub;User Id=sa;Password=cont123;TrustServerCertificate=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Si llega aquí, la conexión es buena
                    Assert.AreEqual(System.Data.ConnectionState.Open, conn.State, "La conexión debería estar abierta.");
                }
                catch (System.Exception ex)
                {
                    // Si falla, el test nos lanza un mensaje
                    Assert.Fail("Error. No se pudo conectar a la base de datos. " + ex.Message);
                }
            }
        }
        
    }
}
