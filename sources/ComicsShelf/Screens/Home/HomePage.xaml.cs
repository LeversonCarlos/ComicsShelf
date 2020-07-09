using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Screens.Home
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class HomePage : ContentPage
   {

      public HomePage()
      {
         InitializeComponent();
         BindingContext = new HomeVM();
      }

   }
}