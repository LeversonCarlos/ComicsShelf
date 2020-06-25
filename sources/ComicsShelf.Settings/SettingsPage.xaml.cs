using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Settings
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SettingsPage : ContentPage
   {
      public SettingsPage()
      {
         InitializeComponent();
         BindingContext = new SettingsVM();
      }
   }
}