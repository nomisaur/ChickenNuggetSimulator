using System;
using System.Collections.Generic;
using System.Linq;
using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended;

public class Input()
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
    public bool IsTouching = false;

    internal bool PreviousIsTouching = false;

    public bool JustTouched = false;
    public bool JustReleased = false;

    public void Update(CNS game)
    {
        // PreviousMouseState = CurrentMouseState;
        CurrentMouseState = Mouse.GetState();
        MouseClick.Position = Vector2.Transform(
            new Vector2(CurrentMouseState.X, CurrentMouseState.Y),
            game.Screen.inverseScreenScaleMatrix
        );
        MouseClick.IsPressed = CurrentMouseState.LeftButton == ButtonState.Pressed;

        // PreviousTouchState = CurrentTouchState;
        CurrentTouchState = TouchPanel.GetState();
        Touches = CurrentTouchState.Select(touchPoint =>
        {
            return new Touch
            {
                Position = Vector2.Transform(
                    touchPoint.Position,
                    game.Screen.inverseScreenScaleMatrix
                ),
                IsPressed =
                    touchPoint.State == TouchLocationState.Pressed
                    || touchPoint.State == TouchLocationState.Moved,
            };
        });

        IsTouching = MouseClick.IsPressed || Touches.Any(t => t.IsPressed);

        JustTouched = IsTouching && !PreviousIsTouching;
        JustReleased = !IsTouching && PreviousIsTouching;

        // Console.WriteLine(
        //     $"IsTouching: {IsTouching}, JustTouched: {JustTouched}, JustReleased: {JustReleased}, prev: {PreviousIsTouching}"
        // );

        PreviousIsTouching = IsTouching;
    }

    public void Draw(CNS game)
    {
        if (MouseClick.IsPressed)
        {
            game.SpriteBatch.FillRectangle(
                new RectangleF(MouseClick.Position.X - 10, MouseClick.Position.Y - 10, 20, 20),
                Color.Red
            );
        }

        foreach (var touch in Touches)
        {
            if (touch.IsPressed)
            {
                game.SpriteBatch.FillRectangle(
                    new RectangleF(touch.Position.X - 10, touch.Position.Y - 10, 20, 20),
                    Color.Blue
                );
            }
        }
    }
}
