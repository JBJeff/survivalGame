
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Silent_Shadow.Managers.CollisionManager
{
	public static class CollisionManager
	{
		// Methode zur Überprüfung der Kollision mit allgemeinen Rechtecken
		public static bool IsCollidingWithAnyWall(Vector2 newPosition, List<Rectangle> colliders, int offsetX, int offsetY, int width, int height)
		{
			Rectangle newBounds = new(
				(int)newPosition.X + offsetX,
				(int)newPosition.Y + offsetY,
				width,
				height
			);

			foreach (var rect in colliders)
			{
				if (newBounds.Intersects(rect))
				{
					return true; // Kollision erkannt
				}
			}

			return false; // Keine Kollision
		}

		// Methode zur Überprüfung der Kollision mit Ausgängen
		public static bool IsCollidingWithExitpoint(Vector2 newPosition, List<Rectangle> exitPoints, int offsetX, int offsetY, int width, int height)
		{
			Rectangle newBounds = new(
				(int)newPosition.X + offsetX,
				(int)newPosition.Y + offsetY,
				width,
				height
			);

			foreach (var rect in exitPoints)
			{
				if (newBounds.Intersects(rect))
				{
					return true; // Kollision erkannt
				}
			}

			return false; // Keine Kollision
		}

		public static bool IsCollidingWithArmor(Vector2 newPosition, List<Rectangle> armor, int offsetX, int offsetY, int width, int height)
		{
			Rectangle newBounds = new(
				(int)newPosition.X + offsetX,
				(int)newPosition.Y + offsetY,
				width,
				height
			);

			foreach (var rect in armor)
			{
				if (newBounds.Intersects(rect))
				{
					return true; // Kollision erkannt
				}
			}

			return false; // Keine Kollision
		}

		public static bool IsCollidingWithCheckpoint(Vector2 newPosition, List<Rectangle> checkPoint, int offsetX, int offsetY, int width, int height)
		{
			Rectangle newBounds = new(
				(int)newPosition.X + offsetX,
				(int)newPosition.Y + offsetY,
				width,
				height
			);

			foreach (var rect in checkPoint)
			{
				if (newBounds.Intersects(rect))
				{
					return true; // Kollision erkannt
				}
			}

			return false; // Keine Kollision
		}

		public static Vector2 FindValidPosition(Vector2 position, Rectangle bounds, List<Rectangle> colliders)
        {
            const int maxAttemptsPerDirection = 100; // Maximale Anzahl der Versuche pro Richtung
            const float stepSize = 1f;              // Schrittgröße, mit der die Position verschoben wird
            Vector2[] directions = new[]
            {
                new Vector2(-stepSize, 0), // Links
                new Vector2(stepSize, 0),  // Rechts
                new Vector2(0, -stepSize), // Oben
                new Vector2(0, stepSize)   // Unten
            };

			// Prüfe jede Richtung separat
            foreach (var direction in directions)
            {
                for (int attempt = 0; attempt < maxAttemptsPerDirection; attempt++)
                {
                    Vector2 newPosition = position + direction * attempt;

                    if (!IsCollidingWithAnyWall(newPosition, colliders, -35, -35, bounds.Width, bounds.Height))
                    {
                        Console.WriteLine($"Gültige Position fuer Corspe gefunden: {newPosition}");
                        return newPosition;
                    }
                }
            }

            Console.WriteLine("Keine gültige Position fuer Corpse gefunden");
            // Gib die ursprüngliche Position zurück, falls keine gültige gefunden wurde
            return position;
        }

	}
}
