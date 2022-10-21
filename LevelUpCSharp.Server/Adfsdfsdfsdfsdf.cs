﻿using System.Collections.Generic;
using System.Linq;
using LevelUpCSharp.Networking;
using LevelUpCSharp.Production;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Server
{
	[Ctrl("v")]
	internal class Adfsdfsdfsdfsdf
	{
		private readonly IEnumerable<Vendor> _vendors;

		public Adfsdfsdfsdfsdf(IEnumerable<Vendor> vendors)
		{
			_vendors = vendors;
		}

		[Worker("s")]
		public IEnumerable<Sandwich> Sandwiches()
		{
			return _vendors.SelectMany(v => v.Buy()).ToArray();
		}
	}
}