using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

namespace ComicsShelf.Droid
{
   [Activity(Label = "Comics Shelf", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
   public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
   {
      protected override void OnCreate(Bundle savedInstanceState)
      {
         TabLayoutResource = Resource.Layout.Tabbar;
         ToolbarResource = Resource.Layout.Toolbar;

         base.OnCreate(savedInstanceState);

         Xamarin.Forms.Forms.SetFlags("Visual_Experimental", "CollectionView_Experimental", "FastRenderers_Experimental");
         Xamarin.Essentials.Platform.Init(this, savedInstanceState);
         Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
         Xamarin.Forms.Forms.Init(this, savedInstanceState);
         CarouselView.FormsPlugin.Android.CarouselViewRenderer.Init();
         LoadApplication(new App());
      }

      public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
      {
         Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
         Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
         base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
      }

   }
}