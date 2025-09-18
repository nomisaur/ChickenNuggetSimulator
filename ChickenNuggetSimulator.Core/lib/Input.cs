using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended;

public class Input
{
    // public MouseState PreviousMouseState;

    public MouseState CurrentMouseState;

    // public TouchCollection PreviousTouchState;

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
        // PreviousMouseState = CurrentMouseState;
        CurrentMouseState = Mouse.GetState();
        MouseClick.Position = Vector2.Transform(
            new Vector2(CurrentMouseState.X, CurrentMouseState.Y),
            Setup.inverseScreenScaleMatrix
        );
        MouseClick.IsPressed = CurrentMouseState.LeftButton == ButtonState.Pressed;

        // PreviousTouchState = CurrentTouchState;
        CurrentTouchState = TouchPanel.GetState();
        Touches = CurrentTouchState.Select(touchPoint =>
        {
            return new Touch
            {
                Position = Vector2.Transform(touchPoint.Position, Setup.inverseScreenScaleMatrix),
                IsPressed =
                    touchPoint.State == TouchLocationState.Pressed
                    || touchPoint.State == TouchLocationState.Moved,
            };
        });
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
