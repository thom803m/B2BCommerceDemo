using B2BCommerceDemo.Core.Models;

namespace B2BCommerceDemo.Core.Policies
{
    public static class OrderStatusPolicy
    {
        public static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions =
            new()
            {
                { OrderStatus.Pending, new[] { OrderStatus.Confirmed, OrderStatus.Cancelled } },
                { OrderStatus.Confirmed, new[] { OrderStatus.Processing, OrderStatus.Cancelled } },
                { OrderStatus.Processing, new[] { OrderStatus.Shipped } },
                { OrderStatus.Shipped, new[] { OrderStatus.Completed } },
                { OrderStatus.Completed, Array.Empty<OrderStatus>() },
                { OrderStatus.Cancelled, Array.Empty<OrderStatus>() }
            };

        public static void Validate(OrderStatus from, OrderStatus to)
        {
            if (!AllowedTransitions.TryGetValue(from, out var allowed))
            {
                throw new InvalidOperationException($"Invalid current status: {from}");
            }

            if (!allowed.Contains(to))
            {
                throw new InvalidOperationException($"Cannot change status from {from} to {to}");
            }
        }
    }
}
