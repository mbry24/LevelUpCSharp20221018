using System;

namespace LevelUpCSharp.Production.Import
{
	public class ImportedGarnish : IGarnish
	{
		public ImportedGarnish(DateTime expDate, string name)
		{
			ExpDate = expDate;
			Name = name;
		}

		public DateTime ExpDate { get; }

		public string Name { get; }
	}
}