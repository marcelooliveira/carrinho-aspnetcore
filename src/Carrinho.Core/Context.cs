using Carrinho.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;

namespace Carrinho.Core
{
    public class Context : DbContext
    {
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<Product> Product { get; set; }

        public Context(DbContextOptions<Context> options) : base(options) { }
    }
}
