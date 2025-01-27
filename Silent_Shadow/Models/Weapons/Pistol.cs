
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silent_Shadow.Managers.EntityManager;

namespace Silent_Shadow.Models.Weapons
{
    public class Pistol : Weapon
    {
		private IEntityManager _entityMgr;

		public Pistol(Vector2 _position)
        {
			WeaponName = "Pistol"; 

			_entityMgr = EntityManagerFactory.GetInstance();

			Cooldown = 0.5f;  // Längere Abklingzeit 
            MaxAmmo = 10;     // Maximale Munition 
            Ammo = MaxAmmo;   // Setze die Anfangsmunition auf das Maximum
            reloadTime = 1.5f; // Zeit zum Nachladen

            Sprite = Globals.Content.Load<Texture2D>("Sprites/pistol");
			SpriteOffset = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
			Position = _position;
			Rotation = 0f;
			Speed = 80f;
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