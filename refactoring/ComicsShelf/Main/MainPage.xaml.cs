using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Main
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class MainPage : MasterDetailPage
   {
      public MainPage()
      {
         InitializeComponent();
         this.MasterBehavior = MasterBehavior.Popover;
      }
   }
}
