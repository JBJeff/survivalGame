
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silent_Shadow.Models.Animations;

namespace Silent_Shadow.Models.Enviroment
{
	/// <summary>
	/// A green light thing, it glows green
	/// </summary>
	///
	/// <remarks>
	///  Didn't have a better name, so it called FruitPunch now.
	/// </remarks>
	public class Fetus : LightSource
	{
		private readonly Animation _spriteAnimation;

		public Fetus(Vector2 position, float radius) : base(position, radius, Color.GreenYellow)
		{
			Sprite = Globals.Content.Load<Texture2D>("Sprites/Fetus");
			_spriteAnimation = new Animation(Sprite, 4, 1, 0.6f);
			Size = 1f;
			Toogleable = false; // NOTE: Chemical light
		}

		public override void Update()
		{
			_spriteAnimation.Update();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			_spriteAnimation.Draw(Position, Rotation, Size, spriteBatch);
		}
	}
}
