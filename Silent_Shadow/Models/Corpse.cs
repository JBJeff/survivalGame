
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Silent_Shadow.Managers.CollisionManager;
using Silent_Shadow.States;

namespace Silent_Shadow.Models
{
	public class Corpse : Entity
	{
		public override Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)(Position.X - (Sprite.Width / 2 * Size)),
                    (int)(Position.Y - (Sprite.Height / 2 * Size)),
                    (int)(Sprite.Width * Size),
                    (int)(Sprite.Height * Size)
                );
            }
        }
		
        // Konstruktor zur Initialisierung der Leiche mit einer Position
        public Corpse(Vector2 position)
        {
            // Lade das Sprite für die Leiche
            Sprite = Globals.Content.Load<Texture2D>("Sprites/Corpse");
            Position = position;
            Size = 0.15f;
			LayerDepth = 0.07f;
			FindPosition();
			Rotation = 0f; // Standardrotation
    	}
		
		public void FindPosition()
		{
			Vector2 newPosition = CollisionManager.FindValidPosition(Position, Bounds, GameState.Instance.Level.Colliders);

			// Überprüfe, ob die Position geändert wurde
			if (newPosition != Position)
			{
				Position = newPosition;  // Nur ändern, wenn die neue Position gültig ist
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			 // Ursprungspunkt ist die Mitte des Sprites
            Vector2 origin = new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);

            // Zeichne das Sprite an der Position, unter Berücksichtigung der Rotation und des Ursprungs
            spriteBatch.Draw(
                Sprite,                             // Die Textur
                Position,                           // Position im Raum (Mittelpunkt der Leiche)
                null,                               // Kein Quellrechteck (volle Textur verwenden)
                Tint,                               // Farbton
                Rotation,                           // Rotation des Sprites
                origin,                             // Ursprungspunkt der Rotation
                Size,                               // Skalierung
                SpriteEffects.None,                 // Keine Spiegelung
                LayerDepth                          // Zeichen-Ebene
            );
        
		}
	}
}