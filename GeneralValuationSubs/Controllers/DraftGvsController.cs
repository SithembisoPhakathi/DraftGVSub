using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using GeneralValuationSubs.Models;

namespace GeneralValuationSubs.Controllers
{
    public class DraftGvsController : Controller
    {
        private readonly UpdatedGvtoolContext _context;
        private readonly RoleManager<IdentityRole> directorRole;
        SqlCommand com = new SqlCommand();
        List<AdminValuer> adminValuers = new();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        List<DraftGv> draftGvs = new List<DraftGv>();
        private readonly ILogger<DraftGvsController> _logger;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        //public DraftGvsController(UpdatedGvtoolContext context, ILogger<DraftGvsController> logger, Microsoft.Extensions.Configuration.IConfiguration config)
        //{
        //    _context = context;
        //    _logger = logger;
        //    _config = config;
        //    con.ConnectionString = _config.GetConnectionString("DefaultConnection");
        //}

        //[HttpPost]
        //public IActionResult Login(string User)
        //{
        //    if (adminValuers.Count > 0)
        //    {
        //        adminValuers.Clear();
        //    }
        //    try
        //    {
        //        con.Open();
        //        com.Connection = con;
        //        com.CommandText = "SELECT [Username_Old],[Username],[Role], [Sector], [Email_Address], [Surname], [First_name], [Phone_No] FROM [UpdatedGVTool].[dbo].[Admin-Valuer] " +
        //            "where Username = '" + User + "' or Username_Old = '" + User + "'";
        //        dr = com.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            adminValuers.Add(new AdminValuer
        //            {
        //                Username = dr["Username"].ToString(),
        //                Role = dr["Role"].ToString(),
        //                Sector = dr["Sector"].ToString(),
        //                EmailAddress = dr["Email_Address"].ToString(),
        //                Surname = dr["Surname"].ToString(),
        //                FirstName = dr["First_name"].ToString(),
        //                PhoneNo = dr["Phone_No"].ToString(),
        //            });
        //        }
        //        con.Close();
        //        if (adminValuers.Count > 0)
        //        {
        //            foreach (var admin in adminValuers)
        //            {

        //                if (admin.Role == "VALUATION ADMIN" || admin.Role == "VALUATION ADMIN-INTERN")
        //                {
        //                    TempData["currentRole"] = admin.Role;
        //                    TempData["currentUserRole"] = "Admin";
        //                    TempData["currentUserSector"] = admin.Sector;
        //                    TempData["currentUserEmail"] = admin.EmailAddress;
        //                    TempData["currentUserSurname"] = admin.Surname;
        //                    TempData["currentUserFirstname"] = admin.FirstName;
        //                    TempData["currentUserPhoneNo"] = admin.PhoneNo;


        //                    return RedirectToAction("Admin", adminValuers);
        //                }
        //                if (admin.Role == "VALUER" || admin.Role == "DEPUTY DIRECTOR" ||
        //                    admin.Role == "SENIOR MANAGER" || admin.Role == "AREA MANAGER" || admin.Role == "INTERN VALUER")
        //                {
        //                    TempData["currentRole"] = admin.Role;
        //                    TempData["currentUserRole"] = "Valuer";
        //                    TempData["currentUserSector"] = admin.Sector;
        //                    TempData["currentUserSurname"] = admin.Surname;
        //                    TempData["currentUserFirstname"] = admin.FirstName;
        //                    TempData["currentUserEmail"] = admin.EmailAddress;

        //                    TempData.Keep("MV_total");
        //                    TempData.Keep("Desc_total");
        //                    TempData.Keep("Ext_total");
        //                    TempData.Keep("Addr_total");
        //                    TempData.Keep("Name_total");
        //                    TempData.Keep("Cat_total");

        //                    return RedirectToAction("Valuer", adminValuers);
        //                }
        //                else
        //                {
        //                    return PartialView("_Error403");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return PartialView("_Error403");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    TempData.Keep("currentEmail");
        //    TempData.Keep("currentRole");
        //    TempData.Keep("currentUserRole");
        //    TempData.Keep("currentUserSector");
        //    TempData.Keep("currentUserEmail");
        //    TempData.Keep("currentUserSurname");
        //    TempData.Keep("currentUserFirstname");
        //    TempData.Keep("currentUserPhoneNo");
        //    TempData.Keep("currentUsername");


        //    return View();
        //}

        // GET: DraftGvs
        //[Authorize(Roles = "Deputy Director,Area Manager,Valuer,Senior Manager")]
        public async Task<IActionResult> Index(string? myroles, string? useremail)
        {


            if (myroles == "Deputy Director")
            {
                return _context.DraftGvs != null ?
                         View(await _context.DraftGvs.Where(a => a.DeputyDirector == useremail).Take(50).OrderByDescending(m => m.DateTime).ToListAsync()) :
                Problem("Database could not open, please refer to Admin.");
            }

            else if (myroles == "Area Manager")
            {
                return _context.DraftGvs != null ?
                     View(await _context.DraftGvs.Where(a => a.AreaManager == useremail).Take(500).OrderByDescending(m => m.DateTime).ToListAsync()) :
                     Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Senior Manager")
            {
                return _context.DraftGvs != null ?
                        View(await _context.DraftGvs.Where(a => a.SeniorManager == useremail).Take(int.MaxValue).OrderByDescending(m => m.DateTime).ToListAsync()) :
                        Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Valuer")
            {
                return _context.DraftGvs != null ?
                        View(await _context.DraftGvs.Where(a => a.Valuer == useremail).Take(500).OrderByDescending(m => m.DateTime).ToListAsync()) :
                        Problem("Database could not open, please refer to Admin.");
            }

            return View();
            //Problem("User does not have a role");
        }
         

        // GET: DraftGvs/Details/5
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

        // GET: DraftGvs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DraftGvs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PremiseId,Town,Ddarea,CalculatedValue,ValuersValue,DraftValue,Gv18marketValue,Gv18category,PurchasePrice,PurchaseDate,PnCproperty,ParentPremiseId,ValueOverride,ValueRevised,CommentValue,CategoryOverride,CategoryRevised,CommentCategory,FlagDelete,CommentDelete,DateTime,UserName")] DraftGv draftGv)
        {
            if (ModelState.IsValid)
            {
                _context.Add(draftGv);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(draftGv);
        }

        // GET: DraftGvs/Edit/5
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

        // GET: DraftGvs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DraftGvs == null)
            {
                return NotFound();
            }

            var draftGv = await _context.DraftGvs.FirstOrDefaultAsync(m => m.Id == id);

            if (draftGv == null)
            {
                return NotFound();
            }

            return View(draftGv);
        }

        // POST: DraftGvs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DraftGvs == null)
            {
                return Problem("Entity set 'UpdatedGvtoolContext.DraftGvs'  is null.");
            }
            var draftGv = await _context.DraftGvs.FindAsync(id);
            if (draftGv != null)
            {
                _context.DraftGvs.Remove(draftGv);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DraftGvExists(int id)
        {
          return (_context.DraftGvs?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult Town()
        { 
            var myroles = this.User.FindFirstValue(ClaimTypes.Role);
            string useremail = this.User.FindFirstValue(ClaimTypes.Email);

            if (myroles == "Deputy Director")
            {
                return _context.DraftGvs != null ?
                        View(_context.DraftGvs.GroupBy(x => x.Town).Select(y => y.First()).Distinct().ToListAsync()) :
                        Problem("Database could not open, please refer to Admin.");
            }

            else if (myroles == "Area Manager")
            {
                return _context.DraftGvs != null ?
                        View(_context.DraftGvs.GroupBy(x => x.Town).Select(y => y.First()).Distinct().ToListAsync()) :
                       Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Senior Manager")
            {
                return _context.DraftGvs != null ?
                         View(_context.DraftGvs.GroupBy(x => x.Town).Select(y => y.First()).Distinct().ToListAsync()) :
                        Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Valuer")
            {
                return _context.DraftGvs != null ?
                        View(_context.DraftGvs.GroupBy(x => x.Town).Select(y => y.First()).Distinct().ToListAsync()) :
                        Problem("Database could not open, please refer to Admin.");
            }

            return PartialView("_Gridview");

            //return NotFound();
            //Problem("User does not have a role and Township");
        }

        [Authorize(Roles = "Deputy Director,Area Manager,Valuer,Senior Manager")]
        public async Task<IActionResult> Township()
        {
            var myroles = this.User.FindFirstValue(ClaimTypes.Role);
            string useremail = this.User.FindFirstValue(ClaimTypes.Email);

            if (myroles == "Deputy Director")
            {
                if (draftGvs.Count > 0)
                {
                    draftGvs.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "SELECT distinct town FROM [GV23DEV].[dbo].[DraftGV] where Sector = (SELECT distinct Sector FROM [GV23DEV].[dbo].[DraftGV] where DeputyDirector = '" + useremail + "') order by Town";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftGvs.Add(new DraftGv
                        {
                            Town = dr["Town"].ToString(),
                        });
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message.ToString());               
                    throw ex;
                }
                return View(draftGvs);
                //return _context.DraftGvs != null ?
                //         View(await _context.DraftGvs.Where(a => a.DeputyDirector == useremail && a.).Take(1000).OrderBy(m => m.Town).ToListAsync()) :
                //         Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Area Manager")
            {
                if (draftGvs.Count > 0)
                {
                    draftGvs.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "SELECT distinct Town FROM [GV23DEV].[dbo].[DraftGV] where Sector = (SELECT distinct Sector FROM [GV23DEV].[dbo].[DraftGV] where AreaManager = '" + useremail + "') order by Town";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftGvs.Add(new DraftGv
                        {
                            Town = dr["Town"].ToString(),
                        });
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message.ToString());               
                    throw ex;
                }
                return View(draftGvs);
            }
            else if (myroles == "Senior Manager")
            {
                if (draftGvs.Count > 0)
                {
                    draftGvs.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "SELECT distinct Town FROM [GV23DEV].[dbo].[DraftGV] where Sector = (SELECT distinct Sector FROM [GV23DEV].[dbo].[DraftGV] where SeniorManager = '" + useremail + "') order by Town";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftGvs.Add(new DraftGv
                        {
                            Town = dr["Town"].ToString(),
                        });
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message.ToString());               
                    throw ex;
                }
                return View(draftGvs);
            }
            else if (myroles == "Valuer")
            {
                if (draftGvs.Count > 0)
                {
                    draftGvs.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "SELECT distinct Town FROM [GV23DEV].[dbo].[DraftGV] where Sector = (SELECT distinct Sector FROM [GV23DEV].[dbo].[DraftGV] where Valuer = '" + useremail + "') order by Town";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftGvs.Add(new DraftGv
                        {
                            Town = dr["Town"].ToString(),
                        });
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message.ToString());               
                    throw ex;
                }
                return View(draftGvs);
            }

            return NotFound();
            Problem("User does not have a role");
        }

        public IActionResult GetRecords(string town_name)
        {
            //return PartialView("_Gridview", await _context.DraftGvs.GroupBy(x => x.Town == town_name).Select(y => y.First()).Distinct().ToListAsync());
            // && b.Gv18category.Contains(category)
            //var property = from b in _context.DraftGvs
            //               select b;
            //property = property.Where(b => b.Town.Contains(town_name));
            //Problem("Database could not open, please refer to Admin.");

            //List<DraftGv> draftGvs = new List<DraftGv>();

            var myroles = this.User.FindFirstValue(ClaimTypes.Role);
            string useremail = this.User.FindFirstValue(ClaimTypes.Email);

            if (myroles == "Deputy Director")
            {
                if (draftGvs.Count > 0)
                {
                    draftGvs.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "SELECT * FROM [GV23DEV].[dbo].[DraftGV] where DeputyDirector = '" + useremail + "' AND Town = '" + town_name + "'";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        draftGvs.Add(new DraftGv
                        {
                            PremiseId = dr["PremiseId"].ToString(),
                            Id = (int)dr["Id"],
                            Town = dr["Town"].ToString(),
                            Gv18category = dr["Gv18category"].ToString(),
                            Gv18marketValue = (int)dr["Gv18marketValue"],
                            ValueRevised = (int)dr["ValueRevised"],
                            CategoryRevised = dr["CategoryRevised"].ToString(),
                            Status = dr["Status"].ToString(),
                        });
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message.ToString());               
                    throw ex;
                }

                //    //return View(property);
                //    //return _context.DraftGvs != null ?
                //    //         View(await _context.DraftGvs.Where(a => a.DeputyDirector == useremail && a.).Take(1000).OrderBy(m => m.Town).ToListAsync()) :
                //    //         Problem("Database could not open, please refer to Admin.");
                //}

                //else if (myroles == "Area Manager")
                //{
                //    if (draftGvs.Count > 0)
                //    {
                //        draftGvs.Clear();
                //    }
                //    try
                //    {
                //        con.Open();
                //        com.Connection = con;
                //        com.CommandText = "SELECT * FROM [GV23DEV].[dbo].[DraftGV] where AreaManager = '" + useremail + "' AND Town = '" + town_name + "'";
                //        dr = com.ExecuteReader();
                //        while (dr.Read())
                //        {
                //            draftGvs.Add(new DraftGv
                //            {
                //                PremiseId = dr["PremiseId"].ToString(),
                //                Id = (int)dr["Id"],
                //                Town = dr["Town"].ToString(),
                //                Gv18category = dr["Gv18category"].ToString(),
                //                Gv18marketValue = (int)dr["Gv18marketValue"],
                //                ValueRevised = (int)dr["ValueRevised"],
                //                CategoryRevised = dr["CategoryRevised"].ToString(),
                //                Status = dr["Status"].ToString(),
                //            });
                //        }
                //        con.Close();

                //    }
                //    catch (Exception ex)
                //    {
                //        //Console.WriteLine(ex.Message.ToString());               
                //        throw ex;
                //    }
                //    return PartialView("_Gridview", draftGvs);
                //    //return View(property);
                //}
                //else if (myroles == "Senior Manager")
                //{
                //    if (draftGvs.Count > 0)
                //    {
                //        draftGvs.Clear();
                //    }
                //    try
                //    {
                //        con.Open();
                //        com.Connection = con;
                //        com.CommandText = "SELECT * FROM [GV23DEV].[dbo].[DraftGV] where SeniorManager = '" + useremail + "' AND Town = '" + town_name + "'";
                //        dr = com.ExecuteReader();
                //        while (dr.Read())
                //        {
                //            draftGvs.Add(new DraftGv
                //            {
                //                PremiseId = dr["PremiseId"].ToString(),
                //                Id = (int)dr["Id"],
                //                Town = dr["Town"].ToString(),
                //                Gv18category = dr["Gv18category"].ToString(),
                //                Gv18marketValue = (int)dr["Gv18marketValue"],
                //                ValueRevised = (int)dr["ValueRevised"],
                //                CategoryRevised = dr["CategoryRevised"].ToString(),
                //                Status = dr["Status"].ToString(),
                //            });
                //        }

                //    }
                //    catch (Exception ex)
                //    {
                //        //Console.WriteLine(ex.Message.ToString());               
                //        throw ex;
                //    }
                //    return PartialView("_Gridview", draftGvs);
                //    //return View(property);
                //}
                //else if (myroles == "Valuer")
                //{
                //    if (draftGvs.Count > 0)
                //    {
                //        draftGvs.Clear();
                //    }
                //    try
                //    {
                //        con.Open();
                //        com.Connection = con;
                //        com.CommandText = "SELECT * FROM [GV23DEV].[dbo].[DraftGV] where Valuer = '" + useremail + "' AND Town = '" + town_name + "'";
                //        dr = com.ExecuteReader();
                //        while (dr.Read())
                //        {
                //            draftGvs.Add(new DraftGv
                //            {
                //                PremiseId = dr["PremiseId"].ToString(),
                //                Id = (int)dr["Id"],
                //                Town = dr["Town"].ToString(),
                //                Gv18category = dr["Gv18category"].ToString(),
                //                Gv18marketValue = (int)dr["Gv18marketValue"],
                //                ValueRevised = (int)dr["ValueRevised"],
                //                CategoryRevised = dr["CategoryRevised"].ToString(),
                //                Status = dr["Status"].ToString(),
                //            });
                //        }
                //        con.Close();

                //    }
                //    catch (Exception ex)
                //    {
                //        //Console.WriteLine(ex.Message.ToString());               
                //        throw ex;
                //    }
                //    return PartialView("_Gridview", draftGvs);
                //    //return View(property);
                //}

                //return PartialView("_Gridview", draftGvs);

                ////return NotFound();
                //Problem("User does not have a role");
            }
            return PartialView("_Gridview", draftGvs);

        }
    }
}
