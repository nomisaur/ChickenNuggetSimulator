using System;
using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Textures(CNS game)
{
    public (Texture2D rested, Texture2D activated) chicken;

    public Texture2D nugget;

    public void LoadTextures()
    {
        chicken.rested = game.Content.Load<Texture2D>("Assets/Chicken/chicken_rested_3500x2800");
        chicken.activated = game.Content.Load<Texture2D>("Assets/Chicken/Chicken_activated_600");
        nugget = game.Content.Load<Texture2D>("Assets/Chicken/nugget");
    }
}

public class Animation
{
    public Texture2D texture;
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
}

public class Animations(CNS game)
{
    public (Animation rested, Texture2D activated) chicken;

    public void LoadAnimations()
    {
        int size = 700;
        chicken.rested = new Animation()
        {
            texture = game.textures.chicken.rested,
            sourceRectangles =
            [
                new Rectangle(0, 0, size, size),
                new Rectangle(700, 0, size, size),
                new Rectangle(1400, 0, size, size),
                new Rectangle(2100, 0, size, size),
                new Rectangle(2800, 0, size, size),
                new Rectangle(0, 700, size, size),
                new Rectangle(700, 700, size, size),
                new Rectangle(1400, 700, size, size),
                new Rectangle(2100, 700, size, size),
                new Rectangle(2800, 700, size, size),
                new Rectangle(0, 1400, size, size),
                new Rectangle(700, 1400, size, size),
                new Rectangle(1400, 1400, size, size),
                new Rectangle(2100, 1400, size, size),
                new Rectangle(2800, 1400, size, size),
                new Rectangle(0, 2100, size, size),
                new Rectangle(700, 2100, size, size),
                new Rectangle(1400, 2100, size, size),
                new Rectangle(2100, 2100, size, size),
                new Rectangle(2800, 2100, size, size),
            ],
        };
    }
}
