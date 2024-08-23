using GeneralValuationSubs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.IO.Compression;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GeneralValuationSubs.Controllers
{
    public class JournalsController : Controller
    {
        List<Journals> journals = new List<Journals>();
        List<Category> categories = new List<Category>();
        List<AdminValuer> adminValuers = new();
        List<JournalHistory> JournalHistories = new List<JournalHistory>();
        SqlCommand com = new SqlCommand();
        SqlConnection con = new SqlConnection();
        SqlDataReader dr;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;


        public JournalsController(Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _config = config;
            con.ConnectionString = _config.GetConnectionString("JournalConnection");
        }

        public IActionResult AllocateTask(string? userName)
        {
            var userSector = TempData["currentUserSector"]; //Assigning temp data with the user sector to get the sectors related to the user
            TempData.Keep("currentUserSector");

            //userName = TempData["currentUserFirstname"].ToString() +' ' + TempData["currentUserSurname"].ToString();

            if (journals.Count > 0)
            {
                journals.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT Top(100) DATEDIFF(DAY, GETDATE(), ISNULL(End_Date, GETDATE())) AS Date_Diff, * FROM [Journals].[dbo].[Details] WHERE Status IN (SELECT [Status_Description] FROM [Journals].[dbo].[Status] WHERE Status_ID IN (1, 2, 3, 5))"; 

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    journals.Add(new Journals
                    {
                        Premise_ID = dr["Premise ID"].ToString(),
                        Account_Number = dr["Account Number"].ToString(),
                        Installation = dr["Installation"].ToString(),
                        Market_Value = dr["Market Value"].ToString(),
                        Category = dr["Category"].ToString(),
                        Valuation_Date = dr["Valuation Date"].ToString(),
                        WEF = dr["WEF"].ToString(),
                        Net_Accrual = dr["Net Accrual"].ToString(),
                        File_Name = dr["File Name"].ToString(),
                        Status = dr["Status"].ToString(),
                        Allocated_Name = dr["Allocated Name"].ToString(),
                        Journal_Id = dr["Journal_Id"].ToString(),
                        ValuationDate = dr["Valuation Date"].ToString(),
                        DateDiff = (int)dr["Date_Diff"]
                    });
                }
                con.Close();

                ViewBag.UserDataList = journals.ToList();
                
            }

            catch (Exception ex)
            {
                throw ex;
            }

            if (adminValuers.Count > 0)
            {
                adminValuers.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "EXEC Journal_List_Procedure";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    adminValuers.Add(new AdminValuer
                    {
                        FirstName = dr["First_Name"].ToString(),
                        Surname = dr["Surname"].ToString(),
                    });
                }
                con.Close();

                ViewBag.ValuersList = adminValuers.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return PartialView("PropPerUserAllocate", new { userName = userName });//journals
        }

        public IActionResult AllocatingTask(List<int> selectedItems, string? JournalName, string? priorityOrIndifference) 
        {
            var userSector = TempData["currentUserSector"]; //Assigning temp data with the user sector to get the sectors related to the user
            TempData.Keep("currentUserSector");

            string userName = TempData["currentUserFirstname"].ToString() + ' ' + TempData["currentUserSurname"].ToString();

            if (journals.Count > 0)
            {
                journals.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;

                com.CommandText = "EXEC AllocatingTask_Procedure @JournalUserName, @JournalId, @PriorityOrIndifference";

                //com.Parameters.AddWithValue("@JournalUserName", JournalName);

                foreach (int journalId in selectedItems)
                {
                    com.Parameters.Clear();

                    com.Parameters.AddWithValue("@JournalId", journalId);
                    com.Parameters.AddWithValue("@JournalUserName", JournalName);
                    com.Parameters.AddWithValue("@PriorityOrIndifference", priorityOrIndifference);
                    dr = com.ExecuteReader();

                    while (dr.Read())
                    {
                        journals.Add(new Journals
                        {

                        });
                    }

                    dr.Close(); 
                }

                con.Close();

                //TempData["journals"] = JsonConvert.SerializeObject(journals);
                TempData["currentUserSector"] = userSector;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("AllocateTask", new { userName = userName });
        }

        public IActionResult PropPerUser(string? userName)
        {
            var userSector = TempData["currentUserSector"]; //Assigning temp data with the user sector to get the sectors related to the user
            TempData.Keep("currentUserSector");

            //userName = TempData["currentUserFirstname"].ToString() +' ' + TempData["currentUserSurname"].ToString();

            if (journals.Count > 0)
            {
                journals.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT D.[Premise ID] ,D.[Account Number] ,D.[Installation] ,D.[Market Value] ,D.[Category] ,D.[Valuation Date] ,D.[WEF] ,D.[Net Accrual] ,D.[File Name] ,D.[Journal_Id] ,D.[Status] ,D.[Allocated Name] FROM [Journals].[dbo].[Details] D LEFT JOIN [Journals].[dbo].[Journals_Audit] J ON D.[Premise ID] = J.[Premise ID] WHERE D.[Allocated Name] = '" + userName + "' AND (J.[Status] IS NULL OR J.[Status] = 'Transaction Processed' OR J.[Status] <> 'Transaction Finalized') GROUP BY D.[Premise ID] ,D.[Account Number] ,D.[Installation] ,D.[Market Value]  ,D.[Category] ,D.[Valuation Date] ,D.[WEF] ,D.[Net Accrual] ,D.[File Name] ,D.[Journal_Id] ,D.[Status] ,D.[Allocated Name]";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    journals.Add(new Journals
                    {
                        Premise_ID = dr["Premise ID"].ToString(),
                        Account_Number = dr["Account Number"].ToString(),
                        Installation = dr["Installation"].ToString(),
                        Market_Value = dr["Market Value"].ToString(),
                        Category = dr["Category"].ToString(),
                        Valuation_Date = dr["Valuation Date"].ToString(),
                        WEF = dr["WEF"].ToString(),
                        Net_Accrual = dr["Net Accrual"].ToString(),
                        File_Name = dr["File Name"].ToString(),
                        Status = dr["Status"].ToString(),
                        Allocated_Name = dr["Allocated Name"].ToString(),
                        Journal_Id = dr["Journal_Id"].ToString()
                    });
                }
                con.Close();

                ViewBag.UserDataList = journals.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return PartialView("PropPerUser", new { userName = userName });//journals
        }

        public IActionResult ViewProperty(string? id)
        {
            if (journals.Count > 0)
            {
                journals.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT DISTINCT [LIS CAT] FROM [Journals].[dbo].[Tariffs table] ORDER BY [LIS CAT]";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    categories.Add(new Category
                    {
                        CatDescName = dr["LIS CAT"].ToString(),
                    });
                }
                con.Close();

                ViewBag.CategoriesList = categories.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            categories.Clear();

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT DISTINCT Financial_Year FROM [Journals].[dbo].[Tariffs table] ORDER BY Financial_Year";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    categories.Add(new Category
                    {
                        Financial_Year = dr["Financial_Year"].ToString(),
                    });
                }
                con.Close();

                ViewBag.FinancialYearList = categories.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            categories.Clear();

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT DISTINCT [Residential Threshold], [Rates_Tariff] FROM [Journals].[dbo].[Tariffs table] WHERE [Residential Threshold] IS NOT NULL ORDER BY [Residential Threshold]";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    categories.Add(new Category
                    {
                        Threshold = Convert.ToDecimal(dr["Residential Threshold"]),
                        Rates_Tariff = Convert.ToDecimal(dr["Rates_Tariff"]),
                    });
                }
                con.Close();

                //ViewBag.Threshold = categories.ToList();
                ViewBag.ThresholdRates_Tariff = JsonConvert.SerializeObject(categories);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            categories.Clear();

            
            if (journals.Count > 0)
            {
                journals.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;

                // First query to get the Premise ID from the Details table
                com.CommandText = "SELECT [Premise ID], [Account Number], [Installation], [Market Value], [Category], [Valuation Date], [WEF], [Net Accrual], [File Name], [Status], [Allocated Name], [Journal_Id], [Valuation Date] FROM [Journals].[dbo].[Details] WHERE Journal_Id = @JournalId";
                com.Parameters.AddWithValue("@JournalId", id);
                string premiseId = null;

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    premiseId = dr["Premise ID"].ToString();

                    journals.Add(new Journals
                    {
                        Premise_ID = premiseId,
                        Account_Number = dr["Account Number"].ToString(),
                        Installation = dr["Installation"].ToString(),
                        Market_Value = dr["Market Value"].ToString(),
                        Category = dr["Category"].ToString(),
                        Valuation_Date = dr["Valuation Date"].ToString(),
                        WEF = dr["WEF"].ToString(),
                        Net_Accrual = dr["Net Accrual"].ToString(),
                        File_Name = dr["File Name"].ToString(),
                        Status = dr["Status"].ToString(),
                        Allocated_Name = dr["Allocated Name"].ToString(),
                        Journal_Id = dr["Journal_Id"].ToString(),
                        ValuationDate = dr["Valuation Date"].ToString()
                    });
                }
                dr.Close();

                ViewBag.JournalListDetails = journals.ToList();

                if (premiseId != null)
                {
                    journals.Clear();

                    // Second query to get the journal details using the Premise ID
                    com.CommandText = "SELECT * FROM [Journals].[dbo].[Journals_Audit] WHERE [Premise ID] = @PremiseID ORDER BY Activity_Date";
                    com.Parameters.AddWithValue("@PremiseID", premiseId);

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        journals.Add(new Journals
                        {
                            Premise_ID = dr["Premise ID"].ToString(),
                            Account_Number = dr["Account Number"].ToString(),
                            Installation = dr["Installation"].ToString(),
                            Market_Value = dr["Market_Value"].ToString(),
                            Category = dr["Category"].ToString(),
                            BillingFrom = (DateTime)dr["BillingFrom"],
                            BillingTo  = (DateTime)dr["BillingTo"],
                            BillingDays = dr["BillingDays"].ToString(),
                            Threshold = dr["Threshold"].ToString(),
                            RatableValue = dr["RatableValue"].ToString(),
                            RatesTariff = dr["RatesTariff"].ToString(),
                            RebateType = dr["RebateType"].ToString(),
                            calculatedRate = dr["calculatedRate"].ToString(),
                            RebateAmount = dr["RebateAmount"].ToString(),
                            UserName = dr["UserName"].ToString(),
                            ToBeCharged = dr["ToBeCharged"].ToString()
                        });
                    }
                }
                con.Close();

                ViewBag.JournalListAudit = journals.ToList(); 
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(journals);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateValue(string? Journal_Id, string? PremiseId , string? Account_Number, 
            string? Installation, string? billingFrom, string? billingTo, string? billingDays, string? Market_Value, decimal? thresholdValue,
            string? RatableValue, float? rateTariffValue, string? RebateType, string? RebateAmount, string? calculatedRate, string? TobeCharged, string? ActualBilling, string? NetAdjustment,
            string? MarketValue1, string? MarketValue2, string? MarketValue3, 
            string? CATDescription, string? CATDescription1, string? CATDescription2, string? CATDescription3, string? Comment, string? WEF_DATE, string? userName, List<IFormFile> files)
        {
            var userID = TempData["currentUser"] as string; ;
            TempData.Keep("currentUser");

            var currentUserSurname = TempData["currentUserSurname"] as string; ;
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"] as string; ;
            TempData.Keep("currentUserFirstname");

            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");

            TempData["CATDescription"] = CATDescription;
            TempData["WEF_DATE"] = WEF_DATE;

            if (string.IsNullOrWhiteSpace(currentUserSurname) || string.IsNullOrWhiteSpace(currentUserFirstname) || string.IsNullOrWhiteSpace(userID))
            {
                TempData["RefreshMessage"] = $"User Surname or Firstname is missing or blank. Please refresh the page.";
                return RedirectToAction("RefreshMessage");
            }

            int count = 0;

            string Premise_ID = PremiseId.ToString();
            string uploadRoot = $"{_config["AppSettings:FileRooTPastJournal"]}";
            string folder = uploadRoot + "\\" + Premise_ID;
            // ******Check existance then create it.******
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string save_files = "";

            string FileNameAttach = "";

            List<string> Upload = new List<string>();
            foreach (IFormFile file in files)
            {
                count++;

                string fileName = Path.GetFileName(file.FileName);
                string filePath = $"{folder}\\{Path.GetFileName(file.FileName)}";

                switch (count)
                {
                    case 1:
                        FileNameAttach += fileName;
                        break;
                    case 2:
                        FileNameAttach += "," + fileName;
                        break;
                    case 3:
                        FileNameAttach += "," + fileName;
                        break;
                    case 4:
                        FileNameAttach += "," + fileName;
                        break;
                    case 5:
                        FileNameAttach += "," + fileName;
                        break;
                    case 6:
                        FileNameAttach += "," + fileName;
                        break;
                    case 7:
                        FileNameAttach += "," + fileName;
                        break;
                    case 8:
                        FileNameAttach += "," + fileName;
                        break;
                    case 9:
                        FileNameAttach += "," + fileName;
                        break;
                    case 10:
                        FileNameAttach += "," + fileName;
                        break;
                }
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                    Upload.Add(fileName);
                }

                save_files = FileNameAttach;
            }
           

            if (JournalHistories.Count > 0)
            {
                JournalHistories.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "INSERT INTO [Journals].[dbo].[Journals_Audit] ([UserName], [UserID], [Premise ID], [Account Number], [Installation], [BillingFrom]  ,[BillingTo] ,[BillingDays]  ,[Category], [Market_Value]  ,[Threshold] ,[RatableValue] ,[RatesTariff] ,[RebateType] ,[RebateAmount] ,[calculatedRate], [Status], [TobeCharged],  [Activity_Date]) " +
                                  "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PremiseId + "','" + Account_Number + "', '" + Installation + "','" + billingFrom + "', '" + billingTo + "', '" + billingDays + "', '" + CATDescription + "', '" + Market_Value + "' , '" + thresholdValue + "', '" + RatableValue + "', '" + rateTariffValue + "', '" + RebateType + "', '" + RebateAmount + "', '" + calculatedRate + "', 'Transaction Processed', '" + TobeCharged + "' , '" + DateTime.Now + "')";
                //while (dr.Read())
                //{
                //    JournalHistories.Add(new JournalHistory
                //    {

                //    });
                //}
                //con.Close();
                com.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("ViewProperty", new { id = Journal_Id });

        }

        [HttpPost]
        public async Task<IActionResult> SubmitTask(string? Journal_Id, string? PremiseId, string? Account_Number,
            string? Installation, string? billingFrom, string? billingTo, string? billingDays, string? Market_Value, decimal? thresholdValue,
            string? RatableValue, float? rateTariffValue, string? RebateType, string? RebateAmount, string? calculatedRate, string? TobeCharged, string? ActualBilling, string? NetAdjustment,
            string? MarketValue1, string? MarketValue2, string? MarketValue3,
            string? CATDescription, string? CATDescription1, string? CATDescription2, string? CATDescription3, string? Comment, string? WEF_DATE, string? userName, List<IFormFile> files)
        {
            var userID = TempData["currentUser"];
            TempData.Keep("currentUser");

            var currentUserSurname = TempData["currentUserSurname"];
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"];
            TempData.Keep("currentUserFirstname");

            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");

            TempData["CATDescription"] = CATDescription;
            TempData["WEF_DATE"] = WEF_DATE;

            int count = 0;

            string Premise_ID = PremiseId.ToString();
            string uploadRoot = $"{_config["AppSettings:FileRooTPastJournal"]}";
            string folder = uploadRoot + "\\" + Premise_ID;
            // ******Check existance then create it.******
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string save_files = "";

            string FileNameAttach = "";

            List<string> Upload = new List<string>();
            foreach (IFormFile file in files)
            {
                count++;

                string fileName = Path.GetFileName(file.FileName);
                string filePath = $"{folder}\\{Path.GetFileName(file.FileName)}";

                switch (count)
                {
                    case 1:
                        FileNameAttach += fileName;
                        break;
                    case 2:
                        FileNameAttach += "," + fileName;
                        break;
                    case 3:
                        FileNameAttach += "," + fileName;
                        break;
                    case 4:
                        FileNameAttach += "," + fileName;
                        break;
                    case 5:
                        FileNameAttach += "," + fileName;
                        break;
                    case 6:
                        FileNameAttach += "," + fileName;
                        break;
                    case 7:
                        FileNameAttach += "," + fileName;
                        break;
                    case 8:
                        FileNameAttach += "," + fileName;
                        break;
                    case 9:
                        FileNameAttach += "," + fileName;
                        break;
                    case 10:
                        FileNameAttach += "," + fileName;
                        break;
                }
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                    Upload.Add(fileName);
                }

                save_files = FileNameAttach;
            }

            if (JournalHistories.Count > 0)
            {
                JournalHistories.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "BEGIN TRANSACTION; UPDATE [Journals].[dbo].[Journals_Audit] SET STATUS = (SELECT [Status_Description] FROM [Journals].[dbo].[Status] WHERE Status_ID = '5') WHERE [Premise ID] = '" + PremiseId + "' UPDATE [Journals].[dbo].[Details] SET [Status] = 'Transaction Finalized', End_Date = GETDATE() WHERE [Premise ID] = '" + PremiseId + "' COMMIT TRANSACTION;";

                com.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("PropPerUser", new { userName = userName });

        }

        public IActionResult Transaction_To_Be_Checked()
        {
            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");

            if (journals.Count > 0)
            {
                journals.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [Premise ID], Status, UserName FROM [Journals].[dbo].[Journals_Audit] WHERE Status IN (SELECT [Status_Description] FROM [Journals].[dbo].[Status] WHERE Status_ID IN (5)) GROUP BY [Premise ID], Status, UserName";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    journals.Add(new Journals
                    {
                        Premise_ID = dr["Premise ID"].ToString(),
                        //Account_Number = dr["Account Number"].ToString(),
                        //Installation = dr["Installation"].ToString(),
                        //Market_Value = dr["Market_Value"].ToString(),
                        //Category = dr["Category"].ToString(),
                        //Valuation_Date = dr["Valuation Date"].ToString(),
                        //WEF = dr["WEF"].ToString(),
                        //Net_Accrual = dr["Net Accrual"].ToString(),
                        //File_Name = dr["File Name"].ToString(),
                        Status = dr["Status"].ToString(),
                        Allocated_Name = dr["UserName"].ToString(),
                        //Journal_Id = dr["Journal_Id"].ToString()

                    });
                }
                con.Close();

                ViewBag.UserDataList = journals.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }            

            TempData["UpdateRevisedValueSuccess"] = "Revised value(s) has been successfully updated";

            return View(journals);//journals
        }

        public IActionResult ViewTransactions(string? PremiseID)
        {
            var userSector = TempData["currentUserSector"]; 
            TempData.Keep("currentUserSector");

            if (journals.Count > 0)
            {
                journals.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [Journals].[dbo].[Journals_Audit] WHERE [Premise ID] = '" + PremiseID + "' order by Activity_Date";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    journals.Add(new Journals
                    {
                        Premise_ID = dr["Premise ID"].ToString(),
                        Account_Number = dr["Account Number"].ToString(),
                        Installation = dr["Installation"].ToString(),
                        Market_Value = dr["Market_Value"].ToString(),
                        Category = dr["Category"].ToString(),
                        BillingFrom = (DateTime)dr["BillingFrom"],
                        BillingTo = (DateTime)dr["BillingTo"],
                        BillingDays = dr["BillingDays"].ToString(),
                        Threshold = dr["Threshold"].ToString(),
                        RatableValue = dr["RatableValue"].ToString(),
                        RatesTariff = dr["RatesTariff"].ToString(),
                        RebateType = dr["RebateType"].ToString(),
                        calculatedRate = dr["calculatedRate"].ToString(),
                        RebateAmount = dr["RebateAmount"].ToString(),
                        UserName = dr["UserName"].ToString(),
                        Activity_Date = (DateTime)dr["Activity_Date"]

                    });
                }
                con.Close();

                ViewBag.PremiseID = journals.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            TempData["UpdateRevisedValueSuccess"] = "Revised value(s) has been successfully updated";

            return View(journals);//journals
        }

        public IActionResult DownloadFiles(string PremiseID)
        {
            string folderPath = @"E:\\JOURNALS_TEST\\" + PremiseID + "";
            //string[] filePaths = Directory.GetFiles(folderPath);

            if (!Directory.Exists(folderPath))
            {
                TempData["ErrorMessage"] = $"Premise Id: {PremiseID} evidence not uploaded";
                return RedirectToAction("ShowError");
            }

            string[] filePaths = Directory.GetFiles(folderPath);

            if (filePaths.Length == 0)
            {
                TempData["ErrorMessage"] = $"Premise Id: {PremiseID} evidence not uploaded";
                return RedirectToAction("ShowError");
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in filePaths)
                    {
                        string fileName = Path.GetFileName(filePath);
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            var entry = archive.CreateEntry(fileName);
                            using (var entryStream = entry.Open())
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }
                }

                //memoryStream.Position = 0;
                byte[] combinedData = memoryStream.ToArray();

                using (var finalMemoryStream = new MemoryStream())
                {
                    //memoryStream.CopyTo(finalMemoryStream);
                    finalMemoryStream.Position = 0;

                    return File(combinedData, "application/zip", PremiseID + " Files.zip");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> TransactionApproveReject(string PremiseId, string ActionType)
        {
            try
            {
                con.Open();
                com.Connection = con;

                if (ActionType == "Approve")
                {
                    com.CommandText = "UPDATE [Journals].[dbo].[Journals_Audit] SET STATUS = 'Approved' WHERE [Premise ID] = @PremiseId";
                    TempData["SuccessMessage"] = $"Transaction successfully {ActionType.ToLower()}d!";
                }
                else if (ActionType == "Reject")
                {
                    com.CommandText = "UPDATE [Journals].[dbo].[Journals_Audit] SET STATUS = 'Rejected' WHERE [Premise ID] = @PremiseId";
                    TempData["SuccessMessage"] = $"Transaction successfully {ActionType.ToLower()}ed!";
                }

                com.Parameters.AddWithValue("@PremiseId", PremiseId);
                com.ExecuteNonQuery();                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }

            return RedirectToAction("Transaction_To_Be_Checked");
        }

        public IActionResult ShowError()
        {
            return View();
        }

        public IActionResult RefreshMessage()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
