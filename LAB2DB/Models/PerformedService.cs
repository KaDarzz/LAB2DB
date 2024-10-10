using System;
using System.Collections.Generic;

namespace LAB2DB.Models;

public partial class PerformedService
{
    public int Id { get; set; }

    public int? ClientId { get; set; }

    public int? ServiceId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly? ServiceDate { get; set; }

    public decimal? Cost { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Service? Service { get; set; }
}
