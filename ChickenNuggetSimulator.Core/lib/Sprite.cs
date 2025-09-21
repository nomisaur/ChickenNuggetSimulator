using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
