using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Controls.Animations
{
   public class TapAnimation : TriggerAction<View>
   {

      protected override async void Invoke(View sender)
      {
         if (sender != null)
            await Animate(sender);
      }

      async Task Animate(View sender)
      {
         // ViewExtensions.CancelAnimations(sender);
         await sender.ScaleTo(0.75, 200, Easing.CubicOut);
         await sender.ScaleTo(1.00, 100, Easing.CubicIn);
      }

   }
}
