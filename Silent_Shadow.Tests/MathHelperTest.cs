
using Microsoft.Xna.Framework;

namespace Silent_Shadow.Tests
{

	[TestFixture]
	public class MathHelperTest
	{
		[Test]
		public void TestToAngle()
		{
			Assert.Multiple(() =>
			{
				Assert.That( // Expect 0 radians for (1,0)
					new Vector2(1, 0).ToAngle(),
					Is.EqualTo(0f).Within(1e-5f));

				Assert.That( // Expect PI/2 radians for (0,1)
					new Vector2(0, 1).ToAngle(),
					Is.EqualTo((float) Math.PI / 2).Within(1e-5f));

				Assert.That( // Expect PI radians for (-1,0)
					new Vector2(-1, 0).ToAngle(),
					Is.EqualTo((float)Math.PI).Within(1e-5f));     
			});
		}

		[Test]
		public void TestGetDirectionVector()
		{
			float rotation = (float) Math.PI / 2;
			Vector2 vector;

			Assert.Multiple(() =>
			{
				vector = MathHelpers.GetDirectionVector(rotation, Direction.Forward);
				Assert.That(vector.X, Is.EqualTo(1).Within(1e-5f));
				Assert.That(vector.Y, Is.EqualTo(0).Within(1e-5f));

				vector = MathHelpers.GetDirectionVector(rotation, Direction.Backward);
				Assert.That(vector.X, Is.EqualTo(-1).Within(1e-5f));
				Assert.That(vector.Y, Is.EqualTo(0).Within(1e-5f));
			
				vector = MathHelpers.GetDirectionVector(rotation, Direction.Left);
				Assert.That(vector.X, Is.EqualTo(0).Within(1e-5f));
				Assert.That(vector.Y, Is.EqualTo(1).Within(1e-5f));

				vector = MathHelpers.GetDirectionVector(rotation, Direction.Right);
				Assert.That(vector.X, Is.EqualTo(0).Within(1e-5f));
				Assert.That(vector.Y, Is.EqualTo(-1).Within(1e-5f));
			});
		}

		[Test]
		public void TestGetTriangle()
		{
			Vector2 position = new(0, 0);
			Vector2 direction = new(1, 0); // facing right
			float visionLength = 10f;
			float visionAngle = 45f;

			Vector2[] triangle = MathHelpers.GetTriangle(position, direction, visionLength, visionAngle);
			
			// SMELL: No Proof, so test ain't testing shit
			// NOTE: Should return 2 points, because the trinagle is relative to a point.
			Assert.That(triangle, Has.Length.EqualTo(2));
		}

		[Test]
		public void TestGetHexagon()
		{
			Vector2 origin = new(0, 0);
			Vector2 direction = new(1, 0); // facing right
			float midDistance = 10f;
			float farDistance = 15f;
			float nearWidth = 3f;
			float midWidth = 4f;
			float farWidth = 5f;
			float offset = 2f;

			Vector2[] hexagon = MathHelpers.GetHexagon(origin, direction, midDistance, farDistance, nearWidth, midWidth, farWidth, offset);

			// SMELL: No Proof, so test ain't testing shit
			Assert.That(hexagon, Has.Length.EqualTo(6));// A hexagon should have 6 vertices
		}

		[Test]
		public void TestPointInTriangle()
		{
			Vector2 A = new(0, 0);
			Vector2 B = new(5, 0);
			Vector2 C = new(0, 5);
			Vector2 P = new(2, 2);

			Assert.Multiple(() =>
			{
				Assert.That(MathHelpers.PointInTriangle(A, B, C, P), Is.True);  // Point (2,2) should be inside the triangle
				P = new Vector2(6, 6);
				Assert.That(MathHelpers.PointInTriangle(A, B, C, P), Is.False); // Point (6,6) should be outside the triangle
			});
		}

		[Test]
		public void TestPointInPolygon()
		{
			Vector2 A = new(0, 0);
			Vector2 B = new(5, 0);
			Vector2 C = new(5, 5);
			Vector2 D = new(0, 5);
			Vector2 P = new(2, 2);

			Assert.Multiple(() =>
			{
				Assert.That(MathHelpers.PointInPolygon(A, B, C, D, P), Is.True); // Point (2,2) should be inside the polygon
				P = new Vector2(6, 6);
				Assert.That(MathHelpers.PointInPolygon(A, B, C, D, P), Is.False); // Point (6,6) should be outside the polygon
			});
		}

		[Test]
		public void TestCrossProduct()
		{
			Vector2 A = new(1, 0);
			Vector2 B = new(0, 1);

			Assert.Multiple(() =>
			{
				Assert.That(MathHelpers.CrossProduct(A, B), Is.EqualTo(1f)); // The cross product of (1,0) and (0,1) is 1
				Assert.That(MathHelpers.CrossProduct(B, A), Is.EqualTo(-1f)); // The reverse cross product is -1
			});		
		}
	}
}
