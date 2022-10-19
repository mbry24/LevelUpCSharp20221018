using System;

namespace LevelUpCSharp.Production.Ingredients
{
	internal class Tomato : IGarnish
	{
		public Tomato()
		{
			ExpDate = DateTime.Now.AddDays(4);
		}

		public DateTime ExpDate { get; }

		public string Name => "Tommato";
	}
}