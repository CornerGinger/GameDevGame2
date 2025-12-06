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
		Pause,
		HowToPlay,
		Level1,
		Level2,
		//Level3,
	}
    public class FlySwatters : Game
    {
		private Random random = new Random();
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
		private int superFliesLeft;
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
			
			superFlies = new SuperFlySprite[] {
				new SuperFlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble(),(float) random.NextDouble()) },
				new SuperFlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble(),(float) random.NextDouble()) },
				new SuperFlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble(),(float) random.NextDouble()) },
				new SuperFlySprite(new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64))) { Velocity = new Vector2((float) random.NextDouble(),(float) random.NextDouble()) },
			};
			
			swatter = new Swatter();
			inputManager = new InputManager();
			fliesLeft = flies.Length;
			superFliesLeft = superFlies.Length;


			flyDeath = new KillParticleSystem(this, 20);
			Components.Add(flyDeath);

			_tilemap = new Tilemap("map.txt");

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			foreach (var fly in flies) fly.LoadContent(Content);
			foreach (var sFly in superFlies) sFly.LoadContent(Content);
			swatter.LoadContent(Content);
			flyHit = Content.Load<SoundEffect>("Boom3");
			backgroundMusic = Content.Load<Song>("FIGHTING");
			spriteFont = Content.Load<SpriteFont>("AgencyFB");
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Volume = 0.7f;
			MediaPlayer.Play(backgroundMusic);
			LoadBestTime();
			_tilemap.LoadContent(Content);
		}

		protected override void Update(GameTime gameTime)
		{
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
					if (inputManager.Play)
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
					if (fliesLeft == 0 && !lvlComplete)
					{
						//SaveTimeIfBest(elapsedTime);
						lvlComplete = true;
					}
					else if (lvlComplete && inputManager.Play)
					{
						curScreen = ScreenState.Level2;
						lvlComplete = false;
						elapsedTime = 0;

						ResetLevel2();
					}
					if (inputManager.ToMenu)
					{
						curScreen = ScreenState.Title;
						foreach (var fly in flies) fly.Dead = false;
						fliesLeft = flies.Length;
					}
					break;
				case ScreenState.Level2:
					elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
					foreach (var sFly in superFlies) sFly.Update(gameTime, graphics);
					swatter.Update(gameTime, inputManager);
					swatter.Color = Color.White;

					if (inputManager.Swat)
					{
						swat = Content.Load<SoundEffect>("Hit6");

						foreach (var sFly in superFlies)
						{
							if (!sFly.Dead && sFly.Bounds.CollidesWith(swatter.Bounds))
							{
								swatter.Color = Color.Red;
								sFly.HandleClick();
								flyHit.Play();
								flyDeath.PlaceKillParticle(new Vector2(sFly.Position.X + 32, sFly.Position.Y + 32));
								shakeTime = 0;
								shaking = true;
							}
						}
					}

					bool allDead = true;
					foreach (var sFly in superFlies)
					{
						if (!sFly.Dead)
						{
							allDead = false;
							break;
						}
					}

					if (allDead)
					{
						lvlComplete = true;
						// You could eventually SaveTimeIfBest(elapsedTime) here if Level 2 is timed
					}

					if (inputManager.ToMenu)
					{
						curScreen = ScreenState.Title;
						ResetLevel1();
						ResetLevel2();

						elapsedTime = 0;
						lvlComplete = false;
						shaking = false;
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
					spriteBatch.DrawString(spriteFont, "Click to start", new Vector2(2, 300), Color.DarkSlateGray);
					break;

				case ScreenState.HowToPlay:
					spriteBatch.DrawString(spriteFont, "Click all the flies!", new Vector2(2, 200), Color.DarkSlateGray);
					spriteBatch.DrawString(spriteFont, "Click to play. To return to the menu, press Space", new Vector2(2, 300), Color.DarkSlateGray);
					break;

				case ScreenState.Level1:
					_tilemap.Draw(gameTime, spriteBatch);

					foreach (var fly in flies)
						fly.Draw(gameTime, spriteBatch);

					if (fliesLeft == 0)
					{
						spriteBatch.DrawString(spriteFont, "YOU WIN LEVEL 1!", new Vector2(350, 2), Color.Gold);
						spriteBatch.DrawString(spriteFont, "Click to go to Level 2", new Vector2(350, 70), Color.DarkSlateGray);
					}

					spriteBatch.DrawString(spriteFont, $"Flies left: {fliesLeft}", new Vector2(10, 5), Color.DarkSlateGray);

					TimeSpan span1 = TimeSpan.FromSeconds(elapsedTime);
					spriteBatch.DrawString(spriteFont, $"{span1.Seconds:D2}:{span1.Milliseconds:D2}", new Vector2(10, 50), Color.DarkSlateGray);

					spriteBatch.DrawString(spriteFont, $"Best: {bestTimeString}", new Vector2(10, 100), Color.Gold);
					break;

				case ScreenState.Level2:
					_tilemap.Draw(gameTime, spriteBatch);

					foreach (var sFly in superFlies) sFly.Draw(gameTime, spriteBatch);

					superFliesLeft = 0;
					foreach (var sFly in superFlies)
						if (!sFly.Dead) superFliesLeft++;

					spriteBatch.DrawString(spriteFont, $"Super flies left: {superFliesLeft}", new Vector2(10, 5), Color.DarkSlateGray);

					if (lvlComplete)
					{
						spriteBatch.DrawString(spriteFont, "YOU WIN LEVEL 2!", new Vector2(350, 2), Color.Gold);
						spriteBatch.DrawString(spriteFont, "Press Space to return to menu", new Vector2(350, 70), Color.DarkSlateGray);
					}

					TimeSpan span2 = TimeSpan.FromSeconds(elapsedTime);
					spriteBatch.DrawString(spriteFont, $"{span2.Seconds:D2}:{span2.Milliseconds:D2}", new Vector2(10, 50), Color.DarkSlateGray);
					break;
			}


			spriteBatch.End();

			spriteBatch.Begin(transformMatrix: shakeTransform);
			if (curScreen == ScreenState.Level1 || curScreen == ScreenState.Level2)
			{
				swatter.Draw(gameTime, spriteBatch);
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

		private void ResetLevel1()
		{
			foreach (var fly in flies)
				fly.Dead = false;

			fliesLeft = flies.Length;
		}

		private void ResetLevel2()
		{
			foreach (var sFly in superFlies)
				sFly.Reset();      // uses SuperFlySprite.Reset()

			superFliesLeft = superFlies.Length;
		}

	}
}
