using MarketAI.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public APIDBContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                       "server=localhost;" +
                       "user=root;" +
                       "password=H2c7V7p6;" +
                       "port=3306;" +
                       "database=marketwb;",
                       new MySqlServerVersion(new Version(8, 0, 11))
                   );
        }
    }
}
