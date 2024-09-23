using Bulky.DataAccess.Data;
using DotNetMastery.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMastery.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _db;
		public ICategoryRepository Category {  get; private set; }

        public IProductRepository Product {  get; private set; }

        public ICompanyRepository Company {  get; private set; }


        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _db = dbContext;
			Category = new CategoryRepository(_db);
			Product = new ProductRepository(_db);
			Company = new CompanyRepository(_db);
        }
        
		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
