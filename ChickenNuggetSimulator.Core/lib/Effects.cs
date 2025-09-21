using System;
using System.IO;
using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;

public abstract class Effect
{
    public bool alive = true;
    public Sprite sprite;
    public float lifespan;
    public float age = 0;

    public abstract void Update(GameTime gameTime);

    public abstract void Draw(GameTime gameTime);
}

public class EffectSystem
{
    public CNS game;
    public Effect[] pool;

    public EffectSystem(CNS _game, int capacity = 30)
    {
        game = _game;
        pool = new Effect[capacity];
    }

    public ref Effect Spawn(Effect effect)
    {
        // Find a free slot (simple linear scan; you can keep a free list)
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i] == null || !pool[i].alive)
            {
                pool[i] = effect;
                return ref pool[i];
            }
        }
        // Pool exhausted; reuse slot 0 (or drop)
        pool[0] = effect;
        return ref pool[0];
    }

    public void Update(GameTime gameTime)
    {
        foreach (Effect effect in pool)
        {
            if (effect == null)
            {
                continue;
            }
            if (effect.alive)
            {
                effect.Update(gameTime);
            }
        }
    }

    public void Draw(GameTime gameTime)
    {
        foreach (Effect effect in pool)
        {
            if (effect == null)
            {
                continue;
            }
            if (effect.alive)
            {
                effect.Draw(gameTime);
            }
        }
    }
}
