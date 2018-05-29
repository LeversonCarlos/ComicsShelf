using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace ComicsShelf.Droid
{
   [Activity(Label = "Comics Shelf", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
   public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
   {
      protected override void OnCreate(Bundle bundle)
      {
         TabLayoutResource = Resource.Layout.Tabbar;
         ToolbarResource = Resource.Layout.Toolbar;

         base.OnCreate(bundle);

         FFImageLoading.Forms.Droid.CachedImageRenderer.Init(true);

         global::Xamarin.Forms.Forms.Init(this, bundle);
         LoadApplication(new App());
      }

      internal delegate void PermissionsResultHandler(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults);
      internal event PermissionsResultHandler OnPermissionsResult;
      public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
      {
         base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
         OnPermissionsResult?.Invoke(requestCode, permissions, grantResults);
      }

   }
}