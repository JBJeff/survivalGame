using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using TiledSharp;
using Silent_Shadow.Models.AI.Navigation;

namespace Silent_Shadow.Models
{
	public class Level()
	{
		// Eigenschaften der Klasse
		public TmxMap Map { get; set; }
		public Dictionary<int, Texture2D> TilesetTextures { get; set; }
		public int TileWidth { get; set; }
		public int TileHeight { get; set; }
		public int TilesPerRow { get; set; }
		public float Scale { get; set; }

#nullable enable
		//zum überprüfen ob eine Kollision mit einem nicht betrettbaren bereich ersteht.
		public List<Rectangle>? Colliders { get; set; }
		public List<Hull>? Hulls { get; set; }

		//Ort wo gespeichert wird
		public List<Rectangle>? CheckPoint { get; set; }

		// Zum verlassen der Map und neuladen einer Map
		public List<Rectangle>? ExitPoints { get; set; }

		public NodeGraph? NodeGraph { get; set; }	
#nullable disable	
	}
}
