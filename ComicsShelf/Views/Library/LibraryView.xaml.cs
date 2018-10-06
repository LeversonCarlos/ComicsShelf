using Xamarin.Forms.Xaml;

namespace ComicsShelf.Views.Library
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