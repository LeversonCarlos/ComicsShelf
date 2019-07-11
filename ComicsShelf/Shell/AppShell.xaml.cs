using System.Linq;

namespace ComicsShelf
{
   public partial class AppShell : Xamarin.Forms.Shell
   {

      public AppShell()
      {
         InitializeComponent();
         this.BindingContext = new ShellVM();
         Xamarin.Forms.Routing.RegisterRoute("splash", typeof(ComicFiles.SplashView));
         Xamarin.Forms.Routing.RegisterRoute("reading", typeof(ComicFiles.ReadingView));
      }

      protected override bool OnBackButtonPressed()
      {

         var segments = this.CurrentState.Location
            .ToString()
            .Replace("//", "/")
            .Split(new string[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);
         if (segments.Contains("splash") || segments.Contains("splash"))
         { return base.OnBackButtonPressed(); }

         var route = segments[0];
         if (route == "home")
         { return base.OnBackButtonPressed(); }

         this.CurrentItem = this.Items[0];
         return true; // PREVENT THE BACK BUTON
      }

      private void NewLocalLibrary_Clicked(object sender, System.EventArgs e)
      {
         (this.BindingContext as ShellVM).NewLibraryCommand.Execute(Libraries.LibraryType.LocalDrive);
      }

      private void NewOneDriveLibrary_Clicked(object sender, System.EventArgs e)
      {
         (this.BindingContext as ShellVM).NewLibraryCommand.Execute(Libraries.LibraryType.OneDrive);
      }

   }
}
