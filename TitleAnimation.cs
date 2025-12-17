using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevGame2
{
	public class TitleAnimation
	{
		private Texture2D _texture;
		private double _timer;
		private int _frame;

		private readonly int _frameWidth;
		private readonly int _frameHeight;
		private readonly int _frameCount;
		private readonly double _secondsPerFrame = 1.2;
		private readonly string _assetName = "flytitleanim";

		public TitleAnimation(int frameWidth, int frameHeight, int frameCount)
		{
			_frameWidth = frameWidth;
			_frameHeight = frameHeight;
			_frameCount = frameCount;
		}

		public void LoadContent(ContentManager content)
		{
			_texture = content.Load<Texture2D>(_assetName);
		}

		public void Update(GameTime gameTime)
		{
			_timer += gameTime.ElapsedGameTime.TotalSeconds;

			if (_timer >= _secondsPerFrame)
			{
				_frame = (_frame + 1) % _frameCount;
				_timer -= _secondsPerFrame;
			}
		}

		public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
		{
			var vp = graphicsDevice.Viewport;

			Rectangle source = new Rectangle(0, _frame * _frameHeight, _frameWidth, _frameHeight);
			Rectangle dest = new Rectangle(0, 0, vp.Width, vp.Height);

			spriteBatch.Draw(_texture, dest, source, Color.White);
		}
	}
}
