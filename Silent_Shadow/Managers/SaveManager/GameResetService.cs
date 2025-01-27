using Silent_Shadow.Models;
using Silent_Shadow.States;
using Silent_Shadow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silent_Shadow.Managers;
using Microsoft.Xna.Framework;
using Silent_Shadow.Managers.EntityManager;
using Silent_Shadow.Managers.LevelManager;

namespace Silent_Shadow._Managers.SaveManager
{
	public static class GameResetService
	{
		public static void RestartGame(GameState gameState, string levelName)
		{
			IEntityManager entityManager = EntityManagerFactory.GetInstance();
			entityManager.ClearAllEntities();
			Hero.Reset();

			gameState.ResetLevelIndex();
			gameState.LoadLevel(levelName);

			Debug.WriteLine("Spiel wurde erfolgreich neu gestartet.");
		}

		public static void RestartGameComplete()
		{
			EntityManager.Instance.ClearAllEntities();
			Hero.Reset();
			GameState.IsGameOver = false;
			GameState.IsGameCompleted = false;

			Game1.Instance.ChangeState(new GameState(Game1.Instance, Game1.Instance.Graphics.GraphicsDevice, Globals.Content));
			Debug.WriteLine("GameState neu gestartet!");
		}
	}
}