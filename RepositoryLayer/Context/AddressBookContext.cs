using System;
using RepositoryLayer.Entity;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Context
{
    public class AddressBookContext : DbContext
    {
        public AddressBookContext(DbContextOptions<AddressBookContext> options) : base(options) { }

        // DbSets for UserEntity and ContactEntity
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<ContactEntity> Contacts { get; set; }
    }
}