
using System;
using Microsoft.Xna.Framework.Audio; 
using Microsoft.Xna.Framework.Media;
using Silent_Shadow.Models;
using Silent_Shadow.Models.Weapons;

namespace Silent_Shadow.Managers
{
	public static class SoundManager
	{
		private static SoundEffect _pistolShot;
		private static SoundEffect _shotgunShot;
		private static SoundEffect _knifeSwing;
		private static SoundEffect _machineGunShot;
		private static SoundEffect _buttonClick;
		private static SoundEffect _powerBox;
		private static Song _menuMusic;
		private static Song _levelSong1;
		private static Song _levelSong2;
		private static Song _levelSong3;
		private static Song _combatSong1;
		private static Song _combatSong2;
		private static Song _combatSong3;
		private static Song _countdown;
		private static Song _roshan;
		private static Song _killed;

		private static SoundEffectInstance _currentWeaponSoundInstance;

		// Laden der Inhalte (sollte einmal zu Beginn des Spiels erfolgen)
		public static void LoadContent()
		{

			//Stellt die Lautstäreke von allen Geräuschen ein
			SoundEffect.MasterVolume = 0.2f;
			MediaPlayer.Volume = 0.2f;
			//Sorgt dafür das sich Lieder wiederholen
			MediaPlayer.IsRepeating = true;

			//Weaponsounds
			_pistolShot = Globals.Content.Load<SoundEffect>("Sounds/Effects/Weapons/pistol");
			_shotgunShot = Globals.Content.Load<SoundEffect>("Sounds/Effects/Weapons/shotgun");
			_knifeSwing = Globals.Content.Load<SoundEffect>("Sounds/Effects/Weapons/knifeswing");
			_machineGunShot = Globals.Content.Load<SoundEffect>("Sounds/Effects/Weapons/maschinegun");
			_buttonClick = Globals.Content.Load<SoundEffect>("Sounds/Effects/click");

			//Sounds
			_powerBox = Globals.Content.Load<SoundEffect>("Sounds/Effects/powerboxsound");
	
			// Hintergrundmusik laden
			_menuMusic = Globals.Content.Load<Song>("Sounds/Music/Menu/main-menu");
			_levelSong1 = Globals.Content.Load<Song>("Sounds/Music/Levels/level-track-1");
			_levelSong2 = Globals.Content.Load<Song>("Sounds/Music/Levels/level-track-2");
			_levelSong3 = Globals.Content.Load<Song>("Sounds/Music/Levels/level-track-3");

			_combatSong1 = Globals.Content.Load<Song>("Sounds/Music/Combat/battle-1");
			_combatSong2 = Globals.Content.Load<Song>("Sounds/Music/Combat/battle-2");
			_combatSong2 = Globals.Content.Load<Song>("Sounds/Music/Combat/battle-3");

			_countdown = Globals.Content.Load<Song>("Sounds/Music/countdown");
			_roshan = Globals.Content.Load<Song>("Sounds/Music/Combat/roshan");

			_killed = Globals.Content.Load<Song>("Sounds/Music/killed");

			// Erstellen der initialen SoundEffectInstance (wird später je nach Waffe ersetzt)
			//_currentWeaponSoundInstance = _pistolShot.CreateInstance();  // Initialisiert mit einem Standard-Waffensound
		}

		// Abspielen des Schussgeräusches basierend auf der Waffe
		public static void PlayWeaponSound(Type weaponType)
		{
			if (weaponType == null)
			{
				Console.WriteLine("Error: Weapon type is null.");
				return;
			}

			if (weaponType == typeof(Pistol))
			{
				_currentWeaponSoundInstance = _pistolShot.CreateInstance();
				_currentWeaponSoundInstance.Play();
			}
			else if (weaponType == typeof(Shotgun))
			{
				_currentWeaponSoundInstance = _shotgunShot.CreateInstance();
				_currentWeaponSoundInstance.Play();
			}
			else if (weaponType == typeof(Knife))
			{
				_currentWeaponSoundInstance = _knifeSwing.CreateInstance();
				_currentWeaponSoundInstance.Play();
			}
			else if (weaponType == typeof(MachineGun))
			{
				_currentWeaponSoundInstance = _machineGunShot.CreateInstance();
				_currentWeaponSoundInstance.Play();
			}
		}
		
		public static void PlaySound(String soundName){
			if (soundName == null)
			{
				Console.WriteLine("Error: Sound Name is null.");
				return;
			}

			if (soundName == "powerbox")
			{
				_currentWeaponSoundInstance = _powerBox.CreateInstance();
				_currentWeaponSoundInstance.Play();
			}
		}
		public static void PlayMenuMusic()
		{
			MediaPlayer.Play(_menuMusic);
		}

		//	Stoppt den aktuellen Song
		public static void StopMusic()
		{
			MediaPlayer.Stop();
		}

		public static void PlayButtonSound()
		{
			_buttonClick.Play();
		}

		// Abspielen der Hintergrundmusik für den Raum
		public static void PlayBackgroundMusic(int level)
		{
			StopMusic();

			switch (level)
			{
				case 0:
					MediaPlayer.Play(_levelSong1);
					break;
				case 1:
					MediaPlayer.Play(_levelSong2);
					break;
				case 2:
					MediaPlayer.Play(_levelSong3);
					break;
				case 99:
					MediaPlayer.Play(_killed);
					break;
				default:
					MediaPlayer.Stop(); // Musik stoppen, wenn kein Raum zugeordnet ist
					break;
			}
		}

		public static void PlayCombatTrack(int track)
		{
			StopMusic();

			switch (track)
			{
				case 0:
					MediaPlayer.Play(_countdown); // searching?
					break;
				case 1:
					MediaPlayer.Play(_combatSong1);
					break;
				case 2:
					MediaPlayer.Play(_combatSong2);
					break;
				case 3:
					MediaPlayer.Play(_combatSong3);
					break;
				case 4:
					MediaPlayer.Play(_roshan); // boss-fight maybe?
					break;
				default:
					MediaPlayer.Stop();
					break;
			}
		}
	}
}