using System;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Production.Ingredients
{
	internal class Chicken : IKeyIngredient
	{
		public DateTime ExpDate { get; }

		public SandwichKind Kind => SandwichKind.Chicken;
	}
}