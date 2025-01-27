using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Silent_Shadow.Managers
{
	public static class AchievementManager
	{
		// Speicherort der Datei im übergeordneten Verzeichnis
		private const string AchievementFile = "../Achievements.txt";

		public static Dictionary<string, int> Achievements { get; private set; } = new Dictionary<string, int>
	{
		{ "Grunt", 0 },
		{ "CCTV", 0 }
	};

		public static void LoadAchievements()
		{
			// Überprüfen, ob die Datei existiert
			if (!File.Exists(AchievementFile))
			{
				// Datei erstellen und Standardwerte speichern
				SaveAchievements();
				return; // Nach Erstellung der Datei ist kein weiteres Laden erforderlich
			}

			// Datei lesen und Achievements laden
			var lines = File.ReadAllLines(AchievementFile);
			foreach (var line in lines)
			{
				var parts = line.Split(':');
				if (parts.Length == 2 && Achievements.ContainsKey(parts[0]))
				{
					Achievements[parts[0]] = int.Parse(parts[1]);
				}
			}
		}

		public static void SaveAchievements()
		{
			// Achievements als Text formatieren und in die Datei schreiben
			var lines = Achievements.Select(kvp => $"{kvp.Key}:{kvp.Value}");
			File.WriteAllLines(AchievementFile, lines);
		}

		public static void IncrementAchievement(string type)
		{
			// Achievement-Wert erhöhen, wenn der Typ existiert
			if (Achievements.ContainsKey(type))
			{
				Achievements[type]++;
			}
		}
	}
}