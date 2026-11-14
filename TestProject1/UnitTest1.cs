using Domain.Entities;
using Domain.Exceptions;
using Xunit;

namespace TestProject1
{
    public class IngredientTests
    {
        [Fact]
        public void RegisterStockExit_WhenStockIsInsufficient_ThrowsAndKeepsStock()
        {
            // Preparar: tenemos 5 litros de leche.
            var ingredient = new Ingredient("Leche", "Litros", 2);
            ingredient.RegisterStockEntry(5);

            // Retirar 6 debe lanzar una excepción.
            Assert.Throws<BusinessRuleException>(() =>
                ingredient.RegisterStockExit(6));

            // El intento fallido no debe modificar las existencias.
            Assert.Equal(5m, ingredient.QuantityStock);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        public void IsBelowMinimum_ReturnsExpectedResult(
            int currentStock,
            bool expected)
        {
            // Preparar: el mínimo es 2 litros.
            var ingredient = new Ingredient("Leche", "Litros", 2);
            ingredient.RegisterStockEntry(currentStock);

            // Consultar si corresponde mostrar la alerta.
            var result = ingredient.IsBelowMinimum();

            // Comparar con el resultado esperado.
            Assert.Equal(expected, result);
        }
    }
}