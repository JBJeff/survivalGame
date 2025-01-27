using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Silent_Shadow.Models.AI.Navigation
{
	static class EarClipper
	{
		// Main triangulation function
		public static List<(Vector2, Vector2, Vector2)> Triangulate(List<Vector2> polygon)
		{
			List<(Vector2, Vector2, Vector2)> triangles = [];

			if (polygon.Count < 3) return triangles;
			List<Vector2> remainingPoints = [.. polygon];

			if (!IsClockwise(remainingPoints))
			{
				remainingPoints.Reverse();
			}

			while (remainingPoints.Count >= 3)
			{
				bool earFound = false;

				for (int i = 0; i < remainingPoints.Count; i++)
				{
					int prevIndex = (i - 1 + remainingPoints.Count) % remainingPoints.Count;
					int nextIndex = (i + 1) % remainingPoints.Count;

					Vector2 prev = remainingPoints[prevIndex];
					Vector2 current = remainingPoints[i];
					Vector2 next = remainingPoints[nextIndex];

					if (IsEar(prev, current, next, remainingPoints))
					{
						triangles.Add((prev, current, next));
						remainingPoints.RemoveAt(i);
						earFound = true;
						break;
					}
				}

				if (!earFound)
				{
					break;
				}
			}

			return triangles;
		}

		// Helper methods
		private static bool IsEar(Vector2 prev, Vector2 current, Vector2 next, List<Vector2> polygon)
		{
			if (MathHelpers.CrossProduct(prev, current, next) <= 0)
			{
				return false;
			}

			foreach (var point in polygon)
			{
				if (point != prev && point != current && point != next && MathHelpers.PointInTriangle(current,prev,next,point))
				{
					return false;
				}
			}

			return true;
		}

		public static bool IsClockwise(List<Vector2> polygon)
		{
			float area = 0;
			for (int i = 0; i < polygon.Count; i++)
			{
				int j = (i + 1) % polygon.Count;
				area += MathHelpers.CrossProduct(polygon[i], polygon[j]);
			}
			return area > 0;
		}

		public static List<Vector2> RemoveCollinearPoints(List<Vector2> vertices)
		{
			List<Vector2> result = [];
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 prev = vertices[(i - 1 + vertices.Count) % vertices.Count];
				Vector2 current = vertices[i];
				Vector2 next = vertices[(i + 1) % vertices.Count];

				if (Math.Abs(MathHelpers.CrossProduct(prev, current, next)) > float.Epsilon)
				{
					result.Add(current);
				}
			}
			return result;
		}
	}
}
