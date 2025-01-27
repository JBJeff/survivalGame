
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silent_Shadow.Models;

namespace Silent_Shadow.Managers.EntityManager
{
	/// <summary>
	/// Manages all Entitys
	/// </summary>
	/// 
	/// <author>Jonas Schwind</author>
	/// <version>1.0</version>
	public interface IEntityManager
	{
		public List<Entity> GetEntities();
		public int GetTotalGruntCount();
		public int GetGruntKilledCount();

		/// <summary>
		/// Add an Entity
		/// </summary>
		/// <param name="entity">Entity to add</param>
		public void Add(Entity entity);

		public void Clear();

		/// <summary>
		/// Reset Kill Counters
		/// </summary>
		public void ResetCounters();
		public void CheckForEntityInTriangle(Entity activator, Vector2 heroPos, Vector2 leftPoint, Vector2 rightPoint);
		public void ResetLevel();
		public void ClearAllEntities();

		/// <summary>
		/// Toggles a electrical lighsource entititys
		/// </summary>		
		public void Blackout();

		/// <summary>
		/// Updates all entitys
		/// </summary>
		public void Update();

		/// <summary>
		/// Draws all entitys
		/// </summary>
		public void Draw(SpriteBatch spriteBatch);

		#if DEBUG
		public void DrawDebug(SpriteBatch spriteBatch);
		#endif
	}
}
