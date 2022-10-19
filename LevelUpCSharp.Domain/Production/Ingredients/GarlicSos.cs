using System;

namespace LevelUpCSharp.Production.Ingredients
{
	internal class GarlicSos : ITopping
	{
		public GarlicSos()
		{
			ExpDate = DateTime.Now.AddDays(5);
		}

		public DateTime ExpDate { get; }

		public string Name => "Garlic Sos";
	}
}