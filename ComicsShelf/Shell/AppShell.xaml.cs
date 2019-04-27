namespace ComicsShelf
{
   public partial class AppShell : Xamarin.Forms.Shell
   {

      public AppShell()
      {
         InitializeComponent();
         this.BindingContext = new ShellVM();
      }
      
      private void NewLocalLibrary_Clicked(object sender, System.EventArgs e)
      {
         (this.BindingContext as ShellVM).NewLibraryCommand.Execute(Libraries.LibraryType.FileSystem);
      }

   }
}
