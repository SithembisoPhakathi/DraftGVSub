using GeneralValuationSubs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Drawing;

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
                com.CommandText = "SELECT Top(100) * FROM [Journals].[dbo].[Details] WHERE Status IN (SELECT [Status_Description] FROM [Journals].[dbo].[Status] WHERE Status_ID IN (1, 2, 5))"; 

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
                        ValuationDate = dr["Valuation Date"].ToString()

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

            TempData["UpdateRevisedValueSuccess"] = "Revised value(s) has been successfully updated";

            return PartialView("PropPerUserAllocate", new { userName = userName });//journals
        }

        public IActionResult AllocatingTask(List<int> selectedItems, string? JournalName) 
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

                // Create a parameterized query to avoid SQL injection
                com.CommandText = "EXEC AllocatingTask_Procedure @JournalUserName, @JournalId";

                com.Parameters.AddWithValue("@JournalUserName", JournalName);

                foreach (int journalId in selectedItems)
                {
                    com.Parameters.Clear();

                    com.Parameters.AddWithValue("@JournalId", journalId);
                    com.Parameters.AddWithValue("@JournalUserName", JournalName);
                    dr = com.ExecuteReader();

                    while (dr.Read())
                    {
                        journals.Add(new Journals
                        {

                        });
                    }

                    dr.Close(); // Close the DataReader before the next iteration
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
                com.CommandText = "SELECT * FROM [Journals].[dbo].[Details] WHERE [Allocated Name] = '" + userName + "'";

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
                com.CommandText = "SELECT DISTINCT [Residential Threshold] FROM [Journals].[dbo].[Tariffs table] WHERE [Residential Threshold] IS NOT NULL ORDER BY [Residential Threshold]";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    categories.Add(new Category
                    {
                        Threshold = Convert.ToDecimal(dr["Residential Threshold"]),
                    });
                }
                con.Close();

                ViewBag.Threshold = categories.ToList();

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
                com.CommandText = "SELECT DISTINCT [Rates_Tariff] FROM [Journals].[dbo].[Tariffs table] ORDER BY [Rates_Tariff]";
                dr = com.ExecuteReader();
                while (dr.Read()) 
                {
                    categories.Add(new Category
                    {
                        Rates_Tariff = Convert.ToDecimal(dr["Rates_Tariff"]),
                    });
                }
                con.Close();

                ViewBag.Rates_Tariff = categories.ToList();

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
                    com.CommandText = "SELECT * FROM [Journals].[dbo].[Journals_Audit] WHERE [Premise ID] = @PremiseID";
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
                            UserName = dr["UserName"].ToString()
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
            string? Installation, string? billingFrom, string? billingTo, string? billingDays, string? Market_Value, decimal? Threshold,
            string? RatableValue, float? RatesTariff, string? RebateType, string? RebateAmount, string? calculatedRate, string? TobeCharged, string? ActualBilling, string? NetAdjustment,
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

            //if (journals.Count > 0)
            //{
            //    journals.Clear();
            //}
            //try
            //{
            //    con.Open();
            //    com.Connection = con;

            //    //string fileNameAttachValue = (files != null && files.FileName != null) ? Path.GetFileName(files.FileName) : null;

            //    com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValued] SET [Market Value1] = '" + MarketValue1 + "', [Market Value2] = '" + MarketValue2 + "', [Market Value3] = '" + MarketValue3 + "', [CAT Description] = '" + CATDescription + "', " + "[CAT Description1] = '" + CATDescription1 + "', [CAT Description2] = '" + CATDescription2 + "', [CAT Description3] = '" + CATDescription3 + "', Comment = '" + Comment + "', WEF_DATE = '" + WEF_DATE + "', Activity_Date = getdate(), " +
            //                        "FileNameAttach = '" + save_files + "', Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2) WHERE DraftId = '" + Journal_Id + "'";

            //    dr = com.ExecuteReader();
            //    while (dr.Read())
            //    {
            //        journals.Add(new Journals
            //        {

            //        });
            //    }

            //    con.Close();

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            if (JournalHistories.Count > 0)
            {
                JournalHistories.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "INSERT INTO [Journals].[dbo].[Journals_Audit] ([UserName], [UserID], [Premise ID], [Account Number], [Installation], [BillingFrom]  ,[BillingTo] ,[BillingDays]  ,[Category], [Market_Value]  ,[Threshold] ,[RatableValue] ,[RatesTariff] ,[RebateType] ,[RebateAmount] ,[calculatedRate]) " +
                                  "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PremiseId + "','" + Account_Number + "', '" + Installation + "','" + billingFrom + "', '" + billingTo + "', '" + billingDays + "', '" + CATDescription + "', '" + Market_Value + "' , '" + Threshold + "', '" + RatableValue + "', '" + RatesTariff + "', '" + RebateType + "', '" + RebateAmount + "', '" + calculatedRate + "')";
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

        public IActionResult Index()
        {
            return View();
        }
    }
}
