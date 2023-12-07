using Entidades.Enumerados;


namespace Entidades.MetodosDeExtension
{
    public static class IngredientesExtension
    {
        public static double CalcularCostoIngredientes(List<EIngrediente> ingredientes, int costoInicial)
        {
            double nuevoCosto = costoInicial;

            foreach (EIngrediente ingrediente in ingredientes)
            {
                nuevoCosto += costoInicial + (int)ingrediente / 100;
            }
            return nuevoCosto;
        }

        public static List<EIngrediente> IngredientesAleatorios(Random rand)
        {
            List<EIngrediente> ingredientes = new List<EIngrediente>()
            {
                EIngrediente.QUESO,
                EIngrediente.PANCETA,
                EIngrediente.ADHERESO,
                EIngrediente.HUEVO,
                EIngrediente.JAMON
            };

            int numeroRandom = rand.Next(1, ingredientes.Count + 1);

            return ingredientes.Take(numeroRandom).ToList();
        }
    }
}
