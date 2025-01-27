
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Silent_Shadow.Models;
using Silent_Shadow.Managers;
using Silent_Shadow.Managers.EntityManager;
using Silent_Shadow.Managers.CollisionManager;
using Silent_Shadow.Models.AI.Navigation;
using Silent_Shadow.Managers.LevelManager;
using Penumbra;
using Silent_Shadow.GUI;
using Silent_Shadow.Models.Weapons;
using Silent_Shadow._Managers.SaveManager;

namespace Silent_Shadow.States
{
	public class GameState : State
	{
		public static GameState Instance { get; set; }
		public GraphicsDeviceManager Graphics { get; set; }

		private readonly IEntityManager _entityMgr;
		private readonly ILevelManager _levelManager;
		private readonly PenumbraComponent _penumbra;
		//private InfoDisplay _infoDisplay;

		public Level Level { get; set; }
		// zum bestimmen in welchen Level man sich befindet
		public int _currentLevelIndex {get; set;}

		//Statische Liste der Levelnamen
		private readonly List<string> _levelNames = ["firstlevel", "Level1_Bosslevel", "Level_3"];

		//für "Game Over" Szenario
		private static bool _isGameOver = false;      // Flag, um zu erkennen, ob Game Over ist
		private static bool _isGameCompleted = false;    // Flag, um zu erkennen, ob Game Completed ist
		private float _gameOverTimer = 0f;      // Timer für die Verzögerung
		private const float _gameOverDelay = 3f; // Verzögerungszeit in Sekunden

		public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
			: base(game, graphicsDevice, content)
		{
			_currentLevelIndex = 0;
			Instance = this;

			_entityMgr = EntityManagerFactory.GetInstance();
			_penumbra = Globals.Game.Penumbra;
			_levelManager = LevelManagerFactory.GetInstance();

			string _levelname = "firstlevel"; // testlevel
			Level = _levelManager.LoadLevel(_levelname);
			SoundManager.PlayBackgroundMusic(0);
			_levelManager.LoadEntitys(Level);

			foreach (Hull hull in Instance.Level.Hulls)
			{
				_penumbra.Hulls.Add(hull);
			}
			//_penumbra.Hulls.Add(Hero.Instance.Hull);
		}

		public Matrix CalculateTranslation()
		{
			var dx = (Globals.ScreenWidth / 4) - Hero.Instance.Position.X;
			var dy = (Globals.ScreenHeight / 4) - Hero.Instance.Position.Y;
			return Matrix.CreateTranslation(dx, dy, 0) * Matrix.CreateScale(2) ;
		}

		public override void PostUpdate(GameTime gameTime)
		{
			//throw new NotImplementedException();

		}
		
		public override async void Update(GameTime gameTime)
		{
			if (_isGameOver || _isGameCompleted )
			{
				_gameOverTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

				// Nach Ablauf der Verzögerung wechsle ins Menü
				if (_gameOverTimer >= _gameOverDelay)
				{
					GameState.Instance.PauseGame(_game, _graphicsDevice, _content);
				}
				return; // Restliches Update überspringen
			}

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				_game.ChangeState(new PauseMenu(_game, _graphicsDevice, _content));
			}

			//_infoDisplay = new InfoDisplay(font, reloadIcon, new Vector2((Globals.ScreenWidth / 4) + Hero.Instance.Position.X - 100, Hero.Instance.Position.Y - (Globals.ScreenHeight / 4) + 20));

			//Existiert der Hero?
			if (Hero.Instance != null)
			{
				//Hero ist Tod, triggered GameOverSzenario
				if (Hero.Instance.IsExpired)
				{
					Debug.WriteLine("Hero ist tot. Trigger GameOver.");
					TriggerGameOver();
					
				}

				//Sobald der Spieler das level verlassen will wird überprüft ob er berechtigt ist
				if (Hero.Instance.IsExit)
				{

					//überprüft ob der Spieler im Exitbereich ist, wenn wird isExit false und ein neues Level startet
					if (CollisionManager.IsCollidingWithExitpoint(Hero.Instance.Position, GameState.Instance.Level.ExitPoints, -15, -15, 40, 40))
					{
						Hero.Instance.IsExit = false;
						LoadNextLevel();
					}
				}
			}

			Globals.Update(gameTime);
			_entityMgr.Update();
			InputManager.Update();

			// Überprüft, ob das Level abgeschlossen ist
			if (EntityManager.Instance.GruntKilledCount == EntityManager.Instance.TotalGruntCount)
			{
				//Spieler darf level verlassen
				Hero.Instance.IsExit = true;
			}
		}

		//verwendet den Counter um den richtigen Name(Datei) vom Level zu finden
		public void LoadNextLevel()
		{
			_entityMgr.ResetLevel();

			if (_currentLevelIndex < _levelNames.Count - 1)
			{
				_currentLevelIndex++;
				string nextLevel = _levelNames[_currentLevelIndex];
				SoundManager.PlayBackgroundMusic(_currentLevelIndex);
				LoadLevel(nextLevel);
			}
			else
			{
				// Spielende oder Rückkehr zum Hauptmenü
				Debug.WriteLine("Alle Levels abgeschlossen!");
				TriggerGameCompletion();
			}
		}


		//lädt ein Level mit einen bestimmten Namen.
		public void LoadLevel(string levelName)
		{
			_penumbra.Lights.Clear();
			_penumbra.Hulls.Clear();

			try
			{
				//Hero.Initialize(new Vector2(40, 529));
				//Clear Methoden müssten EVENTUELL noch eingefügt werden.
				var newLevel = _levelManager.LoadLevel(levelName);

				if (newLevel == null)
				{
					Debug.WriteLine("Fehler: Level konnte nicht geladen werden.");
					return;
				}

				Instance.Level = newLevel;

				_levelManager.LoadEntitys(Instance.Level);
				// Setzt die neue Startposition
				Hero.Instance.Position = new Vector2(50, 529);

				Debug.WriteLine($"Hulls: {Instance.Level.Hulls.Count}");

				foreach (Hull hull in Instance.Level.Hulls)
				{
					Game1.Instance.Penumbra.Hulls.Add(hull);
				}
				
				Debug.WriteLine("Neues Level erfolgreich geladen!");
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Fehler beim Laden des Levels: " + ex.Message);
			}
		}

		#region Save and Loading Methods
		public void SaveGameState()
		{
			GameSaveService.SaveGameState(_currentLevelIndex, Hero.Instance);
		}

		public void LoadGameState()
		{
			// Entfernt alle alten Entitäten aus dem Spiel
			EntityManager.Instance.ResetLevel();

			_penumbra.Lights.Clear();
			_penumbra.Hulls.Clear();

			// Spielstand-Daten laden
			if (GameSaveService.TryLoadGameState(out int playerLevel, out Vector2 playerPosition, out Weapon playerWeapon))
			{
				_currentLevelIndex = playerLevel; // Level-Index aktualisieren
				LoadLevel(_levelNames[_currentLevelIndex]); // Lädt das Level

				// Aktualisiert nur die gespeicherten Werte des Hero
				Hero.Instance.Position = playerPosition;
				Hero.Instance.CurrentWeapon = playerWeapon ?? new MachineGun(new Vector2(0, 0));
				Hero.Instance.IsExit = false;

				Debug.WriteLine("Spielstand erfolgreich geladen!");
			}
			else
			{
				Debug.WriteLine("Laden des Spielstands fehlgeschlagen.");
			}
		}

		//wird noch nicht gebraucht
		public void RestartGame()
		{
			GameResetService.RestartGame(this, "firstlevel");
		}

		//Startet das Game komplett neue
		public void RestartGameComplete()
		{
			GameResetService.RestartGameComplete();
		}

		public int GetCurrentLevelIndex()
		{
			return _currentLevelIndex;
		}

		//wird benutzt um vom Start zu starten, level 1
		public void ResetLevelIndex()
		{
			_currentLevelIndex = 0;
			Debug.WriteLine("Level-Index wurde zurückgesetzt.");
		}
		#endregion

		#region GameOver Scenario
		public static bool IsGameOver
		{
			get => _isGameOver;
			set
			{
				_isGameOver = value;
				Debug.WriteLine($"Game Over Status geändert: {_isGameOver}");
			}
		}

		public static void TriggerGameOver()
		{
			IsGameOver = true;
		}

		public static void ResetGameOver()
		{
			IsGameOver = false;
		}

		#endregion

		
		#region GameCompleted Scenario
		public static bool IsGameCompleted
		{
			get => _isGameCompleted;
			set
			{
				_isGameCompleted = value;
				Debug.WriteLine($"Game Over Status geändert: {_isGameOver}");
			}
		}

		public static void TriggerGameCompleted()
		{
			IsGameCompleted = true;
		}

		public static void ResetGameCompleted()
		{
			IsGameCompleted = false;
		}

		private void TriggerGameCompletion()
		{
			Debug.WriteLine("Spiel abgeschlossen! Alle Level wurden erfolgreich gemeistert.");
	
			IsGameCompleted = true;
			// Timer für den Wechsel zum Hauptmenü

		}


		#endregion

		#region Menü Methoden
		public void PauseGame(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
		{
			game.ChangeState(new PauseMenu(game, graphicsDevice, content));
		}
		#endregion

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			_penumbra.SpriteBatchTransformEnabled = true;
			_penumbra.Transform = CalculateTranslation();

			spriteBatch.Begin(transformMatrix: CalculateTranslation());

			// Zeichnen der Tilemap
			_levelManager.Draw(spriteBatch, Level);
			_entityMgr.Draw(spriteBatch);

			spriteBatch.End();
		}

#if DEBUG
		public override void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch)
		{
			_levelManager.DrawDebug(spriteBatch, Level);
			_entityMgr.DrawDebug(spriteBatch);
		}
#endif
	}
}

