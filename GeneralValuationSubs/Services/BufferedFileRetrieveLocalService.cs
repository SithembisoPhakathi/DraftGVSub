using GeneralValuationSubs.Interface;

namespace GeneralValuationSubs.Services
{
    public class BufferedFileRetrieveLocalService : IBufferedFileRetrieveService
    {
        public async Task<bool> RetrieveFile(IFormFile file, string PremiseID)
        {       
            string path = "";

            try
            {
                if (file != null && file.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "C:\\Users\\10107037\\Documents\\" + PremiseID));  //"C:\\Users\\Montegomery"
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    using (var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.Open))
                    {
                        if (file.Length < 31457280)
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        else
                        {
                            return BadRequest("File too large");
                        }
                    }
                    return true;
                }
                else
                {
                    return BadRequest("No file uploaded");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
        }

        private bool BadRequest(string v)
        {
            throw new NotImplementedException();
        }
    }
}
