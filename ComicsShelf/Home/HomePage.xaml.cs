using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Home
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class HomePage : TabbedPage
   {

      public HomePage()
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
         (this.BindingContext as Helpers.ViewModels.BaseVM).InitializeAsync();
      }

      protected override void OnDisappearing()
      {
         base.OnDisappearing();
         if (this.BindingContext == null) { return; }
         (this.BindingContext as Helpers.ViewModels.BaseVM).FinalizeAsync();
      }

   }
}