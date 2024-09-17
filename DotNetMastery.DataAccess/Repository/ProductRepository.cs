using Bulky.DataAccess.Data;
using Bulky.Models;
using DotNetMastery.DataAccess.Repository.IRepository;
using DotNetMastery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMastery.DataAccess.Repository
{
    internal class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _db = dbContext;
        }
        public void Update(Product product)
        {
            _db.Products.Update(product);
        }
    }
}
