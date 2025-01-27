using Microsoft.Xna.Framework;

namespace Silent_Shadow.Models
{
	public class CollisionObject
	{
		private float _x;
		private float _y;
		private float _width;
		private float _height;

		public Rectangle Bounds
		{
			get
			{
				return new Rectangle(
					(int)(_x * Globals.TileMapScale),
					(int)(_y * Globals.TileMapScale),
					(int)(_width * Globals.TileMapScale),
					(int)(_height * Globals.TileMapScale)
				);
			}
		}

		public CollisionObject(float x, float y, float width, float height)
		{
			_x = x;
			_y = y;
			_width = width;
			_height = height;
		}
	}

	//UNNÖTIG BIS JETZT
	public class Tile
	{
		public string Name { get; set; }
		public int Id { get; set; }

	
	}

}
