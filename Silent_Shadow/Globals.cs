using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Silent_Shadow
{
	/// <summary>
	/// vereinfacht zugriff auf unterschiedliche häufig benutzte Variablen
	/// </summary>
	public static class Globals
	{
		public static Game1 Game { get; set; } = Game1.Instance;

		public static float DeltaTime { get; set; }
		public static ContentManager Content { get; set; }
		public static SpriteBatch SpriteBatch { get; set; }
		public static float TileMapScale { get; set; } = 1.0f;

#if DEBUG
		public static SpriteFont DebugFont { get; set; }

		public const int ScreenWidth = 1920;
		public const int ScreenHeight = 1080;
#else
		public const int ScreenWidth = 1920;
		public const int ScreenHeight = 1080;
#endif

		public static void Update(GameTime gt)
		{
			DeltaTime = (float) gt.ElapsedGameTime.TotalSeconds;
        }
    }
}
