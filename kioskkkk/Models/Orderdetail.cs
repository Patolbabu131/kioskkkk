using System;
using System.Collections.Generic;

namespace kioskkkk.Models;

public partial class Orderdetail
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public int OrderId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Subcategory Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
