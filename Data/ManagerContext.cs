using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManager.Data
{
    public class ManagerContext : IdentityDbContext
    {
        public ManagerContext(DbContextOptions<ManagerContext> options) : base(options)
        {
        }

        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectKeyword> ProjectKeywords { get; set; }
        public DbSet<Student> Students { get; set; }
    }
}
