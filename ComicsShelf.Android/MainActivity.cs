using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Xamarin.CloudDrive.Connector.LocalDrive;
using Xamarin.CloudDrive.Connector.OneDrive;

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
         global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
         CarouselView.FormsPlugin.Android.CarouselViewRenderer.Init();
         this.AddLocalDriveConnector(savedInstanceState);
         this.AddOneDriveConnector(Resources.GetString(Resource.String.onedrive_applicationID), Resources.GetString(Resource.String.onedrive_scopeList));
         LoadApplication(new App());
      }

      public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
      {
         Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
         this.SetLocalDrivePermissionsResult(requestCode, permissions, grantResults);
         base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
      }

      protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
      {
         base.OnActivityResult(requestCode, resultCode, data);
         this.SetOneDriveAuthenticationResult(requestCode, resultCode, data);
      }

   }
}