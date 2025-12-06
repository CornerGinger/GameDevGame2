using GameDevGame1.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDevGame2
{
	/// <summary>
	/// A class representing the bat sprite
	/// </summary>
	public class SuperFlySprite
	{
		private Texture2D texture;
		private double animationTimer;
		private short animationFrame;
		private Vector2 velocity;
		private int deadOrAlive = 0;
		private BoundingCircle bounds;
		private Color tint = Color.White;

		private int clickCount = 0;
		private float speedMultiplier = 1f;

		public Vector2 Position { get; private set; }
		public bool Dead { get; set; } = false;
		/// <summary>
		/// The bounding volume of the sprite
		/// </summary>
		public BoundingCircle Bounds => bounds;

		/// <summary>
		/// The velocity of the fly
		/// </summary>
		public Vector2 Velocity
		{
			get => velocity;
			set
			{
				velocity = FixVelocity(value);
			}
		}

		public SuperFlySprite(Vector2 position)
		{
			this.Position = position;
			this.bounds = new BoundingCircle(position - new Vector2(-32 * 1.5f, -32 * 1.5f), 32 * 1.5f);
		}

		/// <summary>
		/// Loads the fly sprite texture
		/// </summary>
		/// <param name="content">The ContentManager to load with</param>
		public void LoadContent(ContentManager content)
		{
			texture = content.Load<Texture2D>("32x32-flysprite");
		}

		/// <summary>
		/// Updates the fly sprite to fly in a pattern
		/// </summary>
		/// <param name="gameTime">The game time</param>
		public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
		{
			if (Dead)
			{
				return;
				//Velocity = new Vector2(0, 0);
				//deadOrAlive = 1;
			}
			else
			{
				Position += Velocity * speedMultiplier * (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (Position.X < graphics.GraphicsDevice.Viewport.X || Position.X > graphics.GraphicsDevice.Viewport.Width - 64)
				{
					velocity.X *= -1;
				}
				if (Position.Y < graphics.GraphicsDevice.Viewport.Y || Position.Y > graphics.GraphicsDevice.Viewport.Height - 64)
				{
					velocity.Y *= -1;
				}
				deadOrAlive = 0;
			}
			bounds.Center = Position - new Vector2(-48, -48);
		}

		/// <summary>
		/// Draws the animated sprite
		/// </summary>
		/// <param name="gameTime">The game time</param>
		/// <param name="spriteBatch">The SpriteBatch to draw with</param>
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Update animation timer
			animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

			// Update animation frame
			if (Dead)
			{
				return;
			}
			if (animationTimer > 0.1)
			{
				animationFrame++;
				if (animationFrame > 3) animationFrame = 0;
				animationTimer -= 0.3;
			}
			var source = new Rectangle(64 * animationFrame, 64 * deadOrAlive, 64, 64);
			spriteBatch.Draw(texture, Position, source, tint, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0f);
		}

		public Vector2 FixVelocity(Vector2 vel)
		{
			vel.Normalize();
			vel *= 100;
			return vel;
		}

		public void HandleClick()
		{
			if (Dead) return;

			clickCount++;

			if (clickCount == 1)
			{
				speedMultiplier = 3f;
				tint = Color.Red;
				deadOrAlive = 0;
			}
			else if (clickCount >= 2)
			{
				Dead = true;
				tint = Color.White;
				deadOrAlive = 1;
			}
		}

		public void Reset()
		{
			Dead = false;
			clickCount = 0;
			speedMultiplier = 1f;
			tint = Color.White;
		}
	}
}
