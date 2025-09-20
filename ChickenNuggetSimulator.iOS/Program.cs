using System;
using System.Linq;
using ChickenNuggetSimulator.Core;
using Foundation;
using UIKit;

namespace ChickenNuggetSimulator.iOS
{
    [Register("AppDelegate")]
    internal class Program : UIApplicationDelegate
    {
        private static CNS _game;

        /// <summary>
        /// Initializes and starts the game by creating an instance of the
        /// Game class and calls its Run method.
        /// </summary>
        internal static void RunGame()
        {
            _game = new CNS();
            _game.Run();
        }

        /// <summary>
        /// Called when the application has finished launching.
        /// This method starts the game by calling RunGame.
        /// </summary>
        /// <param name="app">The UIApplication instance representing the application.</param>
        public override void FinishedLaunching(UIApplication app)
        {
            SafeArea.GetInsets = () =>
            {
#if IOS13_0_OR_GREATER
                (int L, int T, int R, int B) Zero() => (0, 0, 0, 0);

                // Ensure we're on main thread
                if (!NSThread.IsMain)
                {
                    (int l, int t, int r, int b) result = Zero();
                    UIApplication.SharedApplication.InvokeOnMainThread(() =>
                    {
                        result = SafeArea.GetInsets(); // re-enter on main
                    });
                    return result;
                }

                // iOS 13+: get the foreground, key window from the active UIWindowScene
                var window2 =
                    UIApplication
                        .SharedApplication.ConnectedScenes.OfType<UIWindowScene>()
                        .Where(s => s.ActivationState == UISceneActivationState.ForegroundActive)
                        .SelectMany(s => s.Windows)
                        .FirstOrDefault(w => w.IsKeyWindow)
                    // fallback (older iOS / rare cases)
                    ?? UIApplication.SharedApplication.Windows.FirstOrDefault(w =>
                        !w.Hidden && w.WindowLevel == UIWindowLevel.Normal
                    );

                if (window2 == null)
                {
                    Console.WriteLine("No key window yet; returning zero insets.");
                    return Zero();
                }

                // Prefer the root VC's view insets (more reliable post-layout)
                UIEdgeInsets insets2 = UIEdgeInsets.Zero;
                var vcView = window2.RootViewController?.View;
                if (vcView != null)
                {
                    // Safe once the view has laid out at least once; otherwise may be zero
                    insets2 = vcView.SafeAreaInsets;
                }
                if (insets2 == UIEdgeInsets.Zero)
                {
                    // Fallback to window insets
                    insets2 = window2.SafeAreaInsets;
                }

                // Points -> pixels (MonoGame typically wants pixels)
                nfloat scale2 = UIScreen.MainScreen.Scale;

                return (
                    (int)Math.Round(insets2.Left * scale2),
                    (int)Math.Round(insets2.Top * scale2),
                    (int)Math.Round(insets2.Right * scale2),
                    (int)Math.Round(insets2.Bottom * scale2)
                );

# else
                UIWindow window = null;
                window = UIApplication.SharedApplication.KeyWindow;
                Console.WriteLine("old", UIApplication.SharedApplication.KeyWindow);

                if (window == null)
                {
                    Console.WriteLine("WHY IS WINDOW NULL????");
                    return (0, 0, 0, 0);
                }

                var scale = UIScreen.MainScreen.Scale;
                var insets = window.SafeAreaInsets;

                return (
                    (int)(insets.Left * scale),
                    (int)(insets.Top * scale),
                    (int)(insets.Right * scale),
                    (int)(insets.Bottom * scale)
                );
# endif
            };

            RunGame();
        }

        /// <summary>
        /// The main entry point for the application.
        /// This sets up the application and specifies the UIApplicationDelegate
        /// class to handle application lifecycle events.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, typeof(Program));
        }
    }
}
