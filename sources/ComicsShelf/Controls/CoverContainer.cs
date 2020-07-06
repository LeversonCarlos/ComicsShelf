using System.Windows.Input;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverContainer : Grid
   {

      public CoverContainer()
      {
         RowDefinitions.Add(new RowDefinition { Height = 4 });
         RowDefinitions.Add(new RowDefinition { });
         RowDefinitions.Add(new RowDefinition { Height = 18 });
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
