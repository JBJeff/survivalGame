using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silent_Shadow.Managers;
using Silent_Shadow.Managers.EntityManager;
using Silent_Shadow.Models;
using Silent_Shadow.Models.Weapons;
using Silent_Shadow.States;

namespace Silent_Shadow.GUI
{
	public class InfoDisplay(SpriteFont font, Texture2D reloadIcon, Vector2 position)
	{
		private readonly SpriteFont font = font; // Schriftart für Textanzeige
		private readonly Texture2D reloadIcon = reloadIcon; // Bild für das Nachlade-Symbol
		private Vector2 position = position; // Position der Anzeige
		private readonly Hero hero = Hero.Instance; // Referenz auf das Hero-Objekt
		protected Texture2D spriteMp5 = Globals.Content.Load<Texture2D>("Sprites/mp5");
		protected Texture2D spriteShotgun = Globals.Content.Load<Texture2D>("Sprites/shotgun");
		protected Texture2D spritePistol = Globals.Content.Load<Texture2D>("Sprites/pistol");
		private readonly Texture2D spriteKnife = Globals.Content.Load<Texture2D>("Sprites/knife");
		private Knife knife;

		//private readonly IEntityManager _entityMgr = EntityManagerFactory.GetInstance();

		// Dynamische FontSize
		private float fontSize = 15f;  // Dynamische Schriftgröße
		private const float StandardFontSize = 24f;  // Standard Schriftgröße zur Berechnung der Skalierung

		// für "Game Over" Szenario aktivieren
		public bool _isGameOver = false;  // Standardwert false

		public void Update() { }

		// Methode zum dynamischen Anpassen der Schriftgröße
		public void SetFontSize(float newSize)
		{
			fontSize = newSize; // Schriftgröße anpassen
		}
		//Methode um das Spiel auf GameOver zu setzen
		public void SetGameOver(bool isGameOver)
		{
			_isGameOver = isGameOver;
			Debug.WriteLine($"GameOver gesetzt: {_isGameOver}");
		}


		public void Draw(SpriteBatch spriteBatch)
		{
			// Waffe explizit als Weapon casten, um auf die Eigenschaften zuzugreifen
			var weapon = hero.GetCurrentWeapon() as Weapon;

			// Berechnung des Skalierungsfaktors für die Symbole
			float scaleFactor = fontSize / StandardFontSize;

			// Text mit dynamischer Skalierung
			if (weapon != null && !(weapon is Knife))
			{
				string ammoText = $"{weapon.Ammo}/{weapon.MaxAmmo}";  // Zeigt verbleibende und max. Schüsse an
				spriteBatch.DrawString(font, ammoText, position, Color.White, 0f, Vector2.Zero, scaleFactor, SpriteEffects.None, 0f);
			}

			// Passendes Waffensymbol anzeigen und mit scaleFactor skalieren
			if (weapon is MachineGun)
			{
				// Dynamische Position und Skalierung für MachineGun
				Vector2 iconPosition = new Vector2(position.X - 140 * scaleFactor, position.Y); // Skalierte Position
				spriteBatch.Draw(spriteMp5, iconPosition, null, Color.White, 0f, Vector2.Zero, 0.6f * scaleFactor, SpriteEffects.None, 0f);
			}
			else if (weapon is Shotgun)
			{
				// Dynamische Position und Skalierung für Shotgun
				Vector2 iconPosition = new Vector2(position.X - 160 * scaleFactor, position.Y); // Skalierte Position
				spriteBatch.Draw(spriteShotgun, iconPosition, null, Color.White, 0f, Vector2.Zero, 0.5f * scaleFactor, SpriteEffects.None, 0f);
			}
			else if (weapon is Pistol)
			{
				// Dynamische Position und Skalierung für Pistol
				Vector2 iconPosition = new Vector2(position.X - 90 * scaleFactor, position.Y); // Skalierte Position
				spriteBatch.Draw(spritePistol, iconPosition, null, Color.White, 0f, Vector2.Zero, 0.8f * scaleFactor, SpriteEffects.None, 0f);
			}
			else if (weapon is Knife)
			{
				// Dynamische Position und Skalierung für Knife
				Vector2 iconPosition = new Vector2(position.X - 90 * scaleFactor, position.Y); // Skalierte Position
				spriteBatch.Draw(spriteKnife, iconPosition, null, Color.White, 0f, Vector2.Zero, 0.5f * scaleFactor, SpriteEffects.None, 0f);
			}

			// Wenn die Waffe nachlädt, das Nachlade-Symbol anzeigen und skalieren
			if (weapon != null && weapon.Reloading)
			{
				Vector2 reloadPosition = new Vector2(position.X + 100 * scaleFactor, position.Y); // Rechts neben der Munition
				spriteBatch.Draw(reloadIcon, reloadPosition, null, Color.White, 0f, Vector2.Zero, scaleFactor, SpriteEffects.None, 0f);
			}

			// Armor anzeigen
			string healthText = $"Schutzplatten: {hero.ArmorCount}/3";
			Vector2 healthPosition = new Vector2(position.X - Globals.ScreenWidth / 2 + 125, position.Y + 300);
			spriteBatch.DrawString(font, healthText, healthPosition, Color.Red, 0f, Vector2.Zero, scaleFactor, SpriteEffects.None, 0f);

			// Gegneranzahl und -fortschritt anzeigen
			string enemyProgress = $"Grunt: {EntityManager.Instance.GruntKilledCount}/{EntityManager.Instance.TotalGruntCount}";
			spriteBatch.DrawString(font, enemyProgress, new Vector2(position.X - Globals.ScreenWidth / 2 + 125, position.Y), Color.White, 0f, Vector2.Zero, scaleFactor, SpriteEffects.None, 0f);

			// Mission erfolgreich anzeigen
			if (EntityManager.Instance.GruntKilledCount == EntityManager.Instance.TotalGruntCount)
			{
				string missionSuccess = "Mission erfolgreich!";
				spriteBatch.DrawString(font, missionSuccess, new Vector2(position.X - Globals.ScreenWidth / 2 + 125, position.Y + 40), Color.Green, 0f, Vector2.Zero, scaleFactor, SpriteEffects.None, 0f);
			}

			// Zeige Informationen zur Waffe an, wenn der Spieler in Reichweite ist und die Waffe aufheben kann
			var pickableWeapon = EntityManager.Instance.Entities
							.FirstOrDefault(e => e is Weapon weapon && Vector2.Distance(Hero.Instance.Position, weapon.Position) < 25f);

			if (pickableWeapon != null)
			{
				// Visuelle Rückmeldung, dass die E-Taste gedrückt werden kann
				string pickupText = "Druecke E, um Waffe aufzuheben!";
				Vector2 pickupTextPosition = new Vector2(Hero.Instance.Position.X, Hero.Instance.Position.Y - 40);
				spriteBatch.DrawString(font, pickupText, pickupTextPosition, Color.White, 0f, Vector2.Zero, fontSize / 40f, SpriteEffects.None, 0f);
			}

			//Powerbox Anzeige
			// PowerBox-Nachricht anzeigen
			var powerBox = EntityManager.Instance.Entities
							.FirstOrDefault(e => e is PowerBox box && box.IsPlayerNear) as PowerBox;

			if (powerBox != null)
			{
				string powerBoxText = "Drücke E, um das Licht an/auszuschalten!";
				Vector2 powerBoxTextPosition = new Vector2(Hero.Instance.Position.X, Hero.Instance.Position.Y - 40);
				spriteBatch.DrawString(font, powerBoxText, powerBoxTextPosition, Color.Yellow, 0f, Vector2.Zero, fontSize / 40f, SpriteEffects.None, 0f);
			}

			// Game Over Nachricht anzeigen
			if (GameState.IsGameOver)
			{
				//Debug.WriteLine("Game Over Condition erfüllt. Zeichne Text.");
				string gameOverText = "Game Over";
				float xOffset = -600 * scaleFactor; // Verschiebung nach links
				float yOffset = 100; // Verschiebung nach unten

				// Verwendet die Spielerposition für die Berechnung der Textposition
				Vector2 gameOverPosition = new Vector2(
					position.X + xOffset,
					position.Y + yOffset
				);

				spriteBatch.DrawString(font, gameOverText, gameOverPosition + new Vector2(2, 2), Color.Black, 0f, Vector2.Zero, scaleFactor * 3, SpriteEffects.None, 0f);
				spriteBatch.DrawString(font, gameOverText, gameOverPosition, Color.Red, 0f, Vector2.Zero, scaleFactor * 3, SpriteEffects.None, 0f);
			}
			
			if (GameState.IsGameCompleted)
			{
				{
					
					string gameOverText = "Spiel erfolgreich!";
					float xOffset = -600 * scaleFactor; // Verschiebung nach links
					float yOffset = 100; // Verschiebung nach unten

					// Verwendet die Spielerposition für die Berechnung der Textposition
					Vector2 gameOverPosition = new Vector2(
						position.X + xOffset,
						position.Y + yOffset
					);

					spriteBatch.DrawString(font, gameOverText, gameOverPosition + new Vector2(2, 2), Color.Black, 0f, Vector2.Zero, scaleFactor * 3, SpriteEffects.None, 0f);
					spriteBatch.DrawString(font, gameOverText, gameOverPosition, Color.Green, 0f, Vector2.Zero, scaleFactor * 3, SpriteEffects.None, 0f);
				}
			}
		}
	}
}
