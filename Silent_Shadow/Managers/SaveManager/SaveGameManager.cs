using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silent_Shadow._Managers.SaveManager
{
	//Verarbeitet die daten und umweandlung der Daten, DIE METHODEN WERDEN IM GameSaveService gebraucht
	public static class SaveGameManager
	{
		// Speicherort
		private static readonly string SaveFilePath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory, // bin Debug Ordner
			"SilentShadow",                        // Ordnername innerhalb des Spiels
			"savegame.json"                        // Dateiname
		);

		public static void SaveGameData(Data saveData)
		{
			// Stelle sicher, dass der Ordner existiert
			var directory = Path.GetDirectoryName(SaveFilePath);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
				Debug.WriteLine($"Ordner erstellt: {directory}");
			}

			// Daten in JSON umwandeln
			var json = JsonConvert.SerializeObject(saveData, Formatting.Indented);

			// Schreibe die Datei
			File.WriteAllText(SaveFilePath, json);

			Debug.WriteLine($"Spielstand gespeichert in: {SaveFilePath}");
		}

		public static Data LoadGame()
		{
			if (!File.Exists(SaveFilePath))
			{
				Debug.WriteLine("Kein Spielstand gefunden. Ein neuer Spielstand wird erstellt.");
				return new Data(); // Leerer Spielstand
			}

			var json = File.ReadAllText(SaveFilePath);
			return JsonConvert.DeserializeObject<Data>(json);
		}
	}
}
