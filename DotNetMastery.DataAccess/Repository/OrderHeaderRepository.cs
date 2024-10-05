using Bulky.DataAccess.Data;
using DotNetMastery.DataAccess.Repository.IRepository;
using DotNetMastery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMastery.DataAccess.Repository
{
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void update(OrderHeader orderHeader)
		{
			_db.orderHeaders.Update(orderHeader);
		}

		public void updateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var order = _db.orderHeaders.FirstOrDefault(x => x.Id == id);
			if (order != null)
			{
				order.OrderStatus = orderStatus;
				if(!string.IsNullOrEmpty(paymentStatus))
				{
					order.PaymentStatus = paymentStatus;
				}
				order.OrderDate = DateTime.Now;
			}
		}

		public void updateStripePaymentID(int id, string sessionID, string? paymentIntentID)
		{
			var order = _db.orderHeaders.FirstOrDefault(x => x.Id == id);
            if (!string.IsNullOrEmpty(sessionID))
            {
				order.SessionId = sessionID;
            }
			if (!string.IsNullOrEmpty(paymentIntentID))
			{
				order.PaymentIntentId = paymentIntentID;
			}
		}
	}
}
