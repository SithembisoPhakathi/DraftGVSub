using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GeneralValuationSubs.Models;

public partial class MyDbDeedsContex : DbContext
{
    public MyDbDeedsContex()
    {
    }

    public MyDbDeedsContex(DbContextOptions<MyDbDeedsContex> options)
        : base(options)
    {
    }

    public virtual DbSet<DevelopmentLand> DevelopmentLands { get; set; }

    public virtual DbSet<FillingStation> FillingStations { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Industrial> Industrials { get; set; }

    public virtual DbSet<Office> Offices { get; set; }

    public virtual DbSet<Residential> Residentials { get; set; }

    public virtual DbSet<SalesStatus> SalesStatuses { get; set; }

    public virtual DbSet<SectionalTitle> SectionalTitles { get; set; }

    public virtual DbSet<ShoppingCentre> ShoppingCentres { get; set; }

    public virtual DbSet<VacantLand> VacantLands { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=168.89.27.118;Database=DEEDS;TrustServerCertificate=true;User Id=sa;Password=Code@007");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DevelopmentLand>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Development_Land");

            entity.Property(e => e.Extent).HasMaxLength(255);
            entity.Property(e => e.LegalDescription)
                .HasMaxLength(255)
                .HasColumnName("Legal Description");
            entity.Property(e => e.NameOfBuyer)
                .HasMaxLength(255)
                .HasColumnName("Name of Buyer");
            entity.Property(e => e.NameOfSeller)
                .HasMaxLength(255)
                .HasColumnName("Name of Seller");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(255)
                .HasColumnName("Premise_ID");
            entity.Property(e => e.PurchaseDate)
                .HasColumnType("date")
                .HasColumnName("Purchase_Date");
            entity.Property(e => e.PurchasePrice)
                .HasColumnType("money")
                .HasColumnName("Purchase_Price");
            entity.Property(e => e.RM)
                .HasMaxLength(255)
                .HasColumnName("R/m²");
            entity.Property(e => e.Sector)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TitleDeedNumber)
                .HasMaxLength(255)
                .HasColumnName("Title_Deed_Number");
            entity.Property(e => e.TownShip).HasMaxLength(150);
            entity.Property(e => e.ValidSale)
                .HasMaxLength(255)
                .HasColumnName("Valid Sale");
            entity.Property(e => e.Zoning).HasMaxLength(255);
        });

        modelBuilder.Entity<FillingStation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Filling_Station");

            entity.Property(e => e.Extent).HasMaxLength(255);
            entity.Property(e => e.Gla1)
                .HasMaxLength(255)
                .HasColumnName("GLA1");
            entity.Property(e => e.Gla2)
                .HasMaxLength(255)
                .HasColumnName("GLA2");
            entity.Property(e => e.Gla3)
                .HasMaxLength(255)
                .HasColumnName("GLA3");
            entity.Property(e => e.LegalDescription)
                .HasMaxLength(255)
                .HasColumnName("Legal Description");
            entity.Property(e => e.Literage).HasMaxLength(255);
            entity.Property(e => e.NameOfBuyer)
                .HasMaxLength(255)
                .HasColumnName("Name_of_Buyer");
            entity.Property(e => e.NameOfSeller)
                .HasMaxLength(255)
                .HasColumnName("Name_of_Seller");
            entity.Property(e => e.NoOfStoreys)
                .HasMaxLength(255)
                .HasColumnName("No_of_storeys");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(255)
                .HasColumnName("Premise_ID");
            entity.Property(e => e.PurchaseDate)
                .HasColumnType("date")
                .HasColumnName("Purchase_Date");
            entity.Property(e => e.PurchasePrice)
                .HasColumnType("money")
                .HasColumnName("Purchase_Price");
            entity.Property(e => e.RL)
                .HasMaxLength(255)
                .HasColumnName("R/L");
            entity.Property(e => e.RM)
                .HasMaxLength(255)
                .HasColumnName("R/m²");
            entity.Property(e => e.Sector)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TitleDeedNumber)
                .HasMaxLength(255)
                .HasColumnName("Title_Deed_Number");
            entity.Property(e => e.TotalGrossLettableArea)
                .HasMaxLength(255)
                .HasColumnName("Total_Gross_Lettable Area");
            entity.Property(e => e.TownShip).HasMaxLength(150);
            entity.Property(e => e.ValidSale)
                .HasMaxLength(255)
                .HasColumnName("Valid_Sale");
            entity.Property(e => e.Zoning).HasMaxLength(255);
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Hotel");

            entity.Property(e => e.Extent).HasMaxLength(255);
            entity.Property(e => e.LegalDescription)
                .HasMaxLength(255)
                .HasColumnName("Legal Description");
            entity.Property(e => e.NameOfBuyer)
                .HasMaxLength(255)
                .HasColumnName("Name of Buyer");
            entity.Property(e => e.NameOfSeller)
                .HasMaxLength(255)
                .HasColumnName("Name of Seller");
            entity.Property(e => e.NoOfStoreys)
                .HasMaxLength(255)
                .HasColumnName("No of storeys");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(255)
                .HasColumnName("Premise_ID");
            entity.Property(e => e.PurchaseDate)
                .HasColumnType("date")
                .HasColumnName("Purchase_Date");
            entity.Property(e => e.PurchasePrice)
                .HasColumnType("money")
                .HasColumnName("Purchase_Price");
            entity.Property(e => e.RM)
                .HasMaxLength(255)
                .HasColumnName("R/m²");
            entity.Property(e => e.RRoom)
                .HasMaxLength(255)
                .HasColumnName("R/Room");
            entity.Property(e => e.Sector)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TitleDeedNumber)
                .HasMaxLength(255)
                .HasColumnName("Title_Deed_Number");
            entity.Property(e => e.TotalNumberOfRooms)
                .HasMaxLength(255)
                .HasColumnName("Total Number of Rooms");
            entity.Property(e => e.TownShip).HasMaxLength(150);
            entity.Property(e => e.ValidSale)
                .HasMaxLength(255)
                .HasColumnName("Valid Sale");
            entity.Property(e => e.Zoning).HasMaxLength(255);
        });

        modelBuilder.Entity<Industrial>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Industrial");

            entity.Property(e => e.BasementParking)
                .HasMaxLength(255)
                .HasColumnName("Basement Parking");
            entity.Property(e => e.Carport).HasMaxLength(255);
            entity.Property(e => e.Extent).HasMaxLength(255);
            entity.Property(e => e.LegalDescription)
                .HasMaxLength(255)
                .HasColumnName("Legal Description");
            entity.Property(e => e.NameOfBuyer)
                .HasMaxLength(255)
                .HasColumnName("Name of Buyer");
            entity.Property(e => e.NameOfSeller)
                .HasMaxLength(255)
                .HasColumnName("Name of Seller");
            entity.Property(e => e.Nla1)
                .HasMaxLength(255)
                .HasColumnName("NLA1");
            entity.Property(e => e.Nla2)
                .HasMaxLength(255)
                .HasColumnName("NLA2");
            entity.Property(e => e.Nla3)
                .HasMaxLength(255)
                .HasColumnName("NLA3");
            entity.Property(e => e.NoOfStoreys)
                .HasMaxLength(255)
                .HasColumnName("No of storeys");
            entity.Property(e => e.OpenParking)
                .HasMaxLength(255)
                .HasColumnName("Open Parking");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(255)
                .HasColumnName("Premise_ID");
            entity.Property(e => e.PurchaseDate)
                .HasColumnType("date")
                .HasColumnName("Purchase_Date");
            entity.Property(e => e.PurchasePrice)
                .HasColumnType("money")
                .HasColumnName("Purchase_Price");
            entity.Property(e => e.RM)
                .HasMaxLength(255)
                .HasColumnName("R/m²");
            entity.Property(e => e.Sector)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TitleDeedNumber)
                .HasMaxLength(255)
                .HasColumnName("Title_Deed_Number");
            entity.Property(e => e.TotalLettableArea)
                .HasMaxLength(255)
                .HasColumnName("Total Lettable Area");
            entity.Property(e => e.TownShip).HasMaxLength(150);
            entity.Property(e => e.ValidSale)
                .HasMaxLength(255)
                .HasColumnName("Valid Sale");
            entity.Property(e => e.Zoning).HasMaxLength(255);
        });

        modelBuilder.Entity<Office>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Office");

            entity.Property(e => e.BasementParking)
                .HasMaxLength(255)
                .HasColumnName("Basement_Parking");
            entity.Property(e => e.Carport).HasMaxLength(255);
            entity.Property(e => e.Extent).HasMaxLength(255);
            entity.Property(e => e.LegalDescription)
                .HasMaxLength(255)
                .HasColumnName("Legal_Description");
            entity.Property(e => e.NameOfBuyer)
                .HasMaxLength(255)
                .HasColumnName("Name_of_Buyer");
            entity.Property(e => e.NameOfSeller)
                .HasMaxLength(255)
                .HasColumnName("Name_of_Seller");
            entity.Property(e => e.Nla1)
                .HasMaxLength(255)
                .HasColumnName("NLA1");
            entity.Property(e => e.Nla2)
                .HasMaxLength(255)
                .HasColumnName("NLA2");
            entity.Property(e => e.Nla3)
                .HasMaxLength(255)
                .HasColumnName("NLA3");
            entity.Property(e => e.NoOfStoreys)
                .HasMaxLength(255)
                .HasColumnName("No_of_storeys");
            entity.Property(e => e.OpenParking)
                .HasMaxLength(255)
                .HasColumnName("Open_Parking");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(255)
                .HasColumnName("Premise_ID");
            entity.Property(e => e.PurchaseDate)
                .HasColumnType("date")
                .HasColumnName("Purchase_Date");
            entity.Property(e => e.PurchasePrice)
                .HasColumnType("money")
                .HasColumnName("Purchase_Price");
            entity.Property(e => e.RM)
                .HasMaxLength(255)
                .HasColumnName("R/m²");
            entity.Property(e => e.Sector)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TitleDeedNumber)
                .HasMaxLength(255)
                .HasColumnName("Title_Deed_Number");
            entity.Property(e => e.TotalLettableArea)
                .HasMaxLength(255)
                .HasColumnName("Total_Lettable_Area");
            entity.Property(e => e.TownShip).HasMaxLength(150);
            entity.Property(e => e.ValidSale)
                .HasMaxLength(255)
                .HasColumnName("Valid_Sale");
            entity.Property(e => e.Zoning).HasMaxLength(255);
        });

        modelBuilder.Entity<Residential>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Residential");

            entity.Property(e => e.Carport).HasMaxLength(255);
            entity.Property(e => e.Extent).HasMaxLength(255);
            entity.Property(e => e.Garage).HasMaxLength(255);
            entity.Property(e => e.GrannyFlat)
                .HasMaxLength(255)
                .HasColumnName("Granny_Flat");
            entity.Property(e => e.LegalDescription)
                .HasMaxLength(255)
                .HasColumnName("Legal_Description");
            entity.Property(e => e.NameOfBuyer)
                .HasMaxLength(255)
                .HasColumnName("Name of Buyer");
            entity.Property(e => e.NameOfSeller)
                .HasMaxLength(255)
                .HasColumnName("Name of Seller");
            entity.Property(e => e.NoOfStoreys)
                .HasMaxLength(255)
                .HasColumnName("No_of_storeys");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(255)
                .HasColumnName("Premise_ID");
            entity.Property(e => e.PurchaseDate)
                .HasColumnType("date")
                .HasColumnName("Purchase_Date");
            entity.Property(e => e.PurchasePrice)
                .HasColumnType("money")
                .HasColumnName("Purchase_Price");
            entity.Property(e => e.RM)
                .HasMaxLength(255)
                .HasColumnName("R/m²");
            entity.Property(e => e.Sector)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ServantsQuarters)
                .HasMaxLength(255)
                .HasColumnName("Servants_Quarters");
            entity.Property(e => e.TitleDeedNumber)
                .HasMaxLength(255)
                .HasColumnName("Title_Deed_Number");
            entity.Property(e => e.Tla1).HasMaxLength(255);
            entity.Property(e => e.Tla2).HasMaxLength(255);
            entity.Property(e => e.Tla3).HasMaxLength(255);
            entity.Property(e => e.TotalLivingArea)
                .HasMaxLength(255)
                .HasColumnName("Total Living Area");
            entity.Property(e => e.TownShip).HasMaxLength(150);
            entity.Property(e => e.ValidSale)
                .HasMaxLength(255)
                .HasColumnName("Valid_Sale");
            entity.Property(e => e.Zoning).HasMaxLength(255);
        });

        modelBuilder.Entity<SalesStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sales_Ta__3213E83F675C9DFD");

            entity.ToTable("Sales_Status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SectionalTitle>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sectional_Title");

            entity.Property(e => e.Extent).HasMaxLength(255);
            entity.Property(e => e.LegalDescription)
                .HasMaxLength(255)
                .HasColumnName("Legal_Description");
            entity.Property(e => e.NameOfBuyer)
                .HasMaxLength(255)
                .HasColumnName("Name_of_Buyer");
            entity.Property(e => e.NameOfSeller)
                .HasMaxLength(255)
                .HasColumnName("Name_of_Seller");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(255)
                .HasColumnName("Premise_ID");
            entity.Property(e => e.PurchaseDate)
                .HasMaxLength(255)
                .HasColumnName("Purchase_Date");
            entity.Property(e => e.PurchasePrice)
                .HasMaxLength(255)
                .HasColumnName("Purchase_Price");
            entity.Property(e => e.RM)
                .HasMaxLength(255)
                .HasColumnName("R/m²");
            entity.Property(e => e.SchemeName)
                .HasMaxLength(255)
                .HasColumnName("Scheme_Name");
            entity.Property(e => e.Sector).HasMaxLength(255);
            entity.Property(e => e.TitleDeedNumber)
                .HasMaxLength(255)
                .HasColumnName("Title_Deed_Number");
            entity.Property(e => e.TownShip).HasMaxLength(150);
            entity.Property(e => e.UnitNumber)
                .HasMaxLength(255)
                .HasColumnName("Unit_Number");
            entity.Property(e => e.UnitSize)
                .HasMaxLength(255)
                .HasColumnName("Unit_Size");
            entity.Property(e => e.ValidSale)
                .HasMaxLength(255)
                .HasColumnName("Valid_Sale");
            entity.Property(e => e.Zoning).HasMaxLength(255);
        });

        modelBuilder.Entity<ShoppingCentre>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Shopping_Centre");

            entity.Property(e => e.BasementParking)
                .HasMaxLength(255)
                .HasColumnName("Basement_Parking");
            entity.Property(e => e.Carport).HasMaxLength(255);
            entity.Property(e => e.Extent).HasMaxLength(255);
            entity.Property(e => e.LegalDescription)
                .HasMaxLength(255)
                .HasColumnName("Legal_Description");
            entity.Property(e => e.NameOfBuyer)
                .HasMaxLength(255)
                .HasColumnName("Name_of_Buyer");
            entity.Property(e => e.NameOfSeller)
                .HasMaxLength(255)
                .HasColumnName("Name_of_Seller");
            entity.Property(e => e.Nla1)
                .HasMaxLength(255)
                .HasColumnName("NLA1");
            entity.Property(e => e.Nla2)
                .HasMaxLength(255)
                .HasColumnName("NLA2");
            entity.Property(e => e.Nla3)
                .HasMaxLength(255)
                .HasColumnName("NLA3");
            entity.Property(e => e.NoOfStoreys)
                .HasMaxLength(255)
                .HasColumnName("No_of_storeys");
            entity.Property(e => e.OpenParking)
                .HasMaxLength(255)
                .HasColumnName("Open_Parking");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(255)
                .HasColumnName("Premise_ID");
            entity.Property(e => e.PurchaseDate)
                .HasMaxLength(255)
                .HasColumnName("Purchase_Date");
            entity.Property(e => e.PurchasePrice)
                .HasMaxLength(255)
                .HasColumnName("Purchase_Price");
            entity.Property(e => e.RM)
                .HasMaxLength(255)
                .HasColumnName("R/m²");
            entity.Property(e => e.Sector).HasMaxLength(255);
            entity.Property(e => e.TitleDeedNumber)
                .HasMaxLength(255)
                .HasColumnName("Title_Deed_Number");
            entity.Property(e => e.TotalLettableArea)
                .HasMaxLength(255)
                .HasColumnName("Total_Lettable_Area");
            entity.Property(e => e.ValidSale)
                .HasMaxLength(255)
                .HasColumnName("Valid_Sale");
            entity.Property(e => e.Zoning).HasMaxLength(255);
        });

        modelBuilder.Entity<VacantLand>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Vacant_Land");

            entity.Property(e => e.Extent).HasMaxLength(255);
            entity.Property(e => e.LegalDescription)
                .HasMaxLength(255)
                .HasColumnName("Legal_Description");
            entity.Property(e => e.NameOfBuyer)
                .HasMaxLength(255)
                .HasColumnName("Name_of_Buyer");
            entity.Property(e => e.NameOfSeller)
                .HasMaxLength(255)
                .HasColumnName("Name_of_Seller");
            entity.Property(e => e.PremiseId)
                .HasMaxLength(255)
                .HasColumnName("Premise_ID");
            entity.Property(e => e.PurchaseDate)
                .HasColumnType("date")
                .HasColumnName("Purchase_Date");
            entity.Property(e => e.PurchasePrice)
                .HasColumnType("money")
                .HasColumnName("Purchase_Price");
            entity.Property(e => e.RM)
                .HasMaxLength(255)
                .HasColumnName("R/m²");
            entity.Property(e => e.Sector)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TitleDeedNumber)
                .HasMaxLength(255)
                .HasColumnName("Title_Deed_Number");
            entity.Property(e => e.TownShip).HasMaxLength(150);
            entity.Property(e => e.ValidSale)
                .HasMaxLength(255)
                .HasColumnName("Valid_Sale");
            entity.Property(e => e.Zoning).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
