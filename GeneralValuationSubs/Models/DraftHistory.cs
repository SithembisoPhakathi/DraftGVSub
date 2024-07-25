using System;
using System.Collections.Generic;

namespace GeneralValuationSubs.Models;

public partial class DraftHistory
{
    public int Id { get; set; }

    public string? UserName { get; set; }

    public string? UserId { get; set; }

    public string? PropertyDescription { get; set; }

    public string? RevisedMarketValue { get; set; }
    public string? MarketValue { get; set; }

    public string? RevisedCategory { get; set; } 
    public string? CATDescription { get; set; }

    public string? CommentMarketValue { get; set; }

    public string? CommentCategory { get; set; }

    public string? Status { get; set; }

    public DateTime? ActivityDate { get; set; }

    public string? UserActivity { get; set; }

    public string? ApproverComment { get; set; } 
    public string? Comment { get; set; }

    public string? PremiseId { get; set; }
}
