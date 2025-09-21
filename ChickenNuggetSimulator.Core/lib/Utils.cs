using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

public static class Utils
{
    public static void DrawSprite(CNS game, Sprite sprite)
    {
        game.SpriteBatch.Draw(
            sprite.Texture,
            sprite.Position,
            sprite.SourceRectangle,
            sprite.Color,
            sprite.Rotation,
            sprite.Origin,
            sprite.Scale,
            sprite.Effects,
            sprite.LayerDepth
        );
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
