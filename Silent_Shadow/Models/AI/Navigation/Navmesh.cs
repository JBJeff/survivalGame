
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Silent_Shadow.Models.AI.Navigation
{
	public class NavMesh(List<(Vector2, Vector2, Vector2)> triangles)
	{
		public List<(Vector2, Vector2, Vector2)> Triangles { get; private set; } = triangles;

		public List<Vector2> GetCentroids()
		{
			List<Vector2> centroids = [];

			foreach (var triangle in Triangles)
			{
				Vector2 centroid = CalculateTriangleCentroid(triangle);
				centroids.Add(centroid);
			}

			return centroids;
		}

		private static Vector2 CalculateTriangleCentroid((Vector2, Vector2, Vector2) triangle)
		{
			return new Vector2(
				(triangle.Item1.X + triangle.Item2.X + triangle.Item3.X) / 3,
				(triangle.Item1.Y + triangle.Item2.Y + triangle.Item3.Y) / 3
			);
		}
	}
}