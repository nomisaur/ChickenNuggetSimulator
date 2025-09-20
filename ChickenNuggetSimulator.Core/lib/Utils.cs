using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using State;

public class Utils
{
    public static Texture2D MakeTexture(string path)
    {
        return ChickenNuggetSimulatorGame.Content.Load<Texture2D>(path);
    }

    public static void DrawChicken(Chicken chicken)
    {
        ChickenNuggetSimulatorGame.SpriteBatch.Draw(
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
        ChickenNuggetSimulatorGame.SpriteBatch.FillRectangle(
            new RectangleF(
                ChickenNuggetSimulatorGame.Instance.Insets.Left,
                ChickenNuggetSimulatorGame.Instance.Height * 0.5f,
                10,
                10
            ),
            Color.Red
        );

        ChickenNuggetSimulatorGame.SpriteBatch.FillRectangle(
            new RectangleF(
                ChickenNuggetSimulatorGame.Instance.Width
                    - ChickenNuggetSimulatorGame.Instance.Insets.Right
                    - 10,
                ChickenNuggetSimulatorGame.Instance.Height * 0.5f,
                10,
                10
            ),
            Color.Orange
        );

        ChickenNuggetSimulatorGame.SpriteBatch.FillRectangle(
            new RectangleF(
                ChickenNuggetSimulatorGame.Instance.Width * 0.5f,
                ChickenNuggetSimulatorGame.Instance.Insets.Top,
                10,
                10
            ),
            Color.Blue
        );

        ChickenNuggetSimulatorGame.SpriteBatch.FillRectangle(
            new RectangleF(
                ChickenNuggetSimulatorGame.Instance.Width * 0.5f,
                ChickenNuggetSimulatorGame.Instance.Height
                    - ChickenNuggetSimulatorGame.Instance.Insets.Bottom
                    - 10,
                10,
                10
            ),
            Color.Cyan
        );
    }
}
