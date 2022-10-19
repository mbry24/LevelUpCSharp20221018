using System;

namespace LevelUpCSharp.Production.Import
{
	/// <summary>
	/// Used to hold items coming from json file.
	/// </summary>
	public class ImportedTopping : ITopping
	{
		public ImportedTopping(DateTime expDate, string name)
		{
			ExpDate = expDate;

			Name = name;
		}

		public DateTime ExpDate { get; }
		public string Name { get; }
	}
}