using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameDevGame2
{
	public class BossFlySprite
	{
		private Texture2D _texture;
		private double _animationTimer;
		private short _animationFrame;
		private Vector2 _velocity;
		private BoundingCircle _bounds;

		private const int FrameSize = 64;
		private const float Scale = 3.5f;

		private static readonly Vector2 HitCenterOffset = new Vector2((FrameSize / 2f) * Scale, (FrameSize / 2f) * Scale);
		private const float HitRadius = 18f * Scale;

		private int _maxHp = 20;
		private int _hp;

		private int _hitsSinceRamp = 0;
		private int _nextRampAt = 5;

		private float _speedMultiplier = 2f;
		private int _spawnCount = 4;

		public Vector2 Position { get; private set; }
		public bool Dead => _hp <= 0;

		public int MaxHp => _maxHp;
		public int Hp => _hp;

		public BoundingCircle Bounds => _bounds;

		public BossFlySprite(Vector2 startPos)
		{
			Position = startPos;

			_velocity = new Vector2(120f, 90f);
			_hp = _maxHp;
			_bounds = new BoundingCircle(Position + HitCenterOffset, HitRadius);
		}

		public void LoadContent(ContentManager content)
		{
			_texture = content.Load<Texture2D>("32x32-flysprite");
		}

		public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
		{
			if (Dead) return;

			float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
			Position += _velocity * dt;

			float bossSize = FrameSize * Scale;

			// bounce using the scaled size
			if (Position.X < graphics.GraphicsDevice.Viewport.X || Position.X > graphics.GraphicsDevice.Viewport.Width - bossSize)
				_velocity.X *= -1;

			if (Position.Y < graphics.GraphicsDevice.Viewport.Y || Position.Y > graphics.GraphicsDevice.Viewport.Height - bossSize)
				_velocity.Y *= -1;

			_bounds.Center = Position + HitCenterOffset;
		}

		public bool RegisterHit(out int spawnHowMany)
		{
			spawnHowMany = 0;
			if (Dead) return false;

			_hp--;
			_hitsSinceRamp++;

			if (_hitsSinceRamp >= _nextRampAt && !Dead)
			{
				_hitsSinceRamp = 0;
				_velocity *= _speedMultiplier;
				spawnHowMany = _spawnCount;
				return true;
			}
			return false;
		}

		public void Reset()
		{
			_hp = _maxHp;
			_hitsSinceRamp = 0;
			_velocity = new Vector2(120f, 90f);
			_bounds.Center = Position + HitCenterOffset;
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (Dead) return;

			// animate like FlySprite
			_animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
			if (_animationTimer > 0.1)
			{
				_animationFrame++;
				if (_animationFrame > 3) _animationFrame = 0;
				_animationTimer -= 0.3;
			}

			var source = new Rectangle(FrameSize * _animationFrame, 0, FrameSize, FrameSize);
			spriteBatch.Draw(_texture, Position, source, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
		}
	}
}
