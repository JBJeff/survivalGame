
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Silent_Shadow.Models.AI.Navigation
{
	public enum NodeType
	{
		/// <summary>
		/// A AI Node 
		/// </summary>
		Node,

		/// <summary>
		/// A Navmesh generated AI Node
		/// </summary>
		NavMesh,

		/// <summary>
		/// A ChokePoint
		/// </summary>
		ChokePoint,

		/// <summary>
		/// A posible cover position
		/// </summary>
		Cover,

		/// <summary>
		/// A node on a Moving target
		/// </summary>
		MovingNode
	}

	public class Node(Vector2 position, NodeType type)
	{
		public Vector2 Position { get; set; } = position;
		public List<Node> Neighbors { get; } = [];
		public float Cost { get; } = type switch
		{
			NodeType.ChokePoint => 1f,
			_ => 5f,
		};
		public NodeType NodeType { get; } = type;

		public void AddNeighbor(Node neighbor)
		{
			if (!Neighbors.Contains(neighbor))
			{
				Neighbors.Add(neighbor);
			}
		}
	}
}
