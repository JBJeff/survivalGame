
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silent_Shadow.Managers.EntityManager;

namespace Silent_Shadow.Models.Weapons
{
 	public class Shotgun : Weapon
    {
		private IEntityManager _entityMgr;

		public Shotgun(Vector2 _position) 
        {
			WeaponName = "ShotGun";

			_entityMgr = EntityManagerFactory.GetInstance();

			Cooldown = 1.0f;  // Längere Abklingzeit 
            MaxAmmo = 6;      // Maximale Munition für die Schrotflinte
            Ammo = MaxAmmo;   // Setze die Anfangsmunition auf das Maximum
            reloadTime = 2.5f; // Zeit zum Nachladen

            Sprite = Globals.Content.Load<Texture2D>("Sprites/shotgun");
			SpriteOffset = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
			Position = _position;
			Rotation = 0f;
			Speed = 80f;
			Size = 0.23f;
        }

		protected override void CreateProjectiles(Entity shooter)
        {
			Vector2 direction = new Vector2((float)Math.Cos(shooter.Rotation), (float)Math.Sin(shooter.Rotation));

			Bullet bullet = new Bullet(shooter, direction, 1); // Erstes Projektil (mittig)
			_entityMgr.Add(bullet);

            // Winkel für die seitlichen Schüsse
            float spreadAngle = MathHelper.ToRadians(4f); // 10 Grad Streuwinkel
            Vector2 leftDirection = Vector2.Transform(direction, Matrix.CreateRotationZ(-spreadAngle));
            Vector2 rightDirection = Vector2.Transform(direction, Matrix.CreateRotationZ(spreadAngle));

			bullet = new Bullet(shooter, leftDirection, 1); // Zweites Projektil (links)
			_entityMgr.Add(bullet);
			bullet = new Bullet(shooter, rightDirection, 1); // Drittes Projektil (rechts)
			_entityMgr.Add(bullet);
		}
	}
}
