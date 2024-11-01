﻿using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMastery.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		public ICategoryRepository Category { get; }
		public IProductRepository Product { get; }
		public ICompanyRepository Company { get; }
		public IShoppingCartRepository ShoppingCart { get; }
		public IApplicationUserRepository ApplicationUser { get; }
		public IOrderHeaderRepository OrderHeader { get; }
		public IOrderDetailRepository OrderDetail { get; }
        public void Save();
	}
}
