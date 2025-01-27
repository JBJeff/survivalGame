
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace Silent_Shadow.Models.AI.Navigation
{
	public class NodeGraph
	{
		public List<Node> Nodes { get; set; }

		public NodeGraph()
		{
			Nodes = [];
		}

		/// <summary>
		/// Add a node
		/// </summary>
		/// 
		/// <param name="position"></param>
		/// 
		/// <returns></returns>
		public Node AddNode(Vector2 position, NodeType type)
		{
			var node = new Node(position, type);
			Nodes.Add(node);
			return node;
		}

		/// <summary>
		/// Add multiple nodes
		/// </summary>
		/// 
		/// <param name="positions"></param>
		public void AddNodes(IEnumerable<Vector2> positions, NodeType type)
		{
			foreach (var position in positions)
			{
				AddNode(position, type);
			}
		}

		/// <summary>
		/// Check if two triangles share an edge
		/// </summary>
		/// 
		/// <param name="tri1"></param>
		/// <param name="tri2"></param>
		/// 
		/// <returns></returns>
		private static bool TrianglesShareEdge((Vector2, Vector2, Vector2) tri1, (Vector2, Vector2, Vector2) tri2)
		{
			var edges1 = new[]
			{
				(tri1.Item1, tri1.Item2),
				(tri1.Item2, tri1.Item3),
				(tri1.Item3, tri1.Item1)
			};

			var edges2 = new[]
			{
				(tri2.Item1, tri2.Item2),
				(tri2.Item2, tri2.Item3),
				(tri2.Item3, tri2.Item1)
			};

			return edges1.Any(edge1 =>
				edges2.Any(edge2 =>
					(edge1.Item1 == edge2.Item1 && edge1.Item2 == edge2.Item2) ||
					(edge1.Item1 == edge2.Item2 && edge1.Item2 == edge2.Item1)));
		}
		
		/// <summary>
		/// Check if the line between two points intersects any navmesh edges
		/// </summary>
		/// 
		/// <param name="pointA"></param>
		/// <param name="pointB"></param>
		/// <param name="navmeshPolygon"></param>
		/// 
		/// <returns></returns>
		private static bool IsConnectionValid(Vector2 pointA, Vector2 pointB, Polygon navmeshPolygon)
		{
			// Get the list of vertices from the navmesh polygon
			var vertices = navmeshPolygon.Vertices;
			
			// Iterate through all edges of the polygon (consecutive vertices)
			for (int i = 0; i < vertices.Count(); i++)
			{
				// Get the current edge defined by vertices[i] and the next vertex (wrapping around)
				Vector2 segmentStart = vertices[i];
				Vector2 segmentEnd = vertices[(i + 1) % vertices.Count()]; // Wrap around to form a loop

				// Check if the line between pointA and pointB intersects with the current edge
				if (MathHelpers.LineIntersectsLine(pointA, pointB, segmentStart, segmentEnd))
				{
					return false; // If the line intersects any of the polygon edges, the connection is invalid
				}
			}

			return true;
		}

		/// <summary>
		/// Builds the node graph
		/// </summary>
		/// 
		/// <param name="triangles">Navmesh </param>
		/// <param name="bounds"></param>
		/// <param name="connectionRadius"></param>
		public void BuildGraph(List<(Vector2, Vector2, Vector2)> triangles, Polygon bounds, float connectionRadius = 50f)
		{
			#if DEBUG
			System.Text.StringBuilder sb = new();
			Debug.WriteLine($"Building graph with {Nodes.Count} nodes.");
			#endif

			// Clear existing neighbors
			foreach (var node in Nodes)
			{
				node.Neighbors.Clear();
			}

			// Connect nodes based on navmesh constraints
			for (int i = 0; i < Nodes.Count; i++)
			{
				for (int j = i + 1; j < Nodes.Count; j++)
				{
					bool connected = false;

					// Only check triangle edge sharing for Navmesh nodes
					if (Nodes[i].NodeType == NodeType.NavMesh && Nodes[j].NodeType == NodeType.NavMesh)
					{
						// Ensure index is valid for the triangles list
						if (i < triangles.Count && j < triangles.Count && TrianglesShareEdge(triangles[i], triangles[j]))
						{
							connected = true;
						}
					}

					// Otherwise, connect nodes that are close enough, and no obstacles in between
					if (!connected)
					{
						float distance = Vector2.Distance(Nodes[i].Position, Nodes[j].Position);
						if (distance < connectionRadius && IsConnectionValid(Nodes[i].Position, Nodes[j].Position, bounds))
						{
							connected = true;
						}
					}

					// Add the connection if needed
					if (connected)
					{
						Nodes[i].AddNeighbor(Nodes[j]);
						Nodes[j].AddNeighbor(Nodes[i]);
					}
				}
			}

			// Log connections for debugging
			#if DEBUG
			foreach (var node in Nodes)
			{
				sb.AppendFormat("Node at {0}: has {1} neighbors.\n", node.Position, node.Neighbors.Count);
				foreach (var neighbor in node.Neighbors)
				{
					//Debug.WriteLine();
					sb.AppendFormat("  Neighbor at {0}\n", neighbor.Position);
				}
			}

			Debug.WriteLine(sb.ToString());
			#endif
		}
	}
}
