
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class KitchenOrderService : IKitchenOrderService
    {
        private readonly IGenericRepository<KitchenOrder> _kitchenOrderRepository;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Ingredient> _ingredientRepository;
        private readonly IGenericRepository<RecipeDetail> _recipeDetailRepository;
        private readonly IGenericRepository<StockMovement> _stockMovementRepository;
        private readonly ITransactionManager _transactionManager;

        public KitchenOrderService(
            IGenericRepository<KitchenOrder> kitchenOrderRepository,
            IGenericRepository<Order> orderRepository,
            IGenericRepository<OrderDetail> orderDetailRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<Ingredient> ingredientRepository,
            IGenericRepository<RecipeDetail> recipeDetailRepository,
            IGenericRepository<StockMovement> stockMovementRepository,
            ITransactionManager transactionManager)
        {
            _kitchenOrderRepository = kitchenOrderRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
            _ingredientRepository = ingredientRepository;
            _recipeDetailRepository = recipeDetailRepository;
            _stockMovementRepository = stockMovementRepository;
            _transactionManager = transactionManager;
        }

        // 1. Obtener todas las comandas
        public async Task<List<KitchenOrderDto>> GetAllAsync()
        {
            var kitchenOrders =
                await _kitchenOrderRepository.GetAllWithIncludesAsync(
                    "OrderDetails.Product");

            return kitchenOrders
                .OrderByDescending(k => k.Id)
                .Select(MapToDto)
                .ToList();
        }

        // 2. Obtener una comanda por ID
        public async Task<KitchenOrderDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "El identificador debe ser mayor que cero.");

            var kitchenOrder =
                await _kitchenOrderRepository.GetByFilterWithIncludesAsync(
                    k => k.Id == id,
                    "OrderDetails.Product");

            return kitchenOrder == null
                ? null
                : MapToDto(kitchenOrder);
        }

        // 3. Obtener las comandas de un pedido
        public async Task<List<KitchenOrderDto>> GetByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException(
                    "El identificador del pedido debe ser mayor que cero.");

            var kitchenOrders =
                await _kitchenOrderRepository.GetAllWithIncludesAsync(
                    "OrderDetails.Product");

            return kitchenOrders
                .Where(k => k.OrderId == orderId)
                .OrderBy(k => k.Id)
                .Select(MapToDto)
                .ToList();
        }

        // 4. Crear una comanda
        public async Task<KitchenOrderDto> CreateAsync(
            CreateKitchenOrderDto kitchenOrderDto)
        {
            ArgumentNullException.ThrowIfNull(kitchenOrderDto);

            if (kitchenOrderDto.OrderId <= 0)
                throw new ArgumentException(
                    "El identificador del pedido debe ser mayor que cero.");

            return await _transactionManager.ExecuteAsync(async () =>
            {
                var order =
                    await _orderRepository.GetByFilterWithIncludesAsync(
                        o => o.Id == kitchenOrderDto.OrderId,
                        "KitchenOrders");

                if (order == null)
                    throw new KeyNotFoundException(
                        "No se encontró el pedido.");

                if (order.Status != OrderStatus.Active)
                    throw new BusinessRuleException(
                        "No se pueden agregar comandas a un pedido cerrado.");

                var kitchenOrder = new KitchenOrder
                {
                    OrderId = order.Id,
                    Order = order
                };

                order.AddKitchenOrder(kitchenOrder);

                _kitchenOrderRepository.Add(kitchenOrder);

                foreach (var detailDto in kitchenOrderDto.OrderDetails)
                {
                    var product =
                        await _productRepository.GetIdAsync(
                            detailDto.ProductId);

                    if (product == null)
                        throw new KeyNotFoundException(
                            $"No se encontró el producto {detailDto.ProductId}.");

                    if (!product.ProductIsAvailable)
                        throw new BusinessRuleException(
                            $"El producto {product.ProductName} no está disponible.");

                    var detail = new OrderDetail(
                        detailDto.Quantity,
                        product.ProductPrice)
                    {
                        ProductId = product.ProductId,
                        KitchenOrderId = kitchenOrder.Id
                    };

                    kitchenOrder.AddOrderDetail(detail);

                    _orderDetailRepository.Add(detail);
                }

                order.CalculateTotal();
                _orderRepository.Update(order);

                return MapToDto(kitchenOrder);
            });
        }

        // 5. Agregar un producto a una comanda
        public async Task<KitchenOrderDto> AddOrderDetailAsync(
            int kitchenOrderId,
            CreateOrderDetailDto detailDto)
        {
            ArgumentNullException.ThrowIfNull(detailDto);

            if (kitchenOrderId <= 0)
                throw new ArgumentException(
                    "El identificador de la comanda debe ser mayor que cero.");

            return await _transactionManager.ExecuteAsync(async () =>
            {
                var kitchenOrder =
                    await _kitchenOrderRepository.GetByFilterWithIncludesAsync(
                        k => k.Id == kitchenOrderId,
                        "Order",
                        "OrderDetails.Product");

                if (kitchenOrder == null)
                    throw new KeyNotFoundException(
                        "No se encontró la comanda.");

                if (kitchenOrder.Order.Status != OrderStatus.Active)
                    throw new BusinessRuleException(
                        "No se pueden modificar comandas de pedidos cerrados.");

                var product =
                    await _productRepository.GetIdAsync(detailDto.ProductId);

                if (product == null)
                    throw new KeyNotFoundException(
                        "No se encontró el producto.");

                if (!product.ProductIsAvailable)
                    throw new BusinessRuleException(
                        "El producto no está disponible.");

                var detail = new OrderDetail(
                    detailDto.Quantity,
                    product.ProductPrice)
                {
                    ProductId = product.ProductId,
                    KitchenOrderId = kitchenOrder.Id
                };

                kitchenOrder.AddOrderDetail(detail);

                _orderDetailRepository.Add(detail);

                kitchenOrder.Order.CalculateTotal();
                _orderRepository.Update(kitchenOrder.Order);

                return MapToDto(kitchenOrder);
            });
        }

        // 6. Enviar una comanda a cocina
        public async Task<KitchenOrderDto> SendAsync(int id)
        {
            return await _transactionManager.ExecuteAsync(async () =>
            {
                var kitchenOrder = await GetKitchenOrderOrThrowAsync(id);

                kitchenOrder.Send();

                _kitchenOrderRepository.Update(kitchenOrder);

                return MapToDto(kitchenOrder);
            });
        }

        // 7. Iniciar la preparación y descontar ingredientes
        public async Task<KitchenOrderDto> MarkAsPreparingAsync(
            int id,
            int userId)
        {
            if (userId <= 0)
                throw new ArgumentException(
                    "El identificador del usuario no es válido.");

            return await _transactionManager.ExecuteAsync(async () =>
            {
                var kitchenOrder =
                    await _kitchenOrderRepository.GetByFilterWithIncludesAsync(
                        k => k.Id == id,
                        "OrderDetails");

                if (kitchenOrder == null)
                    throw new KeyNotFoundException(
                        "No se encontró la comanda.");

                if (kitchenOrder.Status != KitchenOrderStatus.Sent)
                    throw new BusinessRuleException(
                        "La comanda debe estar enviada para comenzar su preparación.");

                var consumptions =
                    new List<(Ingredient Ingredient, OrderDetail Detail, decimal Quantity)>();

                foreach (var detail in kitchenOrder.OrderDetails)
                {
                    var recipes =
                        await _recipeDetailRepository.GetListByFilterAsync(
                            r => r.ProductId == detail.ProductId);

                    if (!recipes.Any())
                        throw new BusinessRuleException(
                            $"El producto {detail.ProductId} no tiene una receta configurada.");

                    foreach (var recipe in recipes)
                    {
                        var ingredient =
                            await _ingredientRepository.GetIdAsync(
                                recipe.IngredientId);

                        if (ingredient == null)
                            throw new KeyNotFoundException(
                                $"No se encontró el ingrediente {recipe.IngredientId}.");

                        decimal quantity =
                            recipe.CalculateConsumption(detail.Quantity);

                        consumptions.Add(
                            (ingredient, detail, quantity));
                    }
                }

                // Primero verificamos el stock total necesario
                // para cada ingrediente.
                var groupedConsumptions = consumptions
                    .GroupBy(c => c.Ingredient.Id);

                foreach (var group in groupedConsumptions)
                {
                    decimal totalRequired =
                        group.Sum(c => c.Quantity);

                    var ingredient = group.First().Ingredient;

                    if (!ingredient.HasSufficientStock(totalRequired))
                        throw new BusinessRuleException(
                            $"No hay stock suficiente de {ingredient.Name}.");
                }

                // Si todo el stock está disponible, registramos
                // los descuentos y sus movimientos.
                foreach (var consumption in consumptions)
                {
                    consumption.Ingredient.RegisterStockExit(
                        consumption.Quantity);

                    _ingredientRepository.Update(
                        consumption.Ingredient);

                    var movement = new StockMovement(
                        consumption.Quantity,
                        StockMovementType.Out,
                        $"Consumo por preparación de comanda {id}",
                        userId,
                        ingredientId: consumption.Ingredient.Id,
                        orderDetailId: consumption.Detail.Id);

                    _stockMovementRepository.Add(movement);
                }

                kitchenOrder.MarkAsPreparing();

                _kitchenOrderRepository.Update(kitchenOrder);

                return MapToDto(kitchenOrder);
            });
        }

        // 8. Marcar una comanda como lista
        public async Task<KitchenOrderDto> MarkAsReadyAsync(int id)
        {
            return await _transactionManager.ExecuteAsync(async () =>
            {
                var kitchenOrder = await GetKitchenOrderOrThrowAsync(id);

                kitchenOrder.MarkAsReady();

                _kitchenOrderRepository.Update(kitchenOrder);

                return MapToDto(kitchenOrder);
            });
        }

        // 9. Registrar la entrega
        public async Task<KitchenOrderDto> RegisterDeliveryAsync(int id)
        {
            return await _transactionManager.ExecuteAsync(async () =>
            {
                var kitchenOrder = await GetKitchenOrderOrThrowAsync(id);

                kitchenOrder.RegisterDelivery();

                _kitchenOrderRepository.Update(kitchenOrder);

                return MapToDto(kitchenOrder);
            });
        }

        // 10. Cancelar una comanda
        public async Task<KitchenOrderDto> CancelAsync(int id)
        {
            return await _transactionManager.ExecuteAsync(async () =>
            {
                var kitchenOrder =
                    await _kitchenOrderRepository.GetByFilterWithIncludesAsync(
                        k => k.Id == id,
                        "Order",
                        "Order.KitchenOrders.OrderDetails",
                        "OrderDetails.Product");

                if (kitchenOrder == null)
                    throw new KeyNotFoundException(
                        "No se encontró la comanda.");

                kitchenOrder.Cancel();

                kitchenOrder.Order.CalculateTotal();

                _kitchenOrderRepository.Update(kitchenOrder);
                _orderRepository.Update(kitchenOrder.Order);

                return MapToDto(kitchenOrder);
            });
        }

        // Método auxiliar para buscar una comanda
        private async Task<KitchenOrder> GetKitchenOrderOrThrowAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "El identificador debe ser mayor que cero.");

            var kitchenOrder =
                await _kitchenOrderRepository.GetByFilterWithIncludesAsync(
                    k => k.Id == id,
                    "OrderDetails.Product");

            if (kitchenOrder == null)
                throw new KeyNotFoundException(
                    $"No se encontró la comanda con el id {id}.");

            return kitchenOrder;
        }

        // Conversión de entidad a DTO
        private static KitchenOrderDto MapToDto(KitchenOrder kitchenOrder)
        {
            return new KitchenOrderDto
            {
                Id = kitchenOrder.Id,
                SentDateTime = kitchenOrder.SentDateTime,
                Status = kitchenOrder.Status,
                OrderId = kitchenOrder.OrderId,

                OrderDetails = kitchenOrder.OrderDetails
                    .OrderBy(d => d.Id)
                    .Select(d => new OrderDetailDto
                    {
                        Id = d.Id,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        SubTotal = d.SubTotal,
                        KitchenOrderId = d.KitchenOrderId,
                        ProductId = d.ProductId,
                        ProductName = d.Product?.ProductName ?? string.Empty
                    })
                    .ToList()
            };
        }
    }
}
