
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Penumbra;
using TiledSharp;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using Silent_Shadow.Managers.EntityManager;
using Silent_Shadow.Models;
using Silent_Shadow.Models.AI.Agents;
using Silent_Shadow.Models.AI.Navigation;
using Silent_Shadow.Models.AI.Actions;
using Silent_Shadow.Models.AI.Goals;
using Silent_Shadow.Models.Weapons;
using Silent_Shadow.Models.Enviroment;
using System.IO;

namespace Silent_Shadow.Managers.LevelManager
{
	/// <summary>
	/// Implementation of ILevelManager 
	/// </summary>
	/// 
	/// <remarks>
	/// Use LevelManagerFactory to get (Singleton) Instance.
	/// </remarks>
	/// 
	/// <authors>
	/// <author>Jeffer Böttcher</author>
	/// <author>Jonas Schwind</author>
	/// </authors>
	/// 
	/// <version>1.0</version>
	/// 
	/// <seealso cref="ILevelManager"/>
	public class LevelManager : ILevelManager
	{
		private readonly IEntityManager _entityManager = EntityManagerFactory.GetInstance();
		
#if DEBUG
		private readonly List<Polygon> debugMesh = [];
#endif

		#region Singleton
		private static LevelManager _instance;
		public static LevelManager Instance
		{
			get
			{
				_instance ??= new LevelManager();
				return _instance;
			}
		}
		private LevelManager() { }
		#endregion

		#region Loader Utils

		private static float ParseFloat(PropertyDict props, string key)
		{
			if (props.TryGetValue(key, out string value) && float.TryParse(value, out float result))
			{
				return result;
			}

			return 0f;
		}

		private static List<T> ParseList<T>(PropertyDict props, string key) where T : class
		{
			List<T> list = [];
			if (props.TryGetValue(key, out string value))
			{
				value = value.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

				foreach (string entry in value.Split(';', StringSplitOptions.RemoveEmptyEntries))
				{
					if (Activator.CreateInstance(Type.GetType(entry)) is T instance)
					{
						list.Add(instance);
					}
				}
			}
			return list;
		}

		private static List<Rectangle> LoadAsRectangles(TmxMap map, string groupName)
		{
			var rectangles = new List<Rectangle>();
			foreach (TmxObject obj in map.ObjectGroups[groupName].Objects)
			{
				rectangles.Add(new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height));
			}
			return rectangles;
		}

		#endregion

		#region Map loader funtions

		public Level LoadLevel(string level)
		{
#if DEBUG
			debugMesh.Clear();
#endif
			TmxMap map = new($"Content/Maps/{level}.tmx");
			Dictionary<int, Texture2D> tilesetTextures = LoadTilesetTextures(map); // Dictionary für mehrere Tilesets

			Polygon navMeshPolygon = BuildNavMeshPolygon(map, out List<(Vector2, Vector2, Vector2)> triangles);
			NavMesh navMesh = new(triangles);
			NodeGraph graph = BuildNodeGraph(map, navMesh, navMeshPolygon, triangles);

			return new Level {
				Map = map, 
				TilesetTextures = tilesetTextures, 
				TileWidth = map.Tilesets[0].TileWidth,
				TileHeight = map.Tilesets[0].TileHeight,
				TilesPerRow = tilesetTextures.Values.First().Width / map.Tilesets[0].TileWidth,
				Colliders = LoadAsRectangles(map, "Kollision"), // Kollisionserkennung,
				ExitPoints = LoadAsRectangles(map, "ExitPoint"),
				CheckPoint = LoadAsRectangles(map, "CheckPoint"),
				Hulls = LoadHulls(map),
				NodeGraph = graph
			};
		}

		// für eine Texturen die sich im Unterordner befinden ... 
		private static Texture2D LoadTilesetTexture(string tilesetName)
		{
			//WICHTIG: Liste der möglichen Ordner, die Tilesets enthalten können, könnt welche hinzufügen falls ihr mehr weiter daran arbeitet 
			string[] possiblePaths = [
				$"TileSheets/{tilesetName}",
				$"TileSheets/MansionTileSets/{tilesetName}"
			];

			// die Textur wird aus einem der Ordner geladen
			foreach (var path in possiblePaths)
			{
				try
				{
					return Globals.Content.Load<Texture2D>(path);
				}
				catch (ContentLoadException ex)
				{
					//den nächsten Pfad wählen
					Debug.WriteLine($"Konnte Tileset '{path}' nicht laden: {ex.Message}");
				}
			}

			// Falls nichts gefunden 
			throw new FileNotFoundException($"Tileset '{tilesetName}' konnte in den angegebenen Ordnern nicht gefunden werden.");
		}

		private static Dictionary<int, Texture2D> LoadTilesetTextures(TmxMap map)
		{
			var tilesetTextures = new Dictionary<int, Texture2D>();
			foreach (TmxTileset tileset in map.Tilesets)
			{
				Texture2D texture = LoadTilesetTexture(tileset.Name);
				tilesetTextures.Add(tileset.FirstGid, texture);
				Debug.WriteLine($"Versuche, Tileset '{tileset.Name}' zu laden...");
			}
			return tilesetTextures;
		}

		private static List<Hull> LoadHulls(TmxMap map)
		{
			List<Hull> hulls = [];

			foreach (TmxObject obj in map.ObjectGroups["Hulls"].Objects)
			{
				Rectangle rect = new(
					(int)obj.X,
					(int)obj.Y,
					(int)obj.Width,
					(int)obj.Height
				);

				Vector2[] vertices =
				[
					new(rect.Left, rect.Top),
            		new(rect.Right, rect.Top),
           		 	new(rect.Right, rect.Bottom),
            		new(rect.Left, rect.Bottom)
       			];

				Hull hull = new(vertices)
				{
					Scale = Vector2.One,
					Rotation = 0f,
				};

				hulls.Add(hull);
			}

			return hulls;
		}

		#endregion

		#region Level Pathfinding

		private Polygon BuildNavMeshPolygon(TmxMap map, out List<(Vector2, Vector2, Vector2)> triangles)
		{
			Polygon navMeshPolygon = null;
			triangles = [];

			foreach (TmxObject obj in map.ObjectGroups["Navmesh"].Objects)
			{
				List<Vector2> points = obj.Points.Select(point => new Vector2((float)point.X, (float)point.Y)).ToList();

				navMeshPolygon = new Polygon(points);
				navMeshPolygon.Offset(new Vector2((float)obj.X, (float)obj.Y));

				List<Vector2> vertices = navMeshPolygon.Vertices.ToList();
				if (!EarClipper.IsClockwise(vertices))
				{
					vertices.Reverse();
				}

				navMeshPolygon = new Polygon(vertices);
				List<(Vector2, Vector2, Vector2)> polyTriangles = EarClipper.Triangulate(vertices);
				triangles.AddRange(polyTriangles);

#if DEBUG
				debugMesh.Add(navMeshPolygon);
#endif
			}

			return navMeshPolygon;
		}

		private static NodeGraph BuildNodeGraph(TmxMap map, NavMesh navMesh, Polygon navMeshPolygon, List<(Vector2, Vector2, Vector2)> triangles)
		{
			NodeGraph graph = new();
			graph.AddNodes(navMesh.GetCentroids(), NodeType.NavMesh);

			foreach (TmxObject obj in map.ObjectGroups["Waypoints"].Objects)
			{
				Vector2 position = new((float)obj.X, (float)obj.Y);
				NodeType nodeType = obj.Type.ToString() switch
				{
					"node" => NodeType.Node,
					"chokepoint" => NodeType.ChokePoint,
					"cover" => NodeType.Cover,
					_ => throw new InvalidOperationException($"Unknown node type: {obj.Type}")
				};
				graph.AddNode(position, nodeType);
			}

			graph.BuildGraph(triangles, navMeshPolygon, 200f);
			return graph;
		}

		#endregion

		#region Entity Loader Functions

		public void LoadEntitys(Level level)
		{
			IEntityManager entityManager = EntityManagerFactory.GetInstance();

			if (!level.Map.ObjectGroups.TryGetValue("Entitys", out var entityGroup))
			{
				throw new ArgumentException("The level does not contain an 'Entitys' object group.");
			}

			foreach (var obj in entityGroup.Objects)
			{
				Vector2 position = new((float)obj.X, (float)obj.Y);

				switch (obj.Type.ToLower()) // NOTE: 'Type' is 'class' in Tiled editor
				{
					case "player":
						// NOTE: Only one player can exist
						Hero.Initialize(position);
						entityManager.Add(Hero.Instance);
						break;

					// WEAPONS
					case "pistol":
					case "machinegun":
					case "shotgun":
						LoadItem(entityManager, position, obj.Type);
						break;

					// ENEMYS
					case "grunt":
					case "cctv":
					case "boss":
						LoadEnemy(entityManager, position, obj);
						break;

					// Props
					case "powerbox":
					case "armor":
					case "fruitpunch":
					case "fetus":
					case "screen":
					case "greenscreen":
					case "terminal":
					case "pc":
					case "indoor_light":
						LoadPropObject(entityManager, position, obj);
						break;

					default:
						break;
				}
			}
			entityManager.ResetCounters();
		}

		private static void LoadItem(IEntityManager entityManager, Vector2 position, string itemType)
		{
			switch (itemType.ToLower())
			{
				case "pistol":
					entityManager.Add(new Pistol(position));
					break;

				case "machinegun":
					entityManager.Add(new MachineGun(position));
					break;

				case "shotgun":
					entityManager.Add(new Shotgun(position));
					break;

				default:
					throw new ArgumentException($"Unknown item type: {itemType}");
			}
		}

		private static void LoadEnemy(IEntityManager entityManager, Vector2 position, TmxObject obj)
		{
			PropertyDict props = obj.Properties;

			string name = props["Name"];
			float rotation = ParseFloat(props, "Rotation");
			float speed = ParseFloat(props, "Speed");
			float turnSpeed = ParseFloat(props, "TurnSpeed");
			float stoppingDistance = ParseFloat(props, "StoppingDistance");

			List<Goal> goals = ParseList<Goal>(props, "Goals");
			List<GAction> actions = ParseList<GAction>(props, "Actions");

			switch (obj.Type.ToLower())
			{
				case "grunt":
					entityManager.Add(new Grunt(position, name, rotation, speed, turnSpeed, stoppingDistance, goals, actions));
					break;
				case "cctv":
					entityManager.Add(new CctvCam(position, name, rotation, goals, actions));
					break;
				case "boss":
					entityManager.Add(new Boss(position, rotation, speed, stoppingDistance, goals, actions));
					break;

				default:
					throw new ArgumentException($"Unknown enemy type: {obj.Type}");
			}
		}

		private static void LoadPropObject(IEntityManager entityManager, Vector2 position, TmxObject obj)
		{
			switch (obj.Type.ToLower())
			{
				case "powerbox":
					entityManager.Add(new PowerBox(position));
					break;
				case "armor":
					entityManager.Add(new Armor(position));
					break;
				case "fruitpunch":
					entityManager.Add(new FruitPunch(position, 100f));
					break;
				case "fetus":
					entityManager.Add(new Fetus(position, 100f));
					break;
				case "screen":
					entityManager.Add(new Screen(position, 200f));
					break;
				case "greenscreen":
					entityManager.Add(new GreenScreen(position, 200f));
					break;
				case "terminal":
					entityManager.Add(new Terminal(position, 150f));
					break;
				case "pc":
					entityManager.Add(new Pc(position, 100f));
					break;
				case "indoor_light":
					entityManager.Add(new IndoorLight(position, 220f));
					break;

				default:
					throw new ArgumentException($"Unknown type: {obj.Type}");
			}
		}

		#endregion

		

		// Methode zum Bereinigen eines Levels
		//muss überarbeitet werden findet bis jetzt noch KEINE verwendung.
		public void ClearLevel(Level level)
		{
			if (level == null)
			{
				return;
			}

			// Entfernt alle Entitäten
			_entityManager.Clear();

			// Entfernt alle Kollisionsobjekte
			level.Colliders.Clear();
			level.ExitPoints.Clear();

			// Alle speziellen Level-Ressourcen löschen (Texturen, Sounds, etc.)
			level.TilesetTextures.Clear();

			#if DEBUG
			debugMesh.Clear();
			#endif

			//erweitern 
		}

		#region Drawing

		private static void DrawLayer(SpriteBatch spriteBatch, Level level, TmxLayer layer, bool isWallLayer)
		{
			for (var j = 0; j < layer.Tiles.Count; j++)
			{
				int gid = layer.Tiles[j].Gid;
				if (gid != 0)
				{
					var (texture, tilesetRec) = GetTexturedTile(level, gid);

					if (texture != null)
					{
						float x = j % level.Map.Width * level.Map.TileWidth * Globals.TileMapScale;
						float y = j / level.Map.Width * level.Map.TileHeight * Globals.TileMapScale;

						if (isWallLayer)
						{
							DrawShadow(spriteBatch, texture, tilesetRec, x, y, level);
						}

						DrawTile(spriteBatch, texture, tilesetRec, x, y, level);
					}
				}
			}
		}

		private static(Texture2D texture, Rectangle tilesetRec) GetTexturedTile(Level level, int gid)
		{
			foreach (var tileset in level.Map.Tilesets)
			{
				if (gid >= tileset.FirstGid && gid < tileset.FirstGid + tileset.TileCount)
				{
					var texture = level.TilesetTextures[tileset.FirstGid];
					int localTileId = gid - tileset.FirstGid;
					int column = localTileId % (texture.Width / tileset.TileWidth);
					int row = localTileId / (texture.Width / tileset.TileWidth);

					var tilesetRec = new Rectangle(
						column * tileset.TileWidth,
						row * tileset.TileHeight,
						tileset.TileWidth,
						tileset.TileHeight);

					return (texture, tilesetRec);
				}
			}

			return (null, Rectangle.Empty);
		}

		private static void DrawShadow(SpriteBatch spriteBatch, Texture2D texture, Rectangle tilesetRec, float x, float y, Level level)
		{
			Rectangle dest = new(
					(int) (x + 5), // Slight offset for shadow
					(int) (y + 5), // Slight offset for shadow
					(int) (level.TileWidth * Globals.TileMapScale),
					(int) (level.TileHeight * Globals.TileMapScale));

			spriteBatch.Draw(texture, dest, tilesetRec, Color.Black * 0.3f); // Semi-transparent black for shadow
		}

		private static void DrawTile(SpriteBatch spriteBatch, Texture2D texture, Rectangle tilesetRec, float x, float y, Level level)
		{
			Rectangle dest = new(
				(int) x,
				(int) y,
				(int) (level.TileWidth * Globals.TileMapScale),
				(int) (level.TileHeight * Globals.TileMapScale));

			spriteBatch.Draw(texture, dest, tilesetRec, Color.White);
		}

		public void Draw(SpriteBatch spriteBatch, Level level)
		{
			// Identify the "wände" layer dynamically
			int wallLayerIndex = level.Map.TileLayers.Count - 2; // Second layer from the bottom
			string wallLayerName = "Wände";

			for (var i = 0; i < level.Map.TileLayers.Count; i++)
			{
				var layer = level.Map.TileLayers[i];
				bool isWallLayer = i == wallLayerIndex && layer.Name == wallLayerName;

				DrawLayer(spriteBatch, level, layer, isWallLayer);
			}
		}

#if DEBUG
		public void DrawDebug(SpriteBatch spriteBatch, Level level)
		{
			// Zeichnet alle Kollisionsboxen aus dem Level in Rot
			foreach (var rect in level.Colliders)
			{
				spriteBatch.DrawRectangle(rect, Color.Red);
			}

			foreach (var mesh in debugMesh)
			{
				spriteBatch.DrawPolygon(Vector2.Zero, mesh, Color.Cyan, 2f);
			}

			foreach (Node node in level.NodeGraph.Nodes)
			{
				if (node.NodeType == NodeType.Node)
				{
					spriteBatch.DrawCircle(node.Position, 6f, 30, Color.White);
				}
			}
		}
		#endif

		#endregion
	}
}
