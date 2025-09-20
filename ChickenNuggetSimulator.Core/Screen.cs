using System;
using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

public static class SafeArea
{
    // Set this from platform projects
    public static Func<(int Left, int Top, int Right, int Bottom)> GetInsets;
}

public class Insets
{
    public int Left = 0;
    public int Top = 0;
    public int Right = 0;
    public int Bottom = 0;
}

public class Screen(CNS game, int targetWidth, int targetHeight, bool letterboxed)
{ // The desired resolution.
    public readonly int Width = targetWidth;
    public readonly int Height = targetHeight;

    public int ActualWidth;
    public int ActualHeight;
    public bool letterboxed = letterboxed;

    public Matrix screenScaleMatrix;
    public Matrix inverseScreenScaleMatrix;
    private static bool isResizing = false;

    public static readonly bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

    public static readonly bool IsDesktop =
        OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

    public Insets Insets = new();

    public void SetupScreen(int desktopWidth, int desktopHeight)
    {
        if (IsDesktop)
        {
            game.Graphics.PreferredBackBufferWidth = desktopWidth;
            game.Graphics.PreferredBackBufferHeight = desktopHeight;
        }

#if ANDROID
        Graphics.IsFullScreen = true;
        Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        Graphics.PreferredBackBufferHeight = GraphicsAdapter
            .DefaultAdapter
            .CurrentDisplayMode
            .Height;
#endif

        game.Graphics.SupportedOrientations = DisplayOrientation.Portrait;
        game.Graphics.IsFullScreen = false;

        game.Window.Title = "Chicken Nugget Simulator";
        game.Window.AllowUserResizing = true;
        game.Window.ClientSizeChanged += OnClientSizeChanged;
        game.Services.AddService(typeof(GraphicsDeviceManager), game.Graphics);

        game.Graphics.ApplyChanges();
    }

    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        Console.WriteLine("OnClientSizeChanged");
        if (
            game.Window.ClientBounds.Width > 0
            && game.Window.ClientBounds.Height > 0
            && game.GraphicsDevice != null
        )
        {
            if (!isResizing)
            {
                isResizing = true;
                UpdateScreenScaleMatrix();
                isResizing = false;
            }
        }
    }

    public void UpdateScreenScaleMatrix()
    {
        if (OperatingSystem.IsAndroid())
        {
            game.Graphics.PreferredBackBufferWidth = GraphicsAdapter
                .DefaultAdapter
                .CurrentDisplayMode
                .Width;
            game.Graphics.PreferredBackBufferHeight = GraphicsAdapter
                .DefaultAdapter
                .CurrentDisplayMode
                .Height;
            game.Graphics.ApplyChanges();
        }
        float ActualWidth = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        float ActualHeight = game.GraphicsDevice.PresentationParameters.BackBufferHeight;

        float scaleX = ActualWidth / Width; // how much we’d scale to match width
        float scaleY = ActualHeight / Height; // how much we’d scale to match height

        // contain = Min, cover = Max
        float scale = letterboxed ? MathF.Min(scaleX, scaleY) : MathF.Max(scaleX, scaleY);

        int virtualX = (int)MathF.Round((ActualWidth - Width * scale) * 0.5f);
        int virtualY = (int)MathF.Round((ActualHeight - Height * scale) * 0.5f);

        // Uniform scale matrix
        screenScaleMatrix =
            Matrix.CreateScale(scale) * Matrix.CreateTranslation(virtualX, virtualY, 0);
        inverseScreenScaleMatrix = Matrix.Invert(screenScaleMatrix);

        TouchPanel.DisplayWidth = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        TouchPanel.DisplayHeight = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
        TouchPanel.DisplayOrientation = game.GraphicsDevice
            .PresentationParameters
            .DisplayOrientation;

        (int left, int top, int right, int bottom) = IsMobile ? SafeArea.GetInsets() : (0, 0, 0, 0);

        int viewW = (int)MathF.Round(Width * scale);
        int viewH = (int)MathF.Round(Height * scale);
        // Usable screen bounds after OS insets (still in *screen* pixels)
        int usableRight = (int)ActualWidth - right;
        int usableBottom = (int)ActualHeight - bottom;

        // Convert to *virtual* distances (this naturally includes cover-cropping)
        float inv = 1f / scale;

        int padLeft = Math.Max(0, (int)MathF.Ceiling((left - virtualX) * inv));
        int padTop = Math.Max(0, (int)MathF.Ceiling((top - virtualY) * inv));
        int padRight = Math.Max(0, (int)MathF.Ceiling((virtualX + viewW - usableRight) * inv));
        int padBottom = Math.Max(0, (int)MathF.Ceiling((virtualY + viewH - usableBottom) * inv));

        // Safety clamp so pads never exceed your virtual size
        padLeft = Math.Min(padLeft, Width);
        padTop = Math.Min(padTop, Height);
        padRight = Math.Min(padRight, Width);
        padBottom = Math.Min(padBottom, Height);

        // ← Final distances you want (in *virtual* pixels)
        Insets = new Insets
        {
            Left = padLeft,
            Top = padTop,
            Right = padRight,
            Bottom = padBottom,
        };
    }
}
