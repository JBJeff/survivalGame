
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Penumbra;
using Silent_Shadow.Managers.DialogueManager;
using Silent_Shadow.Models.AI.Actions;
using Silent_Shadow.Models.AI.Goals;
using Silent_Shadow.Managers;
using Silent_Shadow.Models.AI.States;
using Silent_Shadow.States;
using Silent_Shadow.Models.Weapons;
using Silent_Shadow.Models.Animations;
using Silent_Shadow.Managers.EntityManager;

namespace Silent_Shadow.Models.AI.Agents
{

	/// <summary>
	/// Footsoldier type enemy
	/// </summary>
	///
	/// <authors>
	/// <author>Jonas Schwind</author>
	/// </authors>
	///
	/// <version>2.3</version>
	///
	/// <seealso cref="Agent"/>
	public class Grunt : Agent
	{
		#region Assets

		private readonly Texture2D _eyecon;
		private readonly Texture2D _alertIcon;
		private readonly SoundEffect _detectionSound;
		private readonly SoundEffectInstance _detectionSoundInstance;
		private SpriteFont _barkFont;
		private Animation _anim;

		#endregion

		#region Penumbra

		private readonly PenumbraComponent penumbra;
		private Spotlight _flashlight;

		#endregion

		#region Detection

		private Vector2[] _peripheralArea;
		private Vector2[] _visibleArea;
		private bool _hasPlayedDetectionSound = false;

		#endregion

		#region Barks

		private readonly IDialogueManager dialogueManager;
		private float _barkCounter = 0f;
		private bool _hasShownBark = false;

		#endregion

		public override Rectangle Bounds
		{
			get
			{
				int offsetX = -20;
				int offsetY = -15;
				return new Rectangle(
					(int)Position.X + offsetX,
					(int)Position.Y + offsetY,
					32,
					32
				);
			}
		}

		public Grunt(Vector2 position, string name, float rotation, float speed, float turnSpeed, float stoppingDistance, List<Goal> goals, List<GAction> actions) : base(name, rotation, speed, turnSpeed, stoppingDistance, goals, actions)
		{
			#region  Assets

			Sprite = Globals.Content.Load<Texture2D>("Sprites/GruntPistolMove");
			_anim = new(Sprite, 20, 1, 0.05f, 1);
			SpriteOffset = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
			Size = 0.20f;
			_barkFont = Globals.Content.Load<SpriteFont>("Tahoma");
			_eyecon = Globals.Content.Load<Texture2D>("LooseSprites/eye-icon");
			_alertIcon = Globals.Content.Load<Texture2D>("LooseSprites/exclamation-point");
			_detectionSound = Globals.Content.Load<SoundEffect>("Sounds/Effects/Alert");
			_hasPlayedDetectionSound = false;
			_detectionSoundInstance = _detectionSound.CreateInstance();
		
			#endregion

			dialogueManager = DialogueManagerFactory.GetInstance();
			
			// HACK: force-reset all wordstates to esure a fresh start
			WorldState.States.Clear();

			Position = position;
			Path = null;
			CurrentWaypointIndex = 0;

			CurrentWeapon = new MachineGun(Vector2.Zero);
			WorldState.SetState("WeaponLoaded", 0);

			penumbra = Globals.Game.Penumbra;
			_flashlight = new Spotlight
			{
				Position = new Vector2(400, 300),
				Scale = new Vector2(500, 500),
				Radius = MathHelper.ToRadians(0.7f),
				Color = Color.White,
				Intensity = 2f,
				ConeDecay = 1.7f,
				CastsShadows = true,
				ShadowType = ShadowType.Solid
			};
			penumbra.Lights.Add(_flashlight);
		}

		// PERF: Probalby hella expensive
		public override bool PlayerDetected(float deltaTime)
		{
			Hero player = Hero.Instance;

			//TODO: Sollte abgewandelt werden. Nur zum Übergang Implementiert
			if (HearsPlayer())
			{
				Rotation = (float)Math.Atan2(player.Position.Y - Position.Y, player.Position.X - Position.X);
			}
			
			Vector2 direction = MathHelpers.GetDirectionVector(Rotation, Direction.Left);

			VisionCone = MathHelpers.GetTriangle(Position, direction, 122f, 55f);
			_visibleArea = MathHelpers.GetHexagon(Position, direction, 129f, 158f, 65f, 122f, 65f, 0f);
			_peripheralArea = MathHelpers.GetHexagon(Position, direction, 201f, 129f, 122f, 158f, 122f, -10f);

			GameState gameState = (GameState) Game1.Instance.CurrentState;
			List<Rectangle> colliders = gameState.Level.Colliders;
			Vector2 playerPosition = Hero.Instance.Position;

			static bool PlayerInDetectionShape(Vector2[] shape, Vector2 playerPos)
			{
				return MathHelpers.PointInPolygon(shape[0], shape[1], shape[2], shape[3], playerPos) ||
					   MathHelpers.PointInPolygon(shape[2], shape[3], shape[4], shape[5], playerPos);
			}

			if (PlayerInVisionCone(Position, VisionCone[0], VisionCone[1], playerPosition))
			{
				if (!CheckForVisualObstacle(Position, playerPosition, colliders))
				{
					// NOTE: Purposefully ignores visibilty, as this is iluminated by flashlight
					_detectionCounter += 10f * deltaTime;
				}
			}
			else if (PlayerInDetectionShape(_visibleArea, playerPosition))
			{
				if (!CheckForVisualObstacle(Position, playerPosition, colliders))
				{
					_detectionCounter += 10f * player.Visibility * deltaTime;
				}
			}
			else if (PlayerInDetectionShape(_peripheralArea, playerPosition))
			{
				if (!CheckForVisualObstacle(Position, playerPosition, colliders))
				{
					_detectionCounter += 4.5f * player.Visibility * deltaTime;
				}
			}
			else
			{
				_detectionCounter -= 3.5f * deltaTime;
				_detectionCounter = Math.Max(0, _detectionCounter);
			}

			return _detectionCounter >= _detectionThreshold;
		}

		private bool HearsPlayer()
		{
			return MathHelpers.PointInCircle(Hero.Instance.Position.X, Hero.Instance.Position.Y, Position.X, Position.Y, Hero.Instance.SoundArea.Radius);
		}

		private void PlayDetectionSound()
		{
			if (!_hasPlayedDetectionSound)
			{
				_detectionSoundInstance.Play();
				_hasPlayedDetectionSound = true;
			}
		}

		private void CameraDetectPlayer() {

			// Radius, in dem Grunts benachrichtigt werden
			float notificationRadius = 1000f;

			// Hole alle CCTV im Spiel 
			var entities = EntityManager.Instance.Entities;

			foreach (var entitie in entities)
			{
				if (entitie is CctvCam cctv){
					if (cctv.seePlayer) {

						// Berechne die Entfernung zwischen Kamera und Grunt
						if (Vector2.Distance(Position, cctv.Position) <= notificationRadius)
						{
							foreach (var goal in Goals)
							{ 
								if (goal is ElimintateThreat) {
									if (!WorldState.HasState("FoundPlayer"))
									{
									WorldState.States.Add("FoundPlayer", 1);
									}
								}
							}
						}
					}
				}
			}
		}

		public override void Update()
		{
			float deltaTime = Globals.DeltaTime;
			_anim.Update();

			_flashlight.Position = Position;
			_flashlight.Rotation = Rotation;
			
			CameraDetectPlayer();

			switch (AlertState)
			{
				case AlertState.IDLE:
				case AlertState.COUTIOUS:
					if (PlayerDetected(deltaTime))
					{
						if (!WorldState.HasState("playerDetected"))
						{
							WorldState.States.Add("playerDetected", 1);
						}
						AlertState = AlertState.COMBAT;
						PlayDetectionSound();
						SoundManager.PlayCombatTrack(1);
					}
					break;

				case AlertState.ALERT:
					if (PlayerDetected(deltaTime))
					{
						if (!WorldState.HasState("playerDetected"))
						{
							WorldState.States.Add("playerDetected", 1);
						}
						AlertState = AlertState.COMBAT;
						PlayDetectionSound();
						SoundManager.PlayCombatTrack(1);
					}
					_dementiaCounter += 1.5f * deltaTime;
					if (_dementiaCounter >= _detectionThreshold)
					{
						Console.WriteLine("Player lost completly");
						WorldState.States.Remove("playerLost");
						_detectionCounter = 0f;
						_dementiaCounter = 0f;

						AlertState = AlertState.COUTIOUS;
					}
					break;

				case AlertState.COMBAT:
					if (PlayerDetected(deltaTime))
					{
						WorldState.LastKnownPlayerPosition = Hero.Instance.Position;
					}
					else
					{
						_dementiaCounter += 1.5f * deltaTime;
					}

					if (WorldState.HasState("playerDetected") && _dementiaCounter >= _detectionThreshold)
					{
						Console.WriteLine("Player lost");
						WorldState.States.Remove("playerDetected");
						if (!WorldState.HasState("playerLost"))
						{
							WorldState.States.Add("playerLost", 1);
						}
						_detectionCounter = 0f;
						_dementiaCounter = 0f;
						AlertState = AlertState.ALERT;
						SoundManager.PlayCombatTrack(0);
					}
					break;

				default:
					break;
			}
		}

		public override void Die()
		{
			if (AlertState != AlertState.IDLE)
			{
			SoundManager.PlayBackgroundMusic(GameState.Instance._currentLevelIndex);
			}
			penumbra.Lights.Remove(_flashlight);
			base.Die();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			//spriteBatch.Draw(Sprite, Position, null, Tint, Rotation, SpriteOffset, Size, 0, 0);
			
			_anim.Draw(Position, Rotation, Size, spriteBatch);

			if (_detectionCounter >= _detectionThreshold)
			{
				spriteBatch.Draw(_alertIcon, Position, null, Tint, 0, SpriteOffset + new Vector2(600, 1500), .005f, 0, 0);
			}
			else if (_detectionCounter > 0)
			{
				spriteBatch.Draw(_eyecon, Position, null, Tint, 0, SpriteOffset + new Vector2(400, 800), .1f, 0, 0);
			}
		}

#if DEBUG
		public override void DrawDebug(SpriteBatch spriteBatch)
		{
			#region Kollision
			// Zeichnet die Kollisionsbox des Entity in Grün
			spriteBatch.DrawRectangle(Bounds, new Color(0, 255, 0, 100));
			#endregion

			#region Vision Cone debug
			spriteBatch.DrawLine(Position, VisionCone[0], Color.Orange);
			spriteBatch.DrawLine(Position, VisionCone[1], Color.Orange);
			spriteBatch.DrawLine(VisionCone[0], VisionCone[1], Color.Orange);
			#endregion

			#region Peripheral Vision Debug
			spriteBatch.DrawLine(_peripheralArea[0], _peripheralArea[1], Color.Yellow);
			spriteBatch.DrawLine(_peripheralArea[4], _peripheralArea[5], Color.Yellow);
			spriteBatch.DrawLine(_peripheralArea[0], _peripheralArea[2], Color.Yellow);
			spriteBatch.DrawLine(_peripheralArea[1], _peripheralArea[3], Color.Yellow);
			spriteBatch.DrawLine(_peripheralArea[2], _peripheralArea[4], Color.Yellow);
			spriteBatch.DrawLine(_peripheralArea[3], _peripheralArea[5], Color.Yellow);
			#endregion

			#region Visible Area debug
			spriteBatch.DrawLine(_visibleArea[0], _visibleArea[1], Color.Red);
			spriteBatch.DrawLine(_visibleArea[4], _visibleArea[5], Color.Red);
			spriteBatch.DrawLine(_visibleArea[0], _visibleArea[2], Color.Red);
			spriteBatch.DrawLine(_visibleArea[1], _visibleArea[3], Color.Red);
			spriteBatch.DrawLine(_visibleArea[2], _visibleArea[4], Color.Red);
			spriteBatch.DrawLine(_visibleArea[3], _visibleArea[5], Color.Red);
			#endregion

			#region Walk path debug
			if (Path != null && Path.Count > 1)
			{
				for (int i = 0; i < Path.Count - 1; i++)
				{
					Vector2 from = Path[i].Position;
					Vector2 to = Path[i + 1].Position;
					spriteBatch.DrawLine(from, to, Color.White, 3f);
				}
			}
			#endregion

			#region Visline
			if (_detectionCounter > 0)
			{
				spriteBatch.DrawLine(Position, Hero.Instance.Position, Color.Blue);
			}
			#endregion

			#region AI State
			if (WorldState != null && CurrentAction != null)
			{
				string DebugString = $"NAME: {Name}\nFSM: {AIState}\nSTATE: {DebugUtils.DictionaryToString(WorldState.States)}\nGOAL: {CurrentGoal}\nACTION: {CurrentAction.ActionName}\nAlert: {AlertState}\nDET: {_detectionCounter}\nDEM:{_dementiaCounter}";
				spriteBatch.DrawString(Globals.DebugFont, DebugString, Position - new Vector2(20f, -20f), Color.White, 0, Vector2.Zero, 0.25f, 0, 0);
			}
			#endregion
		}
#endif

	}
}