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
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<Table> _tableRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly ITransactionManager _transactionManager;

        public OrderService(
            IGenericRepository<Order> orderRepository,
            IGenericRepository<Table> tableRepository,
            IGenericRepository<User> userRepository,
            ITransactionManager transactionManager)
        {
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _userRepository = userRepository;
            _transactionManager = transactionManager;
        }

        // Obtener todos los pedidos
        public async Task<List<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllWithIncludesAsync(
                "KitchenOrders.OrderDetails.Product"
            );

            return orders
                .OrderByDescending(o => o.DateTime)
                .ThenByDescending(o => o.Id)
                .Select(MapToDto)
                .ToList();
        }

        // Obtener un pedido por su identificador
        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "El identificador del pedido debe ser mayor que cero."
                );

            var order = await _orderRepository.GetByFilterWithIncludesAsync(
                o => o.Id == id,
                "KitchenOrders.OrderDetails.Product"
            );

            if (order == null)
                return null;

            return MapToDto(order);
        }

        // Obtener el pedido activo de una mesa
        public async Task<OrderDto?> GetActiveByTableIdAsync(int tableId)
        {
            if (tableId <= 0)
                throw new ArgumentException(
                    "El identificador de la mesa debe ser mayor que cero."
                );

            var order = await _orderRepository.GetByFilterWithIncludesAsync(
                o => o.TableId == tableId &&
                     o.Status == OrderStatus.Active,
                "KitchenOrders.OrderDetails.Product"
            );

            if (order == null)
                return null;

            return MapToDto(order);
        }

        // RN03, RN04 y RN05: abrir un pedido
        public async Task<OrderDto> CreateAsync(
            CreateOrderDto orderDto,
            int userId)
        {
            ArgumentNullException.ThrowIfNull(orderDto);

            if (orderDto.TableId <= 0)
                throw new ArgumentException(
                    "El identificador de la mesa debe ser mayor que cero."
                );

            return await _transactionManager.ExecuteAsync(async () =>
            {
                // Verificar que el usuario exista y sea un mozo activo
                var user = await _userRepository.GetIdAsync(userId);

                if (user == null ||
                    !user.IsActive ||
                    user.UserRole != UserRole.Waiter)
                {
                    throw new BusinessRuleException(
                        "El pedido debe ser abierto por un mozo activo."
                    );
                }

                // Verificar que la mesa exista
                var table = await _tableRepository.GetIdAsync(
                    orderDto.TableId
                );

                if (table == null)
                    throw new KeyNotFoundException(
                        $"No se ha encontrado la mesa con el id {orderDto.TableId}."
                    );

                // En Table, true significa libre
                if (!table.Status)
                    throw new BusinessRuleException(
                        "La mesa está ocupada. Solo se puede abrir un pedido en una mesa libre."
                    );

                // Comprobar que no exista otro pedido activo para esa mesa
                var activeOrders = await _orderRepository.FilteredCountAsync(
                    o => o.TableId == table.Id &&
                         o.Status == OrderStatus.Active
                );

                if (activeOrders > 0)
                    throw new BusinessRuleException(
                        "La mesa ya tiene un pedido activo."
                    );

                // Ocupar la mesa
                table.Occupy();
                _tableRepository.Update(table);

                // Crear el pedido
                var order = new Order
                {
                    DateTime = DateTime.Now,
                    TableId = table.Id,
                    UserId = userId
                };

                _orderRepository.Add(order);

                return MapToDto(order);
            });
        }

        // RN36: cerrar un pedido
        public async Task<OrderDto> CloseAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "El identificador del pedido debe ser mayor que cero."
                );

            return await _transactionManager.ExecuteAsync(async () =>
            {
                // Obtener el pedido con sus comandas, detalles y productos
                var order = await _orderRepository.GetByFilterWithIncludesAsync(
                    o => o.Id == id,
                    "KitchenOrders.OrderDetails.Product"
                );

                if (order == null)
                    throw new KeyNotFoundException(
                        $"No se ha encontrado el pedido con el id {id}."
                    );

                // Obtener la mesa asociada
                var table = await _tableRepository.GetIdAsync(order.TableId);

                if (table == null)
                    throw new KeyNotFoundException(
                        $"No se ha encontrado la mesa del pedido {id}."
                    );

                // La entidad valida que todas las comandas estén
                // entregadas o canceladas y recalcula el total.
                order.Close();

                // Liberar la mesa
                table.Release();

                _orderRepository.Update(order);
                _tableRepository.Update(table);

                return MapToDto(order);
            });
        }

        // Convertir la entidad Order en OrderDto
        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                DateTime = order.DateTime,
                Status = order.Status,
                Total = order.Total,
                TableId = order.TableId,
                UserId = order.UserId,

                KitchenOrders = order.KitchenOrders
                    .OrderBy(k => k.Id)
                    .Select(k => new KitchenOrderDto
                    {
                        Id = k.Id,
                        SentDateTime = k.SentDateTime,
                        Status = k.Status,
                        OrderId = k.OrderId,

                        OrderDetails = k.OrderDetails
                            .OrderBy(d => d.Id)
                            .Select(d => new OrderDetailDto
                            {
                                Id = d.Id,
                                Quantity = d.Quantity,
                                UnitPrice = d.UnitPrice,
                                SubTotal = d.SubTotal,
                                KitchenOrderId = d.KitchenOrderId,
                                ProductId = d.ProductId,
                                ProductName = d.Product?.ProductName
                                    ?? string.Empty
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }
    }
}