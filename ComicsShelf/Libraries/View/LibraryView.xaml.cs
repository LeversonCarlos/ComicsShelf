using Xamarin.Forms.Xaml;

namespace ComicsShelf.Libraries
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