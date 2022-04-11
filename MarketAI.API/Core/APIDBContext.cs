using MarketAI.API.Models;
using MarketAI.API.Models.Stats;
using MarketAI.API.Models.WB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WildberriesAPI.Models;

namespace MarketAI.API.Core
{
    public class APIDBContext : DbContext
    {
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<PromocodeModel> Promocodes { get; set; }
        public DbSet<SubscriptionModel> Subscriptions { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        public DbSet<TicketMessage> TicketMessages { get; set; }
        public DbSet<TicketModel> Tickets { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<WBAPITokenModel> WBAPITokens { get; set; }
        public DbSet<SMSCode> SMSCodes { get; set; }



        public DbSet<PaymentModel> PaymentModels { get; set; }
        public DbSet<AuthStatsModel> AuthStatsModels { get; set; }

        public DbSet<UserData> UserDatas { get; set; }
        public DbSet<SelfCostModel> SelfCosts { get; set; }
        public DbSet<ExtraExpenseModel> ExtraExpenses { get; set; }


        public DbSet<WBIncomeModel> WBIncomes { get; set; }
        public DbSet<WBOrderModel> WBOrders { get; set; }
        public DbSet<WBSaleModel> WBSales { get; set; }
        public DbSet<WBStockModel> WBStocks { get; set; }
        public DbSet<DetailByPeriodModel> DetailByPeriodModels { get; set; }

        public DbSet<AvailableWBBrand> AvailableWBBrands { get; set; }
        public DbSet<AvailableWBCategory> AvailableWBCategories { get; set; }

        public APIDBContext() : base()
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
            if (!Database.EnsureCreatedAsync().Result)
                Database.MigrateAsync();
        }
        public APIDBContext(DbContextOptions<APIDBContext> options) : base(options)
        {
            if (!Database.EnsureCreated())
                Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                       "server=localhost;" +
                       "user=root;" +
                       "password=H2c7V7p6;" +
                       "port=3306;" +
                       "database=marketwb;" +
                       "Convert Zero Datetime= true;",
                       new MySqlServerVersion(new Version(8, 0, 11)),
                       o =>
                       {
                           o.EnableRetryOnFailure(100);
                       });
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketModel>().HasMany(c => c.Messages)/*.WithOne(e => e.Owner);*/;

            modelBuilder.Entity<WBAPITokenModel>().HasMany(c => c.SelfCosts).WithOne(e => e.WBAPIToken);
            modelBuilder.Entity<WBAPITokenModel>().HasMany(c => c.ExtraExpenses).WithOne(e => e.WBAPIToken);

            modelBuilder.Entity<UserModel>().HasOne(c => c.UserData);
            modelBuilder.Entity<UserModel>().HasMany(c => c.WBAPIKeys).WithOne(e => e.Owner);
            modelBuilder.Entity<UserModel>().HasMany(c => c.Auths).WithOne(e => e.User);
            modelBuilder.Entity<UserModel>().HasMany(c => c.Tickets).WithOne(e => e.OpenedBy);

            //modelBuilder.Entity<TicketMessage>().HasOne(o => o.SentBy);

            modelBuilder.Entity<WBIncomeModel>().HasIndex(o => o.APIKey);
            modelBuilder.Entity<DetailByPeriodModel>().HasIndex(o => o.APIKey);
            modelBuilder.Entity<WBOrderModel>().HasIndex(o => o.APIKey);
            modelBuilder.Entity<WBSaleModel>().HasIndex(o => o.APIKey);
            modelBuilder.Entity<WBStockModel>().HasIndex(o => o.APIKey);
        }
    }
}
