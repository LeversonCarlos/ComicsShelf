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

      protected override async void OnAppearing()
      {
         await (this.BindingContext as LibraryVM).OnAppearing();
         base.OnAppearing();
      }

      protected override async void OnDisappearing()
      {
         await(this.BindingContext as LibraryVM).OnDisappearing();
         base.OnDisappearing();
      }

   }
}