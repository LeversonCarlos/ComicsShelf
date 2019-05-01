using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Libraries
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class LibraryPage : ContentPage
   {
      public LibraryPage()
      {
         InitializeComponent();
      }

      private void DeleteLibrary_Clicked(object sender, System.EventArgs e)
      {
         (Shell.CurrentShell.BindingContext as ShellVM).DeleteLibraryCommand.Execute(Shell.CurrentShell.CurrentItem);
      }

      protected override void OnAppearing()
      {
         (this.BindingContext as LibraryVM).OnAppearing();
         base.OnAppearing();
      }

      protected override void OnDisappearing()
      {
         (this.BindingContext as LibraryVM).OnDisappearing();
         base.OnDisappearing();
      }

   }
}