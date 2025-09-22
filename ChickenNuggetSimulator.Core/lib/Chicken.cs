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
    public float speed;

    public NuggetEffect(CNS _game, GameTime _gameTime)
    {
        game = _game;
        gameTime = _gameTime;
        angle = Utils.getRandomBetween(game, -0.4f, 0.4f);
        speed = Utils.getRandomBetween(game, 1000f, 2000f);
        lifespan = 0.4f;
        sprite = new Sprite()
        {
            Texture = game.textures.nugget,
            Position = game.Screen.Center + new Vector2(0, 200),
            Origin = new Vector2(68, 100),
            Rotation = angle,
        };
    }

    public override void Update(GameTime gameTime)
    {
        age += (float)gameTime.ElapsedGameTime.TotalSeconds;

        float progress = Utils.getProgress(age, lifespan);
        float tailCutoff = 0.7f;
        float tailProgress =
            age > lifespan * tailCutoff
                ? Utils.getProgress(age - lifespan * tailCutoff, lifespan * (1 - tailCutoff))
                : 0;
        float ease = progress * progress;
        float easeIn = 1 + ease;
        float easeOut = 1 - ease; // quadratic ease-out
        float newAngle = angle * easeIn;
        Vector2 direction = Utils.getDirectionFromAngle(newAngle + Utils.turn.quarter);
        sprite.Position +=
            direction * speed * easeOut * (float)gameTime.ElapsedGameTime.TotalSeconds;
        sprite.Rotation = newAngle;
        sprite.Color = Color.White * (1 - tailProgress * tailProgress);
        if (age >= lifespan)
        {
            alive = false;
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Utils.DrawSprite(game, sprite);
    }
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
        new Rectangle();

        Sprite = new Sprite()
        {
            Texture = game.animations.chicken.rested.texture,

            Origin = new Vector2(350, 350),
            Scale = 1.0f,
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
            game.animations.chicken.rested.texture,
            Position,
            game.animations.chicken.rested.sourceRectangles[game.animations.chicken.rested.frame],
            Sprite.Color,
            Sprite.Rotation,
            Sprite.Origin,
            Sprite.Scale,
            Sprite.Effects,
            Sprite.LayerDepth
        );
    }
}
