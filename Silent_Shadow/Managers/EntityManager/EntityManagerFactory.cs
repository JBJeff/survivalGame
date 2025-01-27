
namespace Silent_Shadow.Managers.EntityManager
{
	/// <summary>
	/// Factory for creating instances of IEntityManager.
	/// </summary>
	/// 
	/// <author>Jonas Schwind</author>
	/// <version>1.0</version>
	/// 
	/// <seealso cref="IEntityManager"/>
	public static class EntityManagerFactory
	{
		/// <summary>
		/// Creates an instance of a <see cref="IEntityManager"/>.
		/// </summary>
		/// 
		/// <returns>An instance of <see cref="IEntityManager"/>.</returns>
		public static IEntityManager GetInstance()
		{
			return EntityManager.Instance;
		}
	}
}
