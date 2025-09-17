using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using State;

public class Utils
{
    public static Texture2D MakeTexture(string path)
    {
        return Setup.Content.Load<Texture2D>(path);
    }

    public static void DrawChicken(Chicken chicken)
    {
        Setup.SpriteBatch.Draw(
            chicken.Sprite.Texture,
            chicken.Position,
            chicken.Sprite.SourceRectangle,
            chicken.Sprite.Color,
            chicken.Sprite.Rotation,
            chicken.Sprite.Origin,
            chicken.Sprite.Scale,
            chicken.Sprite.Effects,
            chicken.Sprite.LayerDepth
        );
    }
}
