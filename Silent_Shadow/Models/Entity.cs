using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Silent_Shadow.Models 
{
	public abstract class Entity
	{
		#region variables
		protected Texture2D Sprite { get; set; }
		protected Vector2 SpriteOffset { get; set; }
		protected Color Tint { get; set; } = Color.White;
		public Vector2 Position { get; set; }
		public float Rotation { get; set; }
		public bool IsExpired { get; set; }
		public float Size { get; set; }
		public float Speed { get; set; }
		public virtual Rectangle Bounds
		{
			get
			{
				return new Rectangle(
					(int)(Position.X - (Sprite.Width / 2 * Size)),
					(int)(Position.Y - (Sprite.Height / 2 * Size)),
					(int)(Sprite.Width * Size),
					(int)(Sprite.Height * Size)
				);
			}
		}
		// Standardmäßig auf 0f gesetzt, damit die Entität normal gezeichnet wird
        public float LayerDepth { get; set; } = 0.5f;
		#endregion

		#region Logic

		/// <summary>
		/// Update is Called every Frame
		/// </summary>
		public virtual void Update() {}

		/// <summary>
		/// LateUpdate is called every Frame, after Update
		/// </summary>
		public virtual void LateUpdate() {}

		/// <summary>
		/// Handles the destrucktion of an object
		/// </summary>
		public virtual void Die()
		{
			IsExpired = true;
		}

		#endregion

		/// <summary>
		/// Draws the Entity
		/// </summary>
		/// 
		/// <param name="spriteBatch"></param>
		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Sprite, Position, null, Tint, Rotation, SpriteOffset, Size, 0, LayerDepth);
		}

		#if DEBUG
		/// <summary>
		/// Draws some dubug visualisations
		/// </summary>
		/// 
		/// <param name="spriteBatch"></param>
		public virtual void DrawDebug(SpriteBatch spriteBatch) {}
		#endif
	}
}
