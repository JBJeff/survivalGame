
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Penumbra;
using Silent_Shadow.Managers;
using Silent_Shadow.States;
using Silent_Shadow.Managers.CollisionManager;
using Silent_Shadow.Models.Animations;
using Silent_Shadow.Models.Weapons;
using Silent_Shadow.Models.Enviroment;
using Silent_Shadow.Managers.EntityManager;
using System.Linq;
using System.Collections.Generic;

namespace Silent_Shadow.Models
{
	public class Hero : Entity
	{
		public Vector2 Velocity { get; set; }
		public float Visibility { get; set; }
		
		public Weapon CurrentWeapon { get; set; }
		public Weapon PrimaryWeapon { get; set; }
		public Weapon SecondaryWeapon { get; set; } = new Knife();
		private object lastWeapon;

		#region Singleton
		private static Hero _instance;

		private readonly Texture2D _walkTexture;
		private Animation _anim;
		private Animation _animWalk;

		public Hull Hull { get; set; }

		//für die Kollision
		public override Rectangle Bounds
		{
			get
			{
				// Verschieben des Rechtecks für genauere Kollision
				int offsetX = -15;
				int offsetY = -5;

				return new Rectangle(
					(int) Position.X + offsetX,  // verschiebung auf der X-Achse
					(int) Position.Y + offsetY,  // verschiebing auf der Y-Achse
					32,  // Breite des Rechtecks
					32   // Höhe des Rechtecks
				);
			}
		}
		private Rectangle heroRect; //für die Kollision
		
		//Bestimmt wann der Spieler das Level verlassen kann
		public bool IsExit { get; set; } = false;

		//für die Rüstungsanzeige
		public int ArmorCount { get;  set; }
		public int MaxHealth { get;  set; }
		public int CurrentHealth { get; private set; }
		
		//Verhindert das direkte setzen von Rüstungen nacheinander
		private float armorPickupCooldown = 0f; 
		public bool CanPickupArmor => armorPickupCooldown <= 0f;

		private float _soundAreaSize = 0f;
		public CircleF SoundArea
		{
			get
			{
				return new CircleF(
					new Vector2((int)Position.X, (int)Position.Y),
					_soundAreaSize);
			}
		}

		public static Hero Instance
		{
			get
			{
				if (_instance == null)
				{
					throw new InvalidOperationException("Hero is not initialized. Call Initialize(Vector2 position) first.");
				}
				return _instance;
			}
		}

		private Hero(Vector2 _position)
		{
			_walkTexture = Globals.Content.Load<Texture2D>("Player/FeetWalk");

			Position = _position;
			Rotation = 0f;
			Size = 0.20f;
			Speed = 180f;
			heroRect = Bounds;
			
			Visibility = 1f;

			//Startwaffe Maschinengewehr
			CurrentWeapon = new MachineGun(new Vector2(0, 0));
			
			//Wird benötigt zum neustarten des Spieles
			_animWalk = new Animation(_walkTexture, 20, 1, 0.03f);
			Sprite = Globals.Content.Load<Texture2D>("Player/HeroRifle&ShotgunMove");
			_anim = new(Sprite, 20, 2, 0.05f, 2);

			/*
			Vector2[] hullVertices =
			{
				new(heroRect.Left, heroRect.Top),
				new(heroRect.Right, heroRect.Top),
				new(heroRect.Right, heroRect.Bottom),
				new(heroRect.Left, heroRect.Bottom)
			};

			Hull = new(hullVertices)
			{
				Position = new Vector2(heroRect.Left + heroRect.Width / 2, heroRect.Top + heroRect.Height / 2),
				Scale = Vector2.One,
				Rotation = 0f,
			};
			*/

			Globals.Game.PlayerAlive = true;
		}

		public static void Initialize(Vector2 position)
		{
			if (_instance != null)
			{
				throw new InvalidOperationException("Hero is already initialized.");
			}
			_instance = new Hero(position);
		}
		#endregion

		public static void Reset()
		{
			_instance = null; // Setzt die Instanz zurück, für das neuladen des spiels
		}

		//zum verhindern der Doppelte Armor aufnahme, Zeit zum löschen der Tile
		public void StartArmorCooldown()
		{
			armorPickupCooldown = 3f; // Setzt die Cooldown-Zeit, 
		}

		public void TakeDamage()
		{
			Debug.WriteLine($"Hero nimmt Schaden: ");

			if (ArmorCount > 0)
			{
				// Verbraucht ein Schild, wenn Schaden eintrifft
				ArmorCount--;
				Debug.WriteLine($"Hero nimmt Schaden, ein Schild wurde verbraucht. Verbleibende Schilde: {ArmorCount}");
			}
			else
			{
				Debug.WriteLine("Hero hat keine Schilde mehr und nimmt Schaden. Hero ist tot.");
				Die(); // Der Held stirbt direkt
			}

			Debug.WriteLine($"Hero verbleibende Schilde: {ArmorCount}");
		}

		public void AddArmor()
		{
			if (ArmorCount < 3) // Maximal 3 Rüstungen
			{
				ArmorCount++;
				MaxHealth += 1;
				CurrentHealth = MaxHealth; // Setzt die Gesundheit auf den maximalen Wert zurück
				Debug.WriteLine($"Rüstung aufgenommen! Aktuelle Rüstung: {ArmorCount}, Maximaler Leben: {MaxHealth}");
			}
		}

		public void removeWeapon()
		{
			PrimaryWeapon = null;
			CurrentWeapon = SecondaryWeapon;
		}

		public void FireWeapon()
		{
			// Waffe feuern
			CurrentWeapon?.Fire(this);
		}

		// Waffenwchseln 1=Nahkampf 2=Schusswaffe 3=Pistol 4=Shotgun 5=MachineGun
		private void SwitchWeapon()
		{
			if (InputManager.IsKeyPressed(Keys.D2) && PrimaryWeapon != null)
			{
				CurrentWeapon = PrimaryWeapon;
			}
			else if (InputManager.IsKeyPressed(Keys.D3))
			{
				PrimaryWeapon = new Pistol(new Vector2(0, 0));
			}
			else if (InputManager.IsKeyPressed(Keys.D4))
			{
				PrimaryWeapon = new Shotgun(new Vector2(0, 0));
			}
			else if (InputManager.IsKeyPressed(Keys.D5))
			{
				PrimaryWeapon = new MachineGun(new Vector2(0, 0));
			}
			else if (InputManager.IsKeyPressed(Keys.D1))
			{
				CurrentWeapon = SecondaryWeapon;
			}
			else if (InputManager.IsKeyPressed(Keys.D7))
			{
				Console.WriteLine($"Position des Hero : {Position}");
			}
		}

		internal object GetCurrentWeapon()
		{
			return CurrentWeapon;
		}

		private void ChangeSprite()
		{
			if (lastWeapon != CurrentWeapon)
			{
				if (GetCurrentWeapon() is Knife)
				{
					Sprite = Globals.Content.Load<Texture2D>("Player/KnifeMove");
					_anim = new(Sprite, 20, 1, 0.05f);
				}
				else if (GetCurrentWeapon() is MachineGun)
				{
					Sprite = Globals.Content.Load<Texture2D>("Player/HeroRifle&ShotgunMove");
					_anim = new(Sprite, 20, 2, 0.05f, 2);
				}
				else if (GetCurrentWeapon() is Shotgun)
				{
					Sprite = Globals.Content.Load<Texture2D>("Player/HeroRifle&ShotgunMove");
					_anim = new(Sprite, 20, 2, 0.05f);
				}
				else if (GetCurrentWeapon() is Pistol)
				{
					Sprite = Globals.Content.Load<Texture2D>("Player/HandgunMove");
					_anim = new(Sprite, 20, 1, 0.05f);
				}
			}
			lastWeapon = CurrentWeapon;
		}

		private void HandleMovement()
		{
			Velocity = Speed * InputManager.GetMovementDirection() * Globals.DeltaTime;
			Vector2 previousPosition = Position;
			// Prüft ob gegangen wird oder nicht und passt die größe des Soundkreises und den Sprite an
			if (!InputManager.ActiveKey())
			{
				_animWalk = new(_walkTexture, 20, 1, 0.03f);
				_soundAreaSize = 0f;
			}
			else if (InputManager.IsSneaking())
			{
				//Fürs schleichen
				_soundAreaSize = 0f;
				Velocity /= 2;
			}
			else
			{
				_soundAreaSize = 150f;
			}
			Position += Velocity;
			//6Hull.Position = Position;

			// Prüfe auf Kollision auf wände
			// HACK: For some reason is better if the collision offsets don't match the hitboxoffsets
			if (CollisionManager.IsCollidingWithAnyWall(Position, GameState.Instance.Level.Colliders, -20, -10, 40, 40))
			{
				Position = previousPosition; // Wenn eine Kollision erkannt wird, zurück zur alten Position
			}

			// Prüft auf Kollision mit einem CheckPoint, soll Speichern auslösen
			if (CollisionManager.IsCollidingWithCheckpoint(Position, GameState.Instance.Level.CheckPoint, -15, 0, 40, 40))
			{
				for (int i = 0; i < GameState.Instance.Level.CheckPoint.Count; i++)
				{
					// Überprüftdie Kollision 
					if (Instance.Bounds.Intersects(GameState.Instance.Level.CheckPoint[i]))
					{
						// Aktion beim ersten Betreten des Checkpoints
						Debug.WriteLine("Checkpoint erreicht, Spiel wird gespeichert!");
						GameState.Instance.SaveGameState();

						// Checkpoint aus der Liste entfernen, um ihn nur einmal nutzbar zu machen
						GameState.Instance.Level.CheckPoint.RemoveAt(i);

						// Schleife verlassen, da der Checkpoint entfernt wurde
						break;
					}
				}
			}

			// Setzt die Kollisionsrechteck-Position basierend auf der neuen Position des Helden
			heroRect.X = (int)Position.X;
			heroRect.Y = (int)Position.Y;
		}

		public override void Update()
		{
			// Aktualisiere den Cooldown in jeder Frame
			if (armorPickupCooldown > 0f)
			{
				armorPickupCooldown -= Globals.DeltaTime; // Reduziert die Cooldown-Zeit
			}

			HandleMovement();
			Visibility = CalculateVisibility();

			Vector2 aim = InputManager.GetAimDirection();
			if (aim.LengthSquared() > 0)
			{
				Rotation = aim.ToAngle();
			}

			// Schießen
			if (InputManager.IsLeftMouseHeld())
			{
				FireWeapon();
				if (!CurrentWeapon.ToString().Equals("Silent_Shadow.Models.Knife"))
				{
					_soundAreaSize = 200f;
				}
			}

			//Überprüfen, ob die Waffe bereit ist (Nachladen)
			CurrentWeapon.Update();
			SwitchWeapon(); //Waffenwechsel
			if (CurrentWeapon.IsExpired)
			{
				removeWeapon();
			}

			//Wechseln des Sprites
			ChangeSprite();
			_anim.Update();
			_animWalk.Update();
		}

		private float CalculateVisibility()
		{
			const float DARK_VISIBILITY = 0.1f;
			float visibility = 0f;
			
			IEntityManager entityMgr = EntityManagerFactory.GetInstance();
			List<LightSource> lights = entityMgr.GetEntities().OfType<LightSource>().ToList();

			LightSource nearestLight = null;
			float nearestDistance = float.MaxValue;

			foreach (LightSource light in lights)
			{
				float distance = Vector2.Distance(Position, light.Position);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					nearestLight = light;
				}	
			}

			if (nearestLight != null)
			{
				float range = nearestLight.Radius;
				if (nearestDistance <= range)
				{
					visibility = nearestLight.Glowing ? 1f - (nearestDistance / range) : DARK_VISIBILITY;
				}
			}

			return visibility;
		}

		public override void Die()
		{
			Globals.Game.PlayerAlive = false;
			SoundManager.PlayBackgroundMusic(99);
			base.Die();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			_animWalk.Draw(Position, InputManager.GetRotation(), Size, spriteBatch);
			_anim.Draw(Position, Rotation, Size, spriteBatch);
		}

		#if DEBUG
		public override void DrawDebug(SpriteBatch spriteBatch)
		{
			// Zeichnet die Kollisionsbox des Helden in Grün
			spriteBatch.DrawRectangle(Bounds, new Color(0, 255, 0, 100));
			spriteBatch.DrawCircle(SoundArea, 35, Color.Red);

			#region AI State
			string DebugString = $"VIS: {Visibility}";
			spriteBatch.DrawString(Globals.DebugFont, DebugString, Position - new Vector2(20f, -20f), Color.White, 0, Vector2.Zero, 0.25f, 0, 0);
			#endregion
		}
		#endif
	}
}
