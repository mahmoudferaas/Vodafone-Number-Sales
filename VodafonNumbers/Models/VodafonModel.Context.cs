﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VodafonNumbers.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VodafonEntities : DbContext
    {
        public VodafonEntities()
            : base("name=VodafonEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Number> Numbers { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
    }
}
