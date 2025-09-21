using System;
using System.Collections.Generic;
using System.Net.Mail;
using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

public class NuggetEffect(CNS game)
{
    public Texture2D NuggetTexture = game.Content.Load<Texture2D>("Assets/Chicken/nugget");

    public void Update(Effect self, GameTime gameTime)
    {
        self.age += (float)gameTime.ElapsedGameTime.TotalSeconds;

        float t = MathF.Min(self.age / self.lifespan, 1f);
        float easeIn = 1 + t * t;
        float easeOut = 1 - t * t; // quadratic ease-out
        float newAngle = self.angle * easeIn + 1.5708f;
        float speed = 1300f * easeOut;
        var dir = new Vector2(MathF.Cos(newAngle), MathF.Sin(newAngle));
        self.sprite.Position += dir * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        self.sprite.Rotation = newAngle + 4.71239f;
        if (self.age >= self.lifespan)
        {
            self.alive = false;
        }
    }

    public void Draw(Effect self, GameTime gameTime)
    {
        Utils.DrawSprite(game, self.sprite);
    }

    public Effect MakeNuggetEffect(GameTime gameTime)
    {
        // var angle = 1.4f + (float)game.rng.NextDouble() * (1.8f - 1.4f);
        var angle = ((float)game.rng.NextDouble() - 0.5f) * 0.8f;
        return new Effect()
        {
            alive = true,
            lifespan = 0.4f,
            angle = angle,
            sprite = new Sprite()
            {
                Texture = NuggetTexture,
                Position = game.Screen.Center + new Vector2(0, 200),
                Origin = new Vector2(68, 100),
                Rotation = angle + 1.5708f + 4.71239f,
            },
            Update = Update,
            Draw = Draw,
        };
    }
}

public class Chicken
{
    public CNS game;
    public Sprite Sprite;
    public Dictionary<string, Texture2D> Textures = new();
    public Vector2 Position = Vector2.Zero;
    public Vector2 Size = new Vector2(600, 600);
    public Vector2 Center => new(Size.X * 0.5f, Size.Y * 0.5f);
    public Rectangle Bounds =>
        new((int)(Position.X - Center.X), (int)(Position.Y - Center.Y), (int)Size.X, (int)Size.Y);

    public NuggetEffect nuggetEffect;

    public Chicken(CNS _game)
    {
        game = _game;
        nuggetEffect = new NuggetEffect(game);
        Textures["rested"] = game.Content.Load<Texture2D>("Assets/Chicken/Chicken_rested_600");
        Textures["activated"] = game.Content.Load<Texture2D>(
            "Assets/Chicken/Chicken_activated_600"
        );

        Sprite = new Sprite()
        {
            Texture = Textures["rested"],
            Origin = new Vector2(300, 300),
            Scale = 1.0f,
            SourceRectangle = null,
        };
    }

    public int Click(GameTime gameTime)
    {
        Sprite.Texture = Textures["activated"];
        game.effectSystem.Spawn(nuggetEffect.MakeNuggetEffect(gameTime));
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
            Sprite.Texture = Textures["rested"];
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
