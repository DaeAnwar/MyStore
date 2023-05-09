using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStore.Models;

namespace MyStore.Data
{
    public class MyStoreContext : DbContext
    {
        public MyStoreContext (DbContextOptions<MyStoreContext> options)
            : base(options)
        {
        }

        public DbSet<MyStore.Models.ball> ball { get; set; } = default!;

        public DbSet<MyStore.Models.usersaccounts>? usersaccounts { get; set; }

        public DbSet<MyStore.Models.orders>? orders { get; set; }
        public DbSet<MyStore.Models.report>? report { get; set; }

    }
}
