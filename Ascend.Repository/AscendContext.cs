
using System;
using System.Collections.Generic;
using System.Text;
using ApiTemplate.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiTemplate.Repository
{
    public class AscendContext : DbContext
    {
        public AscendContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Address> Address{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
