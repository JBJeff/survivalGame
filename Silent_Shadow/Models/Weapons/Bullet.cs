using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Silent_Shadow.Managers.CollisionManager;
using Silent_Shadow.States;
using System;

namespace Silent_Shadow.Models.Weapons
{
    public class Bullet : Entity
    {
        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)(Position.X - (Sprite.Width * Size / 2)),
                    (int)(Position.Y - (Sprite.Height * Size / 2)),
                    (int)(Sprite.Width * Size),
                    (int)(Sprite.Height * Size)
                );
            }
        }
        public Vector2 Direction { get; set; }
        private int lifespanFrames; // Anzahl Frames, die das Projektil lebt
        public int Damage { get; }
        public Entity Shooter { get; set; }

        public Bullet(Entity shooter, Vector2 direction, int damage)
        {
            Sprite = Globals.Content.Load<Texture2D>("Sprites/bullet");
            Shooter = shooter;
            Position = shooter.Position;
            Direction = direction;
            Damage = damage;
			lifespanFrames = 220; // z. B. 220 Frames Lebensdauer
            Rotation = (float) Math.Atan2(Direction.Y, Direction.X);
            Size = 0.5f;
			Speed = 900f;

            // Ursprung auf den Mittelpunkt setzen
            SpriteOffset = new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);
        }

        public override void Update()
        {
			// Bewegt das Projektil basierend auf Richtung und konstanter Geschwindigkeit
			Position += Direction * Speed * Globals.DeltaTime;

			//Kollision mit der Wand
			if (CollisionManager.IsCollidingWithAnyWall(Position, GameState.Instance.Level.Colliders, -8, -5, Bounds.Width, Bounds.Height))
			{
				IsExpired = true;
			}

			// Reduziert die Anzahl der verbleibenden Frames
			lifespanFrames--;

            // Markiert das Projektil als abgelaufen, wenn keine Frames mehr übrig sind
            if (lifespanFrames <= 0)
            {
                IsExpired = true;
            }

        }
		#region Bullet settings
		//Primär für die Boss Klasse
		public void SetBulletSpeed(float speed)
		{
			Speed = speed;
		}

		public void SetLifespan(int lifespan)
		{
			lifespanFrames = lifespan; 
		}
		
		// Setzt den Spawnpunkt der Kugel basierend auf der Position des Shooters und Offsets.
		public void SetSpawnPointWithOffset(float offsetX, float offsetY)
		{
			Position = Shooter.Position + new Vector2(offsetX, offsetY);
		}


		#endregion

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Sprite, Position, null, Tint, Rotation, SpriteOffset, Size, 0, 0);
		}
		
		#if DEBUG
		public override void DrawDebug(SpriteBatch spriteBatch)
		{
			// Zeichnet die Kollisionsbox des Entity in Grün
			spriteBatch.DrawRectangle(Bounds, new Color(0, 255, 0, 100));
		}
		#endif
	}
}