using System;
using System.Collections.Generic;

namespace GeneralValuationSubs.Models;

public partial class DraftGvaudit
{
    public int Id { get; set; }

    public int? DraftGvid { get; set; }

    public string? PremiseId { get; set; }

    public string? Town { get; set; }

    public string? ModelUsed { get; set; }

    public string? Ddarea { get; set; }

    public decimal? CalculatedValue { get; set; }

    public decimal? ValuersValue { get; set; }

    public decimal? DraftValue { get; set; }

    public decimal? Gv18marketValue { get; set; }

    public string? Gv18category { get; set; }

    public string? SalesStatus { get; set; }

    public decimal? TimeAdjPurchasePrice { get; set; }

    public decimal? PurchasePrice { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public decimal? DiffsGv23vsSales { get; set; }

    public double? DiffsGv23vsSalesPerc { get; set; }

    public decimal? DiffsGv23vsGv18 { get; set; }

    public double? DiffsGv23vsGv18perc { get; set; }

    public decimal? Nbhdbasis { get; set; }

    public decimal? AdjustedBasis { get; set; }

    public decimal? AdjustedWgba { get; set; }

    public decimal? AbasisTimesAdjWgba { get; set; }

    public int? LislegalArea { get; set; }

    public string? ZoneCode { get; set; }

    public string? VauseCode { get; set; }

    public string? UseDescription { get; set; }

    public string? ProposedUseDescription { get; set; }

    public string? PnCproperty { get; set; }

    public string? ParentPremiseId { get; set; }

    public int? Tla1 { get; set; }

    public int? Tla2 { get; set; }

    public int? Tla3 { get; set; }

    public int? Garage { get; set; }

    public int? Cp { get; set; }

    public int? Gf { get; set; }

    public int? Sq { get; set; }

    public decimal? PrevDraftValue { get; set; }

    public string? Nbhoodcode { get; set; }

    public string? SchemeName { get; set; }

    public string? Note { get; set; }

    public string? Note1 { get; set; }

    public string? Note2 { get; set; }

    public string? ValuersComment { get; set; }

    public string? SeniorManager { get; set; }

    public string? DeputyDirector { get; set; }

    public string? AreaManager { get; set; }

    public string? Valuer { get; set; }

    public bool? ValueOverride { get; set; }

    public int? ValueRevised { get; set; }

    public string? CommentValue { get; set; }

    public bool? CategoryOverride { get; set; }

    public string? CategoryRevised { get; set; }

    public string? CommentCategory { get; set; }

    public bool? FlagDelete { get; set; }

    public string? CommentDelete { get; set; }

    public DateTime? DateTime { get; set; }

    public string? UserName { get; set; }

    public string? Status { get; set; }

    public string? Sector { get; set; }
}
