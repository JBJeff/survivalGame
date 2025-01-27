using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Silent_Shadow.Models
{
	public class Blood : Entity
	{
        // Konstruktor zur Initialisierung der Leiche mit einer Position
        public Blood(Vector2 position)
        {
            // Lade das Sprite für die Leiche
            Sprite = Globals.Content.Load<Texture2D>("Sprites/blood_red");
            Position = position;
            Size = 1f;
			LayerDepth = 0f;
    	}
	}
}