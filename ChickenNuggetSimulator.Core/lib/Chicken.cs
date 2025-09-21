using System;
using System.Collections.Generic;
using System.Net.Mail;
using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace State;

public class Nuggets(CNS game)
{
    public int Count => game.SaveSystem.data.Nuggets;

    public int add(int amount)
    {
        game.SaveSystem.data.Nuggets += amount;
        return Count;
    }

    public int subtract(int amount)
    {
        game.SaveSystem.data.Nuggets = Math.Max(0, Count - amount);
        return Count;
    }
}

public class NuggetEffect : Effect
{
    public CNS game;
    public GameTime gameTime;

    public float angle;

    public NuggetEffect(CNS _game, GameTime _gameTime)
    {
        game = _game;
        gameTime = _gameTime;
        angle = ((float)game.rng.NextDouble() - 0.5f) * 0.8f;
        lifespan = 0.4f;
        sprite = new Sprite()
        {
            Texture = game.textures.nugget,
            Position = game.Screen.Center + new Vector2(0, 200),
            Origin = new Vector2(68, 100),
            Rotation = angle + 1.5708f + 4.71239f,
        };
    }

    public override void Update(GameTime gameTime)
    {
        age += (float)gameTime.ElapsedGameTime.TotalSeconds;

        float t = MathF.Min(age / lifespan, 1f);
        float easeIn = 1 + t * t;
        float easeOut = 1 - t * t; // quadratic ease-out
        float newAngle = angle * easeIn + 1.5708f;
        float speed = 1300f * easeOut;
        var dir = new Vector2(MathF.Cos(newAngle), MathF.Sin(newAngle));
        sprite.Position += dir * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        sprite.Rotation = newAngle + 4.71239f;
        if (age >= lifespan)
        {
            alive = false;
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Utils.DrawSprite(game, sprite);
    }

    // public Effect MakeNuggetEffect(GameTime gameTime)
    // {
    //     // var angle = 1.4f + (float)game.rng.NextDouble() * (1.8f - 1.4f);
    //     var angle = ((float)game.rng.NextDouble() - 0.5f) * 0.8f;
    //     return new Effect()
    //     {
    //         alive = true,
    //         lifespan = 0.4f,
    //         angle = angle,
    //         sprite = new Sprite()
    //         {
    //             Texture = game.textures.nugget,
    //             Position = game.Screen.Center + new Vector2(0, 200),
    //             Origin = new Vector2(68, 100),
    //             Rotation = angle + 1.5708f + 4.71239f,
    //         },
    //         Update = Update,
    //         Draw = Draw,
    //     };
    // }
}

public class Chicken
{
    public CNS game;
    public Sprite Sprite;
    public Vector2 Position = Vector2.Zero;
    public Vector2 Size = new Vector2(600, 600);
    public Vector2 Center => new(Size.X * 0.5f, Size.Y * 0.5f);
    public Rectangle Bounds =>
        new((int)(Position.X - Center.X), (int)(Position.Y - Center.Y), (int)Size.X, (int)Size.Y);

    public Chicken(CNS _game)
    {
        game = _game;

        Sprite = new Sprite()
        {
            Texture = game.textures.chicken.rested,
            Origin = new Vector2(300, 300),
            Scale = 1.0f,
            SourceRectangle = null,
        };
    }

    public int Click(GameTime gameTime)
    {
        Sprite.Texture = game.textures.chicken.activated;
        game.effectSystem.Spawn(new NuggetEffect(game, gameTime));
        return game.nuggets.add(1);
    }

    public void Update(GameTime gameTime)
    {
        var input = game.input;
        if (input.JustTouched)
        {
            if (Bounds.Contains((int)input.MouseClick.Position.X, (int)input.MouseClick.Position.Y))
            {
                Click(gameTime);
            }
            foreach (var touch in input.Touches)
            {
                if (Bounds.Contains((int)touch.Position.X, (int)touch.Position.Y))
                {
                    Click(gameTime);
                }
            }
        }
        if (input.JustReleased)
        {
            Sprite.Texture = game.textures.chicken.rested;
        }
    }

    public void Draw()
    {
        game.SpriteBatch.Draw(
            Sprite.Texture,
            Position,
            Sprite.SourceRectangle,
            Sprite.Color,
            Sprite.Rotation,
            Sprite.Origin,
            Sprite.Scale,
            Sprite.Effects,
            Sprite.LayerDepth
        );
    }
}
