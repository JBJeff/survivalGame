
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Silent_Shadow.Models;
using Silent_Shadow.Models.AI.Agents;
using Silent_Shadow.Models.Weapons;
using System;
using System.Threading.Tasks;
using Silent_Shadow.Models.Enviroment;
using System.IO;

namespace Silent_Shadow.Managers.EntityManager
{
	/// <summary>
	/// IEntityManager Implementation
	/// </summary>
	/// 
	/// <remarks>
	/// <para>This is a Singleton Object</para>
	/// </remarks>
	/// 
	/// <author>Jonas Schwind</author>
	/// <version>1.0</version>
	/// 
	/// <seealso cref="IEntityManager"/>
	public class EntityManager : IEntityManager
	{
		#region variables
		public bool IsUpdating { get; private set; }
		public List<Entity> Entities { get; private set; }
		public List<Entity> AddedEntities { get; private set; }
		public int Count { get { return Entities.Count; } }
		public int TotalGruntCount { get; private set; }
		public int GruntKilledCount { get; private set; }
		#endregion

		#region Singleton
		private static EntityManager _instance;
		public static EntityManager Instance
		{
			get
			{
				_instance ??= new EntityManager();
				return _instance;
			}
		}

		private EntityManager()
		{
			Entities = [];
			AddedEntities = [];
		}
		#endregion

		public void Add(Entity entity)
		{
			if (!IsUpdating)
			{
				Entities.Add(entity);
			}
			else
			{
				AddedEntities.Add(entity);
			}
		}

		public void CheckForWeaponPickup()
		{
			for (int i = 0; i < Entities.Count; i++)
			{
				if (Entities[i] is Weapon weapon && weapon.IsPickupable)
				{
					float distance = Vector2.Distance(Hero.Instance.Position, weapon.Position);

					// Wenn die Distanz weniger als 25 Einheiten beträgt
					if (distance < 25f)
					{
						// Zeige visuelles Feedback für das Aufnehmen der Waffe
						if (InputManager.IsKeyPressed(Keys.E)) // E-Taste drücken
						{
							Hero.Instance.PrimaryWeapon = weapon;  // Setze die primär Waffe des Helden
							Hero.Instance.CurrentWeapon = Hero.Instance.PrimaryWeapon;  // Setze die Waffe als aktuelle Waffe des Helden

							weapon.IsPickupable = false;  // Setze die Waffe auf nicht mehr aufnehmbar

							Entities.RemoveAt(i);  // Lösche die Waffe aus der Liste
							break;  // Beende die Suche, da nur eine Waffe aufgenommen werden kann
						}
					}
				}
			}
		}

		public void CheckForEntityInTriangle(Entity activator, Vector2 heroPos, Vector2 leftPoint, Vector2 rightPoint)
		{
			foreach (var entity in Entities)
			{
				if (entity is Agent agent && !(activator is Agent))
				{
					// Überprüfung, ob der Agent sich innerhalb des Dreiecks befindet
					if (MathHelpers.PointInTriangle(heroPos, leftPoint, rightPoint, agent.Position))
					{
						if (entity is Grunt) {
							// Berechne die Richtung zwischen Projektil und Grunt
							Vector2 direction = entity.Position - Hero.Instance.Position;
							float rotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;
							//Drehung um 180 Grad
							rotation += MathHelper.Pi;

							// Erstelle die Leiche mit Rotation und füge sie in den Puffer hinzu
							Corpse corpse = new Corpse(entity.Position)
                        	{
                            Rotation = rotation
                        	};
                        	Add(corpse); // Füge die Leiche zur Pufferliste hinzu

							AddedEntities.Add(new MachineGun(entity.Position)); // Waffen Drop des Grunt
							AddedEntities.Add(new Blood(entity.Position)); // Füge Blut hinzu
						}
						
						KillEntity(entity);
					}
				}
				else if (entity is Hero hero && activator is Agent)
				{
					// Überprüfung, ob der Hero sich innerhalb des Dreiecks befindet
					if (MathHelpers.PointInTriangle(heroPos, leftPoint, rightPoint, hero.Position))
					{
						KillEntity(hero); // Zur Liste hinzufügen; // Hero wird getötet
					}
				}
			}
		}

		public void ResetCounters()
		{
			TotalGruntCount = Entities.Count(e => e is Grunt);
			GruntKilledCount = 0;
		}

		public void ResetLevel()
		{
			// Behalte den Spieler (Hero) und entferne alle anderen Entitäten
			var player = Hero.Instance;
			Entities = Entities.Where(e => e == player).ToList();

			// Setzt alle Zähler zurück
			ResetCounters();
		}
		
		public void ClearAllEntities()
		{
			// Iteriere durch alle Entitäten und rufe KillEntity auf, um jede zu entfernen
			foreach (var entity in Entities.ToList()) // .ToList() wird verwendet, um eine Kopie der Liste zu erstellen, da wir die Original-Liste während der Iteration ändern
			{
				KillEntity(entity); // Entität wird entfernt oder stirbt
			}

			// Optional: Alle zusätzlichen Entitäten, die eventuell noch in AddedEntities sind, auch entfernen
			foreach (var entity in AddedEntities.ToList())
			{
				KillEntity(entity); // Entferne oder töte auch hinzugefügte Entitäten
			}

			// Leere die Listen vollständig
			Entities.Clear();
			AddedEntities.Clear();
			ResetCounters();

			Debug.WriteLine("Alle Entitäten wurden gelöscht.");
		}

		public void KillEntity(Entity entity)
		{
			if (entity is Boss boss)
			{

				// Überprüfe, ob der Boss noch Leben hat
				if (boss.CurrentHealth <= 0)
				{
					// Wenn der Boss keine Lebenspunkte mehr hat, führe die Methode Die aus
					boss.Die();
					Debug.WriteLine("Boss wurde entfernt.");
				}
				else
				{
					// Der Boss hat noch Leben, daher nur Schaden zufügen und nicht sterben
					Debug.WriteLine("Boss hat noch Leben.");
				}
				return; // Den Rest der Methode überspringen
			}

			if (entity is Hero hero)
			{
				// Überprüfung held Gesundheit
				if (hero.ArmorCount < 0)
				{
					//hero.Die();
					Debug.WriteLine("Hero wurde entfernt.");
				}
				else
				{
					Debug.WriteLine("Hero sollte tot sein");
				}
				return;
			}

			// Überprüfe den Typ der Entität und führe spezifische Logik aus
			if (entity is Grunt)
			{
				GruntKilledCount++; // Zähle Grunt-Kill
				AchievementManager.IncrementAchievement("Grunt"); // Erhöhe Erfolgsfortschritt
			}
			else if (entity is CctvCam)
			{
				AchievementManager.IncrementAchievement("CCTV"); // Erfolgsfortschritt für CCTV
			}

			// Entferne die Entität aus der Liste
			Debug.WriteLine($"KillEntity called for {entity.GetType().Name}. IsExpired: {entity.IsExpired}");

			entity.Die();

			// Rufe SaveAchievements auf, um Fortschritt zu speichern
			AchievementManager.SaveAchievements();
		}

		private void CheckForBulletCollision()
		{
			foreach (var bullet in Entities.OfType<Bullet>().ToList()) // Iteriert über alle Bullet-Entities
			{
				foreach (var entity in Entities.OfType<Entity>().Where(e => e is Agent || e is Hero).ToList()) // Suche nach Agent oder Hero
				{
					// Kollision zwischen Bullet und Entity
					if (bullet.Bounds.Intersects(entity.Bounds))
					{
						
						if (bullet.Shooter == entity)
						{
							continue;
						}

						//Console.WriteLine($"shooter: {bullet.Shooter} entity: {entity.GetType()}");

						// Entfernt Grunt, wenn die Bullet nicht von einem Agenten des gleichen Typs stammt
						if (entity is Grunt grunt && !grunt.IsExpired)
						{
							 // Berechne die Richtung zwischen Projektil und Grunt
							Vector2 direction = grunt.Position - bullet.Position;
							float rotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;
							//Drehung um 180 Grad
							rotation += MathHelper.Pi;

							KillEntity(entity);
							KillEntity(bullet);
							
							// Erstelle die Leiche mit Rotation und füge sie in den Puffer hinzu
							Corpse corpse = new(grunt.Position)
                        	{
                            	Rotation = rotation
                        	};
                        	Add(corpse); // Füge die Leiche zur Pufferliste hinzu

							AddedEntities.Add(new MachineGun(grunt.Position)); // Waffen Drop des Grunt
							AddedEntities.Add(new Blood(grunt.Position)); // Füge Blut hinzu
							break; // Verhindert Mehrfachtreffer(besonders wichtig für den Fehler mit der Shotgun gewesen!)
						}

						// Entfernt Hero, wenn die Bullet von einem Agenten stammt
						if (entity is Hero hero && bullet.Shooter is Agent)
						{
							hero.TakeDamage();
							KillEntity(bullet);
							break;
						}

						// eigene Logik für Bossgegner, da er nicht direkt sterben soll
						if (entity is Boss boss && !(bullet.Shooter is Agent))
						{
							boss.TakeDamage(10); // Boss erleidet Schaden basierend auf der Zahl
							KillEntity(bullet);
							break;
						}
						// Entfernt Kamera, wenn der Held schießt
						if (entity is CctvCam cam && bullet.Shooter is Hero)
						{
							KillEntity(entity);
							KillEntity(bullet);
							break; // Verhindert Mehrfachtreffer(besonders wichtig für den Fehler mit der Shotgun gewesen!)
						}

						//Debug.WriteLine($"Entity {entity.GetType().Name} wurde von einer Bullet getroffen und entfernt.");
					}
				}
			}
		}

		public void Blackout()
		{
			foreach (var light in Entities.OfType<LightSource>())
			{
				light.Toggle();
			}
		}

		public void Update()
		{
			IsUpdating = true;
			foreach (Entity entity in Entities.Where(entity => !entity.IsExpired))
			{
				entity.Update();
				entity.LateUpdate();
			}
			IsUpdating = false;

			// Neue Entitäten hinzufügen
			Entities.AddRange(AddedEntities);
			AddedEntities.Clear();
			
			// Entferne abgelaufene Entitäten und markiere sie für die Entfernung
			Entities = Entities.Where(x => !x.IsExpired).ToList();

			// Überprüfe, ob eine Kollision zwischen Spieler und AI vorliegt
			CheckForBulletCollision();

			// Überprüfe, ob eine Waffe aufgenommen werden kann
			CheckForWeaponPickup();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			// Sortiere die Entitäten nach LayerDepth, um die richtige Zeichnungsreihenfolge zu garantieren
			foreach (var entity in Entities.OrderBy(e => e.LayerDepth))
			{
				entity.Draw(spriteBatch);
			}
		}

		#if DEBUG
		public void DrawDebug(SpriteBatch spriteBatch)
		{
			foreach (var entity in Entities)
			{
				entity.DrawDebug(spriteBatch);
			}
		}
		#endif

		public List<Entity> GetEntities()
		{
			return Entities;
		}

		public int GetTotalGruntCount()
		{
			return TotalGruntCount;
		}

		public int GetGruntKilledCount()
		{
			return GruntKilledCount;
		}

		public void Clear()
		{
			Entities.Clear();
			AddedEntities.Clear();
			//ResetAll();
		}
	}
}
