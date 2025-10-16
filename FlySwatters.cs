using GameDevGame1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace GameDevGame2
{
	public enum ScreenState
	{
		Title,
		HowToPlay,
		Game
	}
    public class FlySwatters : Game
    {
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private FlySprite[] flies;
		private Swatter swatter;
		private InputManager inputManager;
		private SoundEffect flyHit;
		private SoundEffect swat;
		private Song backgroundMusic;
		private SpriteFont spriteFont;
		private int fliesLeft;
		private ScreenState curScreen = ScreenState.Title;
		private double instructTimer = 0;
		private KillParticleSystem fireworks;
		private bool shaking = false;
		private float shakeTime; 
		public FlySwatters()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			Random random = new Random();
			flies = new FlySprite[] {
				new FlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble(),(float) random.NextDouble()) },
				new FlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble(),(float) random.NextDouble()) },
				new FlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble(),(float) random.NextDouble()) },
				new FlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble(),(float) random.NextDouble()) },
				new FlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble(),(float) random.NextDouble()) },
				new FlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble(),(float) random.NextDouble()) },
				new FlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble(),(float) random.NextDouble()) },
				new FlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble(),(float) random.NextDouble()) },
			};
			swatter = new Swatter();
			inputManager = new InputManager();
			fliesLeft = flies.Length;


			fireworks = new KillParticleSystem(this, 20);
			Components.Add(fireworks);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			foreach (var fly in flies) fly.LoadContent(Content);
			swatter.LoadContent(Content);
			flyHit = Content.Load<SoundEffect>("Boom3");
			backgroundMusic = Content.Load<Song>("FIGHTING");
			spriteFont = Content.Load<SpriteFont>("AgencyFB");
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Volume = 0.7f;
			MediaPlayer.Play(backgroundMusic);
			// TODO: use this.Content to load your game content here
		}

		protected override void Update(GameTime gameTime)
		{


			// TODO: Add your update logic here
			inputManager.Update(gameTime);
			if (inputManager.Exit) Exit();

			switch (curScreen)
			{
				case ScreenState.Title:
					if (inputManager.Play)
					{
						curScreen = ScreenState.HowToPlay;
						instructTimer = 0;
					}
					break;
				case ScreenState.HowToPlay:
					instructTimer += gameTime.ElapsedGameTime.TotalSeconds;
					if (instructTimer > 2.0)
					{
						curScreen = ScreenState.Game;
					}
					break;
				case ScreenState.Game:
					foreach (var fly in flies) fly.Update(gameTime, graphics);
					swatter.Update(gameTime, inputManager);
					swatter.Color = Color.White;
					if (inputManager.Swat)
					{
						swat = Content.Load<SoundEffect>("Hit6");
						foreach (var fly in flies)
						{
							if (!fly.Dead && fly.Bounds.CollidesWith(swatter.Bounds))
							{
								swatter.Color = Color.Red;
								fly.Dead = true;
								fliesLeft--;
								flyHit.Play();
								fireworks.PlaceFirework(new Vector2(fly.Position.X + 32, fly.Position.Y + 32));
								shakeTime = 0;
								shaking = true;
							}
						}
					}
					if (inputManager.Play)
					{
						curScreen = ScreenState.Title;
						foreach (var fly in flies) fly.Dead = false;
						fliesLeft = flies.Length;
					}
					break;
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Gray);

			Matrix shakeTransform = Matrix.Identity;
			if (shaking)
			{
				shakeTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				shakeTransform = Matrix.CreateTranslation(2 * MathF.Sin(shakeTime), 2 * MathF.Cos(shakeTime), 0);
				if (shakeTime > 500) shaking = false;
			}

			// TODO: Add your drawing code here
			spriteBatch.Begin(transformMatrix: shakeTransform);
			switch (curScreen)
			{
				case ScreenState.Title:
					spriteBatch.DrawString(spriteFont, "This is a Video Game", new Vector2(2, 200), Color.DarkSlateGray);
					spriteBatch.DrawString(spriteFont, "Press space to start", new Vector2(2, 300), Color.DarkSlateGray);
					break;
				case ScreenState.HowToPlay:
					spriteBatch.DrawString(spriteFont, "Click all the flies!", new Vector2(2, 200), Color.DarkSlateGray);
					spriteBatch.DrawString(spriteFont, "To return to the menu, press Space", new Vector2(2, 300), Color.DarkSlateGray);
					break;
				case ScreenState.Game:
					foreach (var fly in flies)
					{
						fly.Draw(gameTime, spriteBatch);
					}
					if (fliesLeft == 0)
					{
						spriteBatch.DrawString(spriteFont, "YOU WIN!", new Vector2(350, 2), Color.Gold);
						spriteBatch.DrawString(spriteFont, "Press space to play again", new Vector2(350, 70), Color.DarkSlateGray);
					}
					swatter.Draw(gameTime, spriteBatch);
					spriteBatch.DrawString(spriteFont, $"Flies left: {fliesLeft}", new Vector2(2, 2), Color.DarkSlateGray);
					break;
			}

			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
