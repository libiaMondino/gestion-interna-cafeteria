using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Enums;

namespace Application.Models.Products
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public ProductType ProductType { get; set; }
        public bool ProductIsAvailable { get; set; }
        public int? ProductQuantityStock { get; set; }
        public int? ProductMinimumStock { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}