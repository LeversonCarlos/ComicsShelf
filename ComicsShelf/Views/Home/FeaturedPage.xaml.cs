using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Views.Home
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class FeaturedPage : ContentPage
   {
      public FeaturedPage()
      {
         InitializeComponent();
         this.Title = R.Strings.HOME_FEATURED_PAGE_TITLE;
      }
   }
}