using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Home
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