using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Reading
{

   [QueryProperty("LibraryID", "libraryID")]
   [QueryProperty("FileID", "fileID")]
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ReadingPage : ContentPage
   {

      public ReadingPage()
      {
         InitializeComponent();
         BindingContext = new ReadingVM { IsBusy = true };
      }

      ReadingVM Context => BindingContext as ReadingVM;

      public string LibraryID
      {
         set => Context?.SetLibraryID(value);
      }

      public string FileID
      {
         set => Context?.SetFileID(value);
      }

      protected override void OnAppearing()
      {
         Context?.OnAppearing();
         base.OnAppearing();
      }

      protected override void OnDisappearing()
      {
         Context?.OnDisappearing();
         base.OnDisappearing();
      }

   }
}