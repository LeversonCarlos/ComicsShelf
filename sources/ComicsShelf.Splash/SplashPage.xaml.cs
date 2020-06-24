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

      public string LibraryID
      {
         set => (BindingContext as SplashVM)
            ?.SetLibraryID(value)
            ?.LoadLibrary()
            ?.LoadItems();
      }

      public string FileID
      {
         set => (BindingContext as SplashVM)
            ?.SetFileID(value)
            ?.LoadLibrary()
            ?.LoadItems();
      }

   }
}