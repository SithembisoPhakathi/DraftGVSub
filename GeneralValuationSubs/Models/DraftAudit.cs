using System;
using System.Collections.Generic;

namespace GeneralValuationSubs.Models;

public partial class DraftAudit
{
    public long DraftAuditId { get; set; }

    public long DraftId { get; set; }

    public string? PropertyDescription { get; set; }

    public string? TownshipDescription { get; set; }

    public decimal? MarketValue { get; set; }

    public string? MarketCategory { get; set; }

    public decimal? RevisedMarketValue { get; set; }

    public string? RevisedCategory { get; set; }

    public string? CommentMarketValue { get; set; }

    public string? CommentCategory { get; set; }

    public bool? FlagForDelete { get; set; }

    public string? CommentFlagging { get; set; }

    public string? AssignedValuer { get; set; }

    public DateTime? Date { get; set; }

    public bool? BulkUpload { get; set; }

    public string? PremiseId { get; set; }

    public string? Status { get; set; }

    public string? EmailAddress { get; set; }
}
