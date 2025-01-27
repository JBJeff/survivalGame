using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silent_Shadow.Managers;

namespace Silent_Shadow.Models
{
	public class Armor : Entity
	{
		public Armor(Vector2 position)
		{
			Position = position;
			Sprite = Globals.Content.Load<Texture2D>("Sprites/Schild"); // Pfad zur Textur
			Size = 1f;
			LayerDepth = 0f;
		}

		public override void Update()
		{
			// Prüft ob der Spieler in der Nähe ist
			float distance = Vector2.Distance(Hero.Instance.Position, Position);

			if (distance < 50f) // Spieler muss nah genug sein
			{
				if (Hero.Instance.CanPickupArmor) // Nur, wenn Cooldown abgelaufen ist
				{
					Hero.Instance.AddArmor(); // Schutzweste hinzufügen
					Hero.Instance.StartArmorCooldown(); // Cooldown starten
					IsExpired = true; // Entferne die Entität
				}
			}
		}
	}
}

