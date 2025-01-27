
namespace Silent_Shadow.Managers.LevelManager
{
	/// <summary>
	/// Factory for creating instances of ILevelManager
	/// </summary>
	/// 
	/// <author>Jeffrey Böttcher</author>
	/// <version>1.0</version>
	/// 
	/// <seealso cref="ILevelManager"/>
	public static class LevelManagerFactory
	{
		/// <summary>
		/// Creates an instance of a <see cref="IEntityManager"/>.
		/// </summary>
		/// 
		/// <returns>A instance of <see cref="IEntityManager"/>.</returns>
		public static ILevelManager GetInstance()
		{
			return LevelManager.Instance;
		}
	}
}
