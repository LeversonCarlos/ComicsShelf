using Android.App;
using Android.Content;
using Android.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ComicsShelf.Screens.Shells.ShellPage), typeof(ComicsShelf.Droid.CustomRenderers.ShellPageRenderer))]
namespace ComicsShelf.Droid.CustomRenderers
{
   public class ShellPageRenderer : ShellRenderer
   {

      public ShellPageRenderer(Context context) : base(context)
      {
         try
         {
            var _Window = ((Activity)context).Window;
            var statusBarColor = (Color)App.Current.Resources["TabBarBackgroundColor"];
            statusBarColor = statusBarColor.AddLuminosity(-0.1);

            // _Window.SetStatusBarColor(statusBarColor.ToAndroid());
            // _Window.AddFlags(WindowManagerFlags.TranslucentStatus);

            // _Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
            _Window.SetNavigationBarColor(statusBarColor.ToAndroid());

         }
         catch (Exception ex) { ComicsShelf.Helpers.Insights.TrackException(ex); }
      }

      protected override void Dispose(bool disposing)
      {
         try
         {
            var _Window = ((Activity)AndroidContext).Window;

            // _Window.ClearFlags(WindowManagerFlags.TranslucentStatus);

            // _Window.ClearFlags(WindowManagerFlags.TranslucentNavigation);

         }
         catch (Exception ex) { ComicsShelf.Helpers.Insights.TrackException(ex); }
         base.Dispose(disposing);
      }

   }
}