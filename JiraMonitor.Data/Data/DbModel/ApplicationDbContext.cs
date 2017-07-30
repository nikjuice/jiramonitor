using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JiraMonitor.Data.DbModel;

namespace JiraMonitor.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<JqlQuery> JqlQuery { get; set; }

        public DbSet<EmailNotificationSettings> EmailNotificationSettings { get; set; }

        public DbSet<JiraSettings> JiraSettings { get; set; }

        public DbSet<TrackedJiraIssue> TrackedJiraIssue { get; set; }
    }
}
