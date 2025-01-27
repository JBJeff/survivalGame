using System;
using Microsoft.Xna.Framework;

namespace Silent_Shadow
{
	public enum Direction
	{
		Forward,
		Backward,
		Left,
		Right
	}

	public static class MathHelpers
	{

		/// <summary>
		/// converts radians to degrees
		/// </sumary>
		public static float ToAngle(this Vector2 vector)
		{
			return (float) Math.Atan2(vector.Y, vector.X);
		}

		/// <summary>
		/// Calculates a direction vector based on orientation.
		/// </summary>
		///
		/// <param name="rotation">Rotation</param>
		/// <param name="direction">Relative direction (forward, Backward, left, right)</param>
		///
		/// <returns>Dirrection Vector</returns>
		public static Vector2 GetDirectionVector(float rotation, Direction direction)
		{
			return direction switch
			{
				Direction.Forward => new Vector2(
					(float) Math.Cos(rotation - Math.PI / 2),
					(float) Math.Sin(rotation - Math.PI / 2)
				),
				Direction.Backward => new Vector2(
					(float) Math.Cos(rotation + Math.PI / 2),
					(float) Math.Sin(rotation + Math.PI / 2)
				),
				Direction.Left => new Vector2(
					(float) Math.Cos(rotation),
					(float) Math.Sin(rotation)
				),
				Direction.Right => new Vector2(
					(float) Math.Cos(rotation + Math.PI),
					(float) Math.Sin(rotation + Math.PI)
				),
				_ => Vector2.Zero,
			};
		}

		/// <summary>
		/// Defines a triangular region, representing the "vision cone" of the agent.
		/// </summary>
		///
		/// <param name="position">Agents positon</param>
		/// <param name="direction">The direction the cone should face</param>
		/// <param name="visionLength">The range of the cone</param>
		/// <param name="visionAngle">How wide the cone should be</param>
		///
		/// <returns>A triangle</returns>
		public static Vector2[] GetTriangle(Vector2 position, Vector2 direction, float visionLength, float visionAngle)
		{
			float halfAngle = MathHelper.ToRadians(visionAngle / 2);

			Vector2 leftDir = Vector2.Transform(direction, Matrix.CreateRotationZ(-halfAngle));
			Vector2 rightDir = Vector2.Transform(direction, Matrix.CreateRotationZ(halfAngle));

			Vector2 leftPoint = position + leftDir * visionLength;
			Vector2 rightPoint = position + rightDir * visionLength;

			return [leftPoint, rightPoint];
		}

		/// <summary>
		/// Calculates a hexagon-shaped area.
		/// This shape can be used to define a vision or detection zone.
		/// </summary>
		///
		/// <param name="direction">The forward direction of the shape. Must be normalized.</param>
		/// <param name="midDistance">The distance from the origin to the midpoint of the hexagon.</param>
		/// <param name="farDistance">The distance from the midpoint to the far edge of the hexagon.</param>
		/// <param name="nearWidth">The width of the hexagon's near edge.</param>
		/// <param name="midWidth">The width of the hexagon's midpoint edge.</param>
		/// <param name="farWidth">The width of the hexagon's far edge.</param>
		/// <param name="offset">The offset from the origin along the direction vector.</param>
		///
		/// <returns>Hexagon vertices</returns>
		public static Vector2[] GetHexagon(
			Vector2 origin,
			Vector2 direction,
			float midDistance,
			float farDistance,
			float nearWidth,
			float midWidth,
			float farWidth,
			float offset)
		{
			direction = Vector2.Normalize(direction);

			Vector2 offsetPosition = origin + direction * offset;

			float nearHalfWidth = nearWidth / 2;
			float midHalfWidth = midWidth / 2;
			float farHalfWidth = farWidth / 2;

			Vector2 perpendicular = new Vector2(-direction.Y, direction.X);

			// Near edge vertices
			Vector2 leftNear = offsetPosition + perpendicular * -nearHalfWidth;
			Vector2 rightNear = offsetPosition + perpendicular * nearHalfWidth;

			// Midpoint edge vertices
			Vector2 midPointCenter = offsetPosition + direction * midDistance;
			Vector2 leftMid = midPointCenter + perpendicular * -midHalfWidth;
			Vector2 rightMid = midPointCenter + perpendicular * midHalfWidth;

			// Far edge vertices
			Vector2 farPointCenter = midPointCenter + direction * farDistance;
			Vector2 leftFar = farPointCenter + perpendicular * -farHalfWidth;
			Vector2 rightFar = farPointCenter + perpendicular * farHalfWidth;

			return [
				leftNear, rightNear,
				leftMid, rightMid,
				leftFar, rightFar
			];
		}

		/// <summary>
		/// Determines if a given point lies within a triangular area.
		/// </summary>
		///
		/// <param name="v1">The first vertex of the triangle.</param>
		/// <param name="v2">The second vertex of the triangle.</param>
		/// <param name="v3">The third vertex of the triangle.</param>
		/// <param name="point">The point to check.</param>
		/// <param name="tolerance">Tolerance</param>
		///
		/// <returns>True if the point P is within the triangle; otherwise, false.</returns>
		public static bool PointInTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 point, float tolerance = 1e-6f)
		{
			Vector2 AB = v2 - v1;
			Vector2 AC = v3 - v1;
			Vector2 AP = point - v1;

			// dot products
			float dotABAB = Vector2.Dot(AB, AB);
			float dotABAC = Vector2.Dot(AB, AC);
			float dotABAP = Vector2.Dot(AB, AP);
			float dotACAC = Vector2.Dot(AC, AC);
			float dotACAP = Vector2.Dot(AC, AP);

			float invDenom = 1 / (dotABAB * dotACAC - dotABAC * dotABAC);

			// barycentric coordinates
			float u = (dotACAC * dotABAP - dotABAC * dotACAP) * invDenom;
			float v = (dotABAB * dotACAP - dotABAC * dotABAP) * invDenom;

			return (u >= -tolerance) && (v >= -tolerance) && (u + v <= 1 + tolerance);
		}

		/// <summary>
		/// Determines if a given point lies within a 4 sided Polygon.
		/// </summary>
		///
		/// <param name="a">The first vertex of the polygon.</param>
		/// <param name="b">The second vertex of the polygon.</param>
		/// <param name="c">The third vertex of the polygon.</param>
		/// <param name="d">The fourth vertex of the polygon.</param>
		/// <param name="p">The point to check.</param>
		///
		/// <returns>True if the point P is within the polygon; otherwise, false.</returns>
		public static bool PointInPolygon(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Vector2 p)
		{
			// HACK: Redundant calculations avoids a lot of false positives/negatives
			return PointInTriangle(a, b, c, p) || PointInTriangle(a, c, d, p) ||
			   PointInTriangle(a, b, d, p) || PointInTriangle(b, c, d, p);
		}

		/* Bestimmt ob sich etwas in einen vorgegebenen Kreis befindet.
		 * 
		 * 
		 * Parameter "xc" ist die X-Position des Mittelpunktes von dem Kreis
		 * Parameter "yc" ist die Y-Position des Mittelpunktes von dem Kreis
		 * 
		 * Parameter "xp" ist die X-Position des Objects welches bestimmt werden soll ob es im Kreis ist
		 * Parameter "yp" ist die Y-Position des Objects welches bestimmt werden soll ob es im Kreis ist
		 * 
		*/
		public static bool PointInCircle(float xc, float yc, float xp, float yp, float radiant)
		{
			if( Math.Sqrt(Math.Pow((xp - xc),2) + Math.Pow((yp - yc),2)) < radiant) return true;
			else return false;
		}

		/// <summary>
		/// Calculates the Crossproducts of 2 Vectors
		/// </summary>
		///
		/// <param name="a">Vector A</param>
		/// <param name="b">Vector B</param>
		///
		/// <returns>The Crossproduct of the vectors</returns>
		public static float CrossProduct(Vector2 a, Vector2 b)
		{
			return a.X * b.Y - a.Y * b.X;
		}

		/// <summary>
		/// Calculates the Crossproducts of § Vectors
		/// </summary>
		///
		/// <param name="a">Vector A</param>
		/// <param name="b">Vector B</param>
		/// <param name="c">Vector C</param>
		///
		/// <returns>The Crossproduct of the vectors</returns>
		public static float CrossProduct(Vector2 a, Vector2 b, Vector2 c)
		{
			return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
		}

		/// <summary>
		/// Calculates if two lines cross
		/// </summary>
		///
		/// <param name="p1">point 1 of line 1</param>
		/// <param name="p2">point 2 of line 1</param>
		/// <param name="q1">point 1 of line 2</param>
		/// <param name="q2">point 2 of line 2</param>
		///
		/// <returns>True if the lines cross, otherwise false</returns>
		public static bool LineIntersectsLine(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
		{
			Vector2 r = p2 - p1;
			Vector2 s = q2 - q1;

			float denominator = r.X * s.Y - r.Y * s.X;

			if (Math.Abs(denominator) < float.Epsilon)
			{
				return false;
			}

			Vector2 qp = q1 - p1;

			float t = (qp.X * s.Y - qp.Y * s.X) / denominator;
			float u = (qp.X * r.Y - qp.Y * r.X) / denominator;

			return t >= 0 && t <= 1 && u >= 0 && u <= 1;
		}

	}
}