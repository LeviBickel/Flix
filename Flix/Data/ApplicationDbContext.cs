using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Flix.Models;

namespace Flix.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Flix.Models.Movies> Movies { get; set; }
        public DbSet<Flix.Models.UserRolesView> UserRolesView { get; set; }
        public DbSet<Flix.Models.UserWatchedAssociation> UsersWatched { get; set; }
        public DbSet<Flix.Models.TVShowRelationship> tvShowRelationships { get; set; }
        public DbSet<Flix.Models.AdminSettings> AdminSettings { get; set; }
        
    }
}
