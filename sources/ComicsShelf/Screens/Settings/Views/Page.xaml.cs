using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Screens.Settings
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class Page : ContentPage
   {
      public Page()
      {
         InitializeComponent();
         BindingContext = new SettingsVM();
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         (BindingContext as SettingsVM).OnAppearing();
      }

   }
}