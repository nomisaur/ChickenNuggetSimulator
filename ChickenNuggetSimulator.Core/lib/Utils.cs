using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

public static class Utils
{
    public static Texture2D MakeTexture(CNS game, string path)
    {
        return game.Content.Load<Texture2D>(path);
    }

    public static void DrawSafeArea(CNS game)
    {
        game.SpriteBatch.FillRectangle(
            new RectangleF(game.Screen.Insets.Left, game.Screen.Height * 0.5f, 10, 10),
            Color.Red
        );

        game.SpriteBatch.FillRectangle(
            new RectangleF(
                game.Screen.Width - game.Screen.Insets.Right - 10,
                game.Screen.Height * 0.5f,
                10,
                10
            ),
            Color.Orange
        );

        game.SpriteBatch.FillRectangle(
            new RectangleF(game.Screen.Width * 0.5f, game.Screen.Insets.Top, 10, 10),
            Color.Blue
        );

        game.SpriteBatch.FillRectangle(
            new RectangleF(
                game.Screen.Width * 0.5f,
                game.Screen.Height - game.Screen.Insets.Bottom - 10,
                10,
                10
            ),
            Color.Cyan
        );
    }
}
