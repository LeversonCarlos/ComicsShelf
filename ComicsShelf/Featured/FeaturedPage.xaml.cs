using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Featured
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class FeaturedPage : ContentPage
   {

      public FeaturedPage()
      {
         InitializeComponent();
         this.BindingContext = new FeaturedVM();
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         Helpers.AppCenter.TrackEvent("FeaturedPage.OnAppearing");
      }

      protected override void OnDisappearing()
      {
         Helpers.AppCenter.TrackEvent("FeaturedPage.OnDisappearing");
         base.OnDisappearing();
      }

   }
}