using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework.Graphics;

public class Textures(CNS game)
{
    public (Texture2D rested, Texture2D activated) chicken;

    public Texture2D nugget;

    public void LoadTextures()
    {
        chicken.rested = game.Content.Load<Texture2D>("Assets/Chicken/Chicken_rested_600");
        chicken.activated = game.Content.Load<Texture2D>("Assets/Chicken/Chicken_activated_600");
        nugget = game.Content.Load<Texture2D>("Assets/Chicken/nugget");
    }
}
