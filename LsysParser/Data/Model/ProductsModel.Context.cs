﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LsysParser.Data.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class ProductsContext : DbContext
    {
        public ProductsContext()
            : base("name=ProductsContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductFile> ProductFile { get; set; }
        public virtual DbSet<Property> Property { get; set; }
        public virtual DbSet<PropertyName> PropertyName { get; set; }
        public virtual DbSet<PropertyValue> PropertyValue { get; set; }
        public virtual DbSet<Error> Error { get; set; }
    }
}
