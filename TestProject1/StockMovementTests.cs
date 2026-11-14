using System;
using Domain.Entities;
using Domain.Enums;
using Xunit;

namespace TestProject1
{
    public class StockMovementTests
    {
        [Fact]
        public void Constructor_WithValidIngredientEntry_CreatesMovement()
        {
            var movement = new StockMovement(
                quantity: 5m,
                movementType: StockMovementType.In,
                reason: "Reposición de leche",
                userId: 1,
                productId: null,
                ingredientId: 2,
                orderDetailId: null);

            Assert.Equal(5m, movement.Quantity);
            Assert.Equal(StockMovementType.In, movement.MovementType);
            Assert.Equal((int?)2, movement.IngredientId);
            Assert.Null(movement.ProductId);
            Assert.Null(movement.OrderDetailId);
            Assert.True(movement.Validate());
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(null, null)]
        public void Constructor_WithInvalidTarget_Throws(
            int? productId,
            int? ingredientId)
        {
            Assert.Throws<ArgumentException>(() =>
                new StockMovement(
                    quantity: 5m,
                    movementType: StockMovementType.In,
                    reason: "Reposición",
                    userId: 1,
                    productId: productId,
                    ingredientId: ingredientId,
                    orderDetailId: null));
        }

        [Fact]
        public void Constructor_WithEntryLinkedToOrderDetail_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new StockMovement(
                    quantity: 5m,
                    movementType: StockMovementType.In,
                    reason: "Reposición",
                    userId: 1,
                    productId: null,
                    ingredientId: 2,
                    orderDetailId: 3));
        }

        [Fact]
        public void Constructor_WithExitWithoutOrderDetail_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new StockMovement(
                    quantity: 1m,
                    movementType: StockMovementType.Out,
                    reason: "Preparación",
                    userId: 1,
                    productId: null,
                    ingredientId: 2,
                    orderDetailId: null));
        }

        [Fact]
        public void Constructor_WithValidPreparationExit_CreatesMovement()
        {
            var movement = new StockMovement(
                quantity: 0.6m,
                movementType: StockMovementType.Out,
                reason: "Preparación de tres cafés",
                userId: 1,
                productId: null,
                ingredientId: 2,
                orderDetailId: 3);

            Assert.Equal(StockMovementType.Out, movement.MovementType);
            Assert.Equal((int?)3, movement.OrderDetailId);
            Assert.True(movement.Validate());
        }
    }
}