
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using Silent_Shadow.GUI;
using Silent_Shadow.Managers;
using Silent_Shadow.Models;
using Silent_Shadow.States;

namespace Silent_Shadow {

	public class Game1 : Game
	{
		public InfoDisplay _infoDisplay;
		public static Game1 Instance { get; private set; }
		public GraphicsDeviceManager Graphics { get; set; }

		private SpriteBatch _spriteBatch;

		public State CurrentState { get; private set; }
		private State _nextState;

		public PenumbraComponent Penumbra { get; set; }

		public bool _NightVision = false; 

		public bool PlayerAlive { get; set; } = true; // TODO: move to gamestate

		private readonly SpriteFont font;

		public Game1()
		{
			Instance = this;

			Graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = Globals.ScreenWidth,
				PreferredBackBufferHeight = Globals.ScreenHeight
			};
			Graphics.PreferredBackBufferWidth = Globals.ScreenWidth;
			Graphics.PreferredBackBufferHeight = Globals.ScreenHeight;

#if !DEBUG

			Graphics.IsFullScreen = true;
#endif
			Graphics.SynchronizeWithVerticalRetrace = false; // Disable VSync

			Graphics.ApplyChanges();

			Content.RootDirectory = "Content";
			Globals.Content = Content;

			Penumbra = new PenumbraComponent(this);
			Components.Add(Penumbra);

			// NOTE: Uncapping FPS leads to "interesting" sideeffects (comment if broken)a
			IsFixedTimeStep = false;

			IsMouseVisible = true;

			font = Globals.Content.Load<SpriteFont>("Tahoma");
#if DEBUG
			Globals.DebugFont = Content.Load<SpriteFont>("Tahoma");
#endif
		}

		protected override void Initialize()
		{
			//Lade Achievments
			AchievementManager.LoadAchievements();

			// Lade Sounds
			SoundManager.LoadContent();
			Penumbra.AmbientColor = Color.White;
			base.Initialize();
		}

		public void ChangeState(State state)
		{
			_nextState = state;
		}

		protected override void LoadContent()
		{
			Penumbra.Initialize();
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			CurrentState = new MenuState(this, Graphics.GraphicsDevice, Content);

			//	// Beispiel: Punktlicht
			// 	var pointLight = new PointLight
			// 	{
			// 		Position = new Vector2(200, 200),
			// 		Scale = new Vector2(300),
			// 		Color = Color.Red,
			// 		Intensity = 1f
			// 	};
			// 	Penumbra.Lights.Add(pointLight);
		}

		public void LoadUI()
		{
			Texture2D reloadIcon = Globals.Content.Load<Texture2D>("Sprites/Ammo"); //Nachlade Icon
			_infoDisplay = new InfoDisplay(font, reloadIcon, new Vector2((Globals.ScreenWidth / 4) + Hero.Instance.Position.X - 100, Hero.Instance.Position.Y - (Globals.ScreenHeight / 4) + 20));
		}

		protected override void Update(GameTime gameTime)
		{	
			// INFO: Only update if game is focused
			if (IsActive)
			{
				//  Umschalten der Hintergrundbeleuchtung durch Stromkasten schwarz new Color(45, 45, 45)
				if (_NightVision)
				{
					Penumbra.AmbientColor = Color.Green;
				}
				else
				{
					Penumbra.AmbientColor = new Color(45, 45, 45);
				}

				if (_nextState != null)
				{
					CurrentState = _nextState;
					_nextState = null;
				}
				CurrentState.Update(gameTime);

				CurrentState.PostUpdate(gameTime);

				base.Update(gameTime);
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			#region Lit stuff

			if (CurrentState.ToString().Equals("Silent_Shadow.States.GameState"))
			{
				Penumbra.BeginDraw();
				CurrentState.Draw(gameTime, _spriteBatch);
				Penumbra.Draw(gameTime);
				LoadUI();
				_spriteBatch.Begin(transformMatrix: GameState.Instance.CalculateTranslation());
				_infoDisplay.Draw(_spriteBatch);
			}
			else
			{
				CurrentState.Draw(gameTime, _spriteBatch);
				_spriteBatch.Begin();
			}

			#endregion

			#region Unlit shit
#if DEBUG
			CurrentState.DrawDebug(gameTime, _spriteBatch);
#endif
			_spriteBatch.End();
			
			#endregion
		}
	}
}
