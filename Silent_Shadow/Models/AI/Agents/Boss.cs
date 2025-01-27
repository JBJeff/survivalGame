using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Silent_Shadow.Models.Weapons;
using Silent_Shadow.Managers.EntityManager;
using Silent_Shadow.Models.AI.Actions;
using Silent_Shadow.Models.AI.Goals;
using System.Linq;

namespace Silent_Shadow.Models.AI.Agents
{
	// Die Boss-Klasse repräsentiert einen besonderen Gegner mit anderer Logik als die Grunts
	public class Boss : Agent
	{
		#region Attributs
		// Maximale und aktuelle Gesundheit des Bosses
		public int MaxHealth { get; private set; }
		public int CurrentHealth { get; private set; }

		// Attribute für die Kampfmechanik
		private int _bulletsFired; // Anzahl abgefeuerter Kugeln seit dem letzten Nachladen
		private float _reloadTimer; // Timer für das Nachladen
		private const int MaxBullets = 1500; // Maximale Anzahl Kugeln vor dem Nachladen
		private const float ReloadTime = 4f; // Zeit in Sekunden für das Nachladen, Zeit in dem Der Spieler attackieren kann

		// Attribute für die Orbit-Bewegung
		private float _orbitAngle; // Winkel für die Orbit-Berechnung
		private const float OrbitRadius = 200f; // Radius des Orbits
		private const float OrbitSpeed = 1f; // Geschwindigkeit des Orbits

		// Verzögerung der Schüsse
		private float _fireCooldownTimer = 0f; 
		private const float _fireCooldown = 0.025f; 

		// Zufallszahlengenerator für Streuung beim Schießen
		private readonly Random _random = new Random();

		
		private MachineGun _weapon;
		private IEntityManager _entityManager;

		// Sichtweite des Bosses
		private const float SightRange = 300f; 

		// Aktivierungsradius vom Boss
		private const float ActivationRange = 200f; // Radius in dem der Boss reagiert
		private bool _isActive = false; // Ob der Boss aktiv ist und Attackieren kann
		#endregion

		#region Bounds 
		public override Rectangle Bounds
		{
			get
			{
				// Setze die Kollisionsgrenzen des Bosses
				int offsetX = 40; // Verschiebung der Hitbox nach links/rechts
				int offsetY = 20; // Verschiebung der Hitbox nach oben/unten
				int width = 30;   // Breite der Hitbox
				int height = 20;  // Höhe der Hitbox

				return new Rectangle(
					(int)Position.X + offsetX,
					(int)Position.Y + offsetY,
					width,
					height
				);
			}
		}
		#endregion

		public Boss(Vector2 position, float rotation, float speed, float stoppingDistance, List<Goal> goals, List<GAction> actions, int maxHealth = 100)
			: base("BOSS", rotation, speed, 15f, stoppingDistance, goals, actions)
		{
			Position = position;
			Rotation = rotation;
			Speed = speed;
			StoppingDistance = stoppingDistance;

			MaxHealth = maxHealth;
			CurrentHealth = maxHealth;

			_bulletsFired = 0;
			_reloadTimer = 0f;

			// Größe und Sprite-Einstellungen für die Darstellung, Sprites müssten noch hinzugefügt werden
			Size = 1.5f;
			Sprite = Globals.Content.Load<Texture2D>("Boss/boss");
			SpriteOffset = Vector2.Zero;

			
			_weapon = new MachineGun(Vector2.Zero);
			_entityManager = EntityManagerFactory.GetInstance();
		}

		// Methode, um dem Boss Schaden zuzufügen
		public void TakeDamage(int damage)
		{
			Debug.WriteLine($"Boss nimmt Schaden: {damage}");
			CurrentHealth -= damage;
			Debug.WriteLine($"Boss verbleibende Gesundheit: {CurrentHealth}");

			if (CurrentHealth <= 0)
			{
				Debug.WriteLine("Boss ist tot.");
				Die();
			}
		}

		public override void Update()
		{
			Vector2 playerPosition = Hero.Instance.Position; 
			float deltaTime = Globals.DeltaTime; // Zeit seit dem letzten Frame

			// Richtung und Entfernung zum Spieler
			Vector2 directionToPlayer = playerPosition - Position; 
			float distanceToPlayer = directionToPlayer.Length();

			// Prüft Kollisionen zwischen Boss- und Hero-Kugeln
			CheckForBulletCollision();

			// Boss wird nur aktiv, wenn der Spieler nah genug am Gegner ist
			if (!_isActive && distanceToPlayer <= ActivationRange)
			{
				_isActive = true;
				Debug.WriteLine("Boss wird aktiv, Spieler ist nah genug.");
			}

			// Wenn der Boss nicht aktiv ist tut er nichts, außer rumlaufen
			if (!_isActive)
			{
				Debug.WriteLine("Boss bleibt inaktiv, Spieler ist zu weit entfernt.");
				return;
			}

			// Der Boss attackiert den Spieler sobald er in Sichtweite ist
			if (CanSeePlayer(playerPosition, distanceToPlayer))
			{
				Debug.WriteLine("Boss sieht den Spieler und bleibt stehen.");
				HandleCombat(playerPosition, deltaTime);
				return;
			}

			// Bewegung je nach Entfernung zum Spieler
			if (distanceToPlayer > StoppingDistance + OrbitRadius)
			{
				MoveTowards(playerPosition, deltaTime);
			}
			else if (distanceToPlayer < StoppingDistance)
			{
				Vector2 retreatPosition = Position - directionToPlayer; // Zurückweichen
				MoveTowards(retreatPosition, deltaTime);
			}
			else
			{
				OrbitAroundPlayer(deltaTime, playerPosition); // Orbitbewegung, umkreisen 
			}

			// Kampfmechanik, wenn der Spieler im Orbit-Radius ist
			if (distanceToPlayer <= OrbitRadius)
			{
				HandleCombat(playerPosition, deltaTime);
			}
		}

		

		// Methode zur Überprüfung, ob der Boss den Spieler sehen kann
		private bool CanSeePlayer(Vector2 playerPosition, float distanceToPlayer)
		{
			// Spieler muss innerhalb der Sichtweite sein
			if (distanceToPlayer > SightRange)
			{
				return false;
			}

			// Sicht-Check, Entfernung und Blickrichtung
			Vector2 directionToPlayer = playerPosition - Position;
			float angleToPlayer = (float)Math.Atan2(directionToPlayer.Y, directionToPlayer.X);
			float angleDifference = Math.Abs(angleToPlayer - Rotation);

			// Normalisierung des Winkels
			angleDifference = Math.Min(angleDifference, Math.Abs(angleDifference - MathHelper.TwoPi));

			// Spieler ist sichtbar, wenn der Winkel klein genug ist ( 45 Grad Sichtfeld momentan)
			return angleDifference < MathHelper.ToRadians(45);
		}

		// Bewegung in Richtung des Ziels
		private void MoveTowards(Vector2 targetPosition, float deltaTime)
		{
			Vector2 direction = targetPosition - Position;
			direction.Normalize();
			Position += direction * Speed * deltaTime;

			// Aktualisierung der Rotation basierend auf der Bewegungsrichtung
			Rotation = (float)Math.Atan2(direction.Y, direction.X);
		}

		// Orbitalbewegung um den Spieler, ist dafür da um Hindernisse zu umgehen. Später können Tische etc. eingebaut werden um in Deckung zu gehen
		private void OrbitAroundPlayer(float deltaTime, Vector2 playerPosition)
		{
			_orbitAngle += OrbitSpeed * deltaTime;
			Vector2 orbitOffset = new Vector2(
				(float)Math.Cos(_orbitAngle) * OrbitRadius,
				(float)Math.Sin(_orbitAngle) * OrbitRadius
			);

			Vector2 targetPosition = playerPosition + orbitOffset;
			MoveTowards(targetPosition, deltaTime);
		}

		// Kampfverhalten, einschließlich Schießen und Nachladen
		private void HandleCombat(Vector2 playerPosition, float deltaTime)
		{
			_fireCooldownTimer -= deltaTime; // Schussverzögerung aktualisieren

			if (_reloadTimer > 0)
			{
				_reloadTimer -= deltaTime; // Nachladen abwarten
			}
			else if (_fireCooldownTimer <= 0f)
			{
				if (_bulletsFired < MaxBullets)
				{
					FireWithSpread(playerPosition); // Schießen mit Streuung
					_bulletsFired++;
					_fireCooldownTimer = _fireCooldown; // Verzögerung zurücksetzen
				}
				else
				{
					_reloadTimer = ReloadTime; // Nachladen starten
					_bulletsFired = 0; // Kugelzähler zurücksetzen
				}
			}
		}

		// Schießen mit Streuung
		private void FireWithSpread(Vector2 playerPosition)
		{
			Vector2 spread = new Vector2(
				(float)(_random.NextDouble() - 0.5) * 60f, //60f Streuung
				(float)(_random.NextDouble() - 0.5) * 60f
			);

			Vector2 direction = (playerPosition + spread) - Position;
			direction.Normalize();

			Bullet bullet = new Bullet(this, direction, 1);
			bullet.SetBulletSpeed(180f);
			bullet.SetLifespan(10000);
			bullet.SetSpawnPointWithOffset(50f, 30f);

			_entityManager.Add(bullet);

			Debug.WriteLine($"Boss feuert auf Position: {playerPosition + spread}");
		}

		//könnte ausgelagert werden... 
		private void CheckForBulletCollision()
		{
			var bullets = EntityManager.Instance.Entities.OfType<Bullet>().ToList();

			foreach (var bullet1 in bullets)
			{
				foreach (var bullet2 in bullets)
				{
					if (bullet1 == bullet2)
						continue; // Vermeidet Vergleich derselben Kugel

					if (bullet1.Bounds.Intersects(bullet2.Bounds))
					{
						if ((bullet1.Shooter is Hero && bullet2.Shooter is Boss) ||
							(bullet1.Shooter is Boss && bullet2.Shooter is Hero))
						{
							EntityManager.Instance.KillEntity(bullet1);
							EntityManager.Instance.KillEntity(bullet2);
							Debug.WriteLine("Kollision zwischen Hero-Bullet und Boss-Bullet erkannt. Beide werden entfernt.");
						}
					}
				}
			}
		}

		// Methode, die aufgerufen wird, wenn der Boss stirbt
		public override void Die()
		{
			Debug.WriteLine("Boss ist gestorben.");
			IsExpired = true; // Tot
		}

		// Boss zeichnen
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Sprite != null)
			{
				spriteBatch.Draw(Sprite, Position, null, Color.White, 0, SpriteOffset, Size, SpriteEffects.None, 0);
			}
		}
	}
}
