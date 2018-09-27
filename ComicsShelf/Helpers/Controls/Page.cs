using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class Page : ContentPage
   {

      public Page()
      {
         if (Device.RuntimePlatform == Device.iOS)
         { this.Padding = new Thickness(0, 20, 0, 0); }
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         if (this.BindingContext == null) { return; }
         this.SetBinding(Page.TitleProperty, "Title");
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