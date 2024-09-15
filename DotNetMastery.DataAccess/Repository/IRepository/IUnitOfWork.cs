using Microsoft.EntityFrameworkCore.Diagnostics;
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
		public void Save();
	}
}
