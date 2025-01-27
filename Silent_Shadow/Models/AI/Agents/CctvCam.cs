using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Silent_Shadow.Managers.EntityManager;
using Silent_Shadow.Models.AI.Actions;
using Silent_Shadow.Models.AI.Goals;

namespace Silent_Shadow.Models.AI.Agents
{
	public class CctvCam : Agent
	{
		public bool seePlayer {get; set; } = false;
		public CctvCam(Vector2 position, string name, float rotation, List<Goal> goals, List<GAction> actions) : base(name, rotation, 0f, 0f, 0f, goals, actions)
		{
			Sprite = Globals.Content.Load<Texture2D>("LooseSprites/camera");
			SpriteOffset = new Vector2(Sprite.Width / 2, Sprite.Height / 2);

			Position = position;
			Size = 0.4f;
		}

		public override bool PlayerDetected(float deltaTime)
		{
			Vector2 direction = MathHelpers.GetDirectionVector(Rotation, Direction.Forward);
			VisionCone = MathHelpers.GetTriangle(Position, direction, 120f, 60f);

			if (PlayerInVisionCone(Position, VisionCone[0], VisionCone[1], Hero.Instance.Position))
			{
				_detectionCounter += deltaTime * Hero.Instance.Visibility * 3f;

				if (_detectionCounter >= _detectionThreshold)
				{
					seePlayer = true; // Grunts benachrichtigen
					return true;
				}
			}
			else
			{
				_detectionCounter = 0;
				seePlayer = false;
			}

			return false;
		}

		public override void Update()
		{
			PlayerDetected(Globals.DeltaTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Sprite, Position, null, Tint, Rotation, SpriteOffset, Size, 0, 0);
		}

#if DEBUG
		public override void DrawDebug(SpriteBatch spriteBatch)
		{
			if (VisionCone != null)
			{
				#region Vision Cone debug
				spriteBatch.DrawLine(Position, VisionCone[0], Color.Orange);
				spriteBatch.DrawLine(Position, VisionCone[1], Color.Orange);
				spriteBatch.DrawLine(VisionCone[0], VisionCone[1], Color.Orange);
				#endregion

				if (_detectionCounter > 0)
				{
					spriteBatch.DrawLine(Position, Hero.Instance.Position, Color.Blue);
				}

				// Zeichnet die Kollisionsbox des Entity in Grün
				spriteBatch.DrawRectangle(Bounds, new Color(0, 255, 0, 100));

				// Benachrichtigungsradius debug
    			// float notificationRadius = 1000f; // Radius aus NotifyNearbyGrunts
    			// spriteBatch.DrawCircle(Position, notificationRadius, 50, Color.Red, 1f); // Zeichnet den Kreis
			}
		}
#endif
	}
}