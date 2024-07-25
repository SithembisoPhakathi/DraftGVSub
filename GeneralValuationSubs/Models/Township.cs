using System;
using System.Collections.Generic;

namespace GeneralValuationSubs.Models;

public partial class Township
{
    public int Id { get; set; }

    public string? Tc { get; set; }

    public string? TownName { get; set; }

    public string? RegionName { get; set; }

    public string? DeptDir { get; set; }

    public string? SnrManager { get; set; }

    public string? AreaManager { get; set; }

    public string? CandidateDc { get; set; }

    public string? Sector { get; set; }
}
