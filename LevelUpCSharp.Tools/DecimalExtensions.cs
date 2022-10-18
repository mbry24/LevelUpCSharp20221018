using System;

namespace LevelUpCSharp
{
	public static class DecimalExtensions
	{
		public static decimal Multiplay(this decimal source, int factor)
		{
			return source * (1 + (decimal)factor / 100);
		}

	}
}