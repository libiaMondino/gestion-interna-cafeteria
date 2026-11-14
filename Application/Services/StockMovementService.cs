using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class StockMovementService : IStockMovementService
    {
        private readonly IGenericRepository<StockMovement> _movementRepository;
        private readonly IGenericRepository<Ingredient> _ingredientRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly ITransactionManager _transactionManager;

        public StockMovementService(
            IGenericRepository<StockMovement> movementRepository,
            IGenericRepository<Ingredient> ingredientRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<User> userRepository,
            ITransactionManager transactionManager)
        {
            _movementRepository = movementRepository;
            _ingredientRepository = ingredientRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _transactionManager = transactionManager;
        }

        public async Task<List<StockMovementDto>> GetAllAsync()
        {
            var movements = await _movementRepository.GetAllAsync();

            return movements
                .OrderByDescending(m => m.DateTime)
                .ThenByDescending(m => m.Id)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<StockMovementDto?> GetByIdAsync(int id)
        {
            var movement = await _movementRepository.GetIdAsync(id);

            return movement == null ? null : MapToDto(movement);
        }

        public async Task<List<StockMovementDto>> GetByIngredientIdAsync(
            int ingredientId)
        {
            if (ingredientId <= 0)
            {
                throw new ArgumentException(
                    "El identificador del insumo debe ser mayor que cero.");
            }

            var movements = await _movementRepository.GetListByFilterAsync(
                m => m.IngredientId == ingredientId);

            return movements
                .OrderByDescending(m => m.DateTime)
                .ThenByDescending(m => m.Id)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<List<StockMovementDto>> GetByProductIdAsync(
            int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException(
                    "El identificador del producto debe ser mayor que cero.");
            }

            var movements = await _movementRepository.GetListByFilterAsync(
                m => m.ProductId == productId);

            return movements
                .OrderByDescending(m => m.DateTime)
                .ThenByDescending(m => m.Id)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<StockMovementDto> RegisterEntryAsync(
            CreateStockEntryDto stockEntryDto,
            int userId)
        {
            ArgumentNullException.ThrowIfNull(stockEntryDto);

            var movement = new StockMovement(
                stockEntryDto.Quantity,
                StockMovementType.In,
                stockEntryDto.Reason,
                userId,
                stockEntryDto.ProductId,
                stockEntryDto.IngredientId,
                null
            );

            if (decimal.Round(movement.Quantity, 4) != movement.Quantity)
            {
                throw new ArgumentException(
                    "La cantidad admite como máximo cuatro decimales.");
            }

            return await _transactionManager.ExecuteAsync(async () =>
            {
                var user = await _userRepository.GetIdAsync(userId);

                if (user == null || !user.IsActive ||
                    user.UserRole != UserRole.Admin)
                {
                    throw new BusinessRuleException(
                        "La reposición requiere un usuario administrador activo.");
                }

                if (movement.IngredientId.HasValue)
                {
                    var ingredient = await _ingredientRepository.GetIdAsync(
                        movement.IngredientId.Value);

                    if (ingredient == null)
                    {
                        throw new ArgumentException(
                            "El insumo indicado no existe.");
                    }

                    const decimal maxStock = 99999999999999.9999m;

                    if (movement.Quantity > maxStock - ingredient.QuantityStock)
                    {
                        throw new ArgumentException(
                            "La reposición supera la capacidad de almacenamiento del stock.");
                    }

                    ingredient.RegisterStockEntry(movement.Quantity);
                    _ingredientRepository.Update(ingredient);
                }
                else
                {
                    var product = await _productRepository.GetIdAsync(
                        movement.ProductId!.Value);

                    if (product == null)
                    {
                        throw new ArgumentException(
                            "El producto indicado no existe.");
                    }

                    if (product.ProductType != ProductType.Purchased)
                    {
                        throw new BusinessRuleException(
                            "Solo los productos comprados pueden recibir stock propio.");
                    }

                    if (!product.ProductQuantityStock.HasValue)
                    {
                        throw new BusinessRuleException(
                            "El producto comprado no tiene configurado su stock.");
                    }

                    if (movement.Quantity != decimal.Truncate(movement.Quantity))
                    {
                        throw new ArgumentException(
                            "La cantidad de un producto comprado debe ser entera.");
                    }

                    var currentStock = product.ProductQuantityStock.Value;

                    if (currentStock < 0)
                    {
                        throw new BusinessRuleException(
                            "El producto tiene un stock inválido.");
                    }

                    if (movement.Quantity > int.MaxValue - currentStock)
                    {
                        throw new ArgumentException(
                            "La reposición supera la capacidad de almacenamiento del stock.");
                    }

                    product.AddStock((int)movement.Quantity);
                    _productRepository.Update(product);
                }

                _movementRepository.Add(movement);

                return MapToDto(movement);
            });
        }

        private static StockMovementDto MapToDto(StockMovement movement)
        {
            return new StockMovementDto
            {
                Id = movement.Id,
                DateTime = movement.DateTime,
                Quantity = movement.Quantity,
                MovementType = movement.MovementType,
                Reason = movement.Reason,
                UserId = movement.UserId,
                ProductId = movement.ProductId,
                IngredientId = movement.IngredientId,
                OrderDetailId = movement.OrderDetailId
            };
        }
    }
}