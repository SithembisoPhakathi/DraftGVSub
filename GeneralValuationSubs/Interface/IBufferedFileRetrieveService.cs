namespace GeneralValuationSubs.Interface
{
    public interface IBufferedFileRetrieveService
    {
        Task<bool> RetrieveFile(IFormFile file, string PremiseID); 
    }
}
