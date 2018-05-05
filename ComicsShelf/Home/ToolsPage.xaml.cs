using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Home
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ToolsPage : ContentPage
   {
      public ToolsPage()
      {
         InitializeComponent();
         this.Title = R.Strings.HOME_TOOLS_PAGE_TITLE;
      }
   }
}