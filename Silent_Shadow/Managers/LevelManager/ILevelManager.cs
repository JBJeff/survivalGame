using Microsoft.Xna.Framework.Graphics;
using Silent_Shadow.Models;

namespace Silent_Shadow.Managers.LevelManager
{
	/// <summary>
	/// Manages Levels
	/// </summary>
	/// 
	/// <author>Jeffrey Böttcher</author>
	/// <version>1.0</version>
	public interface ILevelManager
	{
		/// <summary>
		/// Loads a Level
		/// </summary>
		/// 
		/// <param name="level"><see cref="Level"/> to load</param>
		public Level LoadLevel(string level);

		/// <summary>
		/// Loads the Entitys of a given Level
		/// </summary>
		/// 
		/// <param name="level"><see cref="Level"/> which objects should be loaded</param>
		public void LoadEntitys(Level level);

		/// <summary>
		/// Draws a given Level
		/// </summary>
		/// 
		/// <param name="spriteBatch"></param>
		/// <param name="level"><see cref="Level"/> to Draw</param>
		public void Draw(SpriteBatch spriteBatch, Level level);

		#if DEBUG
		/// <summary>
		/// Draws debug visualisation
		/// </summary>
		/// 
		/// <param name="spriteBatch"></param>
		/// <param name="level"><see cref="Level"/> to Draw</param>
		public void DrawDebug(SpriteBatch spriteBatch, Level level) {}
		#endif
	}
}