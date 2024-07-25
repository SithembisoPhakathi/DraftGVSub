using GeneralValuationSubs.Interface;
using GeneralValuationSubs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using GeneralValuationSubs.SPClass;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using Microsoft.Net.Http.Headers;
using System.IO.Compression;
using System.Xml.Linq;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using static NuGet.Packaging.PackagingConstants;
using System.Diagnostics.Metrics;
using ExcelDataReader;
using System.Globalization;
using System.Text;
using System.IO;
using System.Data;
//using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace GeneralValuationSubs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UpdatedGvtoolContext _context;
        private readonly RoleManager<IdentityRole> directorRole;
        SqlCommand com = new SqlCommand();
        List<AdminValuer> adminValuers = new();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        List<DraftGv> draftGvs = new List<DraftGv>();
        List<Draft> drafts = new List<Draft>();
        List<NotValued> notvalued = new List<NotValued>();
        List<GV13> GV13s = new List<GV13>();
        List<GV18> GV18s = new List<GV18>();
        List<DraftHistory> draftHistories = new List<DraftHistory>();
        List<Category> categories = new List<Category>();
        readonly IBufferedFileUploadService _bufferedFileUploadService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(UpdatedGvtoolContext context, ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, Microsoft.Extensions.Configuration.IConfiguration config, IBufferedFileUploadService bufferedFileUploadService)
        {            
            _context = context;
            _logger = logger;
            _config = config;
            _hostingEnvironment = webHostEnvironment;
            _bufferedFileUploadService = bufferedFileUploadService;
            con.ConnectionString = _config.GetConnectionString("DefaultConnection");
        }
      
        //[HttpPost]
        public IActionResult Index()
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

            TempData.Keep("currentEmail");
            TempData.Keep("currentRole");
            TempData.Keep("currentUserRole");
            TempData.Keep("currentUserSector");
            TempData.Keep("currentUserEmail");
            TempData.Keep("currentUserSurname");
            TempData.Keep("currentUserFirstname");
            TempData.Keep("currentUserPhoneNo");
            TempData.Keep("currentUsername");

            return View();
        }

        public IActionResult PropPerUser(string? userName)
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
                    com.CommandText = "SELECT DATEDIFF(DAY,Start_Date, GETDATE()) AS Date_Diff,* FROM [UpdatedGVTool].[dbo].[NotValued] N " +
                                      "WHERE (AllocateName LIKE '%" + userName + "%') AND Status IN (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID IN (1, 4))" +
                                      "Order By N.Start_Date asc";

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        drafts.Add(new Draft
                        {
                            DraftId = (int)dr["DraftId"],
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
                            ApproverComment = dr["approverComment"].ToString(),
                            Start_Date = (DateTime)dr["Start_Date"],
                            DateDiff = (int)dr["Date_Diff"]

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

            return PartialView("PropPerUser",new { userName = userName});//drafts
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
                com.CommandText = "SELECT DATEDIFF(DAY,Start_Date, GETDATE()) AS Date_Diff, * FROM [UpdatedGVTool].[dbo].[NotValued] " +
                                 "WHERE (Dept_Dir LIKE '%" + userName + "%' OR Snr_Manager LIKE '%" + userName + "%' OR Area_Manager LIKE '%" + userName + "%' OR Candidate_DC LIKE '%" + userName + "%') AND Status IN (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID IN (1, 4))"; // Not complete
                
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        DraftId = (int)dr["DraftId"],
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


        //[HttpPost]
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
                com.CommandText = "EXEC AllocatingTask_Procedure @valuerName, @DraftId";

                com.Parameters.AddWithValue("@valuerName", valuerName);

                foreach (int draftId in selectedItems)
                {
                    com.Parameters.Clear();

                    com.Parameters.AddWithValue("@DraftId", draftId);
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

        public IActionResult ViewProperty(string? id)
        {             
            if (drafts.Count > 0)
            {
                drafts.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [CAT_DESC_NAME] FROM [UpdatedGVTool].[dbo].[Category] WHERE ACTIVE = 'Y' ORDER BY [CAT_DESC_NAME]";

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
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[NotValued] WHERE DraftId = '" + id + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        DraftId = (int)dr["DraftId"],
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
                        ApproverComment = dr["approverComment"].ToString()
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

        [HttpPost]
        public async Task<IActionResult> UpdateRevisedValue(string? DraftId, string? PremiseId, string? 
            PropertyDescription, string? MarketValue, string? MarketValue1, string? MarketValue2, string? MarketValue3,
            string? CATDescription , string? CATDescription1, string? CATDescription2, string? CATDescription3, string? Comment, string? WEF_DATE, string? userName, List<IFormFile> files)
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
            string uploadRoot = $"{_config["AppSettings:FileRooTPath"]}";
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
                        FileNameAttach +=  ","+fileName;
                        break;
                    case 4:
                        FileNameAttach +=  ","+fileName;
                        break;
                    case 5:
                        FileNameAttach +=  ","+fileName;
                        break;
                    case 6:
                        FileNameAttach +=  ","+fileName;
                        break;
                    case 7:
                        FileNameAttach +=  ","+fileName;
                        break;
                    case 8:
                        FileNameAttach +=  ","+fileName;
                        break;
                    case 9:
                        FileNameAttach +=  ","+fileName;
                        break;
                    case 10:
                        FileNameAttach +=  ","+fileName;
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

                com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValued] SET [Market Value] = '" + MarketValue + "', [Market Value1] = '" + MarketValue1 + "', [Market Value2] = '" + MarketValue2 + "', [Market Value3] = '" + MarketValue3 + "', [CAT Description] = '" + CATDescription + "', " + "[CAT Description1] = '" + CATDescription1 + "', [CAT Description2] = '" + CATDescription2 + "', [CAT Description3] = '" + CATDescription3 + "', Comment = '" + Comment + "', WEF_DATE = '" + WEF_DATE + "', Activity_Date = getdate(), " +
                                    "FileNameAttach = '" + save_files + "', Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2) WHERE DraftId = '" + DraftId + "'";

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
                com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistory] ([UserName],[UserID],[PropertyDescription]," +
                                  "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Comment],[UserActivity],[Status],[PremiseId],[ActivityDate], [Sector]) " +
                                  "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PropertyDescription + "', '" + MarketValue + "' ,'" + MarketValue1 + "', '" + MarketValue2 + "', '" + MarketValue3 + "', '" + CATDescription + "', '" + CATDescription1 + "', '" + CATDescription2 + "', '" + CATDescription3 + "','" + Comment + "','Updated Values', (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2),'" + PremiseId + "', getdate(), '" + userSector + "')";
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

            return RedirectToAction("PropPerUser", new { userName = userName });
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
                                  "[Comment],[approverComment],[Status],[ActivityDate] FROM [UpdatedGVTool].[dbo].[DraftHistory] WHERE Sector = '" + userSector + "'";
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
                com.CommandText = "SELECT DATEDIFF(DAY,Start_Date, GETDATE()) AS Date_Diff ,* FROM [UpdatedGVTool].[dbo].[NotValued] N " +
                                  "WHERE (N.Dept_Dir LIKE '%" + userName + "%' OR N.Snr_Manager LIKE '%" + userName + "%' OR N.Area_Manager LIKE '%" + userName + "%') AND N.Sector = '" + userSector + "' AND N.Status IN (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID IN(2,6)) " +
                                  "Order By N.Start_Date ASC";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        DraftId = (int)dr["DraftId"],
                        PremiseId = dr["Premise ID"].ToString(),
                        PropertyDescription = dr["PROPERTY_DESC"].ToString(),
                        TownshipDescription = dr["Town Name Description"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        MarketValue = dr["Market Value"].ToString(),
                        CATDescription = dr["CAT Description"].ToString(),
                        ValuationType = dr["Valuation Type"].ToString(),
                        ValuationTypeDescription = dr["Valuation Type Description"].ToString(),
                        WEF_DATE = (DateTime)dr["WEF_DATE"],
                        Extent = dr["Unit Legal Area"].ToString(),
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
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[NotValued] WHERE DraftId = '" + id + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        DraftId = (int)dr["DraftId"],
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

        public IActionResult LAB()
        {
            return View();
        }

        public IActionResult DraftApproval(string? DraftId, string? approverComment, string? approval, string PropertyDescription, string? MarketValue,
             string? MarketValue1, string? MarketValue2, string? MarketValue3,
            string? CATDescription, string? CATDescription1, string? CATDescription2, string? CATDescription3, string? Comment, string? CategoryComment, string? PremiseId)
        {
            var userID = TempData["currentUser"];
            TempData.Keep("currentUser");

            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");

            var currentUserSurname = TempData["currentUserSurname"] as string;
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"] as string;
            TempData.Keep("currentUserFirstname");

            if (string.IsNullOrWhiteSpace(currentUserSurname) || string.IsNullOrWhiteSpace(currentUserFirstname))
            {
                TempData["RefreshMessage"] = $"User Surname or Firstname is missing or blank. Please refresh the page.";
                return RedirectToAction("RefreshMessage"); 
            }

            TempData["PropertyDescription"] = PropertyDescription;

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
                    com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistory] ([UserName],[UserID],[PropertyDescription]," +
                                      "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Comment],[UserActivity],[Status],[approverComment],[ActivityDate], [PremiseId], [Sector]) " +
                                       "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PropertyDescription.Replace("'", "''") + "', '" + MarketValue + "','" + MarketValue1 + "', '" + MarketValue2 + "','" + MarketValue3 + "', " + " '" + CATDescription + "','" + CATDescription1 + "','" + CATDescription2 + "','" + CATDescription3 + "','" + Comment + "','Approved Values', " +
                                      "(SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3),'" + approverComment + "', getdate(), '" + PremiseId + "', '" + userSector + "')";
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
                    com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValued]" +
                                      "SET [Status] = (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3), [approverComment] = '" + approverComment + "', [End_Date] = GETDATE() , [ApproverName] = '" + currentUserFirstname + ' ' + currentUserSurname + "'" +
                                      "WHERE DraftId = '" + DraftId + "'";

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
                    com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistory] ([UserName],[UserID],[PropertyDescription]," +
                                      "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Comment],[UserActivity],[Status],[approverComment],[ActivityDate], [PremiseId], [Sector]) " +
                                      "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PropertyDescription.Replace("'", "''") + "', '" + MarketValue + "', '" + MarketValue1 + "','" + MarketValue2 + "','" + MarketValue3 + "'," + " '" + CATDescription + "','" + CATDescription1 + "','" + CATDescription2 + "','" + CATDescription3 + "', '" + Comment + "','Rejected Values', (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 4),'" + approverComment + "', getdate(), '" + PremiseId + "', '" + userSector + "')";

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
                    com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValued]" +
                                        "SET [Status] = (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 4), [approverComment] = '" + approverComment + "'" +
                                        "WHERE DraftId = '" + DraftId + "'";

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
                    com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistory] ([UserName],[UserID],[PropertyDescription]," +
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
                    com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValued]" +
                                      "SET [Status] = (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 7), [approverComment] = '" + approverComment + "', [End_Date] = GETDATE() , [ApproverName] = '" + currentUserFirstname + ' ' + currentUserSurname + "'" +
                                      "WHERE DraftId = '" + DraftId + "'";

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

        public async Task<IActionResult> KillTask(string? DraftId, string? PremiseId, string?
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
            string uploadRoot = $"{_config["AppSettings:FileRooTPath"]}";
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

                com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValued] SET [Market Value] = '" + MarketValue + "', [Market Value1] = '" + MarketValue1 + "', [Market Value2] = '" + MarketValue2 + "', [Market Value3] = '" + MarketValue3 + "', [CAT Description] = '" + CATDescription + "', " + "[CAT Description1] = '" + CATDescription1 + "', [CAT Description2] = '" + CATDescription2 + "', [CAT Description3] = '" + CATDescription3 + "', Comment = '" + Comment + "', WEF_DATE = '" + WEF_DATE + "', Activity_Date = getdate(), " +
                                  "FileNameAttach = '" + save_files + "', Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 6) WHERE DraftId = '" + DraftId + "'";

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
                com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistory] ([UserName],[UserID],[PropertyDescription]," +
                                  "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Comment],[UserActivity],[Status],[PremiseId],[ActivityDate], [Sector]) " +
                                  "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PropertyDescription + "', '" + MarketValue + "' ,'" + MarketValue1 + "', '" + MarketValue2 + "', '" + MarketValue3 + "', '" + CATDescription + "', '" + CATDescription1 + "', '" + CATDescription2 + "', '" + CATDescription3 + "','" + Comment + "','Kill Values', (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 6),'" + PremiseId + "', getdate(), '" + userSector + "')";
                
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

            return RedirectToAction("PropPerUser", new { userName = userName });
        }

        public IActionResult BulkUploadTest(string? userName, string? Township, string? SchemeName, string? Extent)
        //public IActionResult BulkUploadTownshipView(string? userName)
        {
            if (drafts.Count > 0)
            {
                drafts.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [CAT_DESC_NAME] FROM [UpdatedGVTool].[dbo].[Category]";

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
                com.CommandText = "SELECT DISTINCT [Town Name Description] " +
                                  "FROM [UpdatedGVTool].[dbo].[NotValued]" +
                                  "WHERE (Dept_Dir LIKE '%" + userName + "%' OR Snr_Manager LIKE '%" + userName + "%' OR Area_Manager LIKE '%" + userName + "%') ORDER BY [Town Name Description]";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        TownshipDescription = dr["Town Name Description"].ToString(),
                    });
                }
                con.Close();

                ViewBag.TownshipsList = drafts.ToList();

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
                com.CommandText = "SELECT DISTINCT [Scheme Name] " +
                                  "FROM [UpdatedGVTool].[dbo].[NotValued]" +
                                  "WHERE (Dept_Dir LIKE '%" + userName + "%' OR Snr_Manager LIKE '%" + userName + "%' OR Area_Manager LIKE '%" + userName + "%') ORDER BY [Scheme Name]";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft 
                    {
                        SchemeName = dr["Scheme Name"].ToString(),
                    });
                }
                con.Close();

                ViewBag.SchemeName = drafts.ToList();

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
                com.CommandText = "SELECT DISTINCT [Unit Legal Area] " +
                                  "FROM [UpdatedGVTool].[dbo].[NotValued]" +
                                  "WHERE (Dept_Dir LIKE '%" + userName + "%' OR Snr_Manager LIKE '%" + userName + "%' OR Area_Manager LIKE '%" + userName + "%') ORDER BY [Unit Legal Area]";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        Extent = dr["Unit Legal Area"].ToString(),
                    });
                }
                con.Close();

                ViewBag.Extent = drafts.ToList();

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
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[NotValued]" +
                                  "WHERE ([Town Name Description] LIKE '%" + Township + "%' AND [Scheme Name] LIKE '%" + SchemeName + "%' AND [Unit Legal Area] LIKE '%" + Extent + "%') AND Status IN (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID IN (1, 4))";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        DraftId = (int)dr["DraftId"],
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
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.BulkUploadList = new List<Draft>(); 
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }

        public IActionResult Search(List<int> selectedItems, string? Township, string? SchemeName, string? Extent)
        //public IActionResult BulkUploadTownshipView(string? userName)
        {    
            if (drafts.Count > 0)
            {
                drafts.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[NotValued]" +
                                  "WHERE ([Town Name Description] = '" + Township + "' AND [Scheme Name] = '" + SchemeName + "' AND [Unit Legal Area] = '" + Extent + "') AND Status IN (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID IN (1, 4))";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        DraftId = (int)dr["DraftId"],
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
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.BulkUploadList = drafts.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }                       

            return View();
        }

        public async Task<IActionResult> UpdateBulkValue(string? TownNameDesc, string? PremiseId, string? PropertyDescription, string? MarketValue, string? CATDescription, string? Comment, string? WEF_DATE, string? userName, List<IFormFile> files)
        {     
            var userID = TempData["currentUser"];
            TempData.Keep("currentUser");

            var currentUserSurname = TempData["currentUserSurname"];
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"];
            TempData.Keep("currentUserFirstname");

            TempData["CATDescription"] = CATDescription;
            TempData["WEF_DATE"] = WEF_DATE;
            TempData["PropertyDescription"] = PropertyDescription;

            int count = 0;
            string Town= TownNameDesc;

            
            string uploadRoot = $"{_config["AppSettings:FileRooTPath"]}";
            string folder = uploadRoot + "\\" + Town;
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
            byte[] serializedData = HttpContext.Session.Get("DraftId_Item");
            List<int> appealItem = JsonSerializer.Deserialize<List<int>>(serializedData);

            if (appealItem != null)
            {
                foreach (var draftId in appealItem)
                {

                    try
                    {
                        con.Open();
                        com.Connection = con;
                        com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValued] SET [Market Value] = '" + MarketValue + "'," +
                                    "[CAT Description] = '" + CATDescription + "', Comment = '" + Comment + "', WEF_DATE = '" + WEF_DATE + "', Activity_Date = getdate(), " +
                                    "FileNameAttach = '" + save_files + "', Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2) WHERE DraftId = '" + draftId + "'";
                        dr = com.ExecuteReader();

                        con.Close();

                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex.Message.ToString());               
                        throw ex;
                    }

                    finally
                    {
                        con.Close();
                    }
                }
            }

            //if (drafts.Count > 0)
            //{
            //    drafts.Clear();
            //}
            //try
            //{
            //    con.Open();
            //    com.Connection = con;
            //        //string fileNameAttachValue = (files != null && files.FileName != null) ? Path.GetFileName(files.FileName) : null;
            //    foreach (var draftId in selectedItems)
            //    {
            //        com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValued] SET [Market Value] = '" + MarketValue + "'," +
            //                        "[CAT Description] = '" + CATDescription + "', Comment = '" + Comment + "', WEF_DATE = '" + WEF_DATE + "', Activity_Date = getdate(), " +
            //                        "FileNameAttach = '" + save_files + "', Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2) WHERE DraftId = '" + draftId + "'";


            //        while (dr.Read())
            //        {
            //            drafts.Add(new Draft
            //            {
            //                //DraftId = (Int64)dr["DraftId"],
            //                //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
            //                //RevisedCategory = dr["RevisedCategory"].ToString(),
            //                //CommentMarketValue = dr["CommentMarketValue"].ToString(),
            //                //CommentCategory = dr["CommentCategory"].ToString(),
            //                //Status = dr["Status"].ToString(),
            //            });
            //        }

            //        con.Close();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    con.Close();
            //} 

            if (draftHistories.Count > 0)
            {
                draftHistories.Clear(); 
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistory] ([UserName],[UserID],[PropertyDescription]," +
                                  "[MarketValue],[CATDescription],[Comment],[UserActivity],[Status],[PremiseId],[ActivityDate]) " +
                                  "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + PropertyDescription + "', '" + MarketValue + "'," +
                                  " '" + CATDescription + "','" + Comment + "','Updated Values', (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2),'" + PremiseId + "', getdate())";
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

            return RedirectToAction("BulkUploadTest", new { userName = userName });
        }

        public IActionResult BulkUpdateTownship(string? userName, string? township, string? MarketValue, string? CATDescription, string? Comment, string? CategoryComment)
        {            

            if (drafts.Count > 0)
            {
                drafts.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[NotValued] " +
                                  "SET [Market Value] = '" + MarketValue + "'," +
                                  "[CAT Description] = '" + CATDescription + "'," +
                                  "[Comment] = '" + Comment + "'," +
                                  "WHERE [Town Name Description] = '" + township + "' AND [CAT Description] = '" + CATDescription + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        //DraftId = (int)dr["DraftId"],
                        //UserName = dr["UserName"].ToString(),
                        //PremiseId = dr["PremiseId"].ToString(),
                        //UserId = dr["UserID"].ToString(),
                        //UserActivity = dr["UserActivity"].ToString(),
                        //PropertyDescription = dr["PropertyDescription"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //ApproverComment = dr["approverComment"].ToString(),
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

            if (drafts.Count > 0)
            {
                drafts.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT DISTINCT [Town Name Description] " +
                                  "FROM [UpdatedGVTool].[dbo].[NotValued]" +
                                  "WHERE Dept_Dir LIKE '" + userName + "' OR Snr_Manager LIKE '" + userName + "' OR Area_Manager LIKE '" + userName + "' OR Candidate_DC LIKE '" + userName + "' ORDER BY TownNameDescription"; 

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        TownshipDescription = dr["Town Name Description"].ToString(),

                    });
                }
                con.Close();

                ViewBag.TownshipsList = drafts.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("BulkUploadTownshipView", new { userName = userName });
        }


        public IActionResult BulkUpdate(string? id, List<int> selectedItems)
        {
            if (selectedItems != null && selectedItems.Count > 0)
            {
                byte[] serializedData = JsonSerializer.SerializeToUtf8Bytes(selectedItems);
                HttpContext.Session.Set("DraftId_Item", serializedData);
            }


            List<Draft> drafts = new List<Draft>();
            List<Category> categories = new List<Category>();

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

            drafts.Clear();


            if (drafts.Count > 0)
            {
                drafts.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;

                foreach (var data in selectedItems)
                {
                    // Parameterized query to prevent SQL injection
                    com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[NotValued] WHERE DraftId = @DraftId";
                    com.Parameters.AddWithValue("@DraftId", data);

                    //com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[NotValued] WHERE DraftId = '" + id + "'";

                    using (var dr = com.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            drafts.Add(new Draft
                            {
                                DraftId = (int)dr["DraftId"],
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

                            });
                        }
                    }

                    com.Parameters.Clear();
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

            return View(drafts);
        }

        public IActionResult GetDataFilter(string? Township, string? SchemeName, string? Extent)
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
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[NotValued]" +
                                  "WHERE ([Town Name Description] '%" + Township + "%' AND [Scheme Name] LIKE '%" + SchemeName + "%' AND [Unit Legal Area] LIKE '%" + Extent + "%') AND Status IN (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID IN (1, 4))";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        DraftId = (int)dr["DraftId"],
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

            return PartialView("BulkUploadTest", ViewBag.UserDataList);//drafts
        }


        public IActionResult Privacy()
        {
            return RedirectToAction("BulkUploadTownshipView");
        }

        [HttpGet]
        public FileResult GetFileResultDemo(string filename) 
        {
            string path = @"C:\\Users\\10107037\\Downloads\\" + filename;
            string contentType = SPClass.SPClass.GetContenttype(filename);
            return File(path, contentType);
        }        

        [HttpGet]
        public FileStreamResult GetFileStreamResultDemo(string PremiseID, string filename) //download file 
        {
            string path = Path.GetFileName(@"C:\Users\10107037\Downloads\" + PremiseID + "\" " + filename);
            var stream = new MemoryStream(System.IO.File.ReadAllBytes(path));
            string contentType = SPClass.SPClass.GetContenttype(filename);
            return new FileStreamResult(stream, new MediaTypeHeaderValue(contentType))
            {
                FileDownloadName = filename
            };
        }

        [HttpGet]
        public VirtualFileResult GetVirtualFileResultDemo(string PremiseID, string filename)
        {
            string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"C:\\Users\\10107037\\Downloads\\" + PremiseID + "\\" + filename));
            string contentType = SPClass.SPClass.GetContenttype(filename);
            return new VirtualFileResult(path, contentType);
        }

        [HttpGet]
        public FileContentResult GetFileContentResultDemo(string filename)
        {
            string path = "wwwroot/attachment/" + filename;
            byte[] fileContent = System.IO.File.ReadAllBytes(path);
            string contentType = SPClass.SPClass.GetContenttype(filename);
            return new FileContentResult(fileContent, contentType);
        }        

        public FileResult Download(string ImageName)
        {
            var FileVirtualPath = "~/App_Data/uploads/" + ImageName;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }


        public IActionResult DownloadFiles(string PremiseID) 
        {
            string folderPath = @"E:\\Draft\\" + PremiseID + ""; 
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

        public IActionResult ShowError() 
        {
            return View();   
        }

        public IActionResult RefreshMessage() 
        {
            return View();
        }

        public IActionResult UploadExcel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            int failedUploadCount = 0;
            int successfulUploadCount = 0;
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\";

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            //
                            Dictionary<string, int> columnIndices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                            bool isHeaderSkipped = false;
                            while (reader.Read() && !isHeaderSkipped)
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string? columnName = reader.GetValue(i)?.ToString()?.Trim();
                                    if (!string.IsNullOrEmpty(columnName))
                                    {
                                        columnIndices[columnName] = i;
                                    }
                                }
                                isHeaderSkipped = true;
                            }

                            while (reader.Read())
                            {
                                Draft nt = new Draft();
                                nt.PremiseId = reader.GetValue(columnIndices["Premise ID"])?.ToString();
                                nt.TownshipDescription = reader.GetValue(columnIndices["Town Name Description"])?.ToString();
                                nt.PropertyDescription = reader.GetValue(columnIndices["PROPERTY_DESC"])?.ToString();
                                nt.ValuationType = reader.GetValue(columnIndices["Valuation Type"])?.ToString();
                                nt.ValuationTypeDescription = reader.GetValue(columnIndices["Valuation Type Description"])?.ToString();
                                ParseAndSetDate(reader.GetValue(columnIndices["Valuation Effective Date (Wef Date)"])?.ToString(), out DateTime wefDate);
                                nt.WEF_DATE = wefDate;
                                nt.ValuationStatus = reader.GetValue(columnIndices["Valuation Status"])?.ToString();
                                nt.CATDescription = reader.GetValue(columnIndices["CAT Description"])?.ToString();
                                nt.AllocatedName = reader.GetValue(columnIndices["AllocateName"])?.ToString();
                                nt.Area_Manager = reader.GetValue(columnIndices["Area_Manager"])?.ToString();
                                nt.MarketValue = reader.GetValue(columnIndices["Market Value"])?.ToString();
                                ParseAndSetDate(reader.GetValue(columnIndices["Valuation End Date"])?.ToString(), out DateTime endDate);
                                nt.End_Date = endDate;

                                var existingEnt = _context.Drafts.FirstOrDefault(e => e.PremiseId == nt.PremiseId);
                                if (existingEnt != null)
                                {
                                    ViewBag.Results = "PremiseID in the database already exists";
                                    failedUploadCount++;
                                }
                                else
                                {
                                    _context.Add(nt);
                                    await _context.SaveChangesAsync();
                                    successfulUploadCount++;
                                }
                            }

                            while (reader.NextResult()) ;
                            ViewBag.FailedCount = failedUploadCount;
                            ViewBag.SuccessfulCount = successfulUploadCount;

                            if (failedUploadCount > 0)
                            {
                                ViewBag.Message = "PremiseID in the file already exists";
                            }
                            else
                            {
                                ViewBag.Message = "success";
                                ViewBag.Total = $"You have successfully uploaded {successfulUploadCount} records";
                            }
                        }
                        stream.Close();
                    }
                }
                else
                    ViewBag.Message = "empty";
                return View();
                // return RedirectToAction("Index","NotValueds");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during upload: {ErrorMessage}", ex.Message);
                failedUploadCount++;
            }
            return View();
        }
        private void ParseAndSetDate(string? dateString, out DateTime parsedDate)
        {
            if (!string.IsNullOrEmpty(dateString) && dateString != "NULL")
            {
                if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    return;
                }
            }
            parsedDate = DateTime.MinValue;
        }

        public IActionResult GV13ValuationAdmin()
        {
            if (GV13s.Count > 0)
            {
                GV13s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV13_RV_OUTSTANDING] WHERE Status IS NULL OR STATUS = 'REJECTED'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV13s.Add(new GV13
                    {
                        GV13ID = (int)dr["GV13ID"],
                        PREMISE_ID = dr["PREMISE_ID"].ToString(),
                        ObjectionNumber = dr["ObjectionNumber"].ToString(),
                        ObjectionOutcomeValue = dr["OBJ_OutcomeValue"].ToString(),
                        ObjectionOutcomeCategory = dr["OBJ_OUTCOME_CATEGORY"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["OBJ_OUTCOME_EXTENT"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["TOWN_NAME_DESC"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Indicator = dr["Indicator"].ToString()
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.UserDataList = GV13s.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return View();   
        }

        public IActionResult ViewGV13(string? ID)
        {
            if (GV13s.Count > 0)
            {
                GV13s.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [CAT_DESC] FROM [UpdatedGVTool].[dbo].[Category2013] ORDER BY [CAT_DESC]";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    categories.Add(new Category
                    {
                        CatDescName = dr["CAT_DESC"].ToString(),
                    });
                }
                con.Close();

                ViewBag.CategoriesList = categories.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (GV13s.Count > 0)
            {
                GV13s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV13_RV_OUTSTANDING] WHERE GV13ID = '" + ID + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV13s.Add(new GV13
                    {
                        GV13ID = (int)dr["GV13ID"],
                        PREMISE_ID = dr["PREMISE_ID"].ToString(),
                        ObjectionNumber = dr["ObjectionNumber"].ToString(),
                        ObjectionOutcomeValue = dr["OBJ_OutcomeValue"].ToString(),
                        ObjectionOutcomeCategory = dr["OBJ_OUTCOME_CATEGORY"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["OBJ_OUTCOME_EXTENT"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["TOWN_NAME_DESC"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString()
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.UserDataList = GV13s.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return View(GV13s);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLIS(string? GV13ID, string? PremiseId, string? Property_Description, string? LISMarketValue, string? LISCATDescription, string? LISExtent, string? Comment, string? userName, List<IFormFile> files)
        {
            var userID = TempData["currentUser"];
            TempData.Keep("currentUser");

            var currentUserSurname = TempData["currentUserSurname"] as string;
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"] as string;
            TempData.Keep("currentUserFirstname");

            if (string.IsNullOrWhiteSpace(currentUserSurname) || string.IsNullOrWhiteSpace(currentUserFirstname))
            {
                TempData["RefreshMessage"] = $"User Surname or Firstname is missing or blank. Please refresh the page.";
                return RedirectToAction("RefreshMessage");
            }

            if (GV13s.Count > 0) 
            {
                GV13s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;

                //string fileNameAttachValue = (files != null && files.FileName != null) ? Path.GetFileName(files.FileName) : null;

                com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[GV13_RV_OUTSTANDING] SET [LIS_RV_MARKET_VALUE] = '" + LISMarketValue + "', [Comment] = '" + Comment + "', Activity_Date = getdate()," +
                                  "[LIS_RV_CATEGORY] = '" + LISCATDescription + "', [LIS_RV_EXTENT] = '" + LISExtent + "', Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2) WHERE GV13ID = '" + GV13ID + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV13s.Add(new GV13
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
                com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistoryRV] ([UserName],[UserID],[PropertyDescription]," +
                                  "[MarketValue],[CATDescription],[Comment],[UserActivity],[Status],[PremiseId],[ActivityDate]) " +
                                  "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + Property_Description.Replace("'", "''") + "', '" + LISMarketValue + "'," +
                                  " '" + LISCATDescription + "','" + Comment + "','Updated Values', (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2),'" + PremiseId + "', getdate())";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    draftHistories.Add(new DraftHistory
                    {
                        
                    });
                }
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("GV13ValuationAdmin");
        }

        public IActionResult ApprovalListGV13()
        {
            if (GV13s.Count > 0)
            {
                GV13s.Clear();
            }
            try
            {
                var name = "name's";

                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV13_RV_OUTSTANDING] " +
                                  "WHERE Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2)";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV13s.Add(new GV13
                    {
                        GV13ID = (int)dr["GV13ID"],
                        PREMISE_ID = dr["PREMISE_ID"].ToString(),
                        ObjectionNumber = dr["ObjectionNumber"].ToString(),
                        ObjectionOutcomeValue = dr["OBJ_OutcomeValue"].ToString(),
                        ObjectionOutcomeCategory = dr["OBJ_OUTCOME_CATEGORY"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["OBJ_OUTCOME_EXTENT"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["TOWN_NAME_DESC"].ToString(),
                        Status = dr["Status"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Comment = dr["Comment"].ToString(),
                        Indicator = dr["Indicator"].ToString()
                        //Date = (DateTime)dr["Date"],
                    });
                }
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(GV13s);
        }

        public IActionResult ViewPendingGV13(string? id) 
        {
            if (GV13s.Count > 0)
            {
                GV13s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV13_RV_OUTSTANDING] WHERE [GV13ID] = '" + id + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV13s.Add(new GV13
                    {
                        GV13ID = (int)dr["GV13ID"],
                        PREMISE_ID = dr["PREMISE_ID"].ToString(),
                        ObjectionNumber = dr["ObjectionNumber"].ToString(),
                        ObjectionOutcomeValue = dr["OBJ_OutcomeValue"].ToString(),
                        ObjectionOutcomeCategory = dr["OBJ_OUTCOME_CATEGORY"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["OBJ_OUTCOME_EXTENT"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["TOWN_NAME_DESC"].ToString(),
                        Status = dr["Status"].ToString(),
                        LIS_RV_MARKET_VALUE = dr["LIS_RV_MARKET_VALUE"].ToString(),
                        LIS_RV_CATEGORY = dr["LIS_RV_CATEGORY"].ToString(),
                        LIS_RV_EXTENT = dr["LIS_RV_EXTENT"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString()
                        //Date = (DateTime)dr["Date"],
                    });
                }
                con.Close();


                if (GV13s.Count > 0)
                {
                    foreach (var admin in GV13s)
                    {
                        TempData["TownshipDescription"] = admin.TOWN_NAME_DESC;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(GV13s);
        }
         
        public IActionResult ApprovalGV13(string? GV13ID, string? approverComment, string? approval, string Property_Description, string? LIS_RV_MARKET_VALUE, string? CATDescription, string? Comment, string? CategoryComment, string? PremiseId)
        {
            var userID = TempData["currentUser"];
            TempData.Keep("currentUser");

            var currentUserSurname = TempData["currentUserSurname"] as string;
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"] as string;
            TempData.Keep("currentUserFirstname");

            if (string.IsNullOrWhiteSpace(currentUserSurname) || string.IsNullOrWhiteSpace(currentUserFirstname))
            {
                TempData["RefreshMessage"] = $"User Surname or Firstname is missing or blank. Please refresh the page.";
                return RedirectToAction("RefreshMessage");
            }

            TempData["PropertyDescription"] = Property_Description;

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
                    com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistoryRV] ([UserName],[UserID],[PropertyDescription]," +
                                      "[MarketValue],[CATDescription],[Comment],[UserActivity],[Status],[approverComment],[ActivityDate], [PremiseId]) " +
                                      "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + Property_Description.Replace("'", "''") + "', '" + LIS_RV_MARKET_VALUE + "'," +
                                      "'" + CATDescription + "','" + Comment + "','Approved Values', (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3),'" + approverComment + "', getdate(), '" + PremiseId + "')";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftHistories.Add(new DraftHistory
                        {
                            
                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (GV13s.Count > 0)
                {
                    GV13s.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[GV13_RV_OUTSTANDING]" +
                                      "SET [Status] = (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3), [End_Date] = GETDATE()," +
                                      "[approverComment] = '" + approverComment + "' WHERE GV13ID = '" + GV13ID + "'";

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        GV13s.Add(new GV13
                        { 
                            
                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                TempData["DraftApprovalSuccess"] = "Task is successfully approved.";

                return RedirectToAction("ApprovalListGV13");
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
                    com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistoryRV] ([UserName],[UserID],[PropertyDescription]," +
                                      "[MarketValue],[CATDescription],[Comment],[UserActivity],[Status],[approverComment],[ActivityDate], [PremiseId]) " +
                                      "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + Property_Description.Replace("'", "''") + "', '" + LIS_RV_MARKET_VALUE + "'," +
                                      " '" + CATDescription + "', '" + Comment + "','Rejected Values', (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 4),'" + approverComment + "', getdate(), '" + PremiseId + "')";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftHistories.Add(new DraftHistory
                        {
                            
                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (GV13s.Count > 0)
                {
                    GV13s.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[GV13_RV_OUTSTANDING]" +
                                      "SET [Status] = (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 4)," +
                                      "[approverComment] = '" + approverComment + "' WHERE GV13ID = '" + GV13ID + "'";

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        GV13s.Add(new GV13
                        {
                            
                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                TempData["DraftApprovalSuccess"] = "Task is successfully rejected.";
                return RedirectToAction("ApprovalListGV13");
            }

            return View();
        }

        public IActionResult GV18ValuationAdmin() 
        {
            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV18_RV_OUTSTANDING] WHERE Status IS NULL OR STATUS = 'REJECTED'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV18s.Add(new GV18
                    {
                        GV18ID = (int)dr["GV18ID"],
                        PREMISE_ID = dr["Premise_ID"].ToString(),
                        ObjectionNumber = dr["Objection_Number"].ToString(),
                        ObjectionOutcomeValue = dr["Objection_Outcome_Value"].ToString(),
                        ObjectionOutcomeCategory = dr["Objection_Outcome_Category"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["Objection_Outcome_Extent"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["Town_Name_Description"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Indicator = dr["Indicator"].ToString()
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.UserDataList18 = GV18s.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }


        public IActionResult GV18ValuationAdminPV() 
        {
            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV18_PV_OUTSTANDING] WHERE Status IS NULL OR STATUS = 'REJECTED'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV18s.Add(new GV18
                    {
                        GV18ID = (int)dr["GV18ID"],
                        PREMISE_ID = dr["Premise_ID"].ToString(),
                        ObjectionNumber = dr["Objection_Number"].ToString(),
                        ObjectionOutcomeValue = dr["Objection_Outcome_Value"].ToString(),
                        ObjectionOutcomeCategory = dr["Objection_Outcome_Category"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["Objection_Outcome_Extent"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["Town_Name_Description"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Indicator = dr["Indicator"].ToString()
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.UserDataList18 = GV18s.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }

        public IActionResult ViewGV18(string? ID)  
        {
            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [CAT_DESC] FROM [UpdatedGVTool].[dbo].[Category2018] ORDER BY [CAT_DESC]";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    categories.Add(new Category
                    {
                        CatDescName = dr["CAT_DESC"].ToString(),
                    });
                }
                con.Close();

                ViewBag.CategoriesList18 = categories.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV18_RV_OUTSTANDING] WHERE GV18ID = '" + ID + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV18s.Add(new GV18
                    {
                        GV18ID = (int)dr["GV18ID"],
                        PREMISE_ID = dr["Premise_ID"].ToString(),
                        ObjectionNumber = dr["Objection_Number"].ToString(),
                        ObjectionOutcomeValue = dr["Objection_Outcome_Value"].ToString(),
                        ObjectionOutcomeCategory = dr["Objection_Outcome_Category"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["Objection_Outcome_Extent"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["Town_Name_Description"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Indicator = dr["Indicator"].ToString()
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.UserDataList18 = GV18s.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return View(GV18s);
        }

        public IActionResult ViewGV18PV(string? ID) 
        {
            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [CAT_DESC] FROM [UpdatedGVTool].[dbo].[Category2018] ORDER BY [CAT_DESC]";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    categories.Add(new Category
                    {
                        CatDescName = dr["CAT_DESC"].ToString(),
                    });
                }
                con.Close();

                ViewBag.CategoriesList18 = categories.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV18_PV_OUTSTANDING] WHERE GV18ID = '" + ID + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV18s.Add(new GV18
                    {
                        GV18ID = (int)dr["GV18ID"],
                        PREMISE_ID = dr["Premise_ID"].ToString(),
                        ObjectionNumber = dr["Objection_Number"].ToString(),
                        ObjectionOutcomeValue = dr["Objection_Outcome_Value"].ToString(),
                        ObjectionOutcomeCategory = dr["Objection_Outcome_Category"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["Objection_Outcome_Extent"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["Town_Name_Description"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Indicator = dr["Indicator"].ToString()
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.UserDataList18 = GV18s.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return View(GV18s);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateLIS18(string? GV18ID, string? PremiseId, string? Property_Description, string? LISMarketValue,string? MarketValue1,string? MarketValue2,string? MarketValue3, string? LISCATDescription, string? LIS_RV_WEF_DATE, string? CATDescription1,string? CATDescription2,string? CATDescription3,string? LISExtent, string? Extent1, string? Extent2, string? Extent3, string? Comment, string? userName, List<IFormFile> files)
        {                                                                                                                                                               
            var userID = TempData["currentUser"];                                                                                                                    
            TempData.Keep("currentUser");                                                                                                                            
                                                                                                                                                                     
            var currentUserSurname = TempData["currentUserSurname"] as string;                                                                                                 
            TempData.Keep("currentUserSurname");                                                                                                                     
            var currentUserFirstname = TempData["currentUserFirstname"] as string;                                                                                             
            TempData.Keep("currentUserFirstname");

            if (string.IsNullOrWhiteSpace(currentUserSurname) || string.IsNullOrWhiteSpace(currentUserFirstname))
            {
                TempData["RefreshMessage"] = $"User Surname or Firstname is missing or blank. Please refresh the page.";
                return RedirectToAction("RefreshMessage");
            }

            if (GV18s.Count > 0)                                                                                                                                  
            {
                GV18s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;

                //string fileNameAttachValue = (files != null && files.FileName != null) ? Path.GetFileName(files.FileName) : null;

                com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[GV18_RV_OUTSTANDING] SET [LIS_RV_MARKET_VALUE] = '" + LISMarketValue + "'," + " [MarketValue1] = '" + MarketValue1 + "'," + " [MarketValue2] = '" + MarketValue2 + "', [MarketValue3] = '" + MarketValue3 + "', [Comment] = '" + Comment + "', Activity_Date = getdate()," +
                                  "[LIS_RV_CATEGORY] = '" + LISCATDescription + "', [LIS_RV_WEF_DATE] = '" + LIS_RV_WEF_DATE + "' , [CATDescription1] = '" + CATDescription1 + "'," + " [CATDescription2] = '" + CATDescription2 + "', " + " [CATDescription3] = '" + CATDescription3 + "', [LIS_RV_EXTENT] = '" + LISExtent + "', [Extent1] = '" + Extent1 + "', [Extent2] = '" + Extent2 + "', [Extent3] = '" + Extent3 + "', Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2) WHERE GV18ID = '" + GV18ID + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV18s.Add(new GV18
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
                com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistoryRV18] ([UserName],[UserID],[PropertyDescription]," +
                                  "[MarketValue],[CATDescription],[Comment],[UserActivity],[Status],[PremiseId],[ActivityDate], [MarketValue1], [MarketValue2], [MarketValue3], [CATDescription1],[CATDescription2],[CATDescription3], [Extent1], [Extent2], [Extent3]) " +
                                  "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + Property_Description + "', '" + LISMarketValue + "'," +
                                  " '" + LISCATDescription + "','" + Comment + "','Updated Values', (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2),'" + PremiseId + "', getdate(), '" + MarketValue1 + "', '" + MarketValue2 + "', '" + MarketValue3 + "', '" + CATDescription1 + "', '" + CATDescription2 + "', '" + CATDescription3 + "', '" + Extent1 + "', '" + Extent2 + "', '" + Extent3 + "')";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    draftHistories.Add(new DraftHistory
                    {

                    });
                }
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("GV18ValuationAdmin");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLIS18PV(string? GV18ID, string? PremiseId, string? Property_Description, string? LISMarketValue, string? MarketValue1, string? MarketValue2, string? MarketValue3, string? LISCATDescription, string? LIS_RV_WEF_DATE, string? CATDescription1, string? CATDescription2, string? CATDescription3, string? LISExtent, string? Extent1, string? Extent2, string? Extent3, string? Comment, string? userName, List<IFormFile> files)
        {
            var userID = TempData["currentUser"];
            TempData.Keep("currentUser"); 

            var currentUserSurname = TempData["currentUserSurname"] as string;
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"] as string;
            TempData.Keep("currentUserFirstname");


            if (string.IsNullOrWhiteSpace(currentUserSurname) || string.IsNullOrWhiteSpace(currentUserFirstname))
            {
                TempData["RefreshMessage"] = $"User Surname or Firstname is missing or blank. Please refresh the page.";
                return RedirectToAction("RefreshMessage");
            }

            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;

                //string fileNameAttachValue = (files != null && files.FileName != null) ? Path.GetFileName(files.FileName) : null;

                com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[GV18_PV_OUTSTANDING] SET [LIS_RV_MARKET_VALUE] = '" + LISMarketValue + "'," + " [MarketValue1] = '" + MarketValue1 + "'," + " [MarketValue2] = '" + MarketValue2 + "', [MarketValue3] = '" + MarketValue3 + "', [Comment] = '" + Comment + "', Activity_Date = getdate()," +
                                  "[LIS_RV_CATEGORY] = '" + LISCATDescription + "', [LIS_RV_WEF_DATE] = '" + LIS_RV_WEF_DATE + "' , [CATDescription1] = '" + CATDescription1 + "'," + " [CATDescription2] = '" + CATDescription2 + "', " + " [CATDescription3] = '" + CATDescription3 + "', [LIS_RV_EXTENT] = '" + LISExtent + "', [Extent1] = '" + Extent1 + "', [Extent2] = '" + Extent2 + "', [Extent3] = '" + Extent3 + "', Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2) WHERE GV18ID = '" + GV18ID + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV18s.Add(new GV18
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
                com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistoryPV18] ([UserName],[UserID],[PropertyDescription]," +
                                  "[MarketValue],[CATDescription],[Comment],[UserActivity],[Status],[PremiseId],[ActivityDate], [MarketValue1], [MarketValue2], [MarketValue3], [CATDescription1],[CATDescription2],[CATDescription3], [Extent1], [Extent2], [Extent3]) " +
                                  "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + Property_Description + "', '" + LISMarketValue + "'," +
                                  " '" + LISCATDescription + "','" + Comment + "','Updated Values', (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2),'" + PremiseId + "', getdate(), '" + MarketValue1 + "', '" + MarketValue2 + "', '" + MarketValue3 + "', '" + CATDescription1 + "', '" + CATDescription2 + "', '" + CATDescription3 + "', '" + Extent1 + "', '" + Extent2 + "', '" + Extent3 + "')";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    draftHistories.Add(new DraftHistory
                    {

                    });
                }
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("GV18ValuationAdminPV");
        }


        public IActionResult ApprovalListGV18() 
        {
            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV18_RV_OUTSTANDING] " +
                                  "WHERE Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2)";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV18s.Add(new GV18
                    {
                        GV18ID = (int)dr["GV18ID"],
                        PREMISE_ID = dr["Premise_ID"].ToString(),
                        ObjectionNumber = dr["Objection_Number"].ToString(),
                        ObjectionOutcomeValue = dr["Objection_Outcome_Value"].ToString(),
                        ObjectionOutcomeCategory = dr["Objection_Outcome_Category"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["Objection_Outcome_Extent"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["Town_Name_Description"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Status = dr["Status"].ToString(),
                        Comment = dr["Comment"].ToString(),
                        Indicator = dr["Indicator"].ToString()
                        //Date = (DateTime)dr["Date"],
                    });
                }
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(GV18s);
        }

        public IActionResult ApprovalListGV18PV() 
        {
            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV18_PV_OUTSTANDING] " +
                                  "WHERE Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 2)";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV18s.Add(new GV18
                    {
                        GV18ID = (int)dr["GV18ID"],
                        PREMISE_ID = dr["Premise_ID"].ToString(),
                        ObjectionNumber = dr["Objection_Number"].ToString(),
                        ObjectionOutcomeValue = dr["Objection_Outcome_Value"].ToString(),
                        ObjectionOutcomeCategory = dr["Objection_Outcome_Category"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["Objection_Outcome_Extent"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["Town_Name_Description"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Status = dr["Status"].ToString(),
                        Comment = dr["Comment"].ToString(),
                        Indicator = dr["Indicator"].ToString()
                        //Date = (DateTime)dr["Date"],
                    });
                }
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(GV18s);
        }


        public IActionResult ViewPendingGV18(string? id)
        {
            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV18_RV_OUTSTANDING] WHERE [GV18ID] = '" + id + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV18s.Add(new GV18
                    {
                        GV18ID = (int)dr["GV18ID"],
                        PREMISE_ID = dr["Premise_ID"].ToString(),
                        ObjectionNumber = dr["Objection_Number"].ToString(),
                        ObjectionOutcomeValue = dr["Objection_Outcome_Value"].ToString(),
                        ObjectionOutcomeCategory = dr["Objection_Outcome_Category"].ToString(),
                        CATDescription1 = dr["CATDescription1"].ToString(),
                        CATDescription2 = dr["CATDescription2"].ToString(),
                        CATDescription3 = dr["CATDescription3"].ToString(),
                        MarketValue1 = dr["MarketValue1"].ToString(),
                        MarketValue2 = dr["MarketValue2"].ToString(),
                        MarketValue3 = dr["MarketValue3"].ToString(),
                        Extent1 = dr["Extent1"].ToString(),
                        Extent2 = dr["Extent2"].ToString(),
                        Extent3 = dr["Extent3"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["Objection_Outcome_Extent"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["Town_Name_Description"].ToString(),
                        Status = dr["Status"].ToString(),
                        LIS_RV_MARKET_VALUE = dr["LIS_RV_MARKET_VALUE"].ToString(),
                        LIS_RV_CATEGORY = dr["LIS_RV_CATEGORY"].ToString(),
                        LIS_RV_EXTENT = dr["LIS_RV_EXTENT"].ToString(),
                        LIS_RV_WEF_DATE = dr["LIS_RV_WEF_DATE"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Indicator = dr["Indicator"].ToString()
                        //Date = (DateTime)dr["Date"],
                    });
                }
                con.Close();


                if (GV18s.Count > 0)
                {
                    foreach (var admin in GV18s)
                    {
                        TempData["TownshipDescription"] = admin.TOWN_NAME_DESC;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(GV18s);
        }

        public IActionResult ViewPendingGV18PV(string? id)
        {
            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV18_PV_OUTSTANDING] WHERE [GV18ID] = '" + id + "'";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV18s.Add(new GV18
                    {
                        GV18ID = (int)dr["GV18ID"],
                        PREMISE_ID = dr["Premise_ID"].ToString(),
                        ObjectionNumber = dr["Objection_Number"].ToString(),
                        ObjectionOutcomeValue = dr["Objection_Outcome_Value"].ToString(),
                        ObjectionOutcomeCategory = dr["Objection_Outcome_Category"].ToString(),
                        CATDescription1 = dr["CATDescription1"].ToString(),
                        CATDescription2 = dr["CATDescription2"].ToString(),
                        CATDescription3 = dr["CATDescription3"].ToString(),
                        MarketValue1 = dr["MarketValue1"].ToString(),
                        MarketValue2 = dr["MarketValue2"].ToString(),
                        MarketValue3 = dr["MarketValue3"].ToString(),
                        Extent1 = dr["Extent1"].ToString(),
                        Extent2 = dr["Extent2"].ToString(),
                        Extent3 = dr["Extent3"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["Objection_Outcome_Extent"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["Town_Name_Description"].ToString(),
                        Status = dr["Status"].ToString(),
                        LIS_RV_MARKET_VALUE = dr["LIS_RV_MARKET_VALUE"].ToString(),
                        LIS_RV_CATEGORY = dr["LIS_RV_CATEGORY"].ToString(),
                        LIS_RV_EXTENT = dr["LIS_RV_EXTENT"].ToString(),
                        LIS_RV_WEF_DATE = dr["LIS_RV_WEF_DATE"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Indicator = dr["Indicator"].ToString()
                        //Date = (DateTime)dr["Date"],
                    });
                }
                con.Close();


                if (GV18s.Count > 0)
                {
                    foreach (var admin in GV18s)
                    {
                        TempData["TownshipDescription"] = admin.TOWN_NAME_DESC;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(GV18s);
        }

        public IActionResult ApprovalGV18(string? GV18ID, string? approverComment, string? approval, string Property_Description, string? LIS_RV_MARKET_VALUE, string? MarketValue1, string? MarketValue2, string? MarketValue3, string? CATDescription, 
            string? CATDescription1, string? CATDescription2, string? CATDescription3, string? Extent1, string? Extent2, string? Extent3 , string? Comment, string? CategoryComment, string? PremiseId)
        {
            var userID = TempData["currentUser"]; 
            TempData.Keep("currentUser"); 

            var currentUserSurname = TempData["currentUserSurname"] as string;
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"] as string;
            TempData.Keep("currentUserFirstname");

            if (string.IsNullOrWhiteSpace(currentUserSurname) || string.IsNullOrWhiteSpace(currentUserFirstname))
            {
                TempData["RefreshMessage"] = $"User Surname or Firstname is missing or blank. Please refresh the page.";
                return RedirectToAction("RefreshMessage");
            }

            TempData["PropertyDescription"] = Property_Description;

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
                    com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistoryRV18] ([UserName],[UserID],[PropertyDescription]," +
                                      "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Extent1], [Extent2], [Extent3] ,[Comment],[UserActivity],[Status],[approverComment],[ActivityDate], [PremiseId]) " +
                                      "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + Property_Description + "', '" + LIS_RV_MARKET_VALUE + "','" + MarketValue1 + "','" + MarketValue2 + "','" + MarketValue3 + "', " + " '" + CATDescription + "','" + CATDescription1 + "','" + CATDescription2 + "','" + CATDescription3 + "', " +
                                      "'" + Extent1 + "', '" + Extent2 + "', '" + Extent3 + "' , '" + Comment + "','Approved Values', (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3),'" + approverComment + "', getdate(), '" + PremiseId + "')";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftHistories.Add(new DraftHistory
                        {

                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (GV18s.Count > 0)
                {
                    GV18s.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[GV18_RV_OUTSTANDING]" +
                                      "SET [Status] = (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3), [End_Date] = GETDATE()," +
                                      "[approverComment] = '" + approverComment + "' WHERE GV18ID = '" + GV18ID + "'";

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        GV18s.Add(new GV18
                        {

                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                TempData["DraftApprovalSuccess"] = "Task is successfully approved.";

                return RedirectToAction("ApprovalListGV18");
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
                    com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistoryRV18] ([UserName],[UserID],[PropertyDescription]," +
                                    "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Extent1], [Extent2], [Extent3], [Comment],[UserActivity],[Status],[approverComment],[ActivityDate], [PremiseId]) " +
                                    "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + Property_Description + "', '" + LIS_RV_MARKET_VALUE + "','" + MarketValue1 + "','" + MarketValue2 + "','" + MarketValue3 + "', " + " '" + CATDescription + "','" + CATDescription1 + "','" + CATDescription2 + "','" + CATDescription3 + "', '" + Extent1 + "', '" + Extent2 + "', '" + Extent3 + "' ,'" + Comment + "','Approved Values', (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 4),'" + approverComment + "', getdate(), '" + PremiseId + "')";

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftHistories.Add(new DraftHistory
                        {

                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (GV18s.Count > 0)
                {
                    GV18s.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[GV18_RV_OUTSTANDING]" +
                                      "SET [Status] = (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 4)," +
                                      "[approverComment] = '" + approverComment + "' WHERE GV18ID = '" + GV18ID + "'";

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        GV18s.Add(new GV18
                        {

                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                TempData["DraftApprovalSuccess"] = "Task is successfully rejected.";
                return RedirectToAction("ApprovalListGV18");
            }

            return View();
        }


        public IActionResult ApprovalGV18PV(string? GV18ID, string? approverComment, string? approval, string Property_Description, string? LIS_RV_MARKET_VALUE, string? MarketValue1, string? MarketValue2, string? MarketValue3, string? CATDescription,
            string? CATDescription1, string? CATDescription2, string? CATDescription3, string? Extent1, string? Extent2, string? Extent3, string? Comment, string? CategoryComment, string? PremiseId)
        {
            var userID = TempData["currentUser"];
            TempData.Keep("currentUser");

            var currentUserSurname = TempData["currentUserSurname"] as string;
            TempData.Keep("currentUserSurname");
            var currentUserFirstname = TempData["currentUserFirstname"] as string; 
            TempData.Keep("currentUserFirstname");

            if (string.IsNullOrWhiteSpace(currentUserSurname) || string.IsNullOrWhiteSpace(currentUserFirstname))
            {
                TempData["RefreshMessage"] = $"User Surname or Firstname is missing or blank. Please refresh the page.";
                return RedirectToAction("RefreshMessage");
            }

            TempData["PropertyDescription"] = Property_Description;

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
                    com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistoryPV18] ([UserName],[UserID],[PropertyDescription]," +
                                      "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Extent1], [Extent2], [Extent3] ,[Comment],[UserActivity],[Status],[approverComment],[ActivityDate], [PremiseId]) " +
                                      "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + Property_Description + "', '" + LIS_RV_MARKET_VALUE + "','" + MarketValue1 + "','" + MarketValue2 + "','" + MarketValue3 + "', " + " '" + CATDescription + "','" + CATDescription1 + "','" + CATDescription2 + "','" + CATDescription3 + "', " +
                                      "'" + Extent1 + "', '" + Extent2 + "', '" + Extent3 + "' , '" + Comment + "','Approved Values', (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3),'" + approverComment + "', getdate(), '" + PremiseId + "')";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftHistories.Add(new DraftHistory
                        {

                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (GV18s.Count > 0)
                {
                    GV18s.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[GV18_PV_OUTSTANDING]" +
                                      "SET [Status] = (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3), [End_Date] = GETDATE()," +
                                      "[approverComment] = '" + approverComment + "' WHERE GV18ID = '" + GV18ID + "'";

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        GV18s.Add(new GV18
                        {

                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                TempData["DraftApprovalSuccess"] = "Task is successfully approved.";

                return RedirectToAction("ApprovalListGV18PV");
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
                    com.CommandText = "INSERT INTO [UpdatedGVTool].[dbo].[DraftHistoryPV18] ([UserName],[UserID],[PropertyDescription]," +
                                    "[MarketValue],[MarketValue1],[MarketValue2],[MarketValue3],[CATDescription],[CATDescription1],[CATDescription2],[CATDescription3],[Extent1], [Extent2], [Extent3], [Comment],[UserActivity],[Status],[approverComment],[ActivityDate], [PremiseId]) " +
                                    "VALUES('" + currentUserFirstname + ' ' + currentUserSurname + "', '" + userID + "', '" + Property_Description + "', '" + LIS_RV_MARKET_VALUE + "','" + MarketValue1 + "','" + MarketValue2 + "','" + MarketValue3 + "', " + " '" + CATDescription + "','" + CATDescription1 + "','" + CATDescription2 + "','" + CATDescription3 + "', '" + Extent1 + "', '" + Extent2 + "', '" + Extent3 + "' ,'" + Comment + "','Approved Values', (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 4),'" + approverComment + "', getdate(), '" + PremiseId + "')";

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftHistories.Add(new DraftHistory
                        {

                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (GV18s.Count > 0)
                {
                    GV18s.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "UPDATE [UpdatedGVTool].[dbo].[GV18_PV_OUTSTANDING]" +
                                      "SET [Status] = (SELECT [Status_Description] FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 4)," +
                                      "[approverComment] = '" + approverComment + "' WHERE GV18ID = '" + GV18ID + "'";

                    dr = com.ExecuteReader(); 
                    while (dr.Read())
                    {
                        GV18s.Add(new GV18
                        {

                        });
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                TempData["DraftApprovalSuccess"] = "Task is successfully rejected.";
                return RedirectToAction("ApprovalListGV18PV");
            }

            return View();
        }

        public IActionResult GV13ValuationApproved()
        {
            if (GV13s.Count > 0)
            {
                GV13s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV13_RV_OUTSTANDING] WHERE Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3)";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV13s.Add(new GV13
                    {
                        GV13ID = (int)dr["GV13ID"],
                        PREMISE_ID = dr["PREMISE_ID"].ToString(),
                        ObjectionNumber = dr["ObjectionNumber"].ToString(),
                        ObjectionOutcomeValue = dr["OBJ_OutcomeValue"].ToString(),
                        ObjectionOutcomeCategory = dr["OBJ_OUTCOME_CATEGORY"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["OBJ_OUTCOME_EXTENT"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["TOWN_NAME_DESC"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Status = dr["Status"].ToString()
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.UserDataList = GV13s.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }

        public IActionResult GV18ValuationApproved() 
        {
            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV18_RV_OUTSTANDING] WHERE Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3)";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV18s.Add(new GV18
                    {
                        GV18ID = (int)dr["GV18ID"],
                        PREMISE_ID = dr["Premise_ID"].ToString(),
                        ObjectionNumber = dr["Objection_Number"].ToString(),
                        ObjectionOutcomeValue = dr["Objection_Outcome_Value"].ToString(),
                        ObjectionOutcomeCategory = dr["Objection_Outcome_Category"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["Objection_Outcome_Extent"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["Town_Name_Description"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Indicator = dr["Indicator"].ToString(),
                        Status = dr["Status"].ToString()
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.UserDataList18 = GV18s.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }

        public IActionResult GV18ValuationPVApproved() 
        {
            if (GV18s.Count > 0)
            {
                GV18s.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM [UpdatedGVTool].[dbo].[GV18_PV_OUTSTANDING] WHERE Status = (SELECT Status_Description FROM [UpdatedGVTool].[dbo].[Status] WHERE Status_ID = 3)";

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GV18s.Add(new GV18
                    {
                        GV18ID = (int)dr["GV18ID"],
                        PREMISE_ID = dr["Premise_ID"].ToString(),
                        ObjectionNumber = dr["Objection_Number"].ToString(),
                        ObjectionOutcomeValue = dr["Objection_Outcome_Value"].ToString(),
                        ObjectionOutcomeCategory = dr["Objection_Outcome_Category"].ToString(),
                        //MarketValue = Convert.ToSingle(dr["MarketValue"]),
                        ObjectionOutcomeExtent = dr["Objection_Outcome_Extent"].ToString(),
                        //RevisedMarketValue = dr["RevisedMarketValue"].ToString(),
                        //RevisedCategory = dr["RevisedCategory"].ToString(),
                        //CommentMarketValue = dr["CommentMarketValue"].ToString(),
                        //CommentCategory = dr["CommentCategory"].ToString(),
                        //Comment = dr["Comment"].ToString(),
                        //FlagForDelete = (bool)dr["FlagForDelete"],
                        //AssignedValuer = dr["AssignedValuer"].ToString(),
                        // BulkUpload = dr["BulkUpload"].ToString(),
                        TOWN_NAME_DESC = dr["Town_Name_Description"].ToString(),
                        Property_Description = dr["PROPERTY_DESCRIPTION"].ToString(),
                        Indicator = dr["Indicator"].ToString(),
                        Status = dr["Status"].ToString()
                        //Date = (DateTime)dr["Date"],

                    });
                }
                con.Close();

                ViewBag.UserDataList18 = GV18s.ToList();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }

        public IActionResult Stats(string status)
        {
            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");

            if (status == null)
            {
                status = "Approved";
            }
            ViewBag.Message = status;

            if (drafts.Count > 0)
            {
                drafts.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT COUNT(Status) as Number_Of_Approved, [Valuation Type Description], [Unit Type] FROM [UpdatedGVTool].[dbo].[NotValued] where Sector = '" + userSector + "' and Status = '" + status + "' group by [Valuation Type Description],[Unit Type]";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        ValuationTypeDescription = dr["Valuation Type Description"].ToString(),
                        Number_Of_Approved = (int)dr["Number_Of_Approved"],
                        Unit_Type = dr["Unit Type"].ToString()
                    });                        
                }
                con.Close();

                ViewBag.SchemeName = drafts.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(drafts);
        }

        public IActionResult Stats78(string status)
        {
            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");

            if (status == null)
            {
                status = "Approved";
            }

            ViewBag.Message = status;

            if (drafts.Count > 0)
            {
                drafts.Clear();
            }

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT COUNT(Status) as Number_Of_Approved, [Valuation Type Description], [Unit Type] FROM [UpdatedGVTool].[dbo].[NotValuedSection78] where Sector = '" + userSector + "' and Status = '" + status + "' group by [Valuation Type Description],[Unit Type]";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    drafts.Add(new Draft
                    {
                        ValuationTypeDescription = dr["Valuation Type Description"].ToString(),
                        Number_Of_Approved = (int)dr["Number_Of_Approved"],
                        Unit_Type = dr["Unit Type"].ToString()
                    });
                }
                con.Close();

                ViewBag.SchemeName = drafts.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(drafts);
        }

        public IActionResult Details()
        {
            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");
            // dataTable
            if (notvalued.Count > 0)
            {
                notvalued.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [Premise ID],[PROPERTY_DESC],[Valuation Type Description],[Market Value],[CAT Description],[Unit Legal Area],[Status],[Start_Date],[End_Date], [AllocateName],[ApproverName] " +
                                  "FROM [UpdatedGVTool].[dbo].[NotValued] " +
                                  "WHERE Sector = '" + userSector + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    notvalued.Add(new NotValued
                    {
                        Premise_ID = dr["Premise ID"].ToString(),
                        PROPERTY_DESC = dr["PROPERTY_DESC"].ToString(),
                        Valuation_Type_Description = dr["Valuation Type Description"].ToString(),
                        Market_Value = dr["Market Value"].ToString(),
                        Cat_Description = dr["CAT Description"].ToString(),
                        Unit_Legal_Area = dr["Unit Legal Area"].ToString(),
                        Status = dr["Status"].ToString(),
                        Start_Date = dr["Start_Date"].ToString(),
                        End_Date = dr["End_Date"].ToString(),
                        AllocatedName = dr["AllocateName"].ToString(),
                        Approver = dr["ApproverName"].ToString()
                    });
                }
                con.Close();
                ViewBag.AllProp = notvalued.ToList();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(ViewBag.AllProp);
        }

        public IActionResult Details78()
        {
            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");
            // dataTable
            if (notvalued.Count > 0)
            {
                notvalued.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [Premise ID],[PROPERTY_DESC],[Valuation Type Description],[Market Value],[CAT Description],[Unit Legal Area],[Status],[Start_Date],[End_Date],[AllocateName],[ApproverName] " +
                                  "FROM [UpdatedGVTool].[dbo].[NotValuedSection78]" +
                                  "WHERE Sector = '" + userSector + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    notvalued.Add(new NotValued
                    {
                        Premise_ID = dr["Premise ID"].ToString(),
                        PROPERTY_DESC = dr["PROPERTY_DESC"].ToString(),
                        Valuation_Type_Description = dr["Valuation Type Description"].ToString(),
                        Market_Value = dr["Market Value"].ToString(),
                        Cat_Description = dr["CAT Description"].ToString(),
                        Unit_Legal_Area = dr["Unit Legal Area"].ToString(),
                        Status = dr["Status"].ToString(),
                        Start_Date = dr["Start_Date"].ToString(),
                        End_Date = dr["End_Date"].ToString(),
                        AllocatedName = dr["AllocateName"].ToString(),
                        Approver = dr["ApproverName"].ToString()
                    });
                }
                con.Close();
                ViewBag.AllProp78 = notvalued.ToList();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(ViewBag.AllProp78);
        }

        public IActionResult CombinedDetails() 
        {
            var userSector = TempData["currentUserSector"];
            TempData.Keep("currentUserSector");
            // dataTable
            if (notvalued.Count > 0)
            {
                notvalued.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [Town Name Description] ,[Premise ID],[PROPERTY_DESC],[Sector],[Valuation Type Description],[Market Value],[CAT Description],[Unit Legal Area],[Status],[Start_Date],[End_Date],[AllocateName],[ApproverName] FROM [UpdatedGVTool].[dbo].[NotValued] " +
                                  "WHERE Sector = '" + userSector + "' UNION ALL SELECT [Town Name Description], [Premise ID],[PROPERTY_DESC],[Sector],[Valuation Type Description],[Market Value],[CAT Description],[Unit Legal Area],[Status],[Start_Date],[End_Date],[AllocateName],[ApproverName] FROM [UpdatedGVTool].[dbo].[NotValuedSection78] where Sector = '" + userSector + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    notvalued.Add(new NotValued
                    {
                        Premise_ID = dr["Premise ID"].ToString(),
                        PROPERTY_DESC = dr["PROPERTY_DESC"].ToString(),
                        Valuation_Type_Description = dr["Valuation Type Description"].ToString(),
                        Market_Value = dr["Market Value"].ToString(),
                        Cat_Description = dr["CAT Description"].ToString(),
                        Unit_Legal_Area = dr["Unit Legal Area"].ToString(),
                        Status = dr["Status"].ToString(),
                        Start_Date = dr["Start_Date"].ToString(),
                        End_Date = dr["End_Date"].ToString(),
                        AllocatedName = dr["AllocateName"].ToString(),
                        Approver = dr["ApproverName"].ToString()
                    });
                }
                con.Close();
                ViewBag.AllProp78 = notvalued.ToList();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(ViewBag.AllProp78);
        }

    }
}