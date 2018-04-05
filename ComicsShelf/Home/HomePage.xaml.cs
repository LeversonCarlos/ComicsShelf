using Xamarin.Forms.Xaml;

namespace ComicsShelf.Home
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class HomePage : Helpers.Controls.Page
   {
      public HomePage()
      {
         InitializeComponent();
      }
   }
}