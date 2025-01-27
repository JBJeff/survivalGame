
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Silent_Shadow.Controls;
using Silent_Shadow.Managers;

namespace Silent_Shadow.States
{
	public class AchievementMenu : State
	{
		private List<Component> _components;

		public AchievementMenu(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
			: base(game, graphicsDevice, content)
		{
			var buttonTexture = _content.Load<Texture2D>("Controls/Button200");
			var buttonFont = _content.Load<SpriteFont>("Tahoma");

			var backButton = new Button(buttonTexture, buttonFont)
			{
				Position = new Vector2(graphicsDevice.Viewport.Width / 2 - (buttonTexture.Width / 2), 400),
				Text = "Zurück",
			};
			backButton.Click += Button_Back_Clicked;

			_components = new List<Component>() { backButton };
		}

		private void Button_Back_Clicked(object sender, EventArgs e)
		{
			if (GameState.Instance != null)
			{
				_game.ChangeState(new PauseMenu(_game, _graphicsDevice, _content));
			}
			else
			{
				_game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
			}

		}

		public override void Update(GameTime gameTime)
		{
			foreach (var component in _components)
				component.Update(gameTime);
		}

		public override void PostUpdate(GameTime gameTime)
		{
			//Sprites enfernen wenn sie nicht benötigt werden
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			spriteBatch.Begin();

			// Draw achievements
			var font = Globals.Content.Load<SpriteFont>("Tahoma");
			var yPosition = 100;
			foreach (var achievement in AchievementManager.Achievements)
			{
				spriteBatch.DrawString(font, $"{achievement.Key}: Kills {achievement.Value} Level {achievement.Value / 10}", new Vector2(100, yPosition), Color.White);
				yPosition += 50;
			}

			foreach (var component in _components)
				component.Draw(gameTime, spriteBatch);

			spriteBatch.End();
		}

#if DEBUG
		public override void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch)
		{

		}
#endif
	}
}