using Android.App;
using Android.Content;
using Android.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ComicsShelf.Reading.ReadingPage), typeof(ComicsShelf.Droid.CustomRenderers.ReadingView))]
namespace ComicsShelf.Droid.CustomRenderers
{
   public class ReadingView : PageRenderer
   {

      private readonly int originalVisibility;
      public ReadingView(Context context) : base(context)
      {
         try
         {
            var window = ((Activity)context).Window;
            /*
            window.AddFlags(WindowManagerFlags.Fullscreen);
            // window.AddFlags(WindowManagerFlags.KeepScreenOn);
            window.AddFlags(WindowManagerFlags.TranslucentNavigation);
            window.AddFlags(WindowManagerFlags.TranslucentStatus);
            */

            this.originalVisibility = (int)window.DecorView.SystemUiVisibility;
            int newVisibility = (int)window.DecorView.SystemUiVisibility;
            newVisibility |= (int)SystemUiFlags.Fullscreen;
            // newVisibility |= (int)SystemUiFlags.HideNavigation;
            newVisibility |= (int)SystemUiFlags.ImmersiveSticky;
            newVisibility |= (int)SystemUiFlags.LightStatusBar;
            newVisibility |= (int)SystemUiFlags.LightNavigationBar;
            window.DecorView.SystemUiVisibility = (StatusBarVisibility)newVisibility;

         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      protected override void Dispose(bool disposing)
      {
         var window = ((Activity)this.Context).Window;

         /*
         window.ClearFlags(WindowManagerFlags.Fullscreen);
         // window.ClearFlags(WindowManagerFlags.KeepScreenOn);
         window.ClearFlags(WindowManagerFlags.TranslucentNavigation);
         window.ClearFlags(WindowManagerFlags.TranslucentStatus);
         */

         window.DecorView.SystemUiVisibility = (StatusBarVisibility)this.originalVisibility;

         base.Dispose(disposing);
      }

   }
}