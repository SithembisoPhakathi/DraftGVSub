using System.Globalization;

namespace GeneralValuationSubs.Models
{
    public class FileParser
    {
        public async Task<List<DocumentRecord>> ParseFileAsync(IFormFile file, string startKeyword)
        {
            var records = new List<DocumentRecord>();
            bool startReading = false;

            // Define field positions and lengths
            var fieldDefinitions = new[]
            {
                new { Start = 0, Length = 13 },   // DocDate
                new { Start = 14, Length = 2 },   // Type
                new { Start = 16, Length = 10 },  // DocNo
                new { Start = 26, Length = 1 },   // Div
                new { Start = 28, Length = 30 },  // Description
                new { Start = 58, Length = 10 }
            };

            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                string line;

                while ((line = await stream.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Start reading from the line after encountering the keyword
                    if (!startReading)
                    {
                        if (line.Contains(startKeyword))
                        {
                            startReading = true;
                        }
                        continue;
                    }

                    // Only process lines that have enough length for the longest field
                    if (line.Length < fieldDefinitions.Max(f => f.Start + f.Length)) continue;

                    try
                    {
                        var record = new DocumentRecord
                        {
                            DocDate = line.Substring(fieldDefinitions[0].Start, fieldDefinitions[0].Length).Trim(),
                            Type = line.Substring(fieldDefinitions[1].Start, fieldDefinitions[1].Length).Trim(),
                            DocNo = NormalizeDocNo(line.Substring(fieldDefinitions[2].Start, fieldDefinitions[2].Length).Trim()),
                            Div = line.Substring(fieldDefinitions[3].Start, fieldDefinitions[3].Length).Trim(),
                            Description = line.Substring(fieldDefinitions[4].Start, fieldDefinitions[4].Length).Trim(),
                            Amount = ParseAmount(line.Substring(fieldDefinitions[5].Start, fieldDefinitions[5].Length).Trim())
                        };

                        records.Add(record);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        // Handle or log parsing errors for lines that are too short
                        Console.WriteLine($"Failed to parse line: {line} - Error: {ex.Message}");
                    }
                }
            }

            return records;
        }

        private string NormalizeDocNo(string docNo)
        {
            if (double.TryParse(docNo, NumberStyles.Any, CultureInfo.InvariantCulture, out var numericDocNo))
            {
                return numericDocNo.ToString("F0", CultureInfo.InvariantCulture);  // Format without scientific notation
            }
            return docNo;  // Return as-is if parsing fails
        }

        // Helper method to parse Amount, with fallback to 0 if invalid
        private decimal ParseAmount(string amountStr)
        {
            return decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount) ? amount : 0;
        }
    }    
}