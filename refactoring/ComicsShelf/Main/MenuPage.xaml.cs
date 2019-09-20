using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Main
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class MenuPage : ContentPage
   {

      public MenuPage()
      {
         InitializeComponent();
         this.BindingContext = new MenuVM();
      }

   }
}