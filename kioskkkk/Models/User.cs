using System;
using System.Collections.Generic;

namespace kioskkkk.Models;

public partial class User
{
    public int Id { get; set; }

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;
}
