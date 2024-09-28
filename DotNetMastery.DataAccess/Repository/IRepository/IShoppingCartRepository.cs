using DotNetMastery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMastery.DataAccess.Repository.IRepository
{
	public interface IShoppingCartRepository : IRepository<ShoppingCart>
	{
		void Update (ShoppingCart cart);
	}
}
