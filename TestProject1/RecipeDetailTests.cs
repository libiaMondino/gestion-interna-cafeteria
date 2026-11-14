using System;
using Domain.Entities;
using Xunit;

namespace TestProject1
{
    public class RecipeDetailTests
    {
        [Fact]
        public void CalculateConsumption_MultipliesQuantityByUnits()
        {
            // Cada producto requiere 0,2 litros del insumo.
            var detail = new RecipeDetail(
                productId: 1,
                ingredientId: 1,
                requiredQuantity: 0.2m);

            var consumption = detail.CalculateConsumption(3);

            Assert.Equal(0.6m, consumption);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void CalculateConsumption_WithInvalidUnits_Throws(int units)
        {
            var detail = new RecipeDetail(
                productId: 1,
                ingredientId: 1,
                requiredQuantity: 0.2m);

            Assert.Throws<ArgumentException>(() =>
                detail.CalculateConsumption(units));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_WithInvalidQuantity_Throws(int quantity)
        {
            Assert.Throws<ArgumentException>(() =>
                new RecipeDetail(
                    productId: 1,
                    ingredientId: 1,
                    requiredQuantity: quantity));
        }
    }
}