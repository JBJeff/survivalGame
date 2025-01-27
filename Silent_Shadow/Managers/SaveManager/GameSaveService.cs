
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Silent_Shadow._Managers.SaveManager;
using Silent_Shadow.Models;
using Silent_Shadow.Models.Weapons;

namespace Silent_Shadow._Managers.SaveManager
{
	public static class GameSaveService
	{
		public static void SaveGameState(int currentLevelIndex, Hero hero)
		{
			var saveData = new Data
			{
				PlayerLevel = currentLevelIndex,
				CurrentWeapon = hero.CurrentWeapon?.WeaponName,
				PlayerPosition = hero.Position
			};

			SaveGameManager.SaveGameData(saveData);
			Debug.WriteLine("Spielstand gespeichert!");
		}

		public static bool TryLoadGameState(out int playerLevel, out Vector2 playerPosition, out Weapon playerWeapon)
		{
			var saveData = SaveGameManager.LoadGame();
			if (saveData == null)
			{
				Debug.WriteLine("Kein gespeicherter Spielstand gefunden!");
				playerLevel = 0;
				playerPosition = Vector2.Zero;
				playerWeapon = null;
				return false; // Laden fehlgeschlagen
			}

			// Spiel-Daten auslesen
			playerLevel = saveData.PlayerLevel;
			playerPosition = saveData.PlayerPosition;
			playerWeapon = CreateWeaponByID(saveData.CurrentWeapon);
			return true; // Laden erfolgreich
		}


		private static Weapon CreateWeaponByID(string weaponID)
		{
			return weaponID switch
			{
				"Pistol" => new Pistol(new Vector2(0, 0)),
				"Shotgun" => new Shotgun(new Vector2(0, 0)),
				"MachineGun" => new MachineGun(new Vector2(0, 0)),
				"Knife" => new Knife(),
				_ => null
			};
		}
	}
}
