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

            this.originalVisibility = (int)window.DecorView.SystemUiVisibility;
            int newVisibility = (int)window.DecorView.SystemUiVisibility;
            newVisibility |= (int)SystemUiFlags.Fullscreen;
            window.DecorView.SystemUiVisibility = (StatusBarVisibility)newVisibility;

            // window.AddFlags(WindowManagerFlags.Fullscreen);
            window.AddFlags(WindowManagerFlags.TranslucentNavigation);
            window.AddFlags(WindowManagerFlags.TranslucentStatus);

         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      protected override void Dispose(bool disposing)
      {
         var window = ((Activity)this.Context).Window;

         // window.ClearFlags(WindowManagerFlags.KeepScreenOn);
         window.ClearFlags(WindowManagerFlags.TranslucentNavigation);
         window.ClearFlags(WindowManagerFlags.TranslucentStatus);

         window.DecorView.SystemUiVisibility = (StatusBarVisibility)this.originalVisibility;

         base.Dispose(disposing);
      }

   }
}