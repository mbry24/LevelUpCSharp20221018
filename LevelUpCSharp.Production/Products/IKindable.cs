namespace LevelUpCSharp.Products
{
	public interface IKindable<TKey>
	{
		TKey Kind { get; }
	}
}