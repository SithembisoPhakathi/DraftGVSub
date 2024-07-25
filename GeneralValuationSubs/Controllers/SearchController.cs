using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GeneralValuationSubs.Models;

namespace GeneralValuationSubs.Controllers
{
    public class SearchController : Controller
    {
        private readonly UpdatedGvtoolContext _context;
        private readonly RoleManager<IdentityRole> directorRole;
        SqlCommand com = new SqlCommand();
        List<AdminValuer> adminValuers = new();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        List<DraftGv> draftGvs = new List<DraftGv>();
        List<DraftGv> township = new List<DraftGv>();
        private readonly ILogger<DraftGvsController> _logger;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public SearchController(UpdatedGvtoolContext context, ILogger<DraftGvsController> logger, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
            con.ConnectionString = _config.GetConnectionString("DefaultConnection");
        }

       

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string PremiseID, string Township, int StandNo, string? myroles, string? useremail)
        {
            ViewData["PremiseID"] = PremiseID;
            ViewData["Town"] = Township;
            ViewData["StandNo"] = StandNo;

                      

            if (myroles == "Deputy Director")
            {
                return _context.DraftGvs != null ?
                       View("_Gridview", await _context.DraftGvs.Where(a => a.DeputyDirector == useremail && (a.PremiseId == PremiseID || a.Town.Contains(Township) || a.LislegalArea == StandNo)).Take(int.MaxValue).OrderByDescending(m => m.DateTime).ToListAsync()) :
                       Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Area Manager")
            {
                return _context.DraftGvs != null ?
                     View("_Gridview", await _context.DraftGvs.Where(a => a.AreaManager == useremail && (a.PremiseId == PremiseID || a.Town.Contains(Township) || a.LislegalArea == StandNo)).Take(int.MaxValue).OrderByDescending(m => m.DateTime).ToListAsync()) :
                     Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Senior Manager")
            {
                return _context.DraftGvs != null ?
                        View("_Gridview", await _context.DraftGvs.Where(a => a.SeniorManager == useremail && (a.PremiseId == PremiseID || a.Town.Contains(Township) || a.LislegalArea == StandNo)).Take(int.MaxValue).OrderByDescending(m => m.DateTime).ToListAsync()) :
                        Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Valuer")
            {
                return _context.DraftGvs != null ?
                        View("_Gridview", await _context.DraftGvs.Where(a => a.Valuer == useremail && (a.PremiseId == PremiseID || a.Town.Contains(Township) || a.LislegalArea == StandNo)).Take(int.MaxValue).OrderByDescending(m => m.DateTime).ToListAsync()) :
                        Problem("Database could not open, please refer to Admin.");
            }
            return NotFound();
            Problem("User does not have a role");

            //if (myroles == "Deputy Director")
            //{
            //    if (township.Count > 0)
            //    {
            //        township.Clear();
            //    }
            //    try
            //    {
            //        con.Open();
            //        com.Connection = con;
            //        com.CommandText = "SELECT * FROM [GV23DEV].[dbo].[DraftGV] where DeputyDirector = '" + useremail + "'";
            //        dr = com.ExecuteReader();
            //        while (dr.Read())
            //        {
            //            township.Add(new DraftGv
            //            {
            //                Id = (int)dr["ID"],
            //                PremiseId = dr["PremiseId"].ToString(),
            //                Town = dr["Town"].ToString(),
            //                ValueRevised = (dr["ValueRevised"] as int?).GetValueOrDefault(),
            //            });
            //        }
            //        con.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        //Console.WriteLine(ex.Message.ToString());
            //        //"SELECT distinct town FROM [GV23DEV].[dbo].[DraftGV] where Sector = (SELECT distinct Sector FROM [GV23DEV].[dbo].[DraftGV] where DeputyDirector = '" + useremail + "') order by Town";
            //        throw ex;
            //    }
            //    return PartialView("_Gridview", township);
            //}

        }

        public async Task<IActionResult> _Gridview(string Township) 
        {
            //ViewData["PremiseID"] = PremiseID;
            ViewData["Town"] = Township;

            var myroles = this.User.FindFirstValue(ClaimTypes.Role);
            string useremail = this.User.FindFirstValue(ClaimTypes.Email);

            if (myroles == "Deputy Director")
            {
                return _context.DraftGvs != null ?
                       View("_Gridview", await _context.DraftGvs.Where(a => a.DeputyDirector == useremail && a.Town.Contains(Township)).Take(int.MaxValue).OrderByDescending(m => m.DateTime).ToListAsync()) :
                       Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Area Manager")
            {
                return _context.DraftGvs != null ?
                     View("_Gridview", await _context.DraftGvs.Where(a => a.AreaManager == useremail && a.Town.Contains(Township)).Take(int.MaxValue).OrderByDescending(m => m.DateTime).ToListAsync()) :
                     Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Senior Manager")
            {
                return _context.DraftGvs != null ?
                        View("_Gridview", await _context.DraftGvs.Where(a => a.SeniorManager == useremail && a.Town.Contains(Township)).Take(int.MaxValue).OrderByDescending(m => m.DateTime).ToListAsync()) :
                        Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Valuer")
            {
                return _context.DraftGvs != null ?
                        View("_Gridview", await _context.DraftGvs.Where(a => a.Valuer == useremail && a.Town.Contains(Township)).Take(int.MaxValue).OrderByDescending(m => m.DateTime).ToListAsync()) :
                        Problem("Database could not open, please refer to Admin.");
            }
            return NotFound();
            Problem("User does not have a role");

        }        

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DraftGvs == null)
            {
                return NotFound();
            }

            var draftGv = await _context.DraftGvs.FindAsync(id);

            if (draftGv == null)
            {
                return NotFound();
            }
            return View(draftGv);
        }

        // POST: DraftGvs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DraftGv draftGv)
        {
            DraftGvaudit DraftGvaudit = new DraftGvaudit();

            //etc for your other entities

            draftGv.DateTime = DateTime.Now;
            draftGv.UserName = this.User.FindFirstValue(ClaimTypes.Email);
            DraftGvaudit.DraftGvid = draftGv.Id;
            DraftGvaudit.PremiseId = draftGv.PremiseId;
            DraftGvaudit.Town = draftGv.Town;
            DraftGvaudit.Ddarea = draftGv.Ddarea;
            DraftGvaudit.CalculatedValue = draftGv.CalculatedValue;
            DraftGvaudit.ValuersValue = draftGv.ValuersValue;
            DraftGvaudit.DraftValue = draftGv.DraftValue;
            DraftGvaudit.Gv18marketValue = draftGv.Gv18marketValue;
            DraftGvaudit.Gv18category = draftGv.Gv18category;
            DraftGvaudit.PurchaseDate = draftGv.PurchaseDate;
            DraftGvaudit.PurchasePrice = draftGv.PurchasePrice;
            DraftGvaudit.PnCproperty = draftGv.PnCproperty;
            DraftGvaudit.ParentPremiseId = draftGv.ParentPremiseId;
            DraftGvaudit.ValueOverride = draftGv.ValueOverride;
            DraftGvaudit.CommentValue = draftGv.CommentValue;
            DraftGvaudit.ValueRevised = draftGv.ValueRevised;
            DraftGvaudit.CategoryOverride = draftGv.CategoryOverride;
            DraftGvaudit.CategoryRevised = draftGv.CategoryRevised;
            DraftGvaudit.CommentCategory = draftGv.CommentCategory;
            DraftGvaudit.FlagDelete = draftGv.FlagDelete;
            DraftGvaudit.CommentDelete = draftGv.CommentDelete;
            DraftGvaudit.DateTime = draftGv.DateTime;
            DraftGvaudit.UserName = draftGv.UserName;
            DraftGvaudit.Status = draftGv.Status;
            DraftGvaudit.ZoneCode = draftGv.ZoneCode;
            DraftGvaudit.LislegalArea = draftGv.LislegalArea;
            DraftGvaudit.AdjustedWgba = draftGv.AdjustedWgba;
            DraftGvaudit.DiffsGv23vsGv18 = draftGv.DiffsGv23vsGv18;

            if (id != draftGv.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(DraftGvaudit);
                    _context.Update(draftGv);

                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"{draftGv.PremiseId} has been updated successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DraftGvExists(draftGv.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                        //TempData["Message"] = $"unable to update {draftGv.PremiseId}";
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(draftGv);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DraftGvs == null)
            {
                return NotFound();
            }

            DraftGvViewModel gvViewModel = new DraftGvViewModel
            {
                draftGv = _context.DraftGvs.Where(a => a.Id == id).ToList(),
                draftGvAudit = _context.DraftGvaudits.Where(a => a.DraftGvid == id).OrderByDescending(m => m.DateTime).ToList()
            };

            return View(gvViewModel);
        }

        private bool DraftGvExists(int id)
        {
            return (_context.DraftGvs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
