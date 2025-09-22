using System;
using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class AnimatedSprite(CNS game)
{
    public Sprite sprite;
    public Rectangle[] sourceRectangles;

    public int frame = 0;

    public TimeSpan delay = TimeSpan.FromMilliseconds(50);

    public TimeSpan elapsed = TimeSpan.Zero;

    public void Update(GameTime gameTime)
    {
        elapsed += gameTime.ElapsedGameTime;

        if (elapsed >= delay)
        {
            elapsed -= delay;
            frame++;

            if (frame >= sourceRectangles.Length)
            {
                frame = 0;
            }
        }
    }

    public void Draw()
    {
        game.SpriteBatch.Draw(
            sprite.Texture,
            sprite.Position,
            sourceRectangles[frame],
            sprite.Color,
            sprite.Rotation,
            sprite.Origin,
            sprite.Scale,
            sprite.Effects,
            sprite.LayerDepth
        );
    }
}
