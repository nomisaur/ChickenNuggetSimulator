using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended;

namespace State;

public class Input
{
    public MouseState PreviousMouseState;

    public MouseState CurrentMouseState;

    public TouchCollection PreviousTouchState;

    public TouchCollection CurrentTouchState;

    public class Touch
    {
        public Vector2 Position;
        public bool IsPressed;
    }

    public IEnumerable<Touch> Touches;

    public Touch MouseClick = new();

    public void Update()
    {
        PreviousMouseState = CurrentMouseState;
        CurrentMouseState = Mouse.GetState();
        MouseClick.Position = Vector2.Transform(
            new Vector2(CurrentMouseState.X, CurrentMouseState.Y),
            Setup.inverseScreenScaleMatrix
        );
        MouseClick.IsPressed = CurrentMouseState.LeftButton == ButtonState.Pressed;

        PreviousTouchState = CurrentTouchState;
        CurrentTouchState = TouchPanel.GetState();
        Touches = CurrentTouchState.Select(touchPoint =>
        {
            return new Touch
            {
                Position = Vector2.Transform(touchPoint.Position, Setup.inverseScreenScaleMatrix),
                // Position = touchPoint.Position,
                IsPressed =
                    touchPoint.State == TouchLocationState.Pressed
                    || touchPoint.State == TouchLocationState.Moved,
            };
        });
        if (Touches.Count() > 0)
        {
            Console.WriteLine(
                $"Position: {Touches.First().Position}, transformed: {Vector2.Transform(Touches.First().Position, Setup.inverseScreenScaleMatrix)}"
            );
        }
    }

    public void Draw()
    {
        if (MouseClick.IsPressed)
        {
            Setup.SpriteBatch.FillRectangle(
                new RectangleF(MouseClick.Position.X - 10, MouseClick.Position.Y - 10, 20, 20),
                Color.Red
            );
        }

        foreach (var touch in Touches)
        {
            if (touch.IsPressed)
            {
                Setup.SpriteBatch.FillRectangle(
                    new RectangleF(touch.Position.X - 10, touch.Position.Y - 10, 20, 20),
                    Color.Blue
                );
            }
        }
    }
}

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
