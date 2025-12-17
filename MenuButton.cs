using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDevGame2
{
	public class MenuButton
	{
		private string _assetName = "playbuttontitle";

		private Texture2D _texture;

		public Vector2 Position;

		public float Scale { get; set; } = 3f;

		private int _frameWidth;
		private int _frameHeight;

		// 0 = normal (top), 1 = hover (bottom)
		private int _currentFrame = 0;

		private MouseState _prevMouse;

		public bool IsHovering { get; private set; }
		public bool WasClicked { get; private set; }

		public BoundingRectangle Bounds { get; private set; }

		public MenuButton(Vector2 position)
		{
			Position = position;
		}

		public void LoadContent(ContentManager content)
		{
			_texture = content.Load<Texture2D>(_assetName);

			// Two vertical frames
			_frameWidth = _texture.Width;
			_frameHeight = _texture.Height / 2;

			Bounds = new BoundingRectangle(Position, _frameWidth * Scale, _frameHeight * Scale);
		}

		public void Update(GameTime gameTime)
		{
			WasClicked = false;

			var mouse = Mouse.GetState();
			var mouseRect = new BoundingRectangle(mouse.X, mouse.Y, 1, 1);

			// keep bounds in sync if Position changes
			Bounds = new BoundingRectangle(Position, _frameWidth * Scale, _frameHeight * Scale);

			IsHovering = Bounds.CollidesWith(mouseRect);
			_currentFrame = IsHovering ? 1 : 0;

			bool leftJustPressed =
				mouse.LeftButton == ButtonState.Pressed &&
				_prevMouse.LeftButton == ButtonState.Released;

			if (IsHovering && leftJustPressed)
				WasClicked = true;

			_prevMouse = mouse;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			Rectangle src = new Rectangle(
				0,
				_currentFrame * _frameHeight,
				_frameWidth,
				_frameHeight
			);

			spriteBatch.Draw(_texture, Position, src, Color.White, 0f, Vector2.Zero, Scale,	SpriteEffects.None,	0f);
		}
	}
}
