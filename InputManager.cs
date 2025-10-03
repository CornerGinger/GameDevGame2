using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameDevGame2
{
	public class InputManager
	{
		KeyboardState currentKeyboardState;
		KeyboardState priorKeyboardState;
		MouseState currentMouseState;
		MouseState priorMouseState;

		/// <summary>
		/// The current direction
		/// </summary>
		public Vector2 Direction { get; private set; }

		/// <summary>
		/// If the swat functionality has been requested
		/// </summary>
		public bool Swat { get; private set; } = false;

		/// <summary>
		/// If the play functionality has been requested
		/// </summary>
		public bool Play { get; private set; } = false;

		/// <summary>
		/// If the user has requested the game end 
		/// </summary>
		public bool Exit { get; private set; } = false;

		public void Update(GameTime gameTime)
		{
			#region Updating Input State
			priorKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState();

			priorMouseState = currentMouseState;
			currentMouseState = Mouse.GetState();

			#endregion

			#region Direction Input

			//Get position from Keyboard
			if (currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A))
			{
				Direction += new Vector2(-100 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
			}
			if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D))
			{
				Direction += new Vector2(100 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
			}
			if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W))
			{
				Direction += new Vector2(0, -100 * (float)gameTime.ElapsedGameTime.TotalSeconds);
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S))
			{
				Direction += new Vector2(0, 100 * (float)gameTime.ElapsedGameTime.TotalSeconds);
			}

			//Get position from mouse
			Direction = new Vector2(currentMouseState.X, currentMouseState.Y);
			#endregion

			#region Swat Input
			Swat = false;
			if (currentMouseState.LeftButton == ButtonState.Pressed &&
				priorMouseState.LeftButton == ButtonState.Released)
			{
				Swat = true;
			}

			#endregion

			#region Play Input
			Play = false;
			if (currentKeyboardState.IsKeyDown(Keys.Space) &&
				priorKeyboardState.IsKeyUp(Keys.Space))
			{
				Play = true;
			}

			#endregion

			#region Exit Input


			if (currentKeyboardState.IsKeyDown(Keys.Escape))
			{
				Exit = true;
			}
			#endregion
		}
	}
}
