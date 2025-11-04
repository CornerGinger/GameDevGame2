using GameDevGame1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Text.Json;
using System.IO;
using System.Text;

namespace GameDevGame2
{
	public enum ScreenState
	{
		Title,
		HowToPlay,
		Level1,
		//onToLevel2,
		//Level2,
		//onToLevel3,
		//Level3,
	}
    public class FlySwatters : Game
    {
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private FlySprite[] flies;
		private SuperFlySprite[] superFlies;
		private Swatter swatter;
		private InputManager inputManager;
		private SoundEffect flyHit;
		private SoundEffect swat;
		private Song backgroundMusic;
		private SpriteFont spriteFont;
		private string filePath = "saveData.json";
		private string bestTimeString = "00:00";
		// private int superFliesLeft;
		private int fliesLeft;
		private double elapsedTime;
		private ScreenState curScreen = ScreenState.Title;
		private double instructTimer = 0;
		private KillParticleSystem flyDeath;
		private bool shaking = false;
		private bool newRecord = false;
		private bool lvlComplete = false;
		private float shakeTime;

		private Tilemap _tilemap;

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
			/*
			superFlies = new SuperFlySprite[] {
				new SuperFlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble() * 3.0f,(float) random.NextDouble()) * 3.0f },
				new SuperFlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble() * 3.0f,(float) random.NextDouble()) * 3.0f },
				new SuperFlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble() * 3.0f,(float) random.NextDouble()) * 3.0f },
				new SuperFlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble() * 3.0f,(float) random.NextDouble()) * 3.0f },
			};
			*/

			swatter = new Swatter();
			inputManager = new InputManager();
			fliesLeft = flies.Length;
			//superFliesLeft = superFlies.Length;


			flyDeath = new KillParticleSystem(this, 20);
			Components.Add(flyDeath);

			_tilemap = new Tilemap("map.txt");

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
			LoadBestTime();
			_tilemap.LoadContent(Content);
		}

		protected override void Update(GameTime gameTime)
		{


			// TODO: Add your update logic here
			inputManager.Update(gameTime);
			if (inputManager.Exit) Exit();

			switch (curScreen)
			{
				case ScreenState.Title:
					elapsedTime = 0;
					lvlComplete = false;
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
						curScreen = ScreenState.Level1;
					}
					break;
				case ScreenState.Level1:
					if (fliesLeft != 0)
					{
						elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
					}
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
								flyDeath.PlaceKillParticle(new Vector2(fly.Position.X + 32, fly.Position.Y + 32));
								shakeTime = 0;
								shaking = true;
							}
						}
					}
					if (fliesLeft == 0 & !lvlComplete)
					{
						SaveTimeIfBest(elapsedTime);
						lvlComplete = true;
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
				if (shakeTime > 250) shaking = false;
			}
			shakeTransform *= Matrix.CreateTranslation(23, 23, 0);

			// TODO: Add your drawing code here
			spriteBatch.Begin(transformMatrix: shakeTransform);
			switch (curScreen)
			{
				case ScreenState.Title:
					spriteBatch.DrawString(spriteFont, "Fly Swatters", new Vector2(2, 200), Color.DarkSlateGray);
					spriteBatch.DrawString(spriteFont, "Press space to start", new Vector2(2, 300), Color.DarkSlateGray);
					break;
				case ScreenState.HowToPlay:
					spriteBatch.DrawString(spriteFont, "Click all the flies!", new Vector2(2, 200), Color.DarkSlateGray);
					spriteBatch.DrawString(spriteFont, "To return to the menu, press Space", new Vector2(2, 300), Color.DarkSlateGray);
					break;
				case ScreenState.Level1:
					_tilemap.Draw(gameTime, spriteBatch);
					foreach (var fly in flies)
					{
						fly.Draw(gameTime, spriteBatch);
					}
					if (fliesLeft == 0)
					{
						spriteBatch.DrawString(spriteFont, "YOU WIN!", new Vector2(350, 2), Color.Gold);
						spriteBatch.DrawString(spriteFont, "Press space to play again", new Vector2(350, 70), Color.DarkSlateGray);
						if (newRecord)
						{
							spriteBatch.DrawString(spriteFont, "New record!", new Vector2(10, 150), Color.Red);
						}
					}
					swatter.Draw(gameTime, spriteBatch);
					spriteBatch.DrawString(spriteFont, $"Flies left: {fliesLeft}", new Vector2(10, 5), Color.DarkSlateGray);
					TimeSpan span = TimeSpan.FromSeconds(elapsedTime);
					spriteBatch.DrawString(spriteFont, $"{span.Seconds:D2}:{span.Milliseconds:D2}", new Vector2(10, 50), Color.DarkSlateGray);
					spriteBatch.DrawString(spriteFont, $"Best: {bestTimeString}", new Vector2(10, 100), Color.Gold);
					break;
			}

			spriteBatch.End();
			base.Draw(gameTime);
		}

		private void SaveTimeIfBest(double newTime)
		{
			double bestTime = double.MaxValue;

			// If file exists, read previous best time
			if (File.Exists(filePath))
			{
				string jsonData = File.ReadAllText(filePath);
				using (JsonDocument doc = JsonDocument.Parse(jsonData))
				{
					if (doc.RootElement.TryGetProperty("TimeInSeconds", out JsonElement timeEl))
						bestTime = timeEl.GetDouble();
				}
			}

			// Save if new record or file missing
			if (!File.Exists(filePath) || newTime < bestTime)
			{
				newRecord = true;
				TimeSpan span = TimeSpan.FromSeconds(newTime);
				string formatted = $"{span.Seconds:D2}:{span.Milliseconds:D2}";

				var result = new
				{
					TimeInSeconds = newTime,
					FormattedTime = formatted
				};

				string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
				File.WriteAllText(filePath, json);

				bestTimeString = formatted;
			}
			else
			{
				newRecord = false;
			}
		}

		private void LoadBestTime()
		{
			if (!File.Exists(filePath))
			{
				// Create a new blank file with default data
				var result = new { TimeInSeconds = double.MaxValue, FormattedTime = "00:00" };
				string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
				File.WriteAllText(filePath, json);
				return;
			}

			string jsonData = File.ReadAllText(filePath);
			using (JsonDocument doc = JsonDocument.Parse(jsonData))
			{
				if (doc.RootElement.TryGetProperty("FormattedTime", out JsonElement formatted))
					bestTimeString = formatted.GetString();
			}
		}
	}
}
