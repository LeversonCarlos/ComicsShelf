using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Views.Home
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class HomeView : TabbedPage
   {

      public HomeView()
      {
         InitializeComponent();
         if (Device.RuntimePlatform == Device.iOS)
         { this.Padding = new Thickness(0, 20, 0, 0); }
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         if (this.BindingContext == null) { return; }
         this.SetBinding(Page.TitleProperty, "Title");
         (this.BindingContext as Helpers.BaseVM).InitializeAsync();
      }

      protected override void OnDisappearing()
      {
         base.OnDisappearing();
         if (this.BindingContext == null) { return; }
         (this.BindingContext as Helpers.BaseVM).FinalizeAsync();
      }

   }
}