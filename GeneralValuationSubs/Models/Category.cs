using System;
using System.Collections.Generic;

namespace GeneralValuationSubs.Models;

public partial class Category
{
    public int CatId { get; set; }

    public string? LisCatCode { get; set; }

    public string? CatDescName { get; set; }

    public DateTime? CatStartDate { get; set; }

    public DateTime? CatEndDate { get; set; }

    public string? Active { get; set; }
}
