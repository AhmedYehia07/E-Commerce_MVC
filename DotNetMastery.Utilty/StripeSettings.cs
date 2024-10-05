using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMastery.Utilty
{
	public class StripeSettings
	{
		public string SecretKey { get; set; }
		public string Publishablekey { get; set; }
	}
}
