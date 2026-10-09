using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Core_API___AG.Models
{
    public class RestaurantTable
    {
            public int RestaurantTableId { get; set; }
            public string TableNumber { get; set; } = null!;
            public int Capacity { get; set; }
            public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        }

        public class Order
        {
            public int OrderId { get; set; }
            public string CustomerName { get; set; } = null!;
            public string MobileNo { get; set; } = null!;
            public DateTime OrderDate { get; set; }

            [ValidateNever]
            public string? ReceiptImageUrl { get; set; }
            public bool IsPaid { get; set; }

            [Required, Column(TypeName = "money")]
            public decimal TotalAmount { get; set; }

            public int RestaurantTableId { get; set; }

            [ValidateNever]
            public virtual RestaurantTable RestaurantTable { get; set; } = null!;

            public virtual IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        }

        public class OrderItem
        {
            public int OrderItemId { get; set; }
            public int Quantity { get; set; }
            public int OrderId { get; set; }

            [ValidateNever]
            public virtual Order? Order { get; set; }

            public int MenuItemId { get; set; }

            [ValidateNever]
            public virtual MenuItem? MenuItem { get; set; }
        }

        public class MenuItem
        {
            public int MenuItemId { get; set; }
            public string ItemName { get; set; } = null!;
            public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        }

        public class UploadResponse
        {
            public string FileName { get; set; } = default!;
        }
    }

