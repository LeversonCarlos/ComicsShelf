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

      public ReadingView(Context context) : base(context)
      {
         ((Activity)context).Window.AddFlags(WindowManagerFlags.Fullscreen);
         // ((Activity)context).Window.AddFlags(WindowManagerFlags.TranslucentStatus);
         // ((Activity)context).Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
         // ((Activity)context).Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
      }

      protected override void Dispose(bool disposing)
      {
         ((Activity)this.Context).Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
         base.Dispose(disposing);
      }

   }
}