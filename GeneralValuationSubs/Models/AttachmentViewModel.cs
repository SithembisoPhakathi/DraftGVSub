using System.Net.Mail;

namespace GeneralValuationSubs.Models
{
    public class AttachmentViewModel
    {
        //public string FileName { set; get; }
        public string RevisedMarketValue { get; set; }
        public string MarketValueComment { get; set; }
        public string RevisedCategory { get; set; }
        public string RevisedCategoryComment { get; set; }
        public IFormFile attachment { set; get; }
        public List<Draft> attachments { get; set; }
    }
}
