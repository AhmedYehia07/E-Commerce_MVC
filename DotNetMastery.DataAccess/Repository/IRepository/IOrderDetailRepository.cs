using DotNetMastery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMastery.DataAccess.Repository.IRepository
{
	public interface IOrderDetailRepository: IRepository<OrderDetail>
	{
		void update(OrderDetail orderDetail);
	}
}
