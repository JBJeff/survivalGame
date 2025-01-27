using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Silent_Shadow.GUI;
using Silent_Shadow.Managers;
using Silent_Shadow.Models;
using Silent_Shadow.Models.AI;
using System;

namespace Silent_Shadow.States
{
	public abstract class State
	{
		protected Game1 _game;

		protected ContentManager _content;

		protected GraphicsDevice _graphicsDevice;

		public State(Game1 game, GraphicsDevice graphicsDevice,ContentManager content)
		{
			_game = game;
			_graphicsDevice = graphicsDevice;
			_content = content;
		}

		public abstract void Update(GameTime gameTime);

		public abstract void PostUpdate(GameTime gameTime);

		public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

		#if DEBUG
		public virtual void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch) {}
		#endif

		public static implicit operator State(SpriteBatch v)
		{
			throw new NotImplementedException();
		}
	}
}
