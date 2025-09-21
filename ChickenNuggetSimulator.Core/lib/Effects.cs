using System;
using System.IO;
using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;

public class Effect
{
    public bool alive;
    public Sprite sprite;
    public float lifespan;
    public float age = 0;

    public float angle;
    public Action<Effect, GameTime> Update;
    public Action<Effect, GameTime> Draw;
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
                effect.Update(effect, gameTime);
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
                effect.Draw(effect, gameTime);
            }
        }
    }
}
