using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace State;

public class Chicken
{
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
