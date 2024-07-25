using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using GeneralValuationSubs.Models;

namespace GeneralValuationSubs.Controllers
{
    public class TownshipController : Controller
    {
        private readonly UpdatedGvtoolContext _context;
        private readonly RoleManager<IdentityRole> directorRole;
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        List<DraftGv> township = new List<DraftGv>();
        private readonly ILogger<TownshipController> _logger;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public TownshipController(UpdatedGvtoolContext context, ILogger<TownshipController> logger, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
            con.ConnectionString = _config.GetConnectionString("DefaultConnection");
        }

        [Authorize(Roles = "Deputy Director,Area Manager,Valuer,Senior Manager")]
        public async Task<IActionResult> Township() 
        { 
            var myroles = this.User.FindFirstValue(ClaimTypes.Role);
            string useremail = this.User.FindFirstValue(ClaimTypes.Email);

            if (myroles == "Deputy Director")
            {
                if (township.Count > 0)
                {
                    township.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "SELECT distinct town FROM [GV23DEV].[dbo].[DraftGV] where Sector = (SELECT distinct Sector FROM [GV23DEV].[dbo].[DraftGV] where DeputyDirector = '" + useremail + "') order by Town";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        township.Add(new DraftGv
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
                return View(township);
                //return _context.DraftGvs != null ?
                //         View(await _context.DraftGvs.Where(a => a.DeputyDirector == useremail && a.).Take(1000).OrderBy(m => m.Town).ToListAsync()) :
                //         Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Area Manager")
            {
                if (township.Count > 0)
                {
                    township.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "SELECT distinct Town FROM [GV23DEV].[dbo].[DraftGV] where Sector = (SELECT distinct Sector FROM [GV23DEV].[dbo].[DraftGV] where AreaManager = '" + useremail + "') order by Town";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        township.Add(new DraftGv
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
                return View(township);
            }
            else if (myroles == "Senior Manager")
            {
                if (township.Count > 0)
                {
                    township.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "SELECT distinct Town FROM [GV23DEV].[dbo].[DraftGV] where Sector = (SELECT distinct Sector FROM [GV23DEV].[dbo].[DraftGV] where SeniorManager = '" + useremail + "') order by Town";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        township.Add(new DraftGv
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
                return View(township);
            }
            else if (myroles == "Valuer")
            {
                if (township.Count > 0)
                {
                    township.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "SELECT distinct Town FROM [GV23DEV].[dbo].[DraftGV] where Sector = (SELECT distinct Sector FROM [GV23DEV].[dbo].[DraftGV] where Valuer = '" + useremail + "') order by Town";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        township.Add(new DraftGv
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
                return View(township);
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

            List<DraftGv> township = new List<DraftGv>();

            var myroles = this.User.FindFirstValue(ClaimTypes.Role);
            string useremail = this.User.FindFirstValue(ClaimTypes.Email);

            if (myroles == "Deputy Director")
            {
                if (township.Count > 0)
                {
                    township.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "SELECT * FROM [GV23DEV].[dbo].[DraftGV] where DeputyDirector = '" + useremail + "' AND Town = '" + town_name + "'";
                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        township.Add(new DraftGv
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
            }
            return PartialView("_Gridview", township);
        }

        //{     
        //    //return View(property);
        //    //return _context.DraftGvs != null ?
        //    //         View(await _context.DraftGvs.Where(a => a.DeputyDirector == useremail && a.).Take(1000).OrderBy(m => m.Town).ToListAsync()) :
        //    //         Problem("Database could not open, please refer to Admin.");
        //}

        //else if (myroles == "Area Manager")
        //{
        //    if (township.Count > 0)
        //    {
        //        township.Clear();
        //    }
        //    try
        //    {
        //        con.Open();
        //        com.Connection = con;
        //        com.CommandText = "SELECT * FROM [GV23DEV].[dbo].[DraftGV] where AreaManager = '" + useremail + "' AND Town = '" + town_name + "'";
        //        dr = com.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            township.Add(new DraftGv
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
        //    return PartialView("_Gridview", township);
        //    //return View(property);
        //}
        //else if (myroles == "Senior Manager")
        //{
        //    if (township.Count > 0)
        //    {
        //        township.Clear();
        //    }
        //    try
        //    {
        //        con.Open();
        //        com.Connection = con;
        //        com.CommandText = "SELECT * FROM [GV23DEV].[dbo].[DraftGV] where SeniorManager = '" + useremail + "' AND Town = '" + town_name + "'";
        //        dr = com.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            township.Add(new DraftGv
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
        //    return PartialView("_Gridview", township);
        //    //return View(property);
        //}
        //else if (myroles == "Valuer")
        //{
        //    if (township.Count > 0)
        //    {
        //        township.Clear();
        //    }
        //    try
        //    {
        //        con.Open();
        //        com.Connection = con;
        //        com.CommandText = "SELECT * FROM [GV23DEV].[dbo].[DraftGV] where Valuer = '" + useremail + "' AND Town = '" + town_name + "'";
        //        dr = com.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            township.Add(new DraftGv
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
        //    return PartialView("_Gridview", township);
        //    //return View(property);
        //}       

        public IActionResult Indexx(string Town) 
        {
            var myroles = this.User.FindFirstValue(ClaimTypes.Role);
            string useremail = this.User.FindFirstValue(ClaimTypes.Email);

            if (myroles == "Deputy Director")
            {
                return _context.DraftGvs != null ?
                        View(_context.DraftGvs.GroupBy(x => x.Town == Town).Select(y => y.First()).Distinct().ToListAsync()) :
                        Problem("Database could not open, please refer to Admin.");
            }

            else if (myroles == "Area Manager")
            {
                return _context.DraftGvs != null ?
                        View(_context.DraftGvs.GroupBy(x => x.Town == Town).Select(y => y.First()).Distinct().ToListAsync()) :
                       Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Senior Manager")
            {
                return _context.DraftGvs != null ?
                         View(_context.DraftGvs.GroupBy(x => x.Town == Town).Select(y => y.First()).Distinct().ToListAsync()) :
                        Problem("Database could not open, please refer to Admin.");
            }
            else if (myroles == "Valuer")
            {
                return _context.DraftGvs != null ?
                        View(_context.DraftGvs.GroupBy(x => x.Town == Town).Select(y => y.First()).Distinct().ToListAsync()) :
                        Problem("Database could not open, please refer to Admin.");
            }

            return PartialView("_Gridview");

            //return NotFound();
            //Problem("User does not have a role and Township");
        }
    }
}
