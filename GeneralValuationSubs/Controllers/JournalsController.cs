using GeneralValuationSubs.Data;
using GeneralValuationSubs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReadingTextFile.Data;
using System;
using System.Drawing;
using System.Globalization;
using System.IO.Compression;
using System.Net.Mail;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GeneralValuationSubs.Controllers
{
    public class JournalsController : Controller
    {
        List<Journals_Audit> journals = new List<Journals_Audit>();
        List<Category> categories = new List<Category>();
        List<FileName> fileNames = new List<FileName>();
        List<AdminValuer> adminValuers = new();
        List<JournalHistory> JournalHistories = new List<JournalHistory>();
        SqlCommand com = new SqlCommand();
        SqlConnection con = new SqlConnection();
        SqlDataReader dr;
        EmailHelper emailHelper = new EmailHelper();
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;
        private TableViewModel _viewModel;
        private readonly FileParser _fileParser = new FileParser();
        private readonly ApplicationDbContext _AppContext;

        public JournalsController(Microsoft.Extensions.Configuration.IConfiguration config, ApplicationDbContext applicationDbContext)
        {
            _config = config;
            this._AppContext = applicationDbContext;
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
                com.CommandText = "SELECT DISTINCT [File Name] FROM [Journals].[dbo].[Details] WHERE [File Name] IS NOT NULL";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    fileNames.Add(new FileName
                    {
                        FileNames = dr["File Name"].ToString(),
                    });
                }
                con.Close();

                ViewBag.FileNames = fileNames.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return PartialView("PropPerUserAllocate", new { userName = userName });//journals
        }


        [HttpPost]
        public IActionResult AllocateTask(string? userName, string? FileName)
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
                com.CommandText = "SELECT DISTINCT [File Name] FROM [Journals].[dbo].[Details] WHERE [File Name] IS NOT NULL";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    fileNames.Add(new FileName
                    {
                        FileNames = dr["File Name"].ToString(),
                    });
                }
                con.Close();

                ViewBag.FileNames = fileNames.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (journals.Count > 0)
            {
                journals.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT dbo.GetBusinessDaysDifference(GETDATE(), ISNULL(End_Date, GETDATE())) AS Date_Diff, * FROM [Journals].[dbo].[Details] WHERE Status IN (SELECT [Status_Description] FROM [Journals].[dbo].[Status] WHERE Status_ID IN (1, 2, 3, 5, 6, 7)) and [File Name] = '" + FileName + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    journals.Add(new Journals_Audit
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

                ViewBag.LVCList = adminValuers.ToList();
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
                        journals.Add(new Journals_Audit
                        {

                        });
                    }

                    dr.Close();
                }

                con.Close();

                TempData["AllocateTaskMessage"] = $"Task(s) is successfully allocated!";

                TempData["currentUserSector"] = userSector;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            List<string> emails = new List<string>();

            if (journals.Count > 0)
            {
                journals.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;

                com.CommandText = "EXEC [dbo].[SendEmailTask] @FirstNameSurname = '" + JournalName + "'";

                using (SqlDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        emails.Add(reader["Email_Address"].ToString());
                    }
                }

                if (emails.Count > 0)
                {
                    emailHelper.SendEmailTaskAllocated(string.Join(",", emails), JournalName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
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
                com.CommandText = "SELECT D.[Premise ID] ,D.[Account Number] ,D.[Installation] ,D.[Market Value] ,D.[Category] ,D.[Valuation Date] ,D.[WEF] ,D.[Net Accrual] ,D.[File Name] ,D.[Journal_Id] ,D.[Status] ,D.[Allocated Name], D.End_Date, dbo.GetBusinessDaysDifference(GETDATE(), ISNULL(End_Date, GETDATE())) AS Date_Diff FROM [Journals].[dbo].[Details] D LEFT JOIN [Journals].[dbo].[Journals_Audit] J ON D.[Premise ID] = J.[Premise ID] WHERE D.[Allocated Name] = '" + userName + "' AND (J.[Status] IS NULL OR J.[Status] = 'Transaction Processed' OR J.[Status] = 'Rejected' OR J.[Status] NOT IN ('Transaction Finalized', 'Approved')) GROUP BY D.[Premise ID] ,D.[Account Number] ,D.[Installation] ,D.[Market Value]  ,D.[Category] ,D.[Valuation Date] ,D.[WEF] ,D.[Net Accrual] ,D.[File Name] ,D.[Journal_Id] ,D.[Status] ,D.[Allocated Name], D.End_Date";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    journals.Add(new Journals_Audit
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
                        End_Date = (DateTime)dr["End_Date"],
                        DateDiff = (int)dr["Date_Diff"],
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
            DateTime? previousBillingTo = null;
            DateTime? newBillingFrom = null;

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
                com.CommandText = "SELECT DISTINCT Financial_Year FROM [Journals].[dbo].[Tariffs table] WHERE Financial_Year NOT IN ('2024-2025') ORDER BY Financial_Year DESC";

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
                com.CommandText = "SELECT [Premise ID], [Account Number], [Installation], [Market Value], [Category], [Valuation Date], [WEF], [Net Accrual], [File Name], [Status], [Allocated Name], [Journal_Id], [File Name], [Valuation Date] FROM [Journals].[dbo].[Details] WHERE Journal_Id = @JournalId";
                com.Parameters.AddWithValue("@JournalId", id);

                string premiseId = null;

                TempData["Journal_Id"] = id;
                ViewBag.Journal_Id = id;

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    premiseId = dr["Premise ID"].ToString();

                    journals.Add(new Journals_Audit
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
                        ValuationDate = dr["Valuation Date"].ToString(),
                        FileName = dr["File Name"].ToString()
                    });
                }
                dr.Close();

                ViewBag.JournalListDetails = journals.ToList();

                if (premiseId != null)
                {
                    journals.Clear();

                    // Second query to get the journal details using the Premise ID
                    com.CommandText = "SELECT * FROM [Journals].[dbo].[Journals_Audit] WHERE [Premise ID] = @PremiseID AND [Journal_Id] = @Journal_ID ORDER BY Activity_Date";
                    com.Parameters.AddWithValue("@PremiseID", premiseId);
                    com.Parameters.AddWithValue("@Journal_ID", id);

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        journals.Add(new Journals_Audit
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
                            ToBeCharged = dr["ToBeCharged"].ToString(),
                            ActualBilling = dr["ActualBilling"].ToString(),
                            NetAdjustment = dr["NetAdjustment"].ToString(),
                            Transaction_ID = (int)dr["Transaction_ID"],
                            Status = dr["Status"].ToString(),
                            Comment = dr["Comment"].ToString(),
                            ApproverComment = dr["ApproverComment"].ToString(),
                            FileName = dr["File_Name"].ToString()
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
        public async Task<IActionResult> UpdateValue(string? Journal_Id, string? PremiseId, string? Account_Number,
            string? Installation, string? billingFrom, string? billingTo, string? billingDays, string? Market_Value, decimal? thresholdValue,
            string? RatableValue, float? rateTariffValue, string? RebateType, string? RebateAmount, string? calculatedRate, string? TobeCharged, string? ActualBilling,
            string? NetAdjustment, string? MarketValue1, string? MarketValue2, string? MarketValue3, string? FinancialYear, string? FileName,
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
            string folder = uploadRoot + "\\" + "Journal" + ' ' + Journal_Id + ' ' + Premise_ID;
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
                com.CommandText = "BEGIN TRANSACTION; UPDATE [Journals].[dbo].[Details] SET [Status] = (SELECT [Status_Description] FROM [dbo].[Status] WHERE Status_ID = '4') WHERE Journal_Id = '" + Journal_Id + "' INSERT INTO [Journals].[dbo].[Journals_Audit] ([UserName], [UserID], [Premise ID], [Account Number], [Installation], [File_Name], [FinancialYear] , [BillingFrom]  ,[BillingTo] ,[BillingDays]  ,[Category], [Market_Value]  ,[Threshold] ,[RatableValue] ,[RatesTariff] ,[RebateType] ,[RebateAmount] ,[calculatedRate], [Status], [TobeCharged], [ActualBilling], [NetAdjustment], [Activity_Date], [Journal_Id]) " +
                                  "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PremiseId + "','" + Account_Number + "', '" + Installation + "', '" + FileName + "' ,'" + FinancialYear + "' ,'" + billingFrom + "', '" + billingTo + "', '" + billingDays + "', '" + CATDescription + "', '" + Market_Value + "' , '" + thresholdValue + "', '" + RatableValue + "', '" + rateTariffValue + "', '" + RebateType + "', '" + RebateAmount + "', '" + calculatedRate + "', 'Transaction Processed', '" + TobeCharged + "', '" + ActualBilling + "','" + NetAdjustment + "', '" + DateTime.Now + "', '" + Journal_Id + "') COMMIT TRANSACTION;";
                //while (dr.Read())
                //{
                //    JournalHistories.Add(new JournalHistory
                //    {

                //    });
                //}
                //con.Close();
                com.ExecuteNonQuery();

                TempData["SaveMessage"] = $"Transaction successfully saved!";

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
            string? RatableValue, float? rateTariffValue, string? RebateType, string? RebateAmount, string? calculatedRate, string? TobeCharged, string? ActualBilling,
            string? NetAdjustment, string? Comment, string? MarketValue1, string? MarketValue2, string? MarketValue3,
            string? CATDescription, string? CATDescription1, string? CATDescription2, string? CATDescription3, string? WEF_DATE, string? userName, List<IFormFile> files)
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
            string folder = uploadRoot + "\\" + "Journal" + ' ' + Journal_Id + ' ' + Premise_ID;
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
                com.CommandText = "BEGIN TRANSACTION; UPDATE [Journals].[dbo].[Journals_Audit] SET STATUS = (SELECT [Status_Description] FROM [Journals].[dbo].[Status] WHERE Status_ID = '5'), Comment = '" + Comment + "' WHERE [Premise ID] = '" + PremiseId + "' AND [Journal_Id] = '" + Journal_Id + "' UPDATE [Journals].[dbo].[Details] SET [Status] = (SELECT [Status_Description] FROM [Journals].[dbo].[Status] WHERE Status_ID = '5') WHERE [Premise ID] = '" + PremiseId + "' AND [Journal_Id] = '" + Journal_Id + "' COMMIT TRANSACTION;";

                com.ExecuteNonQuery();

                TempData["SubmitTaskMessage"] = $"Task successfully submitted!";

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("PropPerUser", new { userName = userName });

        }

        public IActionResult Pending()
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
                com.CommandText = "SELECT [Premise ID], Status, UserName, Journal_Id, [File_Name] FROM [Journals].[dbo].[Journals_Audit] WHERE Status IN (SELECT [Status_Description] FROM [Journals].[dbo].[Status] WHERE Status_ID IN (5)) GROUP BY [Premise ID], Status, UserName, Journal_Id, File_Name";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    journals.Add(new Journals_Audit
                    {
                        Premise_ID = dr["Premise ID"].ToString(),
                        //Account_Number = dr["Account Number"].ToString(),
                        //Installation = dr["Installation"].ToString(),
                        //Market_Value = dr["Market_Value"].ToString(),
                        //Category = dr["Category"].ToString(),
                        //Valuation_Date = dr["Valuation Date"].ToString(),
                        //WEF = dr["WEF"].ToString(),
                        //Net_Accrual = dr["Net Accrual"].ToString(),
                        FileName = dr["File_Name"].ToString(),
                        Status = dr["Status"].ToString(),
                        Allocated_Name = dr["UserName"].ToString(),
                        Journal_ID = (int)dr["Journal_Id"]
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

        public IActionResult PendingFinalized()
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
                com.CommandText = "SELECT [Premise ID], Status, UserName, Journal_Id, [File_Name] FROM [Journals].[dbo].[Journals_Audit] WHERE Status IN (SELECT [Status_Description] FROM [Journals].[dbo].[Status] WHERE Status_ID IN (5)) GROUP BY [Premise ID], Status, UserName, Journal_Id, File_Name";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    journals.Add(new Journals_Audit
                    {
                        Premise_ID = dr["Premise ID"].ToString(),
                        //Account_Number = dr["Account Number"].ToString(),
                        //Installation = dr["Installation"].ToString(),
                        //Market_Value = dr["Market_Value"].ToString(),
                        //Category = dr["Category"].ToString(),
                        //Valuation_Date = dr["Valuation Date"].ToString(),
                        //WEF = dr["WEF"].ToString(),
                        //Net_Accrual = dr["Net Accrual"].ToString(),
                        FileName = dr["File_Name"].ToString(),
                        Status = dr["Status"].ToString(),
                        Allocated_Name = dr["UserName"].ToString(),
                        Journal_ID = (int)dr["Journal_Id"]
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

        public IActionResult ViewTransactions(string? PremiseID, int? JournalID)
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
                com.CommandText = "SELECT * FROM [Journals].[dbo].[Journals_Audit] WHERE [Premise ID] = '" + PremiseID + "' AND [Journal_Id] = '" + JournalID + "' order by Activity_Date";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    journals.Add(new Journals_Audit
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
                        Activity_Date = (DateTime)dr["Activity_Date"],
                        Transaction_ID = (int)dr["Transaction_ID"],
                        Journal_ID = (int)dr["Journal_Id"]
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

        public IActionResult ViewTransactionsFinalized(string? PremiseID, int? JournalID)
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
                com.CommandText = "SELECT * FROM [Journals].[dbo].[Journals_Audit] WHERE [Premise ID] = '" + PremiseID + "' AND [Journal_Id] = '" + JournalID + "' order by Activity_Date";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    journals.Add(new Journals_Audit
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
                        Activity_Date = (DateTime)dr["Activity_Date"],
                        Transaction_ID = (int)dr["Transaction_ID"],
                        Journal_ID = (int)dr["Journal_Id"]
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

        public IActionResult DownloadFiles(int JournalID, string PremiseID)
        {
            string folderPath = @"E:\\JOURNALS_TEST\\" + "Journal " + JournalID + " " + PremiseID + "";
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

                    return File(combinedData, "application/zip", "Journal " + JournalID + " " + PremiseID + " Files.zip");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> TransactionApproveReject(string PremiseId, int JournalId, string ActionType, string? ApproverComment)
        {
            try
            {
                con.Open();
                com.Connection = con;

                if (ActionType == "Approve")
                {
                    com.CommandText = "BEGIN TRANSACTION; UPDATE [Journals].[dbo].[Details] SET [Status] = (SELECT [Status_Description] FROM [dbo].[Status] WHERE Status_ID = '6') WHERE Journal_Id = '" + JournalId + "'  UPDATE [Journals].[dbo].[Journals_Audit] SET STATUS = (SELECT Status_Description FROM [Journals].[dbo].[Status] WHERE Status_ID = '6'), ApproverComment = '" + ApproverComment + "' WHERE [Premise ID] = @PremiseId AND [Journal_Id] = @JournalId COMMIT TRANSACTION;";
                    TempData["SuccessMessage"] = $"Transaction successfully {ActionType.ToLower()}d!";
                }
                else if (ActionType == "Reject")
                {
                    com.CommandText = "BEGIN TRANSACTION; UPDATE [Journals].[dbo].[Details] SET [Status] = (SELECT [Status_Description] FROM [dbo].[Status] WHERE Status_ID = '7') WHERE Journal_Id = '" + JournalId + "' UPDATE [Journals].[dbo].[Journals_Audit] SET STATUS = (SELECT Status_Description FROM [Journals].[dbo].[Status] WHERE Status_ID = '7'), ApproverComment = '" + ApproverComment + "' WHERE [Premise ID] = @PremiseId AND [Journal_Id] = @JournalId COMMIT TRANSACTION;";
                    TempData["SuccessMessage"] = $"Transaction successfully {ActionType.ToLower()}ed!";
                }

                com.Parameters.AddWithValue("@PremiseId", PremiseId);
                com.Parameters.AddWithValue("@JournalId", JournalId);
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

            return RedirectToAction("Pending");
        }

        public IActionResult Edit_Transaction(string? id)
        {
            if (journals.Count > 0)
            {
                journals.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT DISTINCT Financial_Year FROM [Journals].[dbo].[Tariffs table] WHERE Financial_Year NOT IN ('2024-2025') ORDER BY Financial_Year DESC";

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

            if (journals.Count > 0)
            {
                journals.Clear();
            }
            try
            {
                con.Open();
                com.CommandText = "SELECT * FROM [Journals].[dbo].[Journals_Audit] WHERE [Transaction_ID] = @Transaction_ID ORDER BY Activity_Date";
                com.Parameters.AddWithValue("@Transaction_ID", id);

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    journals.Add(new Journals_Audit
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
                        ToBeCharged = dr["ToBeCharged"].ToString(),
                        ActualBilling = dr["ActualBilling"].ToString(),
                        NetAdjustment = dr["NetAdjustment"].ToString(),
                        FinancialYear = dr["FinancialYear"].ToString(),
                        Transaction_ID = (int)dr["Transaction_ID"]
                    });
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
        public async Task<IActionResult> Edit(string? Journal_Id, string? PremiseId, string? Account_Number,
            string? Installation, string? billingFrom, string? billingTo, string? billingDays, string? Market_Value, decimal? thresholdValue, int? Transaction_ID,
            string? RatableValue, float? rateTariffValue, string? RebateType, string? RebateAmount, string? calculatedRate, string? TobeCharged, string? ActualBilling, string? NetAdjustment, string? FinancialYear,
            string? MarketValue1, string? MarketValue2, string? MarketValue3, string? CATDescription, string? CATDescription1, string? CATDescription2, string? CATDescription3, string? Comment, string? WEF_DATE, string? userName, List<IFormFile> files)
        {
            var userID = TempData["currentUser"] as string; ;
            TempData.Keep("currentUser");

            var currentUserSurname = TempData["currentUserSurname"] as string;
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"] as string;
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

            if (JournalHistories.Count > 0)
            {
                JournalHistories.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "UPDATE [Journals].[dbo].[Journals_Audit] SET [UserName] = '" + currentUserFirstname + ' ' + currentUserSurname + "',  [UserID] = '" + userID + "',  [Account Number] = '" + Account_Number + "',  [Installation] = '" + Installation + "', [FinancialYear] = '" + FinancialYear + "', [BillingFrom] = '" + billingFrom + "', [BillingTo] = '" + billingTo + "', [BillingDays] = '" + billingDays + "', [Category] = '" + CATDescription + "', [Market_Value] = '" + Market_Value + "', [Threshold] = '" + thresholdValue + "', [RatableValue] = '" + RatableValue + "', [RatesTariff] = '" + rateTariffValue + "', [RebateType] = '" + RebateType + "', [RebateAmount] = '" + RebateAmount + "', [calculatedRate] = '" + calculatedRate + "', [Status] = 'Transaction Processed', [TobeCharged] = '" + TobeCharged + "', [ActualBilling] = '" + ActualBilling + "', [NetAdjustment] = '" + NetAdjustment + "', [Activity_Date] = '" + DateTime.Now + "' WHERE [Transaction_ID] = '" + Transaction_ID + "';";

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

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string? Journal_Id, string? PremiseId, string? Account_Number, string? Installation, string? FileName)
        {
            if (file != null && file.Length > 0)
            {
                _viewModel = new TableViewModel();

                string fileContent;
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    fileContent = reader.ReadToEnd();
                }

                _viewModel = ParseData(fileContent, Journal_Id, PremiseId, Account_Number, Installation, FileName);

                return RedirectToAction("ViewProperty", new { id = Journal_Id });
            }

            return View();
        }

        private TableViewModel ParseData(string content, string? Journal_Id, string? PremiseId, string? Account_Number, string? Installation, string? FileName)
        {
            var userID = TempData["currentUser"] as string; ;
            TempData.Keep("currentUser");

            var currentUserSurname = TempData["currentUserSurname"] as string; ;
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"] as string; ;
            TempData.Keep("currentUserFirstname");


            string[] format = new string[] { "dd.MM.yyyy" };
            int amountColumnIndex = -1;
            int docNoIndex = -1;
            int docDateIndex = -1;
            int descriptionIndex = -1;
            int divIndex = -1;
            int typeIndex = -1;

            long docNum = 0;
            double amount = 0;
            string divNum = "";
            string type = "";
            string description = "";
            DateTime Docdate = DateTime.Now;
            string dateVar = "";
            string format2 = "dd/MM/yyyy";

            DateTime dateOnly;
            var model = new TableViewModel();
            var rows = new List<TableRow>();

            using (StringReader reader = new StringReader(content))
            {
                string line;
                bool headersFound = false;

                while ((line = reader.ReadLine()) != null)
                {
                    if (!headersFound)
                    {
                        var potentialHeaders = line.Split('\t');
                        if (potentialHeaders.Contains("Stat"))
                        {
                            model.Headers = new List<string> { "DocDate", "Type", "DocNo", "Div", "Description", "Amount" };
                            headersFound = true;

                            // Find the indexes of required columns only
                            amountColumnIndex = Array.IndexOf(potentialHeaders, "Amount");
                            docDateIndex = Array.IndexOf(potentialHeaders, "DocDate");
                            typeIndex = Array.IndexOf(potentialHeaders, "Type");
                            docNoIndex = Array.IndexOf(potentialHeaders, "DocNo");
                            divIndex = Array.IndexOf(potentialHeaders, "Div");
                            descriptionIndex = Array.IndexOf(potentialHeaders, "Description");
                        }
                    }
                    else
                    {
                        var fields = line.Split('\t');
                        var filteredFields = new List<string>();

                        // Parsing and formatting the DocDate
                        if (docDateIndex >= 0 && docDateIndex < fields.Length)
                        {
                            dateVar = fields[docDateIndex].Replace(".", "/").Trim();
                            Docdate = DateTime.TryParseExact(dateVar, format2, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate)
                                ? parsedDate
                                : DateTime.MinValue;
                            filteredFields.Add(Docdate.ToString("dd/MM/yyyy"));
                        }

                        // Parsing the Type
                        if (typeIndex >= 0 && typeIndex < fields.Length)
                        {
                            type = fields[typeIndex];
                            filteredFields.Add(type);
                        }

                        // Parsing and converting the DocNo
                        if (docNoIndex >= 0 && docNoIndex < fields.Length)
                        {
                            string docNoString = fields[docNoIndex].Replace(",", "").Trim();

                            if (double.TryParse(docNoString, NumberStyles.Any, CultureInfo.InvariantCulture, out double docNoDouble))
                            {
                                docNum = (long)docNoDouble;
                            }
                            else
                            {
                                docNum = 0; // Default value if parsing fails
                            }

                            filteredFields.Add(docNum.ToString());
                        }

                        // Parsing the Div
                        if (divIndex >= 0 && divIndex < fields.Length)
                        {
                            divNum = fields[divIndex].Trim();
                            filteredFields.Add(divNum);
                        }

                        // Parsing the Description
                        if (descriptionIndex >= 0 && descriptionIndex < fields.Length)
                        {
                            description = fields[descriptionIndex];
                            filteredFields.Add(description);
                        }

                        // Parsing and converting the Amount
                        if (amountColumnIndex >= 0 && amountColumnIndex < fields.Length)
                        {
                            string cleanedValue = fields[amountColumnIndex].Replace(",", "").Trim();
                            amount = string.IsNullOrEmpty(cleanedValue) ? 0.0 : Convert.ToDouble(cleanedValue, CultureInfo.InvariantCulture);
                            filteredFields.Add(amount.ToString("F2"));
                        }

                        if (JournalHistories.Count > 0)
                        {
                            JournalHistories.Clear();
                        }
                        try
                        {
                            using (SqlConnection con = new SqlConnection(_config.GetConnectionString("JournalConnection")))
                            {
                                using (SqlCommand com = con.CreateCommand())
                                {
                                    // Open connection once
                                    con.Open();
                                    com.Connection = con;
                                    com.CommandText = "BEGIN TRANSACTION; UPDATE [Journals].[dbo].[Details] SET [Status] = (SELECT [Status_Description] FROM [dbo].[Status] WHERE Status_ID = '4') WHERE Journal_Id = '" + Journal_Id + "' INSERT INTO [Journals].[dbo].[Journals_Audit] ([UserName], [UserID], [Premise ID], [Account Number], [Installation], [File_Name], [FinancialYear] , [BillingFrom]  ,[BillingTo] ,[BillingDays]  ,[Category], [Market_Value]  ,[Threshold] ,[RatableValue] ,[RatesTariff] ,[RebateType] ,[RebateAmount] ,[calculatedRate], [Status], [TobeCharged], [ActualBilling], [NetAdjustment], [Activity_Date], [Journal_Id]) " +
                                                      "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PremiseId + "','" + Account_Number + "', '" + Installation + "', '" + FileName + "' ,'" + description + "' ,'" + Docdate + "', '" + Docdate + "', '" + Docdate + "', '" + description + "', '" + amount + "' , '" + amount + "', '" + amount + "', '" + amount + "', '" + amount + "', '" + amount + "', '" + amount + "', 'Transaction Processed', '" + amount + "', '" + amount + "','" + amount + "', '" + DateTime.Now + "', '" + Journal_Id + "') COMMIT TRANSACTION;";
                                    //while (dr.Read())
                                    //{
                                    //    JournalHistories.Add(new JournalHistory
                                    //    {

                                    //    });
                                    //}
                                    //con.Close();
                                    com.ExecuteNonQuery();

                                    TempData["SaveMessage"] = $"Transaction successfully saved!";

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        //Save record to database if needed
                        //_AppContext.Journals_Audit.Add(new Journals_Audit
                        //{
                        //    BillingFrom = Docdate,
                        //    Category = type,
                        //    Market_Value = docNum.ToString(),
                        //    ToBeCharged = divNum,
                        //    FileName = description,
                        //    RebateAmount = amount.ToString(),
                        //    FinancialYear = null

                        //    //DocDate = Docdate,
                        //    //Type = type,
                        //    //DocNo = docNum,
                        //    //Div = divNum,
                        //    //Description = description,
                        //    //Amount = amount,
                        //    //Financial_Year = null
                        //});
                        //_AppContext.SaveChanges();

                        // Add filtered data to TableRow
                        rows.Add(new TableRow { Columns = filteredFields });
                    }
                }
            }

            model.Rows = rows;
            return model;
        }
    }
}


