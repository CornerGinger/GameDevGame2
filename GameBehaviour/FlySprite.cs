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
	public class FlySprite
	{
		private Texture2D texture;
		private double animationTimer;
		private short animationFrame;
		private Vector2 velocity;
		private BoundingCircle bounds;

		private const float HitRadius = 18f;
		private static readonly Vector2 HitCenterOffset = new Vector2(32, 32);

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

		public FlySprite(Vector2 position)
		{
			this.Position = position;
			this.bounds = new BoundingCircle(position + HitCenterOffset, HitRadius);
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
			}
			else
			{
				Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (Position.X < graphics.GraphicsDevice.Viewport.X || Position.X > graphics.GraphicsDevice.Viewport.Width - 64)
				{
					velocity.X *= -1;
				}
				if (Position.Y < graphics.GraphicsDevice.Viewport.Y || Position.Y > graphics.GraphicsDevice.Viewport.Height - 64)
				{
					velocity.Y *= -1;
				}
			}
			bounds.Center = Position + HitCenterOffset;
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
			var source = new Rectangle(64 * animationFrame, 0, 64, 64);
			spriteBatch.Draw(texture, Position, source, Color.White);
		}

		public Vector2 FixVelocity(Vector2 vel)
		{
			vel.Normalize();
			vel *= 100;
			return vel;
		}
	}
}
