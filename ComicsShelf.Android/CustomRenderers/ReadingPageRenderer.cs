using Android.App;
using Android.Content;
using Android.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ComicsShelf.Screens.Reading.Page), typeof(ComicsShelf.Droid.CustomRenderers.ReadingPageRenderer))]
namespace ComicsShelf.Droid.CustomRenderers
{
   public class ReadingPageRenderer : PageRenderer
   {

      readonly int _Visibility;

      public ReadingPageRenderer(Context context) : base(context)
      {
         try
         {
            var _Window = ((Activity)context).Window;
            _Visibility = (int)_Window.DecorView.SystemUiVisibility;

            /*
            var newVisibility = (int)_Window.DecorView.SystemUiVisibility;
            newVisibility |= (int)SystemUiFlags.Fullscreen;
            newVisibility |= (int)SystemUiFlags.HideNavigation;
            newVisibility |= (int)SystemUiFlags.LowProfile;
            newVisibility |= (int)SystemUiFlags.ImmersiveSticky;
            _Window.DecorView.SystemUiVisibility = (StatusBarVisibility)newVisibility;
            */

            _Window.AddFlags(WindowManagerFlags.Fullscreen);
            // _Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
            // _Window.AddFlags(WindowManagerFlags.TranslucentStatus);
         }
         catch (Exception ex) { ComicsShelf.Helpers.Insights.TrackException(ex); }
      }

      protected override void Dispose(bool disposing)
      {
         try
         {
            var _Window = ((Activity)Context).Window;

            // _Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
            _Window.ClearFlags(WindowManagerFlags.Fullscreen);
            // _Window.ClearFlags(WindowManagerFlags.TranslucentNavigation);
            // _Window.ClearFlags(WindowManagerFlags.TranslucentStatus);

            // _Window.DecorView.SystemUiVisibility = (StatusBarVisibility)_Visibility;

         }
         catch (Exception ex) { ComicsShelf.Helpers.Insights.TrackException(ex); }
         base.Dispose(disposing);
      }

   }
}