using LostInCryptic.Shared.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostInCryptic.Server.DataContext
{
    public class WebsiteDbContext : DbContext
    {
        public DbSet<Question> Questions { get; set; }
        public DbSet<Word> UrlCodes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
          => options.UseSqlite("Data Source=lostInCryptic.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Question>().Ignore(q => q.Number);
        }
    }
}
