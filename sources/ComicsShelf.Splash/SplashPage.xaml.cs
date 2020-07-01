using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Splash
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SplashPage : ContentPage
   {

      public SplashPage()
      {
         InitializeComponent();
         BindingContext = SplashExtentions.Instance;
      }

      SplashVM Context => BindingContext as SplashVM;

      protected override void OnAppearing()
      {
         Context?.OnAppearing();
         base.OnAppearing();
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