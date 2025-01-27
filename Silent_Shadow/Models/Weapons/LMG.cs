using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silent_Shadow.Managers;
using Silent_Shadow.Managers.EntityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silent_Shadow.Models.Weapons
{
	public class LMG : Weapon
	{
		private IEntityManager _entityMgr;

		public LMG(Vector2 _position)
		{
			_entityMgr = EntityManagerFactory.GetInstance();

			Cooldown = 0.1f;
			MaxAmmo = 15;        // Verwende die public Eigenschaft MaxAmmo
			Ammo = MaxAmmo;       // Setze die Anfangsmunition auf das Maximum
			reloadTime = 10f;      // Zeit zum Nachladen

			Sprite = Globals.Content.Load<Texture2D>("Sprites/mp5");
			SpriteOffset = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
			Position = _position;
			Rotation = 0f;
			Speed = 1f;
			Size = 0.3f;
		}

		protected override void CreateProjectiles(Entity shooter)
		{
			Vector2 direction = new Vector2((float)Math.Cos(shooter.Rotation), (float)Math.Sin(shooter.Rotation));
			Bullet bullet = new Bullet(shooter, direction, 1); // Erstellt ein Projektil in die Richtung des Helden
			_entityMgr.Add(bullet);
		}
	}
}
