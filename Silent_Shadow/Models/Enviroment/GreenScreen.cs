using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silent_Shadow.Models.Animations;

namespace Silent_Shadow.Models.Enviroment
{
	public class GreenScreen : LightSource
	{
		private readonly Texture2D _textureDeactivated;
		private readonly Animation _spriteAnimation;

		public GreenScreen(Vector2 position, float radius) : base(position, radius, Color.Green)
		{
			Sprite = Globals.Content.Load<Texture2D>("Sprites/greenscreen");
			_textureDeactivated = Globals.Content.Load<Texture2D>("LooseSprites/screen_off");
			_spriteAnimation = new Animation(Sprite, 4, 1, 0.25f);
			Size = 1f;
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