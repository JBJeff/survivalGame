using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Silent_Shadow._Managers.SaveManager;
using Silent_Shadow.Controls;
using Silent_Shadow.Managers;
using Silent_Shadow.Models;



namespace Silent_Shadow.States
{
	public class MenuState : State
	{
		private List<Component> _components;

		public MenuState(Game1 game, GraphicsDevice graphicsDevice,ContentManager content)
			: base(game, graphicsDevice, content)
		{
			var buttonTexture = _content.Load<Texture2D>("Controls/Button200");
			var buttonFont = _content.Load<SpriteFont>("Tahoma");

			var achievementButton = new Button(buttonTexture, buttonFont)
			{
				Position = new Vector2(graphicsDevice.Viewport.Width / 2 - (buttonTexture.Width / 2), 400),
				Text = "Achievements",
			};
			achievementButton.Click += Button_Achievements_Clicked;

			var newGameButton = new Button(buttonTexture, buttonFont)
			{
				Position = new Vector2(graphicsDevice.Viewport.Width / 2- (buttonTexture.Width/2), 200),
				Text = "Neues Spiel",
			};
			newGameButton.Click += Button_NewGame_Clicked;

			var loadGameButton = new Button(buttonTexture, buttonFont)
			{
				Position = new Vector2(graphicsDevice.Viewport.Width / 2 - (buttonTexture.Width / 2), 300),
				Text = "Laden",
			};
			loadGameButton.Click += Button_LoadGame_Clicked;

			var exitGameButton = new Button(buttonTexture, buttonFont)
			{
				Position = new Vector2(graphicsDevice.Viewport.Width / 2 - (buttonTexture.Width / 2), 500),
				Text = "Verlassen",
			};
			exitGameButton.Click += Button_ExitGame_Clicked;

			_components = new List<Component>()
			{
				achievementButton,
				newGameButton,
				loadGameButton,
				exitGameButton,
			};

			// Überprüfe, ob Level 2 oder höher ist und füge neuen Menüpunkt hinzu
			if (GameState.Instance != null && GameState.Instance.GetCurrentLevelIndex() >= 1) // Level-Index startet bei 0, also Level 2 ist Index 1
			{
				var loadCheckpointButton = new Button(buttonTexture, buttonFont)
				{
					Position = new Vector2(graphicsDevice.Viewport.Width / 2 - (buttonTexture.Width / 2), 500),
					Text = "Letzten Checkpoint laden",
				};
				loadCheckpointButton.Click += Button_LoadCheckpoint_Clicked;

				_components.Add(loadCheckpointButton);
			}

			SoundManager.PlayMenuMusic();
		}

		private void Button_NewGame_Clicked(object sender, EventArgs e)
		{
			// Stoppe die Menü-Musik
			SoundManager.StopMusic();

			// Setze Spielzustand zurück
			GameResetService.RestartGameComplete();

			// Starte ein neues Spiel
			//_game.ChangeState(new GameState(_game, _graphicsDevice, _content));
		}

		private void Button_Achievements_Clicked(object sender, EventArgs e)
		{
			_game.ChangeState(new AchievementMenu(_game, _graphicsDevice, _content));
		}

		private void Button_LoadGame_Clicked(object sender, EventArgs e)
		{
			Debug.WriteLine("Spiel Laden");
		}

		private void Button_ExitGame_Clicked(object sender, EventArgs e)
		{
			System.Threading.Thread.Sleep(1000);
			_game.Exit();
		}

		private void Button_LoadCheckpoint_Clicked(object sender, EventArgs e)
		{
			GameState.IsGameOver = false;
			GameState.IsGameCompleted = false;
			Debug.WriteLine("Lade letzten Checkpoint...");
			SoundManager.StopMusic();
			Hero.Reset();

			// Setzt GameState.Instance auf null, um sicherzustellen, dass eine neue Instanz erstellt wird
			GameState.Instance = null;

			// Lädt die neue GameState-Instanz
			var newGameState = new GameState(_game, _graphicsDevice, _content);
			_game.ChangeState(newGameState);

			// Lade den letzten Checkpoint
			newGameState.LoadGameState();

			Debug.WriteLine("Spielstand erfolgreich geladen und GameState gewechselt.");
		}

		public override void Update(GameTime gameTime)
		{
			foreach(var component in _components)
				component.Update(gameTime);
		}
		public override void PostUpdate(GameTime gameTime)
		{
			//Sprites enfernen wenn sie nicht benötigt werden
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			spriteBatch.Begin();

			foreach (var component in _components)
				component.Draw(gameTime, spriteBatch);

			spriteBatch.End();
		}
	}
}
