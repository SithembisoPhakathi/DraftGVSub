using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace GeneralValuationSubs.Models;

public partial class Draft
{
    public int DraftId { get; set; }
    public int QueryId { get; set; }
    public string? PropertyDescription { get; set; }
    public string? QUERY_No { get; set; }

    public string? TownshipDescription { get; set; }

    public string? MarketValue { get; set; }
    public string? MarketValue1 { get; set; }
    public string? MarketValue2 { get; set; }
    public string? MarketValue3 { get; set; }
    public string? OldMarketValue1 { get; set; }
    public string? OldMarketValue2 { get; set; }
    public string? OldMarketValue3 { get; set; }
    public string? MarketCategory { get; set; }
    public string? CATDescription { get; set; }
    public string? CATDescription1 { get; set; }
    public string? CATDescription2 { get; set; }
    public string? CATDescription3 { get; set; }
    public string? OldCATDescription1 { get; set; }
    public string? OldCATDescription2 { get; set; }
    public string? OldCATDescription3 { get; set; }
    public string? ValuationType { get; set; } 
    public string? ValuationTypeDescription { get; set; }
    public string? Unit_Type { get; set; }
    public int Number_Of_Approved { get; set; }
    public string? Sector { get; set; }

    public DateTime? WEF_DATE { get; set; }

    public string? RevisedMarketValue { get; set; }

    public string? RevisedCategory { get; set; }

    public string? Comment { get; set; }
    public string? Extent { get; set; }
    public string? Extent1 { get; set; }
    public string? Extent2 { get; set; }
    public string? Extent3 { get; set; }
    public string? CommentMarketValue { get; set; }

    public string? CommentCategory { get; set; }

    public bool? FlagForDelete { get; set; }

    public string? CommentFlagging { get; set; }

    public string? AssignedValuer { get; set; }

    public DateTime? Date { get; set; }

    public string? BulkUpload { get; set; }

    public string? PremiseId { get; set; }

    public string? Status { get; set; } 
    public string? ValuationStatus { get; set; }

    public string? Dept_Dir { get; set; }
    public string? Snr_Manager { get; set; }
     
    public string? Area_Manager { get; set; }
    public string? Candidate_DC { get; set; }

    public string? EmailAddress { get; set; }
     
    public string? ApproverComment { get; set; } 

    public string? FileName { get; set; }
    public string? FileNameAttach { get; set; }

    public string? FilePath { get; set; }

    public DateTime Start_Date { get; set; }
    public DateTime End_Date { get; set; }

    public int? DateDiff { get; set; } 

    public string? AllocatedName { get; set; } 

    public string? SchemeName { get; set; }

    public List<Draft> attachments { get; set; }
}
