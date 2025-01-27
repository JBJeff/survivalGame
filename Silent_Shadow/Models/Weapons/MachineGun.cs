using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silent_Shadow.Managers.EntityManager;

namespace Silent_Shadow.Models.Weapons
{
    public class MachineGun : Weapon
    {
		private IEntityManager _entityMgr;

		public MachineGun(Vector2 _position)
        {
			WeaponName = "MachineGun";

			_entityMgr = EntityManagerFactory.GetInstance();

			Cooldown = 0.1f;
            MaxAmmo = 15;         // Verwende die public Eigenschaft MaxAmmo
            Ammo = MaxAmmo;       // Setze die Anfangsmunition auf das Maximum
            reloadTime = 2f;      // Zeit zum Nachladen

            Sprite = Globals.Content.Load<Texture2D>("Sprites/mp5");
			SpriteOffset = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
			Position = _position;
			Rotation = 0f;
			Speed = 80f;
			Size = 0.3f;
        }

		protected override void CreateProjectiles(Entity shooter)
        {
            Vector2 direction = new((float) Math.Cos(shooter.Rotation), (float) Math.Sin(shooter.Rotation));
			Bullet bullet = new(shooter, direction, 1); // Erstellt ein Projektil
			_entityMgr.Add(bullet); 
        }
	}
}
