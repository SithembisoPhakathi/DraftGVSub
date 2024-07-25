using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GeneralValuationSubs.Models;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IO.Compression;

namespace GeneralValuationSubs.Controllers
{
    public class Section78Controller : Controller
    {
        private readonly ILogger<HomeController> _logger;


        private IWebHostEnvironment _environment;
        SqlCommand com = new();
        SqlDataReader dr;
        SqlConnection con = new();
        SqlConnection con2 = new SqlConnection();
        SqlConnection conQ = new SqlConnection();
        List<ObjectionTB> ObjectionTBs = new();
        List<Prop_View_Model> objectionsL = new();
        List<AdminValuer> adminValuers = new();
        List<Draft> drafts = new List<Draft>();
        List<DraftHistory> draftHistories = new List<DraftHistory>();
        List<Category> categories = new List<Category>();
        List<Link> links = new();

        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;


        public Section78Controller(ILogger<HomeController> logger, Microsoft.Extensions.Configuration.IConfiguration config, IWebHostEnvironment environment)
        {
            _config = config;
            _logger = logger;
            /*_db = dBContext;
            _property_DatabaseContext = property_DatabaseContext;*/
            con.ConnectionString = _config.GetConnectionString("defaultConnection");
            con2.ConnectionString = _config.GetConnectionString("defaultConnection");
            conQ.ConnectionString = _config.GetConnectionString("defaultQueryConnection");
            _environment = environment;
        }


        public IActionResult AllocateTask(string? userName)
        {
            var userSector = TempData["currentUserSector"]; //Assigning temp data with the user sector to get the sectors related to the user
            TempData.Keep("currentUserSector");

            //userName = TempData["currentUserFirstname"].ToString() +' ' + TempData["currentUserSurname"].ToString();

            if (drafts.Count > 0)
            {
                drafts.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT DATEDIFF(DAY,Start_Date, GETDATE()) AS Date_Diff, * FROM [UpdatedGVTool].[dbo].[NotValuedSection78] " +
                                 "WHERE (Dept_Dir LIKE '%" + userName + "%' OR Snr_Manager LIKE '%" + userName + "%' OR Area_Manager LIKE '%" + userName + "%' OR Candidate_DC LIKE '%" + userName + "%') AND Status IN (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID IN (5, 4))";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        QueryId = (int)dr["QueryId"],
                        QUERY_No = dr["QUERY_No"].ToString(),
                        PremiseId = dr["Premise ID"].ToString(),
                        PropertyDescription = dr["PROPERTY_DESC"].ToString(),
                        TownshipDescription = dr["Town Name Description"].ToString(),
                        Extent = dr["Unit Legal Area"].ToString(),
                        ValuationType = dr["Valuation Type"].ToString(),
                        ValuationTypeDescription = dr["Valuation Type Description"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        CATDescription = dr["CAT Description"].ToString(),
                        AllocatedName = dr["AllocateName"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        Status = dr["Status"].ToString(),
                        Dept_Dir = dr["Dept_Dir"].ToString(),
                        Snr_Manager = dr["Snr_Manager"].ToString(),
                        Area_Manager = dr["Area_Manager"].ToString(),
                        Candidate_DC = dr["Candidate_DC"].ToString(),
                        Start_Date = (DateTime)dr["Start_Date"],
                        DateDiff = (int)dr["Date_Diff"]
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.UserDataList = drafts.ToList();
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
                com.CommandText = "EXEC ValuersList_Procedure '" + userSector + "'";
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

            return PartialView("PropPerUserAllocate", new { userName = userName });//drafts
        }
        public IActionResult AllocatingTask(List<int> selectedItems, string? valuerName)
        {
            var userSector = TempData["currentUserSector"]; //Assigning temp data with the user sector to get the sectors related to the user
            TempData.Keep("currentUserSector");

            string userName = TempData["currentUserFirstname"].ToString() + ' ' + TempData["currentUserSurname"].ToString();

            if (drafts.Count > 0)
            {
                drafts.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;

                // Create a parameterized query to avoid SQL injection
                com.CommandText = "EXEC AllocatingTask_ProcedureSection78 @valuerName, @QueryId";

                com.Parameters.AddWithValue("@valuerName", valuerName);

                foreach (int QueryId in selectedItems)
                {
                    com.Parameters.Clear();

                    com.Parameters.AddWithValue("@QueryId", QueryId);
                    com.Parameters.AddWithValue("@valuerName", valuerName);
                    dr = com.ExecuteReader();

                    while (dr.Read())
                    {
                        drafts.Add(new Draft
                        {
                        });
                    }

                    dr.Close(); // Close the DataReader before the next iteration
                }

                con.Close();

                //TempData["Drafts"] = JsonConvert.SerializeObject(drafts);
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

            if (objectionsL.Count > 0)
            {
                objectionsL.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "EXEC [Objection_Query].dbo.Queries";


                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    objectionsL.Add(new Prop_View_Model()
                    {
                        Objection_No = dr["Objection_No"].ToString(),
                        Property_Type = dr["Property_Type"].ToString(),
                        Town_Name = dr["Town_Name"].ToString(),
                        Old_Market_Value = dr["Old_Market_Value"].ToString(),
                        Old_Category = dr["Old_Category"].ToString(),
                        Property_Desc = dr["Property_Desc"].ToString(),
                        Unit_key = dr["Unit_key"].ToString(),
                        Valuation_Key = dr["Valuation_Key"].ToString(),
                        objection_Status = dr["objection_Status"].ToString(),
                        QUERY_No = dr["QUERY_No"].ToString(),

                    });
                }
                con.Close();
                ViewBag.Linked_Obj = objectionsL.ToList();

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message.ToString());               
                throw ex;
            }

            //userName = TempData["currentUserFirstname"].ToString() +' ' + TempData["currentUserSurname"].ToString();

            if (drafts.Count > 0)
            {
                drafts.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT DATEDIFF(DAY,Start_Date, GETDATE()) AS Date_Diff,* FROM [UpdatedGVTool].[dbo].[NotValuedSection78] N " +
                                  "WHERE (AllocateName LIKE '%" + userName + "%') AND Status IN (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID IN (5, 4))" +
                                  "Order By N.Start_Date asc";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        QueryId = (int)dr["QueryId"],
                        QUERY_No = dr["QUERY_No"].ToString(),
                        PremiseId = dr["Premise ID"].ToString(),
                        PropertyDescription = dr["PROPERTY_DESC"].ToString(),
                        TownshipDescription = dr["Town Name Description"].ToString(),
                        Extent = dr["Unit Legal Area"].ToString(),
                        ValuationType = dr["Valuation Type"].ToString(),
                        ValuationTypeDescription = dr["Valuation Type Description"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        CATDescription = dr["CAT Description"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        Status = dr["Status"].ToString(),
                        Dept_Dir = dr["Dept_Dir"].ToString(),
                        Snr_Manager = dr["Snr_Manager"].ToString(),
                        Area_Manager = dr["Area_Manager"].ToString(),
                        Candidate_DC = dr["Candidate_DC"].ToString(),
                        Start_Date = (DateTime)dr["Start_Date"],
                        DateDiff = (int)dr["Date_Diff"]
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.UserDataList = drafts.ToList();
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
                com.CommandText = "EXEC ValuersList_Procedure '" + userSector + "'";
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

            return PartialView("PropPerUser", new { userName = userName });//drafts
        }

        

        [HttpPost]
        public async Task<IActionResult> UpdateRevisedValue(string? QueryId, string? PremiseId, string? PropertyDescription, string? MarketValue, string? MarketValue1, string? MarketValue2, string? MarketValue3, string? CATDescription, string? CATDescription1, string? CATDescription2, string? CATDescription3, string? Extent, string? Extent1, string? Extent2, string? Extent3, string? Comment, string? WEF_DATE, string? userName, List<IFormFile> files)
        {
            var userID = TempData["currentUser"];
            TempData.Keep("currentUser");

            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");

            var currentUserSurname = TempData["currentUserSurname"];
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"];
            TempData.Keep("currentUserFirstname");

            TempData["CATDescription"] = CATDescription;
            TempData["CATDescription1"] = CATDescription1;
            TempData["CATDescription2"] = CATDescription2;
            TempData["CATDescription3"] = CATDescription3;
            TempData["WEF_DATE"] = WEF_DATE;
            TempData["PropertyDescription"] = PropertyDescription;

            int count = 0;

            string Premise_ID = PremiseId.ToString();
            string uploadRoot = $"{_config["AppSettings:FileRooTPathSection78"]}";
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

            if (drafts.Count > 0)
            {
                drafts.Clear();
            }
            try
            { 
                con.Open();
                com.Connection = con;

                //string fileNameAttachValue = (files != null && files.FileName != null) ? Path.GetFileName(files.FileName) : null;

                com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValuedSection78] SET [Market Value] = '" + MarketValue + "', " + " [Market Value1] = '" + MarketValue1 + "'," + " [Market Value2] = '" + MarketValue2 + "', [Market Value3] = '" + MarketValue3 + "', " + " [Unit Legal Area] = '" + Extent + "', [Unit Legal Area1] = '" + Extent1 + "'," + "[Unit Legal Area2] = '" + Extent2 + "', " + "[Unit Legal Area3] = '" + Extent3 + "', " + " [CAT Description] = '" + CATDescription + "', " + " [CAT Description1] = '" + CATDescription1 + "'," + " [CAT Description2] = '" + CATDescription2 + "', " + " [CAT Description3] = '" + CATDescription3 + "', Comment = '" + Comment + "', WEF_DATE = '" + WEF_DATE + "', Activity_Date = getdate(), " +
                                 "FileNameAttach = '" + save_files + "', Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2) WHERE QueryId = '" + QueryId + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {

                    });
                }

                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (draftHistories.Count > 0)
            {
                draftHistories.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistorySection78] ([UserName],[UserID],[PropertyDescription]," +
                                  "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Comment],[UserActivity],[Status],[PremiseId],[ActivityDate], [Sector]) " +
                                  "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PropertyDescription + "', '" + MarketValue + "','" + MarketValue1 + "', '" + MarketValue2 + "', '" + MarketValue3 + "','" + CATDescription + "','" + CATDescription1 + "','" + CATDescription2 + "','" + CATDescription3 + "','" + Comment + "','Updated Values', (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2),'" + PremiseId + "', getdate(), '" + userSector + "')";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    draftHistories.Add(new DraftHistory
                    {
                        //QueryId = (Int64)dr["QueryId"],
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Status = dr["Status"].ToString(),
                    });
                }
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("PropPerUser", new { userName = userName });
        }

        public IActionResult DraftApprovalList(string? userName)
        {
            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");

            if (drafts.Count > 0)
            {
                drafts.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT DATEDIFF(DAY,Start_Date, GETDATE()) AS Date_Diff ,* FROM [UpdatedGVTool].[dbo].[NotValuedSection78] N " +
                                  "WHERE (N.Dept_Dir LIKE '%" + userName + "%' OR N.Snr_Manager LIKE '%" + userName + "%' OR N.Area_Manager LIKE '%" + userName + "%') AND N.Sector = '" + userSector + "' AND N.Status IN (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID IN(2,6)) " +
                                  "Order By N.Start_Date ASC";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        QueryId = (int)dr["QueryId"],
                        PremiseId = dr["Premise ID"].ToString(),
                        PropertyDescription = dr["PROPERTY_DESC"].ToString(),
                        TownshipDescription = dr["Town Name Description"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        MarketValue = dr["Market Value"].ToString(),
                        MarketValue1 = dr["Market Value1"].ToString(),
                        MarketValue2 = dr["Market Value2"].ToString(),
                        MarketValue3 = dr["Market Value3"].ToString(),
                        CATDescription = dr["CAT Description"].ToString(),
                        CATDescription1 = dr["CAT Description1"].ToString(),
                        CATDescription2 = dr["CAT Description2"].ToString(),
                        CATDescription3 = dr["CAT Description3"].ToString(),
                        ValuationType = dr["Valuation Type"].ToString(),
                        ValuationTypeDescription = dr["Valuation Type Description"].ToString(),
                        WEF_DATE = (DateTime)dr["WEF_DATE"],
                        Extent = dr["Unit Legal Area"].ToString(),
                        Extent1 = dr["Unit Legal Area2"].ToString(),
                        Extent2 = dr["Unit Legal Area3"].ToString(),
                        Extent3 = dr["Unit Legal Area4"].ToString(),
                        //MarketCategory = dr["MarketCategory"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        Comment = dr["Comment"].ToString(),
                        Dept_Dir = dr["Dept_Dir"].ToString(),
                        Snr_Manager = dr["Snr_Manager"].ToString(),
                        Area_Manager = dr["Area_Manager"].ToString(),
                        Candidate_DC = dr["Candidate_DC"].ToString(),
                        //BulkUpload = dr["BulkUpload"].ToString(),
                        Status = dr["Status"].ToString(),
                        AllocatedName = dr["AllocateName"].ToString(),
                        Start_Date = (DateTime)dr["Start_Date"],
                        DateDiff = (int)dr["Date_Diff"]
                        //Date = (DateTime)dr["Date"],
                    });
                }
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(drafts);
        }

        public IActionResult ViewPendingDraft(string? id)
        {
            if (drafts.Count > 0)
            {
                drafts.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[NotValuedSection78] WHERE QueryId = '" + id + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        QueryId = (int)dr["QueryId"],
                        PremiseId = dr["Premise ID"].ToString(),
                        PropertyDescription = dr["PROPERTY_DESC"].ToString(),
                        TownshipDescription = dr["Town Name Description"].ToString(),
                        MarketValue = dr["Market Value"].ToString(),
                        MarketValue1 = dr["Market Value1"].ToString(),
                        MarketValue2 = dr["Market Value2"].ToString(),
                        MarketValue3 = dr["Market Value3"].ToString(),
                        CATDescription = dr["CAT Description"].ToString(),
                        CATDescription1 = dr["CAT Description1"].ToString(),
                        CATDescription2 = dr["CAT Description2"].ToString(),
                        CATDescription3 = dr["CAT Description3"].ToString(),
                        ValuationType = dr["Valuation Type"].ToString(),
                        ValuationTypeDescription = dr["Valuation Type Description"].ToString(),
                        WEF_DATE = (DateTime)dr["WEF_DATE"],
                        Extent = dr["Unit Legal Area"].ToString(),
                        Extent1 = dr["Unit Legal Area1"].ToString(),
                        Extent2 = dr["Unit Legal Area2"].ToString(),
                        Extent3 = dr["Unit Legal Area3"].ToString(),
                        //MarketCategory = dr["MarketCategory"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        Comment = dr["Comment"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        Dept_Dir = dr["Dept_Dir"].ToString(),
                        Snr_Manager = dr["Snr_Manager"].ToString(),
                        Area_Manager = dr["Area_Manager"].ToString(),
                        Candidate_DC = dr["Candidate_DC"].ToString(),
                        //BulkUpload = dr["BulkUpload"].ToString(),
                        Status = dr["Status"].ToString(),
                        //Date = (DateTime)dr["Date"],
                    });
                }
                con.Close();


                if (drafts.Count > 0)
                {
                    foreach (var admin in drafts)
                    {
                        TempData["TownshipDescription"] = admin.TownshipDescription;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(drafts);
        }
        public IActionResult DraftApproval(string? QueryId, string? approverComment, string? approval, string PropertyDescription, string? MarketValue, string? MarketValue1, string? MarketValue2, string? MarketValue3, string? CATDescription, string? CATDescription1, string? CATDescription2, string? CATDescription3, string? Comment, string? CategoryComment, string? PremiseId)
        {
            var userID = TempData["currentUser"];
            TempData.Keep("currentUser");

            var currentUserSurname = TempData["currentUserSurname"] as string;
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"] as string;
            TempData.Keep("currentUserFirstname");

            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");

            TempData["PropertyDescription"] = PropertyDescription;

			if (string.IsNullOrWhiteSpace(currentUserSurname) || string.IsNullOrWhiteSpace(currentUserFirstname))
			{
				TempData["RefreshMessage"] = $"User Surname or Firstname is missing or blank. Please refresh the page.";
				return RedirectToAction("RefreshMessage");
			}

			if (approval == "Approved")
            {
                if (draftHistories.Count > 0)
                {
                    draftHistories.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistorySection78] ([UserName],[UserID],[PropertyDescription]," +
                                   "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Comment],[UserActivity],[Status],[approverComment],[ActivityDate], [PremiseId], [Sector]) " +
                             "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PropertyDescription.Replace("'", "''") + "', '" + MarketValue + "','" + MarketValue1 + "', '" + MarketValue2 + "','" + MarketValue3 + "', " + " '" + CATDescription + "','" + CATDescription1 + "','" + CATDescription2 + "','" + CATDescription3 + "','" + Comment + "','Approved Values', " +
                             "(SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3),'" + approverComment + "', getdate(), '" + PremiseId + "', '" + userSector + "')";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftHistories.Add(new DraftHistory
                        {
                            //QueryId = (Int64)dr["QueryId"],
                            //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                            //RevisedCategory = dr["RevisedCategory"].ToString(),
                            //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                            //CommentCategory = dr["CommentCategory"].ToString(),
                            //Status = dr["Status"].ToString(),
                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (drafts.Count > 0)
                {
                    drafts.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "UPDATE [UpdatedGVTool].dbo.[NotValuedSection78]" +
                                      "SET [Status] = (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3), [approverComment] = '" + approverComment + "', [End_Date] = GETDATE() , [ApproverName] = '" + currentUserFirstname + ' ' + currentUserSurname + "'" +
                                      "WHERE QueryId = '" + QueryId + "'";

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        drafts.Add(new Draft
                        {
                            //QueryId = (Int64)dr["QueryId"],
                            //PremiseId = dr["PremiseId"].ToString(),
                            //PropertyDescription = dr["PropertyDescription"].ToString(),
                            //TownshipDescription = dr["TownshipDescription"].ToString(),
                            //MarketValue = dr["MarketValue"].ToString(),
                            //MarketCategory = dr["MarketCategory"].ToString(),
                            //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                            //RevisedCategory = dr["RevisedCategory"].ToString(),
                            //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                            //CommentCategory = dr["CommentCategory"].ToString(),
                            //FlagForDelete = (bool)dr["FlagForDelete"],
                            //AssignedValuer = dr["AssignedValuer"].ToString(),
                            //BulkUpload = dr["BulkUpload"].ToString(),
                            //Status = dr["Status"].ToString(),
                            //Date = (DateTime)dr["Date"],

                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                TempData["DraftApprovalSuccess"] = "Task is successfully approved.";

                return RedirectToAction("DraftApprovalList");
            }

            if (approval == "Rejected")
            {
                if (draftHistories.Count > 0)
                {
                    draftHistories.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistorySection78] ([UserName],[UserID],[PropertyDescription]," +
                                     "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Comment],[UserActivity],[Status],[approverComment],[ActivityDate], [PremiseId], [Sector]) " +
                                      "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PropertyDescription.Replace("'", "''") + "', '" + MarketValue + "', '" + MarketValue1 + "','" + MarketValue2 + "','" + MarketValue3 + "'," + " '" + CATDescription + "','" + CATDescription1 + "','" + CATDescription2 + "','" + CATDescription3 + "', '" + Comment + "','Rejected Values', (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 4),'" + approverComment + "', getdate(), '" + PremiseId + "', '" + userSector + "')";

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftHistories.Add(new DraftHistory
                        {
                            //QueryId = (Int64)dr["QueryId"],
                            //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                            //RevisedCategory = dr["RevisedCategory"].ToString(),
                            //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                            //CommentCategory = dr["CommentCategory"].ToString(),
                            //Status = dr["Status"].ToString(),
                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (drafts.Count > 0)
                {
                    drafts.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValuedSection78]" +
                                        "SET [Status] = (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 4), [approverComment] = '" + approverComment + "'" +
                                        "WHERE QueryId = '" + QueryId + "'";

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        drafts.Add(new Draft
                        {
                            //QueryId = (Int64)dr["QueryId"],
                            //PremiseId = dr["PremiseId"].ToString(),
                            //PropertyDescription = dr["PropertyDescription"].ToString(),
                            //TownshipDescription = dr["TownshipDescription"].ToString(),
                            //MarketValue = dr["MarketValue"].ToString(),
                            //MarketCategory = dr["MarketCategory"].ToString(),
                            //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                            //RevisedCategory = dr["RevisedCategory"].ToString(),
                            //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                            //CommentCategory = dr["CommentCategory"].ToString(),
                            //FlagForDelete = (bool)dr["FlagForDelete"],
                            //AssignedValuer = dr["AssignedValuer"].ToString(),
                            //BulkUpload = dr["BulkUpload"].ToString(),
                            //Status = dr["Status"].ToString(),
                            //Date = (DateTime)dr["Date"],

                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                TempData["DraftApprovalSuccess"] = "Task is successfully rejected.";
                return RedirectToAction("DraftApprovalList");
            }
			if (approval == "KillTask")
			{
				if (draftHistories.Count > 0)
				{
					draftHistories.Clear();
				}
				try
				{
					con.Open();
					com.Connection = con;
					com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistorySection78] ([UserName],[UserID],[PropertyDescription]," +
									  "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Comment],[UserActivity],[Status],[approverComment],[ActivityDate], [PremiseId], [Sector]) " +
									   "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PropertyDescription.Replace("'", "''") + "', '" + MarketValue + "','" + MarketValue1 + "', '" + MarketValue2 + "','" + MarketValue3 + "', " + " '" + CATDescription + "','" + CATDescription1 + "','" + CATDescription2 + "','" + CATDescription3 + "','" + Comment + "','Killed Values', " +
									  "(SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 7),'" + approverComment + "', getdate(), '" + PremiseId + "', '" + userSector + "')";
					dr = com.ExecuteReader();
					while (dr.Read())
					{
						draftHistories.Add(new DraftHistory
						{
							//DraftId = (Int64)dr["DraftId"],
							//RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
							//RevisedCategory = dr["RevisedCategory"].ToString(),
							//CommentMarketValue = dr["CommentMarketValue"].ToString(),
							//CommentCategory = dr["CommentCategory"].ToString(),
							//Status = dr["Status"].ToString(),
						});
					}
					con.Close();

				}
				catch (Exception ex)
				{
					throw ex;
				}

				if (drafts.Count > 0)
				{
					drafts.Clear();
				}
				try
				{
					con.Open();
					com.Connection = con;
					com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValuedSection78]" +
									  "SET [Status] = (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 7), [approverComment] = '" + approverComment + "', [End_Date] = GETDATE() , [ApproverName] = '" + currentUserFirstname + ' ' + currentUserSurname + "'" +
									  "WHERE QueryId = '" + QueryId + "'";

					dr = com.ExecuteReader();
					while (dr.Read())
					{
						drafts.Add(new Draft
						{
							//DraftId = (Int64)dr["DraftId"],
							//PremiseId = dr["PremiseId"].ToString(),
							//PropertyDescription = dr["PropertyDescription"].ToString(),
							//TownshipDescription = dr["TownshipDescription"].ToString(),
							//MarketValue = dr["MarketValue"].ToString(),
							//MarketCategory = dr["MarketCategory"].ToString(),
							//RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
							//RevisedCategory = dr["RevisedCategory"].ToString(),
							//CommentMarketValue = dr["CommentMarketValue"].ToString(),
							//CommentCategory = dr["CommentCategory"].ToString(),
							//FlagForDelete = (bool)dr["FlagForDelete"],
							//AssignedValuer = dr["AssignedValuer"].ToString(),
							//BulkUpload = dr["BulkUpload"].ToString(),
							//Status = dr["Status"].ToString(),
							//Date = (DateTime)dr["Date"],

						});
					}
					con.Close();

				}
				catch (Exception ex)
				{
					throw ex;
				}

				TempData["DraftApprovalSuccess"] = "Task is successfully killed.";

				return RedirectToAction("DraftApprovalList");
			}

			return View();

        }

        public IActionResult AuditTrail()
        {
            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");

            if (draftHistories.Count > 0)
            {
                draftHistories.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [ID],[UserName],[PremiseId],[UserActivity],[UserID],[PropertyDescription],[MarketValue],[CATDescription]," +
                                  "[Comment],[approverComment],[Status],[ActivityDate] FROM [UpdatedGVTool].[dbo].[DraftHistorySection78] WHERE Sector = '" + userSector + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    draftHistories.Add(new DraftHistory
                    {
                        Id = (int)dr["ID"],
                        UserName = dr["UserName"].ToString(),
                        PremiseId = dr["PremiseId"].ToString(),
                        UserId = dr["UserID"].ToString(),
                        UserActivity = dr["UserActivity"].ToString(),
                        PropertyDescription = dr["PropertyDescription"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(), 
                        Comment = dr["Comment"].ToString(),
                        MarketValue = dr["MarketValue"].ToString(),
                        CATDescription = dr["CATDescription"].ToString(),
                        ApproverComment = dr["approverComment"].ToString(),
                        Status = dr["Status"].ToString(),
                        ActivityDate = (DateTime)dr["ActivityDate"],
                    });
                }
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(draftHistories);
        }

        public void Valuation(string? queryId, string? objectionNo)
        {
            if (drafts.Count > 0)
            {
                drafts.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [CAT_DESC_NAME] FROM [UpdatedGVTool].[dbo].[Category] WHERE ACTIVE = 'Y'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    categories.Add(new Category
                    {
                        CatDescName = dr["CAT_DESC_NAME"].ToString(),
                    });
                }
                con.Close();

                ViewBag.CategoriesList = categories.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (drafts.Count > 0)
            {
                drafts.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[NotValuedSection78] WHERE QueryId = '" + queryId + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        QueryId = (int)dr["QueryId"],
                        PremiseId = dr["Premise ID"].ToString(),
                        PropertyDescription = dr["PROPERTY_DESC"].ToString(),
                        TownshipDescription = dr["Town Name Description"].ToString(),
                        ValuationType = dr["Valuation Type"].ToString(),
                        ValuationTypeDescription = dr["Valuation Type Description"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        //MarketCategory = dr["MarketCategory"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        Comment = dr["Comment"].ToString(),
                        Extent = dr["Unit Legal Area"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        Dept_Dir = dr["Dept_Dir"].ToString(),
                        Snr_Manager = dr["Snr_Manager"].ToString(),
                        Area_Manager = dr["Area_Manager"].ToString(),
                        Candidate_DC = dr["Candidate_DC"].ToString(),
                        //BulkUpload = dr["BulkUpload"].ToString(),
                        Status = dr["Status"].ToString(),
                        //Date = (DateTime)dr["Date"],
                        ApproverComment = dr["approverComment"].ToString()

                    });
                }
                con.Close();
                ViewBag.OneValuation = drafts.ToList();

                /*foreach (var items in ViewBag.OneValuation)
                {
                    TempData["QueryId"] = @items.QueryId;
                    TempData["PremiseId"] = @items.PremiseId;
                    TempData["PropertyDescription"] = @items.PropertyDescription;
                    TempData["TownshipDescription"] = @items.TownshipDescription;
                    TempData["ValuationTypeDescription"] = @items.ValuationTypeDescription;
                    TempData["Comment"] = @items.Comment;
                    TempData["Extent"] = @items.Extent;
                    TempData["Dept_Dir"] = @items.Dept_Dir;
                    TempData["Snr_Manager"] = @items.Snr_Manager;
                    TempData["Area_Manager"] = @items.Area_Manager;
                    TempData["Candidate_DC"] = @items.Candidate_DC;
                    TempData["Status"] = @items.Status;
                }*/
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //Links
            if (links.Count > 0)
            {
                links.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "EXEC Objection_Query.dbo.GetObjectionFiles @ObjectionRefFiles = '" + objectionNo + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    links.Add(new Link
                    {

                        Files1 = dr["Files1"].ToString(),
                        Files2 = dr["Files2"].ToString(),
                        Files3 = dr["Files3"].ToString(),
                        Files4 = dr["Files4"].ToString(),
                        Files5 = dr["Files5"].ToString(),
                        Files6 = dr["Files6"].ToString(),
                        Files7 = dr["Files7"].ToString(),
                        Files8 = dr["Files8"].ToString(),
                        Files9 = dr["Files9"].ToString(),
                        Files10 = dr["Files10"].ToString(),
                        RepLetter = dr["Rep_letter"].ToString(),

                        Files1_path = dr["Files1_path"].ToString(),
                        Files2_path = dr["Files2_path"].ToString(),
                        Files3_path = dr["Files3_path"].ToString(),
                        Files4_path = dr["Files4_path"].ToString(),
                        Files5_path = dr["Files5_path"].ToString(),
                        Files6_path = dr["Files6_path"].ToString(),
                        Files7_path = dr["Files7_path"].ToString(),
                        Files8_path = dr["Files8_path"].ToString(),
                        Files9_path = dr["Files9_path"].ToString(),
                        Files10_path = dr["Files10_path"].ToString(),
                        Acknowledgement_path = dr["Acknowledgement_path"].ToString(),
                        Acknowledgement = dr["Acknowledgement"].ToString(),
                        Evidence_count = (double)dr["Evidence_count"],
                        RepLetter_Path = dr["RepLetter_Path"].ToString(),
                    });
                }
                con.Close();
                ViewBag.links = links.ToList();
                /*foreach (var items in ViewBag.OneValuation)
                {
                    TempData["QueryId"] = @items.QueryId;
                    TempData["PremiseId"] = @items.PremiseId;
                    TempData["PropertyDescription"] = @items.PropertyDescription;
                    TempData["TownshipDescription"] = @items.TownshipDescription;
                    TempData["ValuationTypeDescription"] = @items.ValuationTypeDescription;
                    TempData["Comment"] = @items.Comment;
                    TempData["Extent"] = @items.Extent;
                    TempData["Dept_Dir"] = @items.Dept_Dir;
                    TempData["Snr_Manager"] = @items.Snr_Manager;
                    TempData["Area_Manager"] = @items.Area_Manager;
                    TempData["Candidate_DC"] = @items.Candidate_DC;
                    TempData["Status"] = @items.Status;
                }*/
            }
            
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public IActionResult FormData(string? propertyID, string? objectionNo, string? queryId, string AppealStatus)
        {
            var User = this.User.Identity.Name;
            TempData["currentUser"] = User;
            if (adminValuers.Count > 0)
            {
                adminValuers.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [Username_Old],[Username],[Role], [Sector], [Email_Address], [Surname], [First_name], [Phone_No] FROM [UpdatedGVTool].[dbo].[Admin-Valuer] " +
                                  "where Username = '" + User + "' or Username_Old = '" + User + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    adminValuers.Add(new AdminValuer
                    {
                        Username = dr["Username"].ToString(),
                        Role = dr["Role"].ToString(),
                        Sector = dr["Sector"].ToString(),
                        EmailAddress = dr["Email_Address"].ToString(),
                        Surname = dr["Surname"].ToString(),
                        FirstName = dr["First_name"].ToString(),
                        PhoneNo = dr["Phone_No"].ToString(),
                    });
                }
                con.Close();
                if (adminValuers.Count > 0)
                {
                    foreach (var admin in adminValuers)
                    {
                        TempData["currentRole"] = admin.Role;
                        TempData["currentUserRole"] = "Admin";
                        TempData["currentUserSector"] = admin.Sector;
                        TempData["currentUserEmail"] = admin.EmailAddress;
                        TempData["currentUserSurname"] = admin.Surname;
                        TempData["currentUserFirstname"] = admin.FirstName;
                        TempData["currentUserPhoneNo"] = admin.PhoneNo;
                    }
                }
                //else
                //{
                //    return PartialView("_Error403");
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (propertyID == "Res" || propertyID == "Res_omission")
            {
                if (queryId != null)
                {
                    Valuation(queryId, objectionNo);
                }

                //--------------------------------------------------------------------------------
                if (ObjectionTBs.Count > 0)
                {
                    ObjectionTBs.Clear();
                }
                try
                {
                    conQ.Open();
                    com.Connection = conQ;
                    //Section 1
                    com.CommandText = "EXEC [Objection_Query].[dbo].FormViewResNotValued @Objection_no = '" + objectionNo + "'";
                    if (AppealStatus == "True")
                    {
                        com.CommandText = "EXEC [Objection_Query].[dbo].AppealFormViewRes @Objection_no = '" + objectionNo + "'";
                    }
                    else
                    {
                        com.CommandText = "EXEC [Objection_Query].[dbo].FormViewResNotValued @Objection_no = '" + objectionNo + "'";
                        
                    }
                    // WHERE [Sector] = '"+ userSector + "' OR [Sector] IS NULL
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        ObjectionTBs.Add(new ObjectionTB
                        {
                            QUERY_Type = dr["QUERY_Type"].ToString(),

                            //Section 1
                            QUERY_No = dr["QUERY_No"].ToString(),
                            OwnerName = dr["Owner_Name"].ToString(),
                            OwnerCompany = dr["Owner_Company"].ToString(),
                            OwnerIdentity = dr["Owner_Identity"].ToString(),
                            OwnerAddress1 = dr["Owner_Address_1"].ToString(),
                            OwnerAddress2 = dr["Owner_Address_2"].ToString(),
                            OwnerAddress3 = dr["Owner_Address_3"].ToString(),
                            OwnerAddress4 = dr["Owner_Address_4"].ToString(),
                            OwnerAddress5 = dr["Owner_Address_5"].ToString(),
                            OwnerPostal1 = dr["Owner_Postal_1"].ToString(),
                            OwnerPostal2 = dr["Owner_Postal_2"].ToString(),
                            OwnerPostal3 = dr["Owner_Postal_3"].ToString(),
                            OwnerPostal4 = dr["Owner_Postal_4"].ToString(),
                            OwnerPostal5 = dr["Owner_Postal_5"].ToString(),
                            OwnerHomePhone = dr["Owner_Home_Phone"].ToString(),
                            OwnerCellPhone = dr["Owner_Cell_Phone"].ToString(),
                            OwnerWorkPhone = dr["Owner_Work_Phone"].ToString(),
                            OwnerFaxPhone = dr["Owner_Fax_Phone"].ToString(),
                            OwnerEmail = dr["Owner_Email"].ToString(),
                            ObjectorName = dr["Objector_Name"].ToString(),
                            ObjectorIdentity = dr["Objector_Identity"].ToString(),
                            ObjectorCompany = dr["Objector_Company"].ToString(),
                            ObjectorPostal1 = dr["Objector_Postal_1"].ToString(),
                            ObjectorPostal2 = dr["Objector_Postal_2"].ToString(),
                            ObjectorPostal3 = dr["Objector_Postal_3"].ToString(),
                            ObjectorPostal4 = dr["Objector_Postal_4"].ToString(),
                            ObjectorPostal5 = dr["Objector_Postal_5"].ToString(),
                            ObjectorHome = dr["Objector_Home"].ToString(),
                            ObjectorCell = dr["Objector_Cell"].ToString(),
                            ObjectorWork = dr["Objector_Work"].ToString(),
                            ObjectorFax = dr["Objector_Fax"].ToString(),
                            ObjectorEmail = dr["Objector_Email"].ToString(),
                            ObjectorStatus = dr["Objector_Status"].ToString(),
                            RepresentativeName = dr["Representative_name"].ToString(),
                            RepPostal1 = dr["Rep_Postal_1"].ToString(),
                            RepPostal2 = dr["Rep_Postal_2"].ToString(),
                            RepPostal3 = dr["Rep_Postal_3"].ToString(),
                            RepPostal4 = dr["Rep_Postal_4"].ToString(),
                            RepPostal5 = dr["Rep_Postal_5"].ToString(),
                            RepHomePhone = dr["Rep_Home_Phone"].ToString(),
                            RepCellPhone = dr["Rep_Cell_Phone"].ToString(),
                            RepWorkPhone = dr["Rep_Work_Phone"].ToString(),
                            RepFaxPhone = dr["Rep_Fax_Phone"].ToString(),
                            RepEmail = dr["Rep_Email"].ToString(),

                            //Section 2
                            PhysicalAddress = dr["physical_address"].ToString(),
                            TownName = dr["Town_Name"].ToString(),
                            Code = dr["Code"].ToString(),
                            Extent = dr["Extent"].ToString(),
                            MunicipalAccountNo = dr["Municipal_Account_No"].ToString(),
                            BondHolderName = dr["BondHolder_Name"].ToString(),
                            RegisteredAmount = dr["Registered_Amount"].ToString(),
                            FullDetails = dr["Full_Details"].ToString(),
                            ServitudeNo = dr["Servitude_No"].ToString(),
                            AffectedArea = dr["Affected_Area"].ToString(),
                            PropertyFavourOf = dr["Property_Favour_Of"].ToString(),
                            PropertyPurpose = dr["Property_Purpose"].ToString(),
                            CompensationPaid = dr["Compensation_Paid"].ToString(),
                            PaymentDate = dr["Payment_Date"].ToString(),
                            CompensationAmount = dr["Compensation_Amount"].ToString(),

                            //Section 3

                            ResNoOfBedroom = dr["Res_No_of_Bedroom"].ToString(),
                            ResNoOfBathRoom = dr["Res_No_of_BathRoom"].ToString(),
                            ResKitchen = dr["Res_Kitchen"].ToString(),
                            ResLounge = dr["Res_Lounge"].ToString(),
                            ResDinningRoom = dr["Res_Dinning_Room"].ToString(),
                            ResLoungeDiningRoom = dr["Res_Lounge_Dining_Room"].ToString(),
                            ResStudy = dr["Res_Study"].ToString(),
                            ResPlayRoom = dr["Res_Play_Room"].ToString(),
                            ResTelevision = dr["Res_Television"].ToString(),
                            ResLaundry = dr["Res_Laundry"].ToString(),
                            ResSeperateToilet = dr["Res_Seperate_Toilet"].ToString(),
                            ResDwellOther1 = dr["Res_Dwell_Other1"].ToString(),
                            ResDwellOther2 = dr["Res_Dwell_Other2"].ToString(),
                            ResDwellOther3 = dr["Res_Dwell_Other3"].ToString(),
                            ResDwellOther4 = dr["Res_Dwell_Other4"].ToString(),
                            ResNoOfGarages = dr["Res_No_of_Garages"].ToString(),
                            ResGrannyRoom = dr["Res_Granny_Room"].ToString(),
                            ResOutbuildOther = dr["Res_Outbuild_Other"].ToString(),
                            ResMainDwellingSize = dr["Res_Main_Dwelling_Size"].ToString(),
                            ResOutsideBuildingSize = dr["Res_Outside_Building_Size"].ToString(),
                            ResOtherBuildingSize = dr["Res_Other_Building_Size"].ToString(),
                            ResTotalBuildingSize = dr["Res_Total_Building_Size"].ToString(),
                            ResSwimmingPool = dr["Res_Swimming_Pool"].ToString(),
                            ResBoreHole = dr["Res_Bore_Hole"].ToString(),
                            ResTennisCourt = dr["Res_Tennis_Court"].ToString(),
                            ResGarden = dr["Res_Garden"].ToString(),
                            ResOtherDwell1 = dr["Res_other_dwell1"].ToString(),
                            ResOtherDwell2 = dr["Res_other_dwell2"].ToString(),
                            ResFence = dr["Res_Fence"].ToString(),
                            ResFenceFront = dr["Res_Fence_Front"].ToString(),
                            ResFenceBack = dr["Res_Fence_Back"].ToString(),
                            ResFenceSide1 = dr["Res_Fence_Side_1"].ToString(),
                            ResFenceSide2 = dr["Res_Fence_Side_2"].ToString(),
                            ResFenceHeightFront = dr["Res_Fence_Height_Front"].ToString(),
                            ResFenceHeightBack = dr["Res_Fence_Height_Back"].ToString(),
                            ResFenceHeightSide1 = dr["Res_Fence_Height_Side1"].ToString(),
                            ResFenceHeightSide2 = dr["Res_Fence_Height_Side2"].ToString(),
                            ResDriveWay = dr["Res_Drive_Way"].ToString(),
                            ResSecurityBoomedArea = dr["Res_Security_Boomed_Area"].ToString(),
                            ResOtherFeatures = dr["Res_Other_features"].ToString(),
                            ResOtherFeaturesCondition = dr["Res_Other_features_Condition"].ToString(),
                            ResGeneralCondition = dr["Res_General_Condition"].ToString(),

                            //Section 4
                            Res4SchemeName = dr["Res4_Scheme_Name"].ToString(),
                            Res4SchemeNo = dr["Res4_Scheme_No"].ToString(),
                            Res4FlatNo = dr["Res4_Flat_No"].ToString(),
                            Res4UnitSize = dr["Res4_Unit_Size"].ToString(),
                            Res4ManagingAgentName = dr["Res4_Managing_Agent_Name"].ToString(),
                            Res4ManagingAgentTelNo = dr["Res4_Managing_Agent_Tel_No"].ToString(),
                            Res4NoOfBedroom = dr["Res4_No_of_Bedroom"].ToString(),
                            Res4NoOfBathRoom = dr["Res4_No_of_BathRoom"].ToString(),
                            Res4MonthlyLevyRes = dr["Res4_Monthly_Levy_Res"].ToString(),
                            Res4Kitchen = dr["Res4_Kitchen"].ToString(),
                            Res4Lounge = dr["Res4_Lounge"].ToString(),
                            Res4DinningRoom = dr["Res4_Dinning_Room"].ToString(),
                            Res4LoungeDiningRoom = dr["Res4_Lounge_Dining_Room"].ToString(),
                            Res4Study = dr["Res4_Study"].ToString(),
                            Res4PlayRoom = dr["Res4_Play_Room"].ToString(),
                            Res4Television = dr["Res4_Television"].ToString(),
                            Res4Laundry = dr["Res4_Laundry"].ToString(),
                            Res4SeperateToilet = dr["Res4_Seperate_Toilet"].ToString(),
                            Res4DwellOther1 = dr["Res4_Dwell_Other1"].ToString(),
                            Res4DwellOther2 = dr["Res4_Dwell_Other2"].ToString(),
                            Res4DwellOther3 = dr["Res4_Dwell_Other3"].ToString(),
                            Res4DwellOther4 = dr["Res4_Dwell_Other4"].ToString(),
                            Res4CommonPropertyOther1 = dr["Res4_Common_Property_Other_1"].ToString(),
                            Res4CommonPropertyOther2 = dr["Res4_Common_Property_Other_2"].ToString(),
                            Res4CommonPropertyOther3 = dr["Res4_Common_Property_Other_3"].ToString(),
                            Res4PoolSize = dr["Res4_Pool_Size"].ToString(),
                            Res4TennisCourtSize = dr["Res4_Tennis_Court_Size"].ToString(),
                            Res4GarageSize = dr["Res4_Garage_Size"].ToString(),
                            Res4CarportSize = dr["Res4_Carport_Size"].ToString(),
                            Res4OpenParkingSize = dr["Res4_Open_Parking_Size"].ToString(),
                            Res4StoreRoomSize = dr["Res4_Store_Room_Size"].ToString(),
                            Res4GardenSize = dr["Res4_Garden_Size"].ToString(),
                            Res4ExclusiveOther = dr["Res4_Exclusive_Other"].ToString(),

                            //Section 5

                            CurrentAskingPrice = dr["Current_Asking_price"].ToString(),
                            PreviousAskingPrice = dr["Previous_Asking_price"].ToString(),
                            AgentName = dr["Agent_Name"].ToString(),
                            UnitNo = dr["Unit_No"].ToString(),
                            OtherNearbySales = dr["Other_Nearby_Sales"].ToString(),
                            SaleDate = dr["Sale_Date"].ToString(),
                            CurrentRecievedOffer = dr["Current_Recieved_Offer"].ToString(),
                            PreviousRecievedOffer = dr["Previous_Recieved_Offer"].ToString(),
                            AgentTelNo = dr["Agent_Tel_No"].ToString(),
                            SuburbName = dr["Suburb_Name"].ToString(),
                            SellingPrice = dr["Selling_Price"].ToString(),


                            //Section 6

                            OldPropertyDescription = dr["Old_Property_Description"].ToString(),
                            OldCategory = dr["Old_Category"].ToString(),
                            OldAddress = dr["Old_Address"].ToString(),
                            OldExtent = dr["Old_Extent"].ToString(),
                            OldMarketValue = dr["Old_Market_Value"].ToString(),
                            OldOwner = dr["Old_Owner"].ToString(),
                            NewPropertyDescription = dr["New_Property_Description"].ToString(),
                            NewCategory = dr["New_Category"].ToString(),
                            NewAddress = dr["New_Address"].ToString(),
                            NewExtent = dr["New_Extent"].ToString(),
                            NewMarketValue = dr["New_Market_Value"].ToString(),
                            NewOwner = dr["New_Owner"].ToString(),
                            ObjectionReasons = dr["Objection_Reasons"].ToString(),
                            Old2Category = dr["Old2_Category"].ToString(),
                            Old2Extent = dr["Old2_Extent"].ToString(),
                            Old2MarketValue = dr["Old2_Market_Value"].ToString(),
                            New2Category = dr["New2_Category"].ToString(),
                            New2Extent = dr["New2_Extent"].ToString(),
                            New2MarketValue = dr["New2_Market_Value"].ToString(),
                            Old3Category = dr["Old3_Category"].ToString(),
                            Old3Extent = dr["Old3_Extent"].ToString(),
                            Old3MarketValue = dr["Old3_Market_Value"].ToString(),
                            New3Category = dr["New3_Category"].ToString(),
                            New3Extent = dr["New3_Extent"].ToString(),
                            New3MarketValue = dr["New3_Market_Value"].ToString(),

                            //Section 7
                            SignatureName = dr["Signature_Name"].ToString(),
                            SignaturePicture = dr["Signature_Picture"].ToString()
                        });
                    }
                    conQ.Close();
                    //TempData["QUERY_No"] = objectionNo;
                    ViewBag.OneResObjection = ObjectionTBs.ToList();

                    foreach (var items in ViewBag.OneResObjection)
                    {

                        TempData["QUERY_Type"] = @items.QUERY_Type;

                        //Section 1
                        TempData["QUERY_No"] = @items.QUERY_No;
                        TempData["OwnerName"] = @items.OwnerName;
                        TempData["OwnerIdentity"] = @items.OwnerIdentity;
                        TempData["OwnerCompany"] = @items.OwnerCompany;
                        TempData["OwnerAddress1"] = @items.OwnerAddress1;
                        TempData["OwnerAddress2"] = @items.OwnerAddress2;
                        TempData["OwnerAddress3"] = @items.OwnerAddress3;
                        TempData["OwnerAddress4"] = @items.OwnerAddress4;
                        TempData["OwnerAddress5"] = @items.OwnerAddress5;
                        TempData["OwnerPostal1"] = @items.OwnerPostal1;
                        TempData["OwnerPostal2"] = @items.OwnerPostal2;
                        TempData["OwnerPostal3"] = @items.OwnerPostal3;
                        TempData["OwnerPostal4"] = @items.OwnerPostal4;
                        TempData["OwnerPostal5"] = @items.OwnerPostal5;
                        TempData["OwnerHomePhone"] = @items.OwnerHomePhone;
                        TempData["OwnerCellPhone"] = @items.OwnerCellPhone;
                        TempData["OwnerWorkPhone"] = @items.OwnerWorkPhone;
                        TempData["OwnerFaxPhone"] = @items.OwnerFaxPhone;
                        TempData["OwnerEmail"] = @items.OwnerEmail;
                        TempData["ObjectorName"] = @items.ObjectorName;
                        TempData["ObjectorIdentity"] = @items.ObjectorIdentity;
                        TempData["ObjectorCompany"] = @items.ObjectorCompany;
                        TempData["ObjectorPostal1"] = @items.ObjectorPostal1;
                        TempData["ObjectorPostal2"] = @items.ObjectorPostal2;
                        TempData["QUERY_No"] = @items.ObjectorPostal3;
                        TempData["ObjectorPostal3"] = @items.ObjectorPostal4;
                        TempData["ObjectorPostal5"] = @items.ObjectorPostal5;
                        TempData["ObjectorHome"] = @items.ObjectorHome;
                        TempData["ObjectorCell"] = @items.ObjectorCell;
                        TempData["ObjectorWork"] = @items.ObjectorWork;
                        TempData["ObjectorFax"] = @items.ObjectorFax;
                        TempData["ObjectorEmail"] = @items.ObjectorEmail;
                        TempData["ObjectorStatus"] = @items.ObjectorStatus;
                        TempData["RepresentativeName"] = @items.RepresentativeName;
                        TempData["RepPostal1"] = @items.RepPostal1;
                        TempData["RepPostal2"] = @items.RepPostal2;
                        TempData["RepPostal3"] = @items.RepPostal3;
                        TempData["RepPostal4"] = @items.RepPostal4;
                        TempData["RepPostal5"] = @items.RepPostal5;
                        TempData["RepHomePhone"] = @items.RepHomePhone;
                        TempData["RepCellPhone"] = @items.RepCellPhone;
                        TempData["RepWorkPhone"] = @items.RepWorkPhone;
                        TempData["RepFaxPhone"] = @items.RepFaxPhone;
                        TempData["RepEmail"] = @items.RepEmail;

                        //Section 2
                        TempData["PhysicalAddress"] = @items.PhysicalAddress;
                        TempData["TownName"] = @items.TownName;
                        TempData["Code"] = @items.Code;
                        TempData["Extent"] = @items.Extent;
                        TempData["MunicipalAccountNo"] = @items.MunicipalAccountNo;
                        TempData["BondHolderName"] = @items.BondHolderName;
                        TempData["RegisteredAmount"] = @items.RegisteredAmount;
                        TempData["FullDetails"] = @items.FullDetails;
                        TempData["ServitudeNo"] = @items.ServitudeNo;
                        TempData["AffectedArea"] = @items.AffectedArea;
                        TempData["PropertyFavourOf"] = @items.PropertyFavourOf;
                        TempData["PropertyPurpose"] = @items.PropertyPurpose;
                        TempData["CompensationPaid"] = @items.CompensationPaid;
                        TempData["PaymentDate"] = @items.PaymentDate;
                        TempData["CompensationAmount"] = @items.CompensationAmount;

                        //Section 3
                        TempData["ResNoOfBedroom"] = @items.ResNoOfBedroom;
                        TempData["ResNoOfBathRoom"] = @items.ResNoOfBathRoom;
                        TempData["ResKitchen"] = @items.ResKitchen;
                        TempData["ResLounge"] = @items.ResLounge;
                        TempData["ResDinningRoom"] = @items.ResDinningRoom;
                        TempData["ResLoungeDiningRoom"] = @items.ResLoungeDiningRoom;
                        TempData["ResStudy"] = @items.ResStudy;
                        TempData["ResPlayRoom"] = @items.ResPlayRoom;
                        TempData["ResTelevision"] = @items.ResTelevision;
                        TempData["ResLaundry"] = @items.ResLaundry;
                        TempData["ResSeperateToilet"] = @items.ResSeperateToilet;
                        TempData["ResDwellOther1"] = @items.ResDwellOther1;
                        TempData["ResDwellOther2"] = @items.ResDwellOther2;
                        TempData["ResDwellOther3"] = @items.ResDwellOther3;
                        TempData["ResDwellOther4"] = @items.ResDwellOther4;
                        TempData["ResNoOfGarages"] = @items.ResNoOfGarages;
                        TempData["ResGrannyRoom"] = @items.ResGrannyRoom;
                        TempData["ResOutbuildOther"] = @items.ResOutbuildOther;
                        TempData["ResMainDwellingSize"] = @items.ResMainDwellingSize;
                        TempData["ResOutsideBuildingSize"] = @items.ResOutsideBuildingSize;
                        TempData["ResOtherBuildingSize"] = @items.ResOtherBuildingSize;
                        TempData["ResTotalBuildingSize"] = @items.ResTotalBuildingSize;
                        TempData["ResSwimmingPool"] = @items.ResSwimmingPool;
                        TempData["ResBoreHole"] = @items.ResBoreHole;
                        TempData["ResTennisCourt"] = @items.ResTennisCourt;
                        TempData["ResGarden"] = @items.ResGarden;
                        TempData["ResOtherDwell1"] = @items.ResOtherDwell1;
                        TempData["ResOtherDwell2"] = @items.ResOtherDwell2;
                        TempData["ResFence"] = @items.ResFence;
                        TempData["ResFenceFront"] = @items.ResFenceFront;
                        TempData["ResFenceBack"] = @items.ResFenceBack;
                        TempData["ResFenceSide1"] = @items.ResFenceSide1;
                        TempData["ResFenceSide2"] = @items.ResFenceSide2;
                        TempData["ResFenceHeightFront"] = @items.ResFenceHeightFront;
                        TempData["ResFenceHeightBack"] = @items.ResFenceHeightBack;
                        TempData["ResFenceHeightSide1"] = @items.ResFenceHeightSide1;
                        TempData["ResFenceHeightSide2"] = @items.ResFenceHeightSide2;
                        TempData["ResDriveWay"] = @items.ResDriveWay;
                        TempData["ResSecurityBoomedArea"] = @items.ResSecurityBoomedArea;
                        TempData["ResOtherFeatures"] = @items.ResOtherFeatures;
                        TempData["ResOtherFeaturesCondition"] = @items.ResOtherFeaturesCondition;
                        TempData["ResGeneralCondition"] = @items.ResGeneralCondition;

                        //Section 4
                        TempData["Res4SchemeName"] = @items.Res4SchemeName;
                        TempData["Res4SchemeNo"] = @items.Res4SchemeNo;
                        TempData["Res4FlatNo"] = @items.Res4FlatNo;
                        TempData["Res4UnitSize"] = @items.Res4UnitSize;
                        TempData["Res4ManagingAgentName"] = @items.Res4ManagingAgentName;
                        TempData["Res4ManagingAgentTelNo"] = @items.Res4ManagingAgentTelNo;
                        TempData["Res4NoOfBedroom"] = @items.Res4NoOfBedroom;
                        TempData["Res4NoOfBathRoom"] = @items.Res4NoOfBathRoom;
                        TempData["Res4MonthlyLevyRes"] = @items.Res4MonthlyLevyRes;
                        TempData["Res4Kitchen"] = @items.Res4Kitchen;
                        TempData["Res4Lounge"] = @items.Res4Lounge;
                        TempData["Res4DinningRoom"] = @items.Res4DinningRoom;
                        TempData["Res4LoungeDiningRoom"] = @items.Res4LoungeDiningRoom;
                        TempData["Res4Study"] = @items.Res4Study;
                        TempData["Res4PlayRoom"] = @items.Res4PlayRoom;
                        TempData["Res4Television"] = @items.Res4Television;
                        TempData["Res4Laundry"] = @items.Res4Laundry;
                        TempData["Res4SeperateToilet"] = @items.Res4SeperateToilet;
                        TempData["Res4DwellOther1"] = @items.Res4DwellOther1;
                        TempData["Res4DwellOther2"] = @items.Res4DwellOther2;
                        TempData["Res4DwellOther3"] = @items.Res4DwellOther3;
                        TempData["Res4DwellOther4"] = @items.Res4DwellOther4;
                        TempData["Res4CommonPropertyOther1"] = @items.Res4CommonPropertyOther1;
                        TempData["Res4CommonPropertyOther2"] = @items.Res4CommonPropertyOther2;
                        TempData["Res4CommonPropertyOther3"] = @items.Res4CommonPropertyOther3;
                        TempData["Res4TennisCourtSize"] = @items.Res4TennisCourtSize;
                        TempData["Res4GarageSize"] = @items.Res4GarageSize;
                        TempData["Res4CarportSize"] = @items.Res4CarportSize;
                        TempData["Res4OpenParkingSize"] = @items.Res4OpenParkingSize;
                        TempData["Res4StoreRoomSize"] = @items.Res4StoreRoomSize;
                        TempData["Res4GardenSize"] = @items.Res4GardenSize;
                        TempData["Res4ExclusiveOther"] = @items.Res4ExclusiveOther;
                        TempData["Res4ExclusiveOther"] = @items.Res4ExclusiveOther;
                        TempData["Res4PoolSize"] = @items.Res4PoolSize;

                        //Section 5
                        TempData["CurrentAskingPrice"] = @items.CurrentAskingPrice;
                        TempData["PreviousAskingPrice"] = @items.PreviousAskingPrice;
                        TempData["AgentName"] = @items.AgentName;
                        TempData["UnitNo"] = @items.UnitNo;
                        TempData["OtherNearbySales"] = @items.OtherNearbySales;
                        TempData["SaleDate"] = @items.SaleDate;
                        TempData["CurrentRecievedOffer"] = @items.CurrentRecievedOffer;
                        TempData["PreviousRecievedOffer"] = @items.PreviousRecievedOffer;
                        TempData["AgentTelNo"] = @items.AgentTelNo;
                        TempData["SuburbName"] = @items.SuburbName;
                        TempData["SellingPrice"] = @items.SellingPrice;

                        //Section 6

                        TempData["OldPropertyDescription"] = @items.OldPropertyDescription;
                        TempData["OldCategory"] = @items.OldCategory;
                        TempData["OldAddress"] = @items.OldAddress;
                        TempData["OldExtent"] = @items.OldExtent;
                        TempData["OldMarketValue"] = @items.OldMarketValue;
                        TempData["OldOwner"] = @items.OldOwner;
                        TempData["NewPropertyDescription"] = @items.NewPropertyDescription;
                        TempData["NewCategory"] = @items.NewCategory;
                        TempData["NewAddress"] = @items.NewAddress;
                        TempData["NewExtent"] = @items.NewExtent;
                        TempData["NewMarketValue"] = @items.NewMarketValue;
                        TempData["NewOwner"] = @items.NewOwner;
                        TempData["ObjectionReasons"] = @items.ObjectionReasons;
                        TempData["Old2Category"] = @items.Old2Category;
                        TempData["Old2Extent"] = @items.Old2Extent;
                        TempData["Old2MarketValue"] = @items.Old2MarketValue;
                        TempData["New2Category"] = @items.New2Category;
                        TempData["New2Extent"] = @items.New2Extent;
                        TempData["New2MarketValue"] = @items.New2MarketValue;
                        TempData["Old3Category"] = @items.Old3Category;
                        TempData["Old3Extent"] = @items.Old3Extent;
                        TempData["Old3MarketValue"] = @items.Old3MarketValue;
                        TempData["New3Category"] = @items.New3Category;
                        TempData["New3Extent"] = @items.New3Extent;
                        TempData["New3MarketValue"] = @items.New3MarketValue;

                        //Section 7
                        TempData["SignatureName"] = @items.SignatureName;
                        TempData["SignaturePicture"] = @items.SignaturePicture;
                      
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message.ToString());               
                    throw ex;
                }

                return PartialView("ResForm", ObjectionTBs);
            }

            if (propertyID == "Agric" || propertyID == "Agric_omission")
            {
                if (queryId != null)
                {
                    Valuation(queryId, objectionNo);
                }

                if (ObjectionTBs.Count > 0)
                {
                    ObjectionTBs.Clear();
                }
                try
                {
                    conQ.Open();
                    com.Connection = conQ;
                    //Section 1
                    com.CommandText = "EXEC [Objection_Query].dbo.FormViewAgriNotValued @Objection_no '" + objectionNo + "'";
                    if (AppealStatus == "True")
                    {
                        com.CommandText = "EXEC [Objection_Query].dbo.AppealFormViewAgri @Objection_no = '" + objectionNo + "'";
                    }
                    else
                    {
                        com.CommandText = "EXEC [Objection_Query].dbo.FormViewAgriNotValued @Objection_no = '" + objectionNo + "'";
                    }
                    // WHERE [Sector] = '"+ userSector + "' OR [Sector] IS NULL
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        ObjectionTBs.Add(new ObjectionTB
                        {
                            QUERY_Type = dr["QUERY_Type"].ToString(),

                            //Section 1
                            QUERY_No = dr["QUERY_No"].ToString(),
                            OwnerName = dr["Owner_Name"].ToString(),
                            OwnerCompany = dr["Owner_Company"].ToString(),
                            OwnerIdentity = dr["Owner_Identity"].ToString(),
                            OwnerAddress1 = dr["Owner_Address_1"].ToString(),
                            OwnerAddress2 = dr["Owner_Address_2"].ToString(),
                            OwnerAddress3 = dr["Owner_Address_3"].ToString(),
                            OwnerAddress4 = dr["Owner_Address_4"].ToString(),
                            OwnerAddress5 = dr["Owner_Address_5"].ToString(),
                            OwnerPostal1 = dr["Owner_Postal_1"].ToString(),
                            OwnerPostal2 = dr["Owner_Postal_2"].ToString(),
                            OwnerPostal3 = dr["Owner_Postal_3"].ToString(),
                            OwnerPostal4 = dr["Owner_Postal_4"].ToString(),
                            OwnerPostal5 = dr["Owner_Postal_5"].ToString(),
                            OwnerHomePhone = dr["Owner_Home_Phone"].ToString(),
                            OwnerCellPhone = dr["Owner_Cell_Phone"].ToString(),
                            OwnerWorkPhone = dr["Owner_Work_Phone"].ToString(),
                            OwnerFaxPhone = dr["Owner_Fax_Phone"].ToString(),
                            OwnerEmail = dr["Owner_Email"].ToString(),
                            ObjectorName = dr["Objector_Name"].ToString(),
                            ObjectorIdentity = dr["Objector_Identity"].ToString(),
                            ObjectorCompany = dr["Objector_Company"].ToString(),
                            ObjectorPostal1 = dr["Objector_Postal_1"].ToString(),
                            ObjectorPostal2 = dr["Objector_Postal_2"].ToString(),
                            ObjectorPostal3 = dr["Objector_Postal_3"].ToString(),
                            ObjectorPostal4 = dr["Objector_Postal_4"].ToString(),
                            ObjectorPostal5 = dr["Objector_Postal_5"].ToString(),
                            ObjectorHome = dr["Objector_Home"].ToString(),
                            ObjectorCell = dr["Objector_Cell"].ToString(),
                            ObjectorWork = dr["Objector_Work"].ToString(),
                            ObjectorFax = dr["Objector_Fax"].ToString(),
                            ObjectorEmail = dr["Objector_Email"].ToString(),
                            ObjectorStatus = dr["Objector_Status"].ToString(),
                            RepresentativeName = dr["Representative_name"].ToString(),
                            RepPostal1 = dr["Rep_Postal_1"].ToString(),
                            RepPostal2 = dr["Rep_Postal_2"].ToString(),
                            RepPostal3 = dr["Rep_Postal_3"].ToString(),
                            RepPostal4 = dr["Rep_Postal_4"].ToString(),
                            RepPostal5 = dr["Rep_Postal_5"].ToString(),
                            RepHomePhone = dr["Rep_Home_Phone"].ToString(),
                            RepCellPhone = dr["Rep_Cell_Phone"].ToString(),
                            RepWorkPhone = dr["Rep_Work_Phone"].ToString(),
                            RepFaxPhone = dr["Rep_Fax_Phone"].ToString(),
                            RepEmail = dr["Rep_Email"].ToString(),

                            //Section 2
                            PhysicalAddress = dr["physical_address"].ToString(),
                            TownName = dr["Town_Name"].ToString(),
                            Code = dr["Code"].ToString(),
                            Extent = dr["Extent"].ToString(),
                            MunicipalAccountNo = dr["Municipal_Account_No"].ToString(),
                            BondHolderName = dr["BondHolder_Name"].ToString(),
                            RegisteredAmount = dr["Registered_Amount"].ToString(),
                            FullDetails = dr["Full_Details"].ToString(),
                            ServitudeNo = dr["Servitude_No"].ToString(),
                            AffectedArea = dr["Affected_Area"].ToString(),
                            PropertyFavourOf = dr["Property_Favour_Of"].ToString(),
                            PropertyPurpose = dr["Property_Purpose"].ToString(),
                            CompensationPaid = dr["Compensation_Paid"].ToString(),
                            PaymentDate = dr["Payment_Date"].ToString(),
                            CompensationAmount = dr["Compensation_Amount"].ToString(),

                            //Section 3
                            AgriNoOfBedroom = dr["Agri_No_of_Bedroom"].ToString(),
                            AgriNoOfBathRoom = dr["Agri_No_of_BathRoom"].ToString(),
                            AgriKitchen = dr["Agri_Kitchen"].ToString(),
                            AgriLounge = dr["Agri_Lounge"].ToString(),
                            AgriDinningRoom = dr["Agri_Dinning_Room"].ToString(),
                            AgriLoungeDiningRoom = dr["Agri_Lounge_Dining_Room"].ToString(),
                            AgriStudy = dr["Agri_Study"].ToString(),
                            AgriPlayRoom = dr["Agri_Play_Room"].ToString(),
                            AgriTelevision = dr["Agri_Television"].ToString(),
                            AgriLaundry = dr["Agri_Laundry"].ToString(),
                            AgriSeperateToilet = dr["Agri_Seperate_Toilet"].ToString(),
                            AgriDwellOther1 = dr["Agri_Dwell_Other1"].ToString(),
                            AgriMainDwellingSize = dr["Agri_Main_Dwelling_Size"].ToString(),
                            AgriBuildingNo = dr["Agri_Building_No"].ToString(),
                            AgriBuildingDescription = dr["Agri_Building_Description"].ToString(),
                            AgriBuildingSize = dr["Agri_Building_Size"].ToString(),
                            AgriBuildingCondition = dr["Agri_Building_Condition"].ToString(),
                            AgriBuildingFunctional = dr["Agri_Building_Functional"].ToString(),
                            AgriAnotherPurposeNotAgriculture = dr["Agri_Another_Purpose_Not_Agriculture"].ToString(),
                            AgriAnotherPurposeNotAgricultureDesc = dr["Agri_Another_Purpose_Not_Agriculture_Desc"].ToString(),
                            AgriNonAgricultural = dr["Agri_Non_Agricultural"].ToString(),
                            AgriGrazing = dr["Agri_Grazing"].ToString(),
                            AgriUnderIrrigation = dr["Agri_Under_Irrigation"].ToString(),
                            AgriDryLand = dr["Agri_Dry_Land"].ToString(),
                            AgriPermanentCrop = dr["Agri_Permanent_Crop"].ToString(),
                            AgriOtherHa1 = dr["Agri_Other_ha_1"].ToString(),
                            AgriOtherHa2 = dr["Agri_Other_ha_2"].ToString(),
                            AgriOtherHa3 = dr["Agri_Other_ha_3"].ToString(),
                            AgriTotalHa = dr["Agri_Total_ha"].ToString(),
                            AgriFenceCondition = dr["Agri_Fence_Condition"].ToString(),
                            AgriGameAreaFenced = dr["Agri_Game_Area_Fenced"].ToString(),
                            AgriNumOfBoreholes = dr["Agri_Num_of_Boreholes"].ToString(),
                            AgriOutputLitresHours = dr["Agri_Output_litres_Hours"].ToString(),
                            AgriDams = dr["Agri_Dams"].ToString(),
                            AgriCapacity = dr["Agri_Capacity"].ToString(),
                            AgriExposedToRiver = dr["Agri_Exposed_To_River"].ToString(),
                            AgriLandClaim = dr["Agri_Land_Claim"].ToString(),
                            AgriClaimDate = dr["Agri_Claim_Date"].ToString(),
                            AgriGazetteNo = dr["Agri_Gazette_No"].ToString(),
                            AgriWaterRights = dr["Agri_Water_Rights"].ToString(),
                            AgriWaterRightsDetails = dr["Agri_Water_Rights_Details"].ToString(),
                            AgriRezoningConsentUse = dr["Agri_Rezoning_Consent_Use"].ToString(),
                            AgriConsentUseDetails = dr["Agri_Consent_Use_Details"].ToString(),
                            AgriLandExcised = dr["Agri_Land_Excised"].ToString(),
                            AgriNewFarmDesc = dr["Agri_New_Farm_Desc"].ToString(),
                            AgriTownshipApplied = dr["Agri_Township_Applied"].ToString(),
                            AgriTownshipAppliedDetail = dr["Agri_Township_Applied_Detail"].ToString(),
                            AgriTenantName = dr["Agri_Tenant_Name"].ToString(),
                            AgriRentalLandSize = dr["Agri_Rental_Land_Size"].ToString(),
                            AgriRental = dr["Agri_Rental"].ToString(),
                            AgriEscalation = dr["Agri_Escalation"].ToString(),
                            AgriOtherContribution = dr["Agri_Other_contribution"].ToString(),
                            AgriLeaseTerm = dr["Agri_Lease_Term"].ToString(),
                            AgriStartDate = dr["Agri_Start_Date"].ToString(),
                            AgriUse = dr["Agri_Use"].ToString(),

                            //Section 5

                            CurrentAskingPrice = dr["Current_Asking_price"].ToString(),
                            PreviousAskingPrice = dr["Previous_Asking_price"].ToString(),
                            AgentName = dr["Agent_Name"].ToString(),
                            UnitNo = dr["Unit_No"].ToString(),
                            OtherNearbySales = dr["Other_Nearby_Sales"].ToString(),
                            SaleDate = dr["Sale_Date"].ToString(),
                            CurrentRecievedOffer = dr["Current_Recieved_Offer"].ToString(),
                            PreviousRecievedOffer = dr["Previous_Recieved_Offer"].ToString(),
                            AgentTelNo = dr["Agent_Tel_No"].ToString(),
                            SuburbName = dr["Suburb_Name"].ToString(),
                            SellingPrice = dr["Selling_Price"].ToString(),


                            //Section 6

                            OldPropertyDescription = dr["Old_Property_Description"].ToString(),
                            OldCategory = dr["Old_Category"].ToString(),
                            OldAddress = dr["Old_Address"].ToString(),
                            OldExtent = dr["Old_Extent"].ToString(),
                            OldMarketValue = dr["Old_Market_Value"].ToString(),
                            OldOwner = dr["Old_Owner"].ToString(),
                            NewPropertyDescription = dr["New_Property_Description"].ToString(),
                            NewCategory = dr["New_Category"].ToString(),
                            NewAddress = dr["New_Address"].ToString(),
                            NewExtent = dr["New_Extent"].ToString(),
                            NewMarketValue = dr["New_Market_Value"].ToString(),
                            NewOwner = dr["New_Owner"].ToString(),
                            ObjectionReasons = dr["Objection_Reasons"].ToString(),
                            Old2Category = dr["Old2_Category"].ToString(),
                            Old2Extent = dr["Old2_Extent"].ToString(),
                            Old2MarketValue = dr["Old2_Market_Value"].ToString(),
                            New2Category = dr["New2_Category"].ToString(),
                            New2Extent = dr["New2_Extent"].ToString(),
                            New2MarketValue = dr["New2_Market_Value"].ToString(),
                            Old3Category = dr["Old3_Category"].ToString(),
                            Old3Extent = dr["Old3_Extent"].ToString(),
                            Old3MarketValue = dr["Old3_Market_Value"].ToString(),
                            New3Category = dr["New3_Category"].ToString(),
                            New3Extent = dr["New3_Extent"].ToString(),
                            New3MarketValue = dr["New3_Market_Value"].ToString(),

                            //Section 7
                            SignatureName = dr["Signature_Name"].ToString(),
                            SignaturePicture = dr["Signature_Picture"].ToString()

                        });
                    }
                    conQ.Close();
                    //TempData["QUERY_No"] = objectionNo;
                    ViewBag.OneAgricObjection = ObjectionTBs.ToList();

                    foreach (var items in ViewBag.OneAgricObjection)
                    {

                        TempData["QUERY_Type"] = @items.QUERY_Type;

                        //Section 1
                        TempData["QUERY_No"] = @items.QUERY_No;
                        TempData["OwnerName"] = @items.OwnerName;
                        TempData["OwnerIdentity"] = @items.OwnerIdentity;
                        TempData["OwnerCompany"] = @items.OwnerCompany;
                        TempData["OwnerAddress1"] = @items.OwnerAddress1;
                        TempData["OwnerAddress2"] = @items.OwnerAddress2;
                        TempData["OwnerAddress3"] = @items.OwnerAddress3;
                        TempData["OwnerAddress4"] = @items.OwnerAddress4;
                        TempData["OwnerAddress5"] = @items.OwnerAddress5;
                        TempData["OwnerPostal1"] = @items.OwnerPostal1;
                        TempData["OwnerPostal2"] = @items.OwnerPostal2;
                        TempData["OwnerPostal3"] = @items.OwnerPostal3;
                        TempData["OwnerPostal4"] = @items.OwnerPostal4;
                        TempData["OwnerPostal5"] = @items.OwnerPostal5;
                        TempData["OwnerHomePhone"] = @items.OwnerHomePhone;
                        TempData["OwnerCellPhone"] = @items.OwnerCellPhone;
                        TempData["OwnerWorkPhone"] = @items.OwnerWorkPhone;
                        TempData["OwnerFaxPhone"] = @items.OwnerFaxPhone;
                        TempData["OwnerEmail"] = @items.OwnerEmail;
                        TempData["ObjectorName"] = @items.ObjectorName;
                        TempData["ObjectorIdentity"] = @items.ObjectorIdentity;
                        TempData["ObjectorCompany"] = @items.ObjectorCompany;
                        TempData["ObjectorPostal1"] = @items.ObjectorPostal1;
                        TempData["ObjectorPostal2"] = @items.ObjectorPostal2;
                        TempData["QUERY_No"] = @items.ObjectorPostal3;
                        TempData["ObjectorPostal3"] = @items.ObjectorPostal4;
                        TempData["ObjectorPostal5"] = @items.ObjectorPostal5;
                        TempData["ObjectorHome"] = @items.ObjectorHome;
                        TempData["ObjectorCell"] = @items.ObjectorCell;
                        TempData["ObjectorWork"] = @items.ObjectorWork;
                        TempData["ObjectorFax"] = @items.ObjectorFax;
                        TempData["ObjectorEmail"] = @items.ObjectorEmail;
                        TempData["ObjectorStatus"] = @items.ObjectorStatus;
                        TempData["RepresentativeName"] = @items.RepresentativeName;
                        TempData["RepPostal1"] = @items.RepPostal1;
                        TempData["RepPostal2"] = @items.RepPostal2;
                        TempData["RepPostal3"] = @items.RepPostal3;
                        TempData["RepPostal4"] = @items.RepPostal4;
                        TempData["RepPostal5"] = @items.RepPostal5;
                        TempData["RepHomePhone"] = @items.RepHomePhone;
                        TempData["RepCellPhone"] = @items.RepCellPhone;
                        TempData["RepWorkPhone"] = @items.RepWorkPhone;
                        TempData["RepFaxPhone"] = @items.RepFaxPhone;
                        TempData["RepEmail"] = @items.RepEmail;

                        //Section 2
                        TempData["PhysicalAddress"] = @items.PhysicalAddress;
                        TempData["TownName"] = @items.TownName;
                        TempData["Code"] = @items.Code;
                        TempData["Extent"] = @items.Extent;
                        TempData["MunicipalAccountNo"] = @items.MunicipalAccountNo;
                        TempData["BondHolderName"] = @items.BondHolderName;
                        TempData["RegisteredAmount"] = @items.RegisteredAmount;
                        TempData["FullDetails"] = @items.FullDetails;
                        TempData["ServitudeNo"] = @items.ServitudeNo;
                        TempData["AffectedArea"] = @items.AffectedArea;
                        TempData["PropertyFavourOf"] = @items.PropertyFavourOf;
                        TempData["PropertyPurpose"] = @items.PropertyPurpose;
                        TempData["CompensationPaid"] = @items.CompensationPaid;
                        TempData["PaymentDate"] = @items.PaymentDate;
                        TempData["CompensationAmount"] = @items.CompensationAmount;

                        //Section 3
                        TempData["AgriNoOfBedroom"] = @items.AgriNoOfBedroom;
                        TempData["Agri_No_of_BathRoom"] = @items.AgriNoOfBathRoom;
                        TempData["Agri_Kitchen"] = @items.AgriKitchen;
                        TempData["AgriLounge"] = @items.AgriLounge;
                        TempData["AgriDinningRoom"] = @items.AgriDinningRoom;
                        TempData["AgriLoungeDiningRoom"] = @items.AgriLoungeDiningRoom;
                        TempData["AgriStudy"] = @items.AgriStudy;
                        TempData["AgriPlayRoom"] = @items.AgriPlayRoom;
                        TempData["AgriTelevision"] = @items.AgriTelevision;
                        TempData["AgriLaundry"] = @items.AgriLaundry;
                        TempData["AgriSeperateToilet"] = @items.AgriSeperateToilet;
                        TempData["AgriDwellOther1"] = @items.AgriDwellOther1;
                        TempData["AgriMainDwellingSize"] = @items.AgriMainDwellingSize;
                        TempData["AgriBuildingNo"] = @items.AgriBuildingNo;
                        TempData["AgriBuildingDescription"] = @items.AgriBuildingDescription;
                        TempData["AgriBuildingSize"] = @items.AgriBuildingSize;
                        TempData["AgriBuildingCondition"] = @items.AgriBuildingCondition;
                        TempData["AgriBuildingFunctional"] = @items.AgriBuildingFunctional;
                        TempData["AgriAnotherPurposeNotAgriculture"] = @items.AgriAnotherPurposeNotAgriculture;
                        TempData["AgriAnotherPurposeNotAgricultureDesc"] = @items.AgriAnotherPurposeNotAgricultureDesc;
                        TempData["AgriNonAgricultural"] = @items.AgriNonAgricultural;
                        TempData["AgriGrazing"] = @items.AgriGrazing;
                        TempData["AgriUnderIrrigation"] = @items.AgriUnderIrrigation;
                        TempData["AgriDryLand"] = @items.AgriDryLand;
                        TempData["AgriPermanentCrop"] = @items.AgriPermanentCrop;
                        TempData["AgriOtherHa1"] = @items.AgriOtherHa1;
                        TempData["AgriOtherHa2"] = @items.AgriOtherHa2;
                        TempData["AgriOtherHa3"] = @items.AgriOtherHa3;
                        TempData["AgriTotalHa"] = @items.AgriTotalHa;
                        TempData["AgriFenceCondition"] = @items.AgriFenceCondition;
                        TempData["AgriGameAreaFenced"] = @items.AgriGameAreaFenced;
                        TempData["AgriNumOfBoreholes"] = @items.AgriNumOfBoreholes;
                        TempData["AgriOutputLitresHours"] = @items.AgriOutputLitresHours;
                        TempData["AgriDams"] = @items.AgriDams;
                        TempData["AgriCapacity"] = @items.AgriCapacity;
                        TempData["AgriExposedToRiver"] = @items.AgriExposedToRiver;
                        TempData["AgriLandClaim"] = @items.AgriLandClaim;
                        TempData["AgriClaimDate"] = @items.AgriClaimDate;
                        TempData["AgriGazetteNo"] = @items.AgriGazetteNo;
                        TempData["AgriWaterRights"] = @items.AgriWaterRights;
                        TempData["AgriWaterRightsDetails"] = @items.AgriWaterRightsDetails;
                        TempData["AgriRezoningConsentUse"] = @items.AgriRezoningConsentUse;
                        TempData["AgriConsentUseDetails"] = @items.AgriConsentUseDetails;
                        TempData["AgriLandExcised"] = @items.PhysicalAddress;
                        TempData["AgriNewFarmDesc"] = @items.AgriNewFarmDesc;
                        TempData["AgriTownshipApplied"] = @items.AgriTownshipApplied;
                        TempData["AgriTownshipAppliedDetail"] = @items.AgriTownshipAppliedDetail;
                        TempData["AgriTenantName"] = @items.AgriTenantName;
                        TempData["AgriRentalLandSize"] = @items.AgriRentalLandSize;
                        TempData["AgriRental"] = @items.AgriRental;
                        TempData["AgriEscalation"] = @items.AgriEscalation;
                        TempData["AgriOtherContribution"] = @items.AgriOtherContribution;
                        TempData["AgriLeaseTerm"] = @items.AgriLeaseTerm;
                        TempData["AgriStartDate"] = @items.AgriStartDate;
                        TempData["AgriUse"] = @items.AgriUse;

                        //Section 5
                        TempData["CurrentAskingPrice"] = @items.CurrentAskingPrice;
                        TempData["PreviousAskingPrice"] = @items.PreviousAskingPrice;
                        TempData["AgentName"] = @items.AgentName;
                        TempData["UnitNo"] = @items.UnitNo;
                        TempData["OtherNearbySales"] = @items.OtherNearbySales;
                        TempData["SaleDate"] = @items.SaleDate;
                        TempData["CurrentRecievedOffer"] = @items.CurrentRecievedOffer;
                        TempData["PreviousRecievedOffer"] = @items.PreviousRecievedOffer;
                        TempData["AgentTelNo"] = @items.AgentTelNo;
                        TempData["SuburbName"] = @items.SuburbName;
                        TempData["SellingPrice"] = @items.SellingPrice;

                        //Section 6

                        TempData["OldPropertyDescription"] = @items.OldPropertyDescription;
                        TempData["OldCategory"] = @items.OldCategory;
                        TempData["OldAddress"] = @items.OldAddress;
                        TempData["OldExtent"] = @items.OldExtent;
                        TempData["OldMarketValue"] = @items.OldMarketValue;
                        TempData["OldOwner"] = @items.OldOwner;
                        TempData["NewPropertyDescription"] = @items.NewPropertyDescription;
                        TempData["NewCategory"] = @items.NewCategory;
                        TempData["NewAddress"] = @items.NewAddress;
                        TempData["NewExtent"] = @items.NewExtent;
                        TempData["NewMarketValue"] = @items.NewMarketValue;
                        TempData["NewOwner"] = @items.NewOwner;
                        TempData["ObjectionReasons"] = @items.ObjectionReasons;
                        TempData["Old2Category"] = @items.Old2Category;
                        TempData["Old2Extent"] = @items.Old2Extent;
                        TempData["Old2MarketValue"] = @items.Old2MarketValue;
                        TempData["New2Category"] = @items.New2Category;
                        TempData["New2Extent"] = @items.New2Extent;
                        TempData["New2MarketValue"] = @items.New2MarketValue;
                        TempData["Old3Category"] = @items.Old3Category;
                        TempData["Old3Extent"] = @items.Old3Extent;
                        TempData["Old3MarketValue"] = @items.Old3MarketValue;
                        TempData["New3Category"] = @items.New3Category;
                        TempData["New3Extent"] = @items.New3Extent;
                        TempData["New3MarketValue"] = @items.New3MarketValue;

                        //Section 7
                        TempData["SignatureName"] = @items.SignatureName;
                        TempData["SignaturePicture"] = @items.SignaturePicture;
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message.ToString());               
                    throw ex;
                }

                return PartialView("AgricForm", ObjectionTBs);

            }

            if (propertyID == "Bus" || propertyID == "Bus_omission")
            {
                if (queryId != null)
                {
                    Valuation(queryId, objectionNo);
                }

                if (ObjectionTBs.Count > 0)
                {
                    ObjectionTBs.Clear();
                }
                try
                {
                    conQ.Open();
                    com.Connection = conQ;
                    //Section 1
                    com.CommandText = "EXEC [Objection_Query].dbo.FormViewBusNotValued @Objection_no = '" + objectionNo + "'";
                    if (AppealStatus == "True")
                    {
                        com.CommandText = "EXEC [Objection_Query].dbo.AppealFormViewBus @Objection_no = '" + objectionNo + "'";
                    }
                    else
                    {
                        com.CommandText = "EXEC [Objection_Query].dbo.FormViewBusNotValued @Objection_no = '" + objectionNo + "'";
                    }
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        ObjectionTBs.Add(new ObjectionTB
                        {
                            QUERY_Type = dr["QUERY_Type"].ToString(),

                            //Section 1
                            QUERY_No = dr["QUERY_No"].ToString(),
                            OwnerName = dr["Owner_Name"].ToString(),
                            OwnerCompany = dr["Owner_Company"].ToString(),
                            OwnerIdentity = dr["Owner_Identity"].ToString(),
                            OwnerAddress1 = dr["Owner_Address_1"].ToString(),
                            OwnerAddress2 = dr["Owner_Address_2"].ToString(),
                            OwnerAddress3 = dr["Owner_Address_3"].ToString(),
                            OwnerAddress4 = dr["Owner_Address_4"].ToString(),
                            OwnerAddress5 = dr["Owner_Address_5"].ToString(),
                            OwnerPostal1 = dr["Owner_Postal_1"].ToString(),
                            OwnerPostal2 = dr["Owner_Postal_2"].ToString(),
                            OwnerPostal3 = dr["Owner_Postal_3"].ToString(),
                            OwnerPostal4 = dr["Owner_Postal_4"].ToString(),
                            OwnerPostal5 = dr["Owner_Postal_5"].ToString(),
                            OwnerHomePhone = dr["Owner_Home_Phone"].ToString(),
                            OwnerCellPhone = dr["Owner_Cell_Phone"].ToString(),
                            OwnerWorkPhone = dr["Owner_Work_Phone"].ToString(),
                            OwnerFaxPhone = dr["Owner_Fax_Phone"].ToString(),
                            OwnerEmail = dr["Owner_Email"].ToString(),
                            ObjectorName = dr["Objector_Name"].ToString(),
                            ObjectorIdentity = dr["Objector_Identity"].ToString(),
                            ObjectorCompany = dr["Objector_Company"].ToString(),
                            ObjectorPostal1 = dr["Objector_Postal_1"].ToString(),
                            ObjectorPostal2 = dr["Objector_Postal_2"].ToString(),
                            ObjectorPostal3 = dr["Objector_Postal_3"].ToString(),
                            ObjectorPostal4 = dr["Objector_Postal_4"].ToString(),
                            ObjectorPostal5 = dr["Objector_Postal_5"].ToString(),
                            ObjectorHome = dr["Objector_Home"].ToString(),
                            ObjectorCell = dr["Objector_Cell"].ToString(),
                            ObjectorWork = dr["Objector_Work"].ToString(),
                            ObjectorFax = dr["Objector_Fax"].ToString(),
                            ObjectorEmail = dr["Objector_Email"].ToString(),
                            ObjectorStatus = dr["Objector_Status"].ToString(),
                            RepresentativeName = dr["Representative_name"].ToString(),
                            RepPostal1 = dr["Rep_Postal_1"].ToString(),
                            RepPostal2 = dr["Rep_Postal_2"].ToString(),
                            RepPostal3 = dr["Rep_Postal_3"].ToString(),
                            RepPostal4 = dr["Rep_Postal_4"].ToString(),
                            RepPostal5 = dr["Rep_Postal_5"].ToString(),
                            RepHomePhone = dr["Rep_Home_Phone"].ToString(),
                            RepCellPhone = dr["Rep_Cell_Phone"].ToString(),
                            RepWorkPhone = dr["Rep_Work_Phone"].ToString(),
                            RepFaxPhone = dr["Rep_Fax_Phone"].ToString(),
                            RepEmail = dr["Rep_Email"].ToString(),

                            //Section 2
                            PhysicalAddress = dr["physical_address"].ToString(),
                            TownName = dr["Town_Name"].ToString(),
                            Code = dr["Code"].ToString(),
                            Extent = dr["Extent"].ToString(),
                            MunicipalAccountNo = dr["Municipal_Account_No"].ToString(),
                            BondHolderName = dr["BondHolder_Name"].ToString(),
                            RegisteredAmount = dr["Registered_Amount"].ToString(),
                            FullDetails = dr["Full_Details"].ToString(),
                            ServitudeNo = dr["Servitude_No"].ToString(),
                            AffectedArea = dr["Affected_Area"].ToString(),
                            PropertyFavourOf = dr["Property_Favour_Of"].ToString(),
                            PropertyPurpose = dr["Property_Purpose"].ToString(),
                            CompensationPaid = dr["Compensation_Paid"].ToString(),
                            PaymentDate = dr["Payment_Date"].ToString(),
                            CompensationAmount = dr["Compensation_Amount"].ToString(),

                            //Section 3
                            BusTenantName = dr["Bus_Tenant_Name"].ToString(),
                            BusRentalLandSize = dr["Bus_Rental_Land_Size"].ToString(),
                            BusRental = dr["Bus_Rental"].ToString(),
                            BusEscalation = dr["Bus_Escalation"].ToString(),
                            BusOtherContribution = dr["Bus_Other_contribution"].ToString(),
                            BusLeaseTerm = dr["Bus_Lease_Term"].ToString(),
                            BusStartDate = dr["Bus_Start_Date"].ToString(),
                            BusBuildingNo = dr["Bus_Building_No"].ToString(),
                            BusBuildingSize = dr["Bus_Building_Size"].ToString(),
                            BusShops = dr["Bus_Shops"].ToString(),
                            BusBuildingCondition = dr["Bus_Building_Condition"].ToString(),
                            BusExtentLandFurtherDev = dr["Bus_Extent_Land_further_Dev"].ToString(),
                            BusOtherFeaturesCondition = dr["Bus_Other_features_Condition"].ToString(),

                            //Section 4
                            Bus4SchemeName = dr["Bus4_Scheme_Name"].ToString(),
                            Bus4SchemeNo = dr["Bus4_Scheme_No"].ToString(),
                            Bus4FlatNo = dr["Bus4_Flat_No"].ToString(),
                            Bus4UnitSize = dr["Bus4_Unit_Size"].ToString(),
                            Bus4ManagingAgentName = dr["Bus4_Managing_Agent_Name"].ToString(),
                            Bus4ManagingAgentTelNo = dr["Bus4_Managing_Agent_Tel_No"].ToString(),
                            Bus4Shops = dr["Bus4_Shops"].ToString(),
                            Bus4Offices = dr["Bus4_Offices"].ToString(),
                            Bus4Factories = dr["Bus4_Factories"].ToString(),
                            Bus4BusSectTitleOther1Name = dr["Bus4_Bus_Sect_Title_Other1_name"].ToString(),
                            Bus4BusSectTitleOther2Name = dr["Bus4_Bus_Sect_Title_Other2_name"].ToString(),
                            Bus4BusSectTitleOther3Name = dr["Bus4_Bus_Sect_Title_Other3_name"].ToString(),
                            Bus4BusSectTitleOther1 = dr["Bus4_Bus_Sect_Title_Other1"].ToString(),
                            Bus4BusSectTitleOther2 = dr["Bus4_Bus_Sect_Title_Other2"].ToString(),
                            Bus4BusSectTitleOther3 = dr["Bus4_Bus_Sect_Title_Other3"].ToString(),
                            Bus4TenantName = dr["Bus4_Tenant_Name"].ToString(),
                            Bus4Rental = dr["Bus4_Rental"].ToString(),
                            Bus4OtherContribution = dr["Bus4_Other_contribution"].ToString(),
                            Bus4MonthlyLevy = dr["Bus4_Monthly_Levy"].ToString(),
                            Bus4RentalLandSize = dr["Bus4_Rental_Land_Size"].ToString(),
                            Bus4Escalation = dr["Bus4_Escalation"].ToString(),
                            Bus4LeaseTerm = dr["Bus4_Lease_Term"].ToString(),
                            Bus4StartDate = dr["Bus4_Start_Date"].ToString(),
                            Bus4PoolSize = dr["Bus4_Pool_Size"].ToString(),
                            Bus4TennisCourtSize = dr["Bus4_Tennis_Court_Size"].ToString(),
                            Bus4CommonPropertyOther1 = dr["Bus4_Common_Property_Other_1"].ToString(),
                            Bus4CommonPropertyOther2 = dr["Bus4_Common_Property_Other_2"].ToString(),
                            Bus4CommonPropertyOther3 = dr["Bus4_Common_Property_Other_3"].ToString(),
                            Bus4GarageSize = dr["Bus4_Garage_Size"].ToString(),
                            Bus4CarportSize = dr["Bus4_Carport_Size"].ToString(),
                            Bus4OpenParkingSize = dr["Bus4_Open_Parking_Size"].ToString(),
                            Bus4StoreRoomSize = dr["Bus4_Store_Room_Size"].ToString(),
                            Bus4GardenSize = dr["Bus4_Garden_Size"].ToString(),
                            Bus4ExclusiveOther = dr["Bus4_Exclusive_Other"].ToString(),


                            //Section 5
                            CurrentAskingPrice = dr["Current_Asking_price"].ToString(),
                            PreviousAskingPrice = dr["Previous_Asking_price"].ToString(),
                            AgentName = dr["Agent_Name"].ToString(),
                            UnitNo = dr["Unit_No"].ToString(),
                            OtherNearbySales = dr["Other_Nearby_Sales"].ToString(),
                            SaleDate = dr["Sale_Date"].ToString(),
                            CurrentRecievedOffer = dr["Current_Recieved_Offer"].ToString(),
                            PreviousRecievedOffer = dr["Previous_Recieved_Offer"].ToString(),
                            AgentTelNo = dr["Agent_Tel_No"].ToString(),
                            SuburbName = dr["Suburb_Name"].ToString(),
                            SellingPrice = dr["Selling_Price"].ToString(),


                            //Section 6

                            OldPropertyDescription = dr["Old_Property_Description"].ToString(),
                            OldCategory = dr["Old_Category"].ToString(),
                            OldAddress = dr["Old_Address"].ToString(),
                            OldExtent = dr["Old_Extent"].ToString(),
                            OldMarketValue = dr["Old_Market_Value"].ToString(),
                            OldOwner = dr["Old_Owner"].ToString(),
                            NewPropertyDescription = dr["New_Property_Description"].ToString(),
                            NewCategory = dr["New_Category"].ToString(),
                            NewAddress = dr["New_Address"].ToString(),
                            NewExtent = dr["New_Extent"].ToString(),
                            NewMarketValue = dr["New_Market_Value"].ToString(),
                            NewOwner = dr["New_Owner"].ToString(),
                            ObjectionReasons = dr["Objection_Reasons"].ToString(),
                            Old2Category = dr["Old2_Category"].ToString(),
                            Old2Extent = dr["Old2_Extent"].ToString(),
                            Old2MarketValue = dr["Old2_Market_Value"].ToString(),
                            New2Category = dr["New2_Category"].ToString(),
                            New2Extent = dr["New2_Extent"].ToString(),
                            New2MarketValue = dr["New2_Market_Value"].ToString(),
                            Old3Category = dr["Old3_Category"].ToString(),
                            Old3Extent = dr["Old3_Extent"].ToString(),
                            Old3MarketValue = dr["Old3_Market_Value"].ToString(),
                            New3Category = dr["New3_Category"].ToString(),
                            New3Extent = dr["New3_Extent"].ToString(),
                            New3MarketValue = dr["New3_Market_Value"].ToString(),

                            //Section 7
                            SignatureName = dr["Signature_Name"].ToString(),
                            SignaturePicture = dr["Signature_Picture"].ToString()

                        });
                    }
                    conQ.Close();
                    //TempData["QUERY_No"] = objectionNo;
                    ViewBag.OneAgricObjection = ObjectionTBs.ToList();

                    foreach (var items in ViewBag.OneAgricObjection)
                    {


                        TempData["QUERY_Type"] = @items.QUERY_Type;

                        //Section 1
                        TempData["QUERY_No"] = @items.ObjectionNo;
                        TempData["OwnerName"] = @items.OwnerName;
                        TempData["OwnerIdentity"] = @items.OwnerIdentity;
                        TempData["OwnerCompany"] = @items.OwnerCompany;
                        TempData["OwnerAddress1"] = @items.OwnerAddress1;
                        TempData["OwnerAddress2"] = @items.OwnerAddress2;
                        TempData["OwnerAddress3"] = @items.OwnerAddress3;
                        TempData["OwnerAddress4"] = @items.OwnerAddress4;
                        TempData["OwnerAddress5"] = @items.OwnerAddress5;
                        TempData["OwnerPostal1"] = @items.OwnerPostal1;
                        TempData["OwnerPostal2"] = @items.OwnerPostal2;
                        TempData["OwnerPostal3"] = @items.OwnerPostal3;
                        TempData["OwnerPostal4"] = @items.OwnerPostal4;
                        TempData["OwnerPostal5"] = @items.OwnerPostal5;
                        TempData["OwnerHomePhone"] = @items.OwnerHomePhone;
                        TempData["OwnerCellPhone"] = @items.OwnerCellPhone;
                        TempData["OwnerWorkPhone"] = @items.OwnerWorkPhone;
                        TempData["OwnerFaxPhone"] = @items.OwnerFaxPhone;
                        TempData["OwnerEmail"] = @items.OwnerEmail;
                        TempData["ObjectorName"] = @items.ObjectorName;
                        TempData["ObjectorIdentity"] = @items.ObjectorIdentity;
                        TempData["ObjectorCompany"] = @items.ObjectorCompany;
                        TempData["ObjectorPostal1"] = @items.ObjectorPostal1;
                        TempData["ObjectorPostal2"] = @items.ObjectorPostal2;
                        TempData["QUERY_No"] = @items.ObjectorPostal3;
                        TempData["ObjectorPostal3"] = @items.ObjectorPostal4;
                        TempData["ObjectorPostal5"] = @items.ObjectorPostal5;
                        TempData["ObjectorHome"] = @items.ObjectorHome;
                        TempData["ObjectorCell"] = @items.ObjectorCell;
                        TempData["ObjectorWork"] = @items.ObjectorWork;
                        TempData["ObjectorFax"] = @items.ObjectorFax;
                        TempData["ObjectorEmail"] = @items.ObjectorEmail;
                        TempData["ObjectorStatus"] = @items.ObjectorStatus;
                        TempData["RepresentativeName"] = @items.RepresentativeName;
                        TempData["RepPostal1"] = @items.RepPostal1;
                        TempData["RepPostal2"] = @items.RepPostal2;
                        TempData["RepPostal3"] = @items.RepPostal3;
                        TempData["RepPostal4"] = @items.RepPostal4;
                        TempData["RepPostal5"] = @items.RepPostal5;
                        TempData["RepHomePhone"] = @items.RepHomePhone;
                        TempData["RepCellPhone"] = @items.RepCellPhone;
                        TempData["RepWorkPhone"] = @items.RepWorkPhone;
                        TempData["RepFaxPhone"] = @items.RepFaxPhone;
                        TempData["RepEmail"] = @items.RepEmail;

                        //Section 2
                        TempData["PhysicalAddress"] = @items.PhysicalAddress;
                        TempData["TownName"] = @items.TownName;
                        TempData["Code"] = @items.Code;
                        TempData["Extent"] = @items.Extent;
                        TempData["MunicipalAccountNo"] = @items.MunicipalAccountNo;
                        TempData["BondHolderName"] = @items.BondHolderName;
                        TempData["RegisteredAmount"] = @items.RegisteredAmount;
                        TempData["FullDetails"] = @items.FullDetails;
                        TempData["ServitudeNo"] = @items.ServitudeNo;
                        TempData["AffectedArea"] = @items.AffectedArea;
                        TempData["PropertyFavourOf"] = @items.PropertyFavourOf;
                        TempData["PropertyPurpose"] = @items.PropertyPurpose;
                        TempData["CompensationPaid"] = @items.CompensationPaid;
                        TempData["PaymentDate"] = @items.PaymentDate;
                        TempData["CompensationAmount"] = @items.CompensationAmount;

                        //Section 3
                        TempData["BusTenantName"] = @items.BusTenantName;
                        TempData["BusRentalLandSize"] = @items.BusRentalLandSize;
                        TempData["BusRental"] = @items.BusRental;
                        TempData["BusEscalation"] = @items.BusEscalation;
                        TempData["BusOtherContribution"] = @items.BusOtherContribution;
                        TempData["BusLeaseTerm"] = @items.BusLeaseTerm;
                        TempData["BusStartDate"] = @items.BusStartDate;
                        TempData["BusBuildingNo"] = @items.BusBuildingNo;
                        TempData["BusBuildingSize"] = @items.BusBuildingSize;
                        TempData["BusShops"] = @items.BusShops;
                        TempData["BusBuildingCondition"] = @items.BusBuildingCondition;
                        TempData["BusExtentLandFurtherDev"] = @items.BusExtentLandFurtherDev;
                        TempData["BusOtherFeaturesCondition"] = @items.BusOtherFeaturesCondition;

                        //Section 4
                        TempData["Bus4SchemeName"] = @items.Bus4SchemeName;
                        TempData["Bus4SchemeNo"] = @items.Bus4SchemeNo;
                        TempData["Bus4FlatNo"] = @items.Bus4FlatNo;
                        TempData["Bus4UnitSize"] = @items.Bus4UnitSize;
                        TempData["Bus4ManagingAgentName"] = @items.Bus4ManagingAgentName;
                        TempData["Bus4ManagingAgentTelNo"] = @items.Bus4ManagingAgentTelNo;
                        TempData["Bus4Shops"] = @items.Bus4Shops;
                        TempData["Bus4Offices"] = @items.Bus4Offices;
                        TempData["Bus4Factories"] = @items.Bus4Factories;
                        TempData["Bus4BusSectTitleOther1Name"] = @items.Bus4BusSectTitleOther1Name;
                        TempData["Bus4BusSectTitleOther2Name"] = @items.Bus4BusSectTitleOther2Name;
                        TempData["Bus4BusSectTitleOther3Name"] = @items.Bus4BusSectTitleOther3Name;
                        TempData["Bus4BusSectTitleOther1"] = @items.Bus4BusSectTitleOther1;
                        TempData["Bus4BusSectTitleOther2"] = @items.Bus4BusSectTitleOther2;
                        TempData["Bus4BusSectTitleOther3"] = @items.Bus4BusSectTitleOther3;
                        TempData["Bus4TenantName"] = @items.Bus4TenantName;
                        TempData["Bus4Rental"] = @items.Bus4Rental;
                        TempData["Bus4Othercontribution"] = @items.Bus4OtherContribution;
                        TempData["Bus4MonthlyLevy"] = @items.Bus4MonthlyLevy;
                        TempData["Bus4RentalLandSize"] = @items.Bus4RentalLandSize;
                        TempData["Bus4Escalation"] = @items.Bus4Escalation;
                        TempData["Bus4LeaseTerm"] = @items.Bus4LeaseTerm;
                        TempData["Bus4StartDate"] = @items.Bus4StartDate;
                        TempData["Bus4PoolSize"] = @items.Bus4PoolSize;
                        TempData["Bus4TennisCourtSize"] = @items.Bus4TennisCourtSize;
                        TempData["Bus4CommonPropertyOther1"] = @items.Bus4CommonPropertyOther1;
                        TempData["Bus4CommonPropertyOther2"] = @items.Bus4CommonPropertyOther2;
                        TempData["Bus4CommonPropertyOther3"] = @items.Bus4CommonPropertyOther3;
                        TempData["Bus4GarageSize"] = @items.Bus4GarageSize;
                        TempData["Bus4CarportSize"] = @items.Bus4CarportSize;
                        TempData["Bus4OpenParkingSize"] = @items.Bus4OpenParkingSize;
                        TempData["Bus4StoreRoomSize"] = @items.Bus4StoreRoomSize;
                        TempData["Bus4GardenSize"] = @items.Bus4GardenSize;
                        TempData["Bus4ExclusiveOther"] = @items.Bus4ExclusiveOther;


                        //Section 5
                        TempData["CurrentAskingPrice"] = @items.CurrentAskingPrice;
                        TempData["PreviousAskingPrice"] = @items.PreviousAskingPrice;
                        TempData["AgentName"] = @items.AgentName;
                        TempData["UnitNo"] = @items.UnitNo;
                        TempData["OtherNearbySales"] = @items.OtherNearbySales;
                        TempData["SaleDate"] = @items.SaleDate;
                        TempData["CurrentRecievedOffer"] = @items.CurrentRecievedOffer;
                        TempData["PreviousRecievedOffer"] = @items.PreviousRecievedOffer;
                        TempData["AgentTelNo"] = @items.AgentTelNo;
                        TempData["SuburbName"] = @items.SuburbName;
                        TempData["SellingPrice"] = @items.SellingPrice;

                        //Section 6

                        TempData["OldPropertyDescription"] = @items.OldPropertyDescription;
                        TempData["OldCategory"] = @items.OldCategory;
                        TempData["OldAddress"] = @items.OldAddress;
                        TempData["OldExtent"] = @items.OldExtent;
                        TempData["OldMarketValue"] = @items.OldMarketValue;
                        TempData["OldOwner"] = @items.OldOwner;
                        TempData["NewPropertyDescription"] = @items.NewPropertyDescription;
                        TempData["NewCategory"] = @items.NewCategory;
                        TempData["NewAddress"] = @items.NewAddress;
                        TempData["NewExtent"] = @items.NewExtent;
                        TempData["NewMarketValue"] = @items.NewMarketValue;
                        TempData["NewOwner"] = @items.NewOwner;
                        TempData["ObjectionReasons"] = @items.ObjectionReasons;
                        TempData["Old2Category"] = @items.Old2Category;
                        TempData["Old2Extent"] = @items.Old2Extent;
                        TempData["Old2MarketValue"] = @items.Old2MarketValue;
                        TempData["New2Category"] = @items.New2Category;
                        TempData["New2Extent"] = @items.New2Extent;
                        TempData["New2MarketValue"] = @items.New2MarketValue;
                        TempData["Old3Category"] = @items.Old3Category;
                        TempData["Old3Extent"] = @items.Old3Extent;
                        TempData["Old3MarketValue"] = @items.Old3MarketValue;
                        TempData["New3Category"] = @items.New3Category;
                        TempData["New3Extent"] = @items.New3Extent;
                        TempData["New3MarketValue"] = @items.New3MarketValue;

                        //Section 7
                        TempData["SignatureName"] = @items.SignatureName;
                        TempData["SignaturePicture"] = @items.SignaturePicture;
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message.ToString());               
                    throw ex;
                }

                return PartialView("BusForm", ObjectionTBs);
            }

            if (propertyID == "Multi" || propertyID == "Multi_omission")
            {
                if (queryId != null)
                {
                    Valuation(queryId, objectionNo);
                }

                if (ObjectionTBs.Count > 0)
                {
                    ObjectionTBs.Clear();
                }
                try
                {
                    conQ.Open();
                    com.Connection = conQ;
                    //Section 1
                    com.CommandText = "EXEC [Objection_Query].dbo.FormViewMultNotValued @Objection_no ='" + objectionNo + "'";
                    if (AppealStatus == "True")
                    {
                        com.CommandText = "EXEC [Objection_Query].dbo.AppealFormViewMult @Objection_no = '" + objectionNo + "'";
                    }
                    else
                    {
                        com.CommandText = "EXEC [Objection_Query].dbo.FormViewMultNotValued @Objection_no = '" + objectionNo + "'";
                    }
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        ObjectionTBs.Add(new ObjectionTB
                        {
                            QUERY_Type = dr["QUERY_Type"].ToString(),

                            //Section 1
                            QUERY_No = dr["QUERY_No"].ToString(),
                            OwnerName = dr["Owner_Name"].ToString(),
                            OwnerCompany = dr["Owner_Company"].ToString(),
                            OwnerIdentity = dr["Owner_Identity"].ToString(),
                            OwnerAddress1 = dr["Owner_Address_1"].ToString(),
                            OwnerAddress2 = dr["Owner_Address_2"].ToString(),
                            OwnerAddress3 = dr["Owner_Address_3"].ToString(),
                            OwnerAddress4 = dr["Owner_Address_4"].ToString(),
                            OwnerAddress5 = dr["Owner_Address_5"].ToString(),
                            OwnerPostal1 = dr["Owner_Postal_1"].ToString(),
                            OwnerPostal2 = dr["Owner_Postal_2"].ToString(),
                            OwnerPostal3 = dr["Owner_Postal_3"].ToString(),
                            OwnerPostal4 = dr["Owner_Postal_4"].ToString(),
                            OwnerPostal5 = dr["Owner_Postal_5"].ToString(),
                            OwnerHomePhone = dr["Owner_Home_Phone"].ToString(),
                            OwnerCellPhone = dr["Owner_Cell_Phone"].ToString(),
                            OwnerWorkPhone = dr["Owner_Work_Phone"].ToString(),
                            OwnerFaxPhone = dr["Owner_Fax_Phone"].ToString(),
                            OwnerEmail = dr["Owner_Email"].ToString(),
                            ObjectorName = dr["Objector_Name"].ToString(),
                            ObjectorIdentity = dr["Objector_Identity"].ToString(),
                            ObjectorCompany = dr["Objector_Company"].ToString(),
                            ObjectorPostal1 = dr["Objector_Postal_1"].ToString(),
                            ObjectorPostal2 = dr["Objector_Postal_2"].ToString(),
                            ObjectorPostal3 = dr["Objector_Postal_3"].ToString(),
                            ObjectorPostal4 = dr["Objector_Postal_4"].ToString(),
                            ObjectorPostal5 = dr["Objector_Postal_5"].ToString(),
                            ObjectorHome = dr["Objector_Home"].ToString(),
                            ObjectorCell = dr["Objector_Cell"].ToString(),
                            ObjectorWork = dr["Objector_Work"].ToString(),
                            ObjectorFax = dr["Objector_Fax"].ToString(),
                            ObjectorEmail = dr["Objector_Email"].ToString(),
                            ObjectorStatus = dr["Objector_Status"].ToString(),
                            RepresentativeName = dr["Representative_name"].ToString(),
                            RepPostal1 = dr["Rep_Postal_1"].ToString(),
                            RepPostal2 = dr["Rep_Postal_2"].ToString(),
                            RepPostal3 = dr["Rep_Postal_3"].ToString(),
                            RepPostal4 = dr["Rep_Postal_4"].ToString(),
                            RepPostal5 = dr["Rep_Postal_5"].ToString(),
                            RepHomePhone = dr["Rep_Home_Phone"].ToString(),
                            RepCellPhone = dr["Rep_Cell_Phone"].ToString(),
                            RepWorkPhone = dr["Rep_Work_Phone"].ToString(),
                            RepFaxPhone = dr["Rep_Fax_Phone"].ToString(),
                            RepEmail = dr["Rep_Email"].ToString(),

                            //Section 2
                            PhysicalAddress = dr["physical_address"].ToString(),
                            TownName = dr["Town_Name"].ToString(),
                            Code = dr["Code"].ToString(),
                            Extent = dr["Extent"].ToString(),
                            MunicipalAccountNo = dr["Municipal_Account_No"].ToString(),
                            BondHolderName = dr["BondHolder_Name"].ToString(),
                            RegisteredAmount = dr["Registered_Amount"].ToString(),
                            FullDetails = dr["Full_Details"].ToString(),
                            ServitudeNo = dr["Servitude_No"].ToString(),
                            AffectedArea = dr["Affected_Area"].ToString(),
                            PropertyFavourOf = dr["Property_Favour_Of"].ToString(),
                            PropertyPurpose = dr["Property_Purpose"].ToString(),
                            CompensationPaid = dr["Compensation_Paid"].ToString(),
                            PaymentDate = dr["Payment_Date"].ToString(),
                            CompensationAmount = dr["Compensation_Amount"].ToString(),

                            //Section 3 Busness
                            BusTenantName = dr["Bus_Tenant_Name"].ToString(),
                            BusRentalLandSize = dr["Bus_Rental_Land_Size"].ToString(),
                            BusRental = dr["Bus_Rental"].ToString(),
                            BusEscalation = dr["Bus_Escalation"].ToString(),
                            BusOtherContribution = dr["Bus_Other_contribution"].ToString(),
                            BusLeaseTerm = dr["Bus_Lease_Term"].ToString(),
                            BusStartDate = dr["Bus_Start_Date"].ToString(),
                            BusBuildingNo = dr["Bus_Building_No"].ToString(),
                            BusBuildingSize = dr["Bus_Building_Size"].ToString(),
                            BusShops = dr["Bus_Shops"].ToString(),
                            BusBuildingCondition = dr["Bus_Building_Condition"].ToString(),
                            BusExtentLandFurtherDev = dr["Bus_Extent_Land_further_Dev"].ToString(),
                            BusOtherFeaturesCondition = dr["Bus_Other_features_Condition"].ToString(),

                            //Section 3 residential

                            ResNoOfBedroom = dr["Res_No_of_Bedroom"].ToString(),
                            ResNoOfBathRoom = dr["Res_No_of_BathRoom"].ToString(),
                            ResKitchen = dr["Res_Kitchen"].ToString(),
                            ResLounge = dr["Res_Lounge"].ToString(),
                            ResDinningRoom = dr["Res_Dinning_Room"].ToString(),
                            ResLoungeDiningRoom = dr["Res_Lounge_Dining_Room"].ToString(),
                            ResStudy = dr["Res_Study"].ToString(),
                            ResPlayRoom = dr["Res_Play_Room"].ToString(),
                            ResTelevision = dr["Res_Television"].ToString(),
                            ResLaundry = dr["Res_Laundry"].ToString(),
                            ResSeperateToilet = dr["Res_Seperate_Toilet"].ToString(),
                            ResDwellOther1 = dr["Res_Dwell_Other1"].ToString(),
                            ResDwellOther2 = dr["Res_Dwell_Other2"].ToString(),
                            ResDwellOther3 = dr["Res_Dwell_Other3"].ToString(),
                            ResDwellOther4 = dr["Res_Dwell_Other4"].ToString(),
                            ResNoOfGarages = dr["Res_No_of_Garages"].ToString(),
                            ResGrannyRoom = dr["Res_Granny_Room"].ToString(),
                            ResOutbuildOther = dr["Res_Outbuild_Other"].ToString(),
                            ResMainDwellingSize = dr["Res_Main_Dwelling_Size"].ToString(),
                            ResOutsideBuildingSize = dr["Res_Outside_Building_Size"].ToString(),
                            ResOtherBuildingSize = dr["Res_Other_Building_Size"].ToString(),
                            ResTotalBuildingSize = dr["Res_Total_Building_Size"].ToString(),
                            ResSwimmingPool = dr["Res_Swimming_Pool"].ToString(),
                            ResBoreHole = dr["Res_Bore_Hole"].ToString(),
                            ResTennisCourt = dr["Res_Tennis_Court"].ToString(),
                            ResGarden = dr["Res_Garden"].ToString(),
                            ResOtherDwell1 = dr["Res_other_dwell1"].ToString(),
                            ResOtherDwell2 = dr["Res_other_dwell2"].ToString(),
                            ResFence = dr["Res_Fence"].ToString(),
                            ResFenceFront = dr["Res_Fence_Front"].ToString(),
                            ResFenceBack = dr["Res_Fence_Back"].ToString(),
                            ResFenceSide1 = dr["Res_Fence_Side_1"].ToString(),
                            ResFenceSide2 = dr["Res_Fence_Side_2"].ToString(),
                            ResFenceHeightFront = dr["Res_Fence_Height_Front"].ToString(),
                            ResFenceHeightBack = dr["Res_Fence_Height_Back"].ToString(),
                            ResFenceHeightSide1 = dr["Res_Fence_Height_Side1"].ToString(),
                            ResFenceHeightSide2 = dr["Res_Fence_Height_Side2"].ToString(),
                            ResDriveWay = dr["Res_Drive_Way"].ToString(),
                            ResSecurityBoomedArea = dr["Res_Security_Boomed_Area"].ToString(),
                            ResOtherFeatures = dr["Res_Other_features"].ToString(),
                            ResOtherFeaturesCondition = dr["Res_Other_features_Condition"].ToString(),
                            ResGeneralCondition = dr["Res_General_Condition"].ToString(),

                            //Section 3 Agriculture
                            AgriNoOfBedroom = dr["Agri_No_of_Bedroom"].ToString(),
                            AgriNoOfBathRoom = dr["Agri_No_of_BathRoom"].ToString(),
                            AgriKitchen = dr["Agri_Kitchen"].ToString(),
                            AgriLounge = dr["Agri_Lounge"].ToString(),
                            AgriDinningRoom = dr["Agri_Dinning_Room"].ToString(),
                            AgriLoungeDiningRoom = dr["Agri_Lounge_Dining_Room"].ToString(),
                            AgriStudy = dr["Agri_Study"].ToString(),
                            AgriPlayRoom = dr["Agri_Play_Room"].ToString(),
                            AgriTelevision = dr["Agri_Television"].ToString(),
                            AgriLaundry = dr["Agri_Laundry"].ToString(),
                            AgriSeperateToilet = dr["Agri_Seperate_Toilet"].ToString(),
                            AgriDwellOther1 = dr["Agri_Dwell_Other1"].ToString(),
                            AgriMainDwellingSize = dr["Agri_Main_Dwelling_Size"].ToString(),
                            AgriBuildingNo = dr["Agri_Building_No"].ToString(),
                            AgriBuildingDescription = dr["Agri_Building_Description"].ToString(),
                            AgriBuildingSize = dr["Agri_Building_Size"].ToString(),
                            AgriBuildingCondition = dr["Agri_Building_Condition"].ToString(),
                            AgriBuildingFunctional = dr["Agri_Building_Functional"].ToString(),
                            AgriAnotherPurposeNotAgriculture = dr["Agri_Another_Purpose_Not_Agriculture"].ToString(),
                            AgriAnotherPurposeNotAgricultureDesc = dr["Agri_Another_Purpose_Not_Agriculture_Desc"].ToString(),
                            AgriNonAgricultural = dr["Agri_Non_Agricultural"].ToString(),
                            AgriGrazing = dr["Agri_Grazing"].ToString(),
                            AgriUnderIrrigation = dr["Agri_Under_Irrigation"].ToString(),
                            AgriDryLand = dr["Agri_Dry_Land"].ToString(),
                            AgriPermanentCrop = dr["Agri_Permanent_Crop"].ToString(),
                            AgriOtherHa1 = dr["Agri_Other_ha_1"].ToString(),
                            AgriOtherHa2 = dr["Agri_Other_ha_2"].ToString(),
                            AgriOtherHa3 = dr["Agri_Other_ha_3"].ToString(),
                            AgriTotalHa = dr["Agri_Total_ha"].ToString(),
                            AgriFenceCondition = dr["Agri_Fence_Condition"].ToString(),
                            AgriGameAreaFenced = dr["Agri_Game_Area_Fenced"].ToString(),
                            AgriNumOfBoreholes = dr["Agri_Num_of_Boreholes"].ToString(),
                            AgriOutputLitresHours = dr["Agri_Output_litres_Hours"].ToString(),
                            AgriDams = dr["Agri_Dams"].ToString(),
                            AgriCapacity = dr["Agri_Capacity"].ToString(),
                            AgriExposedToRiver = dr["Agri_Exposed_To_River"].ToString(),
                            AgriLandClaim = dr["Agri_Land_Claim"].ToString(),
                            AgriClaimDate = dr["Agri_Claim_Date"].ToString(),
                            AgriGazetteNo = dr["Agri_Gazette_No"].ToString(),
                            AgriWaterRights = dr["Agri_Water_Rights"].ToString(),
                            AgriWaterRightsDetails = dr["Agri_Water_Rights_Details"].ToString(),
                            AgriRezoningConsentUse = dr["Agri_Rezoning_Consent_Use"].ToString(),
                            AgriConsentUseDetails = dr["Agri_Consent_Use_Details"].ToString(),
                            AgriLandExcised = dr["Agri_Land_Excised"].ToString(),
                            AgriNewFarmDesc = dr["Agri_New_Farm_Desc"].ToString(),
                            AgriTownshipApplied = dr["Agri_Township_Applied"].ToString(),
                            AgriTownshipAppliedDetail = dr["Agri_Township_Applied_Detail"].ToString(),
                            AgriTenantName = dr["Agri_Tenant_Name"].ToString(),
                            AgriRentalLandSize = dr["Agri_Rental_Land_Size"].ToString(),
                            AgriRental = dr["Agri_Rental"].ToString(),
                            AgriEscalation = dr["Agri_Escalation"].ToString(),
                            AgriOtherContribution = dr["Agri_Other_contribution"].ToString(),
                            AgriLeaseTerm = dr["Agri_Lease_Term"].ToString(),
                            AgriStartDate = dr["Agri_Start_Date"].ToString(),
                            AgriUse = dr["Agri_Use"].ToString(),


                            //Section 4 Residential
                            Res4SchemeName = dr["Res4_Scheme_Name"].ToString(),
                            Res4SchemeNo = dr["Res4_Scheme_No"].ToString(),
                            Res4FlatNo = dr["Res4_Flat_No"].ToString(),
                            Res4UnitSize = dr["Res4_Unit_Size"].ToString(),
                            Res4ManagingAgentName = dr["Res4_Managing_Agent_Name"].ToString(),
                            Res4ManagingAgentTelNo = dr["Res4_Managing_Agent_Tel_No"].ToString(),
                            Res4NoOfBedroom = dr["Res4_No_of_Bedroom"].ToString(),
                            Res4NoOfBathRoom = dr["Res4_No_of_BathRoom"].ToString(),
                            Res4MonthlyLevyRes = dr["Res4_Monthly_Levy_Res"].ToString(),
                            Res4Kitchen = dr["Res4_Kitchen"].ToString(),
                            Res4Lounge = dr["Res4_Lounge"].ToString(),
                            Res4DinningRoom = dr["Res4_Dinning_Room"].ToString(),
                            Res4LoungeDiningRoom = dr["Res4_Lounge_Dining_Room"].ToString(),
                            Res4Study = dr["Res4_Study"].ToString(),
                            Res4PlayRoom = dr["Res4_Play_Room"].ToString(),
                            Res4Television = dr["Res4_Television"].ToString(),
                            Res4Laundry = dr["Res4_Laundry"].ToString(),
                            Res4SeperateToilet = dr["Res4_Seperate_Toilet"].ToString(),
                            Res4DwellOther1 = dr["Res4_Dwell_Other1"].ToString(),
                            Res4DwellOther2 = dr["Res4_Dwell_Other2"].ToString(),
                            Res4DwellOther3 = dr["Res4_Dwell_Other3"].ToString(),
                            Res4DwellOther4 = dr["Res4_Dwell_Other4"].ToString(),
                            Res4CommonPropertyOther1 = dr["Res4_Common_Property_Other_1"].ToString(),
                            Res4CommonPropertyOther2 = dr["Res4_Common_Property_Other_2"].ToString(),
                            Res4CommonPropertyOther3 = dr["Res4_Common_Property_Other_3"].ToString(),
                            Res4PoolSize = dr["Res4_Pool_Size"].ToString(),
                            Res4TennisCourtSize = dr["Res4_Tennis_Court_Size"].ToString(),
                            Res4GarageSize = dr["Res4_Garage_Size"].ToString(),
                            Res4CarportSize = dr["Res4_Carport_Size"].ToString(),
                            Res4OpenParkingSize = dr["Res4_Open_Parking_Size"].ToString(),
                            Res4StoreRoomSize = dr["Res4_Store_Room_Size"].ToString(),
                            Res4GardenSize = dr["Res4_Garden_Size"].ToString(),
                            Res4ExclusiveOther = dr["Res4_Exclusive_Other"].ToString(),

                            //Section 4 business
                            Bus4SchemeName = dr["Bus4_Scheme_Name"].ToString(),
                            Bus4SchemeNo = dr["Bus4_Scheme_No"].ToString(),
                            Bus4FlatNo = dr["Bus4_Flat_No"].ToString(),
                            Bus4UnitSize = dr["Bus4_Unit_Size"].ToString(),
                            Bus4ManagingAgentName = dr["Bus4_Managing_Agent_Name"].ToString(),
                            Bus4ManagingAgentTelNo = dr["Bus4_Managing_Agent_Tel_No"].ToString(),
                            Bus4Shops = dr["Bus4_Shops"].ToString(),
                            Bus4Offices = dr["Bus4_Offices"].ToString(),
                            Bus4Factories = dr["Bus4_Factories"].ToString(),
                            Bus4BusSectTitleOther1Name = dr["Bus4_Bus_Sect_Title_Other1_name"].ToString(),
                            Bus4BusSectTitleOther2Name = dr["Bus4_Bus_Sect_Title_Other2_name"].ToString(),
                            Bus4BusSectTitleOther3Name = dr["Bus4_Bus_Sect_Title_Other3_name"].ToString(),
                            Bus4BusSectTitleOther1 = dr["Bus4_Bus_Sect_Title_Other1"].ToString(),
                            Bus4BusSectTitleOther2 = dr["Bus4_Bus_Sect_Title_Other2"].ToString(),
                            Bus4BusSectTitleOther3 = dr["Bus4_Bus_Sect_Title_Other3"].ToString(),
                            Bus4TenantName = dr["Bus4_Tenant_Name"].ToString(),
                            Bus4Rental = dr["Bus4_Rental"].ToString(),
                            Bus4OtherContribution = dr["Bus4_Other_contribution"].ToString(),
                            Bus4MonthlyLevy = dr["Bus4_Monthly_Levy"].ToString(),
                            Bus4RentalLandSize = dr["Bus4_Rental_Land_Size"].ToString(),
                            Bus4Escalation = dr["Bus4_Escalation"].ToString(),
                            Bus4LeaseTerm = dr["Bus4_Lease_Term"].ToString(),
                            Bus4StartDate = dr["Bus4_Start_Date"].ToString(),
                            Bus4PoolSize = dr["Bus4_Pool_Size"].ToString(),
                            Bus4TennisCourtSize = dr["Bus4_Tennis_Court_Size"].ToString(),
                            Bus4CommonPropertyOther1 = dr["Bus4_Common_Property_Other_1"].ToString(),
                            Bus4CommonPropertyOther2 = dr["Bus4_Common_Property_Other_2"].ToString(),
                            Bus4CommonPropertyOther3 = dr["Bus4_Common_Property_Other_3"].ToString(),
                            Bus4GarageSize = dr["Bus4_Garage_Size"].ToString(),
                            Bus4CarportSize = dr["Bus4_Carport_Size"].ToString(),
                            Bus4OpenParkingSize = dr["Bus4_Open_Parking_Size"].ToString(),
                            Bus4StoreRoomSize = dr["Bus4_Store_Room_Size"].ToString(),
                            Bus4GardenSize = dr["Bus4_Garden_Size"].ToString(),
                            Bus4ExclusiveOther = dr["Bus4_Exclusive_Other"].ToString(),


                            //Section 5
                            CurrentAskingPrice = dr["Current_Asking_price"].ToString(),
                            PreviousAskingPrice = dr["Previous_Asking_price"].ToString(),
                            AgentName = dr["Agent_Name"].ToString(),
                            UnitNo = dr["Unit_No"].ToString(),
                            OtherNearbySales = dr["Other_Nearby_Sales"].ToString(),
                            SaleDate = dr["Sale_Date"].ToString(),
                            CurrentRecievedOffer = dr["Current_Recieved_Offer"].ToString(),
                            PreviousRecievedOffer = dr["Previous_Recieved_Offer"].ToString(),
                            AgentTelNo = dr["Agent_Tel_No"].ToString(),
                            SuburbName = dr["Suburb_Name"].ToString(),
                            SellingPrice = dr["Selling_Price"].ToString(),


                            //Section 6

                            OldPropertyDescription = dr["Old_Property_Description"].ToString(),
                            OldCategory = dr["Old_Category"].ToString(),
                            OldAddress = dr["Old_Address"].ToString(),
                            OldExtent = dr["Old_Extent"].ToString(),
                            OldMarketValue = dr["Old_Market_Value"].ToString(),
                            OldOwner = dr["Old_Owner"].ToString(),
                            NewPropertyDescription = dr["New_Property_Description"].ToString(),
                            NewCategory = dr["New_Category"].ToString(),
                            NewAddress = dr["New_Address"].ToString(),
                            NewExtent = dr["New_Extent"].ToString(),
                            NewMarketValue = dr["New_Market_Value"].ToString(),
                            NewOwner = dr["New_Owner"].ToString(),
                            ObjectionReasons = dr["Objection_Reasons"].ToString(),
                            Old2Category = dr["Old2_Category"].ToString(),
                            Old2Extent = dr["Old2_Extent"].ToString(),
                            Old2MarketValue = dr["Old2_Market_Value"].ToString(),
                            New2Category = dr["New2_Category"].ToString(),
                            New2Extent = dr["New2_Extent"].ToString(),
                            New2MarketValue = dr["New2_Market_Value"].ToString(),
                            Old3Category = dr["Old3_Category"].ToString(),
                            Old3Extent = dr["Old3_Extent"].ToString(),
                            Old3MarketValue = dr["Old3_Market_Value"].ToString(),
                            New3Category = dr["New3_Category"].ToString(),
                            New3Extent = dr["New3_Extent"].ToString(),
                            New3MarketValue = dr["New3_Market_Value"].ToString(),

                            //Section 7
                            SignatureName = dr["Signature_Name"].ToString(),
                            SignaturePicture = dr["Signature_Picture"].ToString()

                        });
                    }
                    conQ.Close();
                    //TempData["QUERY_No"] = objectionNo;
                    ViewBag.OneMultiObjection = ObjectionTBs.ToList();

                    foreach (var items in ViewBag.OneMultiObjection)
                    {

                        TempData["QUERY_Type"] = @items.QUERY_Type;

                        //Section 1
                        TempData["QUERY_No"] = @items.QUERY_No;
                        TempData["OwnerName"] = @items.OwnerName;
                        TempData["OwnerIdentity"] = @items.OwnerIdentity;
                        TempData["OwnerCompany"] = @items.OwnerCompany;
                        TempData["OwnerAddress1"] = @items.OwnerAddress1;
                        TempData["OwnerAddress2"] = @items.OwnerAddress2;
                        TempData["OwnerAddress3"] = @items.OwnerAddress3;
                        TempData["OwnerAddress4"] = @items.OwnerAddress4;
                        TempData["OwnerAddress5"] = @items.OwnerAddress5;
                        TempData["OwnerPostal1"] = @items.OwnerPostal1;
                        TempData["OwnerPostal2"] = @items.OwnerPostal2;
                        TempData["OwnerPostal3"] = @items.OwnerPostal3;
                        TempData["OwnerPostal4"] = @items.OwnerPostal4;
                        TempData["OwnerPostal5"] = @items.OwnerPostal5;
                        TempData["OwnerHomePhone"] = @items.OwnerHomePhone;
                        TempData["OwnerCellPhone"] = @items.OwnerCellPhone;
                        TempData["OwnerWorkPhone"] = @items.OwnerWorkPhone;
                        TempData["OwnerFaxPhone"] = @items.OwnerFaxPhone;
                        TempData["OwnerEmail"] = @items.OwnerEmail;
                        TempData["ObjectorName"] = @items.ObjectorName;
                        TempData["ObjectorIdentity"] = @items.ObjectorIdentity;
                        TempData["ObjectorCompany"] = @items.ObjectorCompany;
                        TempData["ObjectorPostal1"] = @items.ObjectorPostal1;
                        TempData["ObjectorPostal2"] = @items.ObjectorPostal2;
                        TempData["QUERY_No"] = @items.ObjectorPostal3;
                        TempData["ObjectorPostal3"] = @items.ObjectorPostal4;
                        TempData["ObjectorPostal5"] = @items.ObjectorPostal5;
                        TempData["ObjectorHome"] = @items.ObjectorHome;
                        TempData["ObjectorCell"] = @items.ObjectorCell;
                        TempData["ObjectorWork"] = @items.ObjectorWork;
                        TempData["ObjectorFax"] = @items.ObjectorFax;
                        TempData["ObjectorEmail"] = @items.ObjectorEmail;
                        TempData["ObjectorStatus"] = @items.ObjectorStatus;
                        TempData["RepresentativeName"] = @items.RepresentativeName;
                        TempData["RepPostal1"] = @items.RepPostal1;
                        TempData["RepPostal2"] = @items.RepPostal2;
                        TempData["RepPostal3"] = @items.RepPostal3;
                        TempData["RepPostal4"] = @items.RepPostal4;
                        TempData["RepPostal5"] = @items.RepPostal5;
                        TempData["RepHomePhone"] = @items.RepHomePhone;
                        TempData["RepCellPhone"] = @items.RepCellPhone;
                        TempData["RepWorkPhone"] = @items.RepWorkPhone;
                        TempData["RepFaxPhone"] = @items.RepFaxPhone;
                        TempData["RepEmail"] = @items.RepEmail;

                        //Section 2
                        TempData["PhysicalAddress"] = @items.PhysicalAddress;
                        TempData["TownName"] = @items.TownName;
                        TempData["Code"] = @items.Code;
                        TempData["Extent"] = @items.Extent;
                        TempData["MunicipalAccountNo"] = @items.MunicipalAccountNo;
                        TempData["BondHolderName"] = @items.BondHolderName;
                        TempData["RegisteredAmount"] = @items.RegisteredAmount;
                        TempData["FullDetails"] = @items.FullDetails;
                        TempData["ServitudeNo"] = @items.ServitudeNo;
                        TempData["AffectedArea"] = @items.AffectedArea;
                        TempData["PropertyFavourOf"] = @items.PropertyFavourOf;
                        TempData["PropertyPurpose"] = @items.PropertyPurpose;
                        TempData["CompensationPaid"] = @items.CompensationPaid;
                        TempData["PaymentDate"] = @items.PaymentDate;
                        TempData["CompensationAmount"] = @items.CompensationAmount;

                        //Section 3 Business
                        TempData["BusTenantName"] = @items.BusTenantName;
                        TempData["BusRentalLandSize"] = @items.BusRentalLandSize;
                        TempData["BusRental"] = @items.BusRental;
                        TempData["BusEscalation"] = @items.BusEscalation;
                        TempData["BusOtherContribution"] = @items.BusOtherContribution;
                        TempData["BusLeaseTerm"] = @items.BusLeaseTerm;
                        TempData["BusStartDate"] = @items.BusStartDate;
                        TempData["BusBuildingNo"] = @items.BusBuildingNo;
                        TempData["BusBuildingSize"] = @items.BusBuildingSize;
                        TempData["BusShops"] = @items.BusShops;
                        TempData["BusBuildingCondition"] = @items.BusBuildingCondition;
                        TempData["BusExtentLandFurtherDev"] = @items.BusExtentLandFurtherDev;
                        TempData["BusOtherFeaturesCondition"] = @items.BusOtherFeaturesCondition;

                        //Section 3 Residential
                        TempData["ResNoOfBedroom"] = @items.ResNoOfBedroom;
                        TempData["ResNoOfBathRoom"] = @items.ResNoOfBathRoom;
                        TempData["ResKitchen"] = @items.ResKitchen;
                        TempData["ResLounge"] = @items.ResLounge;
                        TempData["ResDinningRoom"] = @items.ResDinningRoom;
                        TempData["ResLoungeDiningRoom"] = @items.ResLoungeDiningRoom;
                        TempData["ResStudy"] = @items.ResStudy;
                        TempData["ResPlayRoom"] = @items.ResPlayRoom;
                        TempData["ResTelevision"] = @items.ResTelevision;
                        TempData["ResLaundry"] = @items.ResLaundry;
                        TempData["ResSeperateToilet"] = @items.ResSeperateToilet;
                        TempData["ResDwellOther1"] = @items.ResDwellOther1;
                        TempData["ResDwellOther2"] = @items.ResDwellOther2;
                        TempData["ResDwellOther3"] = @items.ResDwellOther3;
                        TempData["ResDwellOther4"] = @items.ResDwellOther4;
                        TempData["ResNoOfGarages"] = @items.ResNoOfGarages;
                        TempData["ResGrannyRoom"] = @items.ResGrannyRoom;
                        TempData["ResOutbuildOther"] = @items.ResOutbuildOther;
                        TempData["ResMainDwellingSize"] = @items.ResMainDwellingSize;
                        TempData["ResOutsideBuildingSize"] = @items.ResOutsideBuildingSize;
                        TempData["ResOtherBuildingSize"] = @items.ResOtherBuildingSize;
                        TempData["ResTotalBuildingSize"] = @items.ResTotalBuildingSize;
                        TempData["ResSwimmingPool"] = @items.ResSwimmingPool;
                        TempData["ResBoreHole"] = @items.ResBoreHole;
                        TempData["ResTennisCourt"] = @items.ResTennisCourt;
                        TempData["ResGarden"] = @items.ResGarden;
                        TempData["ResOtherDwell1"] = @items.ResOtherDwell1;
                        TempData["ResOtherDwell2"] = @items.ResOtherDwell2;
                        TempData["ResFence"] = @items.ResFence;
                        TempData["ResFenceFront"] = @items.ResFenceFront;
                        TempData["ResFenceBack"] = @items.ResFenceBack;
                        TempData["ResFenceSide1"] = @items.ResFenceSide1;
                        TempData["ResFenceSide2"] = @items.ResFenceSide2;
                        TempData["ResFenceHeightFront"] = @items.ResFenceHeightFront;
                        TempData["ResFenceHeightBack"] = @items.ResFenceHeightBack;
                        TempData["ResFenceHeightSide1"] = @items.ResFenceHeightSide1;
                        TempData["ResFenceHeightSide2"] = @items.ResFenceHeightSide2;
                        TempData["ResDriveWay"] = @items.ResDriveWay;
                        TempData["ResSecurityBoomedArea"] = @items.ResSecurityBoomedArea;
                        TempData["ResOtherFeatures"] = @items.ResOtherFeatures;
                        TempData["ResOtherFeaturesCondition"] = @items.ResOtherFeaturesCondition;
                        TempData["ResGeneralCondition"] = @items.ResGeneralCondition;

                        //Section 3 Agriculture
                        TempData["AgriNoOfBedroom"] = @items.AgriNoOfBedroom;
                        TempData["Agri_No_of_BathRoom"] = @items.AgriNoOfBathRoom;
                        TempData["Agri_Kitchen"] = @items.AgriKitchen;
                        TempData["AgriLounge"] = @items.AgriLounge;
                        TempData["AgriDinningRoom"] = @items.AgriDinningRoom;
                        TempData["AgriLoungeDiningRoom"] = @items.AgriLoungeDiningRoom;
                        TempData["AgriStudy"] = @items.AgriStudy;
                        TempData["AgriPlayRoom"] = @items.AgriPlayRoom;
                        TempData["AgriTelevision"] = @items.AgriTelevision;
                        TempData["AgriLaundry"] = @items.AgriLaundry;
                        TempData["AgriSeperateToilet"] = @items.AgriSeperateToilet;
                        TempData["AgriDwellOther1"] = @items.AgriDwellOther1;
                        TempData["AgriMainDwellingSize"] = @items.AgriMainDwellingSize;
                        TempData["AgriBuildingNo"] = @items.AgriBuildingNo;
                        TempData["AgriBuildingDescription"] = @items.AgriBuildingDescription;
                        TempData["AgriBuildingSize"] = @items.AgriBuildingSize;
                        TempData["AgriBuildingCondition"] = @items.AgriBuildingCondition;
                        TempData["AgriBuildingFunctional"] = @items.AgriBuildingFunctional;
                        TempData["AgriAnotherPurposeNotAgriculture"] = @items.AgriAnotherPurposeNotAgriculture;
                        TempData["AgriAnotherPurposeNotAgricultureDesc"] = @items.AgriAnotherPurposeNotAgricultureDesc;
                        TempData["AgriNonAgricultural"] = @items.AgriNonAgricultural;
                        TempData["AgriGrazing"] = @items.AgriGrazing;
                        TempData["AgriUnderIrrigation"] = @items.AgriUnderIrrigation;
                        TempData["AgriDryLand"] = @items.AgriDryLand;
                        TempData["AgriPermanentCrop"] = @items.AgriPermanentCrop;
                        TempData["AgriOtherHa1"] = @items.AgriOtherHa1;
                        TempData["AgriOtherHa2"] = @items.AgriOtherHa2;
                        TempData["AgriOtherHa3"] = @items.AgriOtherHa3;
                        TempData["AgriTotalHa"] = @items.AgriTotalHa;
                        TempData["AgriFenceCondition"] = @items.AgriFenceCondition;
                        TempData["AgriGameAreaFenced"] = @items.AgriGameAreaFenced;
                        TempData["AgriNumOfBoreholes"] = @items.AgriNumOfBoreholes;
                        TempData["AgriOutputLitresHours"] = @items.AgriOutputLitresHours;
                        TempData["AgriDams"] = @items.AgriDams;
                        TempData["AgriCapacity"] = @items.AgriCapacity;
                        TempData["AgriExposedToRiver"] = @items.AgriExposedToRiver;
                        TempData["AgriLandClaim"] = @items.AgriLandClaim;
                        TempData["AgriClaimDate"] = @items.AgriClaimDate;
                        TempData["AgriGazetteNo"] = @items.AgriGazetteNo;
                        TempData["AgriWaterRights"] = @items.AgriWaterRights;
                        TempData["AgriWaterRightsDetails"] = @items.AgriWaterRightsDetails;
                        TempData["AgriRezoningConsentUse"] = @items.AgriRezoningConsentUse;
                        TempData["AgriConsentUseDetails"] = @items.AgriConsentUseDetails;
                        TempData["AgriLandExcised"] = @items.PhysicalAddress;
                        TempData["AgriNewFarmDesc"] = @items.AgriNewFarmDesc;
                        TempData["AgriTownshipApplied"] = @items.AgriTownshipApplied;
                        TempData["AgriTownshipAppliedDetail"] = @items.AgriTownshipAppliedDetail;
                        TempData["AgriTenantName"] = @items.AgriTenantName;
                        TempData["AgriRentalLandSize"] = @items.AgriRentalLandSize;
                        TempData["AgriRental"] = @items.AgriRental;
                        TempData["AgriEscalation"] = @items.AgriEscalation;
                        TempData["AgriOtherContribution"] = @items.AgriOtherContribution;
                        TempData["AgriLeaseTerm"] = @items.AgriLeaseTerm;
                        TempData["AgriStartDate"] = @items.AgriStartDate;
                        TempData["AgriUse"] = @items.AgriUse;

                        //Section 4 Residential
                        TempData["Res4SchemeName"] = @items.Res4SchemeName;
                        TempData["Res4SchemeNo"] = @items.Res4SchemeNo;
                        TempData["Res4FlatNo"] = @items.Res4FlatNo;
                        TempData["Res4UnitSize"] = @items.Res4UnitSize;
                        TempData["Res4ManagingAgentName"] = @items.Res4ManagingAgentName;
                        TempData["Res4ManagingAgentTelNo"] = @items.Res4ManagingAgentTelNo;
                        TempData["Res4NoOfBedroom"] = @items.Res4NoOfBedroom;
                        TempData["Res4NoOfBathRoom"] = @items.Res4NoOfBathRoom;
                        TempData["Res4MonthlyLevyRes"] = @items.Res4MonthlyLevyRes;
                        TempData["Res4Kitchen"] = @items.Res4Kitchen;
                        TempData["Res4Lounge"] = @items.Res4Lounge;
                        TempData["Res4DinningRoom"] = @items.Res4DinningRoom;
                        TempData["Res4LoungeDiningRoom"] = @items.Res4LoungeDiningRoom;
                        TempData["Res4Study"] = @items.Res4Study;
                        TempData["Res4PlayRoom"] = @items.Res4PlayRoom;
                        TempData["Res4Television"] = @items.Res4Television;
                        TempData["Res4Laundry"] = @items.Res4Laundry;
                        TempData["Res4SeperateToilet"] = @items.Res4SeperateToilet;
                        TempData["Res4DwellOther1"] = @items.Res4DwellOther1;
                        TempData["Res4DwellOther2"] = @items.Res4DwellOther2;
                        TempData["Res4DwellOther3"] = @items.Res4DwellOther3;
                        TempData["Res4DwellOther4"] = @items.Res4DwellOther4;
                        TempData["Res4CommonPropertyOther1"] = @items.Res4CommonPropertyOther1;
                        TempData["Res4CommonPropertyOther2"] = @items.Res4CommonPropertyOther2;
                        TempData["Res4CommonPropertyOther3"] = @items.Res4CommonPropertyOther3;
                        TempData["Res4TennisCourtSize"] = @items.Res4TennisCourtSize;
                        TempData["Res4GarageSize"] = @items.Res4GarageSize;
                        TempData["Res4CarportSize"] = @items.Res4CarportSize;
                        TempData["Res4OpenParkingSize"] = @items.Res4OpenParkingSize;
                        TempData["Res4StoreRoomSize"] = @items.Res4StoreRoomSize;
                        TempData["Res4GardenSize"] = @items.Res4GardenSize;
                        TempData["Res4ExclusiveOther"] = @items.Res4ExclusiveOther;
                        TempData["Res4ExclusiveOther"] = @items.Res4ExclusiveOther;
                        TempData["Res4PoolSize"] = @items.Res4PoolSize;


                        //Section 4 Business
                        TempData["Bus4SchemeName"] = @items.Bus4SchemeName;
                        TempData["Bus4SchemeNo"] = @items.Bus4SchemeNo;
                        TempData["Bus4FlatNo"] = @items.Bus4FlatNo;
                        TempData["Bus4UnitSize"] = @items.Bus4UnitSize;
                        TempData["Bus4ManagingAgentName"] = @items.Bus4ManagingAgentName;
                        TempData["Bus4ManagingAgentTelNo"] = @items.Bus4ManagingAgentTelNo;
                        TempData["Bus4Shops"] = @items.Bus4Shops;
                        TempData["Bus4Offices"] = @items.Bus4Offices;
                        TempData["Bus4Factories"] = @items.Bus4Factories;
                        TempData["Bus4BusSectTitleOther1Name"] = @items.Bus4BusSectTitleOther1Name;
                        TempData["Bus4BusSectTitleOther2Name"] = @items.Bus4BusSectTitleOther2Name;
                        TempData["Bus4BusSectTitleOther3Name"] = @items.Bus4BusSectTitleOther3Name;
                        TempData["Bus4BusSectTitleOther1"] = @items.Bus4BusSectTitleOther1;
                        TempData["Bus4BusSectTitleOther2"] = @items.Bus4BusSectTitleOther2;
                        TempData["Bus4BusSectTitleOther3"] = @items.Bus4BusSectTitleOther3;
                        TempData["Bus4TenantName"] = @items.Bus4TenantName;
                        TempData["Bus4Rental"] = @items.Bus4Rental;
                        TempData["Bus4Othercontribution"] = @items.Bus4OtherContribution;
                        TempData["Bus4MonthlyLevy"] = @items.Bus4MonthlyLevy;
                        TempData["Bus4RentalLandSize"] = @items.Bus4RentalLandSize;
                        TempData["Bus4Escalation"] = @items.Bus4Escalation;
                        TempData["Bus4LeaseTerm"] = @items.Bus4LeaseTerm;
                        TempData["Bus4StartDate"] = @items.Bus4StartDate;
                        TempData["Bus4PoolSize"] = @items.Bus4PoolSize;
                        TempData["Bus4TennisCourtSize"] = @items.Bus4TennisCourtSize;
                        TempData["Bus4CommonPropertyOther1"] = @items.Bus4CommonPropertyOther1;
                        TempData["Bus4CommonPropertyOther2"] = @items.Bus4CommonPropertyOther2;
                        TempData["Bus4CommonPropertyOther3"] = @items.Bus4CommonPropertyOther3;
                        TempData["Bus4GarageSize"] = @items.Bus4GarageSize;
                        TempData["Bus4CarportSize"] = @items.Bus4CarportSize;
                        TempData["Bus4OpenParkingSize"] = @items.Bus4OpenParkingSize;
                        TempData["Bus4StoreRoomSize"] = @items.Bus4StoreRoomSize;
                        TempData["Bus4GardenSize"] = @items.Bus4GardenSize;
                        TempData["Bus4ExclusiveOther"] = @items.Bus4ExclusiveOther;


                        //Section 5
                        TempData["CurrentAskingPrice"] = @items.CurrentAskingPrice;
                        TempData["PreviousAskingPrice"] = @items.PreviousAskingPrice;
                        TempData["AgentName"] = @items.AgentName;
                        TempData["UnitNo"] = @items.UnitNo;
                        TempData["OtherNearbySales"] = @items.OtherNearbySales;
                        TempData["SaleDate"] = @items.SaleDate;
                        TempData["CurrentRecievedOffer"] = @items.CurrentRecievedOffer;
                        TempData["PreviousRecievedOffer"] = @items.PreviousRecievedOffer;
                        TempData["AgentTelNo"] = @items.AgentTelNo;
                        TempData["SuburbName"] = @items.SuburbName;
                        TempData["SellingPrice"] = @items.SellingPrice;

                        //Section 6

                        TempData["OldPropertyDescription"] = @items.OldPropertyDescription;
                        TempData["OldCategory"] = @items.OldCategory;
                        TempData["OldAddress"] = @items.OldAddress;
                        TempData["OldExtent"] = @items.OldExtent;
                        TempData["OldMarketValue"] = @items.OldMarketValue;
                        TempData["OldOwner"] = @items.OldOwner;
                        TempData["NewPropertyDescription"] = @items.NewPropertyDescription;
                        TempData["NewCategory"] = @items.NewCategory;
                        TempData["NewAddress"] = @items.NewAddress;
                        TempData["NewExtent"] = @items.NewExtent;
                        TempData["NewMarketValue"] = @items.NewMarketValue;
                        TempData["NewOwner"] = @items.NewOwner;
                        TempData["ObjectionReasons"] = @items.ObjectionReasons;
                        TempData["Old2Category"] = @items.Old2Category;
                        TempData["Old2Extent"] = @items.Old2Extent;
                        TempData["Old2MarketValue"] = @items.Old2MarketValue;
                        TempData["New2Category"] = @items.New2Category;
                        TempData["New2Extent"] = @items.New2Extent;
                        TempData["New2MarketValue"] = @items.New2MarketValue;
                        TempData["Old3Category"] = @items.Old3Category;
                        TempData["Old3Extent"] = @items.Old3Extent;
                        TempData["Old3MarketValue"] = @items.Old3MarketValue;
                        TempData["New3Category"] = @items.New3Category;
                        TempData["New3Extent"] = @items.New3Extent;
                        TempData["New3MarketValue"] = @items.New3MarketValue;

                        //Section 7
                        TempData["SignatureName"] = @items.SignatureName;
                        TempData["SignaturePicture"] = @items.SignaturePicture;
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message.ToString());               
                    throw ex;
                }

                return PartialView("MultiForm", ObjectionTBs);
            }

            return View();
        }

        public IActionResult ResForm()
        {
            return View();
        }

        public IActionResult AgricForm()
        {
            return View();
        }

        public IActionResult BusForm()
        {
            return View();
        }

        public IActionResult MultiForm()
        {
            return View();
        }

		public IActionResult DownloadFiles(string PremiseID)
		{
			string folderPath = @"E:\\Draft_Section78\\" + PremiseID + "";
			//string[] filePaths = Directory.GetFiles(folderPath);

			if (!Directory.Exists(folderPath))
			{
				TempData["ErrorMessage"] = $"Town Name Description for: {TempData["TownshipDescription"]} and Premise Id: {PremiseID} evidence not uploaded";
				return RedirectToAction("ShowError");
			}

			string[] filePaths = Directory.GetFiles(folderPath);

			if (filePaths.Length == 0)
			{
				TempData["ErrorMessage"] = $"Town Name Description for: {TempData["TownshipDescription"]} and Premise Id: {PremiseID} evidence not uploaded";
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
		public async Task<IActionResult> KillTask(string? QueryId, string? PremiseId, string?
		   PropertyDescription, string? MarketValue, string? MarketValue1, string? MarketValue2, string? MarketValue3,
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
			TempData["PropertyDescription"] = PropertyDescription;

			int count = 0;

			string Premise_ID = PremiseId.ToString();
			string uploadRoot = $"{_config["AppSettings:FileRooTPathSection78"]}";
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

			if (drafts.Count > 0)
			{
				drafts.Clear();
			}
			try
			{
				con.Open();
				com.Connection = con;

				//string fileNameAttachValue = (files != null && files.FileName != null) ? Path.GetFileName(files.FileName) : null;

				com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValuedSection78] SET [Market Value] = '" + MarketValue + "', [Market Value1] = '" + MarketValue1 + "', [Market Value2] = '" + MarketValue2 + "', [Market Value3] = '" + MarketValue3 + "', [CAT Description] = '" + CATDescription + "', " + "[CAT Description1] = '" + CATDescription1 + "', [CAT Description2] = '" + CATDescription2 + "', [CAT Description3] = '" + CATDescription3 + "', Comment = '" + Comment + "', WEF_DATE = '" + WEF_DATE + "', Activity_Date = getdate(), " +
								  "FileNameAttach = '" + save_files + "', Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 6) WHERE QueryId = '" + QueryId + "'";

				dr = com.ExecuteReader();
				

				con.Close();

			}
			catch (Exception ex)
			{
				throw ex;
			}

			if (draftHistories.Count > 0)
			{
				draftHistories.Clear();
			}
			try
			{
				con.Open();
				com.Connection = con;
				com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistorySection78] ([UserName],[UserID],[PropertyDescription]," +
								  "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Comment],[UserActivity],[Status],[PremiseId],[ActivityDate], [Sector]) " +
								  "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PropertyDescription + "', '" + MarketValue + "' ,'" + MarketValue1 + "', '" + MarketValue2 + "', '" + MarketValue3 + "', '" + CATDescription + "', '" + CATDescription1 + "', '" + CATDescription2 + "', '" + CATDescription3 + "','" + Comment + "','Kill Values', (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 6),'" + PremiseId + "', getdate(), '" + userSector + "')";

				dr = com.ExecuteReader();
				
				con.Close();

			}
			catch (Exception ex)
			{
				throw ex;
			}

			return RedirectToAction("PropPerUser", new { userName = userName });
		}
		public IActionResult ShowError()
		{
			return View();
		}

		/*public IActionResult ValForm()
        {
            return View();
        }*/
	}
}

