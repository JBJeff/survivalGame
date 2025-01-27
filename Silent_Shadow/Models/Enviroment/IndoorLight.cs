
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silent_Shadow.Models.Animations;

namespace Silent_Shadow.Models.Enviroment
{
	/// <summary>
	/// A light thing, it glows
	/// </summary>
	///
	public class IndoorLight : LightSource
	{
		public IndoorLight(Vector2 position, float radius) : base(position, radius, Color.WhiteSmoke)
		{
			Sprite = Globals.Content.Load<Texture2D>("LooseSprites/indoor_light");
			Tint = Color.White * 0.3f;
			LayerDepth = .8f;
			Size = 1f;
		}

		public override void Update()
		{
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Sprite, Position, null, Tint, Rotation, SpriteOffset, Size, SpriteEffects.None, LayerDepth);
		}
	}
}
