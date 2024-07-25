using System;
using System.Collections.Generic;

namespace GeneralValuationSubs.Models;

public partial class Valuer
{
    public int Id { get; set; }

    public string? Sap { get; set; }

    public string? FirstName { get; set; }

    public string? SecondName { get; set; }

    public string? Surname { get; set; }

    public string? Designation { get; set; }

    public string? EmailAddress { get; set; }

    public string? Sector { get; set; }
}
