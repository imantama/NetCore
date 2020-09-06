using Microsoft.EntityFrameworkCore;
using NetCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Context
{
    public class MyContext:DbContext
    {
        

        public MyContext(DbContextOptions<MyContext> options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<UserRole> userRoles { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Entity<UserRole>().HasKey(sc => new { sc.UserId, sc.RoleId });
        //    modelBuilder.Entity<UserRole>().HasKey(sc => sc.UserId);

        //    modelBuilder.Entity<UserRole>()
        //        .HasOne<User>(sc => sc.user)
        //        .WithMany(s => s.userRoles)
        //        .HasForeignKey(sc => sc.UserId);

        //    //base.OnModelCreating(modelBuilder);
        //}
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<UserRole>()
        //        .HasKey(ur => new { ur.UserId, ur.RoleId });
        //    modelBuilder.Entity<UserRole>()
        //        .HasOne(ur => ur.user)
        //        .WithMany(b => b.userRoles)
        //        .HasForeignKey(ur => ur.UserId);
        //    modelBuilder.Entity<UserRole>()
        //        .HasOne(ur => ur.role)
        //        .WithMany(c => c.userRoles)
        //        .HasForeignKey(ur => ur.RoleId);

        //    base.OnModelCreating(modelBuilder);
        //}
    }
    

}
