using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObjectionSystem.Sup_Properties
{
    public partial class Sup4And5
    {
        [Key]
        public int Id { get; set; }
        public string? PropertyId { get; set; }
        public string? SgId { get; set; }
        public string? TownNameDesc { get; set; }
        public string? UnitKey { get; set; }
        public int? UnitNo { get; set; }
        public string? ValuationDate { get; set; }
        public string? ValuationKey { get; set; }
        public string? ValuationRollKey { get; set; }
        public string? RateableArea { get; set; }
        public string? ValuationType { get; set; }
        public string? WefDate { get; set; }
        public string? CatDesc { get; set; }
        public string? Reason { get; set; }
        public string? MarketValue { get; set; }
        public string? AdditionalNotes { get; set; }
        public string? UnitRightsKey { get; set; }
        public string? RightsPremise { get; set; }
        public string? RightDescription { get; set; }
        public string? RightsPremiseSize { get; set; }
        public string? RightsStatus { get; set; }
        public string? ValuationKey2 { get; set; }
        public string? ValuationSplitNo { get; set; }
        public string? ValuationSplitInd { get; set; }
        public string? PremiseId { get; set; }
        public string? PrintRec { get; set; }
        public string? TsOnlyName { get; set; }
        public string? TsExt { get; set; }
        public int? Erf { get; set; }
        public int? Ptn { get; set; }
        public string? Re { get; set; }
        public string? SchemeName { get; set; }
        public string? SchemeNumber { get; set; }
        public string? SchemeYear { get; set; }
        public string? UnitType { get; set; }
        public string? PropertyDesc { get; set; }
        public string? LisStreetAddress { get; set; }
        public string? OwnerName { get; set; }
        public string? SchemeNameDesc { get; set; }
        public string? Tc { get; set; }
        public string? AtSchemeKey { get; set; }
        public string? Sector { get; set; }
        public string? Roll { get; set; }
        public string? RightsUnitType { get; set; }
    }
}
