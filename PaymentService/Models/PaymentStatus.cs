﻿using System;
using System.Collections.Generic;

namespace PaymentService.Models;

public partial class PaymentStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
