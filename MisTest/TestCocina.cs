using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Modelos;

namespace MisTest
{
    [TestClass]
    public class TestCocina
    {
        [TestMethod]
        [ExpectedException(typeof(FileManagerException))]
        public void AlGuardarUnArchivo_ConNombreInvalido_TengoUnaExcepcion()
        {
            //arrange
            string data = "texto";
            string nombreInvalido = "pofj´*~2w";
            bool append = true;

            //act
            FileManager.Guardar(data, nombreInvalido, append);
        }

        [TestMethod]

        public void AlInstanciarUnCocinero_SeEspera_PedidosCero()
        {
            //arrange
            string nombre = "Ramón";

            //act
            Cocinero<Hamburguesa> cocinero = new Cocinero<Hamburguesa>(nombre);

            //assert
            Assert.AreEqual(0, cocinero.CantPedidosFinalizados);
        }
    }
}