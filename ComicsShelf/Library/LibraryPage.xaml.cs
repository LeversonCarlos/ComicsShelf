using Xamarin.Forms.Xaml;

namespace ComicsShelf.Library
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class LibraryPage : Helpers.Controls.Page
   {
      public LibraryPage()
      {
         InitializeComponent();
      }
   }
}