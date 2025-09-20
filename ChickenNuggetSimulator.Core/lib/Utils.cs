using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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

    public static void DrawSafeArea(int left, int top, int right, int bottom)
    {
        Setup.SpriteBatch.FillRectangle(
            new RectangleF(Setup.Instance.Insets.Left, Setup.Instance.Height * 0.5f, 10, 10),
            Color.Red
        );

        Setup.SpriteBatch.FillRectangle(
            new RectangleF(
                Setup.Instance.Width - Setup.Instance.Insets.Right - 10,
                Setup.Instance.Height * 0.5f,
                10,
                10
            ),
            Color.Orange
        );

        Setup.SpriteBatch.FillRectangle(
            new RectangleF(Setup.Instance.Width * 0.5f, Setup.Instance.Insets.Top, 10, 10),
            Color.Blue
        );

        Setup.SpriteBatch.FillRectangle(
            new RectangleF(
                Setup.Instance.Width * 0.5f,
                Setup.Instance.Height - Setup.Instance.Insets.Bottom - 10,
                10,
                10
            ),
            Color.Cyan
        );
    }
}
