using Android.App;
using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ComicsShelf.Helpers.Controls.ReadingPage), typeof(ComicsShelf.Droid.CustomRenderers.ReadingPage))]
namespace ComicsShelf.Droid.CustomRenderers
{
   public class ReadingPage : PageRenderer
   {
      public ReadingPage(Context context) : base(context)
      {
         ((Activity)context).Window.AddFlags(WindowManagerFlags.Fullscreen);
         // ((Activity)context).Window.
      }

      protected override void Dispose(bool disposing)
      {
         ((Activity)this.Context).Window.ClearFlags(WindowManagerFlags.Fullscreen);
         base.Dispose(disposing);
      }

   }
}