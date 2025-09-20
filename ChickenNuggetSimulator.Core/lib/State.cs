using System;
using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace State;

public class Nuggets(ChickenNuggetSimulatorGame game)
{
    public int Count => game.saveSystem.data.Nuggets;

    public int add(int amount)
    {
        game.saveSystem.data.Nuggets += amount;
        return Count;
    }

    public int subtract(int amount)
    {
        game.saveSystem.data.Nuggets = Math.Max(0, Count - amount);
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

    public Vector2 Size = new Vector2(600, 600);
    public Vector2 Center => new(Size.X * 0.5f, Size.Y * 0.5f);

    public Rectangle Bounds =>
        new((int)(Position.X - Center.X), (int)(Position.Y - Center.Y), (int)Size.X, (int)Size.Y);

    public class Textures
    {
        public static Texture2D Rested = Setup.Content.Load<Texture2D>(
            "Assets/Chicken/Chicken_rested_600"
        );
        public static Texture2D Activated = Setup.Content.Load<Texture2D>(
            "Assets/Chicken/Chicken_activated_600"
        );
    }

    public Sprite Sprite = new()
    {
        Texture = Textures.Rested,
        Origin = new Vector2(300, 300),
        Scale = 1.0f,
        SourceRectangle = null,
    };

    public int Click()
    {
        Sprite.Texture = Textures.Activated;
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
        if (input.JustReleased)
        {
            Sprite.Texture = Textures.Rested;
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
