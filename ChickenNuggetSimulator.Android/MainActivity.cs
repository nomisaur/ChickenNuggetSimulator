using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using ChickenNuggetSimulator.Core;
using Microsoft.Xna.Framework;

namespace ChickenNuggetSimulator.Android
{
    /// <summary>
    /// The main activity for the Android application. It initializes the game instance,
    /// sets up the rendering view, and starts the game loop.
    /// </summary>
    /// <remarks>
    /// This class is responsible for managing the Android activity lifecycle and integrating
    /// with the MonoGame framework.
    /// </remarks>
    [Activity(
        Label = "ChickenNuggetSimulator",
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation
            | ConfigChanges.Keyboard
            | ConfigChanges.KeyboardHidden
    )]
    public class MainActivity : AndroidGameActivity
    {
        private CNS _game;
        private View _view;

        /// <summary>
        /// Called when the activity is first created. Initializes the game instance,
        /// retrieves its rendering view, and sets it as the content view of the activity.
        /// Finally, starts the game loop.
        /// </summary>
        /// <param name="bundle">A Bundle containing the activity's previously saved state, if any.</param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _game = new CNS();
            _view = _game.Services.GetService(typeof(View)) as View;

            SetContentView(_view);

            SafeArea.GetInsets = () =>
            {
                var decor = Window?.DecorView;
                if (decor == null)
                    return (0, 0, 0, 0);

                var insetsCompat = ViewCompat.GetRootWindowInsets(decor);
                if (insetsCompat == null)
                    return (0, 0, 0, 0);

                var i = insetsCompat.GetInsets(WindowInsetsCompat.Type.SystemBars());
                return (i.Left, i.Top, i.Right, i.Bottom);
            };

            _game.Run();
        }
    }
}
