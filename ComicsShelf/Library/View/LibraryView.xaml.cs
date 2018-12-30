using Xamarin.Forms.Xaml;

namespace ComicsShelf.Library
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class LibraryView : Helpers.Controls.BasePage
   {
      public LibraryView()
      {
         InitializeComponent();
      }
   }
}