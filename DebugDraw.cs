using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameDevGame2
{
	public static class DebugDraw
	{
		public static void RectOutline(SpriteBatch sb, Texture2D px, BoundingRectangle r, int thickness = 2)
		{
			// top
			sb.Draw(px, new Rectangle((int)r.Left, (int)r.Top, (int)r.Width, thickness), Color.Red);
			// bottom
			sb.Draw(px, new Rectangle((int)r.Left, (int)(r.Bottom - thickness), (int)r.Width, thickness), Color.Red);
			// left
			sb.Draw(px, new Rectangle((int)r.Left, (int)r.Top, thickness, (int)r.Height), Color.Red);
			// right
			sb.Draw(px, new Rectangle((int)(r.Right - thickness), (int)r.Top, thickness, (int)r.Height), Color.Red);
		}

		public static void CircleOutline(SpriteBatch sb, Texture2D px, Vector2 center, float radius, int segments = 32, int thickness = 2)
		{
			Vector2 prev = center + new Vector2(radius, 0);

			for (int i = 1; i <= segments; i++)
			{
				float a = MathHelper.TwoPi * i / segments;
				Vector2 next = center + new Vector2(MathF.Cos(a) * radius, MathF.Sin(a) * radius);

				Line(sb, px, prev, next, thickness);
				prev = next;
			}
		}

		private static void Line(SpriteBatch sb, Texture2D px, Vector2 a, Vector2 b, int thickness)
		{
			Vector2 diff = b - a;
			float len = diff.Length();
			float rot = (len > 0.0001f) ? MathF.Atan2(diff.Y, diff.X) : 0f;

			sb.Draw(px, a, null, Color.Red, rot, Vector2.Zero, new Vector2(len, thickness), SpriteEffects.None, 0f);
		}
	}
}
