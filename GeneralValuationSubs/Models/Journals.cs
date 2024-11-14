using System.ComponentModel.DataAnnotations;

namespace GeneralValuationSubs.Models
{
    public class Journals_Audit
    {
        [Key]
        public int? Transaction_ID { get; set; }
        public string? Premise_ID { get; set; }
        public string? Account_Number { get; set; } 
        public string? Installation { get; set; }
        public string? Market_Value { get; set; }
        public string? Category { get; set; }
        public string? WEF { get; set; }
        public string? Valuation_Date { get; set; }
        public string? Net_Accrual { get; set; }
        public string? File_Name { get; set; }
        public string? Journal_Id { get; set; }
        public string? Status { get; set; }
        public string? Allocated_Name { get; set; }
        public string? ApproverComment { get; set; }
        public string? Comment { get; set; }
        public string? ValuationDate { get; set; }
        public DateTime? BillingFrom { get; set; }
        public DateTime? BillingTo { get; set; }
        public string? BillingDays { get; set; }
        public string? Threshold { get; set; }
        public string? RatableValue { get; set; }
        public string? RatesTariff { get; set; }
        public string? RebateType { get; set; }
        public string? RebateAmount { get; set; }
        public string? calculatedRate { get; set; } 
        public string? UserName { get; set; }
        public DateTime? Activity_Date { get; set; }
        public string? ToBeCharged { get; set; }
        public string? ActualBilling { get; set; }
        public string? NetAdjustment { get; set; }
        public int? DateDiff { get; set; }
        public string? FinancialYear { get; set; }         
        public int? Journal_ID { get; set; } 
        public string? FileName { get; set; }
        public DateTime? End_Date { get; set; }
        public string? DocDate { get; set; }
        public string? Type { get; set; }
        public string? DocNo { get; set; }
        public string? Div { get; set; }
        public string? Description { get; set; }
        public string? Amount { get; set; }
    }
}
