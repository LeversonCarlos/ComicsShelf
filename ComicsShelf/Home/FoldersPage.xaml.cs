using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Home
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class FoldersPage : ContentPage
   {
      public FoldersPage()
      {
         InitializeComponent();
         this.Title = R.Strings.HOME_FOLDERS_PAGE_TITLE;
      }
   }
}