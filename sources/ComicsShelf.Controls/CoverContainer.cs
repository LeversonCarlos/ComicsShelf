using System.Windows.Input;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverContainer : AbsoluteLayout
   {

      public CoverContainer()
      {
         Margin = 0;
         Padding = new Thickness(0, 0, 10, 0);
         WidthRequest = CoverSize.DefaultWidth; ;
         HeightRequest = CoverSize.DefaultHeight;

         GestureRecognizers.Add(new TapGestureRecognizer
         {
            Command = OpenTransition,
            NumberOfTapsRequired = 1
         });

      }

      public static readonly BindableProperty OpenCommandProperty =
         BindableProperty.Create("OpenCommand", typeof(ICommand), typeof(CoverContainer), null);
      public ICommand OpenCommand
      {
         get { return (ICommand)GetValue(OpenCommandProperty); }
         set { SetValue(OpenCommandProperty, value); }
      }

      ICommand OpenTransition
      {
         get => new Command(async () =>
             {
                await this.ScaleTo(0.85, 150, Easing.SpringIn);
                await this.ScaleTo(1.00, 100, Easing.SpringOut);
                Device.BeginInvokeOnMainThread(() => this.OpenCommand?.Execute(this.BindingContext));
             });
      }

   }
}
