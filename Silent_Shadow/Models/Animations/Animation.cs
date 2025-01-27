
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Silent_Shadow.Models.Animations
{
	public class Animation
	{

		private readonly Texture2D _texture;
		private readonly List<Rectangle> _sourceRectangle = new();
		private readonly int _frames;
		private int _frame;
		private readonly float _frameTime;
		private float _frameTimeLeft;
		private bool _active = true;
		//private Vector2 SpriteOffset;
		private Vector2 origin;

		public  Animation (Texture2D texture,  int framesX, int framesY, float frameTime, int row = 1)
		{
			_texture = texture;
			_frames = framesX;
			_frameTime = frameTime;
			_frameTimeLeft = frameTime;
			var frameWidth = _texture.Width/framesX;
			var frameHeight  = _texture.Height/framesY;

			//SpriteOffset = spriteOffset;

			for (int i = 0; i < _frames; i++)
			{
				_sourceRectangle.Add(new(i * frameWidth, (row - 1)* frameHeight, frameWidth, frameHeight));
			}

		}

		public void Start()
		{
			_active = true;
		}

		public void Stop()
		{
			_active = false;
		}

		public void Reset()
		{
			_frame = 0;
			_frameTimeLeft = _frameTime;
		}

		public void Update()
		{
			if (!_active) return;

			_frameTimeLeft -= Globals.DeltaTime;

			if (_frameTimeLeft < 0)
			{
				_frameTimeLeft += _frameTime;
				_frame = (_frame + 1) % _frames;
			}

			origin = new Vector2 (_sourceRectangle[_frame].Width / 2, _sourceRectangle[_frame].Height / 2);
			//origin = new Vector2 (_sourceRectangle[_frame].Width / 2 + SpriteOffset.X , _sourceRectangle[_frame].Height / 2 + SpriteOffset.Y);
		}

		public void Draw(Vector2 pos, float rotation,float size, SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(_texture, pos, _sourceRectangle[_frame], Color.White, rotation, origin, size, 0, 0);
		}
	}
}
