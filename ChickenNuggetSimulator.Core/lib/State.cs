using System;
using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace State;

public class Nuggets
{
    public int Count = 0;

    public int add(int amount)
    {
        Count += amount;
        return Count;
    }

    public int subtract(int amount)
    {
        Count = Math.Max(0, Count - amount);
        return Count;
    }
}

public class Chicken
{
    public ChickenNuggetSimulatorGame game;

    public Chicken(ChickenNuggetSimulatorGame game)
    {
        this.game = game;
    }

    public Vector2 Position = Vector2.Zero;

    public Vector2 Size = new Vector2(700, 700);
    public Vector2 Center => new(Size.X * 0.5f, Size.Y * 0.5f);

    public Rectangle Bounds =>
        new((int)(Position.X - Center.X), (int)(Position.Y - Center.Y), (int)Size.X, (int)Size.Y);

    public Sprite Sprite = new()
    {
        Texture = Setup.Content.Load<Texture2D>("Assets/Chicken/chicken"),
        Origin = new Vector2(700, 700),
        Scale = 0.5f,
        SourceRectangle = new Rectangle(0, 0, 1400, 1400),
    };

    public int Click()
    {
        return game.nuggets.add(1);
    }

    public void Update()
    {
        var input = game.input;
        if (input.JustTouched)
        {
            if (Bounds.Contains((int)input.MouseClick.Position.X, (int)input.MouseClick.Position.Y))
            {
                Click();
            }
            foreach (var touch in input.Touches)
            {
                if (Bounds.Contains((int)touch.Position.X, (int)touch.Position.Y))
                {
                    Click();
                }
            }
        }
    }
}

public class Sprite
{
    public Texture2D Texture;
    public Vector2 Position = Vector2.Zero;

    public Rectangle? SourceRectangle = null;

    public Vector2 Origin = Vector2.Zero;
    public float Scale = 1.0f;
    public float Rotation = 0.0f;
    public Color Color = Color.White;

    public SpriteEffects Effects = SpriteEffects.None;
    public float LayerDepth = 0.0f;
}
