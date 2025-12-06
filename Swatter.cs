using GameDevGame1.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GameDevGame2
{
	public class Swatter
	{
		private Texture2D texture;
		private BoundingRectangle bounds = new BoundingRectangle(new Vector2(200 - 32, 200 - 32), 64, 32);


		/// <summary>
		/// The position of the fly swatter
		/// </summary>
		public Vector2 Position;

		/// <summary>
		/// The color to blend with the swatter
		/// </summary>
		public Color Color { get; set; } = Color.White;

		/// <summary>
		/// The bounding volume of the sprite
		/// </summary>
		public BoundingRectangle Bounds => bounds;

		/// <summary>
		/// Loads the fly swatter texture
		/// </summary>
		/// <param name="content">The ContentManager to load with</param>
		public void LoadContent(ContentManager content)
		{
			texture = content.Load<Texture2D>("swatter");
		}

		/// <summary>
		/// Updates the fly sprite to fly in a pattern
		/// </summary>
		/// <param name="gameTime">The game time</param>
		public void Update(GameTime gameTime, InputManager manager)
		{
			Position = manager.Direction;
			bounds.X = Position.X;
			bounds.Y = Position.Y;
		}

		/// <summary>
		/// Draws the animated sprite
		/// </summary>
		/// <param name="gameTime">The game time</param>
		/// <param name="spriteBatch">The SpriteBatch to draw with</param>
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			var source = new Rectangle(0, 0, 64, 64);
			spriteBatch.Draw(texture, Position, source, Color, 0f, new Vector2(48, 48), 1.5f, SpriteEffects.None, 0);
		}
	}
}
