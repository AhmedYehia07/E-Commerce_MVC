﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMastery.Models.ViewModels
{
	public class ShoppingCartVm
	{
		public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
		public double OrderTotal { get; set; }
	}
}