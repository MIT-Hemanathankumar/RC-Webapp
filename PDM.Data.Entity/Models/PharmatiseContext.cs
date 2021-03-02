using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PDM.Data.Entity.Models
{
    public partial class PharmatiseContext : DbContext
    {
        public PharmatiseContext()
        {
        }

        public PharmatiseContext(DbContextOptions<PharmatiseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MasAddressType> MasAddressType { get; set; }
        public virtual DbSet<MasCategory> MasCategory { get; set; }
        public virtual DbSet<MasCountry> MasCountry { get; set; }
        public virtual DbSet<MasDeliveryType> MasDeliveryType { get; set; }
        public virtual DbSet<MasOrderType> MasOrderType { get; set; }
        public virtual DbSet<MasPickupType> MasPickupType { get; set; }
        public virtual DbSet<MasRole> MasRole { get; set; }
        public virtual DbSet<MasState> MasState { get; set; }
        public virtual DbSet<MasStrength> MasStrength { get; set; }
        public virtual DbSet<MasType> MasType { get; set; }
        public virtual DbSet<MasUom> MasUom { get; set; }
        public virtual DbSet<MasUserDocumentType> MasUserDocumentType { get; set; }
        public virtual DbSet<MasUserRoute> MasUserRoute { get; set; }
        public virtual DbSet<MsaContactType> MsaContactType { get; set; }
        public virtual DbSet<ProAddress> ProAddress { get; set; }
        public virtual DbSet<ProBranch> ProBranch { get; set; }
        public virtual DbSet<ProCaseSheet> ProCaseSheet { get; set; }
        public virtual DbSet<ProCaseSheetDetail> ProCaseSheetDetail { get; set; }
        public virtual DbSet<ProCompany> ProCompany { get; set; }
        public virtual DbSet<ProContact> ProContact { get; set; }
        public virtual DbSet<ProCustomer> ProCustomer { get; set; }
        public virtual DbSet<ProDelivery> ProDelivery { get; set; }
        public virtual DbSet<ProOrder> ProOrder { get; set; }
        public virtual DbSet<ProOrderDetail> ProOrderDetail { get; set; }
        public virtual DbSet<ProProduct> ProProduct { get; set; }
        public virtual DbSet<ProUser> ProUser { get; set; }
        public virtual DbSet<ProUserDocument> ProUserDocument { get; set; }
        public virtual DbSet<ProUserMap> ProUserMap { get; set; }
        public virtual DbSet<ProUserType> ProUserType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=3.7.102.61;Database=Pharmatise;User ID=sa;Password=xg7TuZ&f=Kx6M&d@)pX?ELpqiUmJ!ptT;");
                //optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=pdmdb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MasAddressType>(entity =>
            {
                entity.HasKey(e => e.AddressTypeId)
                    .HasName("PK__tmp_ms_x__8BF56C21BBBB108D");

                entity.Property(e => e.AddressType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<MasCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PK__tmp_ms_x__19093A0B028FE5A0");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<MasCountry>(entity =>
            {
                entity.HasKey(e => e.CountryId)
                    .HasName("PK__MasCount__10D1609F6250D532");

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<MasDeliveryType>(entity =>
            {
                entity.HasKey(e => e.DeliveryTypeId)
                    .HasName("PK__MasDeliv__6B117964E6FD4A23");

                entity.Property(e => e.DeliveryType)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<MasOrderType>(entity =>
            {
                entity.HasKey(e => e.OrderTypeId)
                    .HasName("PK__tmp_ms_x__23AC266C53873203");

                entity.Property(e => e.OrderType)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<MasPickupType>(entity =>
            {
                entity.HasKey(e => e.PickupTypeId)
                    .HasName("PK__tmp_ms_x__F59C5ED11B0FBB27");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.PickupType)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<MasRole>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK__MasRole__8AFACE1ACD3317B7");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<MasState>(entity =>
            {
                entity.HasKey(e => e.StateId)
                    .HasName("PK__MasState__C3BA3B3AE730C28B");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.StateCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.StateName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.MasState)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MasState_MasCountry");
            });

            modelBuilder.Entity<MasStrength>(entity =>
            {
                entity.HasKey(e => e.StrengthId)
                    .HasName("PK__tmp_ms_x__BD0FE8BF42295F64");

                entity.Property(e => e.Strength)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<MasType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK__tmp_ms_x__516F03B5584046B3");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<MasUom>(entity =>
            {
                entity.HasKey(e => e.UomId)
                    .HasName("PK__tmp_ms_x__F6F8D47E6760F205");

                entity.Property(e => e.Uom)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<MasUserDocumentType>(entity =>
            {
                entity.HasKey(e => e.DocumentTypeId)
                    .HasName("PK__MasUserD__DBA390E1648B72DF");

                entity.Property(e => e.DocumentName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<MasUserRoute>(entity =>
            {
                entity.HasKey(e => e.RouteId)
                    .HasName("PK__MasUserR__80979B4D0F640A63");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RouteName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<MsaContactType>(entity =>
            {
                entity.HasKey(e => e.ContactTypeId)
                    .HasName("PK__tmp_ms_x__17E1EE128AE907FE");

                entity.Property(e => e.ContactType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ProAddress>(entity =>
            {
                entity.HasKey(e => e.AddressId)
                    .HasName("PK__ProAddre__091C2AFB82737C16");

                entity.Property(e => e.Address1).HasMaxLength(100);

                entity.Property(e => e.Address2).HasMaxLength(100);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.PostCode)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.HasOne(d => d.AddressType)
                    .WithMany(p => p.ProAddress)
                    .HasForeignKey(d => d.AddressTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProAddress_MasAddressType");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.ProAddress)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProAddress_MasCountry");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.ProAddress)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProAddress_MasState");
            });

            modelBuilder.Entity<ProBranch>(entity =>
            {
                entity.HasKey(e => e.BranchId)
                    .HasName("PK__tmp_ms_x__A1682FC5932A52D3");

                entity.Property(e => e.BranchName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OrderPrefix)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.OrderSuffix)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.ProBranch)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProBranch_ProAddress");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.ProBranch)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProBranch_ProCompany");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProBranchCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProBranch_CreatedUser");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ProBranchModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProBranch_ModifiedUser");
            });

            modelBuilder.Entity<ProCaseSheet>(entity =>
            {
                entity.HasKey(e => e.CaseSheetId)
                    .HasName("PK__tmp_ms_x__641D067BEEEACFEA");

                entity.Property(e => e.CaseSheetName).HasMaxLength(200);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProCaseSheetCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProCaseSheet_CreatedUser");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ProCaseSheet)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProCaseSheet_ProCustomer");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ProCaseSheetModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProCaseSheet_ModifiedUser");
            });

            modelBuilder.Entity<ProCaseSheetDetail>(entity =>
            {
                entity.HasKey(e => e.CaseSheetDetailId);

                entity.Property(e => e.CaseSheetDetailId).ValueGeneratedNever();

                entity.Property(e => e.Caption).HasMaxLength(100);

                entity.Property(e => e.DocumentPath).IsRequired();

                entity.HasOne(d => d.CaseSheet)
                    .WithMany(p => p.ProCaseSheetDetail)
                    .HasForeignKey(d => d.CaseSheetId)
                    .HasConstraintName("FK_ProCaseSheetDetail_ProCaseSheet");
            });

            modelBuilder.Entity<ProCompany>(entity =>
            {
                entity.HasKey(e => e.CompanyId)
                    .HasName("PK__tmp_ms_x__2D971CACE69F2578");

                entity.Property(e => e.CompanyName).HasMaxLength(100);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.LicenseCode).HasMaxLength(100);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProCompanyCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProCompany_CreatedUser");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ProCompanyModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProCompany_ModifiedUser");
            });

            modelBuilder.Entity<ProContact>(entity =>
            {
                entity.HasKey(e => new { e.AddressId, e.ContactTypeId });

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.ProContact)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProContact_ProAddress");

                entity.HasOne(d => d.ContactType)
                    .WithMany(p => p.ProContact)
                    .HasForeignKey(d => d.ContactTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProContact_MsaContactType");
            });

            modelBuilder.Entity<ProCustomer>(entity =>
            {
                entity.HasKey(e => e.CustomerId)
                    .HasName("PK__tmp_ms_x__A4AE64D83B1A3F6C");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("datetime");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastOrderDate).HasColumnType("datetime");

                entity.Property(e => e.MiddleName).HasMaxLength(100);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.NextOrderDate).HasColumnType("datetime");

                entity.Property(e => e.Nhsnumber)
                    .IsRequired()
                    .HasColumnName("NHSNumber")
                    .HasMaxLength(10);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.ProCustomer)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_ProCustomer_ProAddress");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProCustomer)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProCustomer_CreatedUser");
            });

            modelBuilder.Entity<ProDelivery>(entity =>
            {
                entity.HasKey(e => e.DeliveryId)
                    .HasName("PK__tmp_ms_x__626D8FCE186B7E3E");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DeliveryDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProDeliveryCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProDelivery_CreatedUser");

                entity.HasOne(d => d.DeliveryType)
                    .WithMany(p => p.ProDelivery)
                    .HasForeignKey(d => d.DeliveryTypeId)
                    .HasConstraintName("FK_ProDelivery_MasDeliveryType");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ProDeliveryModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProDelivery_ModifiedUser");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.ProDelivery)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProDelivery_ProOrder");
            });

            modelBuilder.Entity<ProOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PK__tmp_ms_x__C3905BCF4E020F49");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.ProOrder)
                    .HasForeignKey(d => d.BranchId)
                    .HasConstraintName("FK_ProOrder_ProBranch");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.ProOrder)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_ProOrder_ProCompany");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProOrderCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProOrder_CreatedUser");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ProOrder)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_ProOrder_ProCustomer");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ProOrderModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProOrder_ModifiedUser");

                entity.HasOne(d => d.OrderType)
                    .WithMany(p => p.ProOrder)
                    .HasForeignKey(d => d.OrderTypeId)
                    .HasConstraintName("FK_ProOrder_MasOrderType");

                entity.HasOne(d => d.PickupType)
                    .WithMany(p => p.ProOrder)
                    .HasForeignKey(d => d.PickupTypeId)
                    .HasConstraintName("FK_ProOrder_MasPickupType");
            });

            modelBuilder.Entity<ProOrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId)
                    .HasName("PK__ProOrder__D3B9D36C74B3541E");

                entity.Property(e => e.Duration).HasMaxLength(50);

                entity.Property(e => e.Quantity).HasColumnType("decimal(10, 5)");

                entity.Property(e => e.Remarks).HasMaxLength(200);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.ProOrderDetail)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProOrderDetail_ProOrder");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProOrderDetail)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProOrderDetail_ProProduct");
            });

            modelBuilder.Entity<ProProduct>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("PK__tmp_ms_x__B40CC6CDB6C863D7");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.ProductCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ProProduct)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProProduct_MasCategory");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProProductCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProProduct_CreatedUser");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ProProductModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProProduct_ModifiedUser");

                entity.HasOne(d => d.Strength)
                    .WithMany(p => p.ProProduct)
                    .HasForeignKey(d => d.StrengthId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProProduct_MasStrength");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.ProProduct)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProProduct_MasType");

                entity.HasOne(d => d.Uom)
                    .WithMany(p => p.ProProduct)
                    .HasForeignKey(d => d.UomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProProduct_MasUom");
            });

            modelBuilder.Entity<ProUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__tmp_ms_x__1788CC4CE0A0CABF");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.MiddleName).HasMaxLength(100);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.ProUser)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_ProUser_ProAddress");

                entity.HasOne(d => d.BranchAdmin)
                    .WithMany(p => p.InverseBranchAdmin)
                    .HasForeignKey(d => d.BranchAdminId)
                    .HasConstraintName("FK_ProUser_BranchAdmin");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.InverseCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProUser_CreatedUser");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.InverseModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProUser_ModifiedUser");

                entity.HasOne(d => d.UserType)
                    .WithMany(p => p.ProUser)
                    .HasForeignKey(d => d.UserTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProUser_ProUserType");
            });

            modelBuilder.Entity<ProUserDocument>(entity =>
            {
                entity.HasKey(e => e.UserDocumentId);

                entity.Property(e => e.Caption).HasMaxLength(100);

                entity.Property(e => e.DocumentPath).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ProUserDocument)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ProUserDocument_ProUser");
            });

            modelBuilder.Entity<ProUserMap>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.CustomerId, e.BranchId, e.CompanyId })
                    .HasName("PK__ProUserM__83B19B5F5AF159BD");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.ProUserMap)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProUserMap_ProBranch");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.ProUserMap)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProUserMap_ProCompany");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ProUserMap)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ProUserMap_ProUser");
            });

            modelBuilder.Entity<ProUserType>(entity =>
            {
                entity.HasKey(e => e.UserTypeId)
                    .HasName("PK__ProUserT__40D2D8168349D97B");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UserType)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
