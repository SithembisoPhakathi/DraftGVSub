using System;
using System.Collections.Generic;

namespace GeneralValuationSubs.Models;

public partial class Residential
{
    public string? PremiseId { get; set; }

    public string? LegalDescription { get; set; }

    public string? Sector { get; set; }

    public string? Tla1 { get; set; }

    public string? Tla2 { get; set; }

    public string? Tla3 { get; set; }

    public string? Garage { get; set; }

    public string? Carport { get; set; }

    public string? GrannyFlat { get; set; }

    public string? ServantsQuarters { get; set; }

    public string? NoOfStoreys { get; set; }

    public string? TotalLivingArea { get; set; }

    public string? NameOfBuyer { get; set; }

    public string? NameOfSeller { get; set; }

    public string? TitleDeedNumber { get; set; }

    public string? Zoning { get; set; }

    public string? Extent { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public decimal? PurchasePrice { get; set; }

    public string? RM { get; set; }

    public string? ValidSale { get; set; }

    public string? TownShip { get; set; }
}
