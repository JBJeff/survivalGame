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
using Silent_Shadow._Managers.SaveManager;
using Silent_Shadow.Controls;
using Silent_Shadow.Managers;
using Silent_Shadow.Models;


namespace Silent_Shadow.States
{
	public class PauseMenu : State
	{
		private List<Component> _components;


		public PauseMenu(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
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
				Position = new Vector2(graphicsDevice.Viewport.Width / 2 - (buttonTexture.Width / 2), 300),
				Text = "Neues Spiel",
			};
			newGameButton.Click += Button_NewGame_Clicked;

			var exitGameButton = new Button(buttonTexture, buttonFont)
			{
				Position = new Vector2(graphicsDevice.Viewport.Width / 2 - (buttonTexture.Width / 2), 500),
				Text = "Verlassen",
			};
			exitGameButton.Click += Button_ExitGame_Clicked;

			_components = new List<Component>()
			{
				//resumeGameButton,
				//loadGameButton,
				newGameButton,
				achievementButton,
				exitGameButton,
			};
			
			if (!GameState.IsGameOver) 
			{
				var resumeGameButton = new Button(buttonTexture, buttonFont)
				{
					Position = new Vector2(graphicsDevice.Viewport.Width / 2 - (buttonTexture.Width / 2), 200),
					Text = "Fortsetzen",
				}; resumeGameButton.Click += Button_ResumeGame_Clicked;

				_components.Add(resumeGameButton);
			}
			//if (GameState.IsGameOver)
			//{
			//	var resumeGameButton = new Button(buttonTexture, buttonFont)
			//	{
			//		Position = new Vector2(graphicsDevice.Viewport.Width / 2 - (buttonTexture.Width / 2), 400),
			//		Text = "Neues Spiel",
			//	};
			//	resumeGameButton.Click += Button_NewGame_Clicked;
			//}

			if (GameState.Instance != null && GameState.Instance.GetCurrentLevelIndex() >= 1) // Level-Index startet bei 0, also Level 2 ist Index 1
			{
				var loadCheckpointButton = new Button(buttonTexture, buttonFont)
				{
					Position = new Vector2(graphicsDevice.Viewport.Width / 2 - (buttonTexture.Width / 2), 200),
					Text = "Checkpoint",
				}; loadCheckpointButton.Click += Button_LoadCheckpoint_Clicked;

				_components.Add(loadCheckpointButton);
			}

			}
		private void Button_Achievements_Clicked(object sender, EventArgs e)
        {
            _game.ChangeState(new AchievementMenu(_game, _graphicsDevice, _content));
        }
		private void Button_NewGame_Clicked(object sender, EventArgs e)
		{
			SoundManager.StopMusic();

			// Setze Spielzustand zurück
			GameResetService.RestartGameComplete();

		}
		private void Button_ResumeGame_Clicked(object sender, EventArgs e)
		{
			if (GameState.Instance != null) // sicherstellung
			{
				_game.ChangeState(GameState.Instance); // Zurück zum aktuellen Spielzustand
				Debug.WriteLine("Spiel wurde aus dem Pause-Menü fortgesetzt.");
			}
			else
			{
				Debug.WriteLine("Kein aktiver Spielzustand vorhanden.");
			}

		}
		/*
		 *	#TODO
		 *	funktion zum Speichern und Laden Implementieren
		 *
		 */
		private void Button_SaveGame_Clicked(object sender, EventArgs e)
		{
			Debug.WriteLine("Spiel Speichern");
		}

		private void Button_LoadCheckpoint_Clicked(object sender, EventArgs e)
		{
			GameState.IsGameOver = false;
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

		private void Button_ExitGame_Clicked(object sender, EventArgs e)
		{
			System.Threading.Thread.Sleep(1000);
			_game.Exit();
		}

		public override void Update(GameTime gameTime)
		{
			foreach (var component in _components)
			{
				component.Update(gameTime);
			}
				
		}
		
		public override void PostUpdate(GameTime gameTime)
		{
			//Sprites enfernen wenn sie nicht benötigt werden
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			spriteBatch.Begin();

			foreach (var component in _components)
			{
				component.Draw(gameTime, spriteBatch);
			}

			spriteBatch.End();
		}
	}
}