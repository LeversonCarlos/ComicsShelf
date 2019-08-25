using Android.App;
using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ComicsShelf.ComicFiles.ReadingView), typeof(ComicsShelf.Droid.CustomRenderers.ReadingView))]
namespace ComicsShelf.Droid.CustomRenderers
{
   public class ReadingView : PageRenderer
   {

      private readonly int originalVisibility;
      public ReadingView(Context context) : base(context)
      {
         var window = ((Activity)context).Window;

         // window.AddFlags(WindowManagerFlags.Fullscreen);
         // window.AddFlags(WindowManagerFlags.TurnScreenOn);

         this.originalVisibility = (int)window.DecorView.SystemUiVisibility;
         int newVisibility = (int)window.DecorView.SystemUiVisibility;
         newVisibility |= (int)SystemUiFlags.Fullscreen;
         newVisibility |= (int)SystemUiFlags.HideNavigation;
         newVisibility |= (int)SystemUiFlags.ImmersiveSticky;
         window.DecorView.SystemUiVisibility = (StatusBarVisibility)newVisibility;

         // ((Activity)context).Window.AddFlags(WindowManagerFlags.TranslucentStatus);
         // ((Activity)context).Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
         // ((Activity)context).Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
      }

      protected override void Dispose(bool disposing)
      {
         var window = ((Activity)this.Context).Window;

         // window.ClearFlags(WindowManagerFlags.TranslucentStatus);
         // window.ClearFlags(WindowManagerFlags.TurnScreenOn);

         window.DecorView.SystemUiVisibility = (StatusBarVisibility)this.originalVisibility;

         base.Dispose(disposing);
      }

   }
}