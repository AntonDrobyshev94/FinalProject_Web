using FinalProject_Web.AuthFinalProjectApp;
using FinalProject_Web.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FinalProject_Web.ContextFolder
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions options) : base(options) { }
        public DbSet<Application> Requests { get; set; }
    }
}
