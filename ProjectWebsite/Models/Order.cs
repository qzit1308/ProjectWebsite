using System;
using System.Collections.Generic;

namespace ProjectWebsite.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string RecipientName { get; set; } = null!;

    public string ShippingAddress { get; set; } = null!;

    public string RecipientPhone { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; } = null!;
}
