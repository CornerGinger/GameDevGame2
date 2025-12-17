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
		Level3,
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

		private double runTimeSeconds = 0;
		private bool timerRunning = false;
		private double finalRunTimeSeconds = 0;
		private double bestTimeSeconds = double.MaxValue;

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
		private bool prevPlay = false;

		private Tilemap _tilemap;
		private TitleAnimation _titleAnim;
		private MenuButton _startButton;

		private BossFlySprite boss;
		private FlySprite[] level3Minions;
		private int level3MinionsAlive;
		private Texture2D _hpPixel;

		// debug stuff
		private Texture2D _debugPixel;
		private bool _showColliders = false; // toggle on/off
		private KeyboardState _prevKb;



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

			boss = new BossFlySprite(new Vector2(300, 200));
			level3Minions = new FlySprite[20];   // capacity
			level3MinionsAlive = 0;

			swatter = new Swatter();
			inputManager = new InputManager();
			fliesLeft = flies.Length;
			superFliesLeft = superFlies.Length;


			flyDeath = new KillParticleSystem(this, 20);
			Components.Add(flyDeath);

			_tilemap = new Tilemap("map.txt");
			_titleAnim = new TitleAnimation(630, 500, 3);
			_startButton = new MenuButton(new Vector2(150, 350));
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// debug stuff
			_debugPixel = new Texture2D(GraphicsDevice, 1, 1);
			_debugPixel.SetData(new[] { Color.White });

			foreach (var fly in flies) fly.LoadContent(Content);
			foreach (var sFly in superFlies) sFly.LoadContent(Content);
			boss.LoadContent(Content);
			_hpPixel = new Texture2D(GraphicsDevice, 1, 1);
			_hpPixel.SetData(new[] { Color.White });

			swatter.LoadContent(Content);
			flyHit = Content.Load<SoundEffect>("Boom3");
			backgroundMusic = Content.Load<Song>("FIGHTING");
			spriteFont = Content.Load<SpriteFont>("AgencyFB");
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Volume = 0.7f;
			MediaPlayer.Play(backgroundMusic);
			LoadBestTime();
			_tilemap.LoadContent(Content);
			_titleAnim.LoadContent(Content);
			_startButton.LoadContent(Content);
		}

		protected override void Update(GameTime gameTime)
		{
			inputManager.Update(gameTime);

			// prevents accidentally skipping menuing
			bool playJustPressed = inputManager.Play && !prevPlay;
			prevPlay = inputManager.Play;

			// Debug key to show collision
			var kb = Keyboard.GetState();
			if (kb.IsKeyDown(Keys.O) && _prevKb.IsKeyUp(Keys.O))
				_showColliders = !_showColliders;
			_prevKb = kb;

			if (inputManager.Exit) Exit();
			switch (curScreen)
			{
				case ScreenState.Title:
					elapsedTime = 0;
					lvlComplete = false;
					_titleAnim.Update(gameTime);
					_startButton.Update(gameTime);
					if (_startButton.WasClicked)
					{
						curScreen = ScreenState.HowToPlay;
					}
					break;
				case ScreenState.HowToPlay:
					instructTimer += gameTime.ElapsedGameTime.TotalSeconds;

					if (playJustPressed)
					{
						curScreen = ScreenState.Level1;

						runTimeSeconds = 0;
						finalRunTimeSeconds = 0;
						timerRunning = true;

						lvlComplete = false;
					}
					break;
				case ScreenState.Level1:
					if (timerRunning && !lvlComplete && fliesLeft != 0)
					{
						runTimeSeconds += gameTime.ElapsedGameTime.TotalSeconds;
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
					if (timerRunning && !lvlComplete && superFliesLeft != 0)
					{
						runTimeSeconds += gameTime.ElapsedGameTime.TotalSeconds;
					}
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

					bool justCompleted = false;

					if (allDead && !lvlComplete)
					{
						lvlComplete = true;
						justCompleted = true;
					}

					// ONLY allow transition on a NEW click (NOT the same click that killed the last enemy)
					if (lvlComplete && playJustPressed && !justCompleted)
					{
						curScreen = ScreenState.Level3;
						lvlComplete = false;
						shaking = false;
						ResetLevel3();
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
				case ScreenState.Level3:
					bool fightStillGoing = !boss.Dead || AnyMinionsAlive();
					if (timerRunning && !lvlComplete && fightStillGoing)
					{
						runTimeSeconds += gameTime.ElapsedGameTime.TotalSeconds;
					}

					boss.Update(gameTime, graphics);

					// update minions
					for (int i = 0; i < level3MinionsAlive; i++)
					{
						if (level3Minions[i] != null)
							level3Minions[i].Update(gameTime, graphics);
					}

					swatter.Update(gameTime, inputManager);
					swatter.Color = Color.White;

					if (inputManager.Swat)
					{
						swat = Content.Load<SoundEffect>("Hit6");

						// Boss hit check
						if (!boss.Dead && boss.Bounds.CollidesWith(swatter.Bounds))
						{
							swatter.Color = Color.Red;

							int spawnHowMany;
							bool ramped = boss.RegisterHit(out spawnHowMany);

							flyHit.Play();
							shakeTime = 0;
							shaking = true;

							if (ramped && spawnHowMany > 0)
								SpawnLevel3Minions(spawnHowMany);
						}

						// Minion hit check
						for (int i = 0; i < level3MinionsAlive; i++)
						{
							var m = level3Minions[i];
							if (m != null && !m.Dead && m.Bounds.CollidesWith(swatter.Bounds))
							{
								swatter.Color = Color.Red;
								m.Dead = true;
								flyHit.Play();
								flyDeath.PlaceKillParticle(new Vector2(m.Position.X + 32, m.Position.Y + 32));
								shakeTime = 0;
								shaking = true;
							}
						}
					}

					bool bossDead = boss.Dead;
					bool minionsDead = !AnyMinionsAlive();

					bool justCompletedBoss = false;

					if (bossDead && minionsDead && !lvlComplete)
					{
						lvlComplete = true;
						justCompletedBoss = true;

						timerRunning = false;
						finalRunTimeSeconds = runTimeSeconds;

						SaveTimeIfBest(finalRunTimeSeconds);
					}

					// wait for a NEW click (NOT the click that finished the fight)
					if (lvlComplete && playJustPressed && !justCompletedBoss)
					{
						curScreen = ScreenState.Title;

						ResetLevel1();
						ResetLevel2();
						ResetLevel3();

						elapsedTime = 0;
						lvlComplete = false;
						shaking = false;
					}

					if (inputManager.ToMenu)
					{
						curScreen = ScreenState.Title;
						ResetLevel1();
						ResetLevel2();
						ResetLevel3();
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
			// shakeTransform *= Matrix.CreateTranslation(23, 23, 0);

			// TODO: Add your drawing code here
			spriteBatch.Begin(transformMatrix: shakeTransform);

			switch (curScreen)
			{
				case ScreenState.Title:
					_titleAnim.Draw(spriteBatch, GraphicsDevice);
					_startButton.Draw(spriteBatch);
					break;

				case ScreenState.HowToPlay:
					spriteBatch.DrawString(spriteFont, "Click all the flies!", new Vector2(2, 200), Color.DarkSlateGray);
					spriteBatch.DrawString(spriteFont, "Click to play. To return to the menu, press Space", new Vector2(2, 300), Color.DarkSlateGray);
					break;

				case ScreenState.Level1:
					_tilemap.Draw(gameTime, spriteBatch, GraphicsDevice);

					foreach (var fly in flies) { fly.Draw(gameTime, spriteBatch); }

					// debug stuff
					if (_showColliders)
					{
						foreach (var fly in flies)
						{
							if (!fly.Dead)
								DebugDraw.CircleOutline(spriteBatch, _debugPixel, fly.Bounds.Center, fly.Bounds.Radius);
						}
					}

					if (fliesLeft == 0)
					{
						spriteBatch.DrawString(spriteFont, "YOU WIN LEVEL 1!", new Vector2(350, 2), Color.Gold);
						spriteBatch.DrawString(spriteFont, "Click to go to Level 2", new Vector2(350, 70), Color.DarkSlateGray);
					}

					spriteBatch.DrawString(spriteFont, $"Flies left: {fliesLeft}", new Vector2(10, 5), Color.DarkSlateGray);

					spriteBatch.DrawString(spriteFont, FormatTime(runTimeSeconds), new Vector2(10, 50), Color.DarkSlateGray);

					spriteBatch.DrawString(spriteFont, $"Best: {bestTimeString}", new Vector2(10, 100), Color.Gold);
					break;

				case ScreenState.Level2:
					_tilemap.Draw(gameTime, spriteBatch, GraphicsDevice);

					foreach (var sFly in superFlies) sFly.Draw(gameTime, spriteBatch);

					// debug stuff
					if (_showColliders)
					{
						foreach (var sFly in superFlies)
						{
							if (!sFly.Dead)
								DebugDraw.CircleOutline(spriteBatch, _debugPixel, sFly.Bounds.Center, sFly.Bounds.Radius);
						}
					}

					superFliesLeft = 0;
					foreach (var sFly in superFlies)
						if (!sFly.Dead) superFliesLeft++;

					spriteBatch.DrawString(spriteFont, $"Super flies left: {superFliesLeft}", new Vector2(10, 5), Color.DarkSlateGray);

					if (lvlComplete)
					{
						spriteBatch.DrawString(spriteFont, "YOU WIN LEVEL 2!", new Vector2(350, 2), Color.Gold);
						spriteBatch.DrawString(spriteFont, "Press Space to enter", new Vector2(350, 70), Color.DarkSlateGray);
						spriteBatch.DrawString(spriteFont, "THE BOSS FIGHT", new Vector2(370, 140), Color.Red);
					}

					spriteBatch.DrawString(spriteFont, FormatTime(runTimeSeconds), new Vector2(10, 50), Color.DarkSlateGray);

					break;

				case ScreenState.Level3:
					_tilemap.Draw(gameTime, spriteBatch, GraphicsDevice);

					boss.Draw(gameTime, spriteBatch);

					// debug stuff
					if (_showColliders && !boss.Dead)
					{
						DebugDraw.CircleOutline(spriteBatch, _debugPixel, boss.Bounds.Center, boss.Bounds.Radius);
					}

					for (int i = 0; i < level3MinionsAlive; i++)
						if (level3Minions[i] != null)
							level3Minions[i].Draw(gameTime, spriteBatch);

					DrawBossHpBar(spriteBatch);
					// Show run timer during the fight, and the final time once completed
					if (!lvlComplete)
					{
						spriteBatch.DrawString(spriteFont, FormatTime(runTimeSeconds), new Vector2(10, 80), Color.White);
					}
					else
					{
						spriteBatch.DrawString(spriteFont, "YOU WIN!", new Vector2(350, 80), Color.Gold);
						spriteBatch.DrawString(spriteFont, "Click to return to menu", new Vector2(280, 140), Color.White);
						spriteBatch.DrawString(spriteFont, $"Final: {FormatTime(finalRunTimeSeconds)}", new Vector2(20, 90), Color.Gold);
						spriteBatch.DrawString(spriteFont, $"Best: {bestTimeString}", new Vector2(20, 130), Color.Gold);
					}

					break;
			}


			spriteBatch.End();

			spriteBatch.Begin();
			if (curScreen == ScreenState.Level1 || curScreen == ScreenState.Level2 || curScreen == ScreenState.Level3)
			{
				swatter.Draw(gameTime, spriteBatch);
			}

			// debug stuff
			if (_showColliders)
			{
				DebugDraw.RectOutline(spriteBatch, _debugPixel, swatter.Bounds);
			}


			spriteBatch.End();

			base.Draw(gameTime);
		}

		private void SaveTimeIfBest(double newTime)
		{
			// bestTimeSeconds was loaded in LoadBestTime()
			if (newTime < bestTimeSeconds)
			{
				newRecord = true;
				bestTimeSeconds = newTime;

				bestTimeString = FormatTime(newTime);

				var result = new
				{
					TimeInSeconds = bestTimeSeconds,
					FormattedTime = bestTimeString
				};

				string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
				File.WriteAllText(filePath, json);
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
				bestTimeSeconds = double.MaxValue;
				bestTimeString = "00:00";

				var result = new { TimeInSeconds = bestTimeSeconds, FormattedTime = bestTimeString };
				string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
				File.WriteAllText(filePath, json);
				return;
			}

			string jsonData = File.ReadAllText(filePath);
			using (JsonDocument doc = JsonDocument.Parse(jsonData))
			{
				if (doc.RootElement.TryGetProperty("TimeInSeconds", out JsonElement timeEl))
					bestTimeSeconds = timeEl.GetDouble();

				if (doc.RootElement.TryGetProperty("FormattedTime", out JsonElement formatted))
					bestTimeString = formatted.GetString() ?? "00:00";
			}
		}

		private string FormatTime(double seconds)
		{
			var span = TimeSpan.FromSeconds(seconds);

			int totalSeconds = (int)span.TotalSeconds;
			int centiseconds = (int)((span.TotalMilliseconds % 1000) / 10.0); // 00-99

			return $"{totalSeconds:D2}:{centiseconds:D2}";
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

		private void ResetLevel3()
		{
			boss.Reset();
			level3MinionsAlive = 0;
			for (int i = 0; i < level3Minions.Length; i++) level3Minions[i] = null; // deletes all the temp boss minions
		}

		private void SpawnLevel3Minions(int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (level3MinionsAlive >= level3Minions.Length) return;

				var pos = new Vector2((float)random.NextDouble() * (GraphicsDevice.Viewport.Width - 64), (float)random.NextDouble() * (GraphicsDevice.Viewport.Height - 64));

				var f = new FlySprite(pos)
				{
					Velocity = new Vector2((float)random.NextDouble(), (float)random.NextDouble())
				};

				f.LoadContent(Content);
				level3Minions[level3MinionsAlive++] = f;
			}
		}

		private bool AnyMinionsAlive()
		{
			for (int i = 0; i < level3MinionsAlive; i++)
			{
				var m = level3Minions[i];
				if (m != null && !m.Dead) return true;
			}
			return false;
		}

		private void DrawBossHpBar(SpriteBatch sb)
		{
			int barWidth = GraphicsDevice.Viewport.Width - 40;
			int barHeight = 20;

			Rectangle back = new Rectangle(20, 20, barWidth, barHeight);

			float pct = (boss.MaxHp == 0) ? 0f : (float)boss.Hp / boss.MaxHp;
			Rectangle fill = new Rectangle(20, 20, (int)(barWidth * pct), barHeight);

			sb.Draw(_hpPixel, back, Color.Black * 0.6f);
			sb.Draw(_hpPixel, fill, Color.Red);
			sb.DrawString(spriteFont, $"Boss HP: {boss.Hp}/{boss.MaxHp}", new Vector2(20, 45), Color.White);
		}
	}
}
