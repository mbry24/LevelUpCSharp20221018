using System;

namespace LevelUpCSharp.Production
{
	public static class PriceExtensions
	{
		public static decimal GetWithVat(this decimal source, int vat)
		{
			if (vat < 0 || vat > 23)
			{
				throw new ArgumentException("out of range");
			}

			return source * (1 + (decimal) vat / 100);
		}

		public static decimal GetVat(this decimal source, int vat)
		{
			if (vat < 0 || vat > 23)
			{
				throw new ArgumentException("out of range");
			}

			return source * (decimal)vat / 100;
		}
	}
}
