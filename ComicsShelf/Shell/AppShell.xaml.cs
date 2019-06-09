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
