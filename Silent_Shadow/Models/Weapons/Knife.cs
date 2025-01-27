
using Microsoft.Xna.Framework;
using Silent_Shadow.Managers;
using Silent_Shadow.Managers.EntityManager;

namespace Silent_Shadow.Models.Weapons
{
    public class Knife : Weapon
    {
		private readonly IEntityManager _entityMgr;

        private const float Range = 90f;
        private const float Angle = 15f;

        public Knife()
        {
			_entityMgr = EntityManagerFactory.GetInstance();
			WeaponName = "Knife";
            MaxAmmo = 1; 

            Ammo = MaxAmmo;
            IsPickupable = true;
            reloadTime = 1f; // 1 Sekunden
        }

        protected override void CreateProjectiles(Entity shooter)
        {
            
        }

        public override void Fire(Entity shooter)
        {
            if (cooldownLeft > 0 || Reloading) return;
        
            // Detect entities within the knife's range and angle
            var aimDirection = InputManager.GetAimDirection();
            DetectAndHitAgents(shooter.Position, aimDirection);

            Ammo = 0;
            // Start cooldown by calling Reload
            Reload();
            // ´Schwinggeräusch abspielen
            SoundManager.PlayWeaponSound(this.GetType());
        }

        private void DetectAndHitAgents(Vector2 heroPosition, Vector2 aimDirection)
        {
            Vector2 leftDir = Vector2.Transform(aimDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(-Angle)));
            Vector2 rightDir = Vector2.Transform(aimDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(Angle)));

            Vector2 leftPoint = heroPosition + leftDir * Range;
            Vector2 rightPoint = heroPosition + rightDir * Range;

			_entityMgr.CheckForEntityInTriangle(this, heroPosition, leftPoint, rightPoint);
        }

	}
}
