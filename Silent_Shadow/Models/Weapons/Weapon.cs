
using Silent_Shadow.Managers;

namespace Silent_Shadow.Models.Weapons
{
	public abstract class Weapon : Entity
	{
		public string WeaponName { get; protected set; } // Eindeutige Erkennung der Waffe um Speichern und laden zu vereinfachen

		public float Cooldown { get; protected set; }
		protected float cooldownLeft;
		public bool IsPickupable { get; set; }  // Eigenschaft für die Aufhebbarkeit der Waffe

		public int MaxAmmo { get; protected set; }
		public int Ammo { get; set; }
		protected float reloadTime;
		public bool Reloading { get; protected set; }

		// Mit Nachladen boolean
        public bool UseAmmoSystem { get; set; } = false; // Wenn false, wird die Waffe nach Ablauf entfernt

		protected Weapon()
		{
			cooldownLeft = 0f;
			Reloading = false;
			Size = 1.0f;  // Größe des Sprites, falls benötigt
			IsPickupable = true;  // Standardmäßig auf aufnahmebereit setzen
		}

		public virtual void Reload()
		{
			if (Reloading || (Ammo == MaxAmmo))
				return;
			cooldownLeft = reloadTime;
			Reloading = true;
			Ammo = MaxAmmo;
		}

		protected abstract void CreateProjectiles(Entity shooter);

		public virtual void Fire(Entity shooter)
		{
			if (cooldownLeft > 0 || Reloading)
			{
				return;
			}
			
			if (Ammo > 0)
			{
				CreateProjectiles(shooter);
				Ammo--;
				// Schussgeräusch abspielen
				SoundManager.PlayWeaponSound(GetType());
				cooldownLeft = Cooldown;
			}
			else
			{
				if (shooter is Hero && UseAmmoSystem)
				{
					Reload(); 
				}
				else
				{
					IsExpired = true;
				}
			}
		}

		public override void Update()
		{
			if (cooldownLeft > 0)
			{
				cooldownLeft -= Globals.DeltaTime;
			}
			else if (Reloading)
			{
				Reloading = false;
			}
		}
	}
}
