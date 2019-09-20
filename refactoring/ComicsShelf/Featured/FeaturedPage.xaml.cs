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
   }
}