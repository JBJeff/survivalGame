using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Screens;
using Silent_Shadow.Models;

namespace Silent_Shadow.Managers 
{
	public static class InputManager
	{
		private static KeyboardState _keyboardState;
		private static MouseState _mouseState;
		private static float rotation;
		private static Vector2 _mousePosition { get { return new Vector2(_mouseState.X, _mouseState.Y); } }

		private static Vector2 _screenCenter { get { return new Vector2(Globals.ScreenWidth / 2, Globals.ScreenHeight / 2);  } }
		

		public static void Update()
		{
			_keyboardState = Keyboard.GetState();
			_mouseState = Mouse.GetState();
			
		}

		public static Vector2 GetMovementDirection()
		{
			Vector2 direction = new Vector2();
			foreach (var key in _keyboardState.GetPressedKeys())
			{
				switch (key)
				{
					case Keys.W:
					case Keys.Up:
						direction.Y -= 1;
						rotation = 4.712389f;
						if (Keyboard.GetState().IsKeyDown(Keys.A))
							rotation = 3.926991f;
						if (Keyboard.GetState().IsKeyDown(Keys.D))
							rotation = 5.497787f;
						break;
					case Keys.A:
					case Keys.Left:
						direction.X -= 3.141593f;
						rotation = 3;
						break;
					case Keys.S:
					case Keys.Down:
						direction.Y += 1;
						rotation = 1.570796f;
						if (Keyboard.GetState().IsKeyDown(Keys.A))
							rotation = 2.356194f;
						if (Keyboard.GetState().IsKeyDown(Keys.D))
							rotation = 0.7853982f;
						break;
					case Keys.D:
					case Keys.Right:
						direction.X += 1;
						rotation = 0;
						break;

				}
			}

			if (direction.LengthSquared() > 1)
			{
				direction.Normalize();
			}

			return direction;
		}

		public static bool IsSneaking()
		{
			return Keyboard.GetState().IsKeyDown(Keys.LeftShift);
		}

		public static Vector2 GetAimDirection()
		{
			Vector2 direction = _mousePosition - _screenCenter;
			if (direction == Vector2.Zero)
			{
				return Vector2.Zero;
			}
			else
			{
				return Vector2.Normalize(direction);
			}
			
		}
		public static float GetRotation()
		{
			return rotation;
		}

		// For Fire
		public static bool IsLeftMouseHeld()
        {
        return _mouseState.LeftButton == ButtonState.Pressed;
        }
		
		//Schleichen 
		public static bool IsKeyDown(Keys key)
		{
			return _keyboardState.IsKeyDown(key);
		}

		//Waffenwechsel durch zahlen
		public static bool IsKeyPressed(Keys key)
        {
        return _keyboardState.IsKeyDown(key) && Keyboard.GetState().IsKeyUp(key);
        }

		public static bool ActiveKey()
		{
			if (GetMovementDirection().X == 0 && GetMovementDirection().Y ==0)
			{
				return false;
			}
			return true;

		}
	}
}
