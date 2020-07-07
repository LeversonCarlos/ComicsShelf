using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Screens.Splash
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SplashPage : ContentPage
   {

      public SplashPage()
      {
         InitializeComponent();
         SplashExtentions.Instance.IsBusy = true;
         BindingContext = SplashExtentions.Instance;
      }

      SplashVM Context => BindingContext as SplashVM;

      protected override void OnAppearing()
      {
         base.OnAppearing();
         Context?.OnAppearing();
      }

      protected override void OnSizeAllocated(double width, double height)
      {
         base.OnSizeAllocated(width, height);
         Context?.OnSizeAllocated(width, height);
      }

      protected override void OnDisappearing()
      {
         Context?.OnDisappearing();
         base.OnDisappearing();
      }

   }
}