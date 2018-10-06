using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class BasePage : ContentPage
   {

      public BasePage()
      {
         if (Device.RuntimePlatform == Device.iOS)
         { this.Padding = new Thickness(0, 20, 0, 0); }
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         if (this.BindingContext == null) { return; }
         this.SetBinding(BasePage.TitleProperty, "Title");
         (this.BindingContext as BaseVM).InitializeAsync();
      }

      protected override void OnDisappearing()
      {
         base.OnDisappearing();
         if (this.BindingContext == null) { return; }
         (this.BindingContext as BaseVM).FinalizeAsync();
      }

   }
}