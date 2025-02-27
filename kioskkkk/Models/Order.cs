using System;
using System.Collections.Generic;

namespace kioskkkk.Models;

public partial class Order
{
    public int Id { get; set; }

    public int Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
