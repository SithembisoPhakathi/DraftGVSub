using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GeneralValuationSubs.Models;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AdminValuer> AdminValuers { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Draft> Drafts { get; set; }

    public virtual DbSet<DraftAudit> DraftAudits { get; set; }

    public virtual DbSet<DraftGv> DraftGvs { get; set; }

    public virtual DbSet<DraftGvaudit> DraftGvaudits { get; set; }

    public virtual DbSet<DraftHistory> DraftHistories { get; set; }

    public virtual DbSet<Township> Townships { get; set; }

    public virtual DbSet<Valuer> Valuers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=168.89.27.100 ;TrustServerCertificate=true;Initial Catalog=UpdatedGVTool;User ID=DraftGVUser;Password=Code@011");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Admin");

            entity.Property(e => e.ConfirmPassword)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("confirmPassword");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(255)
                .HasColumnName("Email_Address");
            entity.Property(e => e.EncryptedPassword)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("First_Name");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasMaxLength(255);
            entity.Property(e => e.SapNumber)
                .HasMaxLength(255)
                .HasColumnName("SAP_Number");
            entity.Property(e => e.SapNumberOld)
                .HasMaxLength(255)
                .HasColumnName("SAP_Number_Old");
            entity.Property(e => e.Surname).HasMaxLength(255);
        });

        modelBuilder.Entity<AdminValuer>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Admin-Valuer");

            entity.Property(e => e.EmailAddress)
                .HasMaxLength(255)
                .HasColumnName("Email_Address");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("First_Name");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Phone_No");
            entity.Property(e => e.Role).HasMaxLength(255);
            entity.Property(e => e.SapNumber)
                .HasMaxLength(255)
                .HasColumnName("SAP_Number");
            entity.Property(e => e.SapNumberOld)
                .HasMaxLength(255)
                .HasColumnName("SAP_Number_Old");
            entity.Property(e => e.SecondName)
                .HasMaxLength(255)
                .HasColumnName("Second_Name");
            entity.Property(e => e.Sector).HasMaxLength(255);
            entity.Property(e => e.Surname).HasMaxLength(255);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.UsernameOld)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("Username_Old");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Category");

            entity.Property(e => e.Active)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.CatDescName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("CAT_DESC_NAME");
            entity.Property(e => e.CatEndDate)
                .HasColumnType("date")
                .HasColumnName("CAT_END_DATE");
            entity.Property(e => e.CatId).HasColumnName("CAT_ID");
            entity.Property(e => e.CatStartDate)
                .HasColumnType("date")
                .HasColumnName("CAT_START_DATE");
            entity.Property(e => e.LisCatCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LIS_CAT_CODE");
        });

        modelBuilder.Entity<Draft>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Draft");

            entity.Property(e => e.ApproverComment)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("approverComment");
            entity.Property(e => e.AssignedValuer).HasMaxLength(50);
            entity.Property(e => e.BulkUpload).HasMaxLength(50);
            entity.Property(e => e.CommentCategory).HasMaxLength(50);
            entity.Property(e => e.CommentFlagging).HasMaxLength(50);
            entity.Property(e => e.CommentMarketValue).HasMaxLength(100);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.DraftId)
                .ValueGeneratedOnAdd()
                .HasColumnName("DraftID");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Email_Address");
            entity.Property(e => e.MarketCategory).HasMaxLength(50);
            entity.Property(e => e.MarketValue)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.PremiseId)
                .HasMaxLength(40)
                .HasColumnName("PremiseID");
            entity.Property(e => e.PropertyDescription).HasMaxLength(50);
            entity.Property(e => e.RevisedCategory).HasMaxLength(50);
            entity.Property(e => e.RevisedMarketValue)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TownshipDescription).HasMaxLength(50);
        });

        modelBuilder.Entity<DraftAudit>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DraftAudit");

            entity.Property(e => e.AssignedValuer).HasMaxLength(50);
            entity.Property(e => e.CommentCategory).HasMaxLength(50);
            entity.Property(e => e.CommentFlagging).HasMaxLength(50);
            entity.Property(e => e.CommentMarketValue).HasMaxLength(100);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.DraftAuditId)
                .ValueGeneratedOnAdd()
                .HasColumnName("DraftAuditID");
            entity.Property(e => e.DraftId).HasColumnName("DraftID");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Email_Address");
            entity.Property(e => e.MarketCategory).HasMaxLength(50);
            entity.Property(e => e.MarketValue).HasColumnType("money");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(40)
                .HasColumnName("PremiseID");
            entity.Property(e => e.PropertyDescription).HasMaxLength(50);
            entity.Property(e => e.RevisedCategory).HasMaxLength(50);
            entity.Property(e => e.RevisedMarketValue).HasColumnType("money");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TownshipDescription).HasMaxLength(50);
        });

        modelBuilder.Entity<DraftGv>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DraftGV");

            entity.Property(e => e.AbasisTimesAdjWgba)
                .HasColumnType("money")
                .HasColumnName("ABasisTimesAdjWGBA");
            entity.Property(e => e.AdjustedBasis).HasColumnType("money");
            entity.Property(e => e.AdjustedWgba)
                .HasColumnType("money")
                .HasColumnName("AdjustedWGBA");
            entity.Property(e => e.AreaManager).HasMaxLength(255);
            entity.Property(e => e.CalculatedValue).HasColumnType("money");
            entity.Property(e => e.CategoryRevised).HasMaxLength(255);
            entity.Property(e => e.CommentCategory).HasMaxLength(255);
            entity.Property(e => e.CommentDelete).HasMaxLength(255);
            entity.Property(e => e.CommentValue).HasMaxLength(255);
            entity.Property(e => e.DateTime).HasColumnType("date");
            entity.Property(e => e.Ddarea)
                .HasMaxLength(255)
                .HasColumnName("DDArea");
            entity.Property(e => e.DeputyDirector).HasMaxLength(255);
            entity.Property(e => e.DiffsGv23vsGv18)
                .HasColumnType("money")
                .HasColumnName("DiffsGV23vsGV18");
            entity.Property(e => e.DiffsGv23vsGv18perc).HasColumnName("DiffsGV23vsGV18Perc");
            entity.Property(e => e.DiffsGv23vsSales)
                .HasColumnType("money")
                .HasColumnName("DiffsGV23vsSales");
            entity.Property(e => e.DiffsGv23vsSalesPerc).HasColumnName("DiffsGV23vsSalesPerc");
            entity.Property(e => e.DraftValue).HasColumnType("money");
            entity.Property(e => e.Gv18category)
                .HasMaxLength(255)
                .HasColumnName("GV18Category");
            entity.Property(e => e.Gv18marketValue).HasColumnType("money");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LislegalArea).HasColumnName("LISLegalArea");
            entity.Property(e => e.ModelUsed).HasMaxLength(255);
            entity.Property(e => e.Nbhdbasis)
                .HasColumnType("money")
                .HasColumnName("NBHDBasis");
            entity.Property(e => e.Nbhoodcode)
                .HasMaxLength(255)
                .HasColumnName("NBHOODCode");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Note1).HasMaxLength(255);
            entity.Property(e => e.Note2).HasMaxLength(255);
            entity.Property(e => e.ParentPremiseId)
                .HasMaxLength(255)
                .HasColumnName("ParentPremiseID");
            entity.Property(e => e.PnCproperty)
                .HasMaxLength(255)
                .HasColumnName("PnCProperty");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(255)
                .HasColumnName("PremiseID");
            entity.Property(e => e.PrevDraftValue).HasColumnType("money");
            entity.Property(e => e.ProposedUseDescription).HasMaxLength(255);
            entity.Property(e => e.PurchaseDate).HasPrecision(0);
            entity.Property(e => e.PurchasePrice).HasColumnType("money");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SalesStatus).HasMaxLength(255);
            entity.Property(e => e.SchemeName).HasMaxLength(255);
            entity.Property(e => e.Sector)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SeniorManager).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TimeAdjPurchasePrice).HasColumnType("money");
            entity.Property(e => e.Town).HasMaxLength(255);
            entity.Property(e => e.UseDescription).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.Valuer).HasMaxLength(255);
            entity.Property(e => e.ValuersComment).HasMaxLength(255);
            entity.Property(e => e.ValuersValue).HasColumnType("money");
            entity.Property(e => e.VauseCode)
                .HasMaxLength(255)
                .HasColumnName("VAUseCode");
            entity.Property(e => e.ZoneCode)
                .HasMaxLength(255)
                .HasColumnName("Zone Code");
        });

        modelBuilder.Entity<DraftGvaudit>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DraftGVAudit");

            entity.Property(e => e.AbasisTimesAdjWgba)
                .HasColumnType("money")
                .HasColumnName("ABasisTimesAdjWGBA");
            entity.Property(e => e.AdjustedBasis).HasColumnType("money");
            entity.Property(e => e.AdjustedWgba)
                .HasColumnType("money")
                .HasColumnName("AdjustedWGBA");
            entity.Property(e => e.AreaManager).HasMaxLength(255);
            entity.Property(e => e.CalculatedValue).HasColumnType("money");
            entity.Property(e => e.CategoryRevised).HasMaxLength(255);
            entity.Property(e => e.CommentCategory).HasMaxLength(255);
            entity.Property(e => e.CommentDelete).HasMaxLength(255);
            entity.Property(e => e.CommentValue).HasMaxLength(255);
            entity.Property(e => e.DateTime).HasColumnType("date");
            entity.Property(e => e.Ddarea)
                .HasMaxLength(255)
                .HasColumnName("DDArea");
            entity.Property(e => e.DeputyDirector).HasMaxLength(255);
            entity.Property(e => e.DiffsGv23vsGv18)
                .HasColumnType("money")
                .HasColumnName("DiffsGV23vsGV18");
            entity.Property(e => e.DiffsGv23vsGv18perc).HasColumnName("DiffsGV23vsGV18Perc");
            entity.Property(e => e.DiffsGv23vsSales)
                .HasColumnType("money")
                .HasColumnName("DiffsGV23vsSales");
            entity.Property(e => e.DiffsGv23vsSalesPerc).HasColumnName("DiffsGV23vsSalesPerc");
            entity.Property(e => e.DraftGvid).HasColumnName("DraftGVID");
            entity.Property(e => e.DraftValue).HasColumnType("money");
            entity.Property(e => e.Gv18category)
                .HasMaxLength(255)
                .HasColumnName("GV18Category");
            entity.Property(e => e.Gv18marketValue).HasColumnType("money");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LislegalArea).HasColumnName("LISLegalArea");
            entity.Property(e => e.ModelUsed).HasMaxLength(255);
            entity.Property(e => e.Nbhdbasis)
                .HasColumnType("money")
                .HasColumnName("NBHDBasis");
            entity.Property(e => e.Nbhoodcode)
                .HasMaxLength(255)
                .HasColumnName("NBHOODCode");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Note1).HasMaxLength(255);
            entity.Property(e => e.Note2).HasMaxLength(255);
            entity.Property(e => e.ParentPremiseId)
                .HasMaxLength(255)
                .HasColumnName("ParentPremiseID");
            entity.Property(e => e.PnCproperty)
                .HasMaxLength(255)
                .HasColumnName("PnCProperty");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(255)
                .HasColumnName("PremiseID");
            entity.Property(e => e.PrevDraftValue).HasColumnType("money");
            entity.Property(e => e.ProposedUseDescription).HasMaxLength(255);
            entity.Property(e => e.PurchaseDate).HasPrecision(0);
            entity.Property(e => e.PurchasePrice).HasColumnType("money");
            entity.Property(e => e.SalesStatus).HasMaxLength(255);
            entity.Property(e => e.SchemeName).HasMaxLength(255);
            entity.Property(e => e.Sector)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SeniorManager).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TimeAdjPurchasePrice).HasColumnType("money");
            entity.Property(e => e.Town).HasMaxLength(255);
            entity.Property(e => e.UseDescription).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.Valuer).HasMaxLength(255);
            entity.Property(e => e.ValuersComment).HasMaxLength(255);
            entity.Property(e => e.ValuersValue).HasColumnType("money");
            entity.Property(e => e.VauseCode)
                .HasMaxLength(255)
                .HasColumnName("VAUseCode");
            entity.Property(e => e.ZoneCode)
                .HasMaxLength(255)
                .HasColumnName("Zone Code");
        });

        modelBuilder.Entity<DraftHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DraftHistory");

            entity.Property(e => e.ActivityDate).HasColumnType("datetime");
            entity.Property(e => e.ApproverComment)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("approverComment");
            entity.Property(e => e.CommentCategory)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CommentMarketValue)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.PropertyDescription)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RevisedCategory)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RevisedMarketValue)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserActivity)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UserID");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Township>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Township");

            entity.Property(e => e.AreaManager)
                .HasMaxLength(255)
                .HasColumnName("Area_Manager");
            entity.Property(e => e.CandidateDc)
                .HasMaxLength(255)
                .HasColumnName("Candidate_DC");
            entity.Property(e => e.DeptDir)
                .HasMaxLength(255)
                .HasColumnName("Dept_Dir");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.RegionName)
                .HasMaxLength(255)
                .HasColumnName("REGION_NAME");
            entity.Property(e => e.Sector).HasMaxLength(255);
            entity.Property(e => e.SnrManager)
                .HasMaxLength(255)
                .HasColumnName("Snr_Manager");
            entity.Property(e => e.Tc)
                .HasMaxLength(255)
                .HasColumnName("TC");
            entity.Property(e => e.TownName)
                .HasMaxLength(255)
                .HasColumnName("TOWN_NAME");
        });

        modelBuilder.Entity<Valuer>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Designation).HasMaxLength(255);
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(255)
                .HasColumnName("Email Address");
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Sap)
                .HasMaxLength(255)
                .HasColumnName("SAP");
            entity.Property(e => e.SecondName).HasMaxLength(255);
            entity.Property(e => e.Sector).HasMaxLength(255);
            entity.Property(e => e.Surname).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
