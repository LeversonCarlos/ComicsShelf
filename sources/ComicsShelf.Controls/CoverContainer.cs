using System.Windows.Input;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverContainer : AbsoluteLayout
   {

      public CoverContainer()
      {
         WidthRequest = Helpers.Cover.DefaultWidth;
         HeightRequest = Helpers.Cover.DefaultHeight;

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
               await this.ScaleTo(0.85, 200, Easing.SpringIn);
               await this.ScaleTo(1.00, 100, Easing.SpringOut);
               OpenCommand?.Execute(this.BindingContext);
            });
      }

   }
}
