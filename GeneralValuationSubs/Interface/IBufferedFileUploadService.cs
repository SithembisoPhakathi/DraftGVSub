namespace GeneralValuationSubs.Interface
{
    public interface IBufferedFileUploadService
    {
        Task<bool> UploadFile(string PremiseID, IFormFile file);  
    }
}
