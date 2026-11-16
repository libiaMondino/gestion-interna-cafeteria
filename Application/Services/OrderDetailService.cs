
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;

        public OrderDetailService(
            IGenericRepository<OrderDetail> orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        // Obtener todos los detalles de pedidos
        public async Task<List<OrderDetailDto>> GetAllAsync()
        {
            var details = await _orderDetailRepository
                .GetAllWithIncludesAsync("Product");

            return details
                .OrderBy(d => d.Id)
                .Select(MapToDto)
                .ToList();
        }

        // Obtener un detalle por su ID
        public async Task<OrderDetailDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "El ID del detalle debe ser mayor que cero.");

            var detail = await _orderDetailRepository
                .GetByFilterWithIncludesAsync(
                    d => d.Id == id,
                    "Product");

            if (detail == null)
                return null;

            return MapToDto(detail);
        }

        // Obtener los detalles de una comanda
        public async Task<List<OrderDetailDto>> GetByKitchenOrderIdAsync(
            int kitchenOrderId)
        {
            if (kitchenOrderId <= 0)
                throw new ArgumentException(
                    "El ID de la comanda debe ser mayor que cero.");

            var details = await _orderDetailRepository
                .GetAllWithIncludesAsync("Product");

            return details
                .Where(d => d.KitchenOrderId == kitchenOrderId)
                .OrderBy(d => d.Id)
                .Select(MapToDto)
                .ToList();
        }

        // Convertir la entidad a DTO
        private static OrderDetailDto MapToDto(OrderDetail detail)
        {
            return new OrderDetailDto
            {
                Id = detail.Id,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                SubTotal = detail.SubTotal,
                KitchenOrderId = detail.KitchenOrderId,
                ProductId = detail.ProductId,
                ProductName = detail.Product?.ProductName ?? string.Empty
            };
        }
    }
}
