using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silent_Shadow._Managers.SaveManager
{
	//Zeigt einfach die Datenstruktur
	//Wird in der Klasse "GameState" zugegriffen um die aktuelle Werte zu Speichern
	[Serializable]
	public class Data
	{

		public int PlayerLevel { get; set; } //Level = TileMap
		
		public string CurrentWeapon { get; set; } //Aktuelle Waffe mit der das neue Level betreten wurde

		public Vector2 PlayerPosition { get; set; }

		//weitere Werte falls wir noch was anderes Speichern wollen, 
		
		//public int ArmorCount { get; set; } //Schilder	

		//Mögliche erweiterung: Crunt Speichern. Aber das passt alles nicht zum Spielprinzip, jedoch eventuell wenn man im Menü geht um dann alles wieder zu laden?
	}
}
