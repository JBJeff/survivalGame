
using Microsoft.Xna.Framework;
using Penumbra;

namespace Silent_Shadow.Models.Enviroment
{
	public abstract class LightSource : Entity
	{
		private readonly PenumbraComponent penumbra = Globals.Game.Penumbra;
		private PointLight light;

		public bool Toogleable { get; set; } = true;
		public bool Glowing { get; private set; } = true;
		public float Radius { get; set;}

		protected LightSource(Vector2 position, float radius, Color color)
		{
			Position = position;
			Radius = radius;

			light = new()
			{
				Position = position,
				Scale = new Vector2(radius),
				Color = color,
				Intensity = 1f,
				CastsShadows = true,
				ShadowType = ShadowType.Occluded
			};

			penumbra.Lights.Add(light);
		}

		public void Toggle()
		{
			if (!Toogleable)
			{
				return;
			}

			if (Glowing)
			{
				light.Enabled = false;
				Glowing = false;
			}
			else
			{
				light.Enabled = true;
				Glowing = true;
			}
		}
	}
}