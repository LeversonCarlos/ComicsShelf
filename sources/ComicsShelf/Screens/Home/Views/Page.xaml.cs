using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Screens.Home
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class Page : ContentPage
   {

      public Page()
      {
         InitializeComponent();
         BindingContext = new HomeVM();
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         (BindingContext as HomeVM)?.OnAppearing();
      }

   }
}