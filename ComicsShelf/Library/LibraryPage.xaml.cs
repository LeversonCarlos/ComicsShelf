using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Library
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class LibraryPage : ContentPage
   {

      public LibraryPage()
      {
         InitializeComponent();
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         Helpers.AppCenter.TrackEvent("LibraryPage.OnAppearing");
      }

      protected override void OnDisappearing()
      {
         Helpers.AppCenter.TrackEvent("LibraryPage.OnDisappearing");
         base.OnDisappearing();
      }

   }
}