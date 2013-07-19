using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace cnnnet.Viewer
{
    public static class GameUtils
    {
        public static Texture2D CreateCircle(GraphicsDevice graphicsDevice, int radius)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            var texture = new Texture2D(graphicsDevice, outerRadius, outerRadius);

            var data = new Color[outerRadius * outerRadius];

            // Color the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                var x = (int)Math.Round(radius + radius * Math.Cos(angle));
                var y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        public static void DrawLine(SpriteBatch batch, Texture2D blank,
              float width, Color color, Vector2 point1, Vector2 point2)
        {
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            var length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }

        public static void DrawTexture(SpriteBatch spriteBatch, Texture2D texture, int posX, int posY)
        {
            spriteBatch.Draw(texture,
                new Vector2(posX - texture.Width / 2, posY - texture.Height / 2),
                Color.White);
        }
    }
}
