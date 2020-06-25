using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Splash
{

   [QueryProperty("LibraryID", "libraryID")]
   [QueryProperty("FileID", "fileID")]
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SplashPage : ContentPage
   {

      public SplashPage()
      {
         InitializeComponent();
         BindingContext = new SplashVM();
      }

      SplashVM Context => BindingContext as SplashVM;

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

      protected override void OnSizeAllocated(double width, double height)
      {
         base.OnSizeAllocated(width, height);
         Context?.OnSizeAllocated(width, height);
      }

      protected override void OnDisappearing()
      {
         Context?.OnDisappearing();
         base.OnDisappearing();
      }

   }
}