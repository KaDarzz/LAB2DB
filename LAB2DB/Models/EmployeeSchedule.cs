using System;
using System.Collections.Generic;

namespace LAB2DB.Models;

public partial class EmployeeSchedule
{
    public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly? WorkDate { get; set; }

    public bool? IsWorking { get; set; }

    public virtual Employee? Employee { get; set; }
}
